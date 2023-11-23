using System;
using KaNet.Utils;
using NUnit.Framework;
using UnityEngine;
using Utils;

public class Tester_Quantizer : MonoBehaviour
{
    [Test]
    public void Test_Quantizer()
    {
        byte data_1 = Quantizer.QuantizeFloatToInt8(14.12345f, 32f);
        float result_1 = Quantizer.DequantizeFloatFromInt8(1, 32f);
        //Debug.Log(result_1);

        byte data_2 = Quantizer.QuantizeFloatToInt8(63f, 32f);
        float result_2 = Quantizer.DequantizeFloatFromInt8(data_2, 32f);
        //Debug.Log(result_2);

        int abc = -70000 & (65536 | int.MinValue);

        Debug.Log(abc);
        Debug.Log(Convert.ToString(abc, 2));
        Debug.Log("");

        Debug.Log(Convert.ToString(-70000, 2));
        Debug.Log(Convert.ToString(65535, 2));
        Debug.Log(Convert.ToString(65536, 2));
        Debug.Log(Convert.ToString(65535 | int.MinValue, 2));
        //Debug.Log(Convert.ToString(int.MaxValue, 2));
        //Debug.Log(Convert.ToString(int.MinValue, 2));

        // Debug.Log(abc);
    }
}

// 00000000000000000000000000000001 = 1
// 00000000000000000000000000000000 = 0
// 00000000000000010000000000000000 = 2 << 16
// 11111111111111111111111111111111 = -1
// 01111111111111111111111111111111 = Max
// 10000000000000000000000000000000 = Min