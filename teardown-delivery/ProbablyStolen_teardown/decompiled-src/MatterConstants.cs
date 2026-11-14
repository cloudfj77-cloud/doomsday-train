using System.Collections.Generic;
using UnityEngine;

public class MatterConstants : MonoBehaviour
{
	public static Dictionary<MatterType, MatterData> matterDict;

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
	private void Awake()
	{
		matterDict = new Dictionary<MatterType, MatterData>();
	}
}
