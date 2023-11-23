using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KaNet.Core;
using KaNet.Session;
using KaNet.Synchronizers;
using KaNet.Utils;
using UnityEngine;

using Utile;
using Utils;
using Utils.Service;

namespace KaNet.Synchronizers
{
	/// <summary>현재 루프의 DeltaTime 및 Global Time Scale정보를 담은 구조체입니다.</summary>
	public readonly struct DeltaTimeInfo
	{
		/// <summary>현재 루프의 DeltaTime입니다.</summary>
		public readonly float DeltaTime;
		/// <summary>전역 Time Scale이 적용된 DeltaTime입니다.</summary>
		public readonly float ScaledDeltaTime;
		/// <summary>전역 Time Scale입니다.</summary>
		public readonly float GlobalTimeScale;

		public DeltaTimeInfo(float deltaTime, float globalTimeScale)
		{
			DeltaTime = deltaTime;
			GlobalTimeScale = globalTimeScale;
			ScaledDeltaTime = DeltaTime * GlobalTimeScale;
		}

		public DeltaTimeInfo(float globalTimeScale)
		{
			DeltaTime = 0;
			GlobalTimeScale = globalTimeScale;
			ScaledDeltaTime = DeltaTime * GlobalTimeScale;
		}
	}

	/// <summary>네트워크 오브젝트 객체를 관리하는 매니저 클래스입니다.</summary>
	public class NetworkObjectManager
	{
		private bool mIsServer => mSessionHandler.NetworkMode.IsServer();
		private bool mIsClient => mSessionHandler.NetworkMode.IsClient();
		public bool IsRunning { get; private set; } = false;

		// Object Instance Manage
		private MonoObjectPoolService mNetworkObjectPool;
		private Dictionary<NetObjectType, GameObject> mObjectTable;

		// DI
		private SessionHandlerService mSessionHandler;

		public event Action<NetSessionInfo> OnReadySessionConnected
		{
			add => mSessionHandler.OnReadySessionConnected += value;
			remove => mSessionHandler.OnReadySessionConnected -= value;
		}
		public event Action<NetSessionInfo> OnSessionDisconnected
		{
			add => mSessionHandler.OnSessionDisconnected += value;
			remove => mSessionHandler.OnSessionDisconnected -= value;
		}

		public NetSessionInfo ClientNetSessionInfo => mSessionHandler.ClientSessionInfo;
		public NetTimestamp CurrentTimestamp => mSessionHandler.CurrentTimestamp;
		public NetSessionID CurrentClientID => mSessionHandler.ClientSessionID;

		// Session
		private List<NetSessionInfo> mReadySessionInfoList = new();
		public NetSessionInfo[] ReadySessionInfoList => mReadySessionInfoList.ToArray();

		// Network Object
		private List<NetworkObject> mNetworkObjectList = new();
		private Dictionary<NetObjectID, NetworkObject> mObjectByID = new();

		// Binded Object
		private BidirectionalMap<Type, NetworkObject> mBindedObjectByType = new();

		private Tessellator mTessellator;

		private NetObjectID mExsitObjectID;
		private NetObjectID getNetExsitObjectID()
		{
			mExsitObjectID++;

			for (int i = 0; i < KaNetGlobal.INITIAL_OBJECT_COUNT; i++)
			{
				if (mObjectByID.ContainsKey(mIdCounter))
				{
					mIdCounter++;
				}
				else
				{
					break;
				}
			}

			return mExsitObjectID;
		}

		// ID Counter
		private NetObjectID mIdCounter;
		private NetObjectID getNewObjectID()
		{
			mIdCounter++;

			for (int i = 0; i < ushort.MaxValue; i++)
			{
				if (mIdCounter < KaNetGlobal.INITIAL_OBJECT_COUNT)
				{
					mIdCounter = KaNetGlobal.INITIAL_OBJECT_COUNT;
					continue;
				}

				if (mObjectByID.ContainsKey(mIdCounter))
				{
					mIdCounter++;
				}
				else
				{
					break;
				}
			}

			return mIdCounter;
		}

		// Creation Events
		public event Action<NetworkObject> OnCreated;

		public float GlobalTimeScale { get; private set; } = 1;

		public NetworkObjectManager
		(
			SessionHandlerService networkSessionHandlerService,
			Dictionary<NetObjectType, GameObject> networkObjectTable
		)
		{
			mSessionHandler = networkSessionHandlerService;
			mObjectTable = networkObjectTable;

			mTessellator = new Tessellator(networkSessionHandlerService);

			OnReadySessionConnected += (s) => mReadySessionInfoList.TryAddUnique(s);
			OnSessionDisconnected += (s) => mReadySessionInfoList.Remove(s);
		}

