using System.Collections.Generic;
using UnityEngine;
using Utile;
using Utils.Service;

namespace Utils.Service
{
	public class MonoObjectPoolService : IServiceable
	{
		private Dictionary<GameObject, MonoObjectPool> mMonoObjectPoolTable = new();
		private Dictionary<GameObject, MonoObjectPool> mMonoInstancePoolTable = new();

		private Transform mBaseTrasnform;

		public MonoObjectPoolService(Transform transform)
		{
			mBaseTrasnform = transform;
		}

		public GameObject CreateObject(GameObject prefab, Vector3 position, Quaternion rotation)
		{
			GameObject poolInstance;
			MonoObjectPool monoPool;

			if (!mMonoObjectPoolTable.TryGetValue(prefab, out monoPool))
			{
				monoPool = new MonoObjectPool(prefab, mBaseTrasnform);
				mMonoObjectPoolTable.Add(prefab, monoPool);
			}

			poolInstance = mMonoObjectPoolTable[prefab].Get(position, rotation);
			mMonoInstancePoolTable.Add(poolInstance, monoPool);
			
			return poolInstance;
		}

		public void Release(GameObject instance)
		{
			if (mMonoInstancePoolTable.ContainsKey(instance))
			{
				mMonoInstancePoolTable[instance].Release(instance);
				mMonoInstancePoolTable.Remove(instance);
				return;
			}

			if (instance != null)
			{
				Object.Destroy(instance);
			}
		}

		public void OnRegistered()
		{
		}

		public void OnUnregistered()
		{
			List<GameObject> destroyObjectList = new();

			foreach (var instance in mMonoInstancePoolTable.Keys)
			{
				destroyObjectList.Add(instance);
			}

			foreach (var instance in destroyObjectList)
			{
				Release(instance);
			}
		}
	}
}
