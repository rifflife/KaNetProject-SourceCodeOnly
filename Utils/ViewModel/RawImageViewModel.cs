using System;
using UnityEngine;
using UnityEngine.UI;
using Utils;


namespace Utils.ViewModel
{
	[Serializable]
	public class RawImageViewModel : ViewModel
	{
		private RawImage mRawImageView;

		public Texture Texture
		{
			get => mRawImageView.texture;
			set => mRawImageView.texture = value;
		}

		public Color Color
		{
			set => mRawImageView.color = value;
			get => mRawImageView.color;
		}

		public RawImageViewModel() {}

		public RawImageViewModel(string viewModelName)
		{
			ViewModelName = viewModelName;
		}

		public override void Initialize(MonoBehaviour mono)
		{
			if (!ViewModelName.IsValid())
			{
				Ulog.LogError(UlogType.UI, $"ViewModelName is null");
				return;
			}

			var findedImages = mono.GetComponentsInChildren<RawImage>(true);
			bool isFinded = false;

			foreach (var rawImage in findedImages)
			{
				if (rawImage.gameObject.name.Equals(ViewModelName))
				{
					mRawImageView = rawImage;
					isFinded = true;
					break;
				}
			}

			if (!isFinded)
			{
				Ulog.LogError(UlogType.UI, $"Mono Initialize failed! There is no {mRawImageView.GetType().Name} component in \"{ViewModelName}\"!");
				return;
			}

			IsAvailable = true;
		}

		public override GameObject GetViewGameObject()
		{
			return mRawImageView.gameObject;
		}
	}
}