		/// <summary>
		/// 네트워크 오브젝트 관리를 시작합니다.
		/// 서버라면 동기화가 시작되고, 클라이언트라면 최초 데이터 요청 패킷을 전송합니다.
		/// </summary>
		/// <param name="objectPoolService"></param>
		public void Start(MonoObjectPoolService objectPoolService)
		{
			mNetworkObjectPool = objectPoolService;
			mExsitObjectID = 0;
			mIdCounter = KaNetGlobal.INITIAL_OBJECT_COUNT;
			IsRunning = true;

			if (mIsClient)
			{
				// Request initial data to server
				NetPacket packet = new NetPacket(30);
				var writer = packet.GetWriter();

				NetBaseHeader header = new NetBaseHeader();
				header.PacketHeader = PacketHeaderType.REQ_OBJ_SYN;
				header.SenderID = mSessionHandler.ClientSessionID;
				header.PacketLength = NetBaseHeader.HEADER_SIZE;
				header.Timestamp = mSessionHandler.CurrentTimestamp;

				writer.WriteAt(header, 0);

				mSessionHandler.SendToServerViaReliable(packet);
			}
		}

		public void Stop()
		{
			Clear(true);
			mTessellator.Clear();
			IsRunning = false;
			mReadySessionInfoList.Clear();
			mBindedObjectByType.Clear();
			GlobalTimeScale = 1;
		}

		public void Clear(bool includeKeepAliveObject = false)
		{
			for (int i = mNetworkObjectList.Count - 1; i >= 0; i--)
			{
				if (mNetworkObjectList[i].KeepAlive && !includeKeepAliveObject)
				{
					continue;
				}

				ReleaseNetworkObject(mNetworkObjectList[i]);
			}

			mNetworkObjectList.Clear();
			mObjectByID.Clear();
		}

		/// <summary>접속한 유저에게 초기 데이터를 전송합니다.</summary>
		/// <param name="sessionInfo">접속한 유저입니다.</param>
		public void SendInitialDataTo(NetSessionInfo sessionInfo)
		{
			List<NetLifeStreamToken> lifeStreams = new();
			foreach (var netObj in mNetworkObjectList)
			{
				lifeStreams.Add(new NetLifeStreamToken(netObj, true));
			}

			PacketGroup pg = new
			(
				SyncType.ReliableFixed,
				mSessionHandler.SendToViaReliable
			);

			pg.AddSendTo(sessionInfo.ID);

			if (TrySerializeSyncLifePacket
			(
				lifeStreams,
				mSessionHandler.ClientSessionID,
				sessionInfo.ID,
				mSessionHandler.CurrentTimestamp,
				pg
			))
			{
				pg.SendAndRelease();
			}
			else
			{
				pg.Release();
			}
		}

		public void SetGlobalTimescale(float timescale)
		{
			GlobalTimeScale = timescale;
			Time.timeScale = GlobalTimeScale;
		}

		#region Getter

		public List<NetSessionID> GetAllNetSessionIDs()
		{
			List<NetSessionID> sessions = new();

			foreach (var s in mReadySessionInfoList)
			{
				sessions.Add(s.ID);
			}

			return sessions;
		}

		public NetSessionID[] GetNetSessionIDsExcept(NetSessionID exception)
		{
			NetSessionID[] sessions = new NetSessionID[mReadySessionInfoList.Count - 1];

			int index = 0;
			foreach (var s in mReadySessionInfoList)
			{
				if (s.ID == exception)
				{
					continue;
				}

				sessions[index] = s.ID;
				index++;
			}

			return sessions;
		}

		public bool TryFindObjectByType<T>(out T networkObject) where T : NetworkObject
		{
			for (int i = 0; i < mNetworkObjectList.Count; i++)
			{
				if (mNetworkObjectList[i] is T)
				{
					networkObject = (T)mNetworkObjectList[i];
					return true;
				}
			}

			networkObject = null;

			return false;
		}

		public bool TryFindObjectsByType<T>(out List<T> networkObjects) where T : NetworkObject
		{
			networkObjects = new();

			for (int i = 0; i < mNetworkObjectList.Count; i++)
			{
				if (mNetworkObjectList[i] is T)
				{
					networkObjects.Add((T)mNetworkObjectList[i]);
				}
			}

			return !networkObjects.IsEmpty();
		}

		public bool TryFindObjectsByID(NetObjectID objectID, out NetworkObject networkObject)
		{
			return mObjectByID.TryGetValue(objectID, out networkObject);
		}

		#endregion

		#region Binder

		public bool TryBindNetworkObjectAsType<T>(T networkObject) where T : NetworkObject
		{
			return mBindedObjectByType.TryAdd(typeof(T), networkObject);
		}

		public bool TryGetBindedNetworkObject<T>(out T networkObject) where T : NetworkObject
		{
			if (mBindedObjectByType.TryGetValue(typeof(T), out var bindedObject))
			{
				networkObject = bindedObject as T;
				return true;
			}

			networkObject = null;
			return false;
		}

