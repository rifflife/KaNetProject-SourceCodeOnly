using System.Collections;
using System.Collections.Generic;

namespace Utils
{
    public static class CollectionExtension
    {
        public static bool IsEqual<T>(this IList<T> lhs, IList<T> rhs)
        {
            if (lhs.Count != rhs.Count)
            {
                return false;
            }

            int count = lhs.Count;

            for (int i = 0; i < count; i++)
            {
                if (!lhs[i].Equals(rhs[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool IsEmpty(this ICollection collection)
		{
            return collection.Count <= 0;
		}

        //public static bool IsEmpty<T>(this ICollection<T> collection)
        //{
        //    return collection.Count <= 0;
        //}

        public static bool IsNullOrEmpty(this ICollection collection)
        {
            return collection == null || collection.Count <= 0;
        }

        public static bool IsNullOrEmpty<T>(this ICollection<T> collection)
        {
            return collection == null || collection.Count <= 0;
        }

        /// <summary>요소가 포함되어 있지 않다면 추가합니다.</summary>
        /// <typeparam name="T">추가할 요소의 타입</typeparam>
        /// <param name="collection">대상 컬렉션</param>
        /// <param name="value">추가할 요소입니다.</param>
        /// <returns>추가하는데 성공한다면 true를 반환합니다.</returns>
        public static bool TryAddUnique<T>(this ICollection<T> collection, T value)
		{
            if (!collection.Contains(value))
			{
                collection.Add(value);
                return true;
			}

            return false;
		}

        public static bool TryAddUniqueByKey<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
		{
            if (dictionary.ContainsKey(key))
			{
                return false;
			}

            dictionary.Add(key, value);
            return true;
		}
    }
}
