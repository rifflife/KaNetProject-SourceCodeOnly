using System;
using KaNet.Synchronizers;
using KaNet.Utils;
using NUnit.Framework;
using UnityEngine;
using Utils;

public class Tester_Tessellator : MonoBehaviour
{
    [Test]
    public void Test_Tessellator()
    {
        Debug.Log(new TessellateCoord(20, 15, 1).GetHashCode());
        Debug.Log(new TessellateCoord(15, 20, 1).GetHashCode());
    }
}
