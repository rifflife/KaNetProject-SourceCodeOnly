using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KaNet.Core;
using KaNet.Synchronizers;
using KaNet.Utils;
using Steamworks;
using Steamworks.Data;
using UnityEngine.UIElements;
using Utils;
using Utils.Service;

namespace KaNet.Session
{
	public enum SessionState
	{
		None,

		Ready,

		Creating,

		Connecting,
		Connected,

		Disconnecting,
		Disconnected,
	}

	/// <summary>네트워크 세션을 관리합니다.</summary>
	public class SessionHandlerService : IServiceable
	{
		public event Action<NetSessionInfo> OnReadySessionConnected;
		public event Action<NetSessionInfo> OnSessionDisconnected;
		public event Action<NetSessionInfo> OnNewSessionConnected;
		public event Action OnSessionConnectionChanged;
		public event Action OnDisconnected;

		// DI Field
		private NetProgramID mProgramID;
		private NetworkObjectManager mNetworkObjectManager;
		public SteamId ClientSessionSteamID => mSteamService.CurrentUserID;

		// Services
		private SteamService mSteamService;
		private SteamLobbyService mLobbyService;
		private SteamNetworkTransporter mTransporter;

		// Session Info
		private BidirectionalMap<SteamId, NetSessionInfo> mSessionBySteamID = new();
		private BidirectionalMap<NetSessionID, NetSessionInfo> mSessionByNetID = new();
		public List<NetSessionInfo> SessionList => mSessionByNetID.ForwardValues.ToList();
		private NetSessionID mSessionIdCounter = 0;
		public NetSessionID ClientSessionID { get; private set; }
		private LobbySetting mLobbySetting;
		private EndPointInfo? mServerEndPoint;
		public NetSessionInfo ClientSessionInfo { get; private set; }
		public NetSessionInfo ServerSessionInfo { get; private set; }
		private NetConnectRequestInfo mConnectRequestInfo;

		// Network
		public NetworkMode NetworkMode { get; private set; }
		//public GameState CurrnetGameState { get; private set; }
		private Stopwatch mNetworkTimer = new Stopwatch();
		public NetTimestamp CurrentTimestamp => new NetTimestamp(mNetworkTimer);

		// State Flag
		public SessionState SessionState { get; private set; } = SessionState.Ready;
		private NetOperationResult mAckCallbackResult;
		private bool mShouldCheckHeartbeat = true;

		// Getters
		public int SessionCount => mSessionBySteamID.Count;
		public bool IsSteamValid => mSteamService.IsValid;

		// For Networking
		private const int CONNECTING_CHECK_INTERVAL = 500;

		public SessionHandlerService(NetProgramID programID)
		{
			// Set ProgramID
			mProgramID = programID;

			// Services
			mSteamService = new SteamService(mProgramID.AppID);
			mLobbyService = new SteamLobbyService();
			mTransporter = new SteamNetworkTransporter();
			mTransporter.OnPacketReceived += onPacketReceived;
			mTransporter.OnP2PDisconnected += onP2PDisconnected;

			// Initialize
			SessionState = SessionState.Ready;
		}

		public void BindNetworkObjectManager(NetworkObjectManager manager)
		{
			mNetworkObjectManager = manager;
		}

		public void OnRegistered()
		{
			mSteamService.OnRegistered();
			if (IsSteamValid)
			{
				mLobbyService.OnRegistered();
				mTransporter.OnRegistered();
			}
		}

		public void OnUnregistered()
		{
			if (IsSteamValid)
			{
				mTransporter.OnUnregistered();
				mLobbyService.OnUnregistered();
			}
			mSteamService.OnUnregistered();
		}

		private NetSessionID getNewSessionID()
		{
			for (int i = 0; i < byte.MaxValue; i++)
			{
				if (mSessionByNetID.Contains(mSessionIdCounter))
				{
					mSessionIdCounter++;
					continue;
				}

				break;
			}

			return mSessionIdCounter;
		}

		#region Network Events

