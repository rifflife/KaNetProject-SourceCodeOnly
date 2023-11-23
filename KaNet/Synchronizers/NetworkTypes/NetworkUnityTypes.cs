using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KaNet.Utils;
using UnityEngine;

namespace KaNet.Synchronizers
{
	public struct NetQuantizeVector3
	{

	}

	[Serializable]
	public struct NetVector2 : INetworkSerializable
	{
		public Vector2 Value;

		public static implicit operator NetVector2(Vector2 vector2) => new NetVector2(vector2);
		public static implicit operator Vector2(NetVector2 netVector2) => netVector2.Value;
		public static implicit operator Vector3(NetVector2 netVector2) => netVector2.Value;

		public NetVector2(Vector2 value) => Value = value;
		public NetVector2(NetPacketReader reader)
		{
			Value.x = reader.ReadFloat();
			Value.y = reader.ReadFloat();
		}

		public int GetSyncDataSize() => 8;

		public void DeserializeFrom(in NetPacketReader reader)
		{
			Value.x = reader.ReadFloat();
			Value.y = reader.ReadFloat();
		}

		public void DeserializeFrom(in NetPacketReader reader, TessellateCoord tesselCoord)
		{
			throw new NotImplementedException();
		}

		public void SerializeTo(in NetPacketWriter writer)
		{
			writer.WriteFloat(Value.x);
			writer.WriteFloat(Value.y);
		}

		public void SerializeTo(in NetPacketWriter writer, TessellateCoord tesselCoord)
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return Value.ToString();
		}
	}

	[Serializable]
	public struct NetVector3 : INetworkSerializable
	{
		public Vector3 Value;

		public static implicit operator NetVector3(Vector3 vector3) => new NetVector3(vector3);
		public static implicit operator Vector3(NetVector3 netVector3) => netVector3.Value;

		public NetVector3(Vector3 value) => Value = value;
		public NetVector3(NetPacketReader reader)
		{
			Value.x = reader.ReadFloat();
			Value.y = reader.ReadFloat();
			Value.z = reader.ReadFloat();
		}

		public int GetSyncDataSize() => 12;

		public void DeserializeFrom(in NetPacketReader reader)
		{
			Value.x = reader.ReadFloat();
			Value.y = reader.ReadFloat();
			Value.z = reader.ReadFloat();
		}

		public void DeserializeFrom(in NetPacketReader reader, TessellateCoord tesselCoord)
		{
			throw new NotImplementedException();
		}

		public void SerializeTo(in NetPacketWriter writer)
		{
			writer.WriteFloat(Value.x);
			writer.WriteFloat(Value.y);
			writer.WriteFloat(Value.z);
		}

		public void SerializeTo(in NetPacketWriter writer, TessellateCoord tesselCoord)
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return Value.ToString();
		}
	}

	[Serializable]
	public struct Net2dRotation : INetworkSerializable
	{
		public NetFloat Value;

		public static implicit operator Net2dRotation(Quaternion value) => new Net2dRotation(value);
		public static implicit operator Quaternion(Net2dRotation value) => value.GetRotation();

		public Quaternion GetRotation()
		{
			return Quaternion.Euler(0, 0, Value);
		}

		public Net2dRotation(Quaternion value)
		{
			Value = value.eulerAngles.z;
		}

		public Net2dRotation(NetPacketReader reader)
		{
			Value = new NetFloat(reader);
		}

		public int GetSyncDataSize() => 4;

		public void DeserializeFrom(in NetPacketReader reader)
		{
			this = new Net2dRotation(reader);
		}

		public void DeserializeFrom(in NetPacketReader reader, TessellateCoord tesselCoord)
		{
			this = new Net2dRotation(reader);
		}

		public void SerializeTo(in NetPacketWriter writer)
		{
			writer.WriteFloat(Value);
		}

		public void SerializeTo(in NetPacketWriter writer, TessellateCoord tesselCoord)
		{
			SerializeTo(writer);
		}

		public override string ToString()
		{
			return Value.ToString();
		}
	}

	[Serializable]
	public struct NetQuaternion : INetworkSerializable
	{
		public Quaternion Value;

		public static implicit operator NetQuaternion(Quaternion value) => new NetQuaternion(value);
		public static implicit operator Quaternion(NetQuaternion value) => value.Value;

		public NetQuaternion(Quaternion value) => Value = value;
		public NetQuaternion(NetPacketReader reader)
		{
			Value.x = reader.ReadFloat();
			Value.y = reader.ReadFloat();
			Value.z = reader.ReadFloat();
			Value.w = reader.ReadFloat();
		}

		public int GetSyncDataSize() => 16;

		public void DeserializeFrom(in NetPacketReader reader)
		{
			this = new NetQuaternion(reader);
		}

		public void DeserializeFrom(in NetPacketReader reader, TessellateCoord tesselCoord)
		{
			this = new NetQuaternion(reader);
		}

		public void SerializeTo(in NetPacketWriter writer)
		{
			writer.WriteFloat(Value.x);
			writer.WriteFloat(Value.y);
			writer.WriteFloat(Value.z);
			writer.WriteFloat(Value.w);
		}

		public void SerializeTo(in NetPacketWriter writer, TessellateCoord tesselCoord)
		{
			SerializeTo(writer);
		}

		public override string ToString()
		{
			return Value.ToString();
		}
	}
}
