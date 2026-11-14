using System.Collections.Generic;
using UnityEngine;

public class WantedUIManager : MonoBehaviour
{
	private List<WantedElement> wantedElements = new List<WantedElement>();

	public static WantedUIManager Instance { get; private set; }

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Object.Destroy(this);
		}
		else
		{
			Instance = this;
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void RegisterElement(WantedElement wantedElement)
	{
		wantedElements.Add(wantedElement);
	}

	public void UnregisterElement(WantedElement wantedElement)
	{
		wantedElements.Remove(wantedElement);
	}

	public void OnArrested(string id)
	{
		foreach (WantedElement wantedElement in wantedElements)
		{
			if (wantedElement.identifier.Equals(id))
			{
				wantedElement.OnArrested();
			}
		}
	}
}
