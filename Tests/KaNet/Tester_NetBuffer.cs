using KaNet.Utils;
using NUnit.Framework;
using UnityEngine;
using Utils;

public class Tester_NetBuffer : MonoBehaviour
{
    [Test]
    public void Test_NetBuffer()
    {
        // Buffer
        NetBuffer netBuffer = new NetBuffer();

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

        // Encoding values
        netBuffer.Write(dataIn_Int8);
        netBuffer.Write(dataIn_UInt8);

        netBuffer.Write(dataIn_Int16);
        netBuffer.Write(dataIn_UInt16);

        netBuffer.Write(dataIn_Int32);
        netBuffer.Write(dataIn_UInt32);

        netBuffer.Write(dataIn_Int64);
        netBuffer.Write(dataIn_UInt64);

        netBuffer.Write(dataIn_String);
        netBuffer.Write(dataIn_Bytes);

        netBuffer.Write(dataIn_Float);
        netBuffer.Write(dataIn_Double);

        // Decoding values
        int readOffset = 0;

        readOffset += netBuffer.ReadInt8(readOffset, out var dataOut_Int8);
        readOffset += netBuffer.ReadUInt8(readOffset, out var dataOut_UInt8);

        readOffset += netBuffer.ReadInt16(readOffset, out var dataOut_Int16);
        readOffset += netBuffer.ReadUInt16(readOffset, out var dataOut_UInt16);

        readOffset += netBuffer.ReadInt32(readOffset, out var dataOut_Int32);
        readOffset += netBuffer.ReadUInt32(readOffset, out var dataOut_UInt32);

        readOffset += netBuffer.ReadInt64(readOffset, out var dataOut_Int64);
        readOffset += netBuffer.ReadUInt64(readOffset, out var dataOut_UInt64);

        readOffset += netBuffer.ReadString(readOffset, out var dataOut_String);
        readOffset += netBuffer.ReadBytes(readOffset, out var dataOut_Bytes);

        readOffset += netBuffer.ReadFloat(readOffset, out var dataOut_Float);
        readOffset += netBuffer.ReadDouble(readOffset, out var dataOut_Double);

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
    public void Test_NetBufferReserve()
    {
        NetBuffer netBuffer = new NetBuffer(100);
        Assert.AreEqual(100, netBuffer.Capacity);

        netBuffer.Reserve(101);
        Assert.AreEqual(200, netBuffer.Capacity);

        netBuffer.Reserve(200);
        Assert.AreEqual(200, netBuffer.Capacity);

        netBuffer.Reserve(300);
        Assert.AreEqual(400, netBuffer.Capacity);

        netBuffer.Reserve(1000);
        Assert.AreEqual(1000, netBuffer.Capacity);

        var bufferData = netBuffer.BufferData;
    }
}