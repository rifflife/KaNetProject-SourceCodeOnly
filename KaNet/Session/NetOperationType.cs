using System.Collections.Generic;

namespace KaNet.Session
{
	public enum NetOperationType : byte
	{
		None = 0,

		// Connection Operation
		TryConnect,
		TryDisconnect,
		TrySinglePlay,
		CreateLobby,

		// Friend Operation
		InviteFriend,

		// System Operation
		ReturnToTitle,
		QuitGame,

		// Event Operation
		OnDisconnected,
		OnJoinRequest,
		OnMemberJoined,
		OnInvited,
		OnLobbyLeave,
		OnLobbyJoined,
	}

	public enum NetOperationResult : byte
	{
		None = 0,
		Running,
		Success,

		Failed,
		InvalidOperation,
		Error,
		AlreadyInTheGame,
		NotInTheLobby,
		SteamInvalid,

		// Connect Callback
		WrongVersion,
		WrongPassword,
		ServerIsFull,
		ServerCannotHandle,
		ServerDisconnected,

		Timeout,
	}

	public static class NetworkOperationExtension
	{
		private static Dictionary<NetOperationType, string> mNetOperationTitleTable = new()
		{
			{ NetOperationType.None,				$"잘못된 접근" },

			// Connection Operation
			{ NetOperationType.TryConnect,			$"접속" },
			{ NetOperationType.TryDisconnect,		$"나가기" },
			{ NetOperationType.TrySinglePlay,		$"혼자하기" },
			{ NetOperationType.CreateLobby,			$"방 만들기" },

			// Friend Operation
			{ NetOperationType.InviteFriend,		$"친구 초대" },

			// System Operation
			{ NetOperationType.ReturnToTitle, $"은신처로" },
			{ NetOperationType.QuitGame, $"게임 종료" },

			// Event Operation
			{ NetOperationType.OnDisconnected,		$"연결 끊김" },
			{ NetOperationType.OnJoinRequest,		$"접속 요청" },
			{ NetOperationType.OnMemberJoined,		$"새로운 유저" },
			{ NetOperationType.OnInvited,			$"초대됨" },
			{ NetOperationType.OnLobbyLeave,		$"로비를 떠남" },
			{ NetOperationType.OnLobbyJoined,		$"로비에 참가함" },
		};

		public static string GetTitle(this NetOperationType netOperationType)
		{
			return mNetOperationTitleTable[netOperationType];
		}

		private static Dictionary<NetOperationResult, string> mNetOperationResultTable = new()
		{
			{ NetOperationResult.None,					$"잘못된 접근" },
			{ NetOperationResult.Running,				$"진행중" },
			{ NetOperationResult.Success,				$"완료되었습니다." },

			{ NetOperationResult.Failed,				$"실패했습니다." },
			{ NetOperationResult.InvalidOperation,		$"유효하지 않은 요청입니다." },
			{ NetOperationResult.Error,					$"알 수 없는 에러" },
			{ NetOperationResult.AlreadyInTheGame,		$"이미 게임에 참가한 상태입니다." },
			{ NetOperationResult.NotInTheLobby,			$"게임에 참가하지 않은 상태입니다." },
			{ NetOperationResult.SteamInvalid,			$"스팀이 유효하지 않은 상태입니다." },

			// Event Operation
			{ NetOperationResult.WrongVersion,			$"버전이 다릅니다." },
			{ NetOperationResult.WrongPassword,			$"비밀번호가 다릅니다." },
			{ NetOperationResult.ServerIsFull,			$"서버가 가득찼습니다." },
			{ NetOperationResult.ServerCannotHandle,	$"서버가 처리할 수 없는 요청입니다." },
			{ NetOperationResult.ServerDisconnected,	$"서버와의 연결이 끊겼습니다." },

			{ NetOperationResult.Timeout,				$"요청 시간이 만료되었습니다." },
		};

		public static string GetMessage(this NetOperationResult netOperationResult)
		{
			return mNetOperationResultTable[netOperationResult];
		}
	}
}
