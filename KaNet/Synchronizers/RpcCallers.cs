using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KaNet.Core;
using KaNet.Synchronizers.Prebinder;
using KaNet.Utils;
using Utils;

namespace KaNet.Synchronizers
{
	public class RpcCallData : INetworkSerializable
	{
		public NetPacket CallData;
		private NetSessionID[] DestinationList = null;

		public RpcCallData(int maxSize)
		{
			CallData = new NetPacket(maxSize);
		}

		/// <summary>보낼 대상을 설정합니다.</summary>
		/// <param name="destinations">보낼 대상입니다. 아무 설정을 하지 않으면 모든 대상에게 전송합니다.</param>
		public void SetDestination(params NetSessionID[] destinations)
		{
			DestinationList = destinations;
		}

		/// <summary>보낼 대상인지 확인합니다.</summary>
		/// <param name="sendTo">목적지입니다.</param>
		/// <returns>보낼 대상이라면 true를 반환합니다.</returns>
		public bool IsDestination(NetSessionID sendTo)
		{
			if (DestinationList == null || DestinationList.IsEmpty())
			{
				return true;
			}
			
			return DestinationList.Contains(sendTo);
		}

		public int GetEntireDataSize()
		{
			return CallData.Size;
		}

		public int GetSyncDataSize()
		{
			return CallData.Size;
		}

		public void DeserializeFrom(in NetPacketReader reader)
		{
			throw new NotImplementedException();
		}

		public void SerializeTo(in NetPacketWriter writer)
		{
			writer.WritePacket(CallData);
		}
	}

	public class RpcBase
	{
		public event Action OnCalled;
		public byte SyncIndex { get; private set; }
		public SyncType SyncType { get; private set; }
		public SyncAuthority SyncAuthority { get; private set; }

		private NetPacketWriter mRpcDataWriter = new NetPacketWriter();
		private List<RpcCallData> mRpcCallList = new();
		public bool HasCallData => mRpcCallList.Count > 0;

		public void SetNetworkOption(SyncType syncType, SyncAuthority syncAuthority)
		{
			SyncType = syncType;
			SyncAuthority = syncAuthority;
		}
		public virtual void BindFunction(object referencFunction) { }

		public void BindIndex(int index)
		{
			SyncIndex = (byte)index;
		}

		public void ResetOnCalledEvent()
		{
			OnCalled = null;
		}

		public void ResetEvent()
		{
			OnCalled = null;
		}

		protected RpcCallData getInternalRpcCallData(int dataSize, out NetPacketWriter rpcDataWriter)
		{
			//RpcCallData data = new RpcCallData(dataSize + 1);
			RpcCallData data = new RpcCallData(dataSize);
			mRpcDataWriter.SetNetPacket(data.CallData);
			//mRpcDataWriter.WriteUInt8(SyncIndex);
			rpcDataWriter = mRpcDataWriter;
			return data;
		}

		protected void addInternalRpcCallData(RpcCallData rpcCallData)
		{
			mRpcCallList.Add(rpcCallData);
			OnCalled?.Invoke();
		}

