using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
#endif
public class UIButton : MonoBehaviour
{
    [SerializeField] string sceneToLoad;

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
}