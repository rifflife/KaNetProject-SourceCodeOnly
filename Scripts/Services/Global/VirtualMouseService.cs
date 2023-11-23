using Gameplay;
using MonoGUI;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using Utils.Service;

public class VirtualMouseService : MonoService
{
	private Dictionary<AimType, View_Mouse> mAimTable = new();

	[SerializeField] private Navigation_Mouse mNavigation;
	private View_Mouse mCurrentMouse;

	public override void OnRegistered()
	{
		base.OnRegistered();

		Ulog.Log(this, "Virtual Mouse Registered");

		// Initialize mouse table
		View_MouseNormal mouseNormal = mNavigation.CreateMouseView<View_MouseNormal>();
		mAimTable.Add(AimType.Arrow, mouseNormal);

		View_MousePistol mousePistor = mNavigation.CreateMouseView<View_MousePistol>();
		mousePistor.Initiailzed();
		mAimTable.Add(AimType.Aim_Pistol, mousePistor);

		View_MouseShotgun mouseShotGun = mNavigation.CreateMouseView<View_MouseShotgun>();
		mouseShotGun.Initiailzed();
		mAimTable.Add(AimType.Aim_Shotgun, mouseShotGun);

		// Setup initial virtual mouse
		ChangeAim(AimType.Arrow);
		//Cursor.visible = false;
	}

	public override void OnUnregistered()
	{
		base.OnUnregistered();
	}

	private void Update()
	{
#if UNITY_EDITOR
		Cursor.visible = true;
#else
		Cursor.visible = false;
#endif
	}

	public void ChangeAim(AimType type)
	{
		mNavigation.ChangeMouse(type);
		mCurrentMouse = mAimTable[type];
		mCurrentMouse.MoveToRealMousePoint();
	}

	/// <summary> �ش� Ŀ���� Ŭ�� ������ �����մϴ�.</summary>
	public void OnClick()
	{
		mCurrentMouse.OnClickAction();
	}

	/// <summary> �ش� Ŀ���� ������ ������ �����մϴ�.</summary>
	public void OnReLoading()
	{
		mCurrentMouse.OnReload();
	}

	/// <summary>
	/// ���� Ŀ���� �ѱ� �ݵ��� GUI�� ǥ���մϴ�.
	/// ���� ���� 0.0f ~ 1.0f ���� ���� �����Ե˴ϴ�.
	/// </summary>
	/// <param name="recoilPercent">�߰��� �ݵ� �ۼ�Ʈ</param>
	public void ApplyRecoil(float recoilPercent)
	{
		mCurrentMouse.ApplyRecoil(recoilPercent);
	}

	/// <summary> ���� ���콺 ��ġ���� Ray�� �߻��մϴ�.</summary>
	public Ray PointToRay()
	{
		return Camera.main.ScreenPointToRay(mCurrentMouse.GetScreenPosition()); ;
	}

	/// <summary> ���� ���콺�� ���� ��ġ���� �����ɴϴ�.</summary>
	public Vector2 GetWorldPoint()
	{
		return Camera.main.ScreenToWorldPoint(mCurrentMouse.GetScreenPosition());
	}

	/// <summary>���콺 ����Ʈ�� ������ Collider���� �����ɴϴ�.</summary>
	public Collider2D[] GetOverlapColliders()
	{
		return Physics2D.OverlapPointAll(GetWorldPoint());
	}

	/// <summary>���콺 ����Ʈ�� ������ Collider���� �����ɴϴ�.</summary>
	public Collider2D[] GetOverlapColliders(int layerMask)
	{
		return Physics2D.OverlapPointAll(GetWorldPoint(), layerMask);
	}

	/// <summary>���콺 ����Ʈ�� ������ Collider�� �����ɴϴ�.</summary>
	public Collider2D GetOverlapCollider()
	{
		return Physics2D.OverlapPoint(GetWorldPoint());
	}

	/// <summary>���콺 ����Ʈ�� ������ Collider�� �����ɴϴ�.</summary>
	public Collider2D GetOverlapCollider(int layerMask)
	{
		return Physics2D.OverlapPoint(GetWorldPoint(), layerMask);
	}
}