		#endregion

		#region Create and release

		public T CreateNetworkObjectAsServer<T>
		(
			NetObjectType type,
			NetSessionID ownerID,
			Vector3 position,
			Quaternion rotation
		) where T : NetworkObject
		{
			if (!mIsServer)
			{
				Ulog.LogError(this, "You are try to create object as server, BUT YOU ARE NOT THE SERVER!");
				return null;
			}

			var no = createNetworkObject
			(
				type,
				getNewObjectID(),
				ownerID,
				position,
				rotation,
				out bool isExist
			);

			if (!isExist)
			{
				no.Common_OnStart();

				if (mIsServer)
				{
					no.Server_OnStart();
				}

				if (mIsClient)
				{
					no.Client_OnStart();
				}

				OnCreated?.Invoke(no);
			}

			return no as T;
		}

		public NetworkObject AddInitialNetworkObject(NetworkObject netObject)
		{
			NetworkObjectPrebinder.PrebindByReflection(netObject.GetType(), netObject);
			netObject.SetInitialInfo(getNetExsitObjectID(), netObject.OwnerID, mSessionHandler.ClientSessionID);
			netObject.InitializeByManager(this, ReleaseNetworkObject, mIsServer, mIsClient);

			if (mObjectByID.TryAdd(netObject.ID, netObject))
			{
				mNetworkObjectList.Add(netObject);
				netObject.Common_OnStart();
				mTessellator.OnAddNetworkObject(netObject);

				if (mIsServer)
				{
					netObject.Server_OnStart();
				}
				// 초기 객체는 클라이언트 측에서 재생성하지 않음으로 OnStart를 호출해야한다.
				// If you are listen server
				if (mIsClient)
				{
					netObject.Client_OnStart();
				}

				return netObject;
			}

			Ulog.LogError(this, $"NetworkObject {netObject.Type} : {netObject.ID} already exist!");
			return null;
		}

		private NetworkObject createNetworkObject
		(
			NetObjectType type,
			NetObjectID objectID,
			NetSessionID ownerID,
			Vector3 position,
			Quaternion rotation,
			out bool isExist
		)
		{
			if (!mObjectTable.TryGetValue(type, out var netObjectPrefab))
			{
				Ulog.LogError(this, $"There is no such type as {type}.");
				isExist = false;
				return null;
			}

			var netObject = mNetworkObjectPool
				.CreateObject(netObjectPrefab, position, rotation)
				.GetComponent<NetworkObject>();

			NetworkObjectPrebinder.PrebindByReflection(netObject.GetType(), netObject);
			netObject.SetInitialInfo(objectID, ownerID, mSessionHandler.ClientSessionID);
			netObject.InitializeByManager(this, ReleaseNetworkObject, mIsServer, mIsClient);

			if (mObjectByID.TryGetValue(netObject.ID, out var existInstance))
			{
				isExist = true;
				return existInstance;
			}
			else
			{
				mObjectByID.Add(netObject.ID, netObject);
				mNetworkObjectList.Add(netObject);
				mTessellator.OnAddNetworkObject(netObject);

				isExist = false;
				return netObject;
			}
		}

		public void ReleaseNetworkObject(NetworkObject networkObject)
		{
			mTessellator.OnRemoveNetworkObject(networkObject);

			if (mIsServer)
			{
				networkObject.Server_OnDestroy();
			}

			if (mIsClient)
			{
				networkObject.Client_OnDestroy();
			}

			networkObject.Common_OnDestroy();

			mNetworkObjectPool.Release(networkObject.gameObject);

			mObjectByID.Remove(networkObject.ID);
			mNetworkObjectList.Remove(networkObject);
		}

		#endregion

		#region Network Events

		public void OnUpdate()
		{
			DeltaTimeInfo deltaTimeInfo = new(Time.deltaTime, GlobalTimeScale);

			// Update events
			if (mIsServer)
			{
				for (int i = 0; i < mNetworkObjectList.Count; i++)
				{
					mNetworkObjectList[i].Server_OnUpdate(deltaTimeInfo);
				}
			}
			if (mIsClient)
			{
				for (int i = 0; i < mNetworkObjectList.Count; i++)
				{
					mNetworkObjectList[i].Client_OnUpdate(deltaTimeInfo);
				}
			}
		}

