using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class CustomMathFunction
{
	/// <summary>
	/// This function calculates the speed coefficient based on the angle between the move direction and the look direction
	/// Starting from 1, the coefficient decreases as the angle between the move direction and the look direction increases, reaching 0.5 at 90 degrees
	/// </summary>
	/// <param name="moveDir"></param>
	/// <param name="lookDir"></param>
	/// <returns></returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float GetMoveSpeedCoeff(Vector3 moveDir, Vector3 lookDir)
	{
		float angleDiff = Vector2.Angle(new Vector2(moveDir.x, moveDir.z), new Vector2(lookDir.x, lookDir.z));

		if (angleDiff < 90f)
		{
			float angleDiffRad = angleDiff * Mathf.Deg2Rad;
			return 1 - (angleDiffRad * angleDiffRad * 2 / (Mathf.PI * Mathf.PI));
		}
		else
		{
			return 0.5f;
		}
	}
}
