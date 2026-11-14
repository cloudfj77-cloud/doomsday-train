using System;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace SodaCraft;

[Serializable]
[VolumeComponentMenu("SodaCraft/TimeOfDayPost")]
public class TimeOfDayPost : VolumeComponent, IPostProcessComponent
{
	public BoolParameter enable = new BoolParameter(value: false);

	public ClampedFloatParameter nightViewAngleFactor = new ClampedFloatParameter(0.2f, 0f, 1f);

	public ClampedFloatParameter nightViewDistanceFactor = new ClampedFloatParameter(0.2f, 0f, 1f);

	public ClampedFloatParameter nightSenseRangeFactor = new ClampedFloatParameter(0.2f, 0f, 1f);

	public ClampedFloatParameter coldLevel = new ClampedFloatParameter(0f, -10f, 10f);

	public ClampedFloatParameter heatLevel = new ClampedFloatParameter(0f, -10f, 10f);

	public bool IsActive()
	{
		return enable.value;
	}

	public bool IsTileCompatible()
	{
		return false;
	}

	public override void Override(VolumeComponent state, float interpFactor)
	{
		TimeOfDayPost timeOfDayPost = state as TimeOfDayPost;
		base.Override(state, interpFactor);
		if (!(timeOfDayPost == null))
		{
			TimeOfDayController.NightViewAngleFactor = timeOfDayPost.nightViewAngleFactor.value;
			TimeOfDayController.NightViewDistanceFactor = timeOfDayPost.nightViewDistanceFactor.value;
			TimeOfDayController.NightSenseRangeFactor = timeOfDayPost.nightSenseRangeFactor.value;
			TimeOfDayController.coldLevel = timeOfDayPost.coldLevel.value;
			TimeOfDayController.heatLevel = timeOfDayPost.heatLevel.value;
		}
	}
}