		public void OnTick()
		{
			if (this.IsSteamValid)
			{
				mTransporter.OnTick();
			}
		}

		public void OnCheckHeartbeat()
		{
			Ulog.Log(this, $"Current session count : {mSessionByNetID.Count}");

			if (SessionState != SessionState.Connected)
			{
				return;
			}

			foreach (var sessionInfo in mSessionBySteamID.ForwardValues.ToList())
			{
				if (!sessionInfo.CheckHeartbeat(CurrentTimestamp, !mShouldCheckHeartbeat))
				{
					Ulog.Log(this, $"Session Timeout : {sessionInfo}");
					DisconnectSession(sessionInfo.SteamID);
				}
			}
		}

		public void onHeartbeatReceived(SteamId from)
		{
			if (this.mSessionBySteamID.TryGetValue(from, out var session))
			{
				session.SetLastHeartbeat(CurrentTimestamp);
			}
		}

		public void OnSendHeartbeat()
		{
			if (NetworkMode.IsServer())
			{
				foreach (var id in mSessionBySteamID.ReverseValues)
				{
					sendPacketByHeaderType(id, PacketHeaderType.REQ_HEARTBEAT);
				}
			}
			if (NetworkMode.IsClient())
			{
				if (ServerSessionInfo != null)
				{
					sendPacketByHeaderType(ServerSessionInfo.SteamID, PacketHeaderType.REQ_HEARTBEAT);
				}
			}
		}

		private void onP2PDisconnected(SteamId p2pSession)
		{
			if (mSessionBySteamID.TryGetValue(p2pSession, out var session))
			{
				mSessionBySteamID.TryRemove(p2pSession);
				mSessionByNetID.TryRemove(session.ID);

				Ulog.Log(this, $"Session disconnected! {session}");
				OnSessionDisconnected?.Invoke(session);
				OnSessionConnectionChanged?.Invoke();

				if (ServerSessionInfo.SteamID == p2pSession)
				{
					mAckCallbackResult = NetOperationResult.ServerDisconnected;
					Disconnect(null);
				}
			}
		}