		public void OnFixedUpdate()
		{
			DeltaTimeInfo deltaTimeInfo = new(Time.fixedDeltaTime, GlobalTimeScale);

			// Fixed update events
			if (mIsServer)
			{
				for (int i = 0; i < mNetworkObjectList.Count; i++)
				{
					mNetworkObjectList[i].Server_OnFixedUpdate(deltaTimeInfo);
				}
			}
			if (mIsClient)
			{
				for (int i = 0; i < mNetworkObjectList.Count; i++)
				{
					mNetworkObjectList[i].Client_OnFixedUpdate(deltaTimeInfo);
				}
			}

			// Serialize
			var teeselTiles = mTessellator.GetAllTesselTile();
			syncFieldDataTo(teeselTiles, SyncType.ReliableInstant);
			onFieldSerialized(SyncType.ReliableInstant);

			syncRPCsDataTo(teeselTiles, SyncType.ReliableInstant);
			onRPCsSerialized(SyncType.ReliableInstant);

			syncFieldDataTo(teeselTiles, SyncType.UnreliableInstant);
			onFieldSerialized(SyncType.UnreliableInstant);

			syncRPCsDataTo(teeselTiles, SyncType.UnreliableInstant);
			onRPCsSerialized(SyncType.UnreliableInstant);
		}

		public void OnNetworkTickUpdate()
		{
			if (mIsServer)
			{
				mTessellator.CalculateCoord();
			}

			// Before fixed update
			if (mIsServer)
			{
				for (int i = 0; i < mNetworkObjectList.Count; i++)
				{
					mNetworkObjectList[i].Server_OnBeforeNetworkTickUpdate();
				}
			}
			if (mIsClient)
			{
				for (int i = 0; i < mNetworkObjectList.Count; i++)
				{
					mNetworkObjectList[i].Client_OnBeforeNetworkTickUpdate();
				}
			}

			// Serialize
			var teeselTiles = mTessellator.GetAllTesselTile();
			syncFieldDataTo(teeselTiles, SyncType.ReliableFixed);
			onFieldSerialized(SyncType.ReliableFixed);

			syncRPCsDataTo(teeselTiles, SyncType.ReliableFixed);
			onRPCsSerialized(SyncType.ReliableFixed);

			syncFieldDataTo(teeselTiles, SyncType.UnreliableFixed);
			onFieldSerialized(SyncType.UnreliableFixed);

			syncRPCsDataTo(teeselTiles, SyncType.UnreliableFixed);
			onRPCsSerialized(SyncType.UnreliableFixed);

			// Send life stream data
			if (mIsServer)
			{
				var tokens = mTessellator.GetLifeStream();

				foreach (var session in mReadySessionInfoList)
				{
					if (session.ID == ClientNetSessionInfo.ID)
					{
						continue;
					}

					PacketGroup streamPacket = new
					(
						SyncType.ReliableFixed,
						mSessionHandler.SendReliableIfReady
					);

					streamPacket.AddSendTo(session.ID);

					if (TrySerializeSyncLifePacket
					(
						tokens,
						mSessionHandler.ClientSessionID,
						session.ID,
						mSessionHandler.CurrentTimestamp,
						streamPacket
					))
					{
						streamPacket.SendAndRelease();
					}
					else
					{
						streamPacket.Release();
					}
				}
			}
		}

		#endregion

		#region Synchronize

		private void onFieldSerialized(SyncType syncType)
		{
			foreach (var no in mObjectByID.Values)
			{
				no.OnFieldSerialized(syncType);
			}
		}

		private void onRPCsSerialized(SyncType syncType)
		{
			foreach (var no in mObjectByID.Values)
			{
				no.OnRPCsSerialized(syncType);
			}
		}

		private void syncFieldDataTo(List<TesselTile> tesselTiles, SyncType syncType)
		{
			Action<NetSessionID, NetPacket> sendAction = null;

			if (syncType.IsReliable())
			{
				sendAction = mSessionHandler.SendReliableIfReady;
			}
			else if (syncType.IsUnreliable())
			{
				sendAction = mSessionHandler.SendUnreliableIfReady;
			}

			foreach (var session in mReadySessionInfoList)
			{
				PacketGroup packetGroup = new PacketGroup(syncType, sendAction);
				packetGroup.AddSendTo(session.ID);

				if (TrySerializeFieldPacket
				(
					tesselTiles,
					syncType,
					mIsServer,
					mSessionHandler.ClientSessionID,
					session.ID,
					CurrentTimestamp,
					packetGroup
				))
				{
					packetGroup.SendAndRelease();
				}
				else
				{
					packetGroup.Release();
				}
			}
		}

		private void syncRPCsDataTo(List<TesselTile> tesselTiles, SyncType syncType)
		{
			Action<NetSessionID, NetPacket> sendAction = null;

			if (syncType.IsReliable())
			{
				sendAction = mSessionHandler.SendReliableIfReady;
			}
			else if (syncType.IsUnreliable())
			{
				sendAction = mSessionHandler.SendUnreliableIfReady;
			}

			foreach (var session in mReadySessionInfoList)
			{
				PacketGroup packetGroup = new PacketGroup(syncType, sendAction);
				packetGroup.AddSendTo(session.ID);

				if (TrySerializeRPCsPacket
				(
					tesselTiles,
					syncType,
					mIsServer,
					mSessionHandler.ClientSessionID,
					session.ID,
					CurrentTimestamp,
					packetGroup
				))
				{
					packetGroup.SendAndRelease();
				}
				else
				{
					packetGroup.Release();
				}
			}
		}

