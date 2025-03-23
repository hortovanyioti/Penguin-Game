using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

[BurstCompile]
public struct ChunkGeneratorJob : IJob
{
	private MeshData _meshData;
	[ReadOnly]
	private NativeArray<int2> _offsets;

	private float _minTerrainHeight;
	private float _maxTerrainHeight;

	public const int CHUNK_SIZE = 241;  //DO NOT CHANGE

	[ReadOnly]
	private ChunkGeneratorConfigJobsafe _config;

	[ReadOnly]
	private NoiseConfigJobsafe _noiseConfig;

	public ChunkGeneratorJob(MeshData meshData, ChunkGeneratorConfigJobsafe cfg, NoiseConfigJobsafe ncfg, NativeArray<int2> offsets)
	{
		_meshData = meshData;
		_minTerrainHeight = float.MaxValue;
		_maxTerrainHeight = float.MinValue;

		_config = cfg;
		_noiseConfig = ncfg;
		_offsets = offsets;
	}

	public void Execute()
	{
		GenerateMeshData();
	}

	public MeshData GenerateMeshData()
	{
		CreateVertices();
		CreateTriangles();
		return _meshData;
	}

	private NativeArray<float> GenerateHeightMap()//TODO bust not compatible with 2d array
	{
		var gridSize = CHUNK_SIZE + 1;
		var _falloffMap = new NativeArray<float>(gridSize * gridSize, Allocator.Temp);
		var heightMap = new NativeArray<float>(gridSize * gridSize, Allocator.Temp);

		_minTerrainHeight = float.MaxValue;
		_maxTerrainHeight = float.MinValue;

		if (_config.UseFalloff)
		{
			_falloffMap = FalloffGenerator.GenerateFalloffMap(_config.FalloffSlope, _config.FalloffOffset, gridSize, gridSize);
		}

		for (int z = 0; z < gridSize; z++)
		{
			for (int x = 0; x < gridSize; x++)
			{
				var y = GenerateHeight(x, z);
				y *= _config.UseFalloff ? _falloffMap[x * gridSize + z] : 1f;
				heightMap[x * gridSize + z] = y;

				if (y < _minTerrainHeight)
				{
					_minTerrainHeight = y;
				}
				else if (y > _maxTerrainHeight)
				{
					_maxTerrainHeight = y;
				}
			}
		}
		return heightMap;
	}

	private float GenerateHeight(int x, int z)
	{
		var height = 0f;

		if (_noiseConfig.Layers.Length == 0)
		{
			return 1f;
		}

		for (int i = 0; i < _noiseConfig.Layers.Length; i++)
		{
			var offsetX = _offsets[i].x + _noiseConfig.Seed;
			var offsetZ = _offsets[i].y + _noiseConfig.Seed;
			var frequency = _noiseConfig.Layers[i].Frequency;
			height += Mathf.PerlinNoise((x + offsetX) / frequency, (z + offsetZ) / frequency)
				* _noiseConfig.Layers[i].Amplitude;
		}

		return height * _noiseConfig.NoiseStrength;
	}

	private void CreateVertices()
	{
		var heightMap = GenerateHeightMap();
		var vertIndex = 0;
		var gridSize = CHUNK_SIZE + 1;

		for (int z = 0; z < gridSize; z += LOD.MeshScale[_config.LevelOfDetail])
		{
			for (int x = 0; x < gridSize; x += LOD.MeshScale[_config.LevelOfDetail])
			{
				//vertices
				var y = heightMap[x * gridSize + z];
				_meshData.Vertices[vertIndex] = new Vector3(x, y, z);

				//uvs
				_meshData.Uvs[vertIndex] = new Vector2((float)x / CHUNK_SIZE, (float)z / CHUNK_SIZE);

				//colors
				float normalizedHeight = Mathf.InverseLerp(_noiseConfig.MinHeight, _noiseConfig.MaxHeight, _meshData.Vertices[vertIndex].y);

				var color = _config.HeightGradient.Evaluate(normalizedHeight);
				_meshData.Colors[vertIndex] = new Color(color.x, color.y, color.z);

				++vertIndex;
			}
		}
	}

	private void CreateTriangles()
	{
		var gridSize = CHUNK_SIZE / LOD.MeshScale[_config.LevelOfDetail];
		var tileCount = 0;
		var i = 0;

		for (var z = 0; z < gridSize; z++)
		{
			for (var x = 0; x < gridSize; x++)
			{
				_meshData.Triangles[i++] = tileCount + 0;
				_meshData.Triangles[i++] = tileCount + gridSize + 1;
				_meshData.Triangles[i++] = tileCount + 1;

				_meshData.Triangles[i++] = tileCount + 1;
				_meshData.Triangles[i++] = tileCount + gridSize + 1;
				_meshData.Triangles[i++] = tileCount + gridSize + 2;

				++tileCount;
			}
			++tileCount;
		}
	}
}
