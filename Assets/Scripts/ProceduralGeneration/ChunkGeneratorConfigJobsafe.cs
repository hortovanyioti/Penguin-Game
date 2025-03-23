using GradientUnsafe;
using System;

[Serializable]
public struct ChunkGeneratorConfigJobsafe
{

	public const int MAX_OFFSET = 100000;

	public bool UseFalloff;
	public float FalloffSlope;
	public float FalloffOffset;

	public int LevelOfDetail;

	public GradientStruct.ReadOnly HeightGradient;
}