		/// <summary>패킷을 수신받았을 때 이벤트입니다.</summary>
		/// <param name="from">송신자 입니다.</param>
		/// <param name="packet">패킷 입니다.</param>
		private void onPacketReceived(SteamId from, byte[] buffer, uint size)
		{
			NetPacket receivedPacket = PacketPool.GetStreamPacket();
			receivedPacket.GetWriter().WriteRawData(buffer, 0, (int)size);

			var reader = receivedPacket.GetReader();
			int packetIndex = 0;

			NetPacket partialPacket = null;

			// Parse packet
			try
			{
				partialPacket = PacketPool.GetStreamPacket();
				while (reader.CanRead(NetBaseHeader.HEADER_SIZE))
				{
					NetBaseHeader header = new NetBaseHeader(reader);

					partialPacket
						.GetWriter()
						.WritePacket(receivedPacket, packetIndex, header.PacketLength);

					// Set packet reader index
					packetIndex += header.PacketLength;
					reader.SetIndex(packetIndex);

					// Set partial packet
					var partialReader = partialPacket.GetReader();
					partialReader.BindBaseHeader(header);
					partialReader.SetIndex(NetBaseHeader.HEADER_SIZE);
					parsePacket(from, header.PacketHeader, partialReader);
				}
			}
			catch (RequestConnectError e)
			{
				Ulog.LogWarning(this, $"Critical packet parse error from : {from}");
				Ulog.LogError(this, e);
				this.DisconnectSession(from);
			}
			catch (RequestDisconnectError e)
			{
				Ulog.LogWarning(this, $"Critical packet parse error from : {from}");
				Ulog.LogError(this, e);
				this.DisconnectSession(from);
			}
			catch (RequestHeartbeatError e)
			{
				Ulog.LogWarning(this, $"Critical packet parse error from : {from}");
				Ulog.LogError(this, e);
				this.DisconnectSession(from);
			}
			catch (RequestObjectSynchronizeError e)
			{
				Ulog.LogWarning(this, $"Critical packet parse error from : {from}");
				Ulog.LogError(this, e);
				this.DisconnectSession(from);
			}
			catch (SyncIndexError e)
			{
				Ulog.LogWarning(this, $"Critical packet parse error from : {from}");
				Ulog.LogError(this, e);
				this.DisconnectSession(from);
			}
			catch (SyncDeserializeFieldError e)
			{
				Ulog.LogWarning(this, $"Critical packet parse error from : {from}");
				Ulog.LogError(this, e);
				this.DisconnectSession(from);
			}
			catch (SyncIgnoreDeserializeFieldError e)
			{
				Ulog.LogWarning(this, $"Critical packet parse error from : {from}");
				Ulog.LogError(this, e);
				this.DisconnectSession(from);
			}
			catch (SyncIgnoreDeserializeRpcError e)
			{
				Ulog.LogWarning(this, $"Critical packet parse error from : {from}");
				Ulog.LogError(this, e);
				this.DisconnectSession(from);
			}
			catch (SyncCountParseError e)
			{
				Ulog.LogWarning(this, $"Critical packet parse error from : {from}");
				Ulog.LogError(this, e);
				this.DisconnectSession(from);
			}
			finally
			{
				PacketPool.ReturnStreamPacket(partialPacket);
			}

			PacketPool.ReturnStreamPacket(receivedPacket);

			void parsePacket(SteamId from, PacketHeaderType headerType, NetPacketReader reader)
			{
				// Find from session
				if (mSessionBySteamID.TryGetValue(from, out NetSessionInfo sessionInfo))
				{
					bool isServer = ServerSessionInfo == sessionInfo;

					switch (headerType)
					{
						case PacketHeaderType.REQ_CONNECT:
							try
							{
								sendConnectAckCallback(from, NetOperationResult.ServerCannotHandle);
								this.DisconnectSession(from);
							}
							catch (Exception e)
							{
								throw new RequestConnectError(e);
							}
							return;

						case PacketHeaderType.REQ_DISCONNECT:
							try
							{
								DisconnectSession(from);
							}
							catch (Exception e)
							{
								throw new RequestDisconnectError(e);
							}
							return;

						case PacketHeaderType.REQ_HEARTBEAT:
							try
							{
								onHeartbeatReceived(from);
							}
							catch (Exception e)
							{
								throw new RequestHeartbeatError(e);
							}
							return;

						case PacketHeaderType.REQ_OBJ_SYN:
							if (isServer)
							{
								Ulog.LogWarning(this, $"You cannot send initial data to server!");
								return;
							}

							try
							{
								mNetworkObjectManager.SendInitialDataTo(sessionInfo);
								sessionInfo.SetIsReady();
								OnReadySessionConnected?.Invoke(sessionInfo);
								OnSessionConnectionChanged?.Invoke();
							}
							catch (Exception e)
							{
								throw new RequestObjectSynchronizeError(e);
							}
							return;

						case PacketHeaderType.SYN_OBJ_LIFE:
							mNetworkObjectManager.OnSyncObjectLife(reader, isServer, sessionInfo.ID);
							return;

						case PacketHeaderType.SYN_OBJ_FIELD:
							mNetworkObjectManager.OnSyncObjectField(reader, isServer, sessionInfo.ID);
							return;

						case PacketHeaderType.SYN_OBJ_RPC:
							mNetworkObjectManager.OnSyncObjectRPCs(reader, isServer, sessionInfo.ID);
							return;
					}
				}
				// From Outside Unknown User
				else
				{
					switch (headerType)
					{
						case PacketHeaderType.REQ_CONNECT:
							onRequestConnect(from, reader);
							return;

						case PacketHeaderType.ACK_CALLBACK:
							onAcknowledgedCallback(from, reader);
							return;
					}
				}

				Ulog.LogWarning(this, $"Cannot handle packet from : {from}, type : {headerType}");
				this.DisconnectSession(from);
				return;
			}
		}

