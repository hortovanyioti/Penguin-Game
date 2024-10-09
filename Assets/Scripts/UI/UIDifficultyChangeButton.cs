using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class UIDifficultyChangeButton : UIButton
{
    new private void Start()
    {
        base.Start();

		DisplayText.text = GameManager.Instance.Difficulty.Preset.ToString();
    }
    private void OnEnable()
    {
        
    }
    public void CycleDifficulty()
    {
        GameManager.Instance.Difficulty.CycleDifficulty();
		DisplayText.text = GameManager.Instance.Difficulty.Preset.ToString();
    }
}