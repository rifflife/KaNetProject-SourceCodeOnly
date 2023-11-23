using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;

using UnityEngine;
using UnityEngine.AI;

using Sirenix.OdinInspector;
using KaNet.Synchronizers;
using Gameplay;
using Utils;

namespace NetworkAI
{
	[RequireComponent(typeof(Entity_Creature))]
	public class Creature_StateController : StateController
	{
		[field: SerializeField, Header("조종자")]
		public Entity_Creature Entity { get; protected set; }

		[field: Header("보조 상태")]
		[field: SerializeField]
		public StateGroup ChaseState { get; private set; }

		[field: Header("판단 변수")]
		[field: SerializeField]
		public EntityBase TargetEntity { get; set; }

		public override void OnValidate()
		{
			base.OnValidate();
			Entity = GetComponent<Entity_Creature>();
		}

		public override void OnInitialize(DeltaTimeInfo deltaTimeInfo)
		{
			base.OnInitialize(deltaTimeInfo);

			TargetEntity = null;
		}

		public void SetChaseMode(DeltaTimeInfo deltaTimeInfo, EntityBase entity)
		{
			if (ChaseState == null)
			{
				Ulog.LogWarning(this, $"ChaseMode가 없습니다.");
				return;
			}

			this.TargetEntity = entity;
			this.ChangeState(deltaTimeInfo, ChaseState);
		}
	}
}
