public class ExplosiveItemDirectory : ItemDirectory
{
	protected override void InitDirectory()
	{
		Add("smoke_grenade", () => SmokeGrenade());
		Add("flashbang_grenade", () => Flashbang());
		Add("c4", () => C4Breaching());
		Add("c4_set", () => C4Set());
		Add("lou_bomb", () => LouBombHelper.CreateBomb());
	}

	private GameItem SmokeGrenade()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/itematlas", "small_smoke");
		gameItem.name = LocHelper.GetLocalizedItem("item_smoke_grenade_name");
		gameItem.maySelectSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.canSelectSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => true;
		gameItem.mayItemTargetItemFunc = (GameItem s, GameItem t) => t.IsTag("enemy");
		gameItem.canItemTargetItemFunc = (GameItem s, GameItem t) => t.IsTag("enemy");
		gameItem.onItemTargetItemFunc = delegate(GameItem s, GameItem t)
		{
			if (GameMaster.current.raidManager.CastAbility(s, t))
			{
				s.Destroy();
			}
		};
		gameItem.unitValue = 60L;
		AudioHelper.InitGrenade(gameItem);
		ContrabandHelper.InitContrabandItem(gameItem, 2);
		gameItem.SetGameItemType("WEAPON");
		gameItem.SetGameItemType("MUNITION");
		gameItem.EnableProficiency("TACTICAL_PROFICIENCY");
		gameItem.SetType("TACTICAL_EQUIPMENT_TAG");
		return gameItem;
	}

	private GameItem C4()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/itematlas", "c4");
		gameItem.maySelectSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.canSelectSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => true;
		gameItem.mayItemTargetItemFunc = (GameItem s, GameItem t) => GraphUtils.FindParentType<GameCharacterItem>(s) == t;
		gameItem.canItemTargetItemFunc = (GameItem s, GameItem t) => GraphUtils.FindParentType<GameCharacterItem>(s) == t;
		gameItem.onItemTargetItemFunc = delegate(GameItem s, GameItem t)
		{
			if (GameMaster.current.raidManager.CastAbility(s, t))
			{
				s.Destroy();
			}
		};
		gameItem.ability = DirectoryMaster.CombatAbility("c4");
		gameItem.unitValue = 80L;
		return gameItem;
	}

	private GameItem C4Breaching()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/itematlas", "c4");
		gameItem.name = LocHelper.GetLocalizedItem("item_c4_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_c4_desc");
		gameItem.unitValue = 250L;
		gameItem.SetGameItemType("WEAPON");
		gameItem.SetGameItemType("MUNITION");
		ContrabandHelper.InitContrabandItem(gameItem, 3);
		return gameItem;
	}

	private GameItem C4Set()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/itematlas", "c4");
		gameItem.name = LocHelper.GetLocalizedItem("item_c4_set_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_c4_set_desc");
		gameItem.unitValue = 250L;
		gameItem.SetGameItemType("WEAPON");
		gameItem.SetGameItemType("MUNITION");
		ContrabandHelper.InitContrabandItem(gameItem, 3);
		return gameItem;
	}

	private GameItem Flashbang()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/itematlas", "small_flash");
		gameItem.name = LocHelper.GetLocalizedItem("item_flashbang_grenade_name");
		gameItem.maySelectSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.canSelectSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => true;
		gameItem.mayItemTargetItemFunc = (GameItem s, GameItem t) => t.IsTag("enemy");
		gameItem.canItemTargetItemFunc = (GameItem s, GameItem t) => t.IsTag("enemy");
		gameItem.onItemTargetItemFunc = delegate(GameItem s, GameItem t)
		{
			if (GameMaster.current.raidManager.CastAbility(s, t))
			{
				s.Destroy();
			}
		};
		gameItem.unitValue = 85L;
		ContrabandHelper.InitContrabandItem(gameItem, 2);
		AudioHelper.InitGrenade(gameItem);
		gameItem.EnableProficiency("TACTICAL_PROFICIENCY");
		gameItem.SetType("TACTICAL_EQUIPMENT_TAG");
		gameItem.SetGameItemType("WEAPON");
		gameItem.SetGameItemType("MUNITION");
		return gameItem;
	}
}
