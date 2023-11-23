using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NewVisableState
{
	None,
	/// <summary> �������� �ִϸ��̼� ���� ��</summary>
	Appearing,
	/// <summary> �������� �ִϸ��̼� �Ϸ�</summary>
	Appeared,
	/// <summary> ������� �ִϸ��̼� ���� ��</summary>
	Disappearing,
	/// <summary> ������� �ִϸ��̼� �Ϸ�</summary>
	Disappered
}

public class NewGUIView : MonoBehaviour, IGUIVisable
{
	private VisableState mState;
	public VisableState State { get => mState; }

	private NewGUIViewSequenceGenerator mSequenceGenerator = null;

	public bool IsShow => State == VisableState.Appeared || State == VisableState.Appearing;

	public bool IsAnimation => mSequenceGenerator != null;


	public void SetVisableStae(VisableState state)
	{
		mState = state;
	}

	/// <summary> �ش� GUI�� �ִϸ��̼��� �����Ѵ�. </summary>
	/// <param name="sequenceGerator">������ �ִϸ��̼ǿ� ������</param>
	protected void SetAnimatnio(NewGUIViewSequenceGenerator sequenceGerator)
	{
		mSequenceGenerator = sequenceGerator;
		mSequenceGenerator.Initialize(this);
	}

	/// <summary> GUI ��Ȱ��ȭ �Ѵ�. </summary>
	/// <param name="startCallback">Hide �ִϸ��̼� ���� �� ȣ��Ǵ� Action</param>
	/// <param name="endCallback">Hide �ִϸ��̼� ������ ȣ��Ǵ� Action</param>
	public void Hide(Action callback = null)
	{
		if (!IsShow)
			return;

		if(!IsAnimation)
		{
			callback?.Invoke();
			gameObject.SetActive(false);
		}

		mSequenceGenerator.Hide(callback);
	}

	/// <summary> GUI Ȱ��ȭ �Ѵ�. </summary>
	/// <param name="startCallback">Show �ִϸ��̼� ���� �� ȣ��Ǵ� Action</param>
	/// <param name="endCallback">Show �ִϸ��̼� ������ ȣ��Ǵ� Action</param>
	public void Show(Action callback = null)
	{
		if (IsShow)
			return;

		if (!IsAnimation)
		{
			callback?.Invoke();
			gameObject.SetActive(false);
		}
		mSequenceGenerator.Show(callback);
	}

	/// <summary> �ܼ� ������Ʈ Ȱ��ȭ Ȥ�� ��Ȱ��ȭ ��� </summary>
	public void SetActive(bool isActive)
	{
		VisableState state = VisableState.Appeared;
		if (!isActive)
			state = VisableState.Disappered;

		gameObject.SetActive(isActive);
		SetVisableStae(state);
	}
}
