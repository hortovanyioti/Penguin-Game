using System;
using UnityEngine;

[Serializable]
public class MeshData
{
	public Vector3[] Vertices { get; set; }
	public Vector2[] Uvs { get; set; }
	public Color[] Colors { get; set; }
	public int[] Triangles { get; set; }
}