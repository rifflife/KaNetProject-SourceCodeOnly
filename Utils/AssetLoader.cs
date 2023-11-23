using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

namespace Utils
{
	public static class AssetLoader
	{
		public static readonly string DataPathOnEnvironment = Application.dataPath.Replace('/', '\\');

		/// <summary>해당 위치의 Prefab을 불러옵니다.</summary>
		/// <param name="folderPathInAsset">폴더 위치</param>
		/// <param name="containContext">포함되어야 하는 문자열 리스트</param>
		/// <returns>Prefab 리스트</returns>
		public static List<GameObject> GetPrefabsFileFromPath(string folderPathInAsset, List<string> containContext = null)
		{
			Queue<string> folder = new Queue<string>();
			List<GameObject> loadedPrefabs = new List<GameObject>();

			folder.Enqueue(folderPathInAsset);

			while (!folder.IsEmpty())
			{
				string path = folder.Dequeue();

				foreach (var child in Directory.GetDirectories(path))
				{
					folder.Enqueue(child);
				}

				var filePaths = Directory.GetFiles(path);

				foreach (string filePath in filePaths)
				{
					bool isNotMatch = false;

					if (containContext != null)
					{
						foreach (string context in containContext)
						{
							if (!string.IsNullOrEmpty(context) && filePath.ToLower().Contains(context.ToLower()) == false)
							{
								isNotMatch = true;
								break;
							}
						}
					}
				
					if (isNotMatch)
						continue;

					string currentFilePath = filePath.Replace(DataPathOnEnvironment, "Assets");

					if (IsPrefabFile(currentFilePath) && TryLoadPrefabFromFilePath(currentFilePath, out var loadedPrefab))
					{
						loadedPrefabs.Add(loadedPrefab);
					}
				}
			}

			return loadedPrefabs;
		}

		public static bool TryLoadPrefabFromFilePath(string prefabFilePath, out GameObject prefabObject)
		{
			prefabObject = AssetDatabase.LoadAssetAtPath(prefabFilePath, typeof(GameObject)) as GameObject;
			return prefabObject != null;
		}

		public static bool IsPrefabFile(string prefabFilePath)
		{
			string fileExtension = Path.GetExtension(prefabFilePath).ToLower();
			return fileExtension == ".prefab";
		}

		/// <summary>해당 위치의 Asset리스트를 불러옵니다.</summary>
		/// <typeparam name="T">Asset 타입</typeparam>
		/// <param name="folderPathInAsset">폴더 위치</param>
		/// <param name="containContext">포함되어야 하는 문자열 리스트</param>
		/// <returns>Asset 리스트</returns>
		public static List<T> GetAssetsFromPath<T>
		(
			string folderPathInAsset,
			List<string> containContext = null
		)
			where T : UnityEngine.Object
		{
			Queue<string> folder = new Queue<string>();
			List<T> loadedPrefabs = new List<T>();

			folder.Enqueue(folderPathInAsset);

			while (!folder.IsEmpty())
			{
				string path = folder.Dequeue();

				foreach (var child in Directory.GetDirectories(path))
				{
					folder.Enqueue(child);
				}

				var filePaths = Directory.GetFiles(path);

				foreach (string filePath in filePaths)
				{
					bool isNotMatch = false;

					if (containContext != null)
					{
						foreach (string context in containContext)
						{
							if (!string.IsNullOrEmpty(context) && filePath.ToLower().Contains(context.ToLower()) == false)
							{
								isNotMatch = true;
								break;
							}
						}
					}

					if (isNotMatch)
						continue;

					string currentFilePath = filePath.Replace(DataPathOnEnvironment, "Assets");

					if (TryLoadAssetFromFilePath<T>(currentFilePath, out var loadedPrefab))
					{
						loadedPrefabs.Add(loadedPrefab);
					}
				}
			}

			return loadedPrefabs;
		}

		public static bool TryLoadAssetFromFilePath<T>(string assetFilePath, out T assetObject)
			where T : UnityEngine.Object
		{
			assetObject = AssetDatabase.LoadAssetAtPath(assetFilePath, typeof(T)) as T;
			return assetObject != null;
		}
	}
}

#endif