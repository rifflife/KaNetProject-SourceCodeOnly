using System;
using KaNet.Utils;
using NUnit.Framework;
using UnityEngine;
using Utils;

public class Tester_NetPacket : MonoBehaviour
{
    [Test]
    public void Test_NetPacketReader()
    {
        // Buffer
        PacketPool.TryAllocateForTest(500, 5);

        NetPacket packet = PacketPool.GetMtuPacket();

        // Test in variables
        sbyte dataIn_Int8 = -123;
        byte dataIn_UInt8 = 123;

        short dataIn_Int16 = -12345;
        ushort dataIn_UInt16 = 12345;

        int dataIn_Int32 = -123456789;
        uint dataIn_UInt32 = 123456789U;

        long dataIn_Int64 = -12345678910L;
        ulong dataIn_UInt64 = 12345678910UL;

        string dataIn_String = "æ»≥Á«œººø‰? ABCDE 12345 abcde! §ª§ª";
        byte[] dataIn_Bytes = new byte[] { 123, 105, 64, 53, 12, 201, 222, 213 };

        float dataIn_Float = 12345.12345F;
        double dataIn_Double = -12345.12345;

        // Write values
        var writer = packet.GetWriter();

        writer.Write(true);

        writer.Write(dataIn_Int8);
        writer.Write(dataIn_UInt8);

        writer.Write(dataIn_Int16);
        writer.Write(dataIn_UInt16);

        writer.Write(dataIn_Int32);
        writer.Write(dataIn_UInt32);

        writer.Write(dataIn_Int64);
        writer.Write(dataIn_UInt64);

        writer.Write(dataIn_String);
        writer.Write(dataIn_Bytes);

        writer.Write(dataIn_Float);
        writer.Write(dataIn_Double);

        // Test Net Buffer Reader
        var reader = packet.GetReader();

        Assert.IsTrue(reader.TryReadBool(out var dataOut_bool));

        Assert.IsTrue(reader.TryReadInt8(out var dataOut_Int8));
        Assert.IsTrue(reader.TryReadUInt8(out var dataOut_UInt8));

        Assert.IsTrue(reader.TryReadInt16(out var dataOut_Int16));
        Assert.IsTrue(reader.TryReadUInt16(out var dataOut_UInt16));

        Assert.IsTrue(reader.TryReadInt32(out var dataOut_Int32));
        Assert.IsTrue(reader.TryReadUInt32(out var dataOut_UInt32));

        Assert.IsTrue(reader.TryReadInt64(out var dataOut_Int64));
        Assert.IsTrue(reader.TryReadUInt64(out var dataOut_UInt64));

        Assert.IsTrue(reader.TryReadString(out var dataOut_String));
        Assert.IsTrue(reader.TryReadBytes(out var dataOut_Bytes));

        Assert.IsTrue(reader.TryReadFloat(out var dataOut_Float));
        Assert.IsTrue(reader.TryReadDouble(out var dataOut_Double));

        Assert.IsFalse(reader.TryReadInt8(out var dataWrong1));
        Assert.IsFalse(reader.TryReadInt16(out var dataWrong2));

        reader.ResetReadIndex();
        reader.OffsetReadIndex(1);

        Assert.IsTrue(reader.TryReadInt8(out var dataOutAgain_Int8));
        Assert.IsTrue(reader.TryReadUInt8(out var dataOutAgain_UInt8));

        // Test
        Assert.IsTrue(dataOut_bool);

        Assert.AreEqual(dataIn_Int8, dataOut_Int8);
        Assert.AreEqual(dataIn_UInt8, dataOut_UInt8);

        Assert.AreEqual(dataIn_Int16, dataOut_Int16);
        Assert.AreEqual(dataIn_UInt16, dataOut_UInt16);

        Assert.AreEqual(dataIn_Int32, dataOut_Int32);
        Assert.AreEqual(dataIn_UInt32, dataOut_UInt32);

        Assert.AreEqual(dataIn_Int64, dataOut_Int64);
        Assert.AreEqual(dataIn_UInt64, dataOut_UInt64);

        Assert.AreEqual(dataIn_String, dataOut_String);
        Assert.IsTrue(dataIn_Bytes.IsEqual(dataOut_Bytes));

        Assert.AreEqual(dataIn_Float, dataOut_Float);
        Assert.AreEqual(dataIn_Double, dataOut_Double);

        Assert.AreEqual(dataIn_Int8, dataOutAgain_Int8);
        Assert.AreEqual(dataIn_UInt8, dataOutAgain_UInt8);
    }