		/// <summary>연결 요청이 들어왔을 때 이벤트입니다. 요청을 수락하거나 거절합니다.</summary>
		public void onRequestConnect(SteamId from, NetPacketReader reader)
		{
			if (!this.NetworkMode.IsServer())
			{
				Ulog.LogWarning(this, $"You cannont handle connection request. You are not server!");
				this.DisconnectSession(from);
				return;
			}

			NetConnectRequestInfo request = new NetConnectRequestInfo();
			try
			{
				request.DeserializeFrom(reader);
			}
			catch
			{
				Ulog.LogWarning(this, $"Cannot handle connect request from : {from}");
				this.DisconnectSession(from);
				return;
			}

			if (request.ProgramID != mConnectRequestInfo.ProgramID)
			{
				sendConnectAckCallback(from, NetOperationResult.WrongVersion);
				this.DisconnectSession(from);
				return;
			}

			if (request.Password != mConnectRequestInfo.Password)
			{
				sendConnectAckCallback(from, NetOperationResult.WrongPassword);
				this.DisconnectSession(from);
				return;
			}

			if (SessionCount > mLobbySetting.MaxPlayer)
			{
				sendConnectAckCallback(from, NetOperationResult.ServerIsFull);
				this.DisconnectSession(from);
				return;
			}

			if (mSessionBySteamID.Contains(from))
			{
				Ulog.LogWarning(this, $"Connection request from : {from}. But the session was already connected!");
				return;
			}

			var newSessionID = onSessionConnected(from);
			sendConnectAckCallback(from, NetOperationResult.Success, newSessionID);
		}

		/// Send ACK_CALLBACK to client
		private void sendConnectAckCallback
		(
			SteamId to,
			NetOperationResult operationResult,
			NetSessionID? requesterSessionID = null
		)
		{
			// Initialize packet
			NetPacket ackPacket = new NetPacket(200);
			var writer = ackPacket.GetWriter();
			writer.OffsetWriteIndex(NetBaseHeader.HEADER_SIZE);

			// Create callback
			NetCallback netCallback = new(NetOperationType.TryConnect, operationResult);
			netCallback.SerializeTo(writer);
			if (requesterSessionID.HasValue)
			{
				requesterSessionID.Value.SerializeTo(writer);
			}

			// Write header
			NetBaseHeader ackHeader = new NetBaseHeader();
			ackHeader.PacketHeader = PacketHeaderType.ACK_CALLBACK;
			ackHeader.SenderID = this.ClientSessionID;
			ackHeader.PacketLength = (ushort)writer.WriteIndex;
			ackHeader.Timestamp = new NetTimestamp(mNetworkTimer);
			writer.WriteAt(ackHeader, 0);

			mTransporter.SendToViaReliable(to, ackPacket);
		}

		/// <summary>패킷 헤더만 전송합니다.</summary>
		/// <param name="to">전송할 대상</param>
		/// <param name="headerType">패킷 헤더</param>
		private void sendPacketByHeaderType(SteamId to, PacketHeaderType headerType)
		{
			// Write header
			NetBaseHeader header = new NetBaseHeader();
			header.PacketHeader = headerType;
			header.SenderID = this.ClientSessionID;
			header.PacketLength = NetBaseHeader.HEADER_SIZE;
			header.Timestamp = new NetTimestamp(mNetworkTimer);

			// Serialize packet
			NetPacket ackPacket = new NetPacket(200);
			var writer = ackPacket.GetWriter();
			header.SerializeTo(writer);

			mTransporter.SendToViaReliable(to, ackPacket);
		}

