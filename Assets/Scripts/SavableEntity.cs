using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SaveableEntity : MonoBehaviour
{
	public object CaptureState()
	{
		var state = new Dictionary<string, object>();
		var saveables = GetComponents<ISaveable>();
	
		foreach (var saveable in saveables)
		{
			state[saveable.GetType().ToString()] = saveable.CaptureState();
			Debug.Log(DictionaryToString(state));
		}
		return state;
	}
	public string DictionaryToString(Dictionary<string, object> dictionary)
	{
		string dictionaryString = "{";
		foreach (KeyValuePair<string, object> keyValues in dictionary)
		{
			dictionaryString += keyValues.Key + " : " + keyValues.Value + ", ";
		}
		return dictionaryString.TrimEnd(',', ' ') + "}";
	}
	public void RestoreState(object state)
	{
		var saveables = GetComponents<ISaveable>();
		foreach (var saveable in saveables)
		{
			string typeName = saveable.GetType().ToString();
			if (state is Dictionary<string, object> dictionary && dictionary.ContainsKey(typeName))
			{
				saveable.RestoreState(dictionary[typeName]);
			}
		}
	}
}