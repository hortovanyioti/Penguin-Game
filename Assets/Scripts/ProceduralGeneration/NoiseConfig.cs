using System;

[Serializable]
public class NoiseConfig
{
	public Noise[] Layers;
	public float NoiseStrength;
	public int Seed;
	public bool RandomizeOffset;

	private const float MIN_HEIGHT = 0f;
	public float MinHeight => MIN_HEIGHT;

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
