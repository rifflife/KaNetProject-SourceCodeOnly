using KaNet.Synchronizers;
using Utils;

namespace KaNet.Session
{
	public class LobbySetting
	{
		public NetProgramID ProgramID { get; set; }

		#region Lobby Setting

		public int MaxPlayer { get; set; } = KaNetGlobal.DEFAULT_MAX_PLAYER;
		/// <summary>스팀 서버를 통해 공개적으로 접속할 수 있습니다.</summary>
		public bool IsPublic { get; set; } = true;
		/// <summary>접속 가능 여부입니다. 싱글플레이인 경우 비활성화 해야 합니다.</summary>
		public bool IsJoinable { get; set; } = true;
		/// <summary>스팀 서버에서 찾을 수 없습니다. 다른 친구가 방에 입장할 수 있습니다.</summary>
		public bool IsFriendsOnly { get; set; } = false;
		/// <summary>스팀 서버를 통해 공개적으로 접속할 수 있습니다. 하지만 친구에게는 보이지 않습니다.</summary>
		public bool IsInvisible { get; set; } = false;

		#endregion
		
		#region Lobby Data

		public string LobbyName { get; private set; } = KaNetGlobal.DEFAULT_LOBBY_NAME;
		public string LobbyPassword { get; private set; } = "";
		public string LobbyDescription { get; private set; } = KaNetGlobal.DEFAULT_LOBBY_DESCRIPTION;

		#endregion

		public LobbySetting() { }

		public LobbySetting
		(
			NetProgramID programID,
			int maxPlayer,
			bool isPublic,
			bool isJoinable,
			bool isFriendsOnly,
			bool isInvisible)
		{
			ProgramID = programID;
			MaxPlayer = maxPlayer;
			IsPublic = isPublic;
			IsJoinable = isJoinable;
			IsFriendsOnly = isFriendsOnly;
			IsInvisible = isInvisible;
		}

		public static LobbySetting DefaultSinglePlaySetting => 
			new LobbySetting(new NetProgramID(), 1, false, false, false, false);
		public static LobbySetting DefaultPublicPlaySetting =>
			new LobbySetting(new NetProgramID(), KaNetGlobal.DEFAULT_MAX_PLAYER, true, true, false, false);

		public const int MAX_LOBBY_TITLE_NAME_LENGTH = 60;
		public const int MAX_LOBBY_PASSWORD_LENGTH = 20;
		public const int MAX_LOBBY_DESCRIPTION_LENGTH = 300;

		#region Setter

		public void SetProgramID(NetProgramID programID)
		{
			this.ProgramID = programID;
		}

		/// <summary>로비 이름을 설정합니다.</summary>
		/// <param name="lobbyName">로비 이름입니다.</param>
		public void SetLobbyName(string lobbyName)
		{
			LobbyName = (lobbyName.Length > MAX_LOBBY_TITLE_NAME_LENGTH) ? lobbyName.Substring(0, MAX_LOBBY_TITLE_NAME_LENGTH) : lobbyName;
		}

		/// <summary>비밀번호를 설정합니다.</summary>
		/// <param name="password">설정할 비밀번호입니다.</param>
		public void SetLobbyPassword(string password)
		{
			LobbyPassword = (password.Length > MAX_LOBBY_PASSWORD_LENGTH) ? password.Substring(0, MAX_LOBBY_PASSWORD_LENGTH) : password;
		}

		/// <summary>로비의 설명을 설정합니다.</summary>
		/// <param name="password">설정할 설명문입니다.</param>
		public void SetLobbyDescription(string description)
		{
			LobbyDescription = (description.Length > MAX_LOBBY_DESCRIPTION_LENGTH) ? description.Substring(0, MAX_LOBBY_DESCRIPTION_LENGTH) : description;
		}

		#endregion

		#region Operation

		public bool HasPassword() => LobbyPassword.IsValid();

		#endregion
	}
}
