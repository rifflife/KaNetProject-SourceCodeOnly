using KaNet.Core;
using KaNet.Synchronizers.Prebinder;
using KaNet.Utils;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Utils;

namespace KaNet.Synchronizers
{
	public abstract class NetworkObject : MonoBehaviour
	{
		/// <summary>네트워크 객체를 나타내는 타입입니다. 타입은 클래스 별로 고유해야합니다.</summary>
		[TitleGroup("Network Object")][ShowInInspector] public abstract NetObjectType Type { get; }
		[TitleGroup("Network Object")][ShowInInspector] public int ObjectID => ID;
		[TitleGroup("Network Object")][ShowInInspector] public int ObjectOwnerID => OwnerID;

		/// <summary>네트워크 객체의 ID 입니다.</summary>
		public NetObjectID ID { get; private set; }
		/// <summary>네트워크 객체의 소유자 ID 입니다.</summary>
		public NetSessionID OwnerID { get; private set; }
		/// <summary>현재 클라이언트의 ID입니다.</summary>
		public NetSessionID ClientID { get; private set; }
		public bool IsOwner { get; private set; }

		//전역 상태를 나타냅니다. 전역인 경우 공간분할과 관계없이 동기화됩니다.
		//public abstract bool IsGlobal { get; }

		/// <summary>소유자에게만 동기화되는지 여부입니다.</summary>
		public virtual bool IsOwnerOnly => false;
		public bool KeepAlive { get; set; }
		/// <summary>현재 서버측에서 실행중인지 여부입니다.</summary>
		public bool IsServerSide { get; private set; }
		/// <summary>현재 클라이언트측에서 실행중인지 여부입니다.</summary>
		public bool IsClientSide { get; private set; }

		/// <summary>네트워크 객체가 해제되었을 때 호출됩니다. 호출 뒤에는 등록된 이벤트가 해제됩니다.</summary>
		public event Action<NetworkObject> OnRelease;
		private Action<NetworkObject> mReleaseAction;

		public virtual void Start()
		{
			Common_OnAfterStart();

			if (IsServerSide)
			{
				Server_OnAfterStart();
			}
			else
			{
				Client_OnAfterStart();
			}
		}

		// DI
		public NetworkObjectManager ObjectManager { get; private set; }

		// Synchronize Fields
		private Dictionary<SyncType, List<Synchronizer>> mSyncFieldBySyncType = new()
		{
			{ SyncType.ReliableFixed, new List<Synchronizer>() },
			{ SyncType.ReliableInstant, new List<Synchronizer>() },
			{ SyncType.UnreliableFixed, new List<Synchronizer>() },
			{ SyncType.UnreliableInstant, new List<Synchronizer>() }
		};
		private List<Synchronizer> mSyncFields = new();

		private Dictionary<SyncType, bool> mIsFieldChanged= new()
		{
			{ SyncType.ReliableFixed, false },
			{ SyncType.ReliableInstant, false },
			{ SyncType.UnreliableFixed, false },
			{ SyncType.UnreliableInstant, false }
		};

		public int SyncFieldCount => mSyncFields.Count;

		// Synchronize RPCs
		private Dictionary<SyncType, List<RpcBase>> mRpcFieldBySyncType = new()
		{
			{ SyncType.ReliableFixed, new List<RpcBase>() },
			{ SyncType.ReliableInstant, new List<RpcBase>() },
			{ SyncType.UnreliableFixed, new List<RpcBase>() },
			{ SyncType.UnreliableInstant, new List<RpcBase>() }
		};
		private List<RpcBase> mSyncRPCs = new();

		private Dictionary<SyncType, bool> mIsRPCsCalled = new()
		{
			{ SyncType.ReliableFixed, false },
			{ SyncType.ReliableInstant, false },
			{ SyncType.UnreliableFixed, false },
			{ SyncType.UnreliableInstant, false }
		};
		public int SyncRPCsCount => mSyncRPCs.Count;

		public void SetInitialInfo(NetObjectID id, NetSessionID ownerID, NetSessionID clientID)
		{
			ID = id;
			OwnerID = ownerID;
			ClientID = clientID;
			IsOwner = OwnerID == ClientID;
		}

		public bool CheckOwnerByID(NetSessionID sessionID)
		{
			return OwnerID == sessionID;
		}