		public bool TrySerializeFieldPacket
		(
			IList<TesselTile> tesselTiles,
			SyncType syncType,
			bool isServer,
			NetSessionID sender,
			NetSessionID sendTo,
			NetTimestamp timestamp,
			PacketGroup packetGroup
		)
		{
			bool isEcho = sender == sendTo;

			var packets = new List<NetPacket>();

			if (tesselTiles.IsNullOrEmpty())
			{
				return false;
			}

			int lastTesselIndex = 0;
			int lastObjIndex = 0;
			int lastFieldIndex = 0;

			// Make base packet
			while (lastTesselIndex < tesselTiles.Count)
			{
				var packet = packetGroup.GetterAction();
				var writer = packet.GetWriter();

				NetBaseHeader baseHeader = new();
				int baseHeaderIndex = writer.WriteIndex;
				if (!writer.CanWrite(baseHeader.GetSyncDataSize()))
				{
					break;
				}
				writer.OffsetWriteIndex(baseHeader.GetSyncDataSize());

				bool isFinished = trySerializeTesselField
				(
					writer,
					tesselTiles,
					sendTo,
					lastTesselIndex,
					lastObjIndex,
					lastFieldIndex,
					out lastTesselIndex,
					out lastObjIndex,
					out lastFieldIndex,
					out bool isSerialized
				);

				if (isSerialized)
				{
					// Write packet headers
					baseHeader.PacketHeader = PacketHeaderType.SYN_OBJ_FIELD;
					baseHeader.PacketLength = (ushort)writer.WriteIndex;
					baseHeader.SenderID = sender;
					baseHeader.Timestamp = timestamp;
					writer.WriteAt(baseHeader, baseHeaderIndex);
					packets.Add(packet);
				}
				else
				{
					packetGroup.ReleaserAction(packet);
				}

				if (isFinished)
				{
					break;
				}
			}

			if (packets.IsEmpty())
			{
				return false;
			}

			packetGroup.AddPacket(packets);
			return true;

			/// <summary>네트워크 객체의 필드를 직렬화합니다.</summary>
			/// <returns>직렬화가 완료되면 true를 반환합니다. 패킷이 가득 찼다면 false를 반환합니다.</returns>
			bool trySerializeTesselField
			(
				NetPacketWriter writer,
				IList<TesselTile> tesselTile,
				NetSessionID sendTo,
				int startTesselIndex,
				int startObjIndex,
				int startFieldIndex,
				out int lastTesselIndex,
				out int lastObjIndex,
				out int lastFieldIndex,
				out bool isSerialized
			)
			{
				isSerialized = false;

				for (int i = startTesselIndex; i < tesselTile.Count; i++)
				{
					var tile = tesselTile[i];

					NetSyncHeader syncHeader = new NetSyncHeader();
					int syncHeaderIndex = writer.WriteIndex;
					if (!writer.CanWrite(syncHeader.GetSyncDataSize()))
					{
						lastTesselIndex = i;
						lastObjIndex = startObjIndex;
						lastFieldIndex = startFieldIndex;

						// 패킷이 가득 참
						return false;
					}
					writer.OffsetWriteIndex(syncHeader.GetSyncDataSize());

					bool isFinished = trySerializeField
					(
						writer,
						tile.ObjectList,
						sendTo,
						startObjIndex,
						startFieldIndex,
						out lastObjIndex,
						out lastFieldIndex,
						out var syncCount
					);

					if (syncCount > 0)
					{
						syncHeader.Coord = tile.TesselCoord;
						syncHeader.Count = (ushort)syncCount;

						writer.WriteAt(syncHeader, syncHeaderIndex);
						isSerialized = true;
					}
					else
					{
						writer.MoveWriteIndex(syncHeaderIndex);
					}

					// 패킷이 가득 참
					if (!isFinished)
					{
						lastTesselIndex = i;
						return false;
					}

					startObjIndex = 0;
					startFieldIndex = 0;
				}

				lastTesselIndex = 0;
				lastObjIndex = 0;
				lastFieldIndex = 0;
				return true;

				/// <summary>네트워크 객체의 필드를 직렬화합니다.</summary>
				/// <returns>직렬화가 완료되면 true를 반환합니다. 아직 남아있다면 false를 반환합니다.</returns>
				bool trySerializeField
				(
					NetPacketWriter writer,
					IList<NetworkObject> netObjs, 
					NetSessionID sendTo,
					int startObjIndex,
					int startFieldIndex,
					out int lastObjIndex,
					out int lastFieldIndex,
					out int syncCount
				)
				{
					syncCount = 0;

					for (int i = startObjIndex; i < netObjs.Count; i++)
					{
						var netObj = netObjs[i];

						// 직렬화시 체크함
						//if (netObj.IsOwnerOnly && netObj.OwnerID != sendTo)
						//{
						//	continue; // It's not authorized.
						//}

						if (!netObj.IsFieldChanged(syncType))
						{
							continue; // It hasn't been changed.
						}

						int idHeaderIndex = writer.WriteIndex;
						if (!writer.CanWrite(netObj.ID.GetSyncDataSize()))
						{
							lastObjIndex = i;
							lastFieldIndex = startFieldIndex;

							// 패킷이 가득 참
							return false;
						}
						writer.OffsetWriteIndex(netObj.ID.GetSyncDataSize());

						if (netObj.TrySerializeChangedPartTo
						(
							writer,
							syncType,
							isServer,
							sender,
							isEcho,
							startFieldIndex,
							out lastFieldIndex
						))
						{
							writer.WriteAt(netObj.ID, idHeaderIndex);
							startFieldIndex = 0;
							lastFieldIndex = 0;
							syncCount++;
						}
						else
						{
							writer.MoveWriteIndex(idHeaderIndex);
							lastObjIndex = i;

							// 패킷이 가득 참
							return false;
						}
					}

					lastObjIndex = 0;
					lastFieldIndex = 0;
					return true;
				}
			}

		}

