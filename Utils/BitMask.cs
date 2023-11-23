using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
	/// <summary>
	/// 가변 길이의 비트마스크 클래스 입니다. 3차원 까지 지원합니다.
	/// Create 정적 함수를 통해 생성합니다. BitMask의 X 크기는 32간격으로 생성됩니다.
	/// </summary>
	public class BitmaskVector
	{
		private int[] mMask;

		private const int FULL_TRUE_BIT = -1;
		private const int POSITION_MASK = 0b0001_1111;
		private const int POSITION_MODULAR = 5;

		private const int INT_BIT_COUNT = 32;

		/// <summary>전체 비트의 크기입니다.</summary>
		public int Size { get; private set; }
		/// <summary>비트의 X 크기입니다.</summary>
		public int SizeX { get; set; }
		/// <summary>비트의 Y 크기입니다.</summary>
		public int SizeY { get; set; }
		/// <summary>비트의 Z 크기입니다.</summary>
		public int SizeZ { get; set; }

		/// <summary>1차원 배열의 인덱스 폭입니다.</summary>
		public int Stride1D => SizeX;
		/// <summary>2차원 배열의 인덱스 폭입니다.</summary>
		public int Stride2D { get; private set; }

		/// <summary>비트마스크의 전체 메모리 크기입니다.</summary>
		public int MemorySize { get; private set; }
		/// <summary>비트마스크의 1차원 메모리 크기입니다.</summary>
		public int MemoryStride1D { get; private set; }

		/// <summary>비트마스크를 생성합니다.</summary>
		/// <param name="bitSize">비트의 크기입니다.</param>
		/// <param name="value">초기화할 boolean</param>
		/// <returns>비트마스크입니다.</returns>
		public static BitmaskVector Create(int bitSize, bool value = false)
		{
			int memoryStride1D = bitSize % 32 != 0 ? bitSize / 32 + 1 : bitSize / 32;

			return new BitmaskVector(memoryStride1D, value);
		}

		/// <summary>비트마스크를 생성합니다.</summary>
		/// <param name="bitSizeX">비트의 X 크기입니다.</param>
		/// <param name="bitSizeY">비트의 Y 크기입니다.</param>
		/// <param name="value">초기화할 boolean</param>
		/// <returns>비트마스크입니다.</returns>
		public static BitmaskVector Create(int bitSizeX, int bitSizeY, bool value = false)
		{
			int memoryStride1D = bitSizeX % 32 != 0 ? bitSizeX / 32 + 1 : bitSizeX / 32;

			return new BitmaskVector(memoryStride1D, bitSizeY, value);
		}

		/// <summary>비트마스크를 생성합니다.</summary>
		/// <param name="bitSizeX">비트의 X 크기입니다.</param>
		/// <param name="bitSizeY">비트의 Y 크기입니다.</param>
		/// <param name="bitSizeZ">비트의 Z 크기입니다.</param>
		/// <param name="value">초기화할 boolean</param>
		/// <returns>비트마스크입니다.</returns>
		public static BitmaskVector Create(int bitSizeX, int bitSizeY, int bitSizeZ, bool value = false)
		{
			int memoryStride1D = bitSizeX % 32 != 0 ? bitSizeX / 32 + 1 : bitSizeX / 32;

			return new BitmaskVector(memoryStride1D, bitSizeY, bitSizeZ, value);
		}

		/// <summary>Byte수 만큼 비트 마스크를 생성합니다.</summary>
		/// <param name="memoryStride1D">차지할 메모리 공간의 byte수 입니다. Size = byte * 8</param>
		/// <param name="value">초기화 할 값</param>
		private BitmaskVector(int memoryStride1D, bool value = false)
		{
			initialize(memoryStride1D, 1, 1, value);
		}

		private BitmaskVector(int memoryStride1D, int sizeY, bool value = false)
		{
			initialize(memoryStride1D, sizeY, 1, value);
		}

		private BitmaskVector(int memoryStride1D, int sizeY, int sizeZ, bool value = false)
		{
			initialize(memoryStride1D, sizeY, sizeZ, value);
		}

		private void initialize(int memoryStride1D, int sizeY, int sizeZ, bool value = false)
		{
			MemoryStride1D = memoryStride1D;

			SizeX = MemoryStride1D * INT_BIT_COUNT;
			SizeY = sizeY;
			SizeZ = sizeZ;

			MemorySize = MemoryStride1D * SizeY * SizeZ;

			Stride2D = Stride1D * SizeY;

			Size = Stride2D * sizeZ;

			mMask = new int[MemorySize];

			if (value)
			{
				Clear(value);
			}
		}

		public bool this[int z, int y, int x]
		{
			get
			{
				int index = x + y * Stride1D + z * Stride2D;

				return (mMask[index >> POSITION_MODULAR] & (1 << (index & POSITION_MASK))) != 0;
			}
			set
			{
				int index = x + y * Stride1D + z * Stride2D;

				if (value)
				{
					mMask[index >> POSITION_MODULAR] |= (1 << (index & POSITION_MASK));
				}
				else
				{
					mMask[index >> POSITION_MODULAR] &= ~(1 << (index & POSITION_MASK));
				}
			}
		}

		public bool this[int y, int x]
		{
			get
			{
				int index = x + y * Stride1D;

				return (mMask[index >> POSITION_MODULAR] & (1 << (index & POSITION_MASK))) != 0;
			}
			set
			{
				int index = x + y * Stride1D;

				if (value)
				{
					mMask[index >> POSITION_MODULAR] |= (1 << (index & POSITION_MASK));
				}
				else
				{
					mMask[index >> POSITION_MODULAR] &= ~(1 << (index & POSITION_MASK));
				}
			}
		}

		/// <summary>해당 비트를 참조합니다.</summary>
		/// <param name="x">bitmask 인덱스</param>
		/// <returns>값</returns>
		public bool this[int x]
		{
			get
			{
				return (mMask[x >> POSITION_MODULAR] & (1 << (x & POSITION_MASK))) != 0;
			}
			set
			{
				if (value)
				{
					mMask[x >> POSITION_MODULAR] |= (1 << (x & POSITION_MASK));
				}
				else
				{
					mMask[x >> POSITION_MODULAR] &= ~(1 << (x & POSITION_MASK));
				}
			}
		}

		public BitmaskVector Clone()
		{
			BitmaskVector copy = new BitmaskVector(MemoryStride1D, SizeY, SizeZ, false);
			Buffer.BlockCopy(this.mMask, 0, copy.mMask, 0, mMask.Length * 4);
			return copy;
		}

		/// <summary>모든 비트를 value로 초기화합니다.</summary>
		/// <param name="value">초기화할 값</param>
		public void Clear(bool value = false)
		{
			int clearValue = value ? FULL_TRUE_BIT : 0;

			for (int i = 0; i < mMask.Length; i++)
			{
				mMask[i] = clearValue;
			}
		}

		/// <summary>모든 비트를 뒤집습니다.</summary>
		public void Flip()
		{
			for (int i = 0; i < mMask.Length; i++)
			{
				mMask[i] = ~mMask[i];
			}
		}

		/// <summary>해당 index의 비트를 반환 시도합니다.</summary>
		/// <param name="x">인덱스 x</param>
		/// <param name="value">값</param>
		/// <returns>반환에 성공하면 true를 반환합니다.</returns>
		public bool TryGetValue(int x, out bool value)
		{
			if (IsValidIndex(x))
			{
				value = this[x];
				return true;
			}

			value = false;
			return false;
		}

		/// <summary>해당 index의 비트를 반환 시도합니다.</summary>
		/// <param name="x">인덱스 x</param>
		/// <param name="y">인덱스 y</param>
		/// <param name="value">값</param>
		/// <returns>반환에 성공하면 true를 반환합니다.</returns>
		public bool TryGetValue(int x, int y, out bool value)
		{
			if (IsValidIndex(x, y))
			{
				value = this[y, x];
				return true;
			}

			value = false;
			return false;
		}

		/// <summary>해당 index의 비트를 반환 시도합니다.</summary>
		/// <param name="x">인덱스 x</param>
		/// <param name="y">인덱스 y</param>
		/// <param name="z">인덱스 z</param>
		/// <param name="value">값</param>
		/// <returns>반환에 성공하면 true를 반환합니다.</returns>
		public bool TryGetValue(int x, int y, int z, out bool value)
		{
			if (IsValidIndex(x, y, z))
			{
				value = this[z, y, x];
				return true;
			}

			value = false;
			return false;
		}

		/// <summary>해당 index의 비트를 value로 설정 시도합니다.</summary>
		/// <param name="x">인덱스 x</param>
		/// <param name="value">값</param>
		/// <returns>설정에 성공하면 true를 반환합니다.</returns>
		public bool TrySet(int x, bool value)
		{
			if (IsValidIndex(x))
			{
				this[x] = value;
				return true;
			}

			return false;
		}

		/// <summary>해당 index의 비트를 value로 설정 시도합니다.</summary>
		/// <param name="x">인덱스 x</param>
		/// <param name="y">인덱스 y</param>
		/// <param name="value">값</param>
		/// <returns>설정에 성공하면 true를 반환합니다.</returns>
		public bool TrySet(int x, int y, bool value)
		{
			if (IsValidIndex(x, y))
			{
				this[y, x] = value;
				return true;
			}

			return false;
		}

		/// <summary>해당 index의 비트를 value로 설정 시도합니다.</summary>
		/// <param name="x">인덱스 x</param>
		/// <param name="y">인덱스 y</param>
		/// <param name="z">인덱스 z</param>
		/// <param name="value">값</param>
		/// <returns>설정에 성공하면 true를 반환합니다.</returns>
		public bool TrySet(int x, int y, int z, bool value)
		{
			if (IsValidIndex(x, y, z))
			{
				this[z, y, x] = value;
				return true;
			}

			return false;
		}

		/// <summary>해당 index의 비트를 false로 설정 시도합니다.</summary>
		/// <param name="x">인덱스 x</param>
		/// <returns>설정에 성공하면 true를 반환합니다.</returns>
		public bool TrySetFalse(int x)
		{
			if (IsValidIndex(x))
			{
				SetFalse(x);
				return true;
			}

			return false;
		}

		/// <summary>해당 index의 비트를 false로 설정 시도합니다.</summary>
		/// <param name="x">인덱스 x</param>
		/// <param name="y">인덱스 y</param>
		/// <returns>설정에 성공하면 true를 반환합니다.</returns>
		public bool TrySetFalse(int x, int y)
		{
			if (IsValidIndex(x, y))
			{
				SetFalse(x, y);
				return true;
			}

			return false;
		}

		/// <summary>해당 index의 비트를 false로 설정 시도합니다.</summary>
		/// <param name="x">인덱스 x</param>
		/// <param name="y">인덱스 y</param>
		/// <param name="z">인덱스 z</param>
		/// <returns>설정에 성공하면 true를 반환합니다.</returns>
		public bool TrySetFalse(int x, int y, int z)
		{
			if (IsValidIndex(x, y, z))
			{
				SetFalse(x, y, z);
				return true;
			}

			return false;
		}

		/// <summary>해당 index의 비트를 true로 설정 시도합니다.</summary>
		/// <param name="x">인덱스 x</param>
		/// <returns>설정에 성공하면 true를 반환합니다.</returns>
		public bool TrySetTrue(int x)
		{
			if (IsValidIndex(x))
			{
				SetTrue(x);
				return true;
			}

			return false;
		}

		/// <summary>해당 index의 비트를 true로 설정 시도합니다.</summary>
		/// <param name="x">인덱스 x</param>
		/// <param name="y">인덱스 y</param>
		/// <returns>설정에 성공하면 true를 반환합니다.</returns>
		public bool TrySetTrue(int x, int y)
		{
			if (IsValidIndex(x, y))
			{
				SetTrue(x, y);
				return true;
			}

			return false;
		}

		/// <summary>해당 index의 비트를 true로 설정 시도합니다.</summary>
		/// <param name="x">인덱스 x</param>
		/// <param name="y">인덱스 y</param>
		/// <param name="z">인덱스 z</param>
		/// <returns>설정에 성공하면 true를 반환합니다.</returns>
		public bool TrySetTrue(int x, int y, int z)
		{
			if (IsValidIndex(x, y, z))
			{
				SetTrue(x, y, z);
				return true;
			}

			return false;
		}

		/// <summary>해당 index의 비트를 false로 설정합니다.</summary>
		/// <param name="x">인덱스 x</param>
		public void SetFalse(int x)
		{
			mMask[x >> POSITION_MODULAR] &= ~(1 << (x & POSITION_MASK));
		}

		/// <summary>해당 index의 비트를 false로 설정합니다.</summary>
		/// <param name="x">인덱스 x</param>
		/// <param name="y">인덱스 y</param>
		public void SetFalse(int x, int y)
		{
			int index = x + y * Stride1D;
			mMask[index >> POSITION_MODULAR] &= ~(1 << (index & POSITION_MASK));
		}

		/// <summary>해당 index의 비트를 false로 설정합니다.</summary>
		/// <param name="x">인덱스 x</param>
		/// <param name="y">인덱스 y</param>
		/// <param name="z">인덱스 z</param>
		public void SetFalse(int x, int y, int z)
		{
			int index = x + y * Stride1D + z * Stride2D;
			mMask[index >> POSITION_MODULAR] &= ~(1 << (index & POSITION_MASK));
		}

		/// <summary>해당 index의 비트를 true로 설정합니다.</summary>
		/// <param name="x">인덱스</param>
		public void SetTrue(int x)
		{
			mMask[x >> POSITION_MODULAR] |= (1 << (x & POSITION_MASK));
		}

		/// <summary>해당 index의 비트를 true로 설정합니다.</summary>
		/// <param name="x">인덱스</param>
		public void SetTrue(int x, int y)
		{
			int index = x + y * Stride1D;
			mMask[index >> POSITION_MODULAR] |= (1 << (index & POSITION_MASK));
		}

		/// <summary>해당 index의 비트를 true로 설정합니다.</summary>
		/// <param name="x">인덱스</param>
		public void SetTrue(int x, int y, int z)
		{
			int index = x + y * Stride1D + z * Stride2D;
			mMask[index >> POSITION_MODULAR] |= (1 << (index & POSITION_MASK));
		}

		/// <summary>모든 비트가 true라면 true를 반환합니다.</summary>
		public bool IsAllTrue()
		{
			foreach (var i in mMask)
			{
				if (i != FULL_TRUE_BIT)
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>모든 비트가 false라면 true를 반환합니다.</summary>
		public bool IsAllFalse()
		{
			foreach (var i in mMask)
			{
				if (i != 0)
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>인덱스가 유효한 범위인지 검사합니다.</summary>
		/// <param name="x">인덱스 x</param>
		/// <returns>유효한 인덱스라면 true를 반환합니다.</returns>
		public bool IsValidIndex(int x)
		{
			return !(x >= Size || x < 0);
		}

		/// <summary>인덱스가 유효한 범위인지 검사합니다.</summary>
		/// <param name="x">인덱스 x</param>
		/// <param name="y">인덱스 y</param>
		/// <returns>유효한 인덱스라면 true를 반환합니다.</returns>
		public bool IsValidIndex(int x, int y)
		{
			return !(x < 0 || x >= SizeX || y < 0 || y >= SizeY);
		}

		/// <summary>인덱스가 유효한 범위인지 검사합니다.</summary>
		/// <param name="x">인덱스 x</param>
		/// <param name="y">인덱스 y</param>
		/// <param name="z">인덱스 z</param>
		/// <returns>유효한 인덱스라면 true를 반환합니다.</returns>
		public bool IsValidIndex(int x, int y, int z)
		{
			return !(x < 0 || x >= SizeX || y < 0 || y >= SizeY || z < 0 || z > SizeZ);
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			int length = mMask.Length;

			for (int i = 0; i < length; i++)
			{
				sb.Append($"{Convert.ToString(mMask[i], 2).PadLeft(INT_BIT_COUNT, '0')}");

				if (i < length - 1)
				{
					sb.Append('\n');
				}
			}

			return sb.ToString();
		}
	}

	public class Bitmask
	{
		private byte mMask;

		/// <summary>전체 비트의 크기입니다.</summary>
		public const int Size = 8;

		/// <summary>Byte수 만큼 비트 마스크를 생성합니다.</summary>
		/// <param name="memoryStride1D">차지할 메모리 공간의 byte수 입니다. Size = byte * 8</param>
		/// <param name="value">초기화 할 값</param>
		public Bitmask(bool value = false)
		{
			Clear(value);
		}

		/// <summary>해당 비트를 참조합니다.</summary>
		/// <param name="x">bitmask 인덱스</param>
		/// <returns>값</returns>
		public bool this[int index]
		{
			get
			{
				return (mMask & (1 << index)) != 0;
			}
			set
			{
				if (value)
				{
					mMask |= (byte)(1 << index);
				}
				else
				{
					mMask &= (byte)~(1 << index);
				}
			}
		}

		public Bitmask Clone()
		{
			Bitmask copy = new Bitmask(false);
			copy.mMask = this.mMask;
			return copy;
		}

		/// <summary>모든 비트를 value로 초기화합니다.</summary>
		/// <param name="value">초기화할 값</param>
		public void Clear(bool value = false)
		{
			mMask = (byte)(value ? 255 : 0);
		}

		/// <summary>모든 비트를 뒤집습니다.</summary>
		public void Flip()
		{
			mMask = (byte)~mMask;
		}

		/// <summary>해당 index의 비트를 반환 시도합니다.</summary>
		/// <param name="index">인덱스 x</param>
		/// <param name="value">값</param>
		/// <returns>반환에 성공하면 true를 반환합니다.</returns>
		public bool TryGetValue(int index, out bool value)
		{
			if (IsValidIndex(index))
			{
				value = this[index];
				return true;
			}

			value = false;
			return false;
		}

		/// <summary>해당 index의 비트를 value로 설정 시도합니다.</summary>
		/// <param name="index">인덱스 x</param>
		/// <param name="value">값</param>
		/// <returns>설정에 성공하면 true를 반환합니다.</returns>
		public bool TrySet(int index, bool value)
		{
			if (IsValidIndex(index))
			{
				this[index] = value;
				return true;
			}

			return false;
		}

		/// <summary>해당 index의 비트를 false로 설정 시도합니다.</summary>
		/// <param name="index">인덱스 x</param>
		/// <returns>설정에 성공하면 true를 반환합니다.</returns>
		public bool TrySetFalse(int index)
		{
			if (IsValidIndex(index))
			{
				SetFalse(index);
				return true;
			}

			return false;
		}

		/// <summary>해당 index의 비트를 true로 설정 시도합니다.</summary>
		/// <param name="index">인덱스 x</param>
		/// <returns>설정에 성공하면 true를 반환합니다.</returns>
		public bool TrySetTrue(int index)
		{
			if (IsValidIndex(index))
			{
				SetTrue(index);
				return true;
			}

			return false;
		}

		/// <summary>해당 index의 비트를 false로 설정합니다.</summary>
		/// <param name="x">인덱스 x</param>
		public void SetFalse(int index)
		{
			mMask &= (byte)~(1 << index);
		}

		/// <summary>해당 index의 비트를 true로 설정합니다.</summary>
		/// <param name="x">인덱스</param>
		public void SetTrue(int index)
		{
			mMask |= (byte)(1 << index);
		}

		/// <summary>모든 비트가 true라면 true를 반환합니다.</summary>
		public bool IsAllTrue()
		{
			return mMask == 255;
		}

		/// <summary>모든 비트가 false라면 true를 반환합니다.</summary>
		public bool IsAllFalse()
		{
			return mMask == 0;
		}

		/// <summary>인덱스가 유효한 범위인지 검사합니다.</summary>
		/// <param name="index">인덱스 x</param>
		/// <returns>유효한 인덱스라면 true를 반환합니다.</returns>
		public bool IsValidIndex(int index)
		{
			return !(index >= Size || index < 0);
		}

		public override string ToString()
		{
			return $"{Convert.ToString(mMask, 2).PadLeft(Size, '0')}";
		}
	}
}
