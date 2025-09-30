using GradientUnsafe;
using Unity.Collections;

public static class Extensions
{
	public static ChunkGeneratorConfigJobsafe ToJobsafe(this ChunkGeneratorConfig cgcfg)
	{
		return new ChunkGeneratorConfigJobsafe
		{
			UseFalloff = cgcfg.UseFalloff,
			FalloffSlope = cgcfg.FalloffSlope,
			FalloffOffset = cgcfg.FalloffOffset,
			LevelOfDetail = cgcfg.LevelOfDetail,
			HeightGradient = cgcfg.HeightGradient.DirectAccessReadOnly()
		};
	}

	public static NoiseConfigJobsafe ToJobsafe(this NoiseConfig nc)
	{
		return new NoiseConfigJobsafe
		{
			Layers = new NativeArray<Noise>(nc.Layers, Allocator.Persistent),
			NoiseStrength = nc.NoiseStrength,
			Seed = nc.Seed,
			RandomizeOffset = nc.RandomizeOffset,
			MinHeight = nc.MinHeight,
			MaxHeight = nc.MaxHeight
		};
	}
}