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
	public class ToggleViewModel : ViewModel
	{
		private Toggle mToggleView;

		public ToggleViewModel() : base() { }

		public ToggleViewModel(string viewModelName) : base(viewModelName) { }

		public ToggleViewModel(MonoBehaviour mono)
		{
			Initialize(mono);
		}

		public ToggleViewModel(MonoBehaviour mono, string viewModelName)
		{
			ViewModelName = viewModelName;
			Initialize(mono);
		}

		public bool IsOn
		{
			set
			{
				mToggleView.isOn = value;
			}
			get
			{
				return mToggleView.isOn;
			}
		}

		public override void Initialize(MonoBehaviour mono)
		{
			if (!ViewModelName.IsValid())
			{
				Ulog.Log(UlogType.UI, $"Worng ViewModelName");
				return;
			}

			var findedToggles = mono.GetComponentsInChildren<Toggle>(true);
			bool isFinded = false;

			foreach (var toggle in findedToggles)
			{
				if (toggle.gameObject.name == ViewModelName)
				{
					mToggleView = toggle;
					isFinded = true;
					break;
				}
			}

			if (!isFinded)
			{
				Ulog.Log(UlogType.UI, $"Mono Initialize failed! There is no \"{mToggleView.GetType().Name}\" component in \"{ViewModelName}\"!");
				return;
			}

			IsAvailable = true;

			return;
		}

		/// <summary>연결된 액션을 초기화하고 새로운 액션을 연결합니다.</summary>
		public void BindAction(UnityAction<bool> onValueChange)
		{
			mToggleView.onValueChanged.RemoveAllListeners();
			mToggleView.onValueChanged.AddListener(onValueChange);
		}
		/// <summary>기존에 있는 액션 유지하면서 추가 액션을 연결합니다.</summary>
		public void AddAction(UnityAction<bool> onValueChange)
		{
			mToggleView.onValueChanged.AddListener(onValueChange);
		}

		public override GameObject GetViewGameObject()
		{
			return mToggleView.gameObject;
		}

	}
}

