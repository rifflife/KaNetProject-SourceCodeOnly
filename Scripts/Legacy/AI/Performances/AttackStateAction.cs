using System.Collections;
using System.Collections.Generic;
using Gameplay;
using UnityEngine;


namespace PluggableAI
{
	[CreateAssetMenu(menuName = "PluggableAI/Actions/Attack")]
	public class AttackStateAction : StateAction
	{
		private float mAttackElapsed;

		public override void OnInitialize(StateController controller)
		{
			mAttackElapsed = 0;
		}

		public override void Act(StateController controller)
		{
			//if (mAttackElapsed > 0)
			//{
			//	mAttackElapsed += deltaTime;
			//}

			//controller.PlayAnimation(AnimationType.Attack);
			//controller.StopAgent();
		}

		private void LookAt(IChaseTargetable controller)
		{
			//Vector3 scale = controller.NavigationAgent.transform.localScale;
			//Vector2 direction = controller.Target.position - controller.NavigationAgent.transform.position;
			//direction.Normalize();

			//if (direction.x > 0)
			//{
			//	scale.x = -Mathf.Abs(scale.x);
			//}
			//else
			//{
			//	scale.x = Mathf.Abs(scale.x);
			//}

			//controller.NavigationAgent.transform.localScale = scale;
		}
	}
}
