using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIButton : MonoBehaviour
{
	[SerializeField] string sceneToLoad;

	public void StartButton()	// Load the scene that is specified in the inspector
	{
		SceneManager.LoadSceneAsync(sceneToLoad);
	}
	public void QuitButton()
	{
		if (Application.isEditor)   // If we are in the Unity Editor, stop playing the scene
		{
			UnityEditor.EditorApplication.isPlaying = false;
		}
		else if (Application.platform == RuntimePlatform.Android)   // If we are on an Android device, move the app to the background
		{
			AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
			activity.Call<bool>("moveTaskToBack", true);
		}
		else if (Application.platform != RuntimePlatform.IPhonePlayer)  // If we are on any other platform (excluding IOS), quit the application
			Application.Quit();
	}
}