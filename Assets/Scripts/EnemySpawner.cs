using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
	[SerializeField] GameObject[] enemyPrefabs;
	[SerializeField] float initialSpawnInterval = 5f;
	[SerializeField] float minSpawnInterval = 2f;
	private float spawnInterval = 0;
	private float spawnTimer = 0;

	[SerializeField] float spawnRange = 5f;


	// Start is called before the first frame update
	void Start()
	{
#if UNITY_EDITOR
		if (enemyPrefabs.Length == 0)
		{
			Debug.LogWarning("No enemy prefabs assigned to the spawner");
		}
#endif

		spawnInterval = initialSpawnInterval;
		spawnTimer = spawnInterval - 3;	//TODO Link to prep time
	}

	// Update is called once per frame
	void Update()
	{
		spawnTimer += Time.deltaTime;
		if (spawnTimer >= spawnInterval)
		{
			SpawnEnemy();
			spawnTimer = 0;
			spawnInterval = Mathf.Max(minSpawnInterval, spawnInterval - 0.2f);
		}
	}

	private void SpawnEnemy()
	{
		int enemyIndex = Random.Range(0, enemyPrefabs.Length);
		Vector3 spawnPosition = transform.position + new Vector3(Random.Range(-spawnRange, spawnRange), 0, Random.Range(-spawnRange, spawnRange));
		GameObject enemy = Instantiate(enemyPrefabs[enemyIndex], spawnPosition, Quaternion.identity);
		enemy.transform.parent = transform;
	}
}
