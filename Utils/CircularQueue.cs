using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
	/// <summary>원형 큐입니다.</summary>
	public class CircularQueue<T>
	{
		private T[] mQueue;

		public int Count { get; private set;}
		public int Capacity { get; private set;}

		private int mFrontIndex;
		private int mTailIndex;

		public CircularQueue(int capacity = 8)
		{
			Capacity = capacity;
			mQueue = new T[capacity];

			Clear();
		}

		public void Clear()
		{
			Count = 0;
			mFrontIndex = 0;
			mTailIndex = 0;
		}

		public void Reserve(int capacity)
		{
			if (Capacity >= capacity)
			{
				return;
			}

			Capacity = capacity;

			T[] allocate = new T[Capacity];
			Array.Copy(mQueue, 0, allocate, 0, mQueue.Length);
			mQueue = allocate;
		}

		public bool TryEnqueue(T value)
		{
			if (IsFull())
			{
				return false;
			}

			Count++;
			mQueue[mTailIndex] = value;
			mTailIndex = (mTailIndex + 1) % Capacity;
			return true;
		}

		public bool TryDequeue(out T value)
		{
			if (IsEmpty())
			{
				value = default(T);
				return false;
			}

			Count--;
			value = mQueue[mFrontIndex];
			mFrontIndex = (mFrontIndex + 1) % Capacity;
			return true;
		}

		public bool IsEmpty() => Count == 0;

		public bool IsFull() => Count == Capacity;
	}
}
