using System;
using System.Text;
using KaNet.Synchronizers;

namespace KaNet.Utils
{
	public class NetPacketReader
	{
		private NetPacket mNetPacket;
		private ArraySegment<byte> mPacketRawData;
		public NetBaseHeader PacketNetBaseHeader { get; private set; }
		public NetSyncHeader PacketSyncHeader { get; private set; }

		public int ReadIndex { get; private set; }
		public byte[] RawMemory => mNetPacket.RawMemory;

		public NetPacketReader(NetPacket packet)
		{
			SetNetPacket(packet);
		}

		/// <summary>Reader에 Header정보를 바인딩합니다. Reader로 패킷을 파싱할 때 사용할 수 있습니다.</summary>
		public void BindBaseHeader(NetBaseHeader header)
		{
			PacketNetBaseHeader = header;
		}

		public void BindSyncHeader(NetSyncHeader syncHeader)
		{
			PacketSyncHeader = syncHeader;
		}

		public void SetNetPacket(NetPacket packet)
		{
			mNetPacket = packet;
			mPacketRawData = mNetPacket.Data;
			ReadIndex = 0;
		}

		public void Release()
		{
			mNetPacket = null;
			mPacketRawData = null;
			ReadIndex = 0;
		}

		public void ResetReadIndex()
		{
			ReadIndex = 0;
		}

		public void OffsetReadIndex(int offset)
		{
			ReadIndex += offset;
		}

		public void SetIndex(int index)
		{
			ReadIndex = index;
		}

		public bool CanRead(int size)
		{
			return ReadIndex + size <= mNetPacket.Size;
		}

		public bool CanReadString() => CanReadBytes();

		public bool CanReadBytes()
		{
			if (!CanRead(2))
				return false;

			int byteSize = PeekUInt16();

			return CanRead(2 + byteSize);
		}

		// Reading

		public bool ReadBool()
		{
			ReadIndex += DataConverter.DecodeBool(mPacketRawData, ReadIndex, out bool data);
			return data;
		}

		public sbyte ReadInt8()
		{
			ReadIndex += DataConverter.DecodeInt8(mPacketRawData, ReadIndex, out var data);
			return data;
		}

		public byte ReadUInt8()
		{
			ReadIndex += DataConverter.DecodeUInt8(mPacketRawData, ReadIndex, out var data);
			return data;
		}

		public short ReadInt16()
		{
			ReadIndex += DataConverter.DecodeInt16(mPacketRawData, ReadIndex, out var data);
			return data;
		}

		public ushort ReadUInt16()
		{
			ReadIndex += DataConverter.DecodeUInt16(mPacketRawData, ReadIndex, out var data);
			return data;
		}

		public int ReadInt32()
		{
			ReadIndex += DataConverter.DecodeInt32(mPacketRawData, ReadIndex, out var data);
			return data;
		}

		public uint ReadUInt32()
		{
			ReadIndex += DataConverter.DecodeUInt32(mPacketRawData, ReadIndex, out var data);
			return data;
		}

		public long ReadInt64()
		{
			ReadIndex += DataConverter.DecodeInt64(mPacketRawData, ReadIndex, out var data);
			return data;
		}

		public ulong ReadUInt64()
		{
			ReadIndex += DataConverter.DecodeUInt64(mPacketRawData, ReadIndex, out var data);
			return data;
		}

		public float ReadFloat()
		{
			ReadIndex += DataConverter.DecodeFloat(mPacketRawData, ReadIndex, out var data);
			return data;
		}

		public double ReadDouble()
		{
			ReadIndex += DataConverter.DecodeDouble(mPacketRawData, ReadIndex, out var data);
			return data;
		}

		public byte[] ReadBytes()
		{
			int curReadIndex = ReadIndex;
			curReadIndex += DataConverter.DecodeInt16(mPacketRawData, ReadIndex, out short dataLength);
			byte[] rawData = new byte[dataLength];
			Buffer.BlockCopy(mPacketRawData.Array, mPacketRawData.Offset + curReadIndex, rawData, 0, dataLength);
			ReadIndex = curReadIndex + dataLength;
			return rawData;
		}

