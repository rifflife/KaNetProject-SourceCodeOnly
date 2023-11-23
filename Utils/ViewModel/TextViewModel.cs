using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Utils.ViewModel
{
	[Serializable]
	public class TextViewModel : ViewModel
	{
		[field: SerializeField] public string TextType { get; private set; } = "";

		private Text mText;

		public TextViewModel() : base() { }

		public TextViewModel(string viewModelName) : base(viewModelName) { }

		public override void Initialize(MonoBehaviour mono)
		{
			if (!ViewModelName.IsValid())
			{
				Ulog.Log(UlogType.UI, $"Worng ViewModelName");
				return;
			}

			var findedTexts = mono.GetComponentsInChildren<Text>(true);
			bool isFinded = false;

			foreach (var button in findedTexts)
			{
				if (button.gameObject.name == ViewModelName)
				{
					mText = button;
					isFinded = true;
					break;
				}
			}

			if (!isFinded)
			{
				Ulog.Log(UlogType.UI, $"Mono Initialize failed! There is no \"{mText.GetType().Name}\" component in \"{ViewModelName}\"!");
				return;
			}

			IsAvailable = true;

			Localization.OnLanguageChanged += onLanguageChanged;
			onLanguageChanged();
		}

		private void Release()
		{
			Localization.OnLanguageChanged -= onLanguageChanged;
		}

		private void onLanguageChanged()
		{
			if (!IsAvailable)
			{
				return;
			}

			mText.text = Localization.GetText(TextType);
		}

		public override GameObject GetViewGameObject()
		{
			return mText.gameObject;
		}
	}
}
