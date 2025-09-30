using System;
using Unity.Collections;

[Serializable]
public struct NoiseConfigJobsafe
{
	public NativeArray<Noise> Layers;
	public float NoiseStrength;
	public int Seed;
	public bool RandomizeOffset;
	public float MinHeight;
	public float MaxHeight;


	public bool IsCreated => Layers.IsCreated;
	public void Dispose()
	{
		Layers.Dispose();
	}
}
