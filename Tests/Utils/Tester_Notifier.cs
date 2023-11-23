using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Utiles;
using Utils;
using Utils.NavMesh;

public class Tester_Notifier : MonoBehaviour
{
	public readonly struct TEST_NotiStruct
	{
		public readonly int Value1;
		public readonly int Value2;

		public TEST_NotiStruct(int value1, int value2)
		{
			Value1 = value1;
			Value2 = value2;
		}
	}

	[Test]
	public void Test_Notifier()
	{
		SubjectData<int> intNoti = new SubjectData<int>(100);

		intNoti.Value = 120;
		Assert.IsTrue(intNoti.IsDirty);

		intNoti.SetPristine();
		Assert.IsFalse(intNoti.IsDirty);

		intNoti.Value = 120;
		Assert.IsFalse(intNoti.IsDirty);

		intNoti.Value = 130;
		Assert.IsTrue(intNoti.IsDirty);

		SubjectData<TEST_NotiStruct> structNoti = new SubjectData<TEST_NotiStruct>(new TEST_NotiStruct(10, 20));
		
		Assert.IsFalse(structNoti.IsDirty);

		structNoti.Value = new TEST_NotiStruct(50, 20);
		Assert.IsTrue(structNoti.IsDirty);

		structNoti.SetPristine();

		structNoti.Value = new TEST_NotiStruct(50, 20);
		Assert.IsFalse(structNoti.IsDirty);
	}
}
