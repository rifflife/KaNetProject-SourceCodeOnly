using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KaNet.Synchronizers;
using KaNet.Utils;
using UnityEngine;

namespace KaNet.Synchronizers
{
	public struct TessellateCoord : INetworkSerializable
	{
		public static readonly TessellateCoord Zero = new TessellateCoord(0, 0, 0);

		public sbyte X;
		public sbyte Y;
		public sbyte Z;

		public TessellateCoord(NetPacketReader reader)
		{
			X = reader.ReadInt8();
			Y = reader.ReadInt8();
			Z = reader.ReadInt8();
		}

		public TessellateCoord(Vector3 position, float cellX, float cellY, float cellZ)
		{
			X = (sbyte)(position.x / cellX);
			Y = (sbyte)(position.y / cellY);
			Z = (sbyte)(position.z / cellZ);
		}

		public TessellateCoord(int x, int y, int z)
		{
			X = (sbyte)x;
			Y = (sbyte)y;
			Z = (sbyte)z;
		}

		public TessellateCoord(NetInt8 x, NetInt8 y, NetInt8 z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public static TessellateCoord operator +(TessellateCoord lhs, TessellateCoord rhs)
		{
			return new TessellateCoord(lhs.X + rhs.X, lhs.Y + rhs.Y, lhs.Z + rhs.Z);
		}

		public static TessellateCoord operator -(TessellateCoord lhs, TessellateCoord rhs)
		{
			return new TessellateCoord(lhs.X - rhs.X, lhs.Y - rhs.Y, lhs.Z + rhs.Z);
		}

		public static TessellateCoord operator *(TessellateCoord lhs, int rhs)
		{
			return new TessellateCoord(lhs.X * rhs, lhs.Y * rhs, lhs.Z * rhs);
		}

		public static TessellateCoord operator *(int lhs, TessellateCoord rhs)
		{
			return new TessellateCoord(lhs * rhs.X, lhs * rhs.Y, lhs * rhs.Z);
		}

		public static bool operator ==(TessellateCoord lhs, TessellateCoord rhs)
		{
			return (lhs.X == rhs.X) && (lhs.Y == rhs.Y) && (lhs.Z == rhs.Z);
		}

		public static bool operator !=(TessellateCoord lhs, TessellateCoord rhs)
		{
			return (lhs.X != rhs.X) || (lhs.Y != rhs.Y) || (lhs.Z == rhs.Z);
		}

		public int GetSyncDataSize() => 3;

		public void DeserializeFrom(in NetPacketReader reader)
		{
			X = reader.ReadInt8();
			Y = reader.ReadInt8();
			Z = reader.ReadInt8();
		}

		public void SerializeTo(in NetPacketWriter writer)
		{
			writer.WriteInt8(X);
			writer.WriteInt8(Y);
			writer.WriteInt8(Z);
		}

		public override bool Equals(object obj)
		{
			return obj is TessellateCoord coord && (X == coord.X) && (Y == coord.Y) && (Z == coord.Z);
		}

		//public override int GetHashCode()
		//{
		//	return HashCode.Combine(X, Y, Z);
		//}

		public override int GetHashCode()
		{
			return (X << 16) | (Y << 8) | (int)Z;
		}

		public override string ToString()
		{
			return $"({X.ToString()}, {Y.ToString()}, {Z.ToString()})";
		}
	}
}
