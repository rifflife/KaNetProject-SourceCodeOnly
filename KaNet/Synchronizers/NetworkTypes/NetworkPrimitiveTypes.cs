// 이 코드는 자동생성된 코드입니다. 수정하지 마세요.

using System;
using System.Text;
using KaNet.Utils;

namespace KaNet.Synchronizers
{

	[Serializable]
	public struct NetBool : INetworkSerializable
	{
		public const int SYNC_SIZE = 1;
		public bool Value;
		public static implicit operator NetBool(bool value) => new NetBool(value);
		public static implicit operator bool(NetBool value) => value.Value;
		public NetBool(bool value) => Value = value;
		public NetBool(in NetPacketReader reader) => Value = reader.ReadBool();
		public int GetSyncDataSize() => 1;
		public void SerializeTo(in NetPacketWriter writer) => writer.WriteBool(Value);
		public void DeserializeFrom(in NetPacketReader reader) => Value = reader.ReadBool();
		public override string ToString() => Value.ToString();
	}

	[Serializable]
	public struct NetUInt8 : INetworkSerializable
	{
		public const int SYNC_SIZE = 1;
		public byte Value;
		public static implicit operator NetUInt8(byte value) => new NetUInt8(value);
		public static implicit operator byte(NetUInt8 value) => value.Value;
		public NetUInt8(byte value) => Value = value;
		public NetUInt8(in NetPacketReader reader) => Value = reader.ReadUInt8();
		public int GetSyncDataSize() => 1;
		public void SerializeTo(in NetPacketWriter writer) => writer.WriteUInt8(Value);
		public void DeserializeFrom(in NetPacketReader reader) => Value = reader.ReadUInt8();
		public override string ToString() => Value.ToString();
	}

	[Serializable]
	public struct NetInt8 : INetworkSerializable
	{
		public const int SYNC_SIZE = 1;
		public sbyte Value;
		public static implicit operator NetInt8(sbyte value) => new NetInt8(value);
		public static implicit operator sbyte(NetInt8 value) => value.Value;
		public NetInt8(sbyte value) => Value = value;
		public NetInt8(in NetPacketReader reader) => Value = reader.ReadInt8();
		public int GetSyncDataSize() => 1;
		public void SerializeTo(in NetPacketWriter writer) => writer.WriteInt8(Value);
		public void DeserializeFrom(in NetPacketReader reader) => Value = reader.ReadInt8();
		public override string ToString() => Value.ToString();
	}

	[Serializable]
	public struct NetUInt16 : INetworkSerializable
	{
		public const int SYNC_SIZE = 2;
		public ushort Value;
		public static implicit operator NetUInt16(ushort value) => new NetUInt16(value);
		public static implicit operator ushort(NetUInt16 value) => value.Value;
		public NetUInt16(ushort value) => Value = value;
		public NetUInt16(in NetPacketReader reader) => Value = reader.ReadUInt16();
		public int GetSyncDataSize() => 2;
		public void SerializeTo(in NetPacketWriter writer) => writer.WriteUInt16(Value);
		public void DeserializeFrom(in NetPacketReader reader) => Value = reader.ReadUInt16();
		public override string ToString() => Value.ToString();
	}

	[Serializable]
	public struct NetInt16 : INetworkSerializable
	{
		public const int SYNC_SIZE = 2;
		public short Value;
		public static implicit operator NetInt16(short value) => new NetInt16(value);
		public static implicit operator short(NetInt16 value) => value.Value;
		public NetInt16(short value) => Value = value;
		public NetInt16(in NetPacketReader reader) => Value = reader.ReadInt16();
		public int GetSyncDataSize() => 2;
		public void SerializeTo(in NetPacketWriter writer) => writer.WriteInt16(Value);
		public void DeserializeFrom(in NetPacketReader reader) => Value = reader.ReadInt16();
		public override string ToString() => Value.ToString();
	}

