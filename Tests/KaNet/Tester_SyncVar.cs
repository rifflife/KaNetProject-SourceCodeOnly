using UnityEngine;
using System.Reflection;
using NUnit.Framework;
using KaNet.Synchronizers;
using KaNet.Core;
using KaNet.Utils;
using System;
using Utiles;
using System.Collections.Generic;

public class Tester_SyncVar : MonoBehaviour
{
	[Test]
	public void Test_SyncVar()
	{
		SyncField<NetInt32> syncVar = new SyncField<NetInt32>();

		Assert.IsFalse(syncVar.IsDirty);

		syncVar.Data = 30;

		Assert.IsTrue(syncVar.IsDirty);
	}

	[Test]
	public void Test_StructSyncVarReferenceTest()
	{
		NetInt32 sync_1 = new(1);
		NetInt32 sync_2 = new(2);
		NetInt32 sync_3 = new(3);

		List<INetworkSerializable> list = new();
		list.Add(sync_1);
		list.Add(sync_2);
		list.Add(sync_3);

		sync_1.Value = 100;

		var sync_1_ref = (NetInt32)list[0];

		Assert.AreEqual(100, sync_1_ref.Value);



	}
}
