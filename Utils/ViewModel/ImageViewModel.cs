using System;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Utils.ViewModel
{
	[Serializable]
	public class ImageViewModel : ViewModel
	{
		private Image mImageView;

		public Sprite Sprite
		{
			set => mImageView.sprite = value;
			get => mImageView.sprite;
		}

		public Color Color
		{
			set => mImageView.color = value;
			get => mImageView.color;
		}

		public ImageViewModel() : base() {}

		public ImageViewModel(string viewModelName) : base(viewModelName) {}

		public override void Initialize(MonoBehaviour mono)
		{
			if (!ViewModelName.IsValid())
			{
				Ulog.LogError(UlogType.UI, $"ViewModelName is null");
				return;
			}

			var findedImages = mono.GetComponentsInChildren<Image>(true);
			bool isFinded = false;

			foreach (var image in findedImages)
			{
				if (image.gameObject.name.Equals(ViewModelName))
				{
					mImageView = image;
					isFinded = true;
					break;
				}
			}

			if (!isFinded)
			{
				Ulog.LogError(UlogType.UI, $"Mono Initialize failed! There is no Image component in \"{ViewModelName}\"!");
				return;
			}

			IsAvailable = true;
		}

		public override GameObject GetViewGameObject()
		{
			return mImageView.gameObject;
		}
	}
}
