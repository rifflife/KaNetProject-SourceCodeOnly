using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using Utils;


namespace PluggableAI
{
	public abstract class StateController : MonoBehaviour
	{
		[Title("초기 AI 상태")]
		[field: SerializeField] public State InitialCurrentState { get; private set; }
		[field: SerializeField] public State InitialRemainState { get; private set; }

		public State CurrentState { get; private set; }
		public State RemainState { get; private set; }

		public float StateTimeElapsed { get; private set; }
		public bool IsAiActive { get; private set; } = false;

		public void Initialize()
		{
			CurrentState = InitialCurrentState;
			RemainState = InitialRemainState;

			IsAiActive = true;
			StateTimeElapsed = 0;
		}

		/// <summary>현재 상태를 다른 상태로 변경</summary>
		public void TransitionToState(State nextState)
		{
			if (nextState.Equals(RemainState)) // 현재 상태 유지
			{
				return;
			}

			CurrentState = nextState;
		}

		/// <summary>상태 지속 시간이 duration을 넘었는지 확인</summary>
		public bool CheckIfCountDownElapsed(float duration)
		{
			return StateTimeElapsed >= duration;
		}

		protected virtual void UpdateAI(float deltaTime)
		{
			if (!IsAiActive)
			{
				StateTimeElapsed = 0;
				return;
			}

			StateTimeElapsed += deltaTime;
			CurrentState.UpdateState(this);
		}
	}
}
