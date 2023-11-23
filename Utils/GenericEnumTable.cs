using System;
using System.Collections;
using System.Collections.Generic;

namespace Utils
{
	public readonly struct EnumPair<EnumType, BaseEnumType>
		where EnumType : Enum
		where BaseEnumType : Enum
	{
		public readonly EnumType Type;
		public readonly BaseEnumType BaseType;

		public EnumPair(EnumType type, BaseEnumType baseType)
		{
			Type = type;
			BaseType = baseType;
		}
	}

	public class GenericEnumTableUInt8<EnumType, BaseEnumType>
		: IEnumerable<EnumPair<EnumType, BaseEnumType>>
		where EnumType : Enum
		where BaseEnumType : Enum
	{
		public void Add(EnumType enumType, BaseEnumType baseEnum)
		{
			mBaseFactionTypeTable.Add(new EnumPair<EnumType, BaseEnumType>(enumType, baseEnum));
		}
		
		private List<EnumPair<EnumType, BaseEnumType>> mBaseFactionTypeTable = new();

		/// <summary>기본 타입을 반환합니다.</summary>
		/// <returns>기본 타입</returns>
		public BaseEnumType GetBaseType(EnumType type)
		{
			for (int i = mBaseFactionTypeTable.Count - 1; i >= 0; i--)
			{
				var table = mBaseFactionTypeTable[i];

				if ((byte)(object)type >= (byte)(object)table.Type)
				{
					return table.BaseType;
				}
			}

			return default(BaseEnumType);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		IEnumerator<EnumPair<EnumType, BaseEnumType>> IEnumerable<EnumPair<EnumType, BaseEnumType>>.GetEnumerator()
		{
			throw new NotImplementedException();
		}
	}

	public class GenericEnumTableUInt16<EnumType, BaseEnumType>
		: IEnumerable<EnumPair<EnumType, BaseEnumType>>
		where EnumType : Enum
		where BaseEnumType : Enum
	{
		public void Add(EnumType enumType, BaseEnumType baseEnum)
		{
			mBaseFactionTypeTable.Add(new EnumPair<EnumType, BaseEnumType>(enumType, baseEnum));
		}

		private List<EnumPair<EnumType, BaseEnumType>> mBaseFactionTypeTable = new();

		/// <summary>기본 타입을 반환합니다.</summary>
		/// <returns>기본 타입</returns>
		public BaseEnumType GetBaseType(EnumType type)
		{
			for (int i = mBaseFactionTypeTable.Count - 1; i >= 0; i--)
			{
				var table = mBaseFactionTypeTable[i];

				if ((ushort)(object)type >= (ushort)(object)table.Type)
				{
					return table.BaseType;
				}
			}

			return default(BaseEnumType);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		IEnumerator<EnumPair<EnumType, BaseEnumType>> IEnumerable<EnumPair<EnumType, BaseEnumType>>.GetEnumerator()
		{
			throw new NotImplementedException();
		}
	}

	public class GenericEnumTableUInt32<EnumType, BaseEnumType>
		: IEnumerable<EnumPair<EnumType, BaseEnumType>>
		where EnumType : Enum
		where BaseEnumType : Enum
	{
		public void Add(EnumType enumType, BaseEnumType baseEnum)
		{
			mBaseFactionTypeTable.Add(new EnumPair<EnumType, BaseEnumType>(enumType, baseEnum));
		}

		private List<EnumPair<EnumType, BaseEnumType>> mBaseFactionTypeTable = new();

		/// <summary>기본 타입을 반환합니다.</summary>
		/// <returns>기본 타입</returns>
		public BaseEnumType GetBaseType(EnumType type)
		{
			for (int i = mBaseFactionTypeTable.Count - 1; i >= 0; i--)
			{
				var table = mBaseFactionTypeTable[i];

				if ((uint)(object)type >= (uint)(object)table.Type)
				{
					return table.BaseType;
				}
			}

			return default(BaseEnumType);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		IEnumerator<EnumPair<EnumType, BaseEnumType>> IEnumerable<EnumPair<EnumType, BaseEnumType>>.GetEnumerator()
		{
			throw new NotImplementedException();
		}
	}
}
