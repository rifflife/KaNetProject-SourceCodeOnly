using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Utils;

using KaNet;
using KaNet.Synchronizers;
using KaNet.Synchronizers.Prebinder;
using KaNet.Utils;

using Sirenix.OdinInspector;
using Utils.ViewModel;

public class Entity_Remote : EntityBase
{
	// Synchronize Position
	[SyncVar(SyncType.UnreliableFixed, SyncAuthority.ServerOnly)]
	private readonly SyncFieldByOrder<NetVector3> mPositionByServer = new();

	public override NetObjectType Type => NetObjectType.Entity_Remote;

	public override void Common_OnStart()
	{
		mPositionByServer.ResetTimestamp();
		mPositionByServer.ResetDeserializeEvent();

		// Ŭ���̾�Ʈ�� �ڱ� �ڽ��� ������ �� ���� ��ü�� Rigidbody�� �и��� �ȵȴ�.
		EntityRigid.bodyType = IsServerSide ? RigidbodyType2D.Dynamic : RigidbodyType2D.Kinematic;
	}

	public override void Server_OnStart()
	{
		base.Server_OnStart();
		mPositionByServer.Data = transform.position;
	}

	public override void Client_OnStart()
	{
		base.Client_OnStart();
		transform.position = mPositionByServer.Data;
	}

	public override void Client_OnUpdate(in DeltaTimeInfo deltaTimeInfo)
	{
		if (IsServerSide)
		{
			mPositionByServer.Data = transform.position;
		}
		else
		{
			transform.position = Vector3.Lerp
			(
				transform.position,
				mPositionByServer.Data,
				KaNetGlobal.NETWORK_INTERPOLATION_VALUE * deltaTimeInfo.DeltaTime
			);
		}
	}
}
