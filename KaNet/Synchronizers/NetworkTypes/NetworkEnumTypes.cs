// 이 코드는 자동생성된 코드입니다. 수정하지 마세요.

using System;
using System.Text;
using KaNet.Utils;

namespace KaNet.Synchronizers
{

	public struct NetUInt8<T> : INetworkSerializable where T : Enum
	{
		public const int SYNC_SIZE = 1;
		public byte Value;
		public static implicit operator NetUInt8<T>(T value) => new NetUInt8<T>(value);
		public static implicit operator T(NetUInt8<T> value) => (T)(object)value.Value;
		public NetUInt8(T value) => Value = (byte)(object)value;
		public NetUInt8(in NetPacketReader reader) => Value = reader.ReadUInt8();
		public int GetSyncDataSize() => 1;
		public void DeserializeFrom(in NetPacketReader reader) => Value = reader.ReadUInt8();
		public void SerializeTo(in NetPacketWriter writer) => writer.WriteUInt8(Value);
		public T GetEnum() => this;
		public override string ToString() => ((T)this).ToString();
	}

	public struct NetInt8<T> : INetworkSerializable where T : Enum
	{
		public const int SYNC_SIZE = 1;
		public sbyte Value;
		public static implicit operator NetInt8<T>(T value) => new NetInt8<T>(value);
		public static implicit operator T(NetInt8<T> value) => (T)(object)value.Value;
		public NetInt8(T value) => Value = (sbyte)(object)value;
		public NetInt8(in NetPacketReader reader) => Value = reader.ReadInt8();
		public int GetSyncDataSize() => 1;
		public void DeserializeFrom(in NetPacketReader reader) => Value = reader.ReadInt8();
		public void SerializeTo(in NetPacketWriter writer) => writer.WriteInt8(Value);
		public T GetEnum() => this;
		public override string ToString() => ((T)this).ToString();
	}

	public struct NetUInt16<T> : INetworkSerializable where T : Enum
	{
		public const int SYNC_SIZE = 2;
		public ushort Value;
		public static implicit operator NetUInt16<T>(T value) => new NetUInt16<T>(value);
		public static implicit operator T(NetUInt16<T> value) => (T)(object)value.Value;
		public NetUInt16(T value) => Value = (ushort)(object)value;
		public NetUInt16(in NetPacketReader reader) => Value = reader.ReadUInt16();
		public int GetSyncDataSize() => 2;
		public void DeserializeFrom(in NetPacketReader reader) => Value = reader.ReadUInt16();
		public void SerializeTo(in NetPacketWriter writer) => writer.WriteUInt16(Value);
		public T GetEnum() => this;
		public override string ToString() => ((T)this).ToString();
	}

	public struct NetInt16<T> : INetworkSerializable where T : Enum
	{
		public const int SYNC_SIZE = 2;
		public short Value;
		public static implicit operator NetInt16<T>(T value) => new NetInt16<T>(value);
		public static implicit operator T(NetInt16<T> value) => (T)(object)value.Value;
		public NetInt16(T value) => Value = (short)(object)value;
		public NetInt16(in NetPacketReader reader) => Value = reader.ReadInt16();
		public int GetSyncDataSize() => 2;
		public void DeserializeFrom(in NetPacketReader reader) => Value = reader.ReadInt16();
		public void SerializeTo(in NetPacketWriter writer) => writer.WriteInt16(Value);
		public T GetEnum() => this;
		public override string ToString() => ((T)this).ToString();
	}

	public struct NetUInt32<T> : INetworkSerializable where T : Enum
	{
		public const int SYNC_SIZE = 4;
		public uint Value;
		public static implicit operator NetUInt32<T>(T value) => new NetUInt32<T>(value);
		public static implicit operator T(NetUInt32<T> value) => (T)(object)value.Value;
		public NetUInt32(T value) => Value = (uint)(object)value;
		public NetUInt32(in NetPacketReader reader) => Value = reader.ReadUInt32();
		public int GetSyncDataSize() => 4;
		public void DeserializeFrom(in NetPacketReader reader) => Value = reader.ReadUInt32();
		public void SerializeTo(in NetPacketWriter writer) => writer.WriteUInt32(Value);
		public T GetEnum() => this;
		public override string ToString() => ((T)this).ToString();
	}

	public struct NetInt32<T> : INetworkSerializable where T : Enum
	{
		public const int SYNC_SIZE = 4;
		public int Value;
		public static implicit operator NetInt32<T>(T value) => new NetInt32<T>(value);
		public static implicit operator T(NetInt32<T> value) => (T)(object)value.Value;
		public NetInt32(T value) => Value = (int)(object)value;
		public NetInt32(in NetPacketReader reader) => Value = reader.ReadInt32();
		public int GetSyncDataSize() => 4;
		public void DeserializeFrom(in NetPacketReader reader) => Value = reader.ReadInt32();
		public void SerializeTo(in NetPacketWriter writer) => writer.WriteInt32(Value);
		public T GetEnum() => this;
		public override string ToString() => ((T)this).ToString();
	}

	public struct NetUInt64<T> : INetworkSerializable where T : Enum
	{
		public const int SYNC_SIZE = 8;
		public ulong Value;
		public static implicit operator NetUInt64<T>(T value) => new NetUInt64<T>(value);
		public static implicit operator T(NetUInt64<T> value) => (T)(object)value.Value;
		public NetUInt64(T value) => Value = (ulong)(object)value;
		public NetUInt64(in NetPacketReader reader) => Value = reader.ReadUInt64();
		public int GetSyncDataSize() => 8;
		public void DeserializeFrom(in NetPacketReader reader) => Value = reader.ReadUInt64();
		public void SerializeTo(in NetPacketWriter writer) => writer.WriteUInt64(Value);
		public T GetEnum() => this;
		public override string ToString() => ((T)this).ToString();
	}

	public struct NetInt64<T> : INetworkSerializable where T : Enum
	{
		public const int SYNC_SIZE = 8;
		public long Value;
		public static implicit operator NetInt64<T>(T value) => new NetInt64<T>(value);
		public static implicit operator T(NetInt64<T> value) => (T)(object)value.Value;
		public NetInt64(T value) => Value = (long)(object)value;
		public NetInt64(in NetPacketReader reader) => Value = reader.ReadInt64();
		public int GetSyncDataSize() => 8;
		public void DeserializeFrom(in NetPacketReader reader) => Value = reader.ReadInt64();
		public void SerializeTo(in NetPacketWriter writer) => writer.WriteInt64(Value);
		public T GetEnum() => this;
		public override string ToString() => ((T)this).ToString();
	}


}
