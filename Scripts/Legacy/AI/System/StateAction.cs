using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PluggableAI;

namespace PluggableAI
{
	/// <summary>  ���� ���¿��� �Ǵ� �ٸ� ���·� ��ȯ�� �� ����Ǵ� �Լ��� ������ �߻� Ŭ���� �� ScriptableObject�Դϴ�.</summary>  
	public abstract class StateAction : ScriptableObject
	{
		public abstract void OnInitialize(StateController controller);
		public abstract void Act(StateController controller);
	}
}
