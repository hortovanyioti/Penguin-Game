using System;
using System.Collections.Generic;
using UnityEngine;

public class HeightBiomeMap : MonoBehaviour
{
	public static HeightBiomeMap Instance { get; private set; }
	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			return;
		}
		Destroy(this);
	}

	static readonly Dictionary<(int Min, int Max), BiomeTypes> RangeToEnumMap = new()
	{
		{ (0, 20), BiomeTypes.Ocean },
		{ (21, 30), BiomeTypes.Desert },
		{ (31, 40), BiomeTypes.Plains },
		{ (41, 50), BiomeTypes.Forest },
		{ (51, 60), BiomeTypes.Taiga },
		{ (61, 100), BiomeTypes.Mountain },
	};

	/// <summary>
	/// Get the biome type based on the value.
	/// value is expected to be between 0f and 1f.
	/// </summary>
	/// <param name="value"></param>
	/// <returns></returns>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	public static BiomeTypes GetBiome(float value)
	{
		value /= EndlessTerrain.Instance._noiseConfig.MaxHeight;
		Debug.Log($"Value: {value}");
		value = Mathf.Clamp01(value);

		int intValue = Convert.ToInt32(value * 100);
		foreach (var range in RangeToEnumMap)
		{
			if (intValue >= range.Key.Min && intValue <= range.Key.Max)
			{
				return range.Value;
			}
		}
		throw new ArgumentOutOfRangeException(nameof(intValue), "Value out of range");
	}
}