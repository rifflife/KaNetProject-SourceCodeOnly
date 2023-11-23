using MonoGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MonoGUI
{
	public enum IngameGuiState
	{
		None = 0,
		/// <summary>�κ� ȭ��</summary>
		Lobby,
		/// <summary>�÷��̾ ����ִ� ����</summary>
		Alive,
		/// <summary>�÷��̾ ����� ����</summary>
		Dead,
		/// <summary>�̺�Ʈ �������� ����</summary>
		Event,
		/// <summary>���� ȭ��</summary>
		Reward,
	}

	public class InGameGUISystem : MonoBehaviour
	{
		[field:SerializeField]
		public Navigation_Ingame Navigation_InGame { get; private set; }
		[field:SerializeField]
		public Navigation_IngameSystem Navigation_IngameSystem { get; private set; }

		public IngameGuiState State { get; private set; } = IngameGuiState.None;

		private GameplayManager mGameplayManager;

		public void InitializeByManager(GameplayManager gameplayManager)
		{
			mGameplayManager = gameplayManager;
		}

		public void SwitchToLobby(IngameSessionHandler sessionHandler)
		{
			Navigation_InGame.SwitchToLobby(sessionHandler);
			State = IngameGuiState.Lobby;
		}

		public void SwitchToAlive(GameplayManager gameplayManager)
		{
			Navigation_InGame.SwitchToAlive(gameplayManager);
			State = IngameGuiState.Alive;
		}

		public void ShowSystemMessage(MessageType messageType, string message)
		{
			Navigation_InGame.ShowSystemMessage(messageType, message);
		}
	}
}
