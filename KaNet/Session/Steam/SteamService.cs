using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Steamworks;
using Utils;
using Utils.Service;

namespace KaNet.Session
{
	public class SteamService : IServiceable
	{
		public SteamId CurrentUserID => SteamClient.SteamId;

		public bool IsValid { get; private set; }

		public uint AppID { get; private set; }

		public SteamService(uint appID)
		{
			AppID = appID;
		}

		public void OnRegistered()
		{
			try
			{
				SteamClient.Init(AppID, true);

				IsValid = SteamClient.IsValid;

				if (!IsValid)
				{
					Ulog.LogError(this, "SteamClient is not valid! It's not initialized!");
					return;
				}

				Ulog.Log(this, "SteamService is up and running!");
			}
			catch (Exception e)
			{
				Ulog.LogError(this, $"SteamService initialize faild!\n{e}");
				return;
			}
		}

		public void OnUnregistered()
		{
			try
			{
				SteamClient.Shutdown();
				Ulog.Log(this, "SteamService ended!");
			}
			catch
			{
				Ulog.LogError(this, "SteamClient Shutdown Error");
			}
		}

		public bool TryGetFriendList(out IEnumerable<Friend> friends)
		{
			if (!IsValid)
			{
				friends = new List<Friend>();
				return false;
			}

			friends = SteamFriends.GetFriends();
			return true;
		}
	}
}