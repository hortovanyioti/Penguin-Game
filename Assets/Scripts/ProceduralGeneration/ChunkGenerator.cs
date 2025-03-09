using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;

public class ChunkGenerator
{
	private MeshData _meshData = new MeshData();
	private int[,] _offsets;

	private float _minTerrainHeight = float.MaxValue;
	private float _maxTerrainHeight = float.MinValue;

	public const int CHUNK_SIZE = 241;  //DO NOT CHANGE

	private ChunkGeneratorConfig _config { get; }
	private NoiseConfig _noiseConfig;

	public ChunkGenerator(ChunkGeneratorConfig cfg, NoiseConfig ncfg, int[,] offsets)
	{
		_config = cfg;
		_noiseConfig = ncfg;
		_offsets = offsets;
	}

	public MeshData GenerateMeshData()
	{
		CreateVertices();
		CreateTriangles();
		return _meshData;
	}


	private float[,] GenerateHeightMap()
	{
		var _falloffMap = new float[0, 0];
		var gridSize = CHUNK_SIZE + 1;
		var heightMap = new float[gridSize, gridSize];

		_minTerrainHeight = float.MaxValue;
		_maxTerrainHeight = float.MinValue;

		if (_config.UseFalloff)
		{
			FalloffGenerator.FalloffSlope = _config.FalloffSlope;
			FalloffGenerator.FalloffOffset = _config.FalloffOffset;
			_falloffMap = FalloffGenerator.GenerateFalloffMap(gridSize, gridSize);
		}

		for (int z = 0; z < gridSize; z++)
		{
			for (int x = 0; x < gridSize; x++)
			{
				var y = GenerateHeight(x, z);
				y *= _config.UseFalloff ? _falloffMap[x, z] : 1f;
				heightMap[x, z] = y;

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
			Debug.LogWarning("NoiseLayers.Length = 0");
			return 1f;
		}

		for (int i = 0; i < _noiseConfig.Layers.Length; i++)
		{
			var offsetX = _offsets[i, 0] + _noiseConfig.Seed;
			var offsetZ = _offsets[i, 1] + _noiseConfig.Seed;
			var frequency = _noiseConfig.Layers[i].Frequency;
			height += Mathf.PerlinNoise((x + offsetX) / frequency, (z + offsetZ) / frequency)
				* _noiseConfig.Layers[i].Amplitude;
		}

		return height * _noiseConfig.NoiseStrength;
	}

	private void CreateVertices()
	{
		var scaledChunkSize = CHUNK_SIZE / LOD.MeshScale[_config.LevelOfDetail] + 1;
		_meshData.Vertices = new Vector3[scaledChunkSize * scaledChunkSize];
		_meshData.Uvs = new Vector2[_meshData.Vertices.Length];
		_meshData.Colors = new Color[_meshData.Vertices.Length];
		var heightMap = GenerateHeightMap();

		var vertIndex = 0;

		for (int z = 0; z < CHUNK_SIZE + 1; z += LOD.MeshScale[_config.LevelOfDetail])
		{
			for (int x = 0; x < CHUNK_SIZE + 1; x += LOD.MeshScale[_config.LevelOfDetail])
			{
				//vertices
				var y = heightMap[x, z];
				_meshData.Vertices[vertIndex] = new Vector3(x, y, z);

				//uvs
				_meshData.Uvs[vertIndex] = new Vector2((float)x / CHUNK_SIZE, (float)z / CHUNK_SIZE);

				//colors
				float normalizedHeight = Mathf.InverseLerp(_noiseConfig.MinHeight, _noiseConfig.MaxHeight, _meshData.Vertices[vertIndex].y);
				_meshData.Colors[vertIndex] = _config.HeightGradient.Evaluate(normalizedHeight);

				++vertIndex;
			}
		}
	}

	private void CreateTriangles()
	{
		var gridSize = CHUNK_SIZE / LOD.MeshScale[_config.LevelOfDetail];
		_meshData.Triangles = new int[gridSize * gridSize * 3 * 2];

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
