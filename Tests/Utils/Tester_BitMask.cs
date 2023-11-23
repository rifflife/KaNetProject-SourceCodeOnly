using System;

using UnityEngine;
using NUnit.Framework;
using Utils;
using Utils.Analytics;

public class Tester_Bitmask : MonoBehaviour
{
	[Test]
	public void Test_BitOperation()
	{
		int i = 7;

		Console.WriteLine(i % 8);
		Console.WriteLine(i & 0b0111);

		FunctionInfo bitDivision = new FunctionInfo("Bit", bitDivisionFunc);
		FunctionInfo normalDivision = new FunctionInfo("Nor", normalDivisionFunc);

		FunctionMeasurer measuerer = new FunctionMeasurer("Division Operation Compare", bitDivision, normalDivision, 20);

		measuerer.Measure();

		void bitDivisionFunc()
		{
			int i = 123155;
			i = i & 0b0111;
		}

		void normalDivisionFunc()
		{
			int i = 123155;
			i = i % 8;
		}
	}

	[Test]
	public void Test_Bitmask()
	{
		const int maxIndex = 2;

		BitmaskVector curMask = BitmaskVector.Create(64);

		Assert.IsTrue(curMask.IsAllFalse());
		Assert.IsFalse(curMask[1]);
		Assert.IsFalse(curMask[2]);

		curMask[10] = true;
		Assert.IsTrue(curMask[10]);
		Assert.IsTrue(!curMask.IsAllFalse());
		curMask[10] = false;
		Assert.IsFalse(curMask[10]);
		Assert.IsTrue(curMask.IsAllFalse());

		Assert.IsTrue(curMask.IsValidIndex(maxIndex * 32 - 1));
		Assert.IsFalse(curMask.IsValidIndex(maxIndex * 32));
		Assert.IsFalse(curMask.IsValidIndex(-1));

		curMask[50] = true;
		Assert.IsTrue(curMask[50]);
		Assert.IsFalse(curMask.IsAllFalse());

		curMask[61] = true;
		Assert.IsTrue(curMask[61]);
		Assert.IsFalse(curMask.IsAllFalse());

		// Test entire operation
		curMask.Clear(true);

		Assert.IsTrue(curMask.IsAllTrue());
		Assert.IsFalse(curMask.IsAllFalse());

		for (int i = 0; i < curMask.Size; i++)
		{
			Assert.IsTrue(curMask[i]);
		}

		curMask.Clear(false);

		Assert.IsTrue(curMask.IsAllFalse());
		Assert.IsFalse(curMask.IsAllTrue());

		for (int i = 0; i < curMask.Size; i++)
		{
			Assert.IsFalse(curMask[i]);
		}
	}

	[Test]
	public void Test_BitmaskTryFunctions()
	{
		BitmaskVector bitMask = BitmaskVector.Create(32, true);

		Assert.IsTrue(bitMask.IsAllTrue());

		Assert.IsFalse(bitMask.TrySet(1000, false));
		Assert.IsFalse(bitMask.TrySetFalse(32));
		Assert.IsTrue(bitMask.TrySetFalse(31));
		Assert.IsFalse(bitMask[31]);

		Assert.IsFalse(bitMask.TrySetTrue(-1));
		bitMask[0] = false;
		Assert.IsFalse(bitMask[0]);
		Assert.IsTrue(bitMask.TrySetTrue(0));
		Assert.IsTrue(bitMask[0]);

		Assert.IsTrue(bitMask.TrySetFalse(2));
		Assert.IsTrue(bitMask.TryGetValue(2, out var value));
		Assert.IsFalse(value);
		Assert.IsFalse(bitMask.TryGetValue(32, out var value2));

		bitMask.Clear(false);
		Assert.IsTrue(bitMask.IsAllFalse());
		bitMask.Clear(true);
		Assert.IsTrue(bitMask.IsAllTrue());
	}

	[Test]
	public void Test_2DBitmask()
	{
		BitmaskVector mask = BitmaskVector.Create(15, 15, true);

		Assert.AreEqual(mask.SizeX, 32);
		Assert.AreEqual(mask.SizeY, 15);

		Assert.IsTrue(mask.IsAllTrue());
		mask.Clear(false);
		Assert.IsTrue(mask.IsAllFalse());
		mask[10, 10] = true;
		Assert.IsTrue(mask[10, 10]);

		for (int y = 0; y < 15; y++)
		{
			for (int x = 0; x < 32; x++)
			{
				mask[y, x] = true;
			}
		}

		Assert.IsTrue(mask.IsAllTrue());

		Assert.IsFalse(mask.IsValidIndex(32, 14));
		Assert.IsTrue(mask.IsValidIndex(31, 14));
		Assert.IsFalse(mask.IsValidIndex(31, 15));

		for (int y = 0; y < 15; y++)
		{
			for (int x = 0; x < 32; x++)
			{
				mask.SetFalse(x, y);
			}
		}

		Assert.IsTrue(mask.IsAllFalse());

		mask[10, 11] = true;
		mask[5, 2] = true;

		BitmaskVector copied = mask.Clone();

		Assert.IsTrue(copied[10, 11]);
		Assert.IsTrue(copied[5, 2]);
	}

	[Test]
	public void Test_3DBitmask()
	{
		BitmaskVector mask = BitmaskVector.Create(45, 5, 7);

		Assert.AreEqual(mask.SizeX, 64);
		Assert.AreEqual(mask.SizeY, 5);
		Assert.AreEqual(mask.SizeZ, 7);

		mask[6, 3, 30] = true;

		Assert.IsTrue(mask[6, 3, 30]);

		for (int z = 0; z < 7; z++)
		{
			for (int y = 0; y < 5; y++)
			{
				for (int x = 0; x < 64; x++)
				{
					mask.SetTrue(x, y, z);
				}
			}
		}

		Assert.IsTrue(mask.IsAllTrue());
	}

	[Test]
	public void Test_SingleBitmask()
	{
		Bitmask mask = new Bitmask(true);

		Assert.IsTrue(mask.IsAllTrue());
		Assert.IsFalse(mask.IsAllFalse());

		for (int i = 0; i < 8; i++)
		{
			mask[i] = false;
		}

		Assert.IsFalse(mask.IsAllTrue());
		Assert.IsTrue(mask.IsAllFalse());

		mask.SetTrue(3);
		Assert.IsTrue(mask[3]);
		Assert.IsTrue(mask.TryGetValue(3, out var result_1));
		Assert.IsTrue(result_1);

		mask[5] = true;
		Assert.IsTrue(mask[5]);
		Assert.IsTrue(mask.TryGetValue(5, out var result_2));
		Assert.IsTrue(result_2);

		Assert.IsFalse(mask.IsAllFalse());
		Assert.IsFalse(mask.IsAllTrue());

		mask[5] = false;
		mask[3] = false;
		Assert.IsTrue(mask.IsAllFalse());

		Assert.IsFalse(mask.TryGetValue(10, out var aehil));
		Assert.IsFalse(mask.TrySet(-1, true));
		Assert.IsFalse(mask.TrySetFalse(8));
		Assert.IsFalse(mask.TrySetTrue(9));
	}
}
