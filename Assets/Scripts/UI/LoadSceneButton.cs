using UnityEngine;

public class LoadSceneButton : UIButton
{
	[SerializeField] string sceneToLoad;
	public string SceneToLoad { get => sceneToLoad; set => sceneToLoad = value; }

	override public void OnClick()
	{
		base.OnClick();
		//TODO: GameManager.LoadScene(SceneToLoad);
	}
}