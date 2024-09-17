using Mirror;
using Steamworks;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
	//Player Data
	[SyncVar] public int ConnectionID;
	[SyncVar] public int PlayerIdNumber;
	[SyncVar] public ulong PlayerSteamID;
	[SyncVar(hook = nameof(PlayerNameUpdate))] public string PlayerName;

	private CustomNetworkManager manager;

	private CustomNetworkManager Manager
	{
		get
		{
			if (manager != null)
			{
				return manager;
			}
			return manager = CustomNetworkManager.singleton as CustomNetworkManager;
		}
	}

	private void Start()
	{
		DontDestroyOnLoad(this.gameObject);
	}

	public override void OnStartAuthority()
	{
		CmdSetPlayerName(SteamFriends.GetPersonaName().ToString());
		gameObject.name = "LocalGamePlayer";
		CustomSteamLobby.Instance.FindLocalPlayer();
		CustomSteamLobby.Instance.UpdateLobbyName();
	}

	public override void OnStartClient()
	{
		Manager.AddPlayer(this);
		CustomSteamLobby.Instance.UpdateLobbyName();
		CustomSteamLobby.Instance.UpdatePlayerList();
	}

	public override void OnStopClient()
	{
		Manager.RemovePlayer(this);
		CustomSteamLobby.Instance.UpdatePlayerList();
	}

	[Command]
	public void CmdSetPlayerName(string PlayerName)
	{
		this.PlayerNameUpdate(this.PlayerName, PlayerName);
	}

	public void PlayerNameUpdate(string oldName, string newName)
	{
		if (isServer)
		{
			this.PlayerName = newName;
		}
		if (isClient)
		{
			CustomSteamLobby.Instance.UpdatePlayerList();
		}
	}

	public void TryStartGame(string SceneName)
	{
		if (isServer)
		{
			CmdStartGame(SceneName);
		}
	}

	[Command]
	public void CmdStartGame(string SceneName)
	{
		Manager.StartGame(SceneName);
	}

	[TargetRpc]
	public void RpcActivateInput(NetworkIdentity identity,int playerIndex)
	{
		Manager.GamePlayers[playerIndex].ActivateInput();
		Debug.Log("Input activated for: " + PlayerName + ". ISSERVER: " + isServer + " | ISCLIENT: " + isClient);
	}

	[Server]
	public void ServerActivateInput(NetworkIdentity identity, int playerIndex)
	{
		Manager.GamePlayers[playerIndex].ActivateInput();
		Debug.Log("Input activated for: " + PlayerName + ". ISSERVER: " + isServer + " | ISCLIENT: " + isClient);
	}

	[ClientRpc(includeOwner = true)]
	public void RpcChangeTimeScale(NetworkIdentity identity, float value)
	{
		Time.timeScale = value;
		Debug.Log("TimeScale changed to " + value +". " + PlayerName + ". ISSERVER: " + isServer + " | ISCLIENT: " + isClient);
	}
}
