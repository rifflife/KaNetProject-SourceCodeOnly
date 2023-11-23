using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// 
/// 네트워크에서의 메세지와 관련된 다국어 메세지 정의 영역입니다.
/// 

namespace Utils
{
	public enum SystemMessageType
	{
		None,
		Success,

		YouAreNotInTheLobby,
		YouAlreadyInTheLobby,

		// Lobby
		Lobby_ClientFailToJoin,
		Lobby_ClientSuccessToJoin,

		Lobby_CreateLobbyFail,
		Lobby_CreateLobbyError,

		Lobby_UserJoined,
		Lobby_UserLeft,
		Lobby_UserDisconnected,
		Lobby_UserKicked,
		Lobby_UserBanned,

		// Game
		Game_UserDead,
	}

	public static partial class Localization
	{
		private static readonly Dictionary<SystemMessageType, string> mSystemMessageTable = new()
		{
			{ SystemMessageType.None, $"SystemMessageType.None" },
			
			{ SystemMessageType.YouAlreadyInTheLobby, $"이미 로비에 입장한 상태입니다." },
			// Lobby
			{ SystemMessageType.Lobby_ClientFailToJoin, $"로비 입장에 실패했습니다.<br>{TextMaker.FormatSeparator}" },
			{ SystemMessageType.Lobby_ClientSuccessToJoin, $"로비 입장에 성공했습니다." },

			{ SystemMessageType.Lobby_CreateLobbyFail, $"로비 생성에 실패했습니다." },
			{ SystemMessageType.Lobby_CreateLobbyError, $"로비 생성 에러.<br>{TextMaker.FormatSeparator}" },

			{ SystemMessageType.Lobby_UserJoined, $"플레이어 \"{TextMaker.FormatSeparator}\"가 입장했습니다." },
			{ SystemMessageType.Lobby_UserLeft, $"플레이어 \"{TextMaker.FormatSeparator}\"가 퇴장했습니다." },
			{ SystemMessageType.Lobby_UserDisconnected, $"플레이어 \"{TextMaker.FormatSeparator}\"는 연결이 끊겼습니다." },
			{ SystemMessageType.Lobby_UserKicked, $"플레이어 \"{TextMaker.FormatSeparator}\"는 퇴장 당했습니다." },
			{ SystemMessageType.Lobby_UserBanned, $"플레이어 \"{TextMaker.FormatSeparator}\"는 차단 당했습니다." },

			// Game
			{ SystemMessageType.Game_UserDead, $"플레이어 \"{TextMaker.FormatSeparator}\"는 죽었습니다."},
		};

		public static string GetSystemMessage(SystemMessageType systemMessageType)
		{
			if (mSystemMessageTable.TryGetValue(systemMessageType, out var result))
			{
				return result;
			}

			return $"{DefaultText} : {systemMessageType}";
		}

		public static string GetSystemMessage(SystemMessageType systemMessageType, object argument)
		{
			string text = GetSystemMessage(systemMessageType);
			return TextMaker.GetStringByFormat(text, argument);
		}

		public static string GetSystemMessage(SystemMessageType systemMessageType, IList arguments)
		{
			string text = GetSystemMessage(systemMessageType);
			return TextMaker.GetStringByFormat(text, arguments);
		}
	}
}
