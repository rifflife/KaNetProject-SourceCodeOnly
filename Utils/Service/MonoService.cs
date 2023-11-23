using System;
using UnityEngine;

namespace Utils.Service
{
	public enum UnregisteredOption
	{
		Destroy = 0,
		Disable,
	}

	/// <summary>모노 서비스 제공자 추상 클래스입니다.</summary>
	public abstract class MonoService : MonoBehaviour, IServiceable
	{
		[field : SerializeField] public UnregisteredOption UnregisteredOption = UnregisteredOption.Destroy;

		/// <summary>모노로 부터 삭제되었을 때 발생합니다. OnDestroy나 OnDisable과 같은 이벤트에서 발생됩니다.</summary>
		public event Action<MonoService> OnRemovedByMono;

		/// <summary>현재 등록된 서비스인지 여부입니다.</summary>
		public bool IsRegistered { get; private set; } = false;

		/// <summary>서비스를 초기화합니다. 등록 절차 직후 호출됩니다.</summary>
		protected virtual void onInitialize() { }

		public virtual void OnRegistered()
		{
			IsRegistered = true;
			gameObject.SetActive(true);
			this.onInitialize();
		}

		public virtual void OnUnregistered()
		{
			if (!IsRegistered)
			{
				return;
			}

			IsRegistered = false;

			if (UnregisteredOption == UnregisteredOption.Disable)
			{
				gameObject.SetActive(false);
				return;
			}

			Destroy(gameObject);
		}

		protected virtual void OnDestroy()
		{
			if (IsRegistered)
			{
				IsRegistered = false;
				OnUnregistered();
				OnRemovedByMono?.Invoke(this);
			}
		}
	}
}
