using System;
using System.Collections.Generic;
using System.Linq;

public class OverlayHelper
{
	public const string CHOICE_PREVIEW_TAG = "CHOICE_PREVIEW_TAG";

	private const int CONFIRMATION_TEXT_WIDTH = 50;

	public static string GetSpacesCentered(int width, string str)
	{
		int num = (width - str.Length) / 2;
		str = ((num <= 0) ? "" : new string(' ', num));
		return str;
	}

	public static string GetLineCentered(int width, string str)
	{
		int num = (width - str.Length) / 2;
		if (num > 0)
		{
			str = new string(' ', num) + str;
		}
		else if (str.Length > width)
		{
			str = str.Substring(0, width - 3) + "...";
		}
		return str;
	}

	public static void AddLineCentered(int width, string str, RichTextBuilder builder)
	{
		builder.AddLine(GetLineCentered(width, str));
	}

	public static void UpdateTopRightElement(RichTextElement topRightElement)
	{
		if (topRightElement == null)
		{
			return;
		}
		ShipManager shipManager = GameMaster.current.shipManager;
		TurnManager turnManager = GameMaster.current.turnManager;
		if (shipManager == null || turnManager == null)
		{
			return;
		}
		RichTextBuilder richTextBuilder = new RichTextBuilder();
		int width = topRightElement.fixedWidth / UISettings.current.fontWidth;
		if (GameMaster.current.InRaid())
		{
			AddLineCentered(width, "Docked:", richTextBuilder);
			if (GameMaster.current.raidManager.station != null)
			{
				string stationName = GameMaster.current.raidManager.station.stationName;
				AddLineCentered(width, stationName, richTextBuilder);
			}
			else
			{
				AddLineCentered(width, "???", richTextBuilder);
			}
		}
		else if (shipManager.isTransit)
		{
			AddLineCentered(width, "In Transit to:", richTextBuilder);
			if (GameMaster.current.shipManager.nextPOI != null)
			{
				string displayName = GameMaster.current.shipManager.nextPOI.displayName;
				AddLineCentered(width, displayName, richTextBuilder);
			}
			else
			{
				AddLineCentered(width, "???", richTextBuilder);
			}
		}
		else
		{
			AddLineCentered(width, "In Orbit:", richTextBuilder);
			if (GameMaster.current.raidManager.station != null)
			{
				string stationName2 = GameMaster.current.raidManager.station.stationName;
				AddLineCentered(width, stationName2, richTextBuilder);
			}
			else if (GameMaster.current.shipManager.nextPOI != null)
			{
				string displayName2 = GameMaster.current.shipManager.nextPOI.displayName;
				AddLineCentered(width, displayName2, richTextBuilder);
			}
			else
			{
				AddLineCentered(width, "???", richTextBuilder);
			}
		}
		string text = turnManager.GetHoursFormatted() + ":" + turnManager.GetMinutesFormatted();
		string text2 = turnManager.GetDaysFormatted() + "/" + turnManager.GetMonthsFormatted() + " " + turnManager.GetYearsFormatted();
		AddLineCentered(width, text + "    " + text2, richTextBuilder);
		topRightElement.SetRichText(richTextBuilder.Build());
	}

	public static void TransformItemChoice(GameItem choiceItem, bool isRealItem = false)
	{
		if (!isRealItem)
		{
			choiceItem.state.Clear();
			choiceItem.modifiedState.Clear();
			choiceItem.SetValue(0L);
		}
		choiceItem.effects = new List<CombatAbilityEffect>();
		choiceItem.bonusAccuracy = 0;
		choiceItem.activateDefault = true;
		choiceItem.mayActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		choiceItem.canActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		GeneralHelper.SetItemOwned(choiceItem);
		choiceItem.maySelectSlotItemFunc = null;
		choiceItem.mayItemTargetItemFunc = null;
		choiceItem.mayItemTargetSlotItemFunc = null;
		choiceItem.mayItemTargetThisFunc = null;
		choiceItem.maySlotItemTargetItemFunc = null;
		choiceItem.mayThisTargetItemFunc = null;
		choiceItem.mayStackItemFunc = (GameItem _, GameItem _) => false;
		choiceItem.mayInventoryAddItemFunc = (GameItem _, GameInventory _) => true;
		choiceItem.maxSlotAddItemFunc = (GameItem _, GameInventory _, SlotMarker _) => 1;
		choiceItem.mayInventoryRemoveItemFunc = (GameItem _, GameInventory _) => false;
		choiceItem.onActivateSlotItemFunc = null;
		choiceItem.itemFeatures.Clear();
	}

