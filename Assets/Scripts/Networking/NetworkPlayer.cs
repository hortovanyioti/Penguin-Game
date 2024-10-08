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
		CustomNetworkManager.Instance.AddPlayer(this);
		CustomSteamLobby.Instance.UpdateLobbyName();
		CustomSteamLobby.Instance.UpdatePlayerList();
	}

	public override void OnStopClient()
	{
		CustomNetworkManager.Instance.RemovePlayer(this);
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
		CustomNetworkManager.Instance.StartGame(SceneName);
	}

	[TargetRpc]
	public void RpcActivateInput(NetworkIdentity identity,int playerIndex)
	{
		CustomNetworkManager.Instance.Players.Game[playerIndex].ActivateInput(true);
		Debug.Log("Input activated for: " + PlayerName + ". ISSERVER: " + isServer + " | ISCLIENT: " + isClient);
	}

	[Server]
	public void ServerActivateInput(NetworkIdentity identity, int playerIndex)
	{
		CustomNetworkManager.Instance.Players.Game[playerIndex].ActivateInput(true);
		Debug.Log("Input activated for: " + PlayerName + ". ISSERVER: " + isServer + " | ISCLIENT: " + isClient);
	}

	[TargetRpc]
	public void RpcChangeTimeScale(NetworkIdentity identity, float value)
	{
		Time.timeScale = value;
		Debug.Log("TimeScale changed to " + value +". " + PlayerName + ". ISSERVER: " + isServer + " | ISCLIENT: " + isClient);
	}

	[Server]
	public void ServerChangeTimeScale(NetworkIdentity identity, float value)
	{
		Time.timeScale = value;
		Debug.Log("TimeScale changed to " + value + ". " + PlayerName + ". ISSERVER: " + isServer + " | ISCLIENT: " + isClient);
	}
}
