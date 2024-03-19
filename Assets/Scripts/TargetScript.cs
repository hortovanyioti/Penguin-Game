using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetScript : MonoBehaviour
{
	void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag == "Bullet")
		{
			this.gameObject.transform.localPosition += new Vector3(1, 0, 0);
			//gameObject.SetActive(false);
		}
	}
}
