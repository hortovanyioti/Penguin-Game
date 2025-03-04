using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerraingGenerator : MonoBehaviour
{
	private Mesh Mesh;
	private Vector3[] _vertices;
	private Vector2[] _uvs;
	private Color[] _colors;
	private int[] _triangles;
	private int[] _offsets;
	private const int MAX_OFFSET = 100000;

	private float _minTerrainHeight = float.MaxValue;
	private float _maxTerrainHeight = float.MinValue;

	public const int CHUNK_SIZE = LOD.CHUNK_SIZE;

	[Range(0, LOD.LOD_LEVELS - 1)]
	public int LevelOfDetail = 0;

	[Header("Noise")]
	public Noise[] NoiseLayers;
	public float NoiseStrength = 1f;
	public int Seed = 0;
	public bool RandomizeOffset = false;

	public bool UseFalloff = true;
	public float FalloffSlope = 1f;
	public float FalloffOffset = 1f;

	[Header("")]
	public Gradient HeightGradient;

	void Start()
	{
		Mesh = GetComponent<MeshFilter>().mesh;
		CreateShape();
		UpdateMesh();
	}

	float updateTimer = 0;
	void Update()
	{
		updateTimer += Time.deltaTime;
		if (updateTimer < 1f)
		{
			return;
		}
		updateTimer = 0;

		CreateShape();
		UpdateMesh();
	}

	public void CreateShape()
	{
		GenerateOffsets();
		CreateVertices();
		CreateTriangles();
	}

	private void GenerateOffsets()
	{
		_offsets = new int[NoiseLayers.Length];

		if (!RandomizeOffset)
		{
			return;
		}

		for (int i = 0; i < NoiseLayers.Length; i++)
		{
			_offsets[i] = UnityEngine.Random.Range(-MAX_OFFSET, MAX_OFFSET);
		}
	}

	private float[,] GenerateHeightMap()
	{
		var _falloffMap = new float[0, 0];
		var gridSize = CHUNK_SIZE + 1;
		var heightMap = new float[gridSize, gridSize];

		_minTerrainHeight = float.MaxValue;
		_maxTerrainHeight = float.MinValue;

		if (UseFalloff)
		{
			FalloffGenerator.FalloffSlope = FalloffSlope;
			FalloffGenerator.FalloffOffset = FalloffOffset;
			_falloffMap = FalloffGenerator.GenerateFalloffMap(gridSize, gridSize);
		}

		for (int z = 0; z < gridSize; z++)
		{
			for (int x = 0; x < gridSize; x++)
			{
				var y = GenerateHeight(x, z);
				y *= UseFalloff ? _falloffMap[x, z] : 1f;
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

		if (NoiseLayers.Length == 0)
		{
			Debug.LogWarning("NoiseLayers.Length = 0");
			return 1f;
		}

		for (int i = 0; i < NoiseLayers.Length; i++)
		{
			var finalOffset = _offsets[i] + Seed;
			height += Mathf.PerlinNoise(x / NoiseLayers[i].Frequency + finalOffset, z / NoiseLayers[i].Frequency + finalOffset)
				* NoiseLayers[i].Amplitude;
		}

		return height / NoiseLayers.Length * NoiseStrength;
	}

	private void CreateVertices()
	{
		var scaledChunkSize = CHUNK_SIZE / LOD.MeshScale[LevelOfDetail] + 1;
		_vertices = new Vector3[scaledChunkSize * scaledChunkSize];
		_uvs = new Vector2[_vertices.Length];
		_colors = new Color[_vertices.Length];
		var heightMap = GenerateHeightMap();

		var vertIndex = 0;

		for (int z = 0; z < CHUNK_SIZE + 1; z += LOD.MeshScale[LevelOfDetail])
		{
			for (int x = 0; x < CHUNK_SIZE + 1; x += LOD.MeshScale[LevelOfDetail])
			{
				//vertices
				var y = heightMap[x, z];
				_vertices[vertIndex] = new Vector3(x, y, z);

				//uvs
				_uvs[vertIndex] = new Vector2((float)x / CHUNK_SIZE, (float)z / CHUNK_SIZE);

				//colors
				float normalizedHeight = Mathf.InverseLerp(_minTerrainHeight, _maxTerrainHeight, _vertices[vertIndex].y);
				_colors[vertIndex] = HeightGradient.Evaluate(normalizedHeight);

				++vertIndex;
			}
		}
	}


	private void CreateTriangles()
	{
		var gridSize = CHUNK_SIZE / LOD.MeshScale[LevelOfDetail];
		_triangles = new int[gridSize * gridSize * 3 * 2];

		var tileCount = 0;
		var i = 0;

		for (var z = 0; z < gridSize; z++)
		{
			for (var x = 0; x < gridSize; x++)
			{
				_triangles[i++] = tileCount + 0;
				_triangles[i++] = tileCount + gridSize + 1;
				_triangles[i++] = tileCount + 1;

				_triangles[i++] = tileCount + 1;
				_triangles[i++] = tileCount + gridSize + 1;
				_triangles[i++] = tileCount + gridSize + 2;

				++tileCount;
			}
			++tileCount;
		}
	}

	public void UpdateMesh()
	{
		Mesh.Clear();

		Mesh.vertices = _vertices;
		Mesh.triangles = _triangles;
		//Mesh.uv = _uvs;
		Mesh.colors = _colors;

		Mesh.RecalculateNormals();
	}
}
