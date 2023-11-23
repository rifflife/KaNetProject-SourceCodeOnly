using UnityEngine;

namespace Utils
{
	public static class UnityExtension
	{
		public static bool TryGetComponentByName<T>(string objectName, out T component)
			where T : Object
		{
			var findedObjects = Object.FindObjectsOfType<T>();

			foreach (var obj in findedObjects)
			{
				if (obj.name == objectName)
				{
					component = obj;
					return true;
				}
			}

			component = null;
			return false;
		}
	}
}
