using System;
using System.Collections.Generic;

namespace KaNet.Synchronizers
{
	public enum BaseNetObjectType : ushort
	{
		None = 0,
		System,
		Data,
		Locator,
		Entity,
		Item,
		Hitscan,
	}

	public enum NetObjectType : ushort
	{
		None = 0,

		/// <summary>시스템 관련 네트워크 객체입니다.</summary>
		System = 100,
		System_GameHandler,
		System_GameplayManager,
		System_DataHandler,
		System_EventDispathcer,
		System_HitscanHandler,
		System_EffectHandler,
		System_IngameSessionHandler,
		System_ChatHandler,
		System_SoundHandler,
		System_EventManager,
		System_LevelScaler,

		System_Test,

		/// <summary>데이터 관련 네트워크 객체입니다.</summary>
		Data = 1000,
		Data_UserSession,

		Data_Test,

		/// <summary>로케이터 관련 네트워크 객체입니다.</summary>
		Locator = 10000,

		Locator_Test,

		/// <summary>엔티티 관련 객체입니다.</summary>
		Entity = 20000,
		Entity_Remote,
		Entity_Creature,

		Entity_Player,

		Entity_TestEnemy,
		Entity_TestCube,
		Entity_TestSpanwer,
		Entity_TestTarget,

		Entity_DummyEnemy,
		Entity_ShieldSpider,

		/// <summary>아이템 관련 객체입니다.</summary>
		Item = 30000,
		Item_FieldItem,

		/// <summary>히트스캔 관련 객체입니다.</summary>
		Hitscan = 40000,
		Hitscan_Area,
	}

	public static class NetObjectTypeExtension
	{
		private static List<(NetObjectType Threshold, BaseNetObjectType BaseType)> mBaseNetObjectTypeTable = new()
		{
			(NetObjectType.System,	BaseNetObjectType.System),
			(NetObjectType.Data,	BaseNetObjectType.Data),
			(NetObjectType.Locator,	BaseNetObjectType.Locator),
			(NetObjectType.Entity,	BaseNetObjectType.Entity),
			(NetObjectType.Item,	BaseNetObjectType.Item),
			(NetObjectType.Hitscan,	BaseNetObjectType.Hitscan),
		};

		/// <summary>네트워크 오브젝트의 기본 타입을 반환합니다.</summary>
		/// <returns>기본 네트워크 오브젝트 타입</returns>
		public static BaseNetObjectType GetBaseType(this NetObjectType type)
		{
			for (int i = mBaseNetObjectTypeTable.Count - 1; i >= 0; i--)
			{
				var table = mBaseNetObjectTypeTable[i];

				if (type >= table.Threshold)
				{
					return table.BaseType;
				}
			}

			return BaseNetObjectType.None;
		}

		public static bool IsBaseType(this NetObjectType type, BaseNetObjectType baseType)
		{
			return type.GetBaseType() == baseType;
		}

		public static bool IsPlayerType(this NetObjectType type)
		{
			return (type == NetObjectType.Entity_Player);
		}
	}
}
