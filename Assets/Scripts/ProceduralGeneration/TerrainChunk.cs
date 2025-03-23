using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class TerrainChunk
{
	GameObject meshObj;
	public Mesh Mesh;
	private MeshData _meshData;
	NativeArray<int2> _offsets;
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

		meshObj.transform.position = new Vector3(coords.x * CHUNK_SIZE, 0, coords.y * CHUNK_SIZE);
		_coords = coords;
		SetVisible(false);
	}

	public MeshData GenerateMeshData(ChunkGeneratorConfigJobsafe cgcfg, NoiseConfigJobsafe ncfg, NativeArray<int2> baseOffsets)
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
		var jobHandle = chunkGeneratorJob.Schedule();
		jobHandle.Complete();
		return chunkGeneratorJob.GenerateMeshData();
	}

	public void ApplyMeshData(Mesh mesh, MeshData meshData)
	{
		mesh.vertices = meshData.Vertices.ToArray();
		mesh.uv = meshData.Uvs.ToArray();
		mesh.colors = meshData.Colors.ToArray();
		mesh.triangles = meshData.Triangles.ToArray();

		mesh.RecalculateNormals();
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
		meshObj.SetActive(visible);
	}

	~TerrainChunk()
	{
		if (_offsets.IsCreated) _offsets.Dispose();
		_meshData.Dispose();
	}
}
