using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using KaNet.Synchronizers;
using UnityEngine;
using UnityEngine.Networking.PlayerConnection;
using Utils;

namespace NetworkAI
{
	//public interface IStateGroupable<T>
	//{
	//	public void OnStart(T controller, DeltaTimeInfo deltaTimeInfo);
	//	public void OnUpdate(T controller, DeltaTimeInfo deltaTimeInfo);
	//}

	/// <summary>�ൿ�� Action�� ��ȯ ������ ���� �׷��Դϴ�.</summary>
	public class StateGroup : MonoBehaviour//, IStateGroupable<T>
		//where T : IStateControllable
	{
		[SerializeField] private StateAction mAction; // ���� ���¿��� �����ϴ� �ൿ
		[SerializeField] private StateTransition[] mTransitions; // ���� ���¿��� ��ȯ�� �� �ִ� ���¿� ����

		/// <summary>���� State�� ������ ����ð��Դϴ�.</summary>
		public float StateElapsedSec { get; private set; }

		/// <summary>������Ʈ�� ������Ʈ�մϴ�.</summary>
		/// <param name="controller"></param>
		/// <param name="deltaTimeInfo"></param>
		public void OnUpdate(StateController controller, DeltaTimeInfo deltaTimeInfo)
		{
			OnAct(controller, deltaTimeInfo);

			if (!HasLock())
			{
				if (TryGetNextStateByConditions(controller, deltaTimeInfo, out var nextState))
				{
					// ��ȯ ������ �����ϴ� State�� �ִٸ� ��ȯ�մϴ�.
					controller.ChangeState(deltaTimeInfo, nextState);
					return;
				}
			}
		}

		/// <summary>��ȯ ������ �����ϴ� ���� State�� ��ȯ�մϴ�.</summary>
		/// <param name="nextState">��ȯ�� ������ ���� State�Դϴ�.</param>
		/// <returns>��ȯ�Ǿ����� true�� ��ȯ�մϴ�.</returns>
		private bool TryGetNextStateByConditions(StateController controller, DeltaTimeInfo deltaTimeInfo, out StateGroup nextState)
		{
			//var previousState = controller.CurrentStateGroup;
			var previousState = controller.CurrentStateGroup;

			foreach (var transition in mTransitions)
			{
				bool checkCondition = transition.Condition.CheckCondition(controller, deltaTimeInfo);

				// Check conditions next state
				nextState = checkCondition ? transition.TrueState : transition.FalseState;

				// Check it's current state
				if (controller.CurrentStateGroup.Equals(nextState))
				{
					continue;
				}

				//Ulog.Log($"Change {previousState} To {controller.CurrentStateGroup}");
				return true;
			}

			nextState = null;
			return false;
		}

		public bool HasLock()
		{
			return mAction.IsLock;
		}

		public void OnStart(StateController controller, DeltaTimeInfo deltaTimeInfo)
		{
			StateElapsedSec = 0.0F;
			mAction.OnStart(controller, deltaTimeInfo);
		}

		public void OnAct(StateController controller, DeltaTimeInfo deltaTimeInfo)
		{
			StateElapsedSec += deltaTimeInfo.ScaledDeltaTime;

			mAction.OnAct(controller, deltaTimeInfo);
		}

		public void OnEnd(StateController controller, DeltaTimeInfo deltaTimeInfo)
		{
			StateElapsedSec = 0.0F;
			mAction.OnEnd(controller, deltaTimeInfo);
		}

		public override string ToString() => name;
	}
}
