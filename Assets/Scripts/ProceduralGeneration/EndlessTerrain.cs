using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SystemMemoryUsage;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Profiling;
public class EndlessTerrain : MonoBehaviour
{
	public static EndlessTerrain Instance { get; private set; }

	public ChunkGeneratorConfig _chunkGeneratorConfig;
	public NoiseConfig _noiseConfig;
	[SerializeField] private Vector2 _appMemoryBounds = new Vector2(2048f, 4096f);
	[SerializeField] private float _minFreeSystemMemory = 1024f;

	private int2[] _baseOffsets;

	[SerializeField] GameObject _terrainChunkPrefab;
	[SerializeField] GameObject _navMeshLinkPrefab;

	[Range(1, 10)]
	public int RenderDistance = 2;
	[field: SerializeField] public Transform Viewer { get; private set; }
	public int2 ViewerCoords => new int2((int)Viewer.position.x / CHUNK_SIZE, (int)Viewer.position.z / CHUNK_SIZE);
	public Vector2 ViewerPos => new Vector2(Viewer.position.x, Viewer.position.z);

	public const int CHUNK_SIZE = 240;   //TODO sync with ChunkGenerator.CHUNK_SIZE

	[Range(1, 3)]
	public int loadBoarder = 1;

	Dictionary<int2, TerrainChunk> terrainChunks = new Dictionary<int2, TerrainChunk>();
	Dictionary<int2x2, NavMeshLinkChain> navMeshLinks = new Dictionary<int2x2, NavMeshLinkChain>();

