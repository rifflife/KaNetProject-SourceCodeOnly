using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Utils.ViewModel
{
	[Serializable]
	public class ButtonViewModel : ViewModel
	{
		private Button mButtonView;
		private UnityAction mOnClickAction;

		public ButtonViewModel() : base() { }

		public ButtonViewModel(string viewModelName) : base(viewModelName) { }

		public ButtonViewModel(MonoBehaviour mono)
		{
			Initialize(mono);
		}

		public ButtonViewModel(MonoBehaviour mono, string viewModelName)
		{
			ViewModelName = viewModelName;
			Initialize(mono);
		}

		public override void Initialize(MonoBehaviour mono)
		{
			if (!ViewModelName.IsValid())
			{
				Ulog.Log(UlogType.UI, $"Worng ViewModelName");
				return;
			}

			var findedButtons = mono.GetComponentsInChildren<Button>(true);
			bool isFinded = false;

			foreach (var button in findedButtons)
			{
				if (button.gameObject.name == ViewModelName)
				{
					mButtonView = button;
					isFinded = true;
					break;
				}
			}

			if (!isFinded)
			{
				Ulog.Log(UlogType.UI, $"Mono Initialize failed! There is no \"{mButtonView.GetType().Name}\" component in \"{ViewModelName}\"!");
				return;
			}

			IsAvailable = true;

			return;
		}

		/// <summary>해당 ViewModel이 바인딩 가능한지 확인합니다.</summary>
		/// <param name="mono"></param>
		/// <returns></returns>
		public bool IsBindable(MonoBehaviour mono)
		{
			if (!ViewModelName.IsValid())
			{
				return false;
			}

			var findedButtons = mono.GetComponentsInChildren<Button>();

			foreach (var button in findedButtons)
			{
				if (button.gameObject.name == ViewModelName)
				{
					mButtonView = button;
					return true;
				}
			}

			return false;
		}
		public void BindAction(UnityAction onClickAction)
		{
			if (!IsAvailable)
			{
				Ulog.Log(UlogType.UI, $"Binding failed! It's not available.");
				return;
			}

			if (mOnClickAction != null)
			{
				mButtonView.onClick.RemoveListener(mOnClickAction);
			}
			mOnClickAction = onClickAction;
			mButtonView.onClick.AddListener(mOnClickAction);
		}

		/// <summary> EventTriggerType에 이벤트를 추가시킬 수 있는 함수입니다. </summary>
		public void BindEventTriggerAction(UnityAction onClickAction, EventTriggerType eventTriggerType = EventTriggerType.PointerClick)
		{
			if (!IsAvailable)
			{
				Ulog.Log(UlogType.UI, $"Binding failed! It's not available.");
				return;
			}

			EventTrigger eventTrigger = mButtonView.GetComponent<EventTrigger>();

			if(!eventTrigger)
            {
				eventTrigger= mButtonView.gameObject.AddComponent<EventTrigger>();
            }

			var entry = new EventTrigger.Entry();
			entry.eventID = eventTriggerType;
			entry.callback.AddListener((e) => onClickAction());
			eventTrigger.triggers.Add(entry);
		}

		public void Release()
		{
			if (!IsAvailable)
			{
				return;
			}

			if (mOnClickAction != null)
			{
				mButtonView.onClick.RemoveListener(mOnClickAction);
			}
			mOnClickAction = null;

			IsAvailable = false;
		}

		public void SetInteractable(bool isInteractable)
		{
			mButtonView.interactable = isInteractable;
		}

		public void SetActive(bool isActive)
		{
			mButtonView.gameObject.SetActive(isActive);
		}

		public override GameObject GetViewGameObject()
		{
			return mButtonView.gameObject;
		}
	}
}

