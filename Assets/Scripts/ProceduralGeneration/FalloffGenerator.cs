using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public struct FalloffGenerator
{
	public static NativeArray<float> GenerateFalloffMap(float falloffSlope, float falloffOffset, int xSize, int ySize)
	{
		var map = new NativeArray<float>(xSize * ySize, Allocator.Temp);
		for (int j = 0; j < ySize; j++)
		{
			for (int i = 0; i < xSize; i++)
			{
				float x = i / (float)xSize * 2 - 1;
				float y = j / (float)ySize * 2 - 1;
				float value = (x * x + y * y) / 2;
				map[i * xSize + j] = Evaluate(falloffSlope, falloffOffset, value);
			}
		}
		return map;
	}
	public static float Evaluate(float falloffSlope, float falloffOffset, float value)
	{
		if (value < 0 || value > 1)
		{
			return 1;
		}
		return 1 - 1 / (1 + Mathf.Pow(falloffOffset / value - falloffOffset, falloffSlope));
	}
}
