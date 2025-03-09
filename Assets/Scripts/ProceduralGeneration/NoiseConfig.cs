using System;
using UnityEngine;

[Serializable]
public class NoiseConfig
{
	public Noise[] Layers;
	public float NoiseStrength = 1f;
	public int Seed = 0;
	public bool RandomizeOffset = false;

	private const float MIN_HEIGHT = 0f;
	public float MinHeight=> MIN_HEIGHT;

	public float MaxHeight =>
		Layers == null ? 1f : NoiseStrength * SumOfAmplitudes();

	private float SumOfAmplitudes()
	{
		if (Layers == null) return 0f;

		float sum = 0f;
		foreach (var layer in Layers)
		{
			sum += layer.Amplitude;
		}
		return sum;
	}
}
