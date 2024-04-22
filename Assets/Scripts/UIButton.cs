using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UIButton : MonoBehaviour
{
	public static UIButton instance;

	[SerializeField] string sceneToLoad;
	[SerializeField] GameObject pauseMenu;

	void Awake()
	{
		if (instance == null)
			instance = this;
	}
	public void StartButton()   // Load the scene that is specified in the inspector
	{
		SceneManager.LoadSceneAsync(sceneToLoad);
	}
	public void QuitButton()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#endif
		if (Application.platform == RuntimePlatform.Android)   // If we are on an Android device, move the app to the background
		{
			AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
			activity.Call<bool>("moveTaskToBack", true);
		}
		else if (Application.platform != RuntimePlatform.IPhonePlayer)  // If we are on any other platform (excluding IOS), quit the application
			Application.Quit();
	}
	public void QuitToMenu() // Load the main menu scene
	{
		SceneManager.LoadSceneAsync(0);
	}

	public void PauseMenu() // Load the pause menu
	{
		if (Time.timeScale == 1)
		{
			Time.timeScale = 0;
			instance.pauseMenu.SetActive(true);
		}
		else
		{
			SaveManager.instance.Save();
			instance.pauseMenu.SetActive(false);
			Time.timeScale = 1;
		}
	}
	public void PauseMenu(InputAction.CallbackContext context) // Load the pause menu
	{
		if (context.started)
		{
			if (Time.timeScale == 1)
			{
				Time.timeScale = 0;
				instance.pauseMenu.SetActive(true);
			}
			else
			{
				SaveManager.instance.Save();
				instance.pauseMenu.SetActive(false);
				Time.timeScale = 1;
			}
		}
	}
}