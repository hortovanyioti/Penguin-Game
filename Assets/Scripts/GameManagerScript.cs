using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
	private float uptime = 0;
	private GUIStyle myStyle = new GUIStyle();
	void Start()
	{
		myStyle.fontSize = 50;
		myStyle.normal.textColor = Color.green;
	}
	void Update()
	{
		uptime += Time.deltaTime;
	}


	public void OnGUI()
	{
		GUI.Label(new Rect(100, 100, 400, 300), uptime.ToString("0.00"), myStyle);
	}
}