		/// <summary>네트워크 콜백이 들어왔을 때 이벤트입니다.</summary>
		public void onAcknowledgedCallback(SteamId from, NetPacketReader reader)
		{
			if (!this.NetworkMode.IsClient())
			{
				Ulog.LogWarning(this, $"You cannont handle acknowledged callback. You are not clinet!");
			}

			NetCallback netCallback = new();
			netCallback.DeserializeFrom(reader);

			switch (netCallback.Operation)
			{
				case NetOperationType.TryConnect:

					if (SessionState == SessionState.Connecting)
					{
						// Connect failed!
						if (netCallback.Result != NetOperationResult.Success)
						{
							SessionState = SessionState.Ready;
							mAckCallbackResult = netCallback.Result;
							return;
						}

						// Connect success!
						NetSessionID mySessionID = new NetSessionID(reader);
						ClientSessionID = mySessionID;
						var clientSessionInfo = new NetSessionInfo
						(
							ClientSessionID,
							ClientSessionSteamID,
							CurrentTimestamp
						);

						// Set client session
						ClientSessionInfo = clientSessionInfo;
						mSessionByNetID.Add(ClientSessionID, ClientSessionInfo);
						mSessionBySteamID.Add(ClientSessionSteamID, ClientSessionInfo);

						// Set server session
						var serverInfo = new NetSessionInfo(0, from, CurrentTimestamp);
						mSessionBySteamID.Add(serverInfo.SteamID, serverInfo);
						mSessionByNetID.Add(serverInfo.ID, serverInfo);
						ServerSessionInfo = serverInfo;

						OnReadySessionConnected?.Invoke(serverInfo);
						OnSessionConnectionChanged?.Invoke();

						// Start client session
						StartClient();
						return;
					}

					break;

				default:
					Ulog.LogWarning(this, $"Ack Callback handle error : {netCallback.Operation}");
					break;
			}
		}

		#endregion

		#region Connection

		public NetSessionID onSessionConnected(SteamId from)
		{
			var currentSession = new NetSessionInfo(getNewSessionID(), from, CurrentTimestamp);
			mSessionBySteamID.Add(currentSession.SteamID, currentSession);
			mSessionByNetID.Add(currentSession.ID, currentSession);

			// Call callback
			OnNewSessionConnected?.Invoke(currentSession);
			OnSessionConnectionChanged?.Invoke();
			Ulog.Log(this, $"Session connected : {currentSession}");
			return currentSession.ID;
		}

		/// <summary>해당 세션을 접속종료합니다.</summary>
		public void DisconnectSession(SteamId steamId)
		{
			if (NetworkMode.IsServer())
			{
				// 해당 세션을 접속종료합니다.
				// 접속 종료 후 onP2PDisconnected가 호출됩니다.
				Ulog.Log(this, $"Session disconnected! SteamID : {steamId}");
				sendPacketByHeaderType(steamId, PacketHeaderType.REQ_DISCONNECT);
				mTransporter.Disconnect(steamId);
			}
			else
			{
				// 서버가 닫혔습니다.
				if (steamId == ServerSessionInfo.SteamID)
				{
					Ulog.Log(this, $"Server is closed!");
					sendPacketByHeaderType(steamId, PacketHeaderType.REQ_DISCONNECT);
					Disconnect(null);
				}
			}
		}

		/// <summary>접속을 종료합니다.</summary>
		/// <param name="netCallback">종료 후 콜백입니다.</param>
		public void Disconnect(Action<NetCallback> netCallback)
		{
			if (SessionState != SessionState.Connected)
			{
				Ulog.LogError(this, $"You cannot disconnect while : {SessionState}");
				return;
			}

			SessionState = SessionState.Ready;

			foreach (var id in mSessionBySteamID.ReverseValues.ToList())
			{
				if (id == mSteamService.CurrentUserID)
				{
					continue;
				}

				DisconnectSession(id);
			}

			// Services
			mLobbyService.TryLeaveLobby(null);
			mTransporter.Stop();
			mNetworkObjectManager.Stop();

			// Clear session info
			mSessionBySteamID.Clear();
			mSessionByNetID.Clear();
			mSessionIdCounter = 0;
			ClientSessionID = 0;
			mLobbySetting = null;
			mServerEndPoint = null;
			ServerSessionInfo = null;
			ClientSessionInfo = null;
			mConnectRequestInfo = new NetConnectRequestInfo();

			// Network
			NetworkMode = NetworkMode.None;
			mNetworkTimer.Stop();

			// State Flag
			mShouldCheckHeartbeat = true;

			NetCallback callback = new NetCallback();
			callback.Operation = NetOperationType.OnDisconnected;
			netCallback?.Invoke(callback);

			OnDisconnected?.Invoke();
			Ulog.Log(this, $"Disconnected!");
		}

