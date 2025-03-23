using UnityEngine;

public class JoystickHandler : MonoBehaviour
{
	private void Awake()
	{
		if (RuntimePlatform.Android == Application.platform)
		{
			this.gameObject.SetActive(true);
		}
	}
}
