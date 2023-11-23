using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Utils
{
	public static class JsonHandler
	{
		public static JsonSerializerSettings LoadOption { get; } = new JsonSerializerSettings()
		{
			TypeNameHandling = TypeNameHandling.Auto,
			NullValueHandling = NullValueHandling.Ignore,
		};

		public static JsonSerializerSettings SaveOption { get; } = new JsonSerializerSettings()
		{
			TypeNameHandling = TypeNameHandling.Auto,
			NullValueHandling = NullValueHandling.Ignore,
			Formatting = Formatting.Indented
		};

		public static bool TrySaveToFile<T>(string path, T instance)
		{
			return TrySaveToFile(new Uri(path), instance);
		}

		public static bool TrySaveToFile<T>(Uri fileUri, T instance)
		{
			string data = ToJson(instance);
			if (FileHandler.TrySaveToFile(fileUri, data))
			{
				return true;
			}

			Ulog.LogError(UlogType.JsonHandler, $"Save instance fail!");
			return false;
		}

		public static bool TryLoadFromFile<T>(string path, out T instance)
		{
			return TryLoadFromFile(new Uri(path), out instance);
		}

		public static bool TryLoadFromFile<T>(Uri fileUri, out T instance)
		{
			if (FileHandler.TryLoadTextFromFile(fileUri, out var data))
			{
				return TryParseToInstance(data, out instance);
			}

			Ulog.LogError(UlogType.JsonHandler, $"Load instance fail!");

			instance = default;
			return false;
		}

		public static string ToJson<T>(T instance)
		{
			return JsonConvert.SerializeObject(instance, SaveOption);
		}

		public static bool TryParseToInstance<T>(string jsonData, out T instance)
		{
			try
			{
				instance = JsonConvert.DeserializeObject<T>(jsonData, LoadOption);
				return instance != null;
			}
			catch (Exception e)
			{
				Ulog.LogError(UlogType.JsonHandler, $"Try parse error! {e}");
				instance = default;
				return false;
			}
		}
	}
}
