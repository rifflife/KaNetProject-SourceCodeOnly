using System;
using KaNet.Utils;

namespace KaNet.Synchronizers
{
	public abstract class Synchronizer
	{
		public abstract event Action OnChanged;

		public byte SyncIndex { get; private set; }
		public SyncType SyncType { get; private set; }
		public SyncAuthority SyncAuthority { get; private set; }

		public abstract bool IsDirty { get; protected set; }
		public bool NeedToBroadcast { get; protected set;}

		public int GetSyncDataSize() => getSyncDataSize() + 1;
		public int GetEntireDataSize() => getEntireDataSize() + 1;
		public void SetBroadcast() => NeedToBroadcast = true;
		protected abstract int getSyncDataSize();
		protected abstract int getEntireDataSize();

		public void SetNetworkOption(SyncType syncType, SyncAuthority syncAuthority)
		{
			SyncType = syncType;
			SyncAuthority = syncAuthority;
		}

		public void BindIndex(int index)
		{
			SyncIndex = (byte)index;
		}

		public abstract void ResetDeserializeEvent();

		public abstract void ResetOnDataChangeEvent();

		public void SerializeChangedPartTo(in NetPacketWriter writer)
		{
			writer.Write(SyncIndex);
			internalSerializeChangedPartTo(writer);
		}

		public void SerializeEntirelyTo(in NetPacketWriter writer)
		{
			writer.Write(SyncIndex);
			internalSerializeEntirelyTo(writer);
		}

		public abstract void OnSeralized();
		public abstract void DeserializeFrom(in NetPacketReader reader);
		public abstract void IgnoreDeserialize(in NetPacketReader reader);

		protected abstract void internalSerializeChangedPartTo(in NetPacketWriter writer);
		protected abstract void internalSerializeEntirelyTo(in NetPacketWriter writer);
	}
}