		public bool TrySerializeRPCsPacket
		(
			IList<TesselTile> tesselTiles,
			SyncType syncType,
			bool isServer,
			NetSessionID sender,
			NetSessionID sendTo,
			NetTimestamp timestamp,
			PacketGroup packetGroup
		)
		{
			bool isEcho = sender == sendTo;

			var packets = new List<NetPacket>();

			if (tesselTiles.IsNullOrEmpty())
			{
				return false;
			}

			int lastTesselIndex = 0;
			int lastObjIndex = 0;
			int lastRPCsIndex = 0;
			int lastRpcCallIndex = 0;

			// Make base packet
			while (lastTesselIndex < tesselTiles.Count)
			{
				var packet = packetGroup.GetterAction();
				var writer = packet.GetWriter();

				NetBaseHeader baseHeader = new();
				int baseHeaderIndex = writer.WriteIndex;
				if (!writer.CanWrite(baseHeader.GetSyncDataSize()))
				{
					break;
				}
				writer.OffsetWriteIndex(baseHeader.GetSyncDataSize());

				bool isFinished = trySerializeTesselRPCs
				(
					writer,
					tesselTiles,
					sendTo,
					lastTesselIndex,
					lastObjIndex,
					lastRPCsIndex,
					lastRpcCallIndex,
					out lastTesselIndex,
					out lastObjIndex,
					out lastRPCsIndex,
					out lastRpcCallIndex,
					out bool isSerialized
				);

				if (isSerialized)
				{
					// Write packet headers
					baseHeader.PacketHeader = PacketHeaderType.SYN_OBJ_RPC;
					baseHeader.PacketLength = (ushort)writer.WriteIndex;
					baseHeader.SenderID = sender;
					baseHeader.Timestamp = timestamp;
					writer.WriteAt(baseHeader, baseHeaderIndex);
					packets.Add(packet);
				}
				else
				{
					packetGroup.ReleaserAction(packet);
				}

				if (isFinished)
				{
					break;
				}
			}

			if (packets.IsEmpty())
			{
				return false;
			}

			packetGroup.AddPacket(packets);
			return true;

			/// <summary>네트워크 객체의 필드를 직렬화합니다.</summary>
			/// <returns>직렬화가 완료되면 true를 반환합니다. 패킷이 가득 찼다면 false를 반환합니다.</returns>
			bool trySerializeTesselRPCs
			(
				NetPacketWriter writer,
				IList<TesselTile> tesselTile,
				NetSessionID sendTo,
				int startTesselIndex,
				int startObjIndex,
				int startRpcIndex,
				int startRpcCallIndex,
				out int lastTesselIndex,
				out int lastObjIndex,
				out int lastRpcIndex,
				out int lastRpcCallIndex,
				out bool isSerialized
			)
			{
				isSerialized = false;

				for (int i = startTesselIndex; i < tesselTile.Count; i++)
				{
					var tile = tesselTile[i];

					NetSyncHeader syncHeader = new NetSyncHeader();
					int syncHeaderIndex = writer.WriteIndex;
					if (!writer.CanWrite(syncHeader.GetSyncDataSize()))
					{
						lastTesselIndex = i;
						lastObjIndex = startObjIndex;
						lastRpcIndex = startRpcIndex;
						lastRpcCallIndex = startRpcCallIndex;

						// 패킷이 가득 참
						return false;
					}
					writer.OffsetWriteIndex(syncHeader.GetSyncDataSize());

					bool isFinished = trySerializeField
					(
						writer,
						tile.ObjectList,
						sendTo,
						startObjIndex,
						startRpcIndex,
						startRpcCallIndex,
						out lastObjIndex,
						out lastRpcIndex,
						out lastRpcCallIndex,
						out var syncCount
					);

					if (syncCount > 0)
					{
						syncHeader.Coord = tile.TesselCoord;
						syncHeader.Count = (ushort)syncCount;

						writer.WriteAt(syncHeader, syncHeaderIndex);
						isSerialized = true;
					}
					else
					{
						writer.MoveWriteIndex(syncHeaderIndex);
					}

					// 패킷이 가득 참
					if (!isFinished)
					{
						lastTesselIndex = i;
						return false;
					}

					startObjIndex = 0;
					startRpcIndex = 0;
				}

				lastTesselIndex = 0;
				lastObjIndex = 0;
				lastRpcIndex = 0;
				lastRpcCallIndex = 0;
				return true;

				/// <summary>네트워크 객체의 필드를 직렬화합니다.</summary>
				/// <returns>직렬화가 완료되면 true를 반환합니다. 아직 남아있다면 false를 반환합니다.</returns>
				bool trySerializeField
				(
					NetPacketWriter writer,
					IList<NetworkObject> netObjs,
					NetSessionID sendTo,
					int startObjIndex,
					int startRPCsIndex,
					int startRpcCallIndex,
					out int lastObjIndex,
					out int lastRPCsIndex,
					out int lastRpcCallIndex,
					out int syncCount
				)
				{
					syncCount = 0;

					for (int i = startObjIndex; i < netObjs.Count; i++)
					{
						var netObj = netObjs[i];

						if (netObj.IsOwnerOnly && netObj.OwnerID != sendTo)
						{
							continue; // It's not authorized.
						}

						if (!netObj.IsRPCsCalled(syncType))
						{
							continue; // It hasn't been changed.
						}

						int idHeaderIndex = writer.WriteIndex;
						if (!writer.CanWrite(netObj.ID.GetSyncDataSize()))
						{
							lastObjIndex = i;
							lastRPCsIndex = startRPCsIndex;
							lastRpcCallIndex = startRpcCallIndex;

							// 패킷이 가득 참
							return false;
						}
						writer.OffsetWriteIndex(netObj.ID.GetSyncDataSize());

						if (netObj.TrySerializeRpcCallData
						(
							writer,
							syncType,
							isServer,
							sender,
							sendTo,
							isEcho,
							startRpcIndex,
							startRpcCallIndex,
							out lastRPCsIndex,
							out lastRpcCallIndex
						))
						{
							writer.WriteAt(netObj.ID, idHeaderIndex);
							startRPCsIndex = 0;
							startRpcCallIndex = 0;
							lastRPCsIndex = 0;
							lastRpcCallIndex = 0;
							syncCount++;
						}
						else
						{
							writer.MoveWriteIndex(idHeaderIndex);
							lastObjIndex = i;

							// 패킷이 가득 참
							return false;
						}
					}

					lastObjIndex = 0;
					lastRPCsIndex = 0;
					lastRpcCallIndex = 0;
					return true;
				}
			}
		}

