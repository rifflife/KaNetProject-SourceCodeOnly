using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KaNet.Synchronizers;
using KaNet.Utils;
using Steamworks;

namespace KaNet.Session
{
	public class NetSessionInfo : INetworkSerializable
	{
		public NetSessionID ID { get; private set; }
		public SteamId SteamID { get; private set; }
		public bool IsReadyToSync { get; private set;}
		public Friend Friend => new Friend(SteamID);
		public string Name => Friend.Name;
		public NetTimestamp LastHeartbeat { get; private set; }

		public NetSessionInfo(NetSessionID id)
		{
			ID = id;
			SteamID = 0;
			IsReadyToSync = false;
		}

		public NetSessionInfo(NetSessionID id, SteamId steamID, NetTimestamp lastHeartbeat)
		{
			ID = id;
			SteamID = steamID;
			IsReadyToSync = false;
			LastHeartbeat = lastHeartbeat;
		}

		public NetSessionInfo(Friend friend)
		{
			ID = 0;
			SteamID = friend.Id;
			IsReadyToSync = false;
		}

		public NetSessionInfo(SteamId steamID)
		{
			ID = 0;
			SteamID = steamID;
			IsReadyToSync = false;
		}

		public void SetIsReady(bool isReady = true)
		{
			IsReadyToSync = isReady;
		}

		public bool IsSameProperties(NetSessionInfo instance)
		{
			return ID == instance.ID && SteamID.Value == instance.SteamID.Value;
		}

		public bool CheckHeartbeat(NetTimestamp heartbeat, bool isStop)
		{
			if (isStop)
			{
				LastHeartbeat = heartbeat;
			}

			return (heartbeat - LastHeartbeat < KaNetGlobal.HEARTBEAT_TIMEOUT_INTERVAL_MS);
		}

		public void SetLastHeartbeat(NetTimestamp timestamp)
		{
			LastHeartbeat = timestamp;
		}

		public override int GetHashCode()
		{
			return Tuple.Create(ID, SteamID.Value).GetHashCode();
		}

		public override string ToString()
		{
			return SteamID == 0 ?
				$"Session : ({ID})" :
				$"Steam Session : ({ID}:{Name}|IsReadyToSync{IsReadyToSync})";
		}

		public int GetSyncDataSize()
		{
			return ID.GetSyncDataSize() + sizeof(ulong) + 1;
		}

		public void SerializeTo(in NetPacketWriter writer)
		{
			ID.SerializeTo(writer);
			writer.WriteUInt64(SteamID.Value);
			writer.WriteBool(IsReadyToSync);
		}

		public void DeserializeFrom(in NetPacketReader reader)
		{
			ID.DeserializeFrom(reader);
			SteamID = reader.ReadUInt64();
			IsReadyToSync = reader.ReadBool();
		}
	}
}
