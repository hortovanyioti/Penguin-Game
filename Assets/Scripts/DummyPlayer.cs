using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Experimental.AI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DummyPlayer : GameCharacter
{

	public override void Hurt(float damage)
	{
		CurrentHealth -= damage;
		if (CurrentHealth <= 0)
		{
			gameObject.SetActive(false);
		}
	}


}
