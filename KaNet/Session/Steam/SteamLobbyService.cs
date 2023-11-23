using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KaNet.SteamworksAPI;
using Steamworks;
using Steamworks.Data;
using Utils;
using Utils.Service;

namespace KaNet.Session
{
	/// <summary>스팀 로비를 컨트롤 하는 클래스입니다.</summary>
	public class SteamLobbyService : IServiceable
	{
		/// <summary>로비가 특정 콜백을 받았을 때의 이벤트입니다.</summary>
		public event Action<NetCallback> OnLobbyOperationCallback;
		public event Action OnJoinLobby;
		public event Action OnLeaveLobby;

		private Lobby? mCurrentLobby;

		private const int MAX_LOBBY_REQUEST = 50;

		public void OnRegistered()
		{
			// Lobby operation
			SteamMatchmaking.OnLobbyCreated += onLobbyCreated;
			SteamMatchmaking.OnLobbyEntered += onLobbyEntered;
			SteamMatchmaking.OnLobbyGameCreated += onLobbyGameCreated; // 아마도 데디케이트 서버 관련일듯
			SteamMatchmaking.OnLobbyInvite += onLobbyInvite;

			// Member operation
			SteamMatchmaking.OnLobbyMemberDataChanged += onLobbyMemberDataChanged;
			SteamMatchmaking.OnLobbyMemberJoined += onLobbyMemberJoined;
			SteamMatchmaking.OnLobbyMemberDisconnected += onLobbyMemberDisconnected;
			SteamMatchmaking.OnLobbyMemberLeave += onLobbyMemberLeave;

			//SteamMatchmaking.OnLobbyMemberKicked += onLobbyMemberKicked;
			//SteamMatchmaking.OnLobbyMemberBanned += onLobbyMemberBanned;

			// Friend operation
			SteamFriends.OnGameLobbyJoinRequested += onGameLobbyJoinRequested;

		}

		public void OnUnregistered()
		{
			// Lobby operation
			SteamMatchmaking.OnLobbyCreated -= onLobbyCreated;
			SteamMatchmaking.OnLobbyEntered -= onLobbyEntered;
			SteamMatchmaking.OnLobbyGameCreated -= onLobbyGameCreated;
			SteamMatchmaking.OnLobbyInvite -= onLobbyInvite;

			// Member operation
			SteamMatchmaking.OnLobbyMemberDataChanged -= onLobbyMemberDataChanged;
			SteamMatchmaking.OnLobbyMemberJoined -= onLobbyMemberJoined;
			SteamMatchmaking.OnLobbyMemberDisconnected -= onLobbyMemberDisconnected;
			SteamMatchmaking.OnLobbyMemberLeave -= onLobbyMemberLeave;

			//SteamMatchmaking.OnLobbyMemberKicked -= onLobbyMemberKicked;
			//SteamMatchmaking.OnLobbyMemberBanned -= onLobbyMemberBanned;

			// Friend operation
			SteamFriends.OnGameLobbyJoinRequested -= onGameLobbyJoinRequested;
		}

		private void onCallback(NetCallback callback)
		{
			OnLobbyOperationCallback?.Invoke(callback);
		}

		#region Backend Operations

		public async Task RequestLobbyList(Action<Lobby[]> requestLobbyCallbacks)
		{
			var lobbyQuery = SteamMatchmaking.LobbyList;
			lobbyQuery.WithMaxResults(MAX_LOBBY_REQUEST);
			lobbyQuery.FilterDistanceClose();
			var lobbys = await lobbyQuery.RequestAsync();
			requestLobbyCallbacks?.Invoke(lobbys);
		}

		#endregion

		#region Operations

		/// <summary>현재 접속한 로비를 반환받습니다.</summary>
		/// <returns>접속한 상태가 아니라면 false를 반환합니다.</returns>
		public bool TryGetCurrentLobby(out Lobby currentLobby)
		{
			if (mCurrentLobby.HasValue)
			{
				currentLobby = mCurrentLobby.Value;
				return true;
			}

			currentLobby = new Lobby();
			return false;
		}

