using GradientUnsafe;
using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour
{
	public static EndlessTerrain Instance { get; private set; }

	public ChunkGeneratorConfig _chunkGeneratorConfig;
	public ChunkGeneratorConfigUnsafe _chunkGeneratorConfigUnsafe;
	public NoiseConfig _noiseConfig;
	private NativeArray<int2> _baseOffsets;

	public GameObject _terrainChunkPrefab;

	[Range(1, 10)]
	public int RenderDistance = 2;
	[field: SerializeField] public Transform Viewer { get; private set; }
	public Vector2 ViewerCoords => new Vector2((int)Viewer.position.x / CHUNK_SIZE, (int)Viewer.position.z / CHUNK_SIZE);
	public Vector2 ViewerPos => new Vector2(Viewer.position.x, Viewer.position.z);

	public const int CHUNK_SIZE = 241;   //TODO sync with ChunkGenerator.CHUNK_SIZE

	[Range(1, 3)]
	public int loadBoarder = 2;
	Dictionary<Vector2, TerrainChunk> terrainChunks = new Dictionary<Vector2, TerrainChunk>();

	float _cleanupTimer = 0f;
	float _cleanupTime = 10f;

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
		//TODO get noiselayers from editor
		_noiseConfig.Layers = new NativeArray<Noise>(1, Allocator.Persistent);
		var noise = new Noise()
		{
			Frequency = 50,
			Amplitude = 30
		};
		_noiseConfig.Layers[0] = noise;

		GenerateOffsets();
	}

	private void Update()
	{
		UpdateVisibleChunks();

		_cleanupTimer += Time.deltaTime;
		if (_cleanupTimer > _cleanupTime)
		{
			_cleanupTimer = 0;
			UpdateAllChunks();
		}
	}

	private void OnValidate()
	{
		if(!UnityEditor.EditorApplication.isPlaying)
		{
			return;
		}

		_chunkGeneratorConfigUnsafe.HeightGradient = _chunkGeneratorConfig.HeightGradient.DirectAccessReadOnly();
		GenerateOffsets();

		foreach (var chunk in terrainChunks)
		{
			var meshData = chunk.Value.GenerateMeshData(_chunkGeneratorConfigUnsafe, _noiseConfig, _baseOffsets);
			chunk.Value.ApplyMeshData(chunk.Value.Mesh, meshData);
		}
		Debug.Log("Validated: " + System.DateTime.Now);
	}

	private void OnDestroy()
	{
		Dispose();
	}

	private void GenerateOffsets()
	{
		if(_baseOffsets.IsCreated)	_baseOffsets.Dispose();//TODO dispose noiseconfig layers and reassign

		_baseOffsets = new NativeArray<int2>(_noiseConfig.Layers.Length, Allocator.Persistent);

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

	private void UpdateVisibleChunks()  //Without loadBoarder, chunks will not unload properly.
	{
		for (int x = -RenderDistance - loadBoarder; x < RenderDistance + loadBoarder; x++)
		{
			for (int y = -RenderDistance - loadBoarder; y < RenderDistance + loadBoarder; y++)
			{
				var chunkCoord = new Vector2(ViewerCoords.x + x, ViewerCoords.y + y);

				if (terrainChunks.ContainsKey(chunkCoord))
				{
					terrainChunks[chunkCoord].UpdateTerrainChunk(ViewerPos / CHUNK_SIZE);
				}
				else
				{
					var newChunk = new TerrainChunk(chunkCoord, this.gameObject, _terrainChunkPrefab);
					var meshData = newChunk.GenerateMeshData(_chunkGeneratorConfigUnsafe, _noiseConfig, _baseOffsets);
					newChunk.ApplyMeshData(newChunk.Mesh, meshData);
					terrainChunks.Add(chunkCoord, newChunk);
				}
			}
		}
	}

	private void UpdateAllChunks()   //Call this sometimes in case chunk unloading fails due to too fast viewer movement.
	{
		foreach (var chunk in terrainChunks)
		{
			chunk.Value.UpdateTerrainChunk(ViewerPos / CHUNK_SIZE);
		}
	}

	public void Dispose()
	{
		if (_baseOffsets.IsCreated) _baseOffsets.Dispose();
		if (_noiseConfig.Layers.IsCreated) _noiseConfig.Dispose();
	}
}
