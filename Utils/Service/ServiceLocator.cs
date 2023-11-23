using System;

namespace Utils.Service
{
	/// <summary>서비스 중개자의 등록 취소 인터페이스입니다.</summary>
	public interface IUnregistrable
	{
		public void UnregisterService();
	}

	/// <summary>서비스 제공자 인터페이스입니다.</summary>
	public interface IServiceable
	{
		/// <summary>서비스가 등록되었을 때 호출됩니다.</summary>
		public void OnRegistered();
		/// <summary>서비스가 등록 취소되었을 때 호출됩니다.</summary>
		public void OnUnregistered();
	}

	/// <summary>서비스 중개자 클래스입니다.</summary>
	/// <typeparam name="T">서비스의 타입 인터페이스 혹은 클래스</typeparam>
	public class ServiceLocator<T> : IUnregistrable
		where T : class, IServiceable
	{
		/// <summary>서비스가 등록되었을 때 발생하는 이벤트입니다.</summary>
		public Action<T> OnServiceRegistered;
		/// <summary>서비스가 등록 취소되었을 때 발생하는 이벤트입니다.</summary>
		public Action<T> OnServiceUnregistered;

		private T mService = null;
		private string mServiceName = "";
		public string ServiceName => mServiceName;

		/// <summary>서비스 중개자를 생성합니다.</summary>
		/// <param name="initialService">초기 서비스 제공자입니다.</param>
		public ServiceLocator(T initialService = null)
		{
			RegisterService(initialService);
		}

		/// <summary>서비스 제공자를 등록합니다.</summary>
		/// <param name="service">서비스 제공자입니다.</param>
		public void RegisterService(T service)
		{
			if (service == null)
			{
				return;
			}

			if (mService != null)
			{
				mService.OnUnregistered();
				OnServiceUnregistered?.Invoke(mService);
			}

			mService = service;
			mService.OnRegistered();
			OnServiceRegistered?.Invoke(mService);

			mServiceName = mService.GetType().Name;
		}

		/// <summary>서비스 제공자를 등록 취소합니다.</summary>
		public void UnregisterService()
		{
			if (mService == null)
			{
				return;
			}

			mService.OnUnregistered();
			OnServiceUnregistered?.Invoke(mService);

			mService = null;
			mServiceName = "";
		}

		/// <summary>서비스 제공자를 반환받습니다. 등록되어있지 않다면 null을 반환합니다.</summary>
		/// <returns>등록된 서비스 제공자 혹은 null</returns>
		public T GetServiceOrNull()
		{
			return mService;
		}

		/// <summary>서비스 제공자를 반환받습니다. 서비스 제공자 반환에 성공하면 true를 반환받습니다.</summary>
		/// <param name="service">서비스 제공자</param>
		/// <returns>서비스 제공자 반환 성공 여부</returns>
		public bool TryGetService(out T service)
		{
			service = mService;
			return mService != null;
		}

		public override string ToString()
		{
			return $"Service:{mServiceName}";
		}
	}
}
