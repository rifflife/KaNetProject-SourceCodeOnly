using System;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace Utils.ViewModel
{
    [Serializable]
    public class ScrollRectViewModel : ViewModel
    {
		private ScrollRect mScrollRectView;

        public ScrollRectViewModel() : base() { }

		public ScrollRectViewModel(string viewModelName) : base(viewModelName) { }

		public override void Initialize(MonoBehaviour mono)
		{
			if (!ViewModelName.IsValid())
			{
				Ulog.Log(UlogType.UI, $"ScrollRectViewModelName is not set");
				return;
			}

			var findedScrollRects = mono.GetComponentsInChildren<ScrollRect>(true);
			bool isFinded = false;

			foreach (var scrollRect in findedScrollRects)
			{
				if (scrollRect.gameObject.name.Equals(ViewModelName))
				{
					mScrollRectView = scrollRect;
					isFinded = true;
					break;
				}
			}

			if (!isFinded)
			{
				Ulog.LogError(UlogType.UI, $"Mono Initialize failed! There is no ScrollRect component in \"{ViewModelName}\"!");
				return;
			}

			IsAvailable = true;
		}

		//private bool IsAvailableMethod()
		//{
		//	if (!IsAvailable)
		//	{
		//		Ulog.LogError(UlogType.UI, $"Binding failed! It's not available.");
		//		return false;
		//	}
		//	return true;
		//}

		public RectTransform GetContent() => mScrollRectView.content;

		public RectTransform GetViewprot() => mScrollRectView.viewport;

		public void BindOnValueChanged(UnityAction<Vector2> onValueChanged)
        {
			releaseOnValueChanged();
			mScrollRectView.onValueChanged.AddListener(onValueChanged);
		}

		private void releaseOnValueChanged()
        {
			if (!IsAvailable) return;

			mScrollRectView.onValueChanged.RemoveAllListeners();
		}

		public void Release()
        {
			if (!IsAvailable) return;

			releaseOnValueChanged(); 

			IsAvailable = false;
        }

		public override GameObject GetViewGameObject()
		{
			return mScrollRectView.gameObject;
		}

		public void SetEnable(bool isEnable)
		{
			mScrollRectView.enabled = isEnable;
		}

		public Scrollbar GetVerticalScrollbar()
		{
			return mScrollRectView.verticalScrollbar;
		}

		public Scrollbar GetHorizontalScrollbar()
		{
			return mScrollRectView.horizontalScrollbar;
		}
	}
}
