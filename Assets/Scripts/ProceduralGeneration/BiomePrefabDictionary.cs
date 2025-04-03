using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum BiomeTypes
{
	Ocean,
	Desert,
	Forest,
	Plains,
	Taiga,
	Mountain,
}


public class BiomePrefabDictionary : MonoBehaviour
{
	public static BiomePrefabDictionary Instance { get; private set; }

	public Dictionary<BiomeTypes, List<GameObject>> BiomePrefabs = new Dictionary<BiomeTypes, List<GameObject>> ();

	public List<GameObject> OceanPrefabs = new();
	public List<GameObject> DesertPrefabs = new();
	public List<GameObject> ForestPrefabs = new();
	public List<GameObject> PlainsPrefabs = new();
	public List<GameObject> TaigaPrefabs = new();
	public List<GameObject> MountainPrefabs = new();

	void Start()
	{
		BiomePrefabs.Add(BiomeTypes.Ocean, OceanPrefabs);
		BiomePrefabs.Add(BiomeTypes.Desert, DesertPrefabs);
		BiomePrefabs.Add(BiomeTypes.Forest, ForestPrefabs);
		BiomePrefabs.Add(BiomeTypes.Plains, PlainsPrefabs);
		BiomePrefabs.Add(BiomeTypes.Taiga, TaigaPrefabs);
		BiomePrefabs.Add(BiomeTypes.Mountain, MountainPrefabs);
	}
	
	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			return;
		}
		Destroy(this);
	}
}