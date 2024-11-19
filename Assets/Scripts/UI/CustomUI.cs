using UnityEngine;

public abstract class CustomUI : MonoBehaviour 
{ 
	protected PlayerScript m_Player;

	protected virtual void Init()
	{
		m_Player = CustomNetworkManager.Instance.Players.Game[0];//TODO: m_Player = GameManagerScript.Instance.PlayerScripts[0]; //TODO: Multiplayer
	}
}