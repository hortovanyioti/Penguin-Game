using System;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance;

	[SerializeField] private List<GameObject> targetSpawners;

	[SerializeField] private GameObject bulletPool;
	public GameObject BulletPool { get { return bulletPool; } private set { bulletPool = value; } }


	[SerializeField] private float spawnTime;
	public float SpawnTime { get { return spawnTime; } private set { spawnTime = value; } }


	[SerializeField] private float gameTime;
	public float GameTime { get { return gameTime; } private set { gameTime = value; } }


	private Difficulty difficulty;
	public Difficulty Difficulty { get { return difficulty; } private set { difficulty = value; } }


	private bool isFiendlyFire = false;
	public bool IsFriendlyFire { get { return isFiendlyFire; } private set { isFiendlyFire = value; } }


	[SerializeField] private bool isGameOver = false;
	public bool IsGameOver { get { return isGameOver; } private set { isGameOver = value; } }

	public event Action GameOver;
	public Volume Brightness;

	[SerializeField] private bool enableTrainerConnection = false;

	//private float timeSinceTargetSpawn;
	private float uptime = 0;
	private float prepareTime = 3;
	private float timeLeft;

	private GUIStyle myStyle = new GUIStyle();

	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			CommunicatorFactory.Enabled = enableTrainerConnection;  //prevent training
			Difficulty = new Difficulty();
			new FileDataHandler("gamesettings.cfg", "", false).LoadData<Difficulty>(Difficulty);
			Difficulty.InitFromPreset();
		}
		else
		{
			Destroy(this.gameObject);
		}
	}

	void Start()
	{
		Time.timeScale = 0;
		//QualitySettings.vSyncCount = 0;//todo: vsync setting
		Application.targetFrameRate = 1000;

		myStyle.fontSize = 50;
		myStyle.normal.textColor = Color.green;
		myStyle.alignment = TextAnchor.UpperCenter;

		DontDestroyOnLoad(this.gameObject);
	}
	void Update()
	{
		if (SceneManager.GetActiveScene().name != "MainScene")  //Dont run in the menu scene
			return;

		if (Time.timeScale != 0)
		{
			Cursor.lockState = CursorLockMode.Locked;
		}
		else
		{
			Cursor.lockState = CursorLockMode.None;
		}
		uptime += Time.deltaTime;

		if (prepareTime > uptime)
		{
			return;
		}
	}

	public void OnGUI() //TODO: Refactor - Move to separate class + replace with UI elements
	{
		if (SceneManager.GetActiveScene().name != "MainScene")  //Dont run in the menu scene
			return;

		if (Time.timeScale == 0)
			return;

		GUI.Label(new Rect(10, 10, 400, 300), (1 / Time.unscaledDeltaTime).ToString("0") + " FPS", myStyle);
		timeLeft = prepareTime + GameTime - uptime;

		if (prepareTime > uptime)
		{
			GUI.Label(new Rect(Screen.width / 2 - 200, Screen.height / 10, 400, 300), Math.Ceiling(prepareTime - uptime).ToString("0"), myStyle);
		}
		else if (timeLeft >= 10)
		{
			GUI.Label(new Rect(Screen.width / 2 - 200, Screen.height / 10, 400, 300), Math.Ceiling(timeLeft).ToString("0"), myStyle);
		}
		else if (timeLeft >= 3)
		{
			GUI.Label(new Rect(Screen.width / 2 - 200, Screen.height / 10, 400, 300), timeLeft.ToString("0.0"), myStyle);
		}
		else if (timeLeft > 0)
		{
			GUI.Label(new Rect(Screen.width / 2 - 200, Screen.height / 10, 400, 300), timeLeft.ToString("0.00"), myStyle);
		}
		else
		{
			IsGameOver = true;
			GameOver?.Invoke();

			return; //TODO
			/*
			playerScripts[0].stats.Calculate();
			int score = (int)(playerScripts[0].stats.Score * 1000);
			int fired = playerScripts[0].stats.ShotsFired;
			int hits = playerScripts[0].stats.TargetsHit;
			float accuracy = playerScripts[0].stats.Accuracy;
			int avgReaction = (int)(playerScripts[0].stats.AvgReactionTime * 1000);
			int medianRt = (int)(playerScripts[0].stats.MedianReactionTime * 1000);

			GUI.Label(new Rect(Screen.width / 2 - 200, Screen.height / 5, 400, 300), "Time's up!", myStyle);
			GUI.Label(new Rect(Screen.width / 1.2f - 200, 200, 400, 300), "Score: " + score.ToString(), myStyle);
			GUI.Label(new Rect(Screen.width / 1.2f - 200, 300, 400, 300), "Shots fired: " + fired.ToString(), myStyle);
			GUI.Label(new Rect(Screen.width / 1.2f - 200, 400, 400, 300), "Targets hit: " + hits.ToString(), myStyle);
			GUI.Label(new Rect(Screen.width / 1.2f - 200, 500, 400, 300), "Accuracy: " + accuracy.ToString("0.0") + "%", myStyle);
			GUI.Label(new Rect(Screen.width / 1.2f - 200, 600, 400, 300), "Avg reaction: " + avgReaction.ToString() + " ms", myStyle);
			GUI.Label(new Rect(Screen.width / 1.2f - 200, 700, 400, 300), "Median reaction: " + medianRt.ToString() + " ms", myStyle);
			*/
		}
	}


	/*inactive
	public void PauseGame() // Load the pause menu
	{
		if (Time.timeScale == 1)
		{
			Time.timeScale = 0;
			pauseMenu.SetActive(true);
		}
		else
		{
			pauseMenu.SetActive(false);
			Time.timeScale = 1;
		}
	}*/
}
