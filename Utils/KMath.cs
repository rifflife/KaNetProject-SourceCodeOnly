using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
	public static class KMath
	{
		public static int Clamp(int value, int maxValue, int minValue)
		{
			if (value > maxValue)
			{
				return maxValue;
			}

			if (value < minValue)
			{
				return minValue;
			}

			return value;
		}

		/// <summary>값을 스냅합니다.</summary>
		/// <param name="value">스냅할 값입니다.</param>
		/// <param name="snap">스냅 크기입니다.</param>
		/// <returns>스냅된 값입니다.</returns>
		public static float SnapBy(float value, float snap)
		{
			return (int)(value / snap) * snap;
		}

		/// <summary>값을 스냅합니다.</summary>
		/// <param name="value">스냅할 값입니다.</param>
		/// <param name="snap">스냅 크기입니다.</param>
		/// <returns>스냅된 값입니다.</returns>
		public static UnityEngine.Vector2 SnapBy(UnityEngine.Vector2 value, float snap)
		{
			float x = SnapBy(value.x, snap);
			float y = SnapBy(value.y, snap);
			return new UnityEngine.Vector2(x, y);
		}
	}
}