		/// <summary>로비로 접속을 시도합니다.</summary>
		/// <param name="targetLobby">대상 로비입니다.</param>
		/// <returns>로비 접속 시도 결과입니다.</returns>
		public async Task<bool> TryJoinToLobbyAsync(Lobby targetLobby)
		{
			var joinResult = await targetLobby.Join();
			return joinResult == RoomEnter.Success;
		}

		/// <summary>로비 생성을 시도합니다.</summary>
		/// <param name="lobbySetting">로비 설정입니다.</param>
		/// <returns>로비 생성 시도 결과입니다.</returns>
		public async Task TryCreateLobbyAync(LobbySetting lobbySetting, Action<NetCallback> lobbyCallback)
		{
			var callback = new NetCallback(NetOperationType.CreateLobby);
			mCurrentLobby = null;

			try
			{
				var operationResult = await SteamMatchmaking.CreateLobbyAsync(lobbySetting.MaxPlayer);

				if (!operationResult.HasValue)
				{
					callback.Result = NetOperationResult.Failed;
					lobbyCallback?.Invoke(callback);
					return;
				}

				operationResult.Value.Setup(lobbySetting);
				
				callback.Result = NetOperationResult.Success;
				lobbyCallback?.Invoke(callback);
				return;
			}
			catch (Exception e)
			{
				callback.Result = NetOperationResult.Error;
				callback.AddArgument(e);
				lobbyCallback?.Invoke(callback);
				return;
			}
		}

		/// <summary>로비 나가기를 시도합니다.</summary>
		public void TryLeaveLobby(Action<NetCallback> lobbyCallback)
		{
			var callback = new NetCallback(NetOperationType.TryDisconnect);

			try
			{
				mCurrentLobby.Value.Leave();
				mCurrentLobby = null;

				callback.Result = NetOperationResult.Success;
				lobbyCallback?.Invoke(callback);

				return;
			}
			catch (Exception e)
			{
				callback.Result = NetOperationResult.Error;
				callback.AddArgument(e);
			}

			lobbyCallback?.Invoke(callback);
			return;
		}

		/// <summary>친구 초대를 시도합니다.</summary>
		/// <param name="friendInfo">초대할 친구의 Steam ID입니다.</param>
		/// <returns>친구 초대 시도 결과입니다.</returns>
		public void TryInviteFriend(NetSessionInfo friendInfo, Action<NetCallback> lobbyCallback)
		{
			var callback = new NetCallback(NetOperationType.InviteFriend);

			if (mCurrentLobby.Value.InviteFriend(friendInfo.SteamID))
			{
				callback.Result = NetOperationResult.Success;
				callback.AddArgument(friendInfo);
			}
			else
			{
				callback.Result = NetOperationResult.Failed;
			}

			lobbyCallback?.Invoke(callback);
		}

		#endregion

		#region Steam Lobby Callbacks

		/// <summary>클라이언트가 로비를 생성했을 때 호출됩니다.</summary>
		/// <param name="result">결과</param>
		/// <param name="lobby">로비 데이터</param>
		private void onLobbyCreated(Result result, Lobby lobby)
		{
			// 단순 출력
			Ulog.Log(UlogType.Lobby, $"You create the lobby [result : {result}][lobby : {lobby.Id}]");
		}

		/// <summary>로비에 입장되었을 때 호출됩니다. 만약 클라이언트가 로비를 생성하면 즉시 입장됩니다.</summary>
		/// <param name="lobby">로비 데이터</param>
		private void onLobbyEntered(Lobby lobby)
		{
			mCurrentLobby = lobby;

			var callback = new NetCallback(NetOperationType.OnLobbyJoined);
			callback.Result = NetOperationResult.Success;
			onCallback(callback);
		}

		// 아마도 데디케이트 관련일듯
		private void onLobbyGameCreated(Lobby lobby, uint ip, ushort port, SteamId id)
		{
			Ulog.Log(UlogType.Lobby, $"OnLobbyGameCreated [ip : {ip}][port : {port}][SteamID : {id}]");
		}

