using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Utils
{
	/// <summary>2차원 정수 좌표계 구조체입니다. 데카르트 좌표계를 사용합니다.</summary>
	[StructLayout(LayoutKind.Sequential)]
	public readonly struct TileCoord
	{
		public readonly int X;
		public readonly int Y;

		public readonly static TileCoord Zero		= new TileCoord(0, 0);
		public readonly static TileCoord One		= new TileCoord(1, 1);
		public readonly static TileCoord Right		= new TileCoord(1, 0);
		public readonly static TileCoord Left		= new TileCoord(-1, 0);
		public readonly static TileCoord Up			= new TileCoord(0, 1);
		public readonly static TileCoord Down		= new TileCoord(0, -1);
		public readonly static TileCoord RightUp	= new TileCoord(1, 1);
		public readonly static TileCoord RightDown	= new TileCoord(1, -1);
		public readonly static TileCoord LeftUp		= new TileCoord(-1, 1);
		public readonly static TileCoord LeftDown	= new TileCoord(-1, -1);

		/// <summary>2차원 정수 좌표 구조체를 생성합니다.</summary>
		/// <param name="x">X 좌표</param>
		/// <param name="y">Y 좌표</param>
		public TileCoord(int x, int y)
		{
			X = x;
			Y = y;
		}

		/// <summary>2차원 정수 좌표 구조체를 생성합니다. 소수점 이하는 버려집니다.</summary>
		/// <param name="x">X 좌표</param>
		/// <param name="y">Y 좌표</param>
		public TileCoord(float x, float y)
		{
			X = (int)x;
			Y = (int)y;
		}

		public UnityEngine.Vector3 ToVector3XZ() => new UnityEngine.Vector3(this.X, 0, this.Y);

		public static TileCoord operator +(TileCoord lhs, TileCoord rhs)
		{
			return new TileCoord(lhs.X + rhs.X, lhs.Y + rhs.Y);
		}

		public static TileCoord operator -(TileCoord lhs, TileCoord rhs)
		{
			return new TileCoord(lhs.X - rhs.X, lhs.Y - rhs.Y);
		}

		public static TileCoord operator *(TileCoord lhs, int rhs)
		{
			return new TileCoord(lhs.X * rhs, lhs.Y * rhs);
		}

		public static TileCoord operator *(int lhs, TileCoord rhs)
		{
			return new TileCoord(rhs.X * lhs, rhs.Y * lhs);
		}

		public static TileCoord operator *(TileCoord lhs, float rhs)
		{
			return new TileCoord(lhs.X * rhs, lhs.Y * rhs);
		}

		public static TileCoord operator *(float lhs, TileCoord rhs)
		{
			return new TileCoord(rhs.X * lhs, rhs.Y * lhs);
		}

		public static TileCoord operator /(TileCoord lhs, int rhs)
		{
			return new TileCoord(lhs.X / rhs, lhs.Y / rhs);
		}

		public static bool operator ==(TileCoord lhs, TileCoord rhs)
		{
			return (lhs.X == rhs.X && lhs.Y == rhs.Y);
		}

		public static bool operator !=(TileCoord lhs, TileCoord rhs)
		{
			return (lhs.X != rhs.X || lhs.Y != rhs.Y);
		}

		public override bool Equals(object obj)
		{
			return obj is TileCoord && this == (TileCoord)obj;
		}

		public override int GetHashCode()
		{
			return Tuple.Create(this.X, this.Y).GetHashCode();
		}

		public override string ToString()
		{
			return $"({X},{Y})";
		}
	}
}