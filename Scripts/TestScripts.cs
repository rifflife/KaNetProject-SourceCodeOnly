using Sirenix.OdinInspector;
using UnityEngine;

public class TestScripts : MonoBehaviour
{
#if UNITY_EDITOR
    [Button]
    public void PrintSomething()
    {
        Debug.Log("��ȣ");
    }

    [Button("��ȣ ���")]
    public void PrintSomething_2()
    {
        Debug.Log("��ȣ");
    }
#endif
}
