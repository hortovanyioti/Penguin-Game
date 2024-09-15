using Mirror;
using Steamworks;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

/// <summary>
/// Manages the Steam invites, lobby and the players in it
/// </summary>

public class CustomSteamLobby : MonoBehaviour
{
	public static CustomSteamLobby Instance;
	
	//Callbacks
	protected Callback<LobbyCreated_t> LobbyCreated;
	protected Callback<GameLobbyJoinRequested_t> JoinRequest;
	protected Callback<LobbyEnter_t> LobbyEntered;

	//Variables
	public ulong CurrentLobbyID;
	private const string HostAddressKey = "HostAddress";
	private CustomNetworkManager Manager;
	public bool autoHostLobby;

	//Player data
	public GameObject PlayerListViewContent;
	public GameObject PlayerListItemPrefab;
	public GameObject LocalPlayerObject;
	private NetworkPlayer LocalPlayerController;

	//Other Data
	private bool PlayerItemCreated = false;
	private List<PlayerListItem> PlayerListItems = new List<PlayerListItem>();

	//UI
	public TextMeshProUGUI LobbyNameText;

	private void Start()
	{
		if (!SteamManager.Initialized)
		{
			Debug.LogError("Steam Manager not initialized! Is Steam running?");
			return;
		}

		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(this.gameObject);
		}

		Manager = GetComponent<CustomNetworkManager>();
		LobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
		JoinRequest = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
		LobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);

		if (autoHostLobby)
		{
			HostLobby();
		}
	}

	/// <summary>
	/// Steam related functions
	/// </summary>
	public void HostLobby()
	{
		SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, Manager.maxConnections);
	}

	private void OnLobbyCreated(LobbyCreated_t callback)
	{
		if (callback.m_eResult != EResult.k_EResultOK)
		{
			return;
		}

		Debug.Log("Lobby created Successfully");
		Manager.StartHost();

		SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey, SteamUser.GetSteamID().ToString());
		SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name", SteamFriends.GetPersonaName().ToString() + "'S LOBBY");
	}
	private void OnJoinRequest(GameLobbyJoinRequested_t callback)
	{
		Debug.Log("Request To Join Lobby");
		SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
	}
	private void OnLobbyEntered(LobbyEnter_t callback)
	{
		//Everyone

		CurrentLobbyID = callback.m_ulSteamIDLobby;
		UpdateLobbyName();

		//Clients
		if (NetworkServer.active)
		{
			return;
		}

		Manager.networkAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey);

		Manager.StartClient();
	}
	public void UpdateLobbyName()
	{
		LobbyNameText.text = SteamMatchmaking.GetLobbyData(new CSteamID(CurrentLobbyID), "name");
	}

	/// <summary>
	/// Player List Functions (Only in lobby)
	/// </summary>

	public void UpdatePlayerList()
	{
		if (!PlayerItemCreated)
		{
			CreateHostPlayerItem(); //Host
		}
		if (PlayerListItems.Count < Manager.GamePlayers.Count)
		{
			CreateClientPlayerItem(); //Client
		}
		if (PlayerListItems.Count > Manager.GamePlayers.Count)
		{
			RemovePlayerItem();
		}
		if (PlayerListItems.Count == Manager.GamePlayers.Count)
		{
			UpdatePlayerItem();
		}
	}

	public void FindLocalPlayer()
	{
		LocalPlayerObject = GameObject.Find("LocalGamePlayer");
		LocalPlayerController = LocalPlayerObject.GetComponent<NetworkPlayer>();
	}

	public void CreateHostPlayerItem()
	{
		foreach (NetworkPlayer player in Manager.GamePlayers)
		{
			GameObject NewPlayerItem = Instantiate(PlayerListItemPrefab);
			PlayerListItem NewPlayerItemScript = NewPlayerItem.GetComponent<PlayerListItem>();

			NewPlayerItemScript.PlayerName = player.PlayerName;
			NewPlayerItemScript.ConnectionID = player.ConnectionID;
			NewPlayerItemScript.PlayerSteamID = player.PlayerSteamID;
			NewPlayerItemScript.SetPlayerValues();
			NewPlayerItem.transform.SetParent(PlayerListViewContent.transform);
			NewPlayerItem.transform.localScale = Vector3.one;

			PlayerListItems.Add(NewPlayerItemScript);
		}
		PlayerItemCreated = true;
	}

	public void CreateClientPlayerItem()
	{
		foreach (NetworkPlayer player in Manager.GamePlayers)
		{
			if (!PlayerListItems.Any(x => x.ConnectionID == player.ConnectionID))
			{
				GameObject NewPlayerItem = Instantiate(PlayerListItemPrefab) as GameObject;
				PlayerListItem NewPlayerItemScript = NewPlayerItem.GetComponent<PlayerListItem>();

				NewPlayerItemScript.PlayerName = player.PlayerName;
				NewPlayerItemScript.ConnectionID = player.ConnectionID;
				NewPlayerItemScript.PlayerSteamID = player.PlayerSteamID;
				NewPlayerItemScript.SetPlayerValues();
				NewPlayerItem.transform.SetParent(PlayerListViewContent.transform);
				NewPlayerItem.transform.localScale = Vector3.one;

				PlayerListItems.Add(NewPlayerItemScript);
			}
		}
	}

	public void UpdatePlayerItem()
	{
		foreach (NetworkPlayer player in Manager.GamePlayers)
		{
			foreach (PlayerListItem PlayerListItem in PlayerListItems)
			{
				if (PlayerListItem.ConnectionID == player.ConnectionID)
				{
					PlayerListItem.PlayerName = player.PlayerName;
					PlayerListItem.SetPlayerValues();
				}
			}
		}
	}
	public void RemovePlayerItem()
	{
		List<PlayerListItem> playerListItemToRemove = new List<PlayerListItem>();

		foreach (PlayerListItem playerlistItem in PlayerListItems)
		{
			if (!Manager.GamePlayers.Any(x => x.ConnectionID == playerlistItem.ConnectionID))
			{
				playerListItemToRemove.Add(playerlistItem);
			}
		}
		if (playerListItemToRemove.Count > 0)
		{
			foreach (PlayerListItem playerlistItemToRemove in playerListItemToRemove)
			{
				GameObject ObjectToRemove = playerlistItemToRemove.gameObject;
				PlayerListItems.Remove(playerlistItemToRemove);
				Destroy(ObjectToRemove);
				ObjectToRemove = null;

			}
		}
	}
	public void StartGame(string SceneName)     //Put this on a button
	{
		LocalPlayerController.TryStartGame(SceneName);
	}
}