    [Test]
    public void Test_NetPacketReader_OneByOne()
    {
        // Buffer
        PacketPool.TryAllocateForTest(500, 5);

        NetPacket packet = PacketPool.GetMtuPacket();

        NetPacketWriter writer = packet.GetWriter();
        NetPacketReader reader;

        // Test in variables
        sbyte dataIn_Int8 = -123;
        byte dataIn_UInt8 = 123;

        short dataIn_Int16 = -12345;
        ushort dataIn_UInt16 = 12345;

        int dataIn_Int32 = -123456789;
        uint dataIn_UInt32 = 123456789U;

        long dataIn_Int64 = -12345678910L;
        ulong dataIn_UInt64 = 12345678910UL;

        string dataIn_String = "æ»≥Á«œººø‰? ABCDE 12345 abcde! §ª§ª";
        byte[] dataIn_Bytes = new byte[] { 123, 105, 64, 53, 12, 201, 222, 213 };

        float dataIn_Float = 12345.12345F;
        double dataIn_Double = -12345.12345;

        // Write and read data one by one
        writer.WriteInt8(dataIn_Int8);
        reader = packet.GetReader();
        Assert.IsTrue(reader.TryReadInt8(out var dataOut_Int8));
        packet.Clear();

        writer.WriteUInt8(dataIn_UInt8);
        reader = packet.GetReader();
        Assert.IsTrue(reader.TryReadUInt8(out var dataOut_UInt8));
        packet.Clear();

        writer.WriteInt16(dataIn_Int16);
        reader = packet.GetReader();
        Assert.IsTrue(reader.TryReadInt16(out var dataOut_Int16));
        packet.Clear();

        writer.WriteUInt16(dataIn_UInt16);
        reader = packet.GetReader();
        Assert.IsTrue(reader.TryReadUInt16(out var dataOut_UInt16));
        packet.Clear();

        writer.WriteInt32(dataIn_Int32);
        reader = packet.GetReader();
        Assert.IsTrue(reader.TryReadInt32(out var dataOut_Int32));
        packet.Clear();

        writer.WriteUInt32(dataIn_UInt32);
        reader = packet.GetReader();
        Assert.IsTrue(reader.TryReadUInt32(out var dataOut_UInt32));
        packet.Clear();

        writer.WriteInt64(dataIn_Int64);
        reader = packet.GetReader();
        Assert.IsTrue(reader.TryReadInt64(out var dataOut_Int64));
        packet.Clear();

        writer.WriteUInt64(dataIn_UInt64);
        reader = packet.GetReader();
        Assert.IsTrue(reader.TryReadUInt64(out var dataOut_UInt64));
        packet.Clear();

        writer.WriteString(dataIn_String);
        reader = packet.GetReader();
        Assert.IsTrue(reader.TryReadString(out var dataOut_String));
        packet.Clear();

        writer.WriteBytes(dataIn_Bytes);
        reader = packet.GetReader();
        Assert.IsTrue(reader.TryReadBytes(out var dataOut_Bytes));
        packet.Clear();

        writer.WriteFloat(dataIn_Float);
        reader = packet.GetReader();
        Assert.IsTrue(reader.TryReadFloat(out var dataOut_Float));
        packet.Clear();

        writer.WriteDouble(dataIn_Double);
        reader = packet.GetReader();
        Assert.IsTrue(reader.TryReadDouble(out var dataOut_Double));
        packet.Clear();

        // Test
        Assert.AreEqual(dataIn_Int8, dataOut_Int8);
        Assert.AreEqual(dataIn_UInt8, dataOut_UInt8);

        Assert.AreEqual(dataIn_Int16, dataOut_Int16);
        Assert.AreEqual(dataIn_UInt16, dataOut_UInt16);

        Assert.AreEqual(dataIn_Int32, dataOut_Int32);
        Assert.AreEqual(dataIn_UInt32, dataOut_UInt32);

        Assert.AreEqual(dataIn_Int64, dataOut_Int64);
        Assert.AreEqual(dataIn_UInt64, dataOut_UInt64);

        Assert.AreEqual(dataIn_String, dataOut_String);
        Assert.IsTrue(dataIn_Bytes.IsEqual(dataOut_Bytes));

        Assert.AreEqual(dataIn_Float, dataOut_Float);
        Assert.AreEqual(dataIn_Double, dataOut_Double);
    }

    [Test]
	public void Test_PacketAppend()
	{
        NetPacket rawPacket = new NetPacket(10);
        var writer = rawPacket.GetWriter();

        writer.Write(true);
        writer.Write(-15);

        byte[] testBuffer = new byte[100];
        NetPacket arrayPacket = new NetPacket(new ArraySegment<byte>(testBuffer, 10, 10));
        arrayPacket.GetWriter().Write(123.456f);

        NetPacket testPacket = new NetPacket(new ArraySegment<byte>(testBuffer, 20, 50));

        var testWriter = testPacket.GetWriter();
        testWriter.WritePacket(arrayPacket);
        testWriter.WritePacket(rawPacket);

        var reader = testPacket.GetReader();

        Assert.AreEqual(123.456f, reader.ReadFloat());
        Assert.IsTrue(reader.ReadBool());
        Assert.AreEqual(-15, reader.ReadInt32());
    }
}
