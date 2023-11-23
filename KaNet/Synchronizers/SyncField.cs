using System;
using KaNet.Utils;
using UnityEngine;

namespace KaNet.Synchronizers
{
	[Serializable]
	public class SyncField<T> : Synchronizer where T : struct, INetworkSerializable
	{
		public override event Action OnChanged;
		public event Action<T> OnDeserialized;

		public SyncField(T value = default(T))
		{
			mData = value;
		}

		public override bool IsDirty { get; protected set; }

		[SerializeField]
		protected T mData;
		public T Data
		{
			get => mData;
			set
			{
				if (!mData.Equals(value))
				{
					mData = value;
					IsDirty = true;
					OnChanged?.Invoke();
				}
			}
		}

		public override void ResetDeserializeEvent()
		{
			OnDeserialized = null;
		}

		public override void ResetOnDataChangeEvent()
		{
			OnChanged = null;
		}

		protected override int getSyncDataSize() => mData.GetSyncDataSize();
		protected override int getEntireDataSize() => mData.GetSyncDataSize();

		public override void OnSeralized()
		{
			if (NeedToBroadcast)
			{
				NeedToBroadcast = false;
				IsDirty = true;
				OnChanged?.Invoke();
			}
			else
			{
				IsDirty = false;
			}
		}

		public override void DeserializeFrom(in NetPacketReader reader)
		{
			mData.DeserializeFrom(reader);
			OnDeserialized?.Invoke(mData);
		}

		public override void IgnoreDeserialize(in NetPacketReader reader)
		{
			reader.OffsetReadIndex(mData.GetSyncDataSize());
		}

		protected override void internalSerializeChangedPartTo(in NetPacketWriter writer)
		{
			mData.SerializeTo(writer);
		}

		protected override void internalSerializeEntirelyTo(in NetPacketWriter writer)
		{
			mData.SerializeTo(writer);
		}

		public override string ToString()
		{
			return mData.ToString();
		}
	}

	/// <summary>최신 데이터로만 동기화받습니다. 서버의 Timestamp영향을 받습니다.</summary>
	[Serializable]
	public class SyncFieldByOrder<T> : Synchronizer where T : struct, INetworkSerializable
	{
		public override event Action OnChanged;
		public event Action<T> OnDeserialized;
		public NetTimestamp LastTimestamp { get; private set; }

		public SyncFieldByOrder(T value = default(T))
		{
			mData = value; 
			ResetTimestamp();
		}

		public override bool IsDirty { get; protected set; }

		[SerializeField]
		protected T mData;
		public T Data
		{
			get => mData;
			set
			{
				if (!mData.Equals(value))
				{
					mData = value;
					IsDirty = true;
					OnChanged?.Invoke();
				}
			}
		}

		public void ResetTimestamp()
		{
			LastTimestamp = -1;
		}

		public override void ResetDeserializeEvent()
		{
			OnDeserialized = null;
		}

		public override void ResetOnDataChangeEvent()
		{
			OnChanged = null;
		}

		protected override int getSyncDataSize() => mData.GetSyncDataSize();
		protected override int getEntireDataSize() => mData.GetSyncDataSize();

		public override void OnSeralized()
		{
			if (NeedToBroadcast)
			{
				NeedToBroadcast = false;
				IsDirty = true;
				OnChanged?.Invoke();
			}
			else
			{
				IsDirty = false;
			}
		}

		public override void DeserializeFrom(in NetPacketReader reader)
		{
			var serverTs = reader.PacketNetBaseHeader.Timestamp;

			if (LastTimestamp > serverTs)
			{
				T temp = default(T);
				temp.DeserializeFrom(reader);
				return;
			}

			LastTimestamp = serverTs;

			mData.DeserializeFrom(reader);
			OnDeserialized?.Invoke(mData);
		}

		public override void IgnoreDeserialize(in NetPacketReader reader)
		{
			reader.OffsetReadIndex(mData.GetSyncDataSize());
		}

		protected override void internalSerializeChangedPartTo(in NetPacketWriter writer)
		{
			mData.SerializeTo(writer);
		}

		protected override void internalSerializeEntirelyTo(in NetPacketWriter writer)
		{
			mData.SerializeTo(writer);
		}

		public override string ToString()
		{
			return mData.ToString();
		}
	}
}
