using System.Collections;
using System.Collections.Generic;
using Gameplay;
using UnityEngine;

namespace PluggableAI
{
	[CreateAssetMenu(menuName = "PluggableAI/Decisions/AttackTarget")]
	public class AttackTargetDecision : Decision
	{
		public override bool Decide(StateController controller)
		{
			bool targetVisible = isPlayerInAttackRange((IAttackTargetable)controller, controller.gameObject);
			return targetVisible;
		}

		/// <summary> 플레이어가 시야 원형 범위안에 있는지 확인하는 함수 </summary>
		private bool isPlayerInAttackRange(IAttackTargetable controller, GameObject gameObject)
		{
			Vector3 position = gameObject.transform.position;

			float radius = controller.AttackTargetRadius;

			Collider2D[] collidersInRadius = Physics2D.OverlapCircleAll(position, radius, GlobalLayer.LAYER_RAYCAST_ENTITY_AREA);

			//Debug.Log(gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("EnemyAttack"));

			foreach (Collider2D collider in collidersInRadius)
			{
				if (collider.gameObject != gameObject)
				{
					controller.AttackTarget = collider.transform;
					return true;
				}
			}
			var stateInfo = gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);
			if (stateInfo.IsName(AnimationType.Attack_Front.GetAnimationName()) && stateInfo.normalizedTime < 1)
			{
				return true;
			}

			return false;
		}
	}
}