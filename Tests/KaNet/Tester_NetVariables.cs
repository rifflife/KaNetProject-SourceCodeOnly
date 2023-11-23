using UnityEngine;
using System.Reflection;
using NUnit.Framework;
using KaNet.Synchronizers;
using KaNet.Core;
using KaNet.Utils;
using System;
using Utiles;

public class Tester_NetVariables : MonoBehaviour
{
	public enum MyEnum : byte
	{
		None,
		Something,
	}

	public void DoSomething()
	{
		NetInt8<MyEnum> wowEnum = new(MyEnum.Something);
		NetInt8<MyEnum> myEnum = MyEnum.Something;
		MyEnum yourEnum = myEnum;
	}

	[Test]
	public void Test_NetVariables()
	{
		NetUInt8<PacketHeaderType> headerType = PacketHeaderType.SYN_OBJ_FIELD;
		PacketHeaderType test = headerType;

		Assert.AreEqual(test, PacketHeaderType.SYN_OBJ_FIELD);
		Assert.AreEqual(test.ToString(), headerType.ToString());
	}
}
