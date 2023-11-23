using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Utils
{
	public static class GuiExtension
	{
		public static void StretchToParent(this RectTransform rect)
		{
			rect.anchoredPosition = Vector2.zero;
			rect.anchorMin = Vector2.zero;
			rect.anchorMax = Vector2.one;
			rect.sizeDelta = Vector2.zero;
		}

		public static void StretchToParent(this RectTransform rect, Transform parent)
		{
			rect.transform.SetParent(parent);
			rect.StretchToParent();
		}
	}
}
