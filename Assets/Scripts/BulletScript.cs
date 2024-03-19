using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
	private float bulletLife = 5.0f;

	void Update()
	{
		bulletLife -= Time.deltaTime;
		if (bulletLife <= 0 || this.transform.position.magnitude > 1000)
		{
			Destroy(gameObject);
		}
	}
}
