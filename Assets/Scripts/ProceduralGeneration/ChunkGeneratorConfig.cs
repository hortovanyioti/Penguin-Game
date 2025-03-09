using System;
using UnityEngine;

[Serializable]
public class ChunkGeneratorConfig {

	public const int MAX_OFFSET = 100000;

	public bool UseFalloff = false;
	public float FalloffSlope = 1f;
	public float FalloffOffset = 1f;

	[Range(0, LOD.LOD_LEVELS - 1)]
	public int LevelOfDetail = 0;

	[Header("")]
	public Gradient HeightGradient;
}