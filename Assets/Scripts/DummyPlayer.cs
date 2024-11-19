using UnityEngine;
using UnityEngine.AI;

public class DummyPlayer : GameCharacter
{
	NavMeshAgent navmeshAgent;

	float navmeshUpdateInterval = 5f;
	float navmeshUpdateTimer = float.MaxValue;
	int spawnRange = 25;
	private void Start()
	{
		navmeshAgent = GetComponent<NavMeshAgent>();
	}

	private void OnEnable()
	{
		CurrentHealth = MaxHealth;
	}

	private void Update()
	{
		navmeshUpdateTimer += Time.deltaTime;
		if (navmeshUpdateTimer >= navmeshUpdateInterval)
		{
			var newDestination = new Vector3(Random.Range(-spawnRange, spawnRange), 0, Random.Range(-spawnRange, spawnRange)) + transform.parent.parent.position;
			navmeshAgent.SetDestination(newDestination);
			navmeshUpdateTimer = 0;
		}
	}

	public override void TakeDamage(float damage)
	{
		CurrentHealth -= damage;
		if (CurrentHealth <= 0)
		{
			gameObject.SetActive(false);
		}
	}

}
