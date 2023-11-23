using UnityEngine;
using PluggableAI;

namespace PluggableAI
{
	/// <summary> 다른 상태로 전환하는 조건을 충족하는지 확인하는 함수를 가지는 추상 클래스 및 ScriptableObject입니다.</summary>  
	public abstract class Decision : ScriptableObject
	{
		public abstract bool Decide(StateController controller);
	}

}