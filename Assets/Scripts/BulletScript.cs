using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
	public GameCharacter Owner { get; private set; }
	public float bulletLife = 5.0f;
	MLRangedEnemy MLAgent;

	static GameObject BulletCollection;


	private void Start()
	{
		if(BulletCollection == null)
		{
			BulletCollection = new GameObject("BulletCollection");
		}
		transform.parent = BulletCollection.transform;
	}

	public void SetOwner(GameCharacter owner)
	{
		Owner = owner;
		MLAgent = Owner.GetComponent<MLRangedEnemy>();
	}

	void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.CompareTag("Target") || collision.gameObject.CompareTag("Player"))
		{
			var gameChar = collision.gameObject.GetComponent<GameCharacter>();
			if (gameChar != null)
			{
				gameChar.Hurt(10);
			}

			if (MLAgent != null)
			{
				MLAgent.BulletFeedback(true);
			}

			//TODO var reactionTime = collision.gameObject.GetComponent<TargetScript>().lifeTime;
			//GameManager.Instance.PlayerScripts[0].TargetHit(reactionTime);//TODO: Multiplayer
			Destroy(gameObject);
		}
		else if (collision.gameObject.CompareTag("Wall"))
		{
			if (MLAgent != null)
			{
				MLAgent.BulletFeedback(false);
			}
			Destroy(gameObject);
		}
	}
	void Update()
	{
		bulletLife -= Time.deltaTime;
		if (bulletLife <= 0 || this.transform.position.magnitude > 10000)
		{
			if (MLAgent != null)
			{
				MLAgent.BulletFeedback(false);
			}
			Destroy(gameObject);
		}
	}
}