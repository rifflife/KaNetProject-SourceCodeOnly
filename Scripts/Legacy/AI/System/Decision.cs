using UnityEngine;
using PluggableAI;

namespace PluggableAI
{
	/// <summary> �ٸ� ���·� ��ȯ�ϴ� ������ �����ϴ��� Ȯ���ϴ� �Լ��� ������ �߻� Ŭ���� �� ScriptableObject�Դϴ�.</summary>  
	public abstract class Decision : ScriptableObject
	{
		public abstract bool Decide(StateController controller);
	}

}