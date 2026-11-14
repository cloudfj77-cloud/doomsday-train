using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Duckov/Perks/Perk ID List")]
public class PerkTreeIDList : ScriptableObject
{
	public List<string> perkTrees = new List<string>();
}
