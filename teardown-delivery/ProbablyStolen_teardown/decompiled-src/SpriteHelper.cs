public class SpriteHelper
{
	public static void UpdateWaterContainerSprite(GameItem gameItem)
	{
		if (!gameItem.IsTag("LIQUID_CONTAINER_TAG"))
		{
			return;
		}
		WaterHelper.UpdateContainerGoodType(gameItem);
		MachineRAS.OnWaterTankLiquidChanged(gameItem);
		if (gameItem.IsTag("WINE_CONTAINER_TAG"))
		{
			return;
		}
		string spritePath = gameItem.spritePath;
		int currentCapacity = WaterHelper.GetCurrentCapacity(gameItem);
		string text = "";
		string text2 = "";
		bool flag = false;
		if (gameItem.identifier.Equals("1ml_dropper"))
		{
			text = "1ml";
			text2 = "3";
			flag = true;
		}
		else if (gameItem.identifier.Equals("5ml_dropper"))
		{
			text = "5ml";
			text2 = "3";
			flag = true;
		}
		else if (gameItem.identifier.Equals("25ml_cylinder"))
		{
			text = "25ml";
			text2 = "3";
			flag = true;
		}
		else if (gameItem.identifier.Equals("50ml_cylinder"))
		{
			text = "50ml";
			text2 = "3";
			flag = true;
		}
		else if (gameItem.identifier.Equals("100ml_cylinder"))
		{
			text = "100ml";
			text2 = "3";
			flag = true;
		}
		else if (gameItem.identifier.Equals("mini_bottle"))
		{
			text = "mini";
			text2 = "3";
			flag = true;
		}
		else if (gameItem.identifier.Equals("small_bottled_water"))
		{
			text = "small";
			text2 = "3";
			flag = true;
		}
		else if (gameItem.identifier.Equals("bottled_water"))
		{
			text = "normal";
			text2 = "3";
			flag = true;
		}
		else if (gameItem.identifier.Equals("large_bottled_water"))
		{
			text = "large";
			text2 = "3";
			flag = true;
		}
		else if (gameItem.identifier.Equals("water_jug"))
		{
			text = "xl";
			text2 = "4";
			flag = true;
		}
		else
		{
			if (!gameItem.identifier.Equals("water_pitcher"))
			{
				return;
			}
			text = "pitcher";
			text2 = "4";
			flag = true;
		}
		string text3 = "";
		text3 = MapCapacity(currentCapacity);
		if (gameItem.IsTag("LIQUID_CONTAINER_DROPPER_TAG") && currentCapacity > 0)
		{
			text3 = "100";
		}
		if (!spritePath.Equals(text + text3) && !flag)
		{
			gameItem.SetSpriteAndShape("Items/water_container" + text2, text + text3);
		}
		else if (flag)
		{
			((GameItemElement)gameItem).HandleInnateModSprite();
		}
		WaterHelper.UpdateWaterBottleSound(gameItem);
	}

	public static string MapCapacity(int currentCapacity)
	{
		if (currentCapacity == 0)
		{
			return "0";
		}
		if (currentCapacity <= 15)
		{
			return "10";
		}
		if (currentCapacity <= 35)
		{
			return "25";
		}
		if (currentCapacity <= 65)
		{
			return "50";
		}
		if (currentCapacity <= 90)
		{
			return "75";
		}
		return "100";
	}

	public static void UpdateDossierSprite(GameGridInventory gridInventory, GameItem dossier)
	{
		int inventoryCapacity = GetInventoryCapacity(gridInventory);
		if (inventoryCapacity == 0)
		{
			dossier.SetSpriteAndShape("Items/dossier", "dossier_0");
		}
		else if (inventoryCapacity <= 33)
		{
			dossier.SetSpriteAndShape("Items/dossier", "dossier_1");
		}
		else if (inventoryCapacity <= 66)
		{
			dossier.SetSpriteAndShape("Items/dossier", "dossier_2");
		}
		else
		{
			dossier.SetSpriteAndShape("Items/dossier", "dossier_3");
		}
	}

	public static void UpdateTrashcanSprite(GameGridInventory gridInventory, GameItem trashcan)
	{
		int inventoryCapacity = GetInventoryCapacity(gridInventory);
		if (inventoryCapacity == 0)
		{
			trashcan.SetSpriteAndShape("Items/items_furniture", "trashcan0");
		}
		else if (inventoryCapacity <= 25)
		{
			trashcan.SetSpriteAndShape("Items/items_furniture", "trashcan25");
		}
		else if (inventoryCapacity <= 50)
		{
			trashcan.SetSpriteAndShape("Items/items_furniture", "trashcan50");
		}
		else if (inventoryCapacity <= 75)
		{
			trashcan.SetSpriteAndShape("Items/items_furniture", "trashcan75");
		}
		else
		{
			trashcan.SetSpriteAndShape("Items/items_furniture", "trashcan100");
		}
	}

	public static int GetInventoryCapacity(GameGridInventory gridInventory)
	{
		int num = gridInventory.inventoryShape.width * gridInventory.inventoryShape.height;
		int num2 = 0;
		foreach (GameItem childItem in gridInventory.childItems)
		{
			num2 += childItem.shape.width * childItem.shape.height;
		}
		if (num == 0)
		{
			return 0;
		}
		return num2 * 100 / num;
	}
}
