using KaNet.Core;
using KaNet.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace KaNet.Utils
{
	public class NetBuffer : IManageable
	{
		// Properties
		private byte[] mBufferData;
		public int Size { get; private set; } = 0;
		public int Capacity { get; private set; } = 0;

		// Getters
		public bool IsEmpty() => Size <= 0;
		public ArraySegment<byte> BufferData => new ArraySegment<byte>(mBufferData, 0, Size);
		public byte[] RawBufferData => mBufferData;
		public IEnumerator GetEnumerator() => BufferData.GetEnumerator();
		public byte this[int index]
		{
			get { return mBufferData[index]; }
		}

		public NetBuffer()
		{
			Capacity = 8;
			mBufferData = new byte[Capacity];
		}
		public NetBuffer(int capacity)
		{
			Capacity = capacity;
			mBufferData = new byte[Capacity];
		}
		public NetBuffer(in NetBuffer netBuffer)
		{
			Size = netBuffer.Size;
			Capacity = netBuffer.Capacity;
			mBufferData = new byte[Capacity];
			Buffer.BlockCopy(netBuffer.mBufferData, 0, this.mBufferData, 0, Size);
		}
		public NetBuffer(byte[] data)
		{
			mBufferData = data;
			Size = data.Length;
			Capacity = data.Length;
		}

		public NetBufferReader GetReader()
		{
			return new NetBufferReader(this);
		}

		public void OnInitialize() => Clear();

		public void OnFinalize() => Clear();

		public void Clear()
		{
			Size = 0;
		}

		public void ForceSetSize(int forceSize)
		{
			if (forceSize > Capacity)
			{
				throw new ArgumentOutOfRangeException("NetBuffer의 Capacity를 넘어서는 Size를 강제로 설정했습니다.");
			}

			Size = forceSize;
		}

		/// <summary>버퍼의 특정 영역을 복제한 Bytes배열을 얻습니다.</summary>
		public bool TryCopyBytes(int offset, int count, out byte[] data)
		{
			if (Size < offset + count)
			{
				data = null;
				return false;
			}

			data = new byte[count];
			Buffer.BlockCopy(mBufferData, offset, data, 0, count);
			return true;
		}

		/// <summary>버퍼의 특정 영역을 복제한 Bytes배열을 얻습니다.</summary>
		public byte[] CopyBytes(int offset, int count)
		{
			byte[] data = new byte[count];
			Buffer.BlockCopy(mBufferData, offset, data, 0, count);
			return data;
		}

		/// <summary>버퍼의 메모리 크기를 늘립니다. 기본적으로 현재 Capacity의 두배씩 늘립니다.</summary>
		/// <param name="capacityNeed">필요한 메모리의 크기입니다.</param>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public void Reserve(int capacityNeed)
		{
			if (capacityNeed < 0)
			{
				throw new ArgumentOutOfRangeException($"Invalid reserve capacity! Capacity : {capacityNeed}");
			}

			if (capacityNeed > Capacity)
			{
				int doubleCapacity = Capacity * 2;
				Capacity = doubleCapacity < capacityNeed ? capacityNeed : doubleCapacity;
				byte[] newBuffer = new byte[Capacity];
				Buffer.BlockCopy(mBufferData, 0, newBuffer, 0, Size);
				mBufferData = newBuffer;
			}
		}

		/// <summary>지정한 크기로 일정하게 나눈 ArraySegment의 List를 반환받습니다.</summary>
		/// <param name="divideSize">나눌 크기입니다.</param>
		/// <returns>나누어진 버퍼의 참조 ArraySegment입니다.</returns>
		/// <exception cref="ArgumentException"></exception>
		public List<ArraySegment<byte>> GetSegmentByDivideSize(int divideSize)
		{
			if (divideSize <= 0)
			{
				throw new ArgumentException("Buffer를 0이하로 나눌 수 없습니다.");
			}

			int segmentCount = Size / divideSize;

			List<ArraySegment<byte>> segmentList = new List<ArraySegment<byte>>();

			int offset = 0;

			// Add divided buffer
			for (int i = 0; i < segmentCount; i++)
			{
				ArraySegment<byte> bufferSegment = new ArraySegment<byte>(mBufferData, offset, divideSize);
				segmentList.Add(bufferSegment);
				offset += divideSize;
			}

			// Add remain of buffer
			if (offset < Size)
			{
				int remainLength = Size - offset;
				ArraySegment<byte> bufferSegment = new ArraySegment<byte>(mBufferData, offset, remainLength);
				segmentList.Add(bufferSegment);
			}

			return segmentList;
		}

		#region Write Operation

		public void Write(sbyte data) => WriteInt8(data);
		public void Write(byte data) => WriteUInt8(data);
		public void Write(short data) => WriteInt16(data);
		public void Write(ushort data) => WriteUInt16(data);
		public void Write(int data) => WriteInt32(data);
		public void Write(uint data) => WriteUInt32(data);
		public void Write(long data) => WriteInt64(data);
		public void Write(ulong data) => WriteUInt64(data);
		public void Write(float data) => WriteFloat(data);
		public void Write(double data) => WriteDouble(data);
		public void Write(string data) => WriteString(data);
		public void Write(byte[] data) => WriteBytes(data);

		public void WriteInt8(sbyte data)
		{
			Reserve(Size + 1);
			Size += DataConverter.EncodeInt8(mBufferData, Size, data);
		}

		public void WriteUInt8(byte data)
		{
			Reserve(Size + 1);
			Size += DataConverter.EncodeUInt8(mBufferData, Size, data);
		}

		public void WriteInt16(short data)
		{
			Reserve(Size + 2);
			Size += DataConverter.EncodeInt16(mBufferData, Size, data);
		}

		public void WriteUInt16(ushort data)
		{
			Reserve(Size + 2);
			Size += DataConverter.EncodeUInt16(mBufferData, Size, data);
		}

		public void WriteInt32(int data)
		{
			Reserve(Size + 4);
			Size += DataConverter.EncodeInt32(mBufferData, Size, data);
		}

		public void WriteUInt32(uint data)
		{
			Reserve(Size + 4);
			Size += DataConverter.EncodeUInt32(mBufferData, Size, data);
		}

		public void WriteInt64(long data)
		{
			Reserve(Size + 8);
			Size += DataConverter.EncodeInt64(mBufferData, Size, data);
		}

		public void WriteUInt64(ulong data)
		{
			Reserve(Size + 8);
			Size += DataConverter.EncodeUInt64(mBufferData, Size, data);
		}

		public void WriteFloat(float data)
		{
			Reserve(Size + 4);
			Size += DataConverter.EncodeFloat(mBufferData, Size, data);
		}

		public void WriteDouble(double data)
		{
			Reserve(Size + 8);
			Size += DataConverter.EncodeDouble(mBufferData, Size, data);
		}

		public void WriteBytes(byte[] data)
		{
			Reserve(Size + 2 + data.Length);
			Size += DataConverter.EncodeBytes(mBufferData, Size, data);
		}

		public void WriteString(string data)
		{
			var rawData = Encoding.UTF8.GetBytes(data);
			WriteBytes(rawData);
		}

		#endregion

		#region Read Operation

		public int ReadInt8(int offset, out sbyte data) => DataConverter.DecodeInt8(mBufferData, offset, out data);
		public int ReadUInt8(int offset, out byte data) => DataConverter.DecodeUInt8(mBufferData, offset, out data);
		public int ReadInt16(int offset, out short data) => DataConverter.DecodeInt16(mBufferData, offset, out data);
		public int ReadUInt16(int offset, out ushort data) => DataConverter.DecodeUInt16(mBufferData, offset, out data);
		public int ReadInt32(int offset, out int data) => DataConverter.DecodeInt32(mBufferData, offset, out data);
		public int ReadUInt32(int offset, out uint data) => DataConverter.DecodeUInt32(mBufferData, offset, out data);
		public int ReadInt64(int offset, out long data) => DataConverter.DecodeInt64(mBufferData, offset, out data);
		public int ReadUInt64(int offset, out ulong data) => DataConverter.DecodeUInt64(mBufferData, offset, out data);
		public int ReadFloat(int offset, out float data) => DataConverter.DecodeFloat(mBufferData, offset, out data);
		public int ReadDouble(int offset, out double data) => DataConverter.DecodeDouble(mBufferData, offset, out data);
		public int ReadString(int offset, out string data) => DataConverter.DecodeString(mBufferData, offset, out data);
		public int ReadBytes(int offset, out byte[] data) => DataConverter.DecodeBytes(mBufferData, offset, out data);

		#endregion
	}
}
