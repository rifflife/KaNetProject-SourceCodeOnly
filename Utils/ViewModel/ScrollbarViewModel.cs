using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utils.ViewModel;

namespace Utils.ViewModel
{
	[Serializable]
	public class ScrollbarViewModel : ViewModel
	{
		private Scrollbar mModel;
		[SerializeField]
		private ImageViewModel Img_ScrollbarBackGround = new(nameof(Img_ScrollbarBackGround));

		public float Value
		{
			get
			{
				return mModel.value;
			}
		}

		public ScrollbarViewModel(string viewModelName)
		{
			ViewModelName = viewModelName;
		}

		public override GameObject GetViewGameObject()
		{
			throw new NotImplementedException();
		}

		public override void Initialize(MonoBehaviour mono)
		{
			if (!ViewModelName.IsValid())
			{
				Ulog.LogError(UlogType.UI, $"ViewModelName is null");
				return;
			}

			var findedScrollbar = mono.GetComponentsInChildren<Scrollbar>(true);
			bool isFinded = false;

			foreach (var scrollbar in findedScrollbar)
			{
				if (scrollbar.gameObject.name.Equals(ViewModelName))
				{
					mModel = scrollbar;
					isFinded = true;
					break;
				}
			}

			if (!isFinded)
			{
				Ulog.LogError(UlogType.UI, $"Mono Initialize failed! There is no {mModel.GetType().Name} component in \"{ViewModelName}\"!");
				return;
			}

			Img_ScrollbarBackGround.Initialize(mono);

			if (!Img_ScrollbarBackGround.IsAvailable)
				return;

			IsAvailable = true;
		}

		public void BindOnValueChanged(UnityAction<float> onValueChange)
		{
			mModel.onValueChanged.RemoveAllListeners();
			mModel.onValueChanged.AddListener(onValueChange);
		}

		public void SetActive(bool isActive)
		{
			mModel.handleRect.gameObject.SetActive(isActive);
			Img_ScrollbarBackGround.GetViewGameObject().SetActive(isActive);
		}
	}
}
