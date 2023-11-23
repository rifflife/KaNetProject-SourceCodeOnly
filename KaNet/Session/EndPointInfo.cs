using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Steamworks.Data;

namespace KaNet.Session
{
	public struct EndPointInfo
	{
		//public EndPoint TargetAddress;
		public Lobby TargetLobby;

		public EndPointInfo(Lobby targetLobby)
		{
			TargetLobby = targetLobby;
		}

		public NetSessionInfo GetServerSessionInfo()
		{
			return new NetSessionInfo(TargetLobby.Owner);
		}
	}
}
