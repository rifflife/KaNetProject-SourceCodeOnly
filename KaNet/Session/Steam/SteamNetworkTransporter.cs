using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KaNet.Utils;
using Steamworks;
using Utils;
using Utils.Analytics;
using Utils.Service;

namespace KaNet.Session
{
	public class TransmissionAnalytics
	{
		private NumericAccumulator mSendReliable = new(nameof(mSendReliable));
		private NumericAccumulator mSendReliableCount = new(nameof(mSendReliableCount));

		private NumericAccumulator mSendUnreliable = new(nameof(mSendUnreliable));
		private NumericAccumulator mSendUnreliableCount = new(nameof(mSendUnreliableCount));

		private NumericAccumulator mReceive = new(nameof(mReceive));
		private NumericAccumulator mReceiveCount = new(nameof(mReceiveCount));

		public void Reset()
		{
			mSendReliable.Reset();
			mSendUnreliable.Reset();
			mSendUnreliableCount.Reset();
			mReceive.Reset();
			mReceiveCount.Reset();
		}

		public void OnSendReliable(int count)
		{
			//mSendReliable.Accumulate
			//mSendUnreliableCount.Accumulate(1);
		}
	}

	public class SteamNetworkTransporter : IServiceable
	{
		public event Action<SteamId> OnP2PDisconnected;
		public event Action<SteamId, byte[], uint> OnPacketReceived;
		private const int CHANNEL = 0;
		private byte[] mReceiveBuffer = new byte[Numeric.KiB * 64];
		public bool IsRunning { get; private set; }

		private HashSet<SteamId> mP2PConnectedUser = new HashSet<SteamId>();

		public void Start()
		{
			IsRunning = true;
			//SteamNetworkingUtils.Timeout
			//SteamNetworkingUtils.ConnectionTimeout
			//SteamNetworkingSockets
		}

		public void Stop()
		{
			IsRunning = false;

			foreach (var user in mP2PConnectedUser)
			{
				Disconnect(user);
			}

			mP2PConnectedUser.Clear();
		}

		public void OnRegistered()
		{
			SteamNetworking.OnP2PSessionRequest = onP2PSessionRequest;
			SteamNetworking.OnP2PConnectionFailed = onP2PConnectionFailed;
			SteamNetworking.AllowP2PPacketRelay(true);
		}

		public void OnUnregistered()
		{
			Stop();
			SteamNetworking.OnP2PSessionRequest = null;
			SteamNetworking.OnP2PConnectionFailed = null;
			SteamNetworking.AllowP2PPacketRelay(false);
		}

		public void Disconnect(SteamId sessionID)
		{
			OnP2PDisconnected?.Invoke(sessionID);

			if (mP2PConnectedUser.TryGetValue(sessionID, out var disconnectUser))
			{
				SteamNetworking.CloseP2PSessionWithUser(disconnectUser);
				Ulog.Log(this, $"P2P disconnect {sessionID}");
			}
		}

		private void onP2PConnectionFailed(SteamId requestID, P2PSessionError errorCode)
		{
			OnP2PDisconnected?.Invoke(requestID);

			Ulog.Log(this, $"P2P connection failed! [Steam ID : {requestID} / ErrorCode : {errorCode}]");
			Disconnect(requestID);
		}

		private void onP2PSessionRequest(SteamId requestID)
		{
			if (!IsRunning)
			{
				Ulog.LogError(this, $"P2P connection accept reject : Transporter is not currently running!");
				return;
			}

			if (SteamNetworking.AcceptP2PSessionWithUser(requestID))
			{
				if (mP2PConnectedUser.Add(requestID))
				{
					Ulog.Log(this, $"P2P connection accept : {requestID}");
				}
			}
			else
			{
				OnP2PDisconnected?.Invoke(requestID);

				Ulog.LogError(this, $"P2P connection accept failed !: {requestID}");
				SteamNetworking.CloseP2PSessionWithUser(requestID);
			}
		}

		public void SendToViaReliable(SteamId sessionID, NetPacket packet)
		{
			packet.GetRawData(out var rawData, out int length);
			SteamNetworking.SendP2PPacket(sessionID, rawData, length, CHANNEL, P2PSend.Reliable);
		}

		public void SendToViaUnreliable(SteamId sessionID, NetPacket packet)
		{
			packet.GetRawData(out var rawData, out int length);
			SteamNetworking.SendP2PPacket(sessionID, rawData, length, CHANNEL, P2PSend.UnreliableNoDelay);
		}

		public void OnTick()
		{
			if (!IsRunning)
			{
				while (SteamNetworking.IsP2PPacketAvailable(CHANNEL))
				{
					uint dataSize = 0;
					SteamId from = new SteamId();
					if (SteamNetworking.ReadP2PPacket(mReceiveBuffer, ref dataSize, ref from))
					{
						Ulog.LogWarning(this, $"Packet ignored. Transporter is not running.");
					}
				}

				return;
			}

			while (SteamNetworking.IsP2PPacketAvailable(CHANNEL))
			{
				uint dataSize = 0;
				SteamId from = new SteamId();
				if (SteamNetworking.ReadP2PPacket(mReceiveBuffer, ref dataSize, ref from))
				{
					if (dataSize > PacketPool.StreamSize)
					{
						Ulog.LogError(this, $"Receive packet overflowed!. Packet size : {dataSize}");
						return;
					}

					OnPacketReceived?.Invoke(from, mReceiveBuffer, dataSize);
				}
				else
				{
					Ulog.LogError(this, $"Receive packet error!");
				}
			}
		}
	}
}