		/// <summary>지정된 EndPoint로 접속을 시도합니다.</summary>
		/// <param name="endPoint">Steam Lobby로 접속할 경우 Lobby정보가 담겨있습니다.</param>
		/// <param name="request">최초 접속시 전달하는 정보입니다. 비밀번호가 포함되어있습니다.</param>
		/// <param name="netCallback">실패시 콜백이 바로 발생됩니다.</param>
		/// <returns></returns>
		public async Task TryConnectTo
		(
			EndPointInfo endPoint,
			NetConnectRequestInfo request,
			Action<NetCallback> netCallback
		)
		{
			await Task.Delay(KaNetGlobal.DEFALUT_GUI_OPERATION_DELAY);

			var callback = new NetCallback(NetOperationType.TryConnect);

			// Check it's available to connect
			if (NetworkMode.IsServer())
			{
				callback.Result = NetOperationResult.InvalidOperation;
				netCallback?.Invoke(callback);
				return;
			}

			this.NetworkMode = NetworkMode.Client;

			if (SessionState == SessionState.Connected)
			{
				callback.Result = NetOperationResult.AlreadyInTheGame;
				netCallback?.Invoke(callback);
				return;
			}
			else if (SessionState != SessionState.Ready)
			{
				callback.Result = NetOperationResult.InvalidOperation;
				netCallback?.Invoke(callback);
				return;
			}

			// Set network mode
			this.SessionState = SessionState.Connecting;

			// Join to lobby
			var isEntered = await mLobbyService.TryJoinToLobbyAsync(endPoint.TargetLobby);

			if (!isEntered)
			{
				callback.Result = NetOperationResult.Failed;
				netCallback?.Invoke(callback);
			}

			// Start to send
			mTransporter.Start();

			// Set EndPoint
			mServerEndPoint = endPoint;

			// Send connect packet
			// Get packet from pool
			var packet = PacketPool.GetMtuPacket();
			var writer = packet.GetWriter();
			writer.OffsetWriteIndex(NetBaseHeader.HEADER_SIZE);

			// Write program id
			request.SerializeTo(writer);

			// Write header
			NetBaseHeader header = new NetBaseHeader
			(
				PacketHeaderType.REQ_CONNECT,
				(ushort)writer.WriteIndex,
				0,
				0
			);

			writer.WriteAt(header, 0);

			ServerSessionInfo = mServerEndPoint.Value.GetServerSessionInfo();

			// Send data
			mTransporter.SendToViaReliable
			(
				ServerSessionInfo.SteamID,
				packet
			);

			// Release packet
			PacketPool.ReturnMtuPacket(packet);

			await Task.Run(() =>
			{
				int checkTime = KaNetGlobal.CONNECTION_TIMEOUT_INTERVAL / CONNECTING_CHECK_INTERVAL;

				for (int i = 0; i < checkTime; i++)
				{
					Task.Delay(CONNECTING_CHECK_INTERVAL).Wait();
					if (SessionState == SessionState.Connecting)
					{
						continue;
					}

					if (SessionState == SessionState.Connected)
					{
						callback.Result = NetOperationResult.Success;
						netCallback?.Invoke(callback);
						return;
					}

					onConnectFailed();

					callback.Result = (mAckCallbackResult == NetOperationResult.Success)
						? NetOperationResult.Failed : mAckCallbackResult;
					netCallback?.Invoke(callback);
					return;
				}

				onConnectFailed();

				callback.Result = NetOperationResult.Timeout;
				netCallback?.Invoke(callback);
			});

			void onConnectFailed()
			{
				SessionState = SessionState.Ready;
				mTransporter.Stop();
			}
		}

