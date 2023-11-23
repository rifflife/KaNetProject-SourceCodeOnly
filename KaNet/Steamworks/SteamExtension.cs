using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Steamworks.Data;
using UnityEngine;
using Utils;

using KaNet;
using KaNet.Session;

namespace KaNet.SteamworksAPI
{
	public static class SteamLobbyExtension
	{
		public static string GetLobbyName(this Lobby lobby)
		{
			return lobby.GetData(KaNetGlobal.KEY_LOBBY_NAME);
		}

		public static string GetLobbyDescription(this Lobby lobby)
		{
			return lobby.GetData(KaNetGlobal.KEY_LOBBY_DESCRIPTION);
		}

		public static string GetGameVersion(this Lobby lobby)
		{
			return lobby.GetData(KaNetGlobal.KEY_GAME_VERSION);
		}

		public static bool HasPassword(this Lobby lobby)
		{
			var value = lobby.GetData(KaNetGlobal.KEY_LOBBY_HAS_PASSWORD);
			bool.TryParse(value, out var result);
			return result;
		}

		/// <summary>로비 제목을 설정합니다. 유효하지 않은 제목이거나, 제목이 너무 길면 기본 방 제목을 사용합니다.</summary>
		/// <param name="name">로비 제목</param>
		public static void SetLobbyName(this Lobby lobby, string name = "")
		{
			if (!name.IsValid() || !lobby.SetData(KaNetGlobal.KEY_LOBBY_NAME, name))
			{
				lobby.SetData(KaNetGlobal.KEY_LOBBY_NAME, KaNetGlobal.DEFAULT_LOBBY_NAME);
			}
		}

		/// <summary>로비 설명을 설정합니다.</summary>
		/// <param name="description">비밀번호 문자열</param>
		public static void SetLobbyDescription(this Lobby lobby, string description = "")
		{
			if (!description.IsValid() || !lobby.SetData(KaNetGlobal.KEY_LOBBY_DESCRIPTION, description))
			{
				lobby.SetData(KaNetGlobal.KEY_LOBBY_DESCRIPTION, KaNetGlobal.DEFAULT_LOBBY_DESCRIPTION);
			}
		}

		/// <summary>게임의 버전을 설정합니다. 기본값 null은 게임의 디폴트 버전입니다.</summary>
		/// <param name="version">설정할 버전. null일 경우 게임의 디폴트 버전으로 설정</param>
		public static void SetGameVersion(this Lobby lobby, string version = null)
		{
			if (!version.IsValid() || !lobby.SetData(KaNetGlobal.KEY_GAME_VERSION, version))
			{
				lobby.SetData(KaNetGlobal.KEY_GAME_VERSION, KaNetGlobal.GameVersion);
			}
		}

		/// <summary>비밀번호 유무를 설정합니다.</summary>
		/// <param name="password"></param>
		public static void SetHasPassword(this Lobby lobby, string password = null)
		{
			if (!password.IsValid() || !lobby.SetData(KaNetGlobal.KEY_LOBBY_HAS_PASSWORD, password))
			{
				lobby.SetData(KaNetGlobal.KEY_LOBBY_HAS_PASSWORD, password.IsValid().ToString());
			}
		}

		public static void Setup(this Lobby lobby, LobbySetting lobbySetting)
		{
			// Setup Lobby Base
			lobby.SetGameVersion(lobbySetting.ProgramID.Version);
			lobby.MaxMembers = lobbySetting.MaxPlayer;

			if (lobbySetting.IsPublic)
			{
				lobby.SetPublic();
			}
			else
			{
				lobby.SetPrivate();
			}

			if (lobbySetting.IsInvisible)
			{
				lobby.SetInvisible();
			}

			if (lobbySetting.IsFriendsOnly)
			{
				lobby.SetFriendsOnly();
			}

			lobby.SetJoinable(lobbySetting.IsJoinable);

			// Setup Lobby Data
			lobby.SetLobbyName(lobbySetting.LobbyName);
			lobby.SetLobbyDescription(lobbySetting.LobbyDescription);
			lobby.SetHasPassword(lobbySetting.LobbyPassword);
		}
	}

	public static class SteamExtension
	{
		public static UnityEngine.Color ToUnityColor(this Steamworks.Data.Color color)
		{
			return new UnityEngine.Color(color.r / 255.0f, color.g / 255.0f, color.b / 255.0f, color.a / 255.0f);
		}
	}
}