		/// <summary>호출된 RPC 데이터를 직렬화합니다.</summary>
		/// <param name="isRemaining">함수 호출 후 아직 호출 데이터가 남아있는지 여부입니다.</param>
		/// <returns>
		/// 직렬화가 완료되었으면 true를 반환합니다.
		/// 직렬화 할 데이터가 남아있고 패킷에 더 쓸 수 없으면 false를 반환합니다.
		/// </returns>
		public bool TrySerializeCallData(NetPacketWriter writer, int startIndex, NetSessionID sendTo, out int endIndex)
		{
			if (startIndex >= mRpcCallList.Count)
			{
				endIndex = -1;
				return true;
			}

			NetUInt8 rpcIndex = this.SyncIndex;
			if (!writer.CanWrite(rpcIndex.GetSyncDataSize()))
			{
				endIndex = startIndex;
				return false;
			}
			int rpcHeaderIndex = writer.WriteIndex;
			writer.OffsetWriteIndex(rpcIndex.GetSyncDataSize());

			NetUInt16 callCount = 0;
			if (!writer.CanWrite(callCount.GetSyncDataSize()))
			{
				endIndex = startIndex;
				return false;
			}
			int countIndex = writer.WriteIndex;
			writer.OffsetWriteIndex(callCount.GetSyncDataSize());

			for (int i = startIndex; i < mRpcCallList.Count; i++)
			{
				if (!mRpcCallList[i].IsDestination(sendTo))
				{
					continue;
				}

				var callData = mRpcCallList[i].CallData;

				if (!writer.CanWrite(callData.Size))
				{
					endIndex = i;
					writer.WriteAt(rpcIndex, rpcHeaderIndex);
					writer.WriteAt(callCount, countIndex);
					return false;
				}

				writer.WritePacket(callData);
				callCount++;
			}

			endIndex = -1;
			writer.WriteAt(rpcIndex, rpcHeaderIndex);
			writer.WriteAt(callCount, countIndex);
			return true;
		}

		public void OnSerialized()
		{
			this.mRpcCallList.Clear();
		}

		public virtual void Deserialize(NetPacketReader reader) { }

		public virtual void IgnoreDeserialize(NetPacketReader reader) { }
	}

	public class RpcCaller : RpcBase
	{
		private Action mReferenceFunction;

		public override void BindFunction(object referenceFunction)
		{
			mReferenceFunction = (Action)referenceFunction;
		}

		public void Invoke(params NetSessionID[] destinations)
		{
			var rpcData = getInternalRpcCallData(0, out var writer);
			rpcData.SetDestination(destinations);
			addInternalRpcCallData(rpcData);
		}

		public override void Deserialize(NetPacketReader reader)
		{
			mReferenceFunction.Invoke();
		}
	}

	public class RpcCaller<T> : RpcBase
		where T : struct, INetworkSerializable
	{
		private Action<T> mReferenceFunction;

		public override void BindFunction(object referenceFunction)
		{
			mReferenceFunction = (Action<T>)referenceFunction;
		}

		public void Invoke(in T arg, params NetSessionID[] destinations)
		{
			int argumentSize = arg.GetSyncDataSize();
			var rpcData = getInternalRpcCallData(argumentSize, out var writer);

			rpcData.SetDestination(destinations);

			arg.SerializeTo(writer);
			addInternalRpcCallData(rpcData);
		}

		public override void Deserialize(NetPacketReader packetReader)
		{
			T arg = new();
			arg.DeserializeFrom(packetReader);
			mReferenceFunction.Invoke(arg);
		}

		public override void IgnoreDeserialize(NetPacketReader reader)
		{
			T arg = new();
			reader.OffsetReadIndex(arg.GetSyncDataSize());
		}
	}

	public class RpcCaller<T0, T1> : RpcBase
		where T0 : struct, INetworkSerializable
		where T1 : struct, INetworkSerializable
	{
		private Action<T0, T1> mReferenceFunction;

		public override void BindFunction(object referenceFunction)
		{
			mReferenceFunction = (Action<T0, T1>)referenceFunction;
		}

		public void Invoke(in T0 arg0, in T1 arg1, params NetSessionID[] destinations)
		{
			int size0 = arg0.GetSyncDataSize();
			int size1 = arg1.GetSyncDataSize();

			var rpcData = getInternalRpcCallData(size0 + size1, out var writer);
			rpcData.SetDestination(destinations);

			arg0.SerializeTo(writer);
			arg1.SerializeTo(writer);

			addInternalRpcCallData(rpcData);
		}

		public override void Deserialize(NetPacketReader packetReader)
		{
			T0 arg0 = new();
			T1 arg1 = new();

			arg0.DeserializeFrom(packetReader);
			arg1.DeserializeFrom(packetReader);

			mReferenceFunction.Invoke(arg0, arg1);
		}

		public override void IgnoreDeserialize(NetPacketReader reader)
		{
			T0 arg0 = new();
			T1 arg1 = new();

			reader.OffsetReadIndex(arg0.GetSyncDataSize());
			reader.OffsetReadIndex(arg1.GetSyncDataSize());
		}
	}

