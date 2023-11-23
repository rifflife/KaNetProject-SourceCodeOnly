using System;
using UnityEngine;
using Utils;


namespace PluggableAI
{
	[Serializable]
	public class StateTransition
	{
		public Decision Decision; // �ٸ� ���·� ��ȯ�Ǵ� ����
		public State TrueState; // ������ true�϶� ��ȯ�Ǵ� ����
		public State FalseState; // ������ false�϶� ��ȯ�Ǵ� ����
	}

	[CreateAssetMenu(menuName = "PluggableAI/State")]
	public class State : ScriptableObject
	{
		[SerializeField]
		private StateAction[] mPerformances; // ���� ���¿��� �����ϴ� �ൿ
		[SerializeField]
		private StateTransition[] mTransitions; // ���� ���¿��� ��ȯ�� �� �ִ� ���¿� ����
		[SerializeField]
		private StateAction[] mEndPerformances; // ���� ���°� ���� �� �����ϴ� �ൿ

		public void UpdateState(StateController controller)
		{
			doPerformance(controller);
			checkTransitions(controller);
		}

		/// <summary> ���� ���¿��� ����Ǵ� �Լ��Դϴ�. </summary>
		private void doPerformance(StateController controller)
		{
			int length = mPerformances.Length;

			for (int i = 0; i < length; i++)
			{
				mPerformances[i].Act(controller);
			}
		}

		/// <summary> ���ǿ� ���� �ٸ� ���·� ��ȯ�ϴ� �Լ��Դϴ�. </summary>
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

				// ���� ���°� �ٸ� ���·� �ٲ���ٸ� �ݺ��� �ߴ�
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
