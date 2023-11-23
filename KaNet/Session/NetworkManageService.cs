using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KaNet.Synchronizers;
using Steamworks;
using Steamworks.Data;
using UnityEngine;
using Utils;
using Utils.Service;

namespace KaNet.Session
{
	public enum GameMode
	{
		None = 0,
		Lobby,
		Ingame,
	}

	/// <summary>네트워크를 관리합니다.</summary>
	public class NetworkManageService : MonoService
	{
		private const int MAX_MTU = KaNetGlobal.DEFAULT_MTU;

		public event Action<Friend> InvitedFrom;
		public event Action<NetCallback> OnNetworkCallback;
		public event Action OnDisconnected;

		// Session Handler
		private SessionHandlerService mSessionHandler;
		private NetworkObjectManager mNetworkObjectManager;
		public NetworkObjectManager ObjectManager => mNetworkObjectManager;

		public bool IsSteamValid => mSessionHandler.IsSteamValid;
		public GameMode IsGameStart { get; set; }

		// DI Fields
		private AsyncSceneLoadService mAsyncSceneLoadService;

		public void OnSceneLoaded(SceneType sceneType, MonoObjectPoolService monoObjectPoolService)
		{
			switch (sceneType)
			{
				case SceneType.scn_game_hideout:
					mNetworkObjectManager.Start(monoObjectPoolService);
					break;

				case SceneType.scn_game_ingame:
					mNetworkObjectManager.Start(monoObjectPoolService);
					break;

				default:
					Ulog.Log(this, $"There is nothing to do when scene {sceneType} loaded!");
					break;
			}
		}

		public void Initialized
		(
			NetProgramID programID,
			AsyncSceneLoadService asyncSceneLoadService,
			Dictionary<NetObjectType, GameObject> networkObjectTable
		)
		{
			mAsyncSceneLoadService = asyncSceneLoadService;

			mSessionHandler = new SessionHandlerService(programID);
			mSessionHandler.OnRegistered();

			mNetworkObjectManager = new NetworkObjectManager(mSessionHandler, networkObjectTable);
			mSessionHandler.BindNetworkObjectManager(mNetworkObjectManager);
			mSessionHandler.OnDisconnected += onDisconnected;

			StartCoroutine(NetworkFixedUpdate());
			StartCoroutine(OnCheckHeartbeat());
			StartCoroutine(OnSendHeartbeat());
		}

		public override void OnUnregistered()
		{
			InvitedFrom = null;

			mSessionHandler.OnUnregistered();

			base.OnUnregistered();
		}

		#region Connection Operation

		/// <summary>현재 접속한 로비를 반환받습니다.</summary>
		/// <returns>접속한 상태가 아니라면 false를 반환합니다.</returns>
		public bool TryGetCurrentLobby(out Lobby currentLobby)
		{
			return mSessionHandler.TryGetCurrentLobby(out currentLobby);
		}

		/// <summary>로비 생성을 시도합니다.</summary>
		/// <param name="lobbySetting">로비 설정</param>
		/// <param name="guiCallback">완료 후 GUI Callback</param>
		public void TryCreateLobby(LobbySetting lobbySetting)
		{
			var t = mSessionHandler.TryStartViaSteamAsync(lobbySetting, (callback) =>
			{
				mThreadUnwinder.Enqueue(() =>
				{
					OnNetworkCallback?.Invoke(callback);
				});

				if (callback.Result != NetOperationResult.Success)
				{
					return;
				}

				// 씬을 전환합니다.
				// 씬 전환 후 scn_game_ingame 씬에서의 관련 Service가 NetworkManageService를 통해
				// 게임을 초기화합니다.
				mThreadUnwinder.Enqueue(() =>
				{
					mAsyncSceneLoadService.TryLoadSceneAsync(SceneType.scn_game_ingame);
				});
			});
		}

