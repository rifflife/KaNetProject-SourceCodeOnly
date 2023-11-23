using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PluggableAI;

namespace PluggableAI
{
	/// <summary>  현재 상태에서 또는 다른 상태로 전환될 때 실행되는 함수를 가지는 추상 클래스 및 ScriptableObject입니다.</summary>  
	public abstract class StateAction : ScriptableObject
	{
		public abstract void OnInitialize(StateController controller);
		public abstract void Act(StateController controller);
	}
}
