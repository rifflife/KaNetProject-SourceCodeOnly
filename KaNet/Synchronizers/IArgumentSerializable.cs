using KaNet.Utils;

namespace KaNet.Synchronizers
{
	public interface INetworkSerializable
	{
		public int GetSyncDataSize();
		public void SerializeTo(in NetPacketWriter writer);
		public void DeserializeFrom(in NetPacketReader reader);
	}
}
