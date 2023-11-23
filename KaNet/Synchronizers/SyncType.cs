namespace KaNet.Synchronizers
{
	public enum SyncType : byte
	{
		None = 0,

		ReliableFixed = 10,
		ReliableInstant = 11,

		UnreliableFixed = 20,
		UnreliableInstant = 21,
	}

	public static class SyncTypeExtension
	{
		public static bool IsReliable(this SyncType syncType)
		{
			return syncType == SyncType.ReliableFixed || syncType == SyncType.ReliableInstant;
		}

		public static bool IsUnreliable(this SyncType syncType)
		{
			return syncType == SyncType.UnreliableFixed || syncType == SyncType.UnreliableInstant;
		}
	}
}
