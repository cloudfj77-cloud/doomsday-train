using System;
using System.Collections.Generic;
using Duckov.Scenes;
using UnityEngine;

public class RandomActiveSelectorWeighted : MonoBehaviour
{
	[Serializable]
	public struct ActiveSet
	{
		public GameObject target;

		public float weight;
	}

	[Range(0f, 1f)]
	public float activeChance = 1f;

	private int activeIndex;

	private int guid;

	private bool setted;

	public List<ActiveSet> selections;

	private void Awake()
	{
		foreach (ActiveSet selection in selections)
		{
			if (!(selection.target == null))
			{
				selection.target.SetActive(value: false);
			}
		}
	}

	private void Update()
	{
		if (!setted && LevelManager.LevelInited)
		{
			Set();
		}
	}

	private void Set()
	{
		if (MultiSceneCore.Instance == null)
		{
			return;
		}
		if (MultiSceneCore.Instance.inLevelData.TryGetValue(guid, out var value))
		{
			activeIndex = (int)value;
		}
		else
		{
			if (UnityEngine.Random.Range(0f, 1f) > activeChance)
			{
				activeIndex = -1;
			}
			else
			{
				activeIndex = GetWeightedRandomIndex();
			}
			MultiSceneCore.Instance.inLevelData.Add(guid, activeIndex);
		}
		if (activeIndex >= 0)
		{
			ActiveSet activeSet = selections[activeIndex];
			if ((bool)activeSet.target)
			{
				activeSet.target.SetActive(value: true);
			}
		}
		setted = true;
		base.enabled = false;
	}

	private int GetWeightedRandomIndex()
	{
		if (selections == null || selections.Count < 1)
		{
			return -1;
		}
		float num = 0f;
		for (int i = 0; i < selections.Count; i++)
		{
			if (!(selections[i].target == null))
			{
				num += Mathf.Max(0f, selections[i].weight);
			}
		}
		if (num <= 0f)
		{
			return GetRandomValidIndex();
		}
		float num2 = UnityEngine.Random.Range(0f, num);
		float num3 = 0f;
		int result = -1;
		for (int j = 0; j < selections.Count; j++)
		{
			if (selections[j].target == null)
			{
				continue;
			}
			float num4 = Mathf.Max(0f, selections[j].weight);
			if (!(num4 <= 0f))
			{
				num3 += num4;
				result = j;
				if (num2 < num3)
				{
					return j;
				}
			}
		}
		return result;
	}

	private int GetRandomValidIndex()
	{
		if (selections == null || selections.Count < 1)
		{
			return -1;
		}
		int num = 0;
		for (int i = 0; i < selections.Count; i++)
		{
			if (selections[i].target != null)
			{
				num++;
			}
		}
		if (num < 1)
		{
			return -1;
		}
		int num2 = UnityEngine.Random.Range(0, num);
		for (int j = 0; j < selections.Count; j++)
		{
			if (!(selections[j].target == null))
			{
				if (num2 == 0)
				{
					return j;
				}
				num2--;
			}
		}
		return -1;
	}
}
