using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Statistics: ISaveable
{
    private float score;
    private int shotsFired;
    private int targetsHit;
    private List<float> reactionTimes = new();
    private float overallReactionTime;
    private float avgReactionTime;
    private float accuracy;

    public float Score { get { return score; } private set { score = value; } }
    public int ShotsFired { get { return shotsFired; } set { shotsFired = value; } }
    public int TargetsHit { get { return targetsHit; } private set { targetsHit = value; } }
    public float OverallReactionTime { get { return overallReactionTime; } private set { overallReactionTime = value; } }
    public float AvgReactionTime { get { return avgReactionTime; } private set { avgReactionTime = value > 0.999f ? 0.999f : value; } }
    public float Accuracy { get { return accuracy; } private set { accuracy = value; } }

    public void TargetHit(float reactionTime)
    {
        if (reactionTime <= 0.1)
            return;

        reactionTimes.Add(reactionTime);
    }
    public void Calculate()
    {
        if(TargetsHit == reactionTimes.Count)
            return;

		TargetsHit = reactionTimes.Count;
		if (TargetsHit != 0)
        {
            OverallReactionTime = reactionTimes.Sum();
			AvgReactionTime = reactionTimes.Average();
			Accuracy = 100f * TargetsHit / ShotsFired;

			foreach (var reactionTime in reactionTimes)
            {
				if (reactionTime < 1f)
                {
					Score += (1f - reactionTime);
				}
			}
        }
        SaveManager.instance.Save();
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
