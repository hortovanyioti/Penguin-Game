using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

enum LinkDirection
{
	LeftRight,
	ForwardBackward
}

public class NavMeshLinkChain : MonoBehaviour
{
	public int2x2 Coords { get; private set; }
	private LinkDirection _linkDirection = LinkDirection.LeftRight;

	private List<NavMeshLink> _navMeshLinks = new List<NavMeshLink>();
	private static int CHUNK_SIZE => EndlessTerrain.CHUNK_SIZE;
	public int Count = 24;
	public int Length = 4;
	public GameObject Prefab;

	void Start()
	{
	}

	public void SetCoords(int2x2 coords)
	{
		SetCoords(coords.c0, coords.c1);
	}

	public void SetCoords(int2 chunk1, int2 chunk2)
	{
		if (chunk1.x == chunk2.x)
		{
			_linkDirection = LinkDirection.ForwardBackward;
		}

		if (chunk1.x > chunk2.x || chunk1.y > chunk2.y)
		{
			Coords = new int2x2(chunk2, chunk1);
		}
		else
		{
			Coords = new int2x2(chunk1, chunk2);
		}

		var pos = new Vector3(Coords.c1.x * CHUNK_SIZE, 0, Coords.c1.y * CHUNK_SIZE);
		this.transform.localPosition = pos;

		this.name = $"NavMeshLink ({Coords.c0.x},{Coords.c0.y}) - ({Coords.c1.x},{Coords.c1.y})";
	}

	public void Bake()
	{
		var width = (float)CHUNK_SIZE / Count;

		for (var i = 0; i < Count; ++i)
		{
			var linkObj = Instantiate(Prefab, transform);
			linkObj.name = $"NavMeshLink_{i}";

			var link = linkObj.GetComponent<NavMeshLink>();
			_navMeshLinks.Add(link);

			var x = Length / 2f;
			var z = (i + 0.5f) * width;

			var rayX = x;
			var rayZ = z;

			var linkX = 0f;
			var linkZ = z;

			var endPointX = x;
			var endPointZ = 0f;

			if (_linkDirection == LinkDirection.ForwardBackward)
			{
				rayX = z;
				rayZ = x;

				linkX = z;
				linkZ = 0f;

				endPointX = 0f;
				endPointZ = x;
			}

			var rayOrigin = this.transform.position + new Vector3(rayX, 10000, rayZ);
			var layerMask = 0;//TODO LayerMask.GetMask("Ground");

			if (!Physics.Raycast(rayOrigin, Vector3.down, out var hit, Mathf.Infinity))
			{
				UnityEngine.Debug.DrawRay(rayOrigin, Vector3.down * 10000, Color.red, 10);
				UnityEngine.Debug.LogWarning($"No hit at x:{rayOrigin.x}, z:{rayOrigin.z}");
				continue;
			}

			Quaternion rotation = Quaternion.FromToRotation(link.transform.up, hit.normal);
			link.transform.localRotation *= rotation;

			link.transform.localPosition = new Vector3(linkX, 0, linkZ) + Vector3.up * hit.point.y;
			link.startPoint = new Vector3(endPointX, 0, endPointZ);
			link.endPoint = new Vector3(-endPointX, 0, -endPointZ);
			link.width = width;
			link.activated = true;
		}
	}
}
