using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Utils.ViewModel
{
	[Serializable]
	public class TextMeshProTextViewModel : ViewModel
	{
		private TextMeshProUGUI mTextView { get; set; } = null;

		public string Text
		{
			set
			{
				if (!IsAvailableMethod())
				{
					return;
				}

				mTextView.text = value;
			}
			get
			{
				if (!IsAvailableMethod())
				{
					return null;
				}

				return mTextView.text;
			}
		}

		public Color Color
		{
			set
			{
				if (!IsAvailableMethod())
				{
					return;
				}

				mTextView.color = value;
			}
			get
			{
				if (!IsAvailableMethod())
				{
					return Color.clear;
				}

				return mTextView.color;
			}
		}

		public float FontSize
		{
			set
			{
				if (!IsAvailableMethod())
				{
					return;
				}

				mTextView.fontSize = value;
			}
			get
			{
				if (!IsAvailableMethod())
				{
					return 0.0f;
				}

				return mTextView.fontSize;
			}
		}

		public TextMeshProTextViewModel() : base() { }

		public TextMeshProTextViewModel(string viewModelName) : base(viewModelName) { }

		public override void Initialize(MonoBehaviour mono)
		{
			if (!ViewModelName.IsValid())
			{
				Ulog.LogError(UlogType.UI, $"ViewModelName is not set");
				return;
			}

			var findedTexts = mono.GetComponentsInChildren<TextMeshProUGUI>(true);
			bool isFinded = false;

			foreach (var text in findedTexts)
			{
				if (text.gameObject.name.Equals(ViewModelName))
				{
					mTextView = text;
					isFinded = true;
					break;
				}
			}

			if (!isFinded)
			{
				Ulog.LogError(UlogType.UI, $"Mono Initialize failed! There is no TextMeshProUGUI component in \"{ViewModelName}\"!");
				return;
			}

			IsAvailable = true;
		}

		public void SetFontAsset(TMP_FontAsset font)
		{
			if (!IsAvailableMethod())
			{
				return;
			}

			mTextView.font = font;
		}

		private bool IsAvailableMethod()
		{
			if (!IsAvailable)
			{
				Ulog.LogError(UlogType.UI, $"Binding failed! It's not available.");
				return false;
			}
			return true;
		}

		public override GameObject GetViewGameObject()
		{
			return mTextView.gameObject;
		}
	}
}
