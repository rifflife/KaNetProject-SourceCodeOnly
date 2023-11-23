using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KaNet.Utils;

namespace KaNet.Synchronizers
{
	public struct NetLifeStreamToken : INetworkSerializable
	{
		public NetBool IsCreate;
		public NetUInt16<NetObjectType> ObjectType;
		public NetObjectID ObjectID;
		public NetSessionID OwnerID;
		public NetVector3 Position;

		public bool IsOwnerOnly;

		public NetLifeStreamToken(NetworkObject netObject, bool isCreate)
		{
			IsOwnerOnly = netObject.IsOwnerOnly;

			IsCreate = isCreate;
			ObjectID = netObject.ID;

			ObjectType = new();
			OwnerID = netObject.OwnerID;
			Position = new();

			if (IsCreate)
			{
				ObjectType = netObject.Type;
				Position = netObject.transform.position;
			}
		}

		public NetLifeStreamToken(in NetPacketReader reader)
		{
			IsOwnerOnly = false;
			IsCreate = new NetBool(reader);

			if (IsCreate)
			{
				ObjectType = new(reader);
				ObjectID = new(reader);
				OwnerID = new(reader);
				Position = new(reader);
			}
			else
			{
				ObjectID = new(reader);
				ObjectType = new();
				OwnerID = new();
				Position = new();
			}
		}

		public int GetSyncDataSize()
		{
			if (IsCreate)
			{
				return IsCreate.GetSyncDataSize()
					+ ObjectType.GetSyncDataSize()
					+ ObjectID.GetSyncDataSize()
					+ OwnerID.GetSyncDataSize()
					+ Position.GetSyncDataSize();
			}

			return IsCreate.GetSyncDataSize() + ObjectID.GetSyncDataSize();
		}

		public void DeserializeFrom(in NetPacketReader reader)
		{
			IsCreate.DeserializeFrom(reader);

			if (IsCreate)
			{
				ObjectType.DeserializeFrom(reader);
				ObjectID.DeserializeFrom(reader);
				OwnerID.DeserializeFrom(reader);
				Position.DeserializeFrom(reader);
			}
			else
			{
				ObjectID.DeserializeFrom(reader);
			}
		}

		public void SerializeTo(in NetPacketWriter writer)
		{
			if (IsCreate)
			{
				IsCreate.SerializeTo(writer);
				ObjectType.SerializeTo(writer);
				ObjectID.SerializeTo(writer);
				OwnerID.SerializeTo(writer);
				Position.SerializeTo(writer);
			}
			else
			{
				IsCreate.SerializeTo(writer);
				ObjectID.SerializeTo(writer);
			}
		}
	}

	public struct NetSyncHeader : INetworkSerializable
	{
		public TessellateCoord Coord;
		public NetUInt16 Count;

		public NetSyncHeader(TessellateCoord coord, NetUInt16 count)
		{
			Coord = coord;
			Count = count;
		}

		public NetSyncHeader(in NetPacketReader reader)
		{
			Coord = new(reader);
			Count = new(reader);
		}

		public int GetSyncDataSize()
		{
			return Coord.GetSyncDataSize() + Count.GetSyncDataSize();
		}

		public void DeserializeFrom(in NetPacketReader reader)
		{
			Coord.DeserializeFrom(reader);
			Count.DeserializeFrom(reader);
		}

		public void SerializeTo(in NetPacketWriter writer)
		{
			Coord.SerializeTo(writer);
			Count.SerializeTo(writer);
		}
	}
}
