using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.UIElements;

public class EndlessTerrain : MonoBehaviour
{
	public static EndlessTerrain Instance { get; private set; }

	public ChunkGeneratorConfig _chunkGeneratorConfig;
	public NoiseConfig _noiseConfig;
	private int[,] _baseOffsets;

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
		GenerateOffsets();

		foreach (var chunk in terrainChunks)
		{
			var meshData = chunk.Value.GenerateMeshData(_chunkGeneratorConfig, _noiseConfig, _baseOffsets);
			chunk.Value.ApplyMeshData(chunk.Value.Mesh, meshData);
		}
		Debug.Log("Validated: " + System.DateTime.Now);
	}
	private void GenerateOffsets()
	{
		_baseOffsets = new int[_noiseConfig.Layers.Length, 2];

		if (!_noiseConfig.RandomizeOffset)
		{
			return;
		}

		for (int i = 0; i < _noiseConfig.Layers.Length; i++)
		{
			_baseOffsets[i, 0] = UnityEngine.Random.Range(-ChunkGeneratorConfig.MAX_OFFSET, ChunkGeneratorConfig.MAX_OFFSET);
			_baseOffsets[i, 1] = UnityEngine.Random.Range(-ChunkGeneratorConfig.MAX_OFFSET, ChunkGeneratorConfig.MAX_OFFSET);
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
					var meshData = newChunk.GenerateMeshData(_chunkGeneratorConfig, _noiseConfig, _baseOffsets);
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
}

public class TerrainChunk
{
	GameObject meshObj;
	public Mesh Mesh;
	int[,] _offsets;
	int _renderDistance => EndlessTerrain.Instance.RenderDistance;
	Vector2 _coords;
	const int CHUNK_SIZE = EndlessTerrain.CHUNK_SIZE;

	public TerrainChunk(Vector2 coords, GameObject parent, GameObject prefab)
	{
		meshObj = UnityEngine.Object.Instantiate(prefab, parent.transform);
		meshObj.name = "Terrain Chunk " + coords.ToString("0");
		var meshFilter = meshObj.GetComponent<MeshFilter>();
		Mesh = meshFilter.sharedMesh;

		if (Mesh == null)
		{
			meshFilter.sharedMesh = Mesh = new Mesh();
		}

		/* UNDONE
		var terraingGeneratorJob = new ChunkGeneratorJob(cgcfg, ncfg, offsets);
		var jobHandle = terraingGeneratorJob.Schedule();
		jobHandle.Complete();

		ApplyMeshData(mesh, terraingGeneratorJob.MeshData);
		*/

		meshObj.transform.position = new Vector3(coords.x * CHUNK_SIZE, 0, coords.y * CHUNK_SIZE);
		//meshObj.transform.localScale = Vector3.one * size / 10f;

		_coords = coords;
		SetVisible(false);
	}

	public MeshData GenerateMeshData(ChunkGeneratorConfig cgcfg, NoiseConfig ncfg, int[,] baseOffsets)
	{
		CalculateOffsets(baseOffsets);
		var chunkGenerator = new ChunkGenerator(cgcfg, ncfg, _offsets);
		return chunkGenerator.GenerateMeshData();
	}

	public void ApplyMeshData(Mesh mesh, MeshData meshData)
	{
		mesh.vertices = meshData.Vertices;
		//mesh.uv = meshData.Uvs;//UNDONE
		mesh.colors = meshData.Colors;
		mesh.triangles = meshData.Triangles;

		mesh.RecalculateNormals();
	}

	private void CalculateOffsets(int[,] _baseOffsets)
	{
		var xlength = _baseOffsets.GetLength(0);
		var ylength = _baseOffsets.GetLength(1);
		_offsets = new int[xlength, ylength];

		for (int i = 0; i < xlength; i++)
		{
			_offsets[i, 0] = _baseOffsets[i, 0] + (int)_coords.x * CHUNK_SIZE;
			_offsets[i, 1] = _baseOffsets[i, 1] + (int)_coords.y * CHUNK_SIZE;
		}
	}

	public void UpdateTerrainChunk(Vector2 viewerPos)
	{
		bool visible = Vector2.Distance(_coords, viewerPos) < _renderDistance;
		SetVisible(visible);
	}
	public void SetVisible(bool visible)
	{
		meshObj.SetActive(visible);
	}
}

struct TerrainChunkCreationJob : IJobParallelFor //TODO
{
	[ReadOnly] public NativeArray<Vector2> chunkCoords;
	public int chunkSize;
	public GameObject parent;

	public void Execute(int index)
	{
		var chunkCoord = chunkCoords[index];
		// Here you can call the TerrainGenerator to generate the terrain data
		// For simplicity, we are just creating a primitive mesh
		var mesh = GameObject.CreatePrimitive(PrimitiveType.Plane);
		mesh.transform.position = new Vector3(chunkCoord.x * chunkSize, 0, chunkCoord.y * chunkSize);
		mesh.transform.localScale = Vector3.one * chunkSize / 10f;
		mesh.transform.parent = parent.transform;
	}
}