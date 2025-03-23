using System;
using Unity.Collections;
using UnityEngine;

[Serializable]
public struct MeshData
{
	public NativeArray<Vector3> Vertices;
	public NativeArray<Vector2> Uvs;
	public NativeArray<Color> Colors;
	public NativeArray<int> Triangles;

	public void Dispose()
	{
		if (Vertices.IsCreated) Vertices.Dispose();
		if (Uvs.IsCreated) Uvs.Dispose();
		if (Colors.IsCreated) Colors.Dispose();
		if (Triangles.IsCreated) Triangles.Dispose();
	}
}