using System.Collections.Generic;
using UnityEngine;

public class GraphHandler : MonoBehaviour
{
	public static List<GraphNodeWindow> windowGraphNodeRoots { get; private set; }

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
	private void Awake()
	{
		windowGraphNodeRoots = new List<GraphNodeWindow>();
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
