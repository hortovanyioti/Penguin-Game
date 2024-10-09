using UnityEngine;

public class QuitButton : UIButton
{

	override public void OnClick()
	{
		base.OnClick();

#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#endif
		Application.Quit();
	}
}