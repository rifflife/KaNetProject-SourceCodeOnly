using System.Collections;
using System.Collections.Generic;
using Gameplay;
using KaNet.Synchronizers;
using UnityEngine;


namespace NetworkAI
{
	public class Action_Attack : StateAction
	{
		[SerializeField] private float mAttackElapsed;

		public override bool IsLock => mAttackElapsed > 0;

		public override void OnStart(StateController controller, DeltaTimeInfo deltaTimeInfo)
		{
			mAttackElapsed = 2.0f;
		}

		public override void OnAct(StateController controller, DeltaTimeInfo deltaTimeInfo)
		{
			var creatureController = (Creature_StateController)controller;

			mAttackElapsed -= deltaTimeInfo.ScaledDeltaTime;

			if (mAttackElapsed > 0)
			{
				return;
			}

			mAttackElapsed = 2.0f;

			var entity = creatureController.Entity;
			var target = creatureController.TargetEntity;

			entity.Server_ProxyAnimationState.Data = AnimationType.Attack_Front;
			entity.Server_ActAttack(AttackType.Normal_1);
			entity.Server_LookAt(target.transform);
			entity.Server_StopAgent();
		}

		public override void OnEnd(StateController controller, DeltaTimeInfo deltaTimeInfo)
		{
		}
	}
}
