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
			Debug.Log("UPDATING SCORE");
			var reactionTime = collision.gameObject.GetComponent<TargetScript>().lifeTime;
			this.transform.parent.parent.GetComponent<PlayerScript>().TargetHit(reactionTime);
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
