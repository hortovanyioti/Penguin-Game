using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Statistics
{
    private float score;
    private int shotsFired;
    private int targetsHit;
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
        if (reactionTime <= 0)
            return;

        if (reactionTime < 1f)
        {
            Score += (1f - reactionTime);
        }
        TargetsHit++;
        OverallReactionTime += reactionTime;
    }
    public void Calculate()
    {
        if (TargetsHit != 0)
            AvgReactionTime = OverallReactionTime / TargetsHit;

        if (ShotsFired > 0)
            Accuracy = 100f * TargetsHit / ShotsFired;
    }
}