		private bool mIsPrebinded = false;
		public void InitializeByManager
		(
			NetworkObjectManager manager, 
			Action<NetworkObject> releaseAction, 
			bool isServerSide,
			bool isClientSide
		)
		{
			// Prevent 
			if (mIsPrebinded)
			{
				return;
			}

			mIsPrebinded = true;

			ObjectManager = manager;
			mReleaseAction = releaseAction;
			IsServerSide = isServerSide;
			IsClientSide = isClientSide;

			// Initialize synchronize field
			for (int i = 0; i < mSyncFields.Count; i++)
			{
				var syncField = mSyncFields[i];

				syncField.BindIndex(i);
				syncField.ResetOnDataChangeEvent();

				if ((syncField.SyncAuthority == SyncAuthority.None) ||
					(syncField.SyncAuthority == SyncAuthority.ServerOnly && IsServerSide) ||
					(syncField.SyncAuthority == SyncAuthority.OwnerBroadcast && (IsOwner || IsServerSide)) ||
					(syncField.SyncAuthority == SyncAuthority.OwnerToServer && IsOwner))
				{
					syncField.OnChanged += () =>
					{
						mIsFieldChanged[syncField.SyncType] = true;
					};
				}

				if (!mSyncFieldBySyncType.ContainsKey(syncField.SyncType))
				{
					mSyncFieldBySyncType.Add(syncField.SyncType, new List<Synchronizer>());
				}

				mSyncFieldBySyncType[syncField.SyncType].Add(syncField);
			}

			// Initialize RPC field
			for (int i = 0; i < mSyncRPCs.Count; i++)
			{
				var syncRPC = mSyncRPCs[i];

				syncRPC.BindIndex(i);
				syncRPC.ResetOnCalledEvent();

				if ((syncRPC.SyncAuthority == SyncAuthority.None) || 
					(syncRPC.SyncAuthority == SyncAuthority.ServerOnly && IsServerSide) ||
					(syncRPC.SyncAuthority == SyncAuthority.OwnerToServer && IsOwner))
				{
					syncRPC.OnCalled += () =>
					{
						mIsRPCsCalled[syncRPC.SyncType] = true;
					};
				}

				if (!mRpcFieldBySyncType.ContainsKey(syncRPC.SyncType))
				{
					mRpcFieldBySyncType.Add(syncRPC.SyncType, new List<RpcBase>());
				}

				mRpcFieldBySyncType[syncRPC.SyncType].Add(syncRPC);
			}
		}

		/// <summary>네트워크 객체를 제거합니다.</summary>
		public void Release()
		{
			OnRelease?.Invoke(this);
			OnRelease = null;
			mReleaseAction?.Invoke(this);
		}

		#region Events

		/// <summary>
		/// 서버와 클라이언트측에서 생성되었을 때 공통적으로 호출됩니다. 
		/// 모든 이벤트 중에 가장 먼저 호출됩니다.
		/// </summary>
		public virtual void Common_OnStart() { }

		/// <summary>서버와 클라이언트측에서 생성되었을 때 호출됩니다. Start함수와 유사합니다.</summary>
		public virtual void Common_OnAfterStart() { }

		/// <summary>
		/// 서버와 클라이언트측에서 소멸되었을 때 공통적으로 호출됩니다.
		/// 모든 이벤트 중에 가장 마지막에 호출됩니다.
		/// </summary>
		public virtual void Common_OnDestroy() { }

		/// <summary>서버측에서 생성되었을 때 호출됩니다. Start함수와 유사합니다.</summary>
		public virtual void Server_OnStart() { }

		/// <summary>서버측에서 생성된 뒤 호출됩니다. OnStart 함수 뒤에 호출됩니다.</summary>
		public virtual void Server_OnAfterStart() { }

		/// <summary>서버측에서 소멸되었을 때 호출됩니다. OnDestroy함수와 유사합니다.</summary>
		public virtual void Server_OnDestroy() { }

		/// <summary>서버측에서 Update될 때 호출됩니다. Update함수와 유사합니다.</summary>
		public virtual void Server_OnUpdate(in DeltaTimeInfo deltaTimeInfo) { }

		/// <summary>서버측에서 FixedUpdate될 때 호출됩니다. FixedUpdate함수와 유사합니다.</summary>
		public virtual void Server_OnFixedUpdate(in DeltaTimeInfo deltaTimeInfo) { }

