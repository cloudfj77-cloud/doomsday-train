public class GunFrameHelper
{
	public const string GUN_FRAME_TAG = "GUN_FRAME_TAG";

	public const string GUN_FRAME_GUN_ID_STRING = "GUN_FRAME_GUN_ID_STRING";

	public const string GUN_FRAME_PRINT_QUALITY_INT = "GUN_FRAME_PRINT_QUALITY_INT";

	public const int GUN_PARTS_TO_FINISH = 1;

	public static void InitFrame(GameItem frame, string gunId, int contrabandLevel)
	{
		frame.EnableTag("GUN_FRAME_TAG");
		frame.ModifyTag("GUN_FRAME_GUN_ID_STRING", delegate(TagState state)
		{
			state.Enable().SetString(gunId);
		});
		frame.shortDescription = LocHelper.GetLocalizedOrDefault("Item", "item_gun_frame_desc", "An incomplete gun frame, add a gun part on it to finish it.");
		ContrabandHelper.InitContrabandItem(frame, contrabandLevel);
		AudioHelper.InitSolidMetal(frame);
		frame.AddItemFeature(ItemFeatureList.EquipementConditionFeature(ItemFeatureList.EquipmentCondition.New));
	}

	public static void SetPrintQuality(GameItem frame, int quality)
	{
		frame.ModifyTag("GUN_FRAME_PRINT_QUALITY_INT", delegate(TagState state)
		{
			state.Enable().SetInt(quality);
		});
	}

	public static bool CanFinish(GameItem frame, GameItem part)
	{
		if (frame == null || part == null)
		{
			return false;
		}
		if (!frame.IsTag("GUN_FRAME_TAG") || !part.IsID("gun_part"))
		{
			return false;
		}
		if (frame.parentInventory == null)
		{
			return false;
		}
		return GeneralHelper.IsItemOwned(frame, part);
	}

	public static void Finish(GameItem frame, GameItem part)
	{
		if (!CanFinish(frame, part))
		{
			return;
		}
		GameInventory parentInventory = frame.parentInventory;
		string @string = frame.GetTagReadonly("GUN_FRAME_GUN_ID_STRING").GetString();
		GameItem gameItem = DirectoryMaster.Item(@string);
		if (gameItem == null)
		{
			Debug.LogError("GunFrameHelper.Finish: no gun registered under '" + @string + "'");
			return;
		}
		if (frame.IsTag("GUN_FRAME_PRINT_QUALITY_INT"))
		{
			GunHelper.SetEquipmentCondition(gameItem, ItemFeatureList.EquipmentCondition.New);
			GunHelper.MultiplyBaseStat(gameItem, frame.GetTagReadonly("GUN_FRAME_PRINT_QUALITY_INT").GetInt());
		}
		else
		{
			GunHelper.SetEquipmentCondition(gameItem, GetCondition(frame));
		}
		part.Destroy();
		frame.Destroy();
		GraphUtils.EmporiumTryAdd(parentInventory, gameItem);
		EmporiumAudioPlayer.Instance.ratchet.Play();
	}

	public static ItemFeatureList.EquipmentCondition GetCondition(GameItem item)
	{
		return item.FindItemFeatureByCategory("CATEGORY_EQUIPMENT_CONDITION")?.realCondition?.identifier switch
		{
			"equipment_condition_new" => ItemFeatureList.EquipmentCondition.New, 
			"equipment_condition_excellent" => ItemFeatureList.EquipmentCondition.Excellent, 
			"equipment_condition_fair" => ItemFeatureList.EquipmentCondition.Fair, 
			"equipment_condition_poor" => ItemFeatureList.EquipmentCondition.Poor, 
			_ => ItemFeatureList.EquipmentCondition.Good, 
		};
	}
}
