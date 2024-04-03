using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UIElements;

public class Statistics
{
	public float score { get; private set; } = 0f;
	public int targetsHit { get; private set; } = 0;
	public float overallReactionTime { get; private set; } = 0f;
	public float accuracy { get; private set; } = 0f;//TODO: Implement this

	public void TargetHit(float reactionTime)
	{
		if (reactionTime <= 0)
			return;
		Debug.Log(reactionTime);
		if (reactionTime < 1f)
		{
			score += (1f - reactionTime);
		}
		targetsHit++;
		overallReactionTime += reactionTime;
	}
}
