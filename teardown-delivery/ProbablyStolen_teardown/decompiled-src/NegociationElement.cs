using System.Collections.Generic;
using UnityEngine;

public abstract class NegociationElement : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
	}

	public abstract string GetNegociationToolID();

	public abstract bool IsNegociationTool();

	public abstract List<NegociationData> GetNegociationData();
}
