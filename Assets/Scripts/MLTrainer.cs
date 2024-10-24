using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor.EditorTools;
using UnityEngine;

public class MLTrainer : MonoBehaviour
{
	[SerializeField] float spawnDistance = 3f;
	[SerializeField] int spawnCountRoot = 2;
	[SerializeField] GameObject trainingEnvironment;

	private void Start()
	{
		//Populate(5, 5);
	}

	[ContextMenu("Populate")]
	public void Populate()
	{
		ClearChildren();

		for (int i = 0; i < spawnCountRoot; i++)
		{
			for (int j = 0; j < spawnCountRoot; j++)
			{
				GameObject newEnemy = Instantiate(trainingEnvironment, new Vector3(i * spawnDistance, 0, j * spawnDistance), Quaternion.identity);
				newEnemy.transform.parent = transform;
			}
		}
	}
	public void ClearChildren()
	{
		while(transform.childCount > 0)
		{
			DestroyImmediate(transform.GetChild(0).gameObject);
		}
	}

}
