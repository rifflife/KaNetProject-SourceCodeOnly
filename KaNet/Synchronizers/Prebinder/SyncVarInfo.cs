using System;

namespace KaNet.Synchronizers.Prebinder
{
	public class SyncVarInfo
	{
		public string FieldName => mFieldName;
		private Func<object, object> mVarInstanceGetter;
		private string mFieldName;
		private SyncType mSyncType;
		private SyncAuthority mSyncAuthority;

		public SyncVarInfo
		(
			Func<object, object> getterAction,
			string fieldName,
			SyncType syncType,
			SyncAuthority syncAuthority
		)
		{
			mVarInstanceGetter = getterAction;
			mFieldName = fieldName;
			mSyncType = syncType;
			mSyncAuthority = syncAuthority;
		}

		public Synchronizer GetSyncVar(object instance)
		{
			var syncVar = mVarInstanceGetter.Invoke(instance) as Synchronizer;
			syncVar.SetNetworkOption(mSyncType, mSyncAuthority);
			return syncVar;
		}

		public override string ToString()
		{
			return mFieldName;
		}
	}
}
