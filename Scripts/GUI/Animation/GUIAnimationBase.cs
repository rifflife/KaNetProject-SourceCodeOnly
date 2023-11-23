using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public abstract class GUIAnimationBase : MonoBehaviour
{
	protected List<Action> SequenceStartActionList { get; private set; } = new();
	protected List<Action> SequenceCompleteActionList { get; private set; } = new();
	protected List<Action> TweenStartActionList { get; private set; } = new();
	protected List<Action> TweenCompleteActionList { get; private set; } = new();

	[field: Title("General Option")]
	[field: SerializeField]
	public string ID { private set; get; } = "";

	[Range(0.0f, 10.0f)]
	[Tooltip("�ִϸ��̼� �ҿ� �ð��Դϴ�.")]
	[SerializeField]
	protected float Duration;

	[field: Range(0.0f, 10.0f)]
	[field: SerializeField]
	protected float Delay { private set; get; }

	[field: Title("Loop")]

	[field: SerializeField]
	protected LoopType loopType { private set; get; } = LoopType.Restart;

	[field: SerializeField]
	[field: PropertyTooltip("Loop Count�� 0 �̻����� �����ؾߵ˴ϴ�.")]
	protected int loopCount { private set; get; } = 1;

	protected bool IsAvailable { set; get; } = false;

	public virtual void Initilize()
	{
		if (loopCount <= 0)
			Ulog.LogWarning(UlogType.UI, $"The animation will not run because it is set to 0 or less. : ID {ID}, GameObject {gameObject.name}");
	}
	public abstract Tween GetTween();

	#region SequenceStart

	/// <summary> ��ü �ִϸ��̼� ������ �� ȣ���մϴ�. </summary>
	public virtual void OnSequenceStart()
	{
		foreach (Action action in SequenceStartActionList)
			action();
	}
	public void AddOnSequenceStart(Action action)
	{
		SequenceStartActionList.Add(action);
	}

	public void RemoveOnSequenceStart(Action action)
	{
		SequenceStartActionList.Remove(action);
	}

	#endregion

	#region SequenceComplete

	/// <summary> ��ü �ִϸ��̼� ���� �� ȣ���մϴ�. </summary>
	public virtual void OnSequenceComplete()
	{
		foreach (Action action in SequenceCompleteActionList)
			action();
	}
	public void AddOnSeqenceComplete(Action action)
	{
		SequenceCompleteActionList.Add(action);
	}

	public void RemoveSeqenceComplete(Action action)
	{
		SequenceCompleteActionList.Remove(action);
	}

	#endregion

	#region TweenStart

	/// <summary> �ش� �ִϸ��̼��� ������ �� ȣ���մϴ�. </summary>
	public virtual void OnTweenStart()
	{
		foreach (Action action in TweenStartActionList)
			action();
	}

	public void AddOnTweenStart(Action action)
	{
		TweenStartActionList.Add(action);
	}

	public void RemoveOnTweenStart(Action action)
	{
		TweenStartActionList.Remove(action);
	}

	#endregion

	#region TweenComplete

	/// <summary> �ش� �ִϸ��̼��� ���� �� ȣ���մϴ�. </summary>
	public virtual void OnTweenComplete()
	{
		foreach (Action action in TweenCompleteActionList)
			action();
	}
	public void AddTweenComplete(Action action)
	{
		TweenCompleteActionList.Add(action);
	}
	public bool RemoveTweenComplete(Action action)
	{
		return TweenCompleteActionList.Remove(action);
	}

	#endregion
}
