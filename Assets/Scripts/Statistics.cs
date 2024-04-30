using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Statistics : ISaveable
{
	[SerializeField] private float score;
	[SerializeField] private int shotsFired;
	[SerializeField] private int targetsHit;
	[SerializeField] private List<float> reactionTimes = new();
	[SerializeField] private List<float> orderedReactionTimes = new();
	[SerializeField] private float overallReactionTime;
	[SerializeField] private float avgReactionTime;
	[SerializeField] private float medianReactionTime;
	[SerializeField] private float accuracy;

	public float Score { get { return score; } private set { score = value; } }
	public int ShotsFired { get { return shotsFired; } set { shotsFired = value; } }
	public int TargetsHit { get { return targetsHit; } private set { targetsHit = value; } }
	public float OverallReactionTime { get { return overallReactionTime; } private set { overallReactionTime = value; } }
	public float AvgReactionTime { get { return avgReactionTime; } private set { avgReactionTime = value > 0.999f ? 0.999f : value; } }
	public float MedianReactionTime { get { return medianReactionTime; } private set { medianReactionTime = value > 0.999f ? 0.999f : value; } }
	public float Accuracy { get { return accuracy; } private set { accuracy = value; } }

	public void TargetHit(float reactionTime)
	{
		if (reactionTime <= 0.1)
			return;

		reactionTimes.Add(reactionTime);
	}
	public void Calculate()
	{
		if (TargetsHit == reactionTimes.Count)
			return;

		TargetsHit = reactionTimes.Count;
		if (TargetsHit != 0)
		{
			OverallReactionTime = reactionTimes.Sum();
			AvgReactionTime = reactionTimes.Average();
			Accuracy = 100f * TargetsHit / ShotsFired;

			orderedReactionTimes = reactionTimes;
			orderedReactionTimes.Sort();

			if (TargetsHit % 2 == 0)
				MedianReactionTime = orderedReactionTimes.ElementAt(TargetsHit / 2 - 1) + orderedReactionTimes.ElementAt(TargetsHit / 2) / 2;
			else
				MedianReactionTime = orderedReactionTimes.ElementAt(TargetsHit / 2);

			foreach (var reactionTime in reactionTimes)
			{
				if (reactionTime < 1f)
				{
					Score += (1f - reactionTime);
				}
			}

		}
		string date = DateTime.Now.Year.ToString("0000") + "_" + DateTime.Now.Month.ToString("00") + "_" + DateTime.Now.Day.ToString("00") + "_" +
			DateTime.Now.Hour.ToString("00") + "_" + DateTime.Now.Minute.ToString("00") + "_" + DateTime.Now.Second.ToString("00");

		new FileDataHandler("stats_" + date + ".json", "", false).SaveData(this);
		//SaveManager.instance.Save();
	}
	public object CaptureState()
	{
		return new Dictionary<string, object>
		{
			{ "score", Score },
			{ "shotsFired", ShotsFired },
			{ "targetsHit", TargetsHit },
			{ "reactionTimes", reactionTimes },
			{ "overallReactionTime", OverallReactionTime },
			{ "avgReactionTime", AvgReactionTime },
			{ "accuracy", Accuracy }
		};
	}
	public void RestoreState(object state)
	{
		var stateDict = (Dictionary<string, object>)state;
		Score = (float)stateDict["score"];
		ShotsFired = (int)stateDict["shotsFired"];
		TargetsHit = (int)stateDict["targetsHit"];
		reactionTimes = ((List<object>)stateDict["reactionTimes"]).Cast<float>().ToList();
		OverallReactionTime = (float)stateDict["overallReactionTime"];
		AvgReactionTime = (float)stateDict["avgReactionTime"];
		Accuracy = (float)stateDict["accuracy"];
	}
}
