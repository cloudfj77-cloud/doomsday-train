using Cysharp.Threading.Tasks;
using Duckov.Economy;
using ItemStatsSystem;
using UnityEngine;

namespace Duckov.Consoles;

[CreateAssetMenu(menuName = "Duckov/Console/Give")]
public class Give : DCommand
{
	public override string CommandWord => "give";

	public override string Execute(DConsole console, string[] args)
	{
		if (args.Length < 1)
		{
			return "Failed giving. Args not enough";
		}
		string text = args[0];
		int result = -1;
		if (!int.TryParse(text, out result))
		{
			result = ItemAssetsCollection.TryGetIDByName(text, ignoreCase: true);
			if (result < 0)
			{
				return "Failed finding item type: " + text;
			}
		}
		int num = 1;
		if (args.Length >= 2 && int.TryParse(args[1], out var result2))
		{
			num = result2;
		}
		if (num < 0)
		{
			return "Amount error.";
		}
		if (!LevelManager.LevelInited)
		{
			return "This command is in-game only. Abort.";
		}
		Cost cost = new Cost((result, num));
		cost.Return(directToBuffer: false, toPlayerInventory: true).Forget();
		return $"Giving {text} x{num}";
	}
}
