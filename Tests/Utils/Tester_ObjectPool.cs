using System.Collections.Generic;
using KaNet.Utils;
using Utils.Analytics;
using NUnit.Framework;
using UnityEngine;
using Utils;

public class Tester_ObjectPool : MonoBehaviour
{
	[Test]
	public void Test_ObjectPool()
	{
		ObjectPool<NetBuffer> netBufferPool = new ObjectPool<NetBuffer>(3);

		List<NetBuffer> tempBuffer = new List<NetBuffer>();

		for (int i = 0; i < 100; i++)
		{
			tempBuffer.Add(netBufferPool.Get());
		}

		foreach (var n in tempBuffer)
		{
			netBufferPool.Return(n);
		}
	}

	[Test]
	public void Test_PoolPerformance()
	{
		/// 초기화 작업이 무거운 클래스일 수록 풀 매니져가 훨씬 더 효율적이고 빠름
		/// 생성자 대신 다른 함수를 통해 초기화 할 필요가 있음

		ObjectPool<NetBuffer> bufferPool = new ObjectPool<NetBuffer>(0);

		FunctionMeasurer poolMasurere = new FunctionMeasurer("Pool Masure", 1);

		poolMasurere.Add(new FunctionInfo("Pool With Net Capacity", () => { 
			var buffer = bufferPool.Get();
			buffer.Reserve(1000);
			bufferPool.Return(buffer);
		}));

		poolMasurere.Add(new FunctionInfo("Non Pool With Net Capacity", () => {
			var buffer = new NetBuffer(1000);
		}));

		poolMasurere.Add(new FunctionInfo("Pool", () => {
			var buffer = bufferPool.Get();
			bufferPool.Return(buffer);
		}));

		poolMasurere.Add(new FunctionInfo("Non Pool", () => {
			var buffer = new NetBuffer();
		}));

		poolMasurere.Measure();
	}
}