	public class RpcCaller<T0, T1, T2> : RpcBase
		where T0 : struct, INetworkSerializable
		where T1 : struct, INetworkSerializable
		where T2 : struct, INetworkSerializable
	{
		private Action<T0, T1, T2> mReferenceFunction;

		public override void BindFunction(object referenceFunction)
		{
			mReferenceFunction = (Action<T0, T1, T2>)referenceFunction;
		}

		public void Invoke(in T0 arg0, in T1 arg1, in T2 arg2, params NetSessionID[] destinations)
		{
			int size0 = arg0.GetSyncDataSize();
			int size1 = arg1.GetSyncDataSize();
			int size2 = arg2.GetSyncDataSize();

			var rpcData = getInternalRpcCallData(size0 + size1 + size2, out var writer);

			arg0.SerializeTo(writer);
			arg1.SerializeTo(writer);
			arg2.SerializeTo(writer);

			addInternalRpcCallData(rpcData);
		}

		public override void Deserialize(NetPacketReader packetReader)
		{
			T0 arg0 = new();
			T1 arg1 = new();
			T2 arg2 = new();

			arg0.DeserializeFrom(packetReader);
			arg1.DeserializeFrom(packetReader);
			arg2.DeserializeFrom(packetReader);

			mReferenceFunction.Invoke(arg0, arg1, arg2);
		}

		public override void IgnoreDeserialize(NetPacketReader reader)
		{
			T0 arg0 = new();
			T1 arg1 = new();
			T2 arg2 = new();

			reader.OffsetReadIndex(arg0.GetSyncDataSize());
			reader.OffsetReadIndex(arg1.GetSyncDataSize());
			reader.OffsetReadIndex(arg2.GetSyncDataSize());
		}
	}

	public class RpcCaller<T0, T1, T2, T3> : RpcBase
		where T0 : struct, INetworkSerializable
		where T1 : struct, INetworkSerializable
		where T2 : struct, INetworkSerializable
		where T3 : struct, INetworkSerializable
	{
		private Action<T0, T1, T2, T3> mReferenceFunction;

		public override void BindFunction(object referenceFunction)
		{
			mReferenceFunction = (Action<T0, T1, T2, T3>)referenceFunction;
		}

		public void Invoke(in T0 arg0, in T1 arg1, in T2 arg2, in T3 arg3, params NetSessionID[] destinations)
		{
			int size0 = arg0.GetSyncDataSize();
			int size1 = arg1.GetSyncDataSize();
			int size2 = arg2.GetSyncDataSize();
			int size3 = arg3.GetSyncDataSize();

			var rpcData = getInternalRpcCallData(size0 + size1 + size2 + size3, out var writer);
			rpcData.SetDestination(destinations);

			arg0.SerializeTo(writer);
			arg1.SerializeTo(writer);
			arg2.SerializeTo(writer);
			arg3.SerializeTo(writer);

			addInternalRpcCallData(rpcData);
		}

		public override void Deserialize(NetPacketReader packetReader)
		{
			T0 arg0 = new();
			T1 arg1 = new();
			T2 arg2 = new();
			T3 arg3 = new();

			arg0.DeserializeFrom(packetReader);
			arg1.DeserializeFrom(packetReader);
			arg2.DeserializeFrom(packetReader);
			arg3.DeserializeFrom(packetReader);

			mReferenceFunction.Invoke(arg0, arg1, arg2, arg3);
		}

		public override void IgnoreDeserialize(NetPacketReader reader)
		{
			T0 arg0 = new();
			T1 arg1 = new();
			T2 arg2 = new();
			T3 arg3 = new();

			reader.OffsetReadIndex(arg0.GetSyncDataSize());
			reader.OffsetReadIndex(arg1.GetSyncDataSize());
			reader.OffsetReadIndex(arg2.GetSyncDataSize());
			reader.OffsetReadIndex(arg3.GetSyncDataSize());
		}
	}
}
