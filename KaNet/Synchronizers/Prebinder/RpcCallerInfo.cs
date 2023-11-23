using System;

namespace KaNet.Synchronizers.Prebinder
{
	public class RpcCallerInfo
	{
		public string RpcMethodName => mRpcMethodName;

		private Func<object, object> mRpcInstanceGetter;
		private string mRpcMethodName;
		private SyncType mSyncType;
		private SyncAuthority mSyncAuthority;

		public RpcCallerInfo
		(
			Func<object, object> getterAction,
			string methodName,
			SyncType syncType,
			SyncAuthority syncAuthority
		)
		{
			mRpcInstanceGetter = getterAction;
			mRpcMethodName = methodName;
			mSyncType = syncType;
			mSyncAuthority = syncAuthority;
		}

		public RpcBase GetRpcCaller(object instance, object rpcAction)
		{
			var rpcCaller = mRpcInstanceGetter.Invoke(instance) as RpcBase;
			rpcCaller.SetNetworkOption(mSyncType, mSyncAuthority);
			rpcCaller.BindFunction(rpcAction);
			return rpcCaller;
		}

		public override string ToString()
		{
			return mRpcMethodName;
		}
	}
}
