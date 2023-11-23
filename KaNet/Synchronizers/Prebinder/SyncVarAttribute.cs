using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaNet.Synchronizers.Prebinder
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Field)]
	public class SyncVarAttribute : Attribute
	{
		public SyncType Type { get; private set; }
		public SyncAuthority Authority { get; private set; }

		public SyncVarAttribute
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