		/// <summary>지정한 EndPoint로 접속을 시도합니다.</summary>
		/// <param name="endPoint">Steam Lobby에 접속하는 경우 Lobby정보가 담겨있습니다.</param>
		/// <param name="request">최초 접속시 현재 세션의 정보입니다.</param>
		public void TryConnectTo(EndPointInfo endPoint, NetConnectRequestInfo request)
		{
			var t = mSessionHandler.TryConnectTo(endPoint, request, (callback) =>
			{
				mThreadUnwinder.Enqueue(() =>
				{
					OnNetworkCallback?.Invoke(callback);
				});

				if (callback.Result != NetOperationResult.Success)
				{
					return;
				}

				// 씬을 전환합니다.
				// 씬 전환 후 scn_game_ingame 씬에서의 관련 Service가 NetworkManageService를 통해
				// 게임을 초기화합니다.
				mThreadUnwinder.Enqueue(() => 
				{
					mAsyncSceneLoadService.TryLoadSceneAsync(SceneType.scn_game_ingame); 
				});
			});
		}

		/// <summary>접속을 종료합니다.</summary>
		public void Disconnect()
		{
			mThreadUnwinder.Enqueue(() =>
			{
				if (mSessionHandler.SessionState == SessionState.Connected)
				{
					mSessionHandler.Disconnect(null);
					return;
				}

				Ulog.Log(this, $"You cannot disconnect while {mSessionHandler.SessionState}");
			});
		}

		/// <summary>접속이 종료되었을 때 호출됩니다. Disconnect호출 뒤에도 호출됩니다.</summary>
		private void onDisconnected()
		{
			mThreadUnwinder.Enqueue(() =>
			{
				OnDisconnected?.Invoke();
				ReturnToTitle();
			});
		}

		#endregion

		/// <summary>친구 초대를 시도합니다.</summary>
		/// <param name="friendInfo">초대할 친구의 정보</param>
		/// <param name="guiCallback">완료 후 GUI Callback</param>
		public void TryInviteFriend(NetSessionInfo friendInfo)
		{
			// Check if you are in the lobby
			mSessionHandler.InviteFriend(friendInfo, (callback) =>
			{
				mThreadUnwinder.Enqueue(() =>
				{
					OnNetworkCallback?.Invoke(callback);
				});
			});
		}

		public void JoinToHideout()
		{
			mThreadUnwinder.Enqueue(() =>
			{
				mSessionHandler.StartHideout();
				mAsyncSceneLoadService.TryLoadSceneAsync(SceneType.scn_game_hideout);
			});
		}

		/// <summary>타이틀로 돌아갑니다. 반드시 접속 종료 후 호출되어야 합니다.</summary>
		private void ReturnToTitle()
		{
			mSessionHandler.StartHideout();

			mThreadUnwinder.Enqueue(() =>
			{
				mAsyncSceneLoadService.TryLoadSceneAsync(SceneType.scn_game_title);
			});
		}

		public void RequestLobbyList(Action<Lobby[]> requestCallback)
		{
			mSessionHandler.RequestLobbyList(requestCallback);
		}

		public bool TryGetFriendList(out IEnumerable<Friend> friends)
		{
			return mSessionHandler.TryGetFriendList(out friends);
		}

		private Queue<Action> mThreadUnwinder = new();

		public void Update()
		{
			while (mThreadUnwinder.Count > 0)
			{
				mThreadUnwinder.Dequeue().Invoke();
			}

			mSessionHandler?.OnTick();
			mNetworkObjectManager?.OnUpdate();
		}

		public void FixedUpdate()
		{
			mNetworkObjectManager?.OnFixedUpdate();
		}

		public IEnumerator NetworkFixedUpdate()
		{
			float fixedTime = KaNetGlobal.NETWORK_TICK_INTERVAL_SEC;

			while (true)
			{
				yield return new WaitForSecondsRealtime(fixedTime);
				mNetworkObjectManager?.OnNetworkTickUpdate();
			}
		}

		public IEnumerator OnCheckHeartbeat()
		{
			float fixedTime = KaNetGlobal.HEARTBEAT_CHECK_INTERVAL_SEC;

			while (true)
			{
				yield return new WaitForSecondsRealtime(fixedTime);
				mSessionHandler?.OnCheckHeartbeat();
			}
		}

		public IEnumerator OnSendHeartbeat()
		{
			float fixedTime = KaNetGlobal.HEARTBEAT_SEND_INTERVAL_SEC;

			while (true)
			{
				yield return new WaitForSecondsRealtime(fixedTime);
				mSessionHandler?.OnSendHeartbeat();
			}
		}
	}
}
