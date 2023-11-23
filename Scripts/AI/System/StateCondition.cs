using KaNet.Synchronizers;
using UnityEngine;

namespace NetworkAI
{
	/// <summary> 다른 상태로 전환하는 조건을 충족하는지 확인하는 함수를 가지는 추상 클래스 및 ScriptableObject입니다.</summary>  
	public abstract class StateCondition : MonoBehaviour// where T : IStateControllable
	{
		public abstract bool CheckCondition(StateController controller, DeltaTimeInfo deltaTimeInfo);
	}
}