		public bool TrySerializeSyncLifePacket
		(
			IList<NetLifeStreamToken> tokens,
			NetSessionID sender,
			NetSessionID sendTo,
			NetTimestamp timestamp,
			PacketGroup packetGroup
		)
		{
			if (tokens.Count <= 0)
			{
				return false;
			}

			var packets = new List<NetPacket>();

			int lifeTokenIndex = 0;
			int lastFieldIndex = 0;

			while (true)
			{
				// Initialize Packet
				var packet = packetGroup.GetterAction();
				var writer = packet.GetWriter();
				NetBaseHeader baseHeader = new NetBaseHeader();
				NetSyncHeader syncHeader = new NetSyncHeader();
				ushort syncCount = 0;

				// Offset header
				writer.OffsetWriteIndex(baseHeader.GetSyncDataSize());
				writer.OffsetWriteIndex(syncHeader.GetSyncDataSize());

				while (lifeTokenIndex < tokens.Count)
				{
					var lifeToken = tokens[lifeTokenIndex];

					int tokenSize = lifeToken.GetSyncDataSize();
					if (!writer.CanWrite(tokenSize))
					{
						// 패킷이 가득 참
						break;
					}
					int tokenHeaderIndex = writer.WriteIndex;

					// Ignore if it doesn't allowed to synchronize
					if (lifeToken.IsOwnerOnly && lifeToken.OwnerID != sendTo)
					{
						lastFieldIndex = 0;
						lifeTokenIndex++;
						continue;
					}

					if (lifeToken.IsCreate)
					{
						writer.OffsetWriteIndex(tokenSize);

						if (!mObjectByID.TryGetValue(lifeToken.ObjectID, out var no))
						{
							// If there is no object don't serialize field, just send life sync token
							writer.WriteAt(lifeToken, tokenHeaderIndex);
							lastFieldIndex = 0;
							lifeTokenIndex++;
							syncCount++;
							continue;
						}

						if (no.TrySerializeEntirePartTo(writer, lastFieldIndex, out lastFieldIndex))
						{
							// Write creation token
							writer.WriteAt(lifeToken, tokenHeaderIndex);
							lastFieldIndex = 0;
							lifeTokenIndex++;
							syncCount++;
							continue;
						}

						// 패킷이 가득 참
						break;
					}
					else
					{
						writer.WriteAt(lifeToken, tokenHeaderIndex);
						lifeTokenIndex++;
						syncCount++;
					}
				}

				if (syncCount > 0)
				{
					// Write packet headers
					baseHeader.PacketHeader = PacketHeaderType.SYN_OBJ_LIFE;
					baseHeader.PacketLength = (ushort)writer.WriteIndex;
					baseHeader.SenderID = sender;
					baseHeader.Timestamp = timestamp;

					syncHeader.Count = syncCount;

					writer.WriteAt(baseHeader, 0);
					writer.WriteAt(syncHeader, baseHeader.GetSyncDataSize());

					// Add packet
					packets.Add(packet);
				}
				else
				{
					packetGroup.ReleaserAction(packet);
				}

				if (lifeTokenIndex >= tokens.Count)
				{
					break;
				}
			}

			if (packets.IsEmpty())
			{
				return false;
			}

			packetGroup.AddPacket(packets);
			return true;
		}

