using System.Threading.Tasks;
using UnityEngine;

public abstract class CustomUI : MonoBehaviour
{
	protected static PlayerScript m_Player;

	protected virtual async void Init()
	{
		while (m_Player == null)
		{
			if (CustomNetworkManager.Instance.Players.Game.Count == 0)
			{
				await Task.Delay(10);
			}
			else
			{
				m_Player = CustomNetworkManager.Instance.Players.Game[0]; //TODO: m_Player = GameManagerScript.Instance.PlayerScripts[0]; //TODO: Multiplayer
			}
		}
	}
}