using System;
using System.Collections.Generic;
using KaNet.Session;
using KaNet.Synchronizers;

namespace KaNet.Utils
{
	public class PacketGroup
	{
		private List<NetPacket> mPacketList;
		public Func<NetPacket> GetterAction { get; private set; }
		public Action<NetPacket> ReleaserAction { get; private set; }
		private Action<NetSessionID, NetPacket> mSendAction;

		private List<NetSessionID> mSendingList = new();

		public PacketGroup(SyncType syncType, Action<NetSessionID, NetPacket> sendAction)
		{
			mPacketList = new();
			mSendAction = sendAction;

			if (syncType.IsReliable())
			{
				GetterAction = PacketPool.GetStreamPacket;
				ReleaserAction = PacketPool.ReturnStreamPacket;
			}
			else if (syncType.IsUnreliable())
			{
				GetterAction = PacketPool.GetMtuPacket;
				ReleaserAction = PacketPool.ReturnMtuPacket;
			}
		}

		public void AddSendTo(NetSessionID sendTo)
		{
			mSendingList.Add(sendTo);
		}

		public void AddPacket(IList<NetPacket> packets)
		{
			mPacketList.AddRange(packets);
		}

		public void Release()
		{
			for (int i = 0; i < mPacketList.Count; i++)
			{
				ReleaserAction(mPacketList[i]);
			}

			mPacketList.Clear();
		}

		public void SendAndRelease()
		{
			foreach (var s in mSendingList)
			{
				for (int i = 0; i < mPacketList.Count; i++)
				{
					mSendAction(s, mPacketList[i]);
				}
			}

			Release();
		}

		//public void SendBySessionInfoAndRelease
		//(
		//	NetSessionInfo netSesionInfo,
		//	Action<NetSessionInfo, NetPacket> sendAction
		//)
		//{
		//	for (int i = 0; i < mPacketList.Count; i++)
		//	{
		//		sendAction(netSesionInfo, mPacketList[i]);
		//	}

		//	release();
		//}
	}
}
