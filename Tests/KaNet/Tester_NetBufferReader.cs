using KaNet.Utils;
using NUnit.Framework;
using UnityEngine;
using Utils;

public class Tester_NetBufferReader : MonoBehaviour
{
    [Test]
    public void Test_NetBufferReader()
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

        // Write values
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

        // Test Net Buffer Reader
        var reader = netBuffer.GetReader();

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

        Assert.IsTrue(reader.TryReadInt8(out var dataOutAgain_Int8));
        Assert.IsTrue(reader.TryReadUInt8(out var dataOutAgain_UInt8));

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

        Assert.AreEqual(dataIn_Int8, dataOutAgain_Int8);
        Assert.AreEqual(dataIn_UInt8, dataOutAgain_UInt8);
    }

    [Test]
    public void Test_NetBufferReader_OneByOne()
    {
        NetBuffer netBuffer = new NetBuffer();
        NetBufferReader reader;

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
        netBuffer.WriteInt8(dataIn_Int8);
        reader = netBuffer.GetReader();
        Assert.IsTrue(reader.TryReadInt8(out var dataOut_Int8));
        netBuffer.Clear();

        netBuffer.WriteUInt8(dataIn_UInt8);
        reader = netBuffer.GetReader();
        Assert.IsTrue(reader.TryReadUInt8(out var dataOut_UInt8));
        netBuffer.Clear();

        netBuffer.WriteInt16(dataIn_Int16);
        reader = netBuffer.GetReader();
        Assert.IsTrue(reader.TryReadInt16(out var dataOut_Int16));
        netBuffer.Clear();

        netBuffer.WriteUInt16(dataIn_UInt16);
        reader = netBuffer.GetReader();
        Assert.IsTrue(reader.TryReadUInt16(out var dataOut_UInt16));
        netBuffer.Clear();

        netBuffer.WriteInt32(dataIn_Int32);
        reader = netBuffer.GetReader();
        Assert.IsTrue(reader.TryReadInt32(out var dataOut_Int32));
        netBuffer.Clear();

        netBuffer.WriteUInt32(dataIn_UInt32);
        reader = netBuffer.GetReader();
        Assert.IsTrue(reader.TryReadUInt32(out var dataOut_UInt32));
        netBuffer.Clear();

        netBuffer.WriteInt64(dataIn_Int64);
        reader = netBuffer.GetReader();
        Assert.IsTrue(reader.TryReadInt64(out var dataOut_Int64));
        netBuffer.Clear();

        netBuffer.WriteUInt64(dataIn_UInt64);
        reader = netBuffer.GetReader();
        Assert.IsTrue(reader.TryReadUInt64(out var dataOut_UInt64));
        netBuffer.Clear();

        netBuffer.WriteString(dataIn_String);
        reader = netBuffer.GetReader();
        Assert.IsTrue(reader.TryReadString(out var dataOut_String));
        netBuffer.Clear();

        netBuffer.WriteBytes(dataIn_Bytes);
        reader = netBuffer.GetReader();
        Assert.IsTrue(reader.TryReadBytes(out var dataOut_Bytes));
        netBuffer.Clear();

        netBuffer.WriteFloat(dataIn_Float);
        reader = netBuffer.GetReader();
        Assert.IsTrue(reader.TryReadFloat(out var dataOut_Float));
        netBuffer.Clear();

        netBuffer.WriteDouble(dataIn_Double);
        reader = netBuffer.GetReader();
        Assert.IsTrue(reader.TryReadDouble(out var dataOut_Double));
        netBuffer.Clear();

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
}