		/// <summary>누군가 로비로 초대했을 때 호출됩니다.</summary>
		/// <param name="fromFriend">초대한 사람</param>
		/// <param name="targetLobby">초대한 사람의 로비 정보</param>
		private void onLobbyInvite(Friend fromFriend, Lobby targetLobby)
		{
			//var callback = new NetCallback(NetOperationType.OnInvited);

			//callback.Result = IsClientEntered ? NetOperationResult.AlreadyInTheGame : NetOperationResult.Success;
			//callback.AddArgument(targetLobby);
			//callback.AddArgument(fromFriend);
			//onCallback(callback);
		}

		#endregion

		#region Steam Member Callbacks

		private void onLobbyMemberDataChanged(Lobby lobby, Friend userData)
		{
			// 뭔지 정확히 모르겠음
			Ulog.Log(UlogType.Lobby, $"onLobbyMemberDataChanged User : {userData}");
		}

		/// <summary>유저가 현재 로비로 접속했을 때 호출됩니다.</summary>
		/// <param name="lobby">유저가 접속하려는 로비</param>
		/// <param name="friend">접속하려는 유저</param>
		private void onLobbyMemberJoined(Lobby lobby, Friend friend)
		{
			//var callback = new NetCallback(NetOperationType.OnMemberJoined);

			//callback.Result = IsClientEntered ? NetOperationResult.Success : NetOperationResult.NotInTheLobby;
			//callback.AddArgument(friend);
			//onCallback(callback);
		}

		/// <summary>유저가 로비를 떠났을 때 호출됩니다.</summary>
		/// <param name="lobby">유저가 떠난 로비</param>
		/// <param name="friend">떠나려는 유저</param>
		private void onLobbyMemberLeave(Lobby lobby, Friend friend)
		{
			//if (friend.IsMe)
			//{
			//	onLeaveLobby();
			//}
			//else
			//{
			//	var callback = new NetCallback(NetOperationType.OnDisconnected);

			//	callback.Result = IsClientEntered ? NetOperationResult.Success : NetOperationResult.NotInTheLobby;
			//	callback.AddArgument(friend);
			//	onCallback(callback);
			//}
		}

		/// <summary>유저가 로비로 부터 연결이 끊겼을 때 호출됩니다.</summary>
		/// <param name="lobby">유저가 접속 끊긴 로비</param>
		/// <param name="friend">접속이 끊긴 유저</param>
		private void onLobbyMemberDisconnected(Lobby lobby, Friend friend)
		{
			//if (friend.IsMe)
			//{
			//	onLeaveLobby();
			//}
			//else
			//{
			//	var callback = new NetCallback(NetOperationType.OnDisconnected);

			//	callback.Result = IsClientEntered ? NetOperationResult.Success : NetOperationResult.NotInTheLobby;
			//	callback.AddArgument(friend);
			//	onCallback(callback);
			//}
		}

		//private void onLobbyMemberKicked(Lobby lobby, Friend kickedUser, Friend userWhoKick)
		//{
		//	if (kickedUser.IsMe)
		//	{
		//		TryLeaveLobby();
		//	}
		//	// 채팅창에 출력
		//}

		//private void onLobbyMemberBanned(Lobby lobby, Friend bannedUser, Friend userWhoBanned)
		//{
		//	// 채팅창에 출력
		//}

		#endregion

		/// <summary>클라이언트가 스팀 친구 목록에서 친구의 방에 참가하기를 했을 때 호출됩니다.</summary>
		/// <param name="targetLobby">접속할 로비</param>
		/// <param name="targetFriendID">선택한 친구</param>
		private void onGameLobbyJoinRequested(Lobby targetLobby, SteamId targetFriendID)
		{
			//var callback = new NetCallback(NetOperationType.OnJoinRequest);

			//callback.Result = IsClientEntered ? NetOperationResult.AlreadyInTheGame : NetOperationResult.Success;
			//callback.AddArgument(targetLobby);
			//callback.AddArgument(targetFriendID);
			//onCallback(callback);
		}
	}
}
