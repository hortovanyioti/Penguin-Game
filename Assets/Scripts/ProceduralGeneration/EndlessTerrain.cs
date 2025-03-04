using System;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UIElements;

public class EndlessTerrain : MonoBehaviour
{
	public const int ViewDistance = 8;
	[field: SerializeField] public Transform Viewer { get; private set; }
	public Vector2 ViewerCoords => new Vector2((int)Viewer.position.x / chunkSize, (int)Viewer.position.z / chunkSize);
	public Vector2 ViewerPos => new Vector2(Viewer.position.x, Viewer.position.z);

	public const int chunkSize = 250;
	public int loadBoarder = 2;
	Dictionary<Vector2, TerrainChunk> terrainChunks = new Dictionary<Vector2, TerrainChunk>();


	float _safetyTimer = 0f;
	float _safetyTime = 10f;

	private void Update()
	{
		UpdateVisibleChunks();

		_safetyTimer += Time.deltaTime;
		if (_safetyTimer > _safetyTime)
		{
			_safetyTimer = 0;
			CheckAllChunks();
		}
	}

	private void UpdateVisibleChunks()  //Without loadBoarder, chunks will not unload properly.
	{
		for (int x = -ViewDistance - loadBoarder; x < ViewDistance + loadBoarder; x++)
		{
			for (int y = -ViewDistance - loadBoarder; y < ViewDistance + loadBoarder; y++)
			{
				var chunkCoord = new Vector2(ViewerCoords.x + x, ViewerCoords.y + y);

				if (terrainChunks.ContainsKey(chunkCoord))
				{
					terrainChunks[chunkCoord].UpdateTerrainChunk(ViewerPos / chunkSize);
				}
				else
				{
					terrainChunks.Add(chunkCoord, new TerrainChunk(chunkCoord, this.gameObject));
				}
			}
		}
	}

	private void CheckAllChunks()   //Call this sometimes in case chunk unloading fails due to too fast viewer movement.
	{
		foreach (var chunk in terrainChunks)
		{
			chunk.Value.UpdateTerrainChunk(ViewerPos / chunkSize);
		}
	}

	public class TerrainChunk
	{
		GameObject mesh;
		Vector2 _coords;
		int size = EndlessTerrain.chunkSize;

		public TerrainChunk(Vector2 coords, GameObject parent)
		{
			_coords = coords;
			mesh = GameObject.CreatePrimitive(PrimitiveType.Plane);
			mesh.transform.position = new Vector3(coords.x * size, 0, coords.y * size);
			mesh.transform.localScale = Vector3.one * size / 10f;
			mesh.transform.parent = parent.transform;

			SetVisible(false);
		}

		public void SetVisible(bool visible)
		{
			mesh.SetActive(visible);
		}

		public void UpdateTerrainChunk(Vector2 viewerPos)
		{
			bool visible = Vector2.Distance(_coords, viewerPos) < EndlessTerrain.ViewDistance;
			SetVisible(visible);
		}
	}


}