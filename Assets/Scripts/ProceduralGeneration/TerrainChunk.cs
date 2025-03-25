using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class TerrainChunk
{
	const int CHUNK_SIZE = EndlessTerrain.CHUNK_SIZE;
	private static int _renderDistance => EndlessTerrain.Instance.RenderDistance;

	public bool HasNewMeshData = false;

	private GameObject meshObj;
	private Mesh _mesh;
	private MeshData _meshData;
	private NativeArray<int2> _offsets;
	private Vector2 _coords;
	private JobHandle _handle;

	public TerrainChunk(Vector2 coords, GameObject parent, GameObject prefab)
	{
		meshObj = UnityEngine.Object.Instantiate(prefab, parent.transform);
		meshObj.name = "Terrain Chunk " + coords.ToString("0");
		var meshFilter = meshObj.GetComponent<MeshFilter>();
		_mesh = meshFilter.sharedMesh;

		if (_mesh == null)
		{
			meshFilter.sharedMesh = _mesh = new Mesh();
		}

		meshObj.transform.position = new Vector3(coords.x * CHUNK_SIZE, 0, coords.y * CHUNK_SIZE);
		_coords = coords;
		SetVisible(false);
	}

	public void GenerateMeshData(ChunkGeneratorConfigJobsafe cgcfg, NoiseConfigJobsafe ncfg, NativeArray<int2> baseOffsets)
	{
		var scaledSize = CHUNK_SIZE / LOD.MeshScale[cgcfg.LevelOfDetail];
		_meshData = new MeshData()
		{
			Vertices = new NativeArray<Vector3>((scaledSize + 1) * (scaledSize + 1), Allocator.Persistent),
			Uvs = new NativeArray<Vector2>((scaledSize + 1) * (scaledSize + 1), Allocator.Persistent),
			Colors = new NativeArray<Color>((scaledSize + 1) * (scaledSize + 1), Allocator.Persistent),
			Triangles = new NativeArray<int>(scaledSize * scaledSize * 6, Allocator.Persistent)
		};

		CalculateOffsets(baseOffsets);

		var chunkGeneratorJob = new ChunkGeneratorJob(_meshData, cgcfg, ncfg, _offsets);
		_handle = chunkGeneratorJob.Schedule();
		HasNewMeshData = true;
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
		HasNewMeshData = false;
	}

	private void CalculateOffsets(NativeArray<int2> baseOffsets)
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

	public void UpdateTerrainChunk(Vector2 viewerPos)
	{
		bool visible = Vector2.Distance(_coords, viewerPos) < _renderDistance;
		SetVisible(visible);
	}

	public void SetVisible(bool visible)
	{
		if (meshObj.gameObject.activeSelf != visible)
		{
			meshObj.SetActive(visible);
		}
	}

	~TerrainChunk()
	{
		if (_offsets.IsCreated) _offsets.Dispose();
		_meshData.Dispose();
	}
}