		/// <summary>
		/// 서버측에서 NetworkTickUpdate직전에 호출됩니다.
		/// NetworkTickUpdate시 객체가 직렬화됩니다.
		/// </summary>
		public virtual void Server_OnBeforeNetworkTickUpdate() { }

		/// <summary>클라이언트측에서 생성되었을 때 호출됩니다. Start함수와 유사합니다.</summary>
		public virtual void Client_OnStart() { }

		/// <summary>클라이언트측에서 생성된 뒤 호출됩니다. OnStart 함수 뒤에 호출됩니다.</summary>
		public virtual void Client_OnAfterStart() { }

		/// <summary>클라이언트측에서 소멸되었을 때 호출됩니다. OnDestroy함수와 유사합니다.</summary>
		public virtual void Client_OnDestroy() { }

		/// <summary>클라이언트측에서 Update될 때 호출됩니다. Update함수와 유사합니다.</summary>
		public virtual void Client_OnUpdate(in DeltaTimeInfo deltaTimeInfo) { }

		/// <summary>클라이언트측에서 FixedUpdate될 때 호출됩니다. FixedUpdate함수와 유사합니다.</summary>
		public virtual void Client_OnFixedUpdate(in DeltaTimeInfo deltaTimeInfo) { }

		/// <summary>
		/// 클라이언트측에서 NetworkTickUpdate직전에 호출됩니다.
		/// NetworkTickUpdate시 객체가 직렬화됩니다.
		/// </summary>
		public virtual void Client_OnBeforeNetworkTickUpdate() { }

		public bool IsFieldChanged(SyncType syncType)
		{
			return mIsFieldChanged[syncType];
		}

		public bool IsRPCsCalled(SyncType syncType)
		{
			return mIsRPCsCalled[syncType];
		}

		public void OnFieldSerialized(SyncType syncType)
		{
			mIsFieldChanged[syncType] = false;
			foreach (var syncList in mSyncFieldBySyncType[syncType])
			{
				syncList.OnSeralized();
			}
		}

		public void OnRPCsSerialized(SyncType syncType)
		{
			mIsRPCsCalled[syncType] = false;
			foreach (var syncList in mRpcFieldBySyncType[syncType])
			{
				syncList.OnSerialized();
			}
		}

		#endregion

		#region Deserialize

		/// <summary>필드를 역직렬화 합니다.</summary>
		/// <param name="isFromServer">서버에서 송신되었는지 여부입니다.</param>
		/// <param name="from">패킷을 송신한 대상입니다.</param>
		/// <returns>직렬화에 실패하면 false를 반환합니다.</returns>
		public bool TryDeserializeFields(in NetPacketReader reader, bool isFromServer, NetSessionID from)
		{
			bool isOwned = CheckOwnerByID(from);

			if (!reader.TryReadUInt8(out var fieldCount))
			{
				Ulog.LogError(this, $"Deserialize fields failed! There is no count! Network Object : {this}");
				return false;
			}

			for (int i = 0; i < fieldCount; i++)
			{
				if (!reader.TryReadUInt8(out var syncIndex))
				{
					Ulog.LogError(this, $"Deserialize fields failed! There is no index! Network Object : {this}");
					return false;
				}

				Synchronizer field;

				try
				{
					field = mSyncFields[syncIndex]; 
				}
				catch 
				{
					throw new SyncIndexError(this, syncIndex); 
				}

				try
				{
					if (!ShouldDeserialize(field.SyncAuthority, isOwned, isFromServer))
					{
						field.IgnoreDeserialize(reader);
						continue;
					}
				}
				catch
				{
					throw new SyncIgnoreDeserializeFieldError(this, field);
				}

				try
				{
					field.DeserializeFrom(reader);

					if (field.SyncAuthority == SyncAuthority.OwnerBroadcast && IsServerSide)
					{
						field.SetBroadcast();
					}
				}
				catch
				{
					throw new SyncDeserializeFieldError(this, field);
				}
			}

			return true;
		}

