using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KaNet.Core;
using KaNet.Utils;

namespace KaNet.Synchronizers
{

	[Serializable]
	public struct NetString : INetworkSerializable
	{
		public string Value;
		public static implicit operator NetString(string value) => new NetString(value);
		public static implicit operator string(NetString value) => value.Value;
		public NetString(string value) => Value = value;
		public NetString(in NetPacketReader reader) => Value = reader.ReadString();
		public int GetSyncDataSize() => Encoding.UTF8.GetByteCount(Value) + 2;
		public void SerializeTo(in NetPacketWriter writer) => writer.WriteString(Value);
		public void DeserializeFrom(in NetPacketReader reader) => Value = reader.ReadString();
		public override string ToString() => Value.ToString();
	}

	public struct NetObjectTypeStruct : INetworkSerializable
	{
		public NetObjectType Type;
		public BaseNetObjectType BaseType => NetObjectTypeExtension.GetBaseType(Type);
		public static implicit operator NetObjectTypeStruct(NetObjectType value) => new NetObjectTypeStruct(value);
		public static implicit operator NetObjectType(NetObjectTypeStruct value) => value.Type;
		public NetObjectTypeStruct(NetObjectType value) => Type = value;
		public NetObjectTypeStruct(NetPacketReader reader) => Type = (NetObjectType)reader.ReadUInt16();
		public int GetSyncDataSize() => 2;
		public void SerializeTo(in NetPacketWriter writer) => writer.WriteUInt16((ushort)Type);
		public void DeserializeFrom(in NetPacketReader reader) => Type = (NetObjectType)reader.ReadUInt16();
		public override string ToString() => Type.ToString();
		public static bool operator ==(NetObjectTypeStruct lhs, NetObjectTypeStruct rhs) => lhs.Type == rhs.Type;
		public static bool operator !=(NetObjectTypeStruct lhs, NetObjectTypeStruct rhs) => lhs.Type != rhs.Type;

		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is NetObjectTypeStruct))
			{
				return false;
			}

			return this == (NetObjectTypeStruct)obj;
		}

		public override int GetHashCode()
		{
			return Type.GetHashCode();
		}
	}

	/// <summary>프로그램 식별 구조체입니다.</summary>
	public struct NetProgramID : INetworkSerializable
	{
		public NetString GameName;
		public NetString Version;
		public NetUInt32 AppID;

		public NetProgramID(string gameName, string version, uint appID)
		{
			GameName = gameName;
			Version = version;
			AppID = appID;
		}

		public int GetSyncDataSize()
		{
			return GameName.GetSyncDataSize() + Version.GetSyncDataSize() + AppID.GetSyncDataSize();
		}

		public void SerializeTo(in NetPacketWriter writer)
		{
			GameName.SerializeTo(writer);
			Version.SerializeTo(writer);
			AppID.SerializeTo(writer);
		}

		public void DeserializeFrom(in NetPacketReader reader)
		{
			GameName.DeserializeFrom(reader);
			Version.DeserializeFrom(reader);
			AppID.DeserializeFrom(reader);
		}

		public override string ToString()
		{
			return
				$"Name : {GameName}\n" +
				$"Version : {Version}\n" +
				$"AppID : {AppID}";
		}

		public static bool operator ==(NetProgramID lhs, NetProgramID rhs)
		{
			return lhs.GameName == rhs.GameName &&
				lhs.Version == rhs.Version && 
				lhs.AppID == rhs.AppID;
		}

		public static bool operator !=(NetProgramID lhs, NetProgramID rhs)
		{
			return !(lhs == rhs);
		}

		public override bool Equals(object obj)
		{
			if (obj is NetProgramID)
			{
				return this == (NetProgramID)obj;
			}

			return false;
		}

		public override int GetHashCode()
		{
			return new Tuple<NetString, NetString, NetUInt32>
				(GameName, Version, AppID).GetHashCode();
		}
	}

	/// <summary>세션을 나타내는 구조체입니다.</summary>
	public struct NetSessionID : INetworkSerializable
	{
		public byte Value;
		public static implicit operator NetSessionID(byte value) => new NetSessionID(value);
		public static implicit operator byte(NetSessionID value) => value.Value;
		public NetSessionID(byte value) => Value = value;
		public NetSessionID(in NetPacketReader reader) => Value = reader.ReadUInt8();
		public int GetSyncDataSize() => 1;
		public void SerializeTo(in NetPacketWriter writer) => writer.WriteUInt8(Value);
		public void DeserializeFrom(in NetPacketReader reader) => Value = reader.ReadUInt8();
		public override string ToString() => $"SessionID : {Value}";

		public static bool operator ==(NetSessionID lhs, NetSessionID rhs)
		{
			return lhs.Value == rhs.Value;
		}

		public static bool operator !=(NetSessionID lhs, NetSessionID rhs)
		{
			return lhs.Value != rhs.Value;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is NetSessionID))
			{
				return false;
			}

			return this == (NetSessionID)obj;
		}

		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}
	}

	public struct NetObjectID : INetworkSerializable
	{
		public ushort Value;
		public static implicit operator NetObjectID(ushort value) => new NetObjectID(value);
		public static implicit operator ushort(NetObjectID value) => value.Value;
		public NetObjectID(ushort value) => Value = value;
		public NetObjectID(in NetPacketReader reader) => Value = reader.ReadUInt16();
		public int GetSyncDataSize() => 2;
		public void SerializeTo(in NetPacketWriter writer) => writer.WriteUInt16(Value);
		public void DeserializeFrom(in NetPacketReader reader) => Value = reader.ReadUInt16();
		public override string ToString() => $"NetworkObjectID : {Value}";

		public static bool operator ==(NetObjectID lhs, NetObjectID rhs)
		{
			return lhs.Value == rhs.Value;
		}

		public static bool operator !=(NetObjectID lhs, NetObjectID rhs)
		{
			return lhs.Value != rhs.Value;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is NetObjectID))
			{
				return false;
			}

			return this == (NetObjectID)obj;
		}

		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}
	}

	public struct NetTimestamp : INetworkSerializable
	{
		public int MillisecondsTick;
		public static implicit operator NetTimestamp(long millisecondsTick) => new NetTimestamp(millisecondsTick);
		public static implicit operator int(NetTimestamp timeStamp) => timeStamp.MillisecondsTick;
		public static implicit operator DateTime(NetTimestamp timeStamp) => new DateTime(timeStamp);

		public NetTimestamp(float seconds)
		{
			MillisecondsTick = (int)(seconds * 1000);
		}

		public NetTimestamp(long millisecondsTick)
		{
			MillisecondsTick = (int)millisecondsTick;
		}

		public NetTimestamp(Stopwatch sw)
		{
			MillisecondsTick = (int)sw.ElapsedMilliseconds;
		}

		public DateTime GetDateTime() => new DateTime(MillisecondsTick);
		public int GetSyncDataSize() => 4;

		public void SerializeTo(in NetPacketWriter writer) => writer.WriteInt32(MillisecondsTick);
		public void DeserializeFrom(in NetPacketReader reader) => MillisecondsTick = reader.ReadInt32();

		public override string ToString()
		{
			return MillisecondsTick.ToString();
		}
	}

	public struct NetBaseHeader : INetworkSerializable
	{
		public const int HEADER_SIZE = 8;

		public NetUInt8<PacketHeaderType> PacketHeader;
		public NetUInt16 PacketLength;
		public NetSessionID SenderID;
		public NetTimestamp Timestamp;

		public NetBaseHeader
		(
			PacketHeaderType header,
			ushort packetLength,
			NetSessionID senderID,
			NetTimestamp timestamp
		)
		{
			PacketHeader = header;
			PacketLength = packetLength;
			SenderID = senderID;
			Timestamp = timestamp;
		}

		public NetBaseHeader(in NetPacketReader reader)
		{
			PacketHeader = new();
			PacketHeader.DeserializeFrom(reader);

			PacketLength = new();
			PacketLength.DeserializeFrom(reader);

			SenderID = new ();
			SenderID.DeserializeFrom(reader);

			Timestamp = new ();
			Timestamp.DeserializeFrom(reader);
		}

		public int GetSyncDataSize() => 8;

		public void SerializeTo(in NetPacketWriter writer)
		{
			PacketHeader.SerializeTo(writer);
			PacketLength.SerializeTo(writer);
			SenderID.SerializeTo(writer);
			Timestamp.SerializeTo(writer);
		}

		public void DeserializeFrom(in NetPacketReader reader)
		{
			PacketHeader.DeserializeFrom(reader);
			PacketLength.DeserializeFrom(reader);
			SenderID.DeserializeFrom(reader);
			Timestamp.DeserializeFrom(reader);
		}
	}

	public struct NetBaseSyncHeader : INetworkSerializable
	{
		public const int HEADER_SIZE = 6;

		public TessellateCoord TesselCoord;
		public NetUInt16 SyncCount;

		public NetBaseSyncHeader(TessellateCoord tesselCoord, NetUInt16 syncCount)
		{
			TesselCoord = tesselCoord;
			SyncCount = syncCount;
		}

		public int GetSyncDataSize() => 6;

		public void DeserializeFrom(in NetPacketReader reader)
		{
			TesselCoord.DeserializeFrom(reader);
			SyncCount.DeserializeFrom(reader);
		}

		public void SerializeTo(in NetPacketWriter writer)
		{
			TesselCoord.SerializeTo(writer);
			SyncCount.SerializeTo(writer);
		}
	}

	public struct NetConnectRequestInfo : INetworkSerializable
	{
		public NetProgramID ProgramID;
		public NetString Password;

		public NetConnectRequestInfo(NetProgramID programID, NetString password)
		{
			ProgramID = programID;
			Password = password;
		}

		public int GetSyncDataSize()
		{
			return ProgramID.GetSyncDataSize() + Password.GetSyncDataSize();
		}

		public void DeserializeFrom(in NetPacketReader reader)
		{
			ProgramID.DeserializeFrom(reader);
			Password.DeserializeFrom(reader);
		}

		public void SerializeTo(in NetPacketWriter writer)
		{
			ProgramID.SerializeTo(writer);
			Password.SerializeTo(writer);
		}

		public static bool operator ==(NetConnectRequestInfo lhs, NetConnectRequestInfo rhs)
		{
			return (lhs.ProgramID == rhs.ProgramID && lhs.Password == rhs.Password);
		}

		public static bool operator !=(NetConnectRequestInfo lhs, NetConnectRequestInfo rhs)
		{
			return !(lhs == rhs);
		}

		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is NetConnectRequestInfo))
			{
				return false;
			}

			return this == (NetConnectRequestInfo)obj;
		}

		public override int GetHashCode()
		{
			return new Tuple<NetProgramID, NetString>(ProgramID, Password).GetHashCode();
		}
	}

	public struct NetMapPointerID : INetworkSerializable
	{
		public ushort Value;
		public static implicit operator NetMapPointerID(ushort value) => new NetMapPointerID(value);
		public static implicit operator ushort(NetMapPointerID value) => value.Value;
		public NetMapPointerID(ushort value) => Value = value;
		public NetMapPointerID(in NetPacketReader reader) => Value = reader.ReadUInt16();
		public int GetSyncDataSize() => 2;
		public void SerializeTo(in NetPacketWriter writer) => writer.WriteUInt16(Value);
		public void DeserializeFrom(in NetPacketReader reader) => Value = reader.ReadUInt16();
		public override string ToString() => $"MapPointerID : {Value}";

		public static bool operator ==(NetMapPointerID lhs, NetMapPointerID rhs)
		{
			return lhs.Value == rhs.Value;
		}

		public static bool operator !=(NetMapPointerID lhs, NetMapPointerID rhs)
		{
			return lhs.Value != rhs.Value;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is NetMapPointerID))
			{
				return false;
			}

			return this == (NetMapPointerID)obj;
		}

		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}
	}
}
