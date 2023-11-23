using UnityEngine;
using System.Reflection;
using NUnit.Framework;
using KaNet.Synchronizers;
using KaNet.Core;
using KaNet.Utils;
using System;
using Utiles;

public class Tester_NetObjectType : MonoBehaviour
{
	[Test]
	public void Test_NetObjectType()
	{
		Assert.AreEqual(BaseNetObjectType.System, NetObjectType.System_Test.GetBaseType());
		Assert.AreEqual(BaseNetObjectType.Data, NetObjectType.Data_Test.GetBaseType());
	}
}
