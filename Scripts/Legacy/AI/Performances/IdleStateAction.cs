using System.Collections;
using System.Collections.Generic;
using Gameplay;
using UnityEngine;


namespace PluggableAI
{
	[CreateAssetMenu(menuName = "PluggableAI/Actions/Idle")]
	public class IdleStateAction : StateAction
	{
		public override void OnInitialize(StateController controller)
		{
		}
		public override void Act(StateController controller)
		{
			Stop(controller);
			PlayAnimation(controller);
		}

		private void Stop(StateController controller)
		{
			//controller.StopAgent();
		}

		private void PlayAnimation(StateController controller)
		{
			//controller.PlayAnimation(AnimationType.Idle.GetAnimationName(), true);
		}
	}
}
