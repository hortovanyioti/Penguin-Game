using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetScript : MonoBehaviour
{
	private AudioSource targetHitSound;
	public float lifeTime { get; private set; } = 0f;
	void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag == "Bullet")
		{
			targetHitSound.Play();
			gameObject.transform.localScale = new Vector3(0f, 0f, 0f);
		}
	}
	void Start()
	{
		targetHitSound = GetComponent<AudioSource>();
	}
	private void Update()
	{
		lifeTime += Time.deltaTime;
		if (gameObject.transform.localScale.x == 0 && !targetHitSound.isPlaying)  //If the target has been hit and the sound has finished playing, deactivate the target
		{
			Destroy(gameObject);
		}
	}
}