		/// <summary>스팀 백엔드를 통해서 방을 생성합니다.</summary>
		/// <param name="lobbySetting">로비 설정입니다.</param>
		/// <param name="netCallback">콜백</param>
		public async Task TryStartViaSteamAsync
		(
			LobbySetting lobbySetting,
			Action<NetCallback> netCallback
		)
		{
			await Task.Delay(KaNetGlobal.DEFALUT_GUI_OPERATION_DELAY);

			// Check it's available to create lobby
			NetCallback callback = new NetCallback();
			callback.Operation = NetOperationType.CreateLobby;

			if (SessionState == SessionState.Connected)
			{
				callback.Result = NetOperationResult.AlreadyInTheGame;
				netCallback?.Invoke(callback);
				return;
			}
			else if (SessionState != SessionState.Ready)
			{
				callback.Result = NetOperationResult.InvalidOperation;
				netCallback?.Invoke(callback);
				return;
			}

			mLobbySetting = lobbySetting;

			if (!mSteamService.IsValid)
			{
				callback.Result = NetOperationResult.SteamInvalid;
				netCallback?.Invoke(callback);
				return;
			}

			// Create lobby
			await mLobbyService.TryCreateLobbyAync(lobbySetting, (lobbyCallback) =>
			{
				if (lobbyCallback.Result != NetOperationResult.Success)
				{
					netCallback?.Invoke(lobbyCallback);
					return;
				}

				mConnectRequestInfo = new NetConnectRequestInfo
				(
					lobbySetting.ProgramID,
					lobbySetting.LobbyPassword
				);

				// Start to run server
				if (!StartServer())
				{
					lobbyCallback.Result = NetOperationResult.Failed;
					netCallback?.Invoke(lobbyCallback);
					return;
				}

				// Add self server session
				NetSessionInfo serverInfo = new(0, mSteamService.CurrentUserID, CurrentTimestamp);
				serverInfo.SetIsReady();
				ClientSessionID = getNewSessionID();
				mSessionBySteamID.Add(serverInfo.SteamID, serverInfo);
				mSessionByNetID.Add(ClientSessionID, serverInfo);
				ServerSessionInfo = serverInfo;
				ClientSessionInfo = serverInfo;
				OnReadySessionConnected?.Invoke(serverInfo);
				OnSessionConnectionChanged?.Invoke();

				netCallback?.Invoke(lobbyCallback);
			});
		}

		#endregion

		#region Starter

		public bool StartHideout()
		{
			return true;
		}

		public bool StartSingleplayer()
		{
			NetworkMode = NetworkMode.Singleplayer;

			onSessionStarted();
			return true;
		}

		public bool StartServer()
		{
			NetworkMode = NetworkMode.Server_Listen;

			onSessionStarted();
			return true;
		}

		public bool StartClient()
		{
			NetworkMode = NetworkMode.Client;

			onSessionStarted();
			return true;
		}

		#endregion

		#region Event

		public void OnMapLoadStart()
		{
			mShouldCheckHeartbeat = true;
		}

		public void OnMapLoadFinished()
		{
			mShouldCheckHeartbeat = false;
		}

		private void onSessionStarted()
		{
			mNetworkTimer.Reset();
			mNetworkTimer.Start();
			mTransporter.Start();
			SessionState = SessionState.Connected;
			mShouldCheckHeartbeat = true;
		}

		private void onLeaveLobbby()
		{

		}

		private void onJoinLobby()
		{

		}

		#endregion

		#region Operation

		/// <summary>현재 접속한 로비를 반환받습니다.</summary>
		/// <returns>접속한 상태가 아니라면 false를 반환합니다.</returns>
		public bool TryGetCurrentLobby(out Lobby currentLobby)
		{
			return mLobbyService.TryGetCurrentLobby(out currentLobby);
		}

