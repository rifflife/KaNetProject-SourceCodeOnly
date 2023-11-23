using System;

namespace KaNet.Synchronizers.Prebinder
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Field)]
	public class RpcCallAttribute : Attribute
	{
		public SyncType Type { get; private set; }
		public SyncAuthority Authority { get; private set; }

		public RpcCallAttribute
		(
			SyncType syncType = SyncType.ReliableFixed,
			SyncAuthority Authority = SyncAuthority.ServerOnly
		)
		{
			Type = syncType;
			this.Authority = Authority;
		}
	}
}
