using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
	[Obsolete("GC 수행을 덜 하지만 유의미하지 않음")]
	public class CachedDictionary<Key, Value>
	{
		private Dictionary<Key, Value> mDictionary = new();
		private List<Key> mKeyList = new();
		private List<Value> mValueList = new();

		public IList<Key> Keys => mKeyList;
		public IList<Value> Values => mValueList;

		public void Add(Key key, Value value)
		{
			mDictionary.Add(key, value);
			mKeyList.Add(key);
			mValueList.Add(value);
		}

		public void Remove(Key key)
		{
			var value = mDictionary[key];
			mDictionary.Remove(key);
			mKeyList.Remove(key);
			mValueList.Remove(value);
		}

		public void Clear()
		{
			mDictionary.Clear();
			mKeyList.Clear();
			mValueList.Clear();
		}
	}
}
