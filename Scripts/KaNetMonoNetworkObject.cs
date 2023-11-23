using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

using KaNet.Synchronizers;
using KaNet.Synchronizers.Prebinder;

using Utils;

public class KaNetMonoNetworkObject : NetworkObject
{
	public void Start()
	{
		NetworkObjectPrebinder.PrebindByReflection(this.GetType(), this);
	}

	[ShowInInspector]
	public override NetObjectType Type => NetObjectType.None;

	//public override NetworkObjectType Type => throw new System.NotImplementedException();

	[SyncVar]
	public readonly SyncField<NetInt32> SomeValue = new(12345);

	[RpcCall]
	public readonly RpcCaller<NetInt32, NetBool> RPC_DoSomething = new();
	public void DoSomething(NetInt32 arg1, NetBool arg2)
	{
		int v = arg1 + 10;
		bool b = arg2;
	}

	public void SomeAction()
	{
		RPC_DoSomething.Invoke(500, false);
	}

	#region Server Side

	/// <summary>���������� �����Ǿ��� �� ȣ��˴ϴ�. Start�Լ��� �����մϴ�.</summary>
	public override void Server_OnStart()
	{

	}

	/// <summary>���������� ���ŵǾ��� �� ȣ��˴ϴ�. OnDestroy�Լ��� �����մϴ�.</summary>
	public override void Server_OnDestroy()
	{

	}

	/// <summary>���������� Update�� �� ȣ��˴ϴ�. Update�Լ��� �����մϴ�.</summary>
	public override void Server_OnUpdate(in DeltaTimeInfo deltaTimeInfo)
	{

	}

	/// <summary>���������� FixedUpdate�� �� ȣ��˴ϴ�. FixedUpdate�Լ��� �����մϴ�.</summary>
	public override void Server_OnFixedUpdate(in DeltaTimeInfo deltaTimeInfo)
	{

	}

	#endregion

	#region Client Side

	/// <summary>Ŭ���̾�Ʈ������ �����Ǿ��� �� ȣ��˴ϴ�. Start�Լ��� �����մϴ�.</summary>
	public override void Client_OnStart()
	{

	}

	/// <summary>Ŭ���̾�Ʈ������ ���ŵǾ��� �� ȣ��˴ϴ�. OnDestroy�Լ��� �����մϴ�.</summary>
	public override void Client_OnDestroy()
	{

	}

	/// <summary>Ŭ���̾�Ʈ������ Update�� �� ȣ��˴ϴ�. Update�Լ��� �����մϴ�.</summary>
	public override void Client_OnUpdate(in DeltaTimeInfo deltaTimeInfo)
	{

	}

	/// <summary>Ŭ���̾�Ʈ������ FixedUpdate�� �� ȣ��˴ϴ�. FixedUpdate�Լ��� �����մϴ�.</summary>
	public override void Client_OnFixedUpdate(in DeltaTimeInfo deltaTimeInfo)
	{

	}

	#endregion
}
