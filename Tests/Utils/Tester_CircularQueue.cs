using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Utils;

public class Tester_CircularQueue: MonoBehaviour
{
	[Test]
	public void Test_CircularQueue()
	{
		CircularQueue<int> circularQueue = new CircularQueue<int>(100);

		int repeatCount = 100;

		Assert.IsFalse(circularQueue.TryDequeue(out var abc));

		for (int i = 0; i < repeatCount; i++)
		{
			circularQueue.TryEnqueue(i);
		}

		Assert.IsFalse(circularQueue.TryEnqueue(10));

		for (int i = 0; i < repeatCount; i++)
		{
			Assert.IsTrue(circularQueue.TryDequeue(out var dequeueValue));
			Assert.AreEqual(i, dequeueValue);
		}
	}
}
