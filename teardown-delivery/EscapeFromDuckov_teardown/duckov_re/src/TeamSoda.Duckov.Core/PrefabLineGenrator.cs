using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Points))]
[ExecuteInEditMode]
public class PrefabLineGenrator : MonoBehaviour, IOnPointsChanged
{
	[Serializable]
	public struct SapwnInfo
	{
		public List<PrefabPair> prefabs;

		public float rotateOffset;

		[Range(0f, 1f)]
		public float flatten;

		public Vector3 posOffset;

		public GameObject GetRandomPrefab()
		{
			if (!HasPrefab())
			{
				return null;
			}
			float num = 0f;
			for (int i = 0; i < prefabs.Count; i++)
			{
				num += prefabs[i].weight;
			}
			float num2 = UnityEngine.Random.Range(0f, num);
			for (int j = 0; j < prefabs.Count; j++)
			{
				if (num2 <= prefabs[j].weight)
				{
					return prefabs[j].prefab;
				}
				num2 -= prefabs[j].weight;
			}
			return prefabs[prefabs.Count - 1].prefab;
		}

		public bool HasPrefab()
		{
			if (prefabs != null)
			{
				return prefabs.Count > 0;
			}
			return false;
		}
	}

	[Serializable]
	public struct PrefabPair
	{
		public GameObject prefab;

		public float weight;
	}

	[Serializable]
	public struct GapInfo
	{
		[Min(0f)]
		public float startDistance;

		[Min(0f)]
		public float length;

		public float EndDistance => startDistance + length;

		public void Normalize()
		{
			if (length < 0f)
			{
				length = 0f;
			}
		}

		public bool Overlaps(float segmentStartDistance, float segmentEndDistance)
		{
			if (segmentStartDistance < EndDistance)
			{
				return startDistance < segmentEndDistance;
			}
			return false;
		}
	}

	private struct DistanceRange
	{
		public float startDistance;

		public float endDistance;

		public float Length => endDistance - startDistance;

		public DistanceRange(float startDistance, float endDistance)
		{
			this.startDistance = startDistance;
			this.endDistance = endDistance;
		}
	}

	private const float DistanceEpsilon = 0.001f;

	[SerializeField]
	private float prefabLength = 2f;

	public SapwnInfo spawnPrefab;

	public SapwnInfo startPrefab;

	public SapwnInfo endPrefab;

	public List<GapInfo> gapInfos = new List<GapInfo>();

	[SerializeField]
	private Points points;

	[SerializeField]
	[HideInInspector]
	private List<BoxCollider> colliderObjects;

	[SerializeField]
	private float updateTick = 0.5f;

	private float lastModifyTime;

	private bool dirty;

	public List<Vector3> searchedPointsLocalSpace;

	private readonly List<float> searchedPointDistances = new List<float>();

	private float searchedLineLength;

	private List<Vector3> originPoints => points.points;

	public void OnPointsChanged()
	{
	}
}
