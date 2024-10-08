using Mirror;
using Steamworks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Extends the Mirror.NetworkManager class
/// Top of the network hierarchy
/// Stores the list of players in the game
/// </summary>

public class CustomNetworkManager : NetworkManager
{

	[SerializeField] private NetworkPlayer GamePlayerPrefab;

	private static CustomNetworkManager instance;

	public static CustomNetworkManager Instance
	{
		get
		{
			if (instance != null)
			{
				return instance;
			}
			return instance = CustomNetworkManager.singleton as CustomNetworkManager;
		}
	}

	public (
		List<GameObject> GameObjects, 
		List<NetworkPlayer> Network, 
		List<PlayerScript> Game) Players { get; } = (new List<GameObject>(), new List<NetworkPlayer>(), new List<PlayerScript>());

	public override void OnServerAddPlayer(NetworkConnectionToClient conn)
	{
		if (SceneManager.GetActiveScene().name == "MainMenuCoop")
		{
			NetworkPlayer GamePlayerInstance = Instantiate(GamePlayerPrefab);
			GamePlayerInstance.ConnectionID = conn.connectionId;
			GamePlayerInstance.PlayerIdNumber = Players.Network.Count + 1;
			GamePlayerInstance.PlayerSteamID = (ulong)SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)CustomSteamLobby.Instance.CurrentLobbyID, Players.Network.Count);

			NetworkServer.AddPlayerForConnection(conn, GamePlayerInstance.gameObject);
		}
	}

	public void StartGame(string SceneName)
	{
		ServerChangeScene(SceneName);

		for (int i = 0; i < Players.Network.Count; i++)
		{

			bool isLocalPlayer = (CSteamID)Players.Network[i].PlayerSteamID == SteamUser.GetSteamID();

			if (isLocalPlayer)
			{
				Players.Network[i].ServerChangeTimeScale(Players.Network[i].netIdentity, 1);
				Players.Network[i].ServerActivateInput(Players.Network[i].netIdentity, i);
			}
			else
			{
				Players.Network[i].RpcChangeTimeScale(Players.Network[i].netIdentity, 1);
				Players.Network[i].RpcActivateInput(Players.Network[i].netIdentity, i);
			}
		}
	}

	public void AddPlayer(NetworkPlayer player)
	{
		Players.Network.Add(player);
		Players.GameObjects.Add(player.gameObject);
		Players.Game.Add(player.GetComponent<PlayerScript>());
	}

	public void RemovePlayer(NetworkPlayer player)
	{
		Players.Game.RemoveAt(Players.Network.IndexOf(player));
		Players.GameObjects.Remove(player.gameObject);
		Players.Network.Remove(player);
	}
}