		/// <summary>친구를 초대합니다.</summary>
		/// <param name="friendInfo">초대할 세션 정보</param>
		/// <param name="netCallback">콜백</param>
		public void InviteFriend(NetSessionInfo friendInfo, Action<NetCallback> netCallback)
		{
			// Steam Error Callback
			if (!mSteamService.IsValid)
			{
				NetCallback callback = new NetCallback();
				callback.Operation = NetOperationType.CreateLobby;
				callback.Result = NetOperationResult.SteamInvalid;
				netCallback?.Invoke(callback);
				return;
			}
			mLobbyService.TryInviteFriend(friendInfo, netCallback);
		}

		/// <summary>로비를 탐색합니다.</summary>
		/// <param name="requestCallback"></param>
		public void RequestLobbyList(Action<Lobby[]> requestCallback)
		{
			var task = mLobbyService.RequestLobbyList(requestCallback);
		}

		/// <summary>현재 스팀의 친구 목록을 받아옵니다.</summary>
		/// <param name="friends">친구 목록입니다.</param>
		/// <returns>스팀이 활성화되지 않았다면 false를 반환합니다.</returns>
		public bool TryGetFriendList(out IEnumerable<Friend> friends)
		{
			return mSteamService.TryGetFriendList(out friends);
		}

		#endregion

		#region Send To Ready Players

		[Obsolete("사용되지 않음")]
		public void SendToReadyClientViaReliable(NetPacket packet)
		{
			foreach (var session in mSessionBySteamID.ForwardValues)
			{
				if (session.IsReadyToSync)
				{
					mTransporter.SendToViaReliable(session.SteamID, packet);
				}
			}
		}

		[Obsolete("사용되지 않음")]
		public void SendToReadyClientViaUnreliable(NetPacket packet)
		{
			foreach (var session in mSessionBySteamID.ForwardValues)
			{
				if (session.IsReadyToSync)
				{
					mTransporter.SendToViaUnreliable(session.SteamID, packet);
				}
			}
		}

		public void SendReliableIfReady(NetSessionID to, NetPacket packet)
		{
			if (mSessionByNetID.TryGetValue(to, out var sessionInfo))
			{
				mTransporter.SendToViaReliable(sessionInfo.SteamID, packet);
			}
		}

		public void SendUnreliableIfReady(NetSessionID to, NetPacket packet)
		{
			if (mSessionByNetID.TryGetValue(to, out var sessionInfo))
			{
				mTransporter.SendToViaUnreliable(sessionInfo.SteamID, packet);
			}
		}

		#endregion

		#region Send

		public void SendToViaReliable(NetSessionInfo to, NetPacket packet)
		{
			if (mSessionBySteamID.TryGetValue(to, out var id))
			{
				mTransporter.SendToViaReliable(id, packet);
			}
		}

		public void SendToViaUnreliable(NetSessionInfo to, NetPacket packet)
		{
			if (mSessionBySteamID.TryGetValue(to, out var id))
			{
				mTransporter.SendToViaUnreliable(id, packet);
			}
		}

		public void SendToViaReliable(NetSessionID to, NetPacket packet)
		{
			if (mSessionByNetID.TryGetValue(to, out var info))
			{
				mTransporter.SendToViaReliable(info.SteamID, packet);
			}
		}

		public void SendToViaUnreliable(NetSessionID to, NetPacket packet)
		{
			if (mSessionByNetID.TryGetValue(to, out var info))
			{
				mTransporter.SendToViaUnreliable(info.SteamID, packet);
			}
		}

		public void SendToServerViaReliable(NetPacket packet)
		{
			if (ServerSessionInfo != null)
			{
				mTransporter.SendToViaReliable(ServerSessionInfo.SteamID, packet);
			}
		}

		public void SendToServerViaUnreliable(NetPacket packet)
		{
			if (ServerSessionInfo != null)
			{
				mTransporter.SendToViaUnreliable(ServerSessionInfo.SteamID, packet);
			}
		}

		#endregion
	}
}
