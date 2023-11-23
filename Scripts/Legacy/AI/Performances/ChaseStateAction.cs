using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PluggableAI
{
	[CreateAssetMenu(menuName = "PluggableAI/Actions/Chase")]
	public class ChaseStateAction : StateAction
	{
		public override void OnInitialize(StateController controller)
		{
			Debug.Log("������ �����մϴ�");
		}

		public override void Act(StateController controller)
		{
			Chase((IChaseTargetable)controller);
			LookAt((IChaseTargetable)controller);
			
			PlayAnimation(controller);
		}

		private void Chase(IChaseTargetable controller)
		{
			//controller.NavigationAgent.destination = controller.Target.position; // State Initialize �����
			//controller.NavigationAgent.isStopped = false;
		}

		private void PlayAnimation(StateController controller)
		{
			//controller.PlayAnimation("EnemyRun", true);
		}

		private void LookAt(IChaseTargetable controller)
		{
			//Vector3 scale = controller.NavigationAgent.transform.localScale;

			//if (controller.NavigationAgent.velocity.x > 0)
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