	float _cleanupTimer = 0f;
	float _cleanupTime = 5f;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			return;
		}
		Destroy(this);
	}

	private void Start()
	{
		GenerateOffsets();
	}

	private void Update()
	{
		UpdateVisibleChunks();

		_cleanupTimer += Time.deltaTime;
		if (_cleanupTimer > _cleanupTime)
		{
			_cleanupTimer = 0;
			UpdateVisibilityAllChunks();
			MemoryCheck();
		}
	}

	private void LateUpdate()
	{
		ApplyNextMeshData();
		GenerateNavMeshLinks();
	}

	private void OnValidate()
	{
		if (!UnityEditor.EditorApplication.isPlaying)
		{
			return;
		}

		GenerateOffsets();

		foreach (var chunk in terrainChunks)
		{
			chunk.Value.GenerateMeshData();
		}
		UnityEngine.Debug.Log("Validated: " + System.DateTime.Now);
	}

	private void GenerateOffsets()
	{
		_baseOffsets = new int2[_noiseConfig.Layers.Length];

		if (!_noiseConfig.RandomizeOffset)
		{
			return;
		}

		for (int i = 0; i < _noiseConfig.Layers.Length; i++)
		{
			_baseOffsets[i] = new int2(
				UnityEngine.Random.Range(-ChunkGeneratorConfig.MAX_OFFSET, ChunkGeneratorConfig.MAX_OFFSET),
				UnityEngine.Random.Range(-ChunkGeneratorConfig.MAX_OFFSET, ChunkGeneratorConfig.MAX_OFFSET)
			);
		}
	}

	private void GenerateNavMeshLinks()
	{
		foreach (var chunk in terrainChunks)
		{
			if (chunk.Value.State == ChunkState.GeneratedNavMesh)
			{
				var chunkKeys = new int2[4] {
					new int2(chunk.Key.x + 1, chunk.Key.y),
					new int2(chunk.Key.x - 1, chunk.Key.y),
					new int2(chunk.Key.x, chunk.Key.y + 1),
					new int2(chunk.Key.x, chunk.Key.y - 1)
				};

				//Smaller key always first
				var linkKeys = new int2x2[4] {
					new int2x2(chunk.Key, chunkKeys[0]),
					new int2x2(chunkKeys[1], chunk.Key),
					new int2x2(chunk.Key, chunkKeys[2]),
					new int2x2(chunkKeys[3], chunk.Key)
				};

				for (int i = 0; i < chunkKeys.Length; i++)
				{
					//If the next chunk exists and navmesh is generated, and the link does not exist
					if (terrainChunks.TryGetValue(chunkKeys[i], out var nextChunk) &&
						nextChunk.State >= ChunkState.GeneratedNavMesh &&
						!navMeshLinks.TryGetValue(linkKeys[i], out var link))
					{
						//Create a new link
						var newLink = Instantiate(_navMeshLinkPrefab, this.transform).GetComponent<NavMeshLinkChain>();
						newLink.SetCoords(linkKeys[i]);
						newLink.Bake();
						navMeshLinks.Add(linkKeys[i], newLink);

						switch (i)
						{
							case 0:
								chunk.Value.NavMeshLinks.Right = newLink;
								nextChunk.NavMeshLinks.Left = newLink;
								break;
							case 1:
								chunk.Value.NavMeshLinks.Left = newLink;
								nextChunk.NavMeshLinks.Right = newLink;
								break;
							case 2:
								chunk.Value.NavMeshLinks.Forward = newLink;
								nextChunk.NavMeshLinks.Backward = newLink;
								break;
							case 3:
								chunk.Value.NavMeshLinks.Backward = newLink;
								nextChunk.NavMeshLinks.Forward = newLink;
								break;
						}
					}
				}

				chunk.Value.ForceState(ChunkState.GeneratedNavMeshLinks);
			}
		}
	}
	private void ApplyNextMeshData()
	{
		//Only one chunk per frame
		for (int i = 0; i < terrainChunks.Count; i++)
		{
			var chunk = terrainChunks.ElementAt(i).Value;
			if (chunk.State == ChunkState.MeshGenerated)
			{
				chunk.ApplyMeshData();
				return;
			}
		}
	}

	private void UpdateVisibleChunks()  //Without loadBoarder, chunks will not unload properly.
	{
		for (int x = -RenderDistance - loadBoarder; x < RenderDistance + loadBoarder; x++)
		{
			for (int y = -RenderDistance - loadBoarder; y < RenderDistance + loadBoarder; y++)
			{
				var chunkCoord = new int2(ViewerCoords.x + x, ViewerCoords.y + y);

				if (terrainChunks.ContainsKey(chunkCoord))
				{
					terrainChunks[chunkCoord].UpdateVisibility(ViewerPos / CHUNK_SIZE);
				}
				else
				{
					var newChunk = Instantiate(_terrainChunkPrefab, this.transform).GetComponent<TerrainChunk>();
					newChunk.Init(chunkCoord, this.gameObject, _terrainChunkPrefab, _chunkGeneratorConfig, _noiseConfig, _baseOffsets);
					newChunk.GenerateMeshData();
					terrainChunks.Add(chunkCoord, newChunk);
				}
			}
		}
	}

	private void UpdateVisibilityAllChunks()   //Call this sometimes in case chunk unloading fails due to too fast viewer movement.
	{
		foreach (var chunk in terrainChunks)
		{
			chunk.Value.UpdateVisibility(ViewerPos / CHUNK_SIZE);
		}
	}

	private void MemoryCheck()
	{
		var totalUsedMemoryMB = (Profiler.GetTotalAllocatedMemoryLong() / (1024f * 1024f));
		var totalReservedMemoryMB = (Profiler.GetTotalReservedMemoryLong() / (1024f * 1024f));
		var totalUnusedReservedMemoryMB = (Profiler.GetTotalUnusedReservedMemoryLong() / (1024f * 1024f));
		//UnityEngine.Debug.Log($"Total Allocated Memory: {totalUsedMemoryMB} MB | Total Reserved Memory {totalReservedMemoryMB} | Total Unused Reserved Memory {totalUnusedReservedMemoryMB}");

		MemoryInfo.GetMemoryStatus(out var systemTotalMemory, out var systemAvailableMemory);
		//UnityEngine.Debug.Log($"Total Memory: {(int)systemTotalMemory} MB");
		//UnityEngine.Debug.Log($"Available Memory: {(int)systemAvailableMemory} MB");

		if (totalUsedMemoryMB > _appMemoryBounds.y ||
			(systemAvailableMemory < _minFreeSystemMemory && totalUsedMemoryMB > _appMemoryBounds.x))
		{
			var unusedChunkCoords = new List<int2>();
			foreach (var chunk in terrainChunks)
			{
				var diff = chunk.Key - ViewerCoords;
				var squreDistance = diff.x * diff.x + diff.y * diff.y;
				if (squreDistance > 100f)//TODO change this to dynamic variable
				{
					unusedChunkCoords.Add(chunk.Key);
				}
			}
			foreach (var coord in unusedChunkCoords)
			{
				terrainChunks.Remove(coord);
				Destroy(terrainChunks[coord].gameObject);
			}
		}
	}
}