		/// <summary>RPC를 역직렬화 합니다.</summary>
		/// <param name="isFromServer">서버에서 송신되었는지 여부입니다.</param>
		/// <param name="from">패킷을 송신한 대상입니다.</param>
		/// <returns>직렬화에 실패하면 false를 반환합니다.</returns>
		public bool TryDeserializeRPCs(in NetPacketReader reader, bool isFromServer, NetSessionID from)
		{
			bool isOwned = CheckOwnerByID(from);

			if (!reader.TryReadUInt8(out var rpcCount))
			{
				Ulog.LogError(this, $"Deserialize RPCs failed! There is no count! Network Object : {this}");
				return false;
			}

			for (int i = 0; i < rpcCount; i++)
			{
				if (!reader.TryReadUInt8(out var rpcIndex))
				{
					Ulog.LogError(this, $"Deserialize RPCs failed! There is no index! Network Object : {this}");
					return false;
				}

				RpcBase rpc;
				NetUInt16 count;

				try
				{
					rpc = mSyncRPCs[rpcIndex];
				}
				catch
				{
					throw new SyncIndexError(this, rpcIndex);
				}

				try
				{
					count = new NetUInt16(reader);
				}
				catch
				{
					throw new SyncCountParseError();
				}

				try
				{
					if (!ShouldDeserialize(rpc.SyncAuthority, isOwned, isFromServer))
					{
						for (int c = 0; c < count; c++)
						{
							rpc.IgnoreDeserialize(reader);
						}
						continue;
					}
				}
				catch (Exception e)
				{
					Ulog.LogError(e);
					throw new SyncIgnoreDeserializeRpcError(this, rpc);
				}

				try
				{
					for (int c = 0; c < count; c++)
					{
						rpc.Deserialize(reader);
					}
				}
				catch (Exception e)
				{
					Ulog.LogError(e);
					throw new SyncDeserializeRpcError(this, rpc);
				}
			}

			return true;
		}

		#endregion

		#region Serialize

		/// <summary>변경된 부분만 직렬화합니다.</summary>
		/// <param name="isByServer">서버로 부터 호출되었는지 여부입니다.</param>
		/// <param name="bySessionID">호출한 대상입니다.</param>
		/// <param name="startPropertyIndex">직렬화를 시작할 Index입니다.</param>
		/// <param name="lastPropertyIndex">마지막으로 직렬화 Index입니다.</param>
		/// <returns>직렬화가 완료되면 true를 반환합니다. 아직 직렬화할 데이터가 있다면 false를 반환합니다.</returns>
		public bool TrySerializeChangedPartTo
		(
			in NetPacketWriter writer,
			SyncType syncType,
			bool isByServer,
			NetSessionID bySessionID,
			bool isEcho,
			int startPropertyIndex, 
			out int lastPropertyIndex
		)
		{
			// Offset Counter
			if (!writer.CanWrite(1))
			{
				lastPropertyIndex = startPropertyIndex;
				return false;
			}

			NetUInt8 count = 0;
			int counterIndex = writer.WriteIndex;
			writer.OffsetWriteIndex(1);

			// Get targets
			bool isOwned = CheckOwnerByID(bySessionID);
			var syncFields = mSyncFieldBySyncType[syncType];

			// Serialize
			for (int i = startPropertyIndex; i < syncFields.Count; i++)
			{
				var field = syncFields[i];

				if ((!field.IsDirty))
				{
					continue;
				}

				if (!ShouldSerializeField(field.SyncAuthority, isByServer, isOwned, isEcho))
				{
					continue;
				}

				if (!writer.CanWrite(field.GetSyncDataSize()))
				{
					lastPropertyIndex = i;
					return false;
				}

				field.SerializeChangedPartTo(writer);
				count++;
			}

			// Serialize count even 0
			writer.WriteAt(count, counterIndex);

			lastPropertyIndex = -1;
			return true;
		}

		/// <summary>모든 데이터 영역을 직렬화합니다.</summary>
		/// <param name="startPropertyIndex">직렬화를 시작할 Index입니다.</param>
		/// <param name="lastPropertyIndex">마지막으로 직렬화 Index입니다.</param>
		/// <returns>직렬화가 완료되면 true를 반환합니다. 아직 직렬화할 데이터가 있다면 false를 반환합니다.</returns>
		public bool TrySerializeEntirePartTo
		(
			in NetPacketWriter writer,
			int startPropertyIndex, 
			out int lastPropertyIndex
		)
		{
			// Offset Counter
			if (!writer.CanWrite(1))
			{
				lastPropertyIndex = startPropertyIndex;
				return false;
			}

			// Get targets
			NetUInt8 count = 0;
			int counterIndex = writer.WriteIndex;
			writer.OffsetWriteIndex(1);

			// Serialize
			for (int i = startPropertyIndex; i < mSyncFields.Count; i++)
			{
				if (!writer.CanWrite(mSyncFields[i].GetEntireDataSize()))
				{
					lastPropertyIndex = i;
					return false;
				}

				mSyncFields[i].SerializeEntirelyTo(writer);
				count++;
			}

			// Serialize count even 0
			writer.WriteAt(count, counterIndex);

			lastPropertyIndex = -1;
			return true;
		}