	public static void CheckDockValidParty(Action next)
	{
		if ((from gci in GraphUtils.FindChildrenType<GameCharacterItem>(GameMaster.current.shipManager.ship)
			select !gci.isDead).Any())
		{
			next?.Invoke();
		}
		else
		{
			OverlayHandler.current.ShowConfirmation("Problem", new RichTextBuilder(50).AddLine("This ship cannot dock without a valid party inside.").Build(), null, null, canYes: true, showYes: false);
		}
	}

	public static void ConfirmDock(Action next)
	{
		OverlayHandler.current.ShowConfirmation("Confirmation", new RichTextBuilder(50).AddLine("Are you sure you want to dock the ship?", ignoreEmpty: true, RenderHandler.ColorPalette.None, bold: true).AddLine().AddLine("Docking the ship will not use a turn.")
			.AddLine("Your ship will land and arrive at the hangar immediately.")
			.Build(), delegate
		{
			next?.Invoke();
		});
	}

	public static void CheckUndockValidParty(Action next)
	{
		if ((from gci in GraphUtils.FindChildrenType<GameCharacterItem>(GameMaster.current.shipManager.ship)
			select !gci.isDead).Any())
		{
			next?.Invoke();
		}
		else
		{
			OverlayHandler.current.ShowConfirmation("Problem", new RichTextBuilder(50).AddLine("This ship cannot undock without a valid party inside.").Build(), null, null, canYes: true, showYes: false);
		}
	}

	public static void ConfirmUndock(Action next)
	{
		OverlayHandler.current.ShowConfirmation("Confirmation", new RichTextBuilder(50).AddLine("Are you sure you want to undock the ship?", ignoreEmpty: true, RenderHandler.ColorPalette.None, bold: true).AddLine().AddLine("Any character or item left behind may potentially be lost forever!")
			.Build(), delegate
		{
			next?.Invoke();
		});
	}

	public static void WarnUndockMissingCharacters(Action next)
	{
		IEnumerable<GameCharacterItem> enumerable = from gci in GameMaster.current.raidManager.GetAllPlayerCharacters()
			where !GraphUtils.FindParentEquals(gci, GameMaster.current.shipManager.ship)
			select gci;
		if (enumerable.Any())
		{
			RichTextBuilder richTextBuilder = new RichTextBuilder(50);
			richTextBuilder.AddLine("Warning, some characters will be left behind!", ignoreEmpty: true, RenderHandler.ColorPalette.LightRed, bold: true, italic: false, underline: true).AddLine();
			foreach (GameCharacterItem item in enumerable)
			{
				richTextBuilder.AddLine(item.GetDisplayName());
			}
			richTextBuilder.AddLine().AddLine("Are you sure you want to continue?");
			OverlayHandler.current.ShowConfirmation("Confirmation", richTextBuilder.Build(), delegate
			{
				next?.Invoke();
			});
		}
		else
		{
			next?.Invoke();
		}
	}

	public static void CheckFuelConfirmation(Action next, int distance)
	{
		if (PropulsionHelper.HasEnoughFuel(distance))
		{
			OverlayHandler.current.ShowConfirmation("Confirmation", new RichTextBuilder(50).AddLine("Are you sure you want to leave?", ignoreEmpty: true, RenderHandler.ColorPalette.None, bold: true).AddLine().AddLine("This action will end the current turn.")
				.AddLine("Any item or character left here might be lost forever!")
				.Build(), delegate
			{
				AudioHelper.ShuttleJump();
				next?.Invoke();
			});
		}
		else
		{
			OverlayHandler.current.ShowConfirmation("Problem", new RichTextBuilder(50).AddLine("Not enough fuel to jump to this destination!").Build(), null, null, canYes: true, showYes: false);
		}
	}
}
