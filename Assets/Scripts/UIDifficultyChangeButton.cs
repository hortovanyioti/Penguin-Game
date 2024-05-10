using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class UIDifficultyChangeButton : UIButton
{
    new private void Start()
    {
        base.Start();

        text.text = GameManagerScript.Instance.Difficulty.Preset.ToString();
    }
    private void OnEnable()
    {
        
    }
    public void CycleDifficulty()
    {
        GameManagerScript.Instance.Difficulty.CycleDifficulty();
        text.text = GameManagerScript.Instance.Difficulty.Preset.ToString();
    }
}