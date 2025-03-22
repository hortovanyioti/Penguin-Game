using System;
using UnityEngine;

using GradientUnsafe;

[Serializable]
public struct ChunkGeneratorConfigUnsafe {

	public const int MAX_OFFSET = 100000;

	public bool UseFalloff;
	public float FalloffSlope;
	public float FalloffOffset;

	[Range(0, LOD.LOD_LEVELS - 1)]
	public int LevelOfDetail;

	[Header("")]
	public GradientStruct.ReadOnly HeightGradient;
}