using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PluggableAI
{
	[CreateAssetMenu(menuName = "PluggableAI/Decisions/ChaseTarget")]
	public class ChaseTargetDecision : Decision
	{
		public override bool Decide(StateController controller)
		{
			bool targetVisible = isPlayerInViewRange((IChaseTargetable)controller, controller.gameObject);
			return targetVisible;
		}

		/// <summary> 플레이어가 시야 원형 범위안에 있는지 확인하는 함수 </summary>
		private bool isPlayerInViewRange(IChaseTargetable controller, GameObject gameObject)
		{
			Vector3 position = gameObject.transform.position;

			float radius = controller.ChaseTargetRadius;

			Collider2D[] collidersInRadius = Physics2D.OverlapCircleAll(position, radius, GlobalLayer.LAYER_RAYCAST_ENTITY_AREA);

			foreach (Collider2D collider in collidersInRadius)
			{
				if (collider.gameObject != gameObject)
				{
					controller.Target = collider.transform;
					return true;
				}
			}

			return false;
		}
	}
}