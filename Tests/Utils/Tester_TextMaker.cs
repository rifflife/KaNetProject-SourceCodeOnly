using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using UnityEngine;
using Utils;
using Utils.Analytics;

public class Tester_TextMaker: MonoBehaviour
{
	public class Dog
	{
		private string mName;
		private int mAge;

		public Dog(string name, int age)
		{
			mName = name;
			mAge = age;
		}

		public override string ToString()
		{
			return $"[{mName}{mAge}]";
		}
	}
	
	[Test]
	public void Test_TextMaker()
	{
		// TryReplaceFrist
		{
			string separator = TextMaker.FormatSeparator;
			string replaceText = "최지욱";

			string formatText = $"{separator}A{separator}B{separator}CD{separator}E";
			string result;

			Assert.IsTrue(TextMaker.TryReplaceFrist(formatText, separator, replaceText, out result));
			Assert.AreEqual($"{replaceText}A{separator}B{separator}CD{separator}E", result);

			Assert.IsTrue(TextMaker.TryReplaceFrist(result, separator, replaceText, out result));
			Assert.AreEqual($"{replaceText}A{replaceText}B{separator}CD{separator}E", result);

			Assert.IsTrue(TextMaker.TryReplaceFrist(result, separator, replaceText, out result));
			Assert.AreEqual($"{replaceText}A{replaceText}B{replaceText}CD{separator}E", result);

			Assert.IsTrue(TextMaker.TryReplaceFrist(result, separator, replaceText, out result));
			Assert.AreEqual($"{replaceText}A{replaceText}B{replaceText}CD{replaceText}E", result);

			Assert.IsFalse(TextMaker.TryReplaceFrist(result, separator, replaceText, out result));
			Assert.AreEqual($"{replaceText}A{replaceText}B{replaceText}CD{replaceText}E", result);
		}

		{
			string testString = "A<format>B<format>C<format>";

			List<Dog> dogList = new List<Dog>();
			dogList.Add(new Dog("강아지", 100));
			dogList.Add(new Dog("송아지", 99));

			string printText;

			printText = TextMaker.GetStringByFormat(testString, dogList);
			Assert.AreEqual("A[강아지100]B[송아지99]C<format>", printText);

			dogList.Add(new Dog("멍멍이", 17));
			printText = TextMaker.GetStringByFormat(testString, dogList);
			Assert.AreEqual("A[강아지100]B[송아지99]C[멍멍이17]", printText);

			dogList.Add(new Dog("멍멍이2", 157));
			dogList.Add(new Dog("멍멍이3", 158));

			printText = TextMaker.GetStringByFormat(testString, dogList);
			Assert.AreEqual("A[강아지100]B[송아지99]C[멍멍이17]", printText);

			Assert.AreEqual("A와우B<format>C<format>", TextMaker.GetStringByFormat(testString, "와우"));
		}
	}

	[Test]
	public void Test_StringByteCounter()
	{
		string testString = "2tf";

		int stringByteLength = Encoding.UTF8.GetByteCount(testString);
		var rawData = Encoding.UTF8.GetBytes(testString);

		Assert.AreEqual(stringByteLength, rawData.Length);

		// Byte 변환이 GetCount에 비해 4배 이상 느리다.
		FunctionMeasurer cunter = new FunctionMeasurer("String Byte Counter", 1);

		cunter.Add(new FunctionInfo("ByteLength", () => {
			var rawData = Encoding.UTF8.GetBytes(testString).Length;
		}));

		cunter.Add(new FunctionInfo("Get Count", () => {
			int stringByteLength = Encoding.UTF8.GetByteCount(testString);
		}));

		cunter.Measure();
	}
}
