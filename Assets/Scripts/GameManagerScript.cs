using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
	public static GameManagerScript instance;

	[SerializeField] private GameObject[] player;
	[SerializeField] private GameObject target;
	[SerializeField] private float spawnTime;
	[SerializeField] private float gameTime;

	[SerializeField] private float xMax;
	[SerializeField] private float xMin;
	[SerializeField] private float zMax;
	[SerializeField] private float zMin;
	[SerializeField] private float yMax;
	[SerializeField] private float yMin;

	private GameObject targetPool;
	//private float timeSinceTargetSpawn;
	private float uptime = 0;
	private float prepareTime = 3;
	private GUIStyle myStyle = new GUIStyle();
	private bool isGameOver = false;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			Destroy(this.gameObject);
		}
	}

	void Start()
	{
		targetPool = this.transform.Find("TargetPool").gameObject;
		Time.timeScale = 1;
		myStyle.fontSize = 72;
		myStyle.normal.textColor = Color.green;
		myStyle.alignment = TextAnchor.UpperCenter;
	}
	void Update()
	{
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
		GUI.Label(new Rect(100, 100, 400, 300), uptime.ToString("0.0"), myStyle);
		float timeLeft = prepareTime + gameTime - uptime;

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
			isGameOver = true;
			if (targetPool.transform.childCount != 0)
				Destroy(targetPool.GetComponentInChildren<TargetScript>().gameObject);

			int score = (int)(player[0].GetComponent<PlayerScript>().stats.score * 1000);
			int hits = player[0].GetComponent<PlayerScript>().stats.targetsHit;
			int avgReaction = 0;
			if (hits != 0)
				avgReaction = (int)(player[0].GetComponent<PlayerScript>().stats.overallReactionTime * 1000 / hits);

			GUI.Label(new Rect(Screen.width / 2 - 200, Screen.height / 5, 400, 300), "Time's up!", myStyle);
			GUI.Label(new Rect(Screen.width / 1.2f - 200, 200, 400, 300), "Score: "+score.ToString(), myStyle);
			GUI.Label(new Rect(Screen.width / 1.2f - 200, 500, 400, 300), "Targets hit: "+hits.ToString(), myStyle);
			GUI.Label(new Rect(Screen.width / 1.2f - 200, 800, 400, 300), "Avg reaction:"+avgReaction.ToString()+" ms", myStyle);
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
		//newTarget.transform.Rotate(90f, 0f, 0f);
	}
}