	[Serializable]
	public struct NetUInt32 : INetworkSerializable
	{
		public const int SYNC_SIZE = 4;
		public uint Value;
		public static implicit operator NetUInt32(uint value) => new NetUInt32(value);
		public static implicit operator uint(NetUInt32 value) => value.Value;
		public NetUInt32(uint value) => Value = value;
		public NetUInt32(in NetPacketReader reader) => Value = reader.ReadUInt32();
		public int GetSyncDataSize() => 4;
		public void SerializeTo(in NetPacketWriter writer) => writer.WriteUInt32(Value);
		public void DeserializeFrom(in NetPacketReader reader) => Value = reader.ReadUInt32();
		public override string ToString() => Value.ToString();
	}

	[Serializable]
	public struct NetInt32 : INetworkSerializable
	{
		public const int SYNC_SIZE = 4;
		public int Value;
		public static implicit operator NetInt32(int value) => new NetInt32(value);
		public static implicit operator int(NetInt32 value) => value.Value;
		public NetInt32(int value) => Value = value;
		public NetInt32(in NetPacketReader reader) => Value = reader.ReadInt32();
		public int GetSyncDataSize() => 4;
		public void SerializeTo(in NetPacketWriter writer) => writer.WriteInt32(Value);
		public void DeserializeFrom(in NetPacketReader reader) => Value = reader.ReadInt32();
		public override string ToString() => Value.ToString();
	}

	[Serializable]
	public struct NetUInt64 : INetworkSerializable
	{
		public const int SYNC_SIZE = 8;
		public ulong Value;
		public static implicit operator NetUInt64(ulong value) => new NetUInt64(value);
		public static implicit operator ulong(NetUInt64 value) => value.Value;
		public NetUInt64(ulong value) => Value = value;
		public NetUInt64(in NetPacketReader reader) => Value = reader.ReadUInt64();
		public int GetSyncDataSize() => 8;
		public void SerializeTo(in NetPacketWriter writer) => writer.WriteUInt64(Value);
		public void DeserializeFrom(in NetPacketReader reader) => Value = reader.ReadUInt64();
		public override string ToString() => Value.ToString();
	}

	[Serializable]
	public struct NetInt64 : INetworkSerializable
	{
		public const int SYNC_SIZE = 8;
		public long Value;
		public static implicit operator NetInt64(long value) => new NetInt64(value);
		public static implicit operator long(NetInt64 value) => value.Value;
		public NetInt64(long value) => Value = value;
		public NetInt64(in NetPacketReader reader) => Value = reader.ReadInt64();
		public int GetSyncDataSize() => 8;
		public void SerializeTo(in NetPacketWriter writer) => writer.WriteInt64(Value);
		public void DeserializeFrom(in NetPacketReader reader) => Value = reader.ReadInt64();
		public override string ToString() => Value.ToString();
	}

	[Serializable]
	public struct NetFloat : INetworkSerializable
	{
		public const int SYNC_SIZE = 4;
		public float Value;
		public static implicit operator NetFloat(float value) => new NetFloat(value);
		public static implicit operator float(NetFloat value) => value.Value;
		public NetFloat(float value) => Value = value;
		public NetFloat(in NetPacketReader reader) => Value = reader.ReadFloat();
		public int GetSyncDataSize() => 4;
		public void SerializeTo(in NetPacketWriter writer) => writer.WriteFloat(Value);
		public void DeserializeFrom(in NetPacketReader reader) => Value = reader.ReadFloat();
		public override string ToString() => Value.ToString();
	}

	[Serializable]
	public struct NetDouble : INetworkSerializable
	{
		public const int SYNC_SIZE = 8;
		public double Value;
		public static implicit operator NetDouble(double value) => new NetDouble(value);
		public static implicit operator double(NetDouble value) => value.Value;
		public NetDouble(double value) => Value = value;
		public NetDouble(in NetPacketReader reader) => Value = reader.ReadDouble();
		public int GetSyncDataSize() => 8;
		public void SerializeTo(in NetPacketWriter writer) => writer.WriteDouble(Value);
		public void DeserializeFrom(in NetPacketReader reader) => Value = reader.ReadDouble();
		public override string ToString() => Value.ToString();
	}


}
