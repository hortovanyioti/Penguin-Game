
using UnityEngine;

public class Difficulty
{
	public enum ePreset
	{
		EASY,
		MEDIUM,
		HARD,
		CUSTOM
	}
	[SerializeField] private ePreset preset;
	public ePreset Preset { get { return preset; } private set { preset = value; } }

	private float targetScale;
	public float TargetScale { get { return targetScale; } private set { targetScale = value; } }

	public void InitFromPreset()
	{
		switch (Preset)
		{
			case ePreset.EASY:
				TargetScale = 1.5f;
				break;
			case ePreset.MEDIUM:
				TargetScale = 1;
				break;
			case ePreset.HARD:
				TargetScale = 0.5f;
				break;
			default:    //TODO: Implement custom
				TargetScale = 1;
				break;
		}
	}
	public void CycleDifficulty()
	{
		if (Preset == ePreset.CUSTOM)
		{
			Preset = 0;
		}
		else
		{
			Preset++;
		}
		new FileDataHandler("gamesettings.cfg", "", false).SaveData(this);
		InitFromPreset();
	}
}