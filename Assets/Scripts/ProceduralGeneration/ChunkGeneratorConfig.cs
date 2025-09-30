using System;
using UnityEngine;

[Serializable]
public class ChunkGeneratorConfig
{

	public static int MAX_OFFSET => ChunkGeneratorConfigJobsafe.MAX_OFFSET;

	public bool UseFalloff;
	public float FalloffSlope;
	public float FalloffOffset;

	[Range(0, LOD.LOD_LEVELS - 1)]
	public int LevelOfDetail;

	[Header("")]
	public Gradient HeightGradient;
}