		public void OnSyncObjectLife(in NetPacketReader reader, bool isFromServer, NetSessionID from)
		{
			if (mIsServer)
			{
				//Ulog.LogError(this, $"You try to sync object life cycle. You are the server!");
				return;
			}

			// Parse packet header
			while (true)
			{
				NetSyncHeader header = new NetSyncHeader();
				if (!reader.CanRead(header.GetSyncDataSize()))
				{
					break;
				}

				header.DeserializeFrom(reader);
				reader.BindSyncHeader(header);

				for (int i = 0; i < header.Count; i++)
				{
					NetLifeStreamToken token = new NetLifeStreamToken(reader);

					if (token.IsCreate)
					{
						if (this.mObjectByID.TryGetValue(token.ObjectID, out var exsitObject))
						{
							exsitObject.TryDeserializeFields(reader, isFromServer, from);
							continue;
						}

						// Create network object
						var no = this.createNetworkObject
						(
							token.ObjectType,
							token.ObjectID,
							token.OwnerID,
							token.Position,
							Quaternion.identity,
							out bool isExist
						);

						no?.TryDeserializeFields(reader, isFromServer, from);

						if (!isExist)
						{
							no.Common_OnStart();

							if (mIsServer)
							{
								no.Server_OnStart();
							}

							if (mIsClient)
							{
								no.Client_OnStart();
							}

							OnCreated?.Invoke(no);
						}
					}
					else
					{
						if (mObjectByID.TryGetValue(token.ObjectID, out var no))
						{
							no.Release();
						}
					}
				}
			}
		}

		public void OnSyncObjectField(in NetPacketReader reader, bool isFromServer, NetSessionID from)
		{
			// Parse packet header
			while (true)
			{
				NetSyncHeader header = new NetSyncHeader();
				if (!reader.CanRead(header.GetSyncDataSize()))
				{
					break;
				}

				header.DeserializeFrom(reader);
				reader.BindSyncHeader(header);

				for (int i = 0; i < header.Count; i++)
				{
					NetObjectID objectID = new NetObjectID(reader);

					if (mObjectByID.TryGetValue(objectID, out var no))
					{
						no.TryDeserializeFields(reader, isFromServer, from);
					}
					else
					{
						//Ulog.LogError(this, $"Synchornize error! There is no NetworkObject id : {objectID}");
						return;
					}
				}
			}
		}

		public void OnSyncObjectRPCs(in NetPacketReader reader, bool isFromServer, NetSessionID from)
		{
			while (true)
			{
				NetSyncHeader header = new NetSyncHeader();
				if (!reader.CanRead(header.GetSyncDataSize()))
				{
					break;
				}
				header.DeserializeFrom(reader);
				reader.BindSyncHeader(header);

				for (int i = 0; i < header.Count; i++)
				{
					NetObjectID objectID = new NetObjectID(reader);

					if (mObjectByID.TryGetValue(objectID, out var no))
					{
						no.TryDeserializeRPCs(reader, isFromServer, from);
					}
					else
					{
						//Ulog.LogError(this, $"Synchornize error! There is no NetworkObject id : {objectID}");
						return;
					}
				}
			}
		}

		#endregion
	}
}
