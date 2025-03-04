using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FalloffGenerator
{
	[Range(0, 10)]
	public static float FalloffSlope { get; set; } = 1f;

	[Range(0, 10)]
	public static float FalloffOffset { get; set; } = 1f;

	public static float[,] GenerateFalloffMap(int xSize, int ySize)
	{
		float[,] map = new float[xSize, ySize];
		for (int j = 0; j < ySize; j++)
		{
			for (int i = 0; i < xSize; i++)
			{
				float x = i / (float)xSize * 2 - 1;
				float y = j / (float)ySize * 2 - 1;
				//float value = Mathf.Max(x, y);
				//float value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
				float value = (x * x + y * y) / 2;
				map[i, j] = Evaluate(value);
			}
		}
		return map;
	}
	public static float Evaluate(float value)
	{
		if (value < 0 || value > 1)
		{
			Debug.LogError("0<=value<=1 invalid: " + value);
			return 1;
		}
		return 1 - 1 / (1 + Mathf.Pow(FalloffOffset / value - FalloffOffset, FalloffSlope));
	}
}
