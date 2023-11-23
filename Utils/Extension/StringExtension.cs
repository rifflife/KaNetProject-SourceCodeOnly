using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
	public static class StringExtension
	{
		/// <summary>
		/// 유효한 문자열인지 여부를 반환합니다.
		/// null이거나, 비어있거나, 공백으로만 이루어져 있다면 유효하지 않은 문자열입니다.
		/// </summary>
		/// <param name="data">검사할 문자열입니다.</param>
		/// <returns>유효한 문자열인지 여부입니다.</returns>
		public static bool IsValid(this string data)
		{
			if (data == null || data == "")
			{
				return false;
			}

			int length = data.Length;

			for (int i = 0; i < length; i++)
			{
				if (data[i] != ' ')
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>해당 문자열이 알파벳과 숫자로만 이루어져있는지 판단합니다.</summary>
		/// <param name="data">검사할 문자열</param>
		/// <returns>해당 문자열이 알파벳과 숫자로만 이루어져있다면 true를 반환합니다.</returns>
		public static bool IsOnlyAlphabetAndNumber(this string data)
		{
			foreach (char c in data)
			{
				if (!((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (c >= '0' && c <= '9')))
				{
					return false;
				}
			}

			return true;
		}
	}
}
