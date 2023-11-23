using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using System.Reflection;
using NUnit.Framework;
using KaNet.Synchronizers;
using KaNet.Core;
using KaNet.Utils;

public class Tester_RpcCaller : MonoBehaviour
{
	[Test]
	public void Test_RpcCaller()
	{
		//NetBuffer rpcBuffer = new NetBuffer();

		//RpcCaller<NetInt32> rpcCaller = new RpcCaller<NetInt32>(TestFunction);
		////rpcCaller.BindStream(rpcBuffer);
		//rpcCaller.Invoke(new NetInt32(40));

		//var rpcReader = rpcBuffer.GetReader();
		//rpcReader.ReadInt8();

		//rpcCaller.Deserialize(rpcReader);
	}

	private void TestAction()
	{

	}

	private void TestFunction(in NetInt32 value)
	{
		Debug.Log($"{MethodBase.GetCurrentMethod().Name}(in NetInt32 {value})");
	}
}
