using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KaNet.Synchronizers;

namespace KaNet
{
	public static class KaNetGlobal
	{
		// Lobby Data Key
		public const int DEFAULT_MAX_PLAYER = 4;
		public const int SYSTEM_MAX_PLAYER = 64;
		public static readonly string KEY_LOBBY_NAME = "lobby_name";
		public static readonly string KEY_LOBBY_HAS_PASSWORD = "lobby_has_password";
		public static readonly string KEY_LOBBY_DESCRIPTION = "lobby_description";

		public static readonly string KEY_GAME_VERSION = "game_version";

		public static readonly string DEFAULT_LOBBY_NAME = "Let's get in to the hell!";
		public static readonly string DEFAULT_LOBBY_DESCRIPTION = "The trolley dilemma server.";
		public static readonly string ERROR_VERSION = "ERROR_VERSION";

		public static readonly int DEFALUT_GUI_OPERATION_DELAY = 150;

		public static string GameVersion { get; set; }  = "test_version";

		// Syntax
		public static readonly string RPC_PREFIX = "RPC_";

		// Low Level Network
		public const int CONNECTION_TIMEOUT_INTERVAL = 10000; // ms
		public const int DEFAULT_MTU = 1200;
		public const int UDP_HEADER_SIZE = 8;

		public const int VALID_PORT_FIRST = 49152;
		public const int VALID_PORT_LAST = 65535;

		public const float HEARTBEAT_CHECK_INTERVAL_SEC = 5.0f;
		public const float HEARTBEAT_SEND_INTERVAL_SEC = 3.0F;
		public const int HEARTBEAT_TIMEOUT_INTERVAL_MS = (int)(HEARTBEAT_CHECK_INTERVAL_SEC * 1000) * 2000;

		// Synchronizer
		public const int INITIAL_OBJECT_COUNT = 1000;
		public const int NETWORK_TICK = 30;
		public const int NETWORK_INTERPOLATION_VALUE = NETWORK_TICK;
		public const float NETWORK_TICK_INTERVAL_SEC = 1.0F / NETWORK_TICK;
		public const int NETWORK_TICK_INTERVAL_MS = (int)(NETWORK_TICK_INTERVAL_SEC * 1000);
		public static readonly NetTimestamp LAG_COMPENSATION_INTERVAL_SEC = new NetTimestamp(1000);
		public static readonly NetTimestamp FIXED_SYNC_DELAY = NETWORK_TICK_INTERVAL_MS * 2;

		public static bool IsValidPort(int portNumber)
		{
			return portNumber >= VALID_PORT_FIRST && portNumber <= VALID_PORT_LAST;
		}
	}
}
