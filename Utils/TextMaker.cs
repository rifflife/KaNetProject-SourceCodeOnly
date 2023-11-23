using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
	public static class TextMaker
	{
		/// <summary>문자열 포멧 구분자입니다.</summary>
		public static readonly string FormatSeparator = "<format>";

		/// <summary>문자열 포멧 구분자를 지정한 객체로 대입한 문자열을 반환합니다.</summary>
		/// <param name="formatText">포멧 구분자를 지정한 문자열 대상 입니다.</param>
		/// <param name="argumentList">객체입니다.</param>
		/// <returns>포멧 구분자를 객체 문자열로 대체한 문자열입니다.</returns>
		public static string GetStringByFormat(string formatText, object argument)
		{
			string insertString = argument == null ? "" : argument.ToString();

			TryReplaceFrist(formatText, FormatSeparator, insertString, out formatText);
			return formatText;
		}

		/// <summary>문자열 포멧 구분자들을 지정한 객체 집합으로 대입한 문자열을 반환합니다.</summary>
		/// <param name="formatText">포멧 구분자들을 지정한 문자열 대상 입니다.</param>
		/// <param name="argumentList">객체 리스트입니다.</param>
		/// <returns>포멧 구분자들을 객체 집합 문자열로 대체한 문자열입니다.</returns>
		public static string GetStringByFormat(string formatText, IList argumentList)
		{
			foreach (object argument in argumentList)
			{
				string insertString = argument == null ? "" : argument.ToString();
				if (!TryReplaceFrist(formatText, FormatSeparator, insertString, out formatText))
				{
					break;
				}
			}

			return formatText;
		}

		/// <summary>문자열에서 첫번째로 존재하는 oldValue를 newValue로 교체합니다.</summary>
		/// <returns>교체할 oldValue 문자열이 없다면 false를 반환합니다.</returns>
		public static bool TryReplaceFrist(string text, string oldValue, string newValue, out string result)
		{
			int startIndex = text.IndexOf(oldValue);

			if (startIndex < 0)
			{
				result = text;
				return false;
			}

			text = text.Remove(startIndex, oldValue.Length);
			text = text.Insert(startIndex, newValue);

			result = text;
			return true;
		}
	}
}
