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
		[field: SerializeField, Header("������")]
		public Entity_Creature Entity { get; protected set; }

		[field: Header("���� ����")]
		[field: SerializeField]
		public StateGroup ChaseState { get; private set; }

		[field: Header("�Ǵ� ����")]
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
				Ulog.LogWarning(this, $"ChaseMode�� �����ϴ�.");
				return;
			}

			this.TargetEntity = entity;
			this.ChangeState(deltaTimeInfo, ChaseState);
		}
	}
}
