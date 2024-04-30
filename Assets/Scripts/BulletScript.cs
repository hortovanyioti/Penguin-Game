using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
	private float bulletLife = 5.0f;

	void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag == "Target")
		{
			var reactionTime = collision.gameObject.GetComponent<TargetScript>().lifeTime;
			GameManagerScript.instance.PlayerScripts[0].TargetHit(reactionTime);//TODO: Multiplayer
			Destroy(gameObject);
		}
	}
	void Update()
	{
		bulletLife -= Time.deltaTime;
		if (bulletLife <= 0 || this.transform.position.magnitude > 1000)
		{
			Destroy(gameObject);
		}
	}
}