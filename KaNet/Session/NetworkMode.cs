namespace KaNet.Session
{
	public enum NetworkMode
	{
		None = 0,

		/// <summary>싱글 플레이 모드입니다. 모든 패킷은 곧바로 로컬 클라이언트로 에코됩니다.</summary>
		Singleplayer = 1,

		/// <summary>리슨 서버 모드입니다.</summary>
		Server_Listen = 10,

		/// <summary>데디케이트 서버 모드입니다.</summary>
		Server_Dedicated = 11,

		/// <summary>클라이언트 모드입니다.</summary>
		Client = 20,
	}

	public static class NetworkModeExtensions
	{
		public static bool IsServer(this NetworkMode mode)
		{
			return mode == NetworkMode.Singleplayer || 
				mode == NetworkMode.Server_Listen || 
				mode == NetworkMode.Server_Dedicated;
		}

		public static bool IsClient(this NetworkMode mode)
		{
			return mode == NetworkMode.Singleplayer ||
				mode == NetworkMode.Server_Listen || 
				mode == NetworkMode.Client;
		}
	}
}