		public string ReadString()
		{
			var rawData = ReadBytes();
			return Encoding.UTF8.GetString(rawData);
		}

		// Try Reading

		public bool TryReadBool(out bool data)
		{
			if (!CanRead(1))
			{
				data = false;
				return false;
			}

			data = ReadBool();
			return true;
		}

		public bool TryReadInt8(out sbyte data)
		{
			if (!CanRead(1))
			{
				data = 0;	
				return false;
			}

			data = ReadInt8();
			return true;
		}

		public bool TryReadUInt8(out byte data)
		{
			if (!CanRead(1))
			{
				data = 0;
				return false;
			}

			data = ReadUInt8();
			return true;
		}

		public bool TryReadInt16(out short data)
		{
			if (!CanRead(2))
			{
				data = 0;
				return false;
			}

			data = ReadInt16();
			return true;
		}

		public bool TryReadUInt16(out ushort data)
		{
			if (!CanRead(2))
			{
				data = 0;
				return false;
			}

			data = ReadUInt16();
			return true;
		}

		public bool TryReadInt32(out int data)
		{
			if (!CanRead(4))
			{
				data = 0;
				return false;
			}

			data = ReadInt32();
			return true;
		}

		public bool TryReadUInt32(out uint data)
		{
			if (!CanRead(4))
			{
				data = 0;
				return false;
			}

			data = ReadUInt32();
			return true;
		}

		public bool TryReadInt64(out long data)
		{
			if (!CanRead(8))
			{
				data = 0;
				return false;
			}

			data = ReadInt64();
			return true;
		}

		public bool TryReadUInt64(out ulong data)
		{
			if (!CanRead(8))
			{
				data = 0;
				return false;
			}

			data = ReadUInt64();
			return true;
		}

		public bool TryReadFloat(out float data)
		{
			if (!CanRead(4))
			{
				data = 0;
				return false;
			}

			data = ReadFloat();
			return true;
		}

		public bool TryReadDouble(out double data)
		{
			if (!CanRead(8))
			{
				data = 0;
				return false;
			}

			data = ReadDouble();
			return true;
		}

		public bool TryReadBytes(out byte[] data)
		{
			if (!CanReadBytes())
			{
				data = null;
				return false;
			}

			data = ReadBytes();
			return true;
		}

		public bool TryReadString(out string data)
		{
			if (!CanReadString())
			{
				data = null;
				return false;
			}

			data = ReadString();
			return true;
		}

		// Peeking

		public sbyte PeekInt8()
		{
			DataConverter.DecodeInt8(mPacketRawData, ReadIndex, out var data);
			return data;
		}

		public byte PeekUInt8()
		{
			DataConverter.DecodeUInt8(mPacketRawData, ReadIndex, out var data);
			return data;
		}

		public short PeekInt16()
		{
			DataConverter.DecodeInt16(mPacketRawData, ReadIndex, out var data);
			return data;
		}

		public ushort PeekUInt16()
		{
			DataConverter.DecodeUInt16(mPacketRawData, ReadIndex, out var data);
			return data;
		}

		public int PeekInt32()
		{
			DataConverter.DecodeInt32(mPacketRawData, ReadIndex, out var data);
			return data;
		}

		public uint PeekUInt32()
		{
			DataConverter.DecodeUInt32(mPacketRawData, ReadIndex, out var data);
			return data;
		}

		public long PeekInt64()
		{
			DataConverter.DecodeInt64(mPacketRawData, ReadIndex, out var data);
			return data;
		}

		public ulong PeekUInt64()
		{
			DataConverter.DecodeUInt64(mPacketRawData, ReadIndex, out var data);
			return data;
		}

		public float PeekFloat()
		{
			DataConverter.DecodeFloat(mPacketRawData, ReadIndex, out var data);
			return data;
		}

		public double PeekDouble()
		{
			DataConverter.DecodeDouble(mPacketRawData, ReadIndex, out var data);
			return data;
		}
	}
}
