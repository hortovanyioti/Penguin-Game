using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
	public static SaveManager instance;
	private string savePath;

	private SaveableEntity[] saveables;
	private void Awake()
	{
		savePath = $"{Application.persistentDataPath}/saveFile.json";
		if (null == instance)
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
		Load();
	}

	public void Save()
	{
		saveables = FindObjectsByType<SaveableEntity>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID);
		var state = LoadFile();
		foreach (var saveable in saveables)
		{
			state[saveable.GetType().ToString()] = saveable.CaptureState();
		}
		SaveFile(state);
		Debug.Log("Data saved");
		saveables = null;
	}
	public void Load()
	{
		saveables = FindObjectsByType<SaveableEntity>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID);
		var state = LoadFile();
		foreach (var saveable in saveables)
		{
			string typeName = saveable.GetType().ToString();
			if (state.ContainsKey(typeName))
			{
				saveable.RestoreState(state[typeName]);
			}
		}
		Debug.Log("Data loaded");
		saveables = null;
	}
	private void SaveFile(object state)
	{
		var json = JsonConvert.SerializeObject(state);
		Debug.Log(json);
		System.IO.File.WriteAllText(savePath, json);
	}
	private Dictionary<string, object> LoadFile()
	{
		if (System.IO.File.Exists(savePath))
		{
			var json = System.IO.File.ReadAllText(savePath);
			if (json == "")
				return new Dictionary<string, object>();

			Debug.Log(json);
			return JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
		}
		return new Dictionary<string, object>();
	}
}