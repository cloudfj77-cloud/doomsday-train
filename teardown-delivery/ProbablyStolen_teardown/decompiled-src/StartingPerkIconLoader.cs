using System.Collections.Generic;
using UnityEngine;

public class StartingPerkIconLoader : MonoBehaviour
{
	[SerializeField]
	private List<PerkIconEntry> perkIconList = new List<PerkIconEntry>();

	[ES3NonSerializable]
	public static Dictionary<string, Sprite> perkIcons { get; private set; } = new Dictionary<string, Sprite>();


	public static Sprite GetIcon(StartingPerk perk)
	{
		Sprite value = null;
		if (perk == null)
		{
			perkIcons.TryGetValue("default", out value);
			return value;
		}
		if (!string.IsNullOrEmpty(perk.iconKey) && CustomUIManager.Instance != null)
		{
			value = CustomUIManager.Instance.GetSprite(perk.iconKey, quiet: true);
		}
		if (value == null)
		{
			perkIcons.TryGetValue(perk.id, out value);
		}
		if (value == null)
		{
			perkIcons.TryGetValue("default", out value);
		}
		return value;
	}

	private void Start()
	{
		perkIcons.Clear();
		foreach (PerkIconEntry perkIcon in perkIconList)
		{
			if (!string.IsNullOrEmpty(perkIcon.perkName) && perkIcon.Sprite != null)
			{
				perkIcons[perkIcon.perkName] = perkIcon.Sprite;
			}
		}
	}
}