		/// <summary>RPC를 직렬화합니다.</summary>
		/// <returns>직렬화가 완료되면 true를 반환합니다. 아직 직렬화할 데이터가 있다면 false를 반환합니다.</returns>
		public bool TrySerializeRpcCallData
		(
			in NetPacketWriter writer,
			SyncType syncType,
			bool isByServer,
			NetSessionID bySessionID,
			NetSessionID sendTo,
			bool isEcho,
			int startRpcIndex,
			int startRpcCallIndex,
			out int lastRpcIndex,
			out int lastRpcCallIndex
		)
		{
			// Offset Counter
			if (!writer.CanWrite(1))
			{
				lastRpcIndex = startRpcIndex;
				lastRpcCallIndex = startRpcCallIndex;
				return false;
			}

			NetUInt8 rpcCount = 0;
			int counterIndex = writer.WriteIndex;
			writer.OffsetWriteIndex(rpcCount.GetSyncDataSize());

			// Get targets
			bool isOwned = CheckOwnerByID(bySessionID);
			var rpcFields = mRpcFieldBySyncType[syncType];

			// Serialize
			for (int i = startRpcIndex; i < rpcFields.Count; i++)
			{
				var rpc = rpcFields[i];

				if ((!rpc.HasCallData))
				{
					continue;
				}

				if (!ShouldSerializeRPC(rpc.SyncAuthority, isByServer, isOwned))
				{
					continue;
				}

				if (!rpc.TrySerializeCallData(writer, startRpcCallIndex, sendTo, out lastRpcCallIndex))
				{
					lastRpcIndex = i;
					writer.WriteAt(rpcCount, counterIndex);
					return false;
				}

				rpcCount++;
			}

			// Serialize count even 0
			writer.WriteAt(rpcCount, counterIndex);

			lastRpcIndex = -1;
			lastRpcCallIndex = -1;
			return true;
		}

		#endregion

		public override string ToString()
		{
			return $"{Type} : [ID : {ID}][Owner : {OwnerID}]";
		}

		public static bool ShouldSerializeField
		(
			SyncAuthority authority, 
			bool isByServer, 
			bool isOwned, 
			bool isEcho
		)
		{
			switch (authority)
			{
				case SyncAuthority.ServerOnly:
					if (isByServer)
					{
						return !isEcho;
					}
					else
					{
						return false;
					}

				case SyncAuthority.OwnerToServer:
					if (!isOwned)
					{
						return false;
					}

					if (isByServer)
					{
						return !isEcho;
					}
					else
					{
						return true;
					}

				case SyncAuthority.OwnerBroadcast:
					// 서버에서 보내는 경우 자기 자신을 제외한 모두에게 보낸다.
					if (isByServer)
					{
						return !isEcho;
					}
					// 클라이언트에서 보내는 경우 소유자인 경우만 보낸다.
					else
					{
						return isOwned;
					}
			}

			return true;
		}

		public static bool ShouldSerializeRPC
		(
			SyncAuthority authority,
			bool isByServer,
			bool isOwned
		)
		{
			switch (authority)
			{
				case SyncAuthority.ServerOnly:
					return isByServer;

				case SyncAuthority.OwnerToServer:
					return isOwned;

				case SyncAuthority.OwnerBroadcast:
					throw new AuthorityError("RPC는 Broadcast할 수 없습니다.");
			}

			return true;
		}

		public static bool ShouldDeserialize(SyncAuthority authority, bool isOwned, bool isFromServer)
		{
			switch (authority)
			{
				case SyncAuthority.ServerOnly:
					// 서버에서 온 경우만 동기화한다.
					return isFromServer;

				case SyncAuthority.OwnerToServer:
					// 소유자에게서 온 경우만 동기화한다.
					return isOwned || isFromServer;

				case SyncAuthority.OwnerBroadcast:
					// 소유자에게서 온 경우만 동기화한다.
					return isOwned || isFromServer;
			}

			return true;
		}
	}
}
