using System.Collections.Generic;

namespace KaNet.Synchronizers
{
	public class SessionTessllationInfo
	{
		// 동기화 범위의 절반 입니다.
		public const int SyncRangeX = 2;
		public const int SyncRangeY = 2;
		public const int SyncRangeZ = 1;

		public NetSessionID SessionID { get; private set; }
		private List<NetworkObject> mSyncObjectList = new();
		public TessellateCoord SessionPivotCoord { get; private set; }

		public List<NetLifeStreamToken> mLifeStream = new();

		public List<NetworkObject> GetSyncObjectList()
		{
			return mSyncObjectList;
		}

		public SessionTessllationInfo(NetSessionID sessionID)
		{
			SessionID = sessionID;
		}

		public void UpdateCoord(TessellateCoord coord)
		{
			SessionPivotCoord = coord;
		}

		public void OnUpdate()
		{

		}

		public void OnNetworkTick()
		{

		}
	}
}
