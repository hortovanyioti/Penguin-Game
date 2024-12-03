using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSpawner : MonoBehaviour
{
	[SerializeField] private GameObject targetPrefab;
	[Header("Target spawning border")]
	[SerializeField] private float xMax;
	[SerializeField] private float xMin;
	[SerializeField] private float zMax;
	[SerializeField] private float zMin;
	[SerializeField] private float yMax;
	[SerializeField] private float yMin;


	// Start is called before the first frame update
	void Start()
	{
#if UNITY_EDITOR
		if (targetPrefab == null)
		{
			Debug.LogWarning("No enemy prefabs assigned to the spawner");
		}
#endif

	}

	// Update is called once per frame
	void Update()
	{
		TrySpawn();
	}

	private void TrySpawn()
	{
		if (GameManager.Instance.IsGameOver)
			return;

		var n = targetPool.transform.childCount;
		for (int i = 0; i < n; i++)
		{
			if (targetPool.transform.GetChild(i).localScale.x != 0) //If a target is still alive, no spawn
			{
				return;
			}
		}
		Spawn();
	}
	private void Spawn()
	{
		GameObject newTarget = Instantiate(targetPrefab);
		newTarget.transform.localScale = new Vector3(
			newTarget.transform.localScale.x * Difficulty.TargetScale,
			newTarget.transform.localScale.y * Difficulty.TargetScale,
			newTarget.transform.localScale.z);  //Scaling z below 0.2f causes unstable behaviour		
		
		newTarget.transform.parent = this.transform;
		newTarget.transform.position = new Vector3(UnityEngine.Random.Range(xMin, xMax), UnityEngine.Random.Range(yMin, yMax), UnityEngine.Random.Range(zMin, zMax));
		newTarget.transform.LookAt(new Vector3(0f, newTarget.transform.position.y, 0f));
	}
}
