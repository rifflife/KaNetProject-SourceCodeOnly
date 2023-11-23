using System.Collections;
using System.Collections.Generic;
using KaNet.Synchronizers;
using UnityEngine;

namespace Gameplay
{
	[CreateAssetMenu(fileName = "WeaponInfoData", menuName = "Entity Data/Weapon Info Data", order = 0)]
	public class WeaponInfoData : ScriptableObject
	{
		[field: Header("ID")]
		/// <summary>������ Ÿ���Դϴ�.</summary>
		[field: SerializeField] public EquipmentType WeaponType { get; protected set; }
		/// <summary>����Ʈ Ÿ���Դϴ�.</summary>
		[field: SerializeField] public EffectType EffectType { get; protected set; }
		/// <summary>��Ʈ���� Ÿ���Դϴ�.</summary>
		[field: SerializeField] public HitscanType HitscanType { get; protected set; }

		[field: Header("Data")]
		/// <summary>���� Ƚ���Դϴ�.</summary>
		[field: SerializeField] public byte PenetrateCount { get; protected set; }
		/// <summary>�ִ� �߻� �Ÿ��Դϴ�.</summary>
		[field: SerializeField] public float MaxDistance { get; protected set; }
		/// <summary>������ �������Դϴ�.</summary>
		[field : SerializeField] public short Damage { get; protected set; }
		/// <summary>�߻��� ������ �ӵ��Դϴ�.</summary>
		[field: SerializeField] public float Speed { get; protected set; }
		/// <summary>�Ʊ� ���� ��� �����Դϴ�.</summary>
		[field: SerializeField] public bool AllowFriendlyFire { get; protected set; }
		
		public WeaponInfo GetWeaponInfo()
		{
			return new WeaponInfo
			(
				WeaponType,
				EffectType,
				HitscanType,
				PenetrateCount,
				MaxDistance,
				Damage,
				Speed,
				AllowFriendlyFire
			);
		}
	}
}