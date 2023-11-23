using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
	public static class FileHandler
	{
		public static bool TrySaveToFile(string filePath, string data)
		{
			return TrySaveToFile(new Uri(filePath), data);
		}

		public static bool TrySaveToFile(Uri fileUri, string data)
		{
			try
			{
				using (FileStream fs = new FileStream(fileUri.OriginalString, FileMode.Create))
				using (StreamWriter sw = new StreamWriter(fs))
				{
					sw.Write(data);
				}

				return true;
			}
			catch (Exception e)
			{
				Ulog.LogError(UlogType.FileHandler, e);
				return false;
			}
		}

		public static bool TryLoadTextFromFile(string filePath, out string data)
		{
			return TryLoadTextFromFile(new Uri(filePath), out data);
		}

		public static bool TryLoadTextFromFile(Uri fileUri, out string data)
		{
			try
			{
				using (FileStream fs = new FileStream(fileUri.OriginalString, FileMode.Open))
				using (StreamReader sr = new StreamReader(fs))
				{
					data = sr.ReadToEnd();
				}

				return true;
			}
			catch (Exception e)
			{
				Ulog.LogError(UlogType.FileHandler, e);
				data = null;
				return false;
			}
		}
	}
}
