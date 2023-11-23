using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils.Service;

namespace Utils
{
	public class ObjectPool<T> where T : class, IManageable, new()
	{
		public int Count => mObjectStack.Count;
		private Stack<T> mObjectStack;

		public ObjectPool(int capacity = 8)
		{
			mObjectStack = new Stack<T>(capacity);

			for (int i = 0; i < capacity; i++)
			{
				mObjectStack.Push(new T());
			}
		}

		public T Get()
		{
			T obj = mObjectStack.IsEmpty() ? new T() : mObjectStack.Pop();
			obj.OnInitialize();
			return obj;
		}

		public void Return(T obj)
		{
			if (obj == null)
			{
				return;
			}

			obj.OnFinalize();
			mObjectStack.Push(obj);
		}
	}

	public class StaticObjectPool<T> where T : class, IManageable
	{
		public int Capacity { get; private set; }
		public int Count { get; private set; }
		private Stack<T> mObjectStack;

		public StaticObjectPool(IEnumerable<T> initialObjects)
		{
			mObjectStack = new Stack<T>(initialObjects);
			Capacity = mObjectStack.Count;
			Count = Capacity;
		}

		public T Get()
		{
			T obj = mObjectStack.Pop();
			obj.OnInitialize();
			Count++;
			return obj;
		}

		public void Return(T obj)
		{
			if (obj == null)
			{
				return;
			}

			obj.OnFinalize();
			mObjectStack.Push(obj);
			Count--;
		}
	}
}
