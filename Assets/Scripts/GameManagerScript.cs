using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour
{
    public static GameManagerScript instance;

	[SerializeField] private GameObject targetPool;
	public GameObject TargetPool { get { return targetPool; } private set { targetPool = value; } }


	[SerializeField] private GameObject bulletPool;
	public GameObject BulletPool { get { return bulletPool; } private set { bulletPool = value; } }


	[SerializeField] private GameObject pauseMenu;
	public GameObject PauseMenu { get { return pauseMenu; } private set { pauseMenu = value; } }


	[SerializeField] private GameObject[] playerObjects;
	public GameObject[] PlayerObjects { get { return playerObjects; } }


	private PlayerScript[] playerScripts;
	public PlayerScript[] PlayerScripts { get { return playerScripts; } private set { playerScripts = value; } }


	[SerializeField] private float spawnTime;
	public float SpawnTime { get { return spawnTime; } private set { spawnTime = value; } }


	[SerializeField] private float gameTime;
	public float GameTime { get { return gameTime; } private set { gameTime = value; } }


    private Difficulty difficulty;
    public Difficulty Difficulty { get { return difficulty; } private set { difficulty = value; } }


    [SerializeField] private bool isGameOver = false;
	public bool IsGameOver { get { return isGameOver; } private set { isGameOver = value; } }


	[SerializeField] private GameObject target;
	[SerializeField] private float xMax;
	[SerializeField] private float xMin;
	[SerializeField] private float zMax;
	[SerializeField] private float zMin;
	[SerializeField] private float yMax;
	[SerializeField] private float yMin;

	//private float timeSinceTargetSpawn;
	private float uptime = 0;
	private float prepareTime = 3;
	private float timeLeft;

	private GUIStyle myStyle = new GUIStyle();

	void Awake()
	{
		if (instance == null)
		{
			instance = this;
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
        if (SceneManager.GetActiveScene().buildIndex == 0)//Dont run in the menu scene
            return;

        Time.timeScale = 1;
		//QualitySettings.vSyncCount = 0;//todo: vsync setting
		Application.targetFrameRate = 1000;

		PlayerScripts = new PlayerScript[PlayerObjects.Length];
		for (var i = 0; i < PlayerObjects.Length; i++)
		{
			if (PlayerObjects[i] != null)
				PlayerScripts[i] = PlayerObjects[i].GetComponent<PlayerScript>();
		}

		target.transform.localScale *= Difficulty.TargetScale;

		myStyle.fontSize = 50;
		myStyle.normal.textColor = Color.green;
		myStyle.alignment = TextAnchor.UpperCenter;

		/*TODO: TEMPORARY SOLUTION*/
		pauseMenu.SetActive(true);
		pauseMenu.SetActive(false);
	}
	void Update()
	{
		if (SceneManager.GetActiveScene().buildIndex==0)//Dont run in the menu scene
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
		//timeSinceTargetSpawn += Time.deltaTime;

		/*if (timeSinceTargetSpawn >= spawnTime)
		{
			SpawnTarget();
			timeSinceTargetSpawn = 0;
		}
		*/
		CheckSpawn();
	}

	public void OnGUI()
	{
        if (SceneManager.GetActiveScene().buildIndex == 0)//Dont run in the menu scene
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
			if (targetPool.transform.childCount != 0)
				Destroy(targetPool.GetComponentInChildren<TargetScript>().gameObject);

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
		}
	}
	private void CheckSpawn()
	{
		if (isGameOver)
			return;

		var n = targetPool.transform.childCount;
		for (int i = 0; i < n; i++)
		{
			if (targetPool.transform.GetChild(i).localScale.x != 0) //If a target is still alive, no spawn
			{
				return;
			}
		}
		SpawnTarget();
	}
	public void SpawnTarget()
	{
		GameObject newTarget = Instantiate(target);
        newTarget.transform.parent = targetPool.transform;
		newTarget.transform.position = new Vector3(UnityEngine.Random.Range(xMin, xMax), UnityEngine.Random.Range(yMin, yMax), UnityEngine.Random.Range(zMin, zMax));
		newTarget.transform.LookAt(new Vector3(0f, newTarget.transform.position.y, 0f));
	}
}
