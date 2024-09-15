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

	public List<NetworkPlayer> GamePlayers { get; } = new List<NetworkPlayer>();

	public override void OnServerAddPlayer(NetworkConnectionToClient conn)
	{
		if (SceneManager.GetActiveScene().name == "MainMenuCoop")
		{
			NetworkPlayer GamePlayerInstance = Instantiate(GamePlayerPrefab);
			GamePlayerInstance.ConnectionID = conn.connectionId;
			GamePlayerInstance.PlayerIdNumber = GamePlayers.Count + 1;
			GamePlayerInstance.PlayerSteamID = (ulong)SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)CustomSteamLobby.Instance.CurrentLobbyID, GamePlayers.Count);

			NetworkServer.AddPlayerForConnection(conn, GamePlayerInstance.gameObject);
		}
	}

	public void StartGame(string SceneName)
	{
		ServerChangeScene(SceneName);
		foreach (var player in GamePlayers)
		{
			player.GetComponent<PlayerScript>().ActivatePhysics();

			bool isLocalPlayer = (CSteamID)player.PlayerSteamID == SteamUser.GetSteamID();

			if(isLocalPlayer)
			{
				player.ServerActivateInput(player);
			}
			else
			{
				player.TestRPC();
				player.ClientActivateInput();
			}
		}
	}
}
