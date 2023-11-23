using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utils;

namespace Utils.ViewModel
{
	[Serializable]
	public class SliderViewModel : ViewModel
	{
		private Slider mSliderView;

		public SliderViewModel() : base() {}

		public SliderViewModel(string viewModelName) : base(viewModelName) {}

		public float Value
		{
			set
			{
				mSliderView.value = value;
			}
			get
			{
				return mSliderView.value;
			}
		}

		public override void Initialize(MonoBehaviour mono)
		{
			if (!ViewModelName.IsValid())
			{
				Ulog.LogError(UlogType.UI, $"ViewModelName is null");
				return;
			}

			var findedSlider = mono.GetComponentsInChildren<Slider>(true);
			bool isFinded = false;

			foreach (var slider in findedSlider)
			{
				if (slider.gameObject.name.Equals(ViewModelName))
				{
					mSliderView = slider;
					isFinded = true;
					break;
				}
			}

			if (!isFinded)
			{
				Ulog.LogError(UlogType.UI, $"Mono Initialize failed! There is no Slider component in \"{ViewModelName}\"!");
				return;
			}

			IsAvailable = true;
		}

		public override GameObject GetViewGameObject()
		{
			return mSliderView.gameObject;
		}

		public void BindAction(UnityAction<float> onValueChange)
		{
			mSliderView.onValueChanged.RemoveAllListeners();
			mSliderView.onValueChanged.AddListener(onValueChange);
		}
	}
}
