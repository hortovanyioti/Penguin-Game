using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerraingGenerator : MonoBehaviour
{
	private Mesh Mesh;
	private Vector3[] _vertices;
	private int[] _triangles;
	private Vector2[] _uvs;
	private Color[] _colors;
	private int[] _offsets;
	private const int MAX_OFFSET = 100000;

	private float minTerrainHeight = float.MaxValue;
	private float maxTerrainHeight = float.MinValue;

	public int GridSizeX = 10;
	public int GridSizeZ = 10;
	[Header("Noise")]
	public Noise[] NoiseSettings;
	public float NoiseStrength = 1f;
	public int ManualOffset = 0;
	public bool RandomizeOffset = false;

	[Header("")]
	public Gradient HeightGradient;

	void Start()
	{
		Mesh = GetComponent<MeshFilter>().mesh;
		CreateShape();
		UpdateMesh();
	}

	void Update()
	{
		CreateShape();
		UpdateMesh();
	}

	public void CreateShape()
	{
		GenerateOffsets();
		CreateVertices();
		CreateTriangles();
		CreateUVs();
		CreateColors();
	}

	private void GenerateOffsets()
	{
		_offsets = new int[NoiseSettings.Length];

		if (!RandomizeOffset)
		{
			return;
		}

		for (int i = 0; i < NoiseSettings.Length; i++)
		{
			_offsets[i] = Random.Range(-MAX_OFFSET, MAX_OFFSET);
		}
	}

	private void CreateVertices()
	{
		if (GridSizeX < 1 || GridSizeZ < 1)
		{
			Debug.LogError("Grid size must be at least 1");
			return;
		}

		_vertices = new Vector3[(GridSizeX + 1) * (GridSizeZ + 1)];
		var vertIndex = 0;
		minTerrainHeight = float.MaxValue;
		maxTerrainHeight = float.MinValue;

		for (int z = 0; z < GridSizeZ + 1; z++)
		{
			for (int x = 0; x < GridSizeX + 1; x++)
			{
				var y = GenerateHeight(x, z);
				_vertices[vertIndex] = new Vector3(x, y, z);
				++vertIndex;

				if (y < minTerrainHeight)
				{
					minTerrainHeight = y;
				}
				else if (y > maxTerrainHeight)
				{
					maxTerrainHeight = y;
				}
			}
		}
	}

	private float GenerateHeight(int x, int z)
	{
		var height = 0f;
		for (int i = 0; i < NoiseSettings.Length; i++)
		{
			var finalOffset = _offsets[i] + ManualOffset;
			height += Mathf.PerlinNoise(x / NoiseSettings[i].Frequency + finalOffset, z / NoiseSettings[i].Frequency + finalOffset)
				* NoiseSettings[i].Amplitude;
		}
		return height / NoiseSettings.Length * NoiseStrength;
	}

	/// <summary>
	/// Triangles are defined by 3 vertices. The order of the vertices should be clockwise.
	/// The vertices are stored in a 1D array. 
	/// </summary>
	private void CreateTriangles()
	{
		_triangles = new int[GridSizeX * GridSizeZ * 3 * 2];


		var tileCount = 0;  //Digest in squares, cut into 2 triangles
		var i = -1;

		for (var z = 0; z < GridSizeZ; z++)
		{
			for (var x = 0; x < GridSizeX; x++)
			{
				_triangles[++i] = tileCount + 0;
				_triangles[++i] = tileCount + GridSizeX + 1;
				_triangles[++i] = tileCount + 1;

				_triangles[++i] = tileCount + 1;
				_triangles[++i] = tileCount + GridSizeX + 1;
				_triangles[++i] = tileCount + GridSizeX + 2;

				++tileCount;
			}
			++tileCount;    //Skip the vertex between the rows
		}
	}

	private void CreateUVs()
	{
		var i = 0;
		_uvs = new Vector2[_vertices.Length];
		for (int z = 0; z < GridSizeZ + 1; z++)
		{
			for (int x = 0; x < GridSizeX + 1; x++)
			{
				_uvs[i] = new Vector2((float)x / GridSizeX, (float)z / GridSizeZ);
				++i;
			}
		}
	}

	private void CreateColors()
	{
		var i = 0;
		_colors = new Color[_vertices.Length];
		for (int z = 0; z < GridSizeZ + 1; z++)
		{
			for (int x = 0; x < GridSizeX + 1; x++)
			{
				float normalizedHeight = Mathf.InverseLerp(minTerrainHeight, maxTerrainHeight, _vertices[i].y);
				_colors[i] = HeightGradient.Evaluate(normalizedHeight);
				++i;
			}
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
