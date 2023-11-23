using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Utils.ViewModel
{
	public struct TMP_InputFieldEvent
	{
		public UnityAction<string> OnValueChangedAction;
		public UnityAction<string> OnEndEditAction;
		public UnityAction<string> OnSelect;
		public UnityAction<string> OnDeselect;
	}

	[Serializable]
	public class TextMeshProInputFieldViewModel : ViewModel
	{

		private TMP_InputField mInputFieldView;

		public TextMeshProInputFieldViewModel() : base() {}

		public TextMeshProInputFieldViewModel(string viewModelName) : base(viewModelName) {}

		public string InputText => mInputFieldView.text ?? "NONE";

		public override void Initialize(MonoBehaviour mono)
		{
			if (!ViewModelName.IsValid())
			{
				Ulog.LogError(UlogType.UI, $"Worng ViewModelName");
				return;
			}

			var findedInputFields = mono.GetComponentsInChildren<TMP_InputField>(true);
			bool isFinded = false;

			foreach (var inputField in findedInputFields)
			{
				if (inputField.gameObject.name.Equals(ViewModelName))
				{
					mInputFieldView = inputField;
					isFinded = true;
					break;
				}
			}

			if (!isFinded)
			{
				Ulog.LogError(UlogType.UI, $"Mono Initialize failed! There is no TMP_InputField component in \"{ViewModelName}\"!");
				return;
			}

			IsAvailable = true;
		}

		public void SetText(string context)
		{
			if (IsAvailable)
			{
				mInputFieldView.text = context;
			}
		}

		public void BindAction(in TMP_InputFieldEvent eventGroup)
		{
			if (!IsAvailable)
			{
				Ulog.LogError(UlogType.UI, $"Binding failed! It's not available.");
				return;
			}

			ReleaseBind();

			if (eventGroup.OnValueChangedAction != null)
				mInputFieldView.onValueChanged.AddListener(eventGroup.OnValueChangedAction);

			if (eventGroup.OnEndEditAction != null)
				mInputFieldView.onEndEdit.AddListener(eventGroup.OnEndEditAction);

			if (eventGroup.OnSelect != null)
				mInputFieldView.onSelect.AddListener(eventGroup.OnSelect);

			if (eventGroup.OnDeselect != null)
				mInputFieldView.onDeselect.AddListener(eventGroup.OnDeselect);
		}

		private void ReleaseBind()
		{
			mInputFieldView.onValueChanged.RemoveAllListeners();
			mInputFieldView.onEndEdit.RemoveAllListeners();
			mInputFieldView.onSelect.RemoveAllListeners();
			mInputFieldView.onDeselect.RemoveAllListeners();
		}

		public void Release()
		{
			if (!IsAvailable)
			{
				return;
			}

			ReleaseBind();
			mInputFieldView = null;

			IsAvailable = false;
		}

		public override GameObject GetViewGameObject()
		{
			return mInputFieldView.gameObject;
		}

		public void Select()
		{
			mInputFieldView.Select();
		}

		public bool IsSelect()
		{
			var selectObject = EventSystem.current.currentSelectedGameObject;
			return selectObject == mInputFieldView.gameObject;
		}
	}
}
