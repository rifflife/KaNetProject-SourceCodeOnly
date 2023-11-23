using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
	public static partial class Localization
	{
		public static string GetText(string textType)
		{
			if (!Enum.TryParse(typeof(TextType), textType, out var curTextType))
			{
				return textType;
			}

			var findTextType = (TextType)curTextType;

			if (mTextTable.TryGetValue(findTextType, out var result))
			{
				return result;
			}

			return "ERROR_TEXT";
		}
	}
}
