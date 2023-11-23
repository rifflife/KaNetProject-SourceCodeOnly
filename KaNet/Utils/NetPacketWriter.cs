using System;
using System.Text;
using KaNet.Synchronizers;

namespace KaNet.Utils
{
	public class NetPacketWriter
	{
		private NetPacket mNetPacket;
		private ArraySegment<byte> mPacketRawData;

		public int WriteIndex => mNetPacket.Size;
		public byte[] RawMemory => mNetPacket.RawMemory;

		public NetPacketWriter() { }
		public NetPacketWriter(NetPacket packet)
		{
			mNetPacket = packet;
			mPacketRawData = mNetPacket.Data;
		}
		
		public void SetNetPacket(NetPacket packet)
		{
			mNetPacket = packet;
			mPacketRawData = mNetPacket.Data;
		}

		public void Release()
		{
			mNetPacket = null;
			mPacketRawData = null;
		}

		public int GetRemainingSize()
		{
			return mNetPacket.RemainingSize;
		}

		public bool CanWriteString(string data)
		{
			return CanWrite(Encoding.UTF8.GetByteCount(data) + 2);
		}

		public bool CanWriteBytes(byte[] data)
		{
			return CanWrite(data.Length + 2);
		}

		public bool CanWrite(int size)
		{
			return mNetPacket.Size + size <= mNetPacket.MaxSize;
		}

		public void OffsetWriteIndex(int offset)
		{
			mNetPacket.Size += offset;
		}

		public void MoveWriteIndex(int index)
		{
			mNetPacket.Size = index;
		}

		// Header

		public void WriteAt(INetworkSerializable data, int index)
		{
			int tempSize = mNetPacket.Size;
			mNetPacket.Size = index;
			data.SerializeTo(this);

			mNetPacket.Size = tempSize;
			if (mNetPacket.Size < index + data.GetSyncDataSize())
			{
				mNetPacket.Size = index + data.GetSyncDataSize();
			}
		}

		// Custom Write

		public bool TryWritePacket(NetPacket packet)
		{
			if (!CanWrite(packet.Size))
			{
				return false;
			}

			WritePacket(packet);
			return true;
		}

		public void WritePacket(NetPacket packet)
		{
			Buffer.BlockCopy
			(
				packet.Data.Array,
				packet.Data.Offset,
				mPacketRawData.Array,
				mPacketRawData.Offset + mNetPacket.Size,
				packet.Size
			);
			mNetPacket.Size += packet.Size;
		}

		public void WritePacket(NetPacket packet, int offset, int count)
		{
			Buffer.BlockCopy
			(
				packet.Data.Array,
				packet.Data.Offset + offset,
				mPacketRawData.Array,
				mPacketRawData.Offset,
				count
			);

			mNetPacket.Size += count;
		}

		// Default Write

		public void WriteRawData(byte[] data, int offest, int count)
		{
			Buffer.BlockCopy(data, offest, mPacketRawData.Array, mPacketRawData.Offset, count);
			mNetPacket.Size += count;
		}

		public void WriteBool(bool data)
		{
			mNetPacket.Size += DataConverter.EncodeBool(mPacketRawData, mNetPacket.Size, data);
		}

		public void WriteInt8(sbyte data)
		{
			mNetPacket.Size += DataConverter.EncodeInt8(mPacketRawData, mNetPacket.Size, data);
		}

		public void WriteUInt8(byte data)
		{
			mNetPacket.Size += DataConverter.EncodeUInt8(mPacketRawData, mNetPacket.Size, data);
		}

		public void WriteInt16(short data)
		{
			mNetPacket.Size += DataConverter.EncodeInt16(mPacketRawData, mNetPacket.Size, data);
		}

		public void WriteUInt16(ushort data)
		{
			mNetPacket.Size += DataConverter.EncodeUInt16(mPacketRawData, mNetPacket.Size, data);
		}

		public void WriteInt32(int data)
		{
			mNetPacket.Size += DataConverter.EncodeInt32(mPacketRawData, mNetPacket.Size, data);
		}

		public void WriteUInt32(uint data)
		{
			mNetPacket.Size += DataConverter.EncodeUInt32(mPacketRawData, mNetPacket.Size, data);
		}

		public void WriteInt64(long data)
		{
			mNetPacket.Size += DataConverter.EncodeInt64(mPacketRawData, mNetPacket.Size, data);
		}

		public void WriteUInt64(ulong data)
		{
			mNetPacket.Size += DataConverter.EncodeUInt64(mPacketRawData, mNetPacket.Size, data);
		}

		public void WriteFloat(float data)
		{
			mNetPacket.Size += DataConverter.EncodeFloat(mPacketRawData, mNetPacket.Size, data);
		}

		public void WriteDouble(double data)
		{
			mNetPacket.Size += DataConverter.EncodeDouble(mPacketRawData, mNetPacket.Size, data);
		}

		public void WriteBytes(byte[] data)
		{
			mNetPacket.Size += DataConverter.EncodeBytes(mPacketRawData, mNetPacket.Size, data);
		}

		public void WriteString(string data)
		{
			mNetPacket.Size += DataConverter.EncodeString(mPacketRawData, mNetPacket.Size, data);
		}

		public void WriteUInt8(byte data, int offset)
		{
			DataConverter.EncodeUInt8(mPacketRawData, offset, data);
		}

		public void WriteUInt16(ushort data, int offset)
		{
			DataConverter.EncodeUInt16(mPacketRawData, offset, data);
		}

		// Write Overloading

		public void Write(bool data) => WriteBool(data);
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
	}
}
