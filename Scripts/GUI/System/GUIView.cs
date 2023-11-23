using UnityEngine;
using System.Collections.Generic;
using Utils;
using Sirenix.OdinInspector;
using System;

public enum VisableState
{
	None,
	Appearing,
	Appeared,
	Disappearing,
	Disappered
}

public abstract class GUIView : MonoBehaviour
{
	protected GUIViewSequenceGenerator mSequenceGenerator { private set; get; } = null;
	protected List<GUIPopupView> mPopupViewList;

	private RectTransform mViewRectTransfrom;
	public RectTransform ViewRectTransfrom 
	{
		private set
		{
			mViewRectTransfrom = value;
		}
		get
		{
			if (mViewRectTransfrom == null)
				mViewRectTransfrom = GetComponent<RectTransform>();

			return mViewRectTransfrom;
		}
	}
	public VisableState State { get; set; }

	public bool IsShow => State == VisableState.Appeared || State == VisableState.Appearing;
	public bool IsPlaying => State == VisableState.Appearing || State == VisableState.Disappearing;

	protected virtual void viewSetUp()
	{
		if (TryGetComponent<GUIViewSequenceGenerator>(out var generator))
		{
			generator.Initialize(this);
			mSequenceGenerator = generator;
		}

		Close();
	}

	/// <summary> �ִϸ��̼��� �����鼭 View�� �������ϴ�. </summary>
	public virtual void Show(Action callback = null)
	{
		if (mSequenceGenerator == null || !mSequenceGenerator.IsShowAnimatnioAvailable)
			Open(callback);
		else
			mSequenceGenerator.PlayShow(callback);
	}

	/// <summary> �ִϸ��̼��� �����鼭 View�� ������ϴ�. </summary>
	public virtual void Hide(Action callback = null)
	{
		if (mSequenceGenerator == null || !mSequenceGenerator.IsHideAnimationAvailable)
			Close(callback);
		else
			mSequenceGenerator.PlayHide(callback);
	}

	/// <summary> �ִϸ��̼� ���� View�� �������ϴ�.</summary>
	public void Open(Action callback = null)
	{
		callback?.Invoke();
		gameObject.SetActive(true);
		State = VisableState.Appeared;
	}

	/// <summary> �ִϸ��̼� ���� View�� ������ϴ�. </summary>
	public void Close(Action callback = null)
	{
		callback?.Invoke();
		gameObject.SetActive(false);
		State = VisableState.Disappered;
	}

}
