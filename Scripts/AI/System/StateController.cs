using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;

using UnityEngine;

using Sirenix.OdinInspector;
using KaNet.Synchronizers;
using Utils;

namespace NetworkAI
{
	/// <summary>FSM의 상태 컨트롤러입니다.</summary>
	public abstract class StateController : MonoBehaviour
	{
		[field: SerializeField, Header("초기 상태")]
		public StateGroup InitialCurrentStateGroup { get; protected set; }

		[field: SerializeField, Header("현재 상태")]
		public StateGroup CurrentStateGroup { get; protected set; }
		
		[MinMaxSlider(0F, 0.5F), Header("AI 업데이트 주기")]
		public Vector2 UpdateIntervalRange = new Vector2(0.125F, 0.25F);

		public virtual void OnValidate()
		{
			UpdateIntervalRange = KMath.SnapBy(UpdateIntervalRange, 1.0F / 32);
		}

		private float mUpdateDelay;
		private float mUpdateInterval;

		public virtual void OnInitialize(DeltaTimeInfo deltaTimeInfo)
		{
			// Initialize state
			CurrentStateGroup = InitialCurrentStateGroup;

			// Initial start
			CurrentStateGroup.OnStart(this, deltaTimeInfo);
		}

		public virtual void OnUpdate(DeltaTimeInfo deltaTimeInfo)
		{
			if (mUpdateDelay < mUpdateInterval)
			{
				mUpdateDelay += deltaTimeInfo.ScaledDeltaTime;
				return;
			}

			DeltaTimeInfo stateDeltaTimeInfo= new DeltaTimeInfo
			(
				mUpdateDelay,
				deltaTimeInfo.GlobalTimeScale
			);

			CurrentStateGroup.OnUpdate(this, stateDeltaTimeInfo);

			mUpdateDelay = 0;
			mUpdateInterval = UpdateIntervalRange.GetRandomFromMinMax();
		}

		public void ChangeState(DeltaTimeInfo deltaTimeInfo, StateGroup stateGroup)
		{
			CurrentStateGroup?.OnEnd(this, deltaTimeInfo);
			CurrentStateGroup = stateGroup;
			CurrentStateGroup.OnStart(this, deltaTimeInfo);
		}
	}
}
