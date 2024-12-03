using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
