using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonoGUI;
using Utils;
using Utils.ViewModel;
using System;

public enum MessageType : byte
{
	Normal,
	//�߰��� �� �ִ°�
}

public class View_SystemMessage : MonoGUI_View
{
	[SerializeField] private TextMeshProTextViewModel Text_Message = new(nameof(Text_Message));
	private IEnumerator mDuration;

	public override void OnInitialized()
	{
		Ulog.Log($"[HUD] Draw SystemMessage");
		Text_Message.Initialize(this);
	}

	/// <summary> �ý��� �޼����� ����մϴ�. </summary>
	/// <param name="type">��� ȿ�� Ÿ��</param>
	/// <param name="msg">�޼���</param>
	/// <param name="time">�����ð��� Show����� Hide������ ������ �ð��Դϴ�.</param>
	public void Message(MessageType type, string msg, float time)
	{
		if (mDuration != null)
		{
			StopCoroutine(mDuration);
		}

		if (type == MessageType.Normal)
		{
			Text_Message.Text = msg;
			Show();
		}

		mDuration = duration(time);
		StartCoroutine(mDuration);
	}

	private IEnumerator duration(float t)
	{
		//���̴� ������ ������ ���� ��� �մϴ�.
		while(CurrentState == GuiState.Showing)
		{
			yield return null;
		}	
		//���̴� ���� ���ĺ��� ���ӽð���ŭ ����մϴ�.
		yield return new WaitForSeconds(t);
		Hide();
	}

	public void Close()
	{
		if (mDuration != null)
		{
			StopCoroutine(mDuration);
			mDuration = null;
		}

		ShowSkip();
		Hide();
		HideSkip();
	}

}
