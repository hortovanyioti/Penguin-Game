using System.Collections.Generic;
using System.Diagnostics;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class TerrainChunk
{
	const int CHUNK_SIZE = EndlessTerrain.CHUNK_SIZE;
	private static int _renderDistance => EndlessTerrain.Instance.RenderDistance;
	private static int _objectDensitiy = 150;

	public bool MeshDataReady => _isMeshUpdateInProgress && _handle.IsCompleted;

	private bool _isMeshUpdateInProgress = false;
	private bool _objectsGenerated = false;
	private GameObject _meshObj;
	private Mesh _mesh;
	private MeshCollider _meshCollider;
	private MeshData _meshData;
	private NativeArray<int2> _offsets;
	private Vector2 _coords;
	private JobHandle _handle;

	private readonly ChunkGeneratorConfigJobsafe _chunkGeneratorConfigJobsafe;
	private readonly NoiseConfigJobsafe _noiseConfigJobsafe;
	private readonly int2[] _baseOffsets;

	public TerrainChunk(Vector2 coords, GameObject parent, GameObject prefab, ChunkGeneratorConfig cgcfg, NoiseConfig ncfg, int2[] baseOffsets)
	{
		_chunkGeneratorConfigJobsafe = cgcfg.ToJobsafe();
		_noiseConfigJobsafe = ncfg.ToJobsafe();
		_baseOffsets = baseOffsets;

		_meshObj = UnityEngine.Object.Instantiate(prefab, parent.transform);
		_meshObj.name = "Terrain Chunk " + coords.ToString("0");
		_meshCollider = _meshObj.GetComponent<MeshCollider>();
		var meshFilter = _meshObj.GetComponent<MeshFilter>();
		_mesh = meshFilter.sharedMesh;

		if (_mesh == null)
		{
			meshFilter.sharedMesh = _mesh = new Mesh();
		}

		var positionScale = CHUNK_SIZE;

		_meshObj.transform.position = new Vector3(coords.x * positionScale, 0, coords.y * positionScale);
		_coords = coords;
		SetVisible(false);
	}

	public void GenerateMeshData()
	{
		var scaledSize = CHUNK_SIZE / LOD.MeshScale[_chunkGeneratorConfigJobsafe.LevelOfDetail];
		_meshData = new MeshData()
		{
			Vertices = new NativeArray<Vector3>((scaledSize + 1) * (scaledSize + 1), Allocator.Persistent),
			Uvs = new NativeArray<Vector2>((scaledSize + 1) * (scaledSize + 1), Allocator.Persistent),
			Colors = new NativeArray<Color>((scaledSize + 1) * (scaledSize + 1), Allocator.Persistent),
			Triangles = new NativeArray<int>(scaledSize * scaledSize * 6, Allocator.Persistent)
		};

		CalculateOffsets(_baseOffsets);

		var chunkGeneratorJob = new ChunkGeneratorJob(_meshData, _chunkGeneratorConfigJobsafe, _noiseConfigJobsafe, _offsets);
		_handle = chunkGeneratorJob.Schedule();
		_isMeshUpdateInProgress = true;
	}

	public void ApplyMeshData()
	{
		if (_handle == null || !_handle.IsCompleted) return;

		_handle.Complete();

		_mesh.vertices = _meshData.Vertices.ToArray();
		_mesh.uv = _meshData.Uvs.ToArray();
		_mesh.colors = _meshData.Colors.ToArray();
		_mesh.triangles = _meshData.Triangles.ToArray();

		_mesh.RecalculateNormals();
		_meshCollider.sharedMesh = _mesh;

		_isMeshUpdateInProgress = false;
	}

	private void CalculateOffsets(int2[] baseOffsets)
	{
		_offsets = new NativeArray<int2>(baseOffsets.Length, Allocator.Persistent);

		for (int i = 0; i < baseOffsets.Length; i++)
		{
			_offsets[i] = new int2(
				baseOffsets[i].x + (int)_coords.x * CHUNK_SIZE,
				baseOffsets[i].y + (int)_coords.y * CHUNK_SIZE
			);
		}
	}

	public void GenerateObjects(int seed)
	{
		var maxScale = new float2x3(
			0.8f, 1.2f,
			0.8f, 1.2f,
			0.8f, 1.2f);

		var rng = new Unity.Mathematics.Random();
		rng.InitState((uint)seed);

		for (int i = 0; i < _objectDensitiy; i++)
		{
			var x = rng.NextFloat(CHUNK_SIZE);
			var y = rng.NextFloat(CHUNK_SIZE);
			var rayOrigin = new Vector3(x, _noiseConfigJobsafe.MaxHeight + 1, y) + _meshObj.transform.position;

			if (!Physics.Raycast(rayOrigin, Vector3.down, out var hit, Mathf.Infinity) || !hit.transform.CompareTag("Ground"))
			{
				UnityEngine.Debug.DrawRay(rayOrigin, Vector3.down * 1000, Color.red, 10);
				continue;
			}

			var biome = HeightBiomeMap.GetBiome(hit.point.y);

			var biomePrefabCount = BiomePrefabDictionary.Instance.BiomePrefabs[biome].Count;
			if (biomePrefabCount == 0)
			{
				UnityEngine.Debug.LogWarning($"No prefabs for biome {biome}");
				continue;
			}

			var prefabIndex = rng.NextInt(biomePrefabCount);
			var prefab = BiomePrefabDictionary.Instance.BiomePrefabs[biome][prefabIndex];
			var go = UnityEngine.Object.Instantiate(prefab);
			var transform = go.transform;

			transform.SetParent(_meshObj.transform);
			transform.localScale = new Vector3(
				rng.NextFloat(maxScale.c0.x, maxScale.c0.y),
				rng.NextFloat(maxScale.c1.x, maxScale.c1.y),
				rng.NextFloat(maxScale.c2.x, maxScale.c2.y)
				);

			transform.SetLocalPositionAndRotation(hit.point - _meshCollider.transform.position, Quaternion.Euler(0, rng.NextFloat(360f), 0));
		}
	}

	public void UpdateTerrainChunk(Vector2 viewerPos)
	{
		bool visible = Vector2.Distance(_coords, viewerPos) < _renderDistance;
		SetVisible(visible);
	}

	public void SetVisible(bool visible)
	{
		if (_meshObj.gameObject.activeSelf == visible)
		{
			return;
		}

		_meshObj.SetActive(visible);
		if (!_objectsGenerated && visible)
		{
			GenerateObjects(_noiseConfigJobsafe.Seed);
			_objectsGenerated = true;
		}
	}

	~TerrainChunk()
	{
		if (_offsets.IsCreated) _offsets.Dispose();
		_meshData.Dispose();
		_noiseConfigJobsafe.Dispose();
	}
}
