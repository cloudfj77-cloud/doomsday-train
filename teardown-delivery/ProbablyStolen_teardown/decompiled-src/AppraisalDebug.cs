using System.Collections.Generic;
using System.Linq;
using System.Text;
using QFSW.QC;

public static class AppraisalDebug
{
	[Command("appraisal-rank", "Prints the appraisal rank, progress, blacklist and customer pool, or sets the rank (0 Junior, 1 Appraiser, 2 Senior, 3 Master) with its certificate and guides", Platform.AllPlatforms, MonoTargetType.Single)]
	public static string Rank(int rank = -1)
	{
		if (rank >= 0)
		{
			if (rank > 3)
			{
				return $"Rank must be 0 (Junior) to {3} (Master).";
			}
			AppraisalRankHelper.SetRank(rank);
		}
		return Status();
	}

	[Command("appraisal-block", "Blocks (or allows, with false) an appraisal item type by id, following the certificate window's rules: hand, ribwich, plasma_fuel or a mod id", Platform.AllPlatforms, MonoTargetType.Single)]
	public static string Block(string typeId, bool blocked = true)
	{
		AppraisalItemType appraisalItemType = AppraisalItemRegistry.Get(typeId);
		if (appraisalItemType == null)
		{
			return "No appraisal item type '" + typeId + "'. Known: " + string.Join(", ", from t in AppraisalItemRegistry.GetAll()
				select t.id) + ".";
		}
		int rank = AppraisalRankHelper.GetRank();
		string text = null;
		if (appraisalItemType.unlockRank > rank)
		{
			text = "it is not unlocked at this rank";
		}
		else if (blocked && !AppraisalItemRegistry.CanBlock(appraisalItemType, rank))
		{
			text = "it is the newest qualification and the rank is below Master";
		}
		else if (blocked && AppraisalItemRegistry.IsLastAllowed(appraisalItemType, rank))
		{
			text = "it is the last allowed type";
		}
		if (text != null)
		{
			return "Cannot " + (blocked ? "block" : "allow") + " '" + appraisalItemType.id + "': " + text + ".\n" + Status();
		}
		List<string> list = PlayerStore.Instance.appraisalBlacklist ?? (PlayerStore.Instance.appraisalBlacklist = new List<string>());
		list.Remove(appraisalItemType.id);
		if (blocked)
		{
			list.Add(appraisalItemType.id);
		}
		return "'" + appraisalItemType.id + "' is now " + (blocked ? "blocked" : "allowed") + ".\n" + Status();
	}

	[Command("appraisal-customer", "Queues a random appraisal customer as the next client, bringing 2 to 4 items of one type from the current pool", Platform.AllPlatforms, MonoTargetType.Single)]
	public static string Customer()
	{
		PlayerStore.Instance.isAppraisalIntroduced = true;
		PlayerStore.Instance.storeClientManager.AddNextClient(StoreClientListAppraisal.CreateAppraisalRandom());
		List<AppraisalItemType> pool = AppraisalItemRegistry.GetPool(AppraisalRankHelper.GetRank());
		return "Queued an appraisal customer. Pool: " + string.Join(", ", pool.Select((AppraisalItemType t) => t.id)) + ".";
	}

	private static string Status()
	{
		int rank = AppraisalRankHelper.GetRank();
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine($"Rank {rank} ({AppraisalRankHelper.GetRankName(rank)}), progress {AppraisalRankHelper.GetProgressPercent()}%, " + (AppraisalRankHelper.CanProgress() ? "promotion possible" : "no further promotion") + ", appraisal " + (PlayerStore.Instance.isAppraisalIntroduced ? "introduced" : "not introduced") + ".");
		foreach (AppraisalItemType item in from t in AppraisalItemRegistry.GetAll()
			orderby t.unlockRank
			select t)
		{
			stringBuilder.AppendLine(string.Format(arg1: (item.unlockRank <= rank) ? ((!AppraisalItemRegistry.IsBlocked(item, rank)) ? ((!AppraisalItemRegistry.IsLastAllowed(item, rank)) ? (AppraisalItemRegistry.CanBlock(item, rank) ? "allowed" : "allowed, newest qualification (cannot be blocked)") : "allowed, last one (cannot be blocked)") : "blocked") : ("locked (unlocks at rank " + item.unlockRank + ")"), format: "  {0,-16} {1}", arg0: item.id));
		}
		stringBuilder.AppendLine("Pool: " + string.Join(", ", from t in AppraisalItemRegistry.GetPool(rank)
			select t.id));
		return stringBuilder.ToString();
	}
}
