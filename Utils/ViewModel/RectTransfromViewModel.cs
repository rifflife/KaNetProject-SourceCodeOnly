using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Utils.ViewModel
{
	[Serializable]
	public class RectTransfromViewModel : ViewModel
	{
		private RectTransform mRect;

		public RectTransfromViewModel() : base() {}

		public RectTransfromViewModel(string viewModelName) : base(viewModelName) {}

		public override void Initialize(MonoBehaviour mono)
		{
			var findTransform = mono.GetComponentsInChildren<RectTransform>(true);

			bool isFinded = false;

			foreach (var t in findTransform)
			{
				if (t.gameObject.name == ViewModelName)
				{
					mRect = t;
					isFinded = true;
					break;
				}
			}

			if (!isFinded)
			{
				Ulog.Log(UlogType.UI, $"Mono Initialize failed! There is no \"{mRect.GetType().Name}\" component in \"{ViewModelName}\"!");
				return;
			}

			IsAvailable = true;

			return;
		}

		public override GameObject GetViewGameObject()
		{
			return mRect.gameObject;
		}

		public void SetOffsetMin(Vector2 min)
		{
			mRect.offsetMin = min;
		}

		public void SetOffsetMax(Vector2 max)
		{
			mRect.offsetMax = max;
		}

		public void SetRight(float right)
		{
			mRect.offsetMax = new Vector2(right, mRect.offsetMax.y);
		}

		public void SetLeft(float left)
		{
			mRect.offsetMin = new Vector2(left, mRect.offsetMin.y);
		}

		public void SetTop(float top)
		{
			mRect.offsetMax = new Vector2(mRect.offsetMax.x, top);
		}

		public void SetBottom(float bottom)
		{
			mRect.offsetMin = new Vector2(mRect.offsetMin.x, bottom);
		}

		public void SetMinAnchors(Vector2 min)
		{
			mRect.anchorMin = min;
		}

		public void SetMaxAnchors(Vector2 max)
		{
			mRect.anchorMax = max;
		}

		public void SetMinAnchorX(float x)
		{
			SetMinAnchors(new Vector2(x, mRect.anchorMin.y));
		}

		public void SetMaxAnchorX(float x)
		{
			SetMaxAnchors(new Vector2(x, mRect.anchorMax.y));
		}
	}
}
