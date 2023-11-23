using System.Collections.Generic;
using UnityEngine;

namespace Utile
{
	public class MonoObjectPool
	{
		public int Count => mObjectStack.Count;
		private Stack<GameObject> mObjectStack = new();
		private GameObject mReferenceInstance;
		private Transform mTransform;

		public MonoObjectPool(GameObject referenceInstance, Transform transform)
		{
			mReferenceInstance = referenceInstance;
			mTransform = transform;
		}

		public GameObject Get(Vector3 position, Quaternion rotation)
		{
			if (mObjectStack.TryPop(out GameObject instance))
			{
				instance.SetActive(true);
				instance.transform.position = position;
				instance.transform.rotation = rotation;
				return instance;
			}

			var go = Object.Instantiate(mReferenceInstance, position, rotation, mTransform);
			go.SetActive(true);
			return go;
		}

		public void Release(GameObject instance)
		{
			instance.SetActive(false);
			mObjectStack.Push(instance);
		}

		public void Clear()
		{
			while (mObjectStack.Count > 0)
			{
				var instance = mObjectStack.Pop();
				Object.Destroy(instance);
			}
		}
	}
}