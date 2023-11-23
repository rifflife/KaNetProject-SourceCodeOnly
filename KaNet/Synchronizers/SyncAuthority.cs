namespace KaNet.Synchronizers
{
	public enum SyncAuthority : byte
	{
		None = 0,

		/// <summary>
		/// 서버만 객체를 직렬화 할 수 있습니다.
		/// 클라이언트는 객체가 변화해도 직렬화하지 않습니다.
		/// </summary>
		ServerOnly = 1,
		/// <summary>
		/// 소유자만 객체를 직렬화 할 수 있습니다.
		/// 서버로 동기화됩니다.
		/// 소유자가 아닌 경우 객체가 변화해도 직렬화하지 않습니다.
		/// </summary>
		OwnerToServer = 2,
		/// <summary>
		/// 모든 클라이언트에게 동기화합니다.
		/// </summary>
		OwnerBroadcast = 3,
	}
}
