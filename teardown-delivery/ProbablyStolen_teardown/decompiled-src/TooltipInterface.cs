using UnityEngine;

public abstract class TooltipInterface : MonoBehaviour
{
	public abstract string GetTooltip();

	public abstract bool IsTooltipEnabled();
}
