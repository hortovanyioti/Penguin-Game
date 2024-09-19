using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
	public float bulletLife = 5.0f;

	void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag == "Target")
		{
			var reactionTime = collision.gameObject.GetComponent<TargetScript>().lifeTime;
			//GameManager.Instance.PlayerScripts[0].TargetHit(reactionTime);//TODO: Multiplayer
			Destroy(gameObject);
		}
	}
	void Update()
	{
		bulletLife -= Time.deltaTime;
		if (bulletLife <= 0 || this.transform.position.magnitude > 10000)
		{
			Destroy(gameObject);
		}
	}
}