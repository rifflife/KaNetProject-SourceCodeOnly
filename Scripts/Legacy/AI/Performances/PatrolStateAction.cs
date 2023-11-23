using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PluggableAI
{
	[CreateAssetMenu(menuName = "PluggableAI/Actions/Patrol")]
	public class PatrolStateAction : StateAction
	{
		public override void OnInitialize(StateController controller)
		{

		}

		public override void Act(StateController controller)
		{
			//Patrol((IPatrolable)controller, controller.gameObject);
			//LookAt((IPatrolable)controller);
			//PlayAnimation(controller);
		}

		//private void PlayAnimation(StateController controller)
		//{
		//	//controller.PlayAnimation("EnemyRun", true);
		//}
		//private void Patrol(IPatrolable controller, GameObject gameObject)
		//{
		//	controller.NavigationAgent.isStopped = false;

		//	if (!controller.IsReturn)
		//	{
		//		Vector2 destination = controller.StartPosition + controller.PatrolDistance;
		//		controller.NavigationAgent.destination = destination;

		//		Vector2 pos1 = gameObject.transform.position;
		//		Vector2 pos2 = destination;

		//		//Debug.Log(Mathf.Abs(Vector3.Distance(pos1, pos2)));

		//		if (Mathf.Abs(Vector2.Distance(pos1, pos2)) < 0.5)
		//		{
		//			controller.IsReturn = true;
		//		}
		//	}
		//	else
		//	{
		//		controller.NavigationAgent.destination = controller.StartPosition;

		//		Vector2 pos1 = gameObject.transform.position;
		//		Vector2 pos2 = controller.StartPosition;

		//		if (Mathf.Abs(Vector2.Distance(pos1, pos2)) < 0.5)
		//		{
		//			controller.IsReturn = false;
		//		}
		//	}


		//}

		//private void LookAt(IPatrolable controller)
		//{
		//	Vector3 scale = controller.NavigationAgent.transform.localScale;

		//	if (controller.NavigationAgent.velocity.x > 0)
		//	{
		//		scale.x = -Mathf.Abs(scale.x);
		//	}
		//	else
		//	{
		//		scale.x = Mathf.Abs(scale.x);
		//	}

		//	controller.NavigationAgent.transform.localScale = scale;
		//}
	}
}