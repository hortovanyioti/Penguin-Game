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

	public List<NetworkPlayer> NetworkPlayers { get; } = new List<NetworkPlayer>();     //Represents the network related data of the players
	public List<PlayerScript> GamePlayers { get; } = new List<PlayerScript>();          //Represents the game related data of the players

	public override void OnServerAddPlayer(NetworkConnectionToClient conn)
	{
		if (SceneManager.GetActiveScene().name == "MainMenuCoop")
		{
			NetworkPlayer GamePlayerInstance = Instantiate(GamePlayerPrefab);
			GamePlayerInstance.ConnectionID = conn.connectionId;
			GamePlayerInstance.PlayerIdNumber = NetworkPlayers.Count + 1;
			GamePlayerInstance.PlayerSteamID = (ulong)SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)CustomSteamLobby.Instance.CurrentLobbyID, NetworkPlayers.Count);

			NetworkServer.AddPlayerForConnection(conn, GamePlayerInstance.gameObject);
		}
	}

	public void StartGame(string SceneName)
	{
		ServerChangeScene(SceneName);

		NetworkPlayers[0].RpcChangeTimeScale(1);

		for (int i = 0; i < NetworkPlayers.Count; i++)
		{

			bool isLocalPlayer = (CSteamID)NetworkPlayers[i].PlayerSteamID == SteamUser.GetSteamID();

			if (isLocalPlayer)
			{
				NetworkPlayers[i].ServerActivateInput(NetworkPlayers[i].netIdentity, i);
			}
			else
			{
				NetworkPlayers[i].RpcActivateInput(NetworkPlayers[i].netIdentity, i);
			}
		}
	}

	public void AddPlayer(NetworkPlayer player)
	{
		NetworkPlayers.Add(player);
		GamePlayers.Add(player.GetComponent<PlayerScript>());
	}

	public void RemovePlayer(NetworkPlayer player)
	{
		GamePlayers.RemoveAt(NetworkPlayers.IndexOf(player));
		NetworkPlayers.Remove(player);
	}
}
