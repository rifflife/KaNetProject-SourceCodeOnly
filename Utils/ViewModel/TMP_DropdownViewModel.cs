using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace Utils.ViewModel
{
	[Serializable]
	public class TMP_DropdownViewModel : ViewModel
	{
		private TMP_Dropdown mDropdownView;

		public TMP_DropdownViewModel() : base() { }

		public TMP_DropdownViewModel(string viewModelName) : base(viewModelName) { }

		public TMP_DropdownViewModel(MonoBehaviour mono)
		{
			Initialize(mono);
		}

		public TMP_DropdownViewModel(MonoBehaviour mono, string viewModelName)
		{
			ViewModelName = viewModelName;
			Initialize(mono);
		}

		public int Value
		{
			set
			{
				mDropdownView.value = value;
			}
			get
			{
				return mDropdownView.value;
			}
		}


		public override void Initialize(MonoBehaviour mono)
		{
			if (!ViewModelName.IsValid())
			{
				Ulog.Log(UlogType.UI, $"Worng ViewModelName");
				return;
			}

			var findedDropdown = mono.GetComponentsInChildren<TMP_Dropdown>(true);
			bool isFinded = false;

			foreach (var dropdown in findedDropdown)
			{
				if (dropdown.gameObject.name == ViewModelName)
				{
					mDropdownView = dropdown;
					isFinded = true;
					break;
				}
			}

			if (!isFinded)
			{
				Ulog.Log(UlogType.UI, $"Mono Initialize failed! There is no \"{mDropdownView.GetType().Name}\" component in \"{ViewModelName}\"!");
				return;
			}

			IsAvailable = true;

			mDropdownView.ClearOptions();

			return;
		}

		public void BindAction(UnityAction<int> onCalueChangeAction)
		{
			mDropdownView.onValueChanged.RemoveAllListeners();
			mDropdownView.onValueChanged.AddListener(onCalueChangeAction);
		}

		public void AddOption(TMP_Dropdown.OptionData item)
		{
			mDropdownView.options.Add(item);
			mDropdownView.RefreshShownValue();
		}

		public void AddOptions(List<TMP_Dropdown.OptionData> optionDataList)
		{
			mDropdownView.AddOptions(optionDataList);
		}

		public void ClearOptions()
		{
			mDropdownView.ClearOptions();
		}

		public override GameObject GetViewGameObject()
		{
			return mDropdownView.gameObject;
		}
	}
}

