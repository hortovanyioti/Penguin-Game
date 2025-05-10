using Unity.AI.Navigation;
using UnityEngine;

[RequireComponent(typeof(NavMeshSurface))]
public class CustomNavMesh : MonoBehaviour
{
	NavMeshSurface navMeshSurface;
	[SerializeField] bool autoBake = true;

	void Awake()
	{
		navMeshSurface = GetComponent<NavMeshSurface>();
		if (autoBake)
		{
			Bake();
		}
	}

	public void Bake()
	{
		navMeshSurface.BuildNavMesh();
	}
}
