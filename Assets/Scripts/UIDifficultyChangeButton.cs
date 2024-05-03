using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class UIDifficultyChangeButton : UIButton
{
    new private void Start()
    {
        base.Start();

        /*StartCoroutine(UIInit());
        IEnumerator UIInit()
        {
            while (GameManagerScript.instance.Difficulty == null)
                yield return new WaitForSeconds(0.001f);

            StopCoroutine(UIInit());
        }*/
        text.text = GameManagerScript.instance.Difficulty.Preset.ToString();
    }
    private void OnEnable()
    {
        
    }
    public void CycleDifficulty()
    {
        GameManagerScript.instance.Difficulty.CycleDifficulty();
        text.text = GameManagerScript.instance.Difficulty.Preset.ToString();
    }
}