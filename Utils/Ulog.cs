using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Utils
{
	public enum UlogType
	{
		None = 0,
		Process = 1, // The initializer when the game initial started
		Service = 2,
		GlobalServiceLocator,
















		// Game
		InGame = 100,
		UI,






















		// Network
		Network = 200,
		Lobby,
		UdpSocket,
		SteamworksAPI,
		Session,
		NetPacketPool,
		NetworkReflection,









		// External
		Editor = 1000,
		CodeGenerator,
		JsonHandler,
		FileHandler,

		Tester,
	}

	public static class Ulog
	{
		private static ULogger mLogger = new UnityLogger();
		public static ULogger Logger => mLogger;

		public static void Initialize(ULogger loggerInstance)
		{
			mLogger = loggerInstance;
		}

		/// <summary>로그 메세지를 출력합니다.</summary>
		/// <param name="log">출력할 오브젝트 입니다.</param>
		[Conditional("UNITY_LOGGER")]
		public static void Log(object log) => mLogger.Log(log);

		/// <summary>로그 메세지를 출력합니다.</summary>
		/// <param name="caller">호출한 객체입니다.</param>
		/// <param name="log">출력할 오브젝트 입니다.</param>
		[Conditional("UNITY_LOGGER")]
		public static void Log(object caller, object log) => mLogger.Log(caller, log);

		/// <summary>로그 메세지를 출력합니다.</summary>
		/// <param name="logType">출력할 로그의 타입입니다.</param>
		/// <param name="log">출력할 오브젝트 입니다.</param>
		[Conditional("UNITY_LOGGER")]
		public static void Log(UlogType logType, object log) => mLogger.Log(logType, log);

		/// <summary>오류 로그 메세지를 출력합니다.</summary>
		/// <param name="log">출력할 오브젝트 입니다.</param>
		[Conditional("UNITY_LOGGER")]
		public static void LogError(object log) => mLogger.LogError(log);

		/// <summary>오류 로그 메세지를 출력합니다.</summary>
		/// <param name="caller">호출한 객체입니다.</param>
		/// <param name="log">출력할 오브젝트 입니다.</param>
		[Conditional("UNITY_LOGGER")]
		public static void LogError(object caller, object log) => mLogger.LogError(caller, log);

		/// <summary>오류 로그 메세지를 출력합니다.</summary>
		/// <param name="logType">출력할 로그의 타입입니다.</param>
		/// <param name="log">출력할 오브젝트 입니다.</param>
		[Conditional("UNITY_LOGGER")]
		public static void LogError(UlogType logType, object log) => mLogger.LogError(logType, log);

		/// <summary>경고 로그 메세지를 출력합니다.</summary>
		/// <param name="log">출력할 오브젝트 입니다.</param>
		[Conditional("UNITY_LOGGER")]
		public static void LogWarning(object log) => mLogger.LogWarning(log);

		/// <summary>경고 로그 메세지를 출력합니다.</summary>
		/// <param name="caller">호출한 객체입니다.</param>
		/// <param name="log">출력할 오브젝트 입니다.</param>
		[Conditional("UNITY_LOGGER")]
		public static void LogWarning(object caller, object log) => mLogger.LogWarning(caller, log);

		/// <summary>경고 로그 메세지를 출력합니다.</summary>
		/// <param name="logType">출력할 로그의 타입입니다.</param>
		/// <param name="log">출력할 오브젝트 입니다.</param>
		[Conditional("UNITY_LOGGER")]
		public static void LogWarning(UlogType logType, object log) => mLogger.LogWarning(logType, log);
		public static void LogNoInitialize(object caller) => Ulog.LogError(caller, $"It's not available. Please use after initialization.");
		public static void LogNoComponent(MonoBehaviour mono, object component) => Ulog.LogError($"There is no {component.GetType().Name} in {mono.gameObject.name}");

		public static void ChangeLogger(ULogger logger) => mLogger = logger;
		public static void SetConsoleLogger() => mLogger = new ConsoleLogger();
		public static void SetUnityLogger() => mLogger = new UnityLogger();
	}

	public abstract class ULogger
	{
		protected const string ERROR_PREFIX = "[ERROR]";
		protected const string WARNING_PREFIX = "[WARNING]";

		public abstract void Log(object log);
		public abstract void Log(object caller, object log);
		public abstract void Log(UlogType logType, object log);
		public abstract void LogError(object log);
		public abstract void LogError(object caller, object log);
		public abstract void LogError(UlogType logType, object log);
		public abstract void LogWarning(object log);
		public abstract void LogWarning(object caller, object log);
		public abstract void LogWarning(UlogType logType, object log);
	}

	public class UnityLogger : ULogger
	{
		public override void Log(object log)
		{
			string message = log.ToString();
			UnityEngine.Debug.Log(message);
		}

		public override void Log(object caller, object log)
		{
			string message = $"[{caller.GetType().Name}] {log}";
			UnityEngine.Debug.Log(message);
		}

		public override void Log(UlogType logType, object log) 
		{
			string message = $"[{logType}] {log}";
			UnityEngine.Debug.Log(message);
		}

		public override void LogError(object log)
		{
			string message = $"{ERROR_PREFIX} {log}";
			UnityEngine.Debug.LogError(message);
		}

		public override void LogError(object caller, object log)
		{
			string message = $"[{caller.GetType().Name}]{ERROR_PREFIX} {log}";
			UnityEngine.Debug.LogError(message);
		}

		public override void LogError(UlogType logType, object log)
		{
			string message = $"[{logType}]{ERROR_PREFIX} {log}";
			UnityEngine.Debug.LogError(message);
		}

		public override void LogWarning(object log)
		{
			string message = $"{WARNING_PREFIX} {log}";
			UnityEngine.Debug.LogWarning(message);
		}

		public override void LogWarning(object caller, object log)
		{
			string message = $"[{caller.GetType().Name}]{WARNING_PREFIX} {log}";
			UnityEngine.Debug.LogWarning(message);
		}

		public override void LogWarning(UlogType logType, object log)
		{
			string message = $"[{logType}]{WARNING_PREFIX} {log}";
			UnityEngine.Debug.LogWarning(message);
		}
	}

	public class ConsoleLogger : ULogger
	{
		public override void Log(object log) => Console.WriteLine(log);
		public override void Log(object caller, object log) => Console.WriteLine($"[{caller.GetType().Name}] {log}");
		public override void Log(UlogType logType, object log) => Console.WriteLine($"[{logType}] {log}");
		public override void LogError(object log) => Console.WriteLine($"{ERROR_PREFIX} {log}");
		public override void LogError(object caller, object log) => Console.WriteLine($"[{caller.GetType().Name}]{ERROR_PREFIX} {log}");
		public override void LogError(UlogType logType, object log) => Console.WriteLine($"[{logType}]{ERROR_PREFIX} {log}");
		public override void LogWarning(object log) => Console.WriteLine($"{WARNING_PREFIX} {log}");
		public override void LogWarning(object caller, object log) => Console.WriteLine($"[{caller.GetType().Name}]{WARNING_PREFIX} {log}");
		public override void LogWarning(UlogType logType, object log) => Console.WriteLine($"[{logType}]{WARNING_PREFIX} {log}");
	}

	public class NullLogger : ULogger
	{
		public override void Log(object log) { }
		public override void Log(object caller, object log) { }
		public override void Log(UlogType logType, object log) { }
		public override void LogError(object log) { }
		public override void LogError(object caller, object log) { }
		public override void LogError(UlogType logType, object log) { }
		public override void LogWarning(object log) { }
		public override void LogWarning(object caller, object log) { }
		public override void LogWarning(UlogType logType, object log) { }
	}
}

