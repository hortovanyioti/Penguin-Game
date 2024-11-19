using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class CustomNavMesh : MonoBehaviour
{
	NavMeshSurface navMeshSurface;
	[SerializeField] bool autoBake = true;

	void Awake()
	{
		navMeshSurface = GetComponent<NavMeshSurface>();
		if (autoBake)
		{
			navMeshSurface.BuildNavMesh();
		}
	}
}
