using System;
using UnityEngine;
using Utils;


namespace PluggableAI
{
	[Serializable]
	public class StateTransition
	{
		public Decision Decision; // 다른 상태로 전환되는 조건
		public State TrueState; // 조건이 true일때 전환되는 상태
		public State FalseState; // 조건이 false일때 전환되는 상태
	}

	[CreateAssetMenu(menuName = "PluggableAI/State")]
	public class State : ScriptableObject
	{
		[SerializeField]
		private StateAction[] mPerformances; // 현재 상태에서 실행하는 행동
		[SerializeField]
		private StateTransition[] mTransitions; // 현재 상태에서 전환될 수 있는 상태와 조건
		[SerializeField]
		private StateAction[] mEndPerformances; // 현재 상태가 끝날 때 실행하는 행동

		public void UpdateState(StateController controller)
		{
			doPerformance(controller);
			checkTransitions(controller);
		}

		/// <summary> 현재 상태에서 실행되는 함수입니다. </summary>
		private void doPerformance(StateController controller)
		{
			int length = mPerformances.Length;

			for (int i = 0; i < length; i++)
			{
				mPerformances[i].Act(controller);
			}
		}

		/// <summary> 조건에 따라 다른 상태로 전환하는 함수입니다. </summary>
		private void checkTransitions(StateController controller)
		{
			int length = mTransitions.Length;
			State previousState = controller.CurrentState;

			for (int i = 0; i < length; i++)
			{
				bool decisionSucceded = mTransitions[i].Decision.Decide(controller);

				if (decisionSucceded)
				{
					controller.TransitionToState(mTransitions[i].TrueState);
				}
				else
				{
					controller.TransitionToState(mTransitions[i].FalseState);
				}

				// 현재 상태가 다른 상태로 바뀌었다면 반복문 중단
				if (!previousState.Equals(controller.CurrentState))
				{
					int performancesCount = controller.CurrentState.mPerformances.Length;
					for (int count = 0; count < performancesCount; count++)
					{
						controller.CurrentState.mPerformances[count].OnInitialize(controller);
					}

					Ulog.Log($"Change {previousState} To {controller.CurrentState}");

					break;
				}
			}
		}

		public override string ToString()
		{
			return name;
		}
	}
}
