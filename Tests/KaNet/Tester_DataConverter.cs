using System;
using KaNet.Utils;
using NUnit.Framework;
using UnityEngine;
using Utils;

public class Tester_DataConverter : MonoBehaviour
{
    [Test]
    public void Test_DataConverting()
    {
        // Buffer
        byte[] buffer = new byte[Numeric.KiB];
        int offset;

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
        offset = 0;

        offset += DataConverter.EncodeBool(buffer, offset, true);

        offset += DataConverter.EncodeInt8(buffer, offset, dataIn_Int8);
        offset += DataConverter.EncodeUInt8(buffer, offset, dataIn_UInt8);

        offset += DataConverter.EncodeInt16(buffer, offset, dataIn_Int16);
        offset += DataConverter.EncodeUInt16(buffer, offset, dataIn_UInt16);

        offset += DataConverter.EncodeInt32(buffer, offset, dataIn_Int32);
        offset += DataConverter.EncodeUInt32(buffer, offset, dataIn_UInt32);

        offset += DataConverter.EncodeInt64(buffer, offset, dataIn_Int64);
        offset += DataConverter.EncodeUInt64(buffer, offset, dataIn_UInt64);

        offset += DataConverter.EncodeString(buffer, offset, dataIn_String);
        offset += DataConverter.EncodeBytes(buffer, offset, dataIn_Bytes);

        offset += DataConverter.EncodeFloat(buffer, offset, dataIn_Float);
        offset += DataConverter.EncodeDouble(buffer, offset, dataIn_Double);

        // Decoding values
        offset = 0;

        offset += DataConverter.DecodeBool(buffer, offset, out var dataOut_bool);

        offset += DataConverter.DecodeInt8(buffer, offset, out var dataOut_Int8);
        offset += DataConverter.DecodeUInt8(buffer, offset, out var dataOut_UInt8);

        offset += DataConverter.DecodeInt16(buffer, offset, out var dataOut_Int16);
        offset += DataConverter.DecodeUInt16(buffer, offset, out var dataOut_UInt16);

        offset += DataConverter.DecodeInt32(buffer, offset, out var dataOut_Int32);
        offset += DataConverter.DecodeUInt32(buffer, offset, out var dataOut_UInt32);

        offset += DataConverter.DecodeInt64(buffer, offset, out var dataOut_Int64);
        offset += DataConverter.DecodeUInt64(buffer, offset, out var dataOut_UInt64);

        offset += DataConverter.DecodeString(buffer, offset, out var dataOut_String);
        offset += DataConverter.DecodeBytes(buffer, offset, out var dataOut_Bytes);

        offset += DataConverter.DecodeFloat(buffer, offset, out var dataOut_Float);
        offset += DataConverter.DecodeDouble(buffer, offset, out var dataOut_Double);

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
    }


    [Test]
    public void Test_DataConvertingArraySegment()
    {
        // Buffer
        byte[] bufferData = new byte[Numeric.KiB];
        ArraySegment<byte> buffer = new ArraySegment<byte>(bufferData);
        int offset;

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
        offset = 0;

        offset += DataConverter.EncodeBool(buffer, offset, true);

        offset += DataConverter.EncodeInt8(buffer, offset, dataIn_Int8);
        offset += DataConverter.EncodeUInt8(buffer, offset, dataIn_UInt8);

        offset += DataConverter.EncodeInt16(buffer, offset, dataIn_Int16);
        offset += DataConverter.EncodeUInt16(buffer, offset, dataIn_UInt16);

        offset += DataConverter.EncodeInt32(buffer, offset, dataIn_Int32);
        offset += DataConverter.EncodeUInt32(buffer, offset, dataIn_UInt32);

        offset += DataConverter.EncodeInt64(buffer, offset, dataIn_Int64);
        offset += DataConverter.EncodeUInt64(buffer, offset, dataIn_UInt64);

        offset += DataConverter.EncodeString(buffer, offset, dataIn_String);
        offset += DataConverter.EncodeBytes(buffer, offset, dataIn_Bytes);

        offset += DataConverter.EncodeFloat(buffer, offset, dataIn_Float);
        offset += DataConverter.EncodeDouble(buffer, offset, dataIn_Double);

        // Decoding values
        offset = 0;

        offset += DataConverter.DecodeBool(buffer, offset, out var dataOut_bool);

        offset += DataConverter.DecodeInt8(buffer, offset, out var dataOut_Int8);
        offset += DataConverter.DecodeUInt8(buffer, offset, out var dataOut_UInt8);

        offset += DataConverter.DecodeInt16(buffer, offset, out var dataOut_Int16);
        offset += DataConverter.DecodeUInt16(buffer, offset, out var dataOut_UInt16);

        offset += DataConverter.DecodeInt32(buffer, offset, out var dataOut_Int32);
        offset += DataConverter.DecodeUInt32(buffer, offset, out var dataOut_UInt32);

        offset += DataConverter.DecodeInt64(buffer, offset, out var dataOut_Int64);
        offset += DataConverter.DecodeUInt64(buffer, offset, out var dataOut_UInt64);

        offset += DataConverter.DecodeString(buffer, offset, out var dataOut_String);
        offset += DataConverter.DecodeBytes(buffer, offset, out var dataOut_Bytes);

        offset += DataConverter.DecodeFloat(buffer, offset, out var dataOut_Float);
        offset += DataConverter.DecodeDouble(buffer, offset, out var dataOut_Double);

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
    }
}
