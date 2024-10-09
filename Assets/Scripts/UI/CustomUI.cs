using UnityEngine;

public abstract class CustomUI : MonoBehaviour 
{ 
	protected PlayerScript m_Player;

	protected virtual void Init()
	{
		m_Player = null;//TODO: m_Player = GameManagerScript.Instance.PlayerScripts[0]; //TODO: Multiplayer
	}
}