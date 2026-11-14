using System.Collections.Generic;

public class MenuManager
{
	private readonly MenuScene menuScene;

	public string selectedFaction;

	public Scenario selectedScenario;

	public MenuManager(MenuScene menuScene)
	{
		this.menuScene = menuScene;
	}

	public void StartMenu()
	{
		menuScene.StartMenu();
	}

	private List<GameItem> GetDefaultCharacters()
	{
		List<GameItem> list = new List<GameItem>();
		GameCharacterItem gameCharacterItem = DirectoryMaster.CharacterItem("albert");
		GraphUtils.TryAcceptAll(gameCharacterItem, DirectoryMaster.Item("combat_knife"));
		GameItem gameItem = DirectoryMaster.Item("stun_baton");
		GraphUtils.TryAcceptAll(gameItem, DirectoryMaster.Item("energy_credit"));
		GraphUtils.TryAcceptAll(gameCharacterItem, gameItem);
		GraphUtils.TryAcceptAll(gameCharacterItem, DirectoryMaster.Item("flashbang_grenade"));
		GraphUtils.TryAcceptAll(gameCharacterItem, DirectoryMaster.Item("smoke_grenade"));
		GraphUtils.TryAcceptAll(gameCharacterItem, DirectoryMaster.Item("retaliate"));
		GraphUtils.TryAcceptAll(gameCharacterItem, DirectoryMaster.Item("diversion_item_ability"));
		GraphUtils.TryAcceptAll(gameCharacterItem, DirectoryMaster.Item("backpack_small"));
		GraphUtils.TryAcceptAll(gameCharacterItem, DirectoryMaster.Item("tactical_proficiency"));
		OrganHelper.EquipHumanoidWithDefaultOrgan(gameCharacterItem);
		gameCharacterItem.isPlayerControlled = true;
		list.Add(gameCharacterItem);
		gameCharacterItem = DirectoryMaster.CharacterItem("benjamin");
		GraphUtils.TryAcceptAll(gameCharacterItem, DirectoryMaster.Item("glock_receiver2"));
		GameItem gameItem2 = DirectoryMaster.Item("energy_hand_canon");
		GraphUtils.TryAcceptAll(gameItem2, DirectoryMaster.Item("energy_credit"));
		GraphUtils.TryAcceptAll(gameCharacterItem, gameItem2);
		GameItem gameItem3 = DirectoryMaster.Item("mirage_generator");
		GraphUtils.TryAcceptAll(gameItem3, DirectoryMaster.Item("energy_credit"));
		GraphUtils.TryAcceptAll(gameCharacterItem, gameItem3);
		GraphUtils.TryAcceptAll(gameCharacterItem, DirectoryMaster.Item("green_injector"));
		GraphUtils.TryAcceptAll(gameCharacterItem, DirectoryMaster.Item("backpack_small_modular"));
		GraphUtils.TryAcceptAll(gameCharacterItem, DirectoryMaster.Item("technical_proficiency"));
		gameCharacterItem.isPlayerControlled = true;
		list.Add(gameCharacterItem);
		gameCharacterItem = DirectoryMaster.CharacterItem("charlotte");
		GraphUtils.TryAcceptAll(gameCharacterItem, DirectoryMaster.Item("glock_receiver"));
		GraphUtils.TryAcceptAll(gameCharacterItem, DirectoryMaster.Item("turquoise_injector"));
		GraphUtils.TryAcceptAll(gameCharacterItem, DirectoryMaster.Item("green_injector"));
		GraphUtils.TryAcceptAll(gameCharacterItem, DirectoryMaster.Item("hemostatic_bandage_item"));
		gameCharacterItem.isPlayerControlled = true;
		GameItem gameItem4 = DirectoryMaster.Item("satchel");
		GraphUtils.TryAcceptAll(gameItem4, DirectoryMaster.Item("hemostatic_bandage_item"));
		GraphUtils.TryAcceptAll(gameItem4, DirectoryMaster.Item("hemostatic_bandage_item"));
		GraphUtils.TryAcceptAll(gameItem4, DirectoryMaster.Item("blue_injector"));
		GraphUtils.TryAcceptAll(gameItem4, DirectoryMaster.Item("smelling_salt"));
		GraphUtils.TryAcceptAll(gameCharacterItem, gameItem4);
		GraphUtils.TryAcceptAll(gameCharacterItem, DirectoryMaster.Item("medical_proficiency"));
		GraphUtils.TryAcceptAll(gameCharacterItem, DirectoryMaster.Item("medical_assist_item_ability"));
		GraphUtils.TryAcceptAll(gameCharacterItem, DirectoryMaster.Item("brace_item_ability"));
		list.Add(gameCharacterItem);
		gameCharacterItem = DirectoryMaster.CharacterItem("daniel");
		GraphUtils.TryAcceptAll(gameCharacterItem, DirectoryMaster.Item("smoke_grenade"));
		GraphUtils.TryAcceptAll(gameCharacterItem, DirectoryMaster.Item("green_injector"));
		GraphUtils.TryAcceptAll(gameCharacterItem, DirectoryMaster.Item("armor_lining"));
		GraphUtils.TryAcceptAll(gameCharacterItem, DirectoryMaster.Item("overwatch"));
		GraphUtils.TryAcceptAll(gameCharacterItem, DirectoryMaster.Item("mark_item_ability"));
		GraphUtils.TryAcceptAll(gameCharacterItem, DirectoryMaster.Item("backpack_small"));
		gameCharacterItem.isPlayerControlled = true;
		list.Add(gameCharacterItem);
		list.ForEach(delegate(GameItem gci)
		{
			GraphUtils.TryAcceptAll(gci, DirectoryMaster.Item("wait"));
		});
		return list;
	}

	public List<Scenario> GetAvailableFactionScenarios()
	{
		List<Scenario> list = new List<Scenario>();
		Scenario scenario = new Scenario
		{
			ship = DirectoryMaster.Item("cargo_ship"),
			totalPoints = 5000L,
			richTextDescription = new RichTextBuilder().AddLine("Sok-27C", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen, bold: true, italic: false, underline: true).AddLine("  Cargo Ship", ignoreEmpty: true, RenderHandler.ColorPalette.LightBlue).Add("  Designer: ")
				.AddLine("Sokolov Heavy Industry", ignoreEmpty: true, RenderHandler.ColorPalette.LightRed)
				.AddLine(" ")
				.AddLine("  This ship is the cargo variant of the Sok-27. A popular ship for young entreprising captain. It's low price tag made the entry cost to space freight accessible. While the ship can officialy seat 3 person, 2 two passenger seats are cramped and uncomfortable. While having a smaller cargo hold than most other cargo ship. The Sok-27C require little to none port infrastructure to operate in. It is often seen transporting vital supplies to high risk area during shortage.The Sok-27C captain are often joked as opportunist in the freight industry.", ignoreEmpty: true, RenderHandler.ColorPalette.White)
				.AddLine(" ")
				.Add("  Starting Difficulty: ")
				.AddLine("Normal", ignoreEmpty: true, RenderHandler.ColorPalette.LightOrange)
				.Add("  Unique Feature: ")
				.AddLine("Large Cargo Hold, Good Versatility", ignoreEmpty: true, RenderHandler.ColorPalette.LightBlue)
				.Add("  Playstyle: ")
				.AddLine("Merchant, Smuggler, Offloader", ignoreEmpty: true, RenderHandler.ColorPalette.LightOrange)
		};
		ShipHelper.EquipCargoShip(scenario.ship);
		scenario.selectableCharacters = new GameGridScrollableInventory(10, 20);
		scenario.selectableCharacters.AddFastUnchecked(GetDefaultCharacters());
		scenario.selectableItems = new GameGridScrollableInventory(10, 20);
		list.Add(scenario);
		scenario = new Scenario
		{
			ship = DirectoryMaster.Item("military_transport_ship"),
			richTextDescription = new RichTextBuilder().AddLine("A-101 Albatross", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen, bold: true, italic: false, underline: true).AddLine("  Military Transport Shuttle", ignoreEmpty: true, RenderHandler.ColorPalette.LightBlue).Add("  Designer: ")
				.AddLine("Republic Navy", ignoreEmpty: true, RenderHandler.ColorPalette.LightBlue)
				.AddLine(" ")
				.AddLine("  Originally design for boarding operation The Albatross is lightly armed and armored. The republic extensively used the A-101 in anti-piracy activities. Launched from larger ships, a couple of A-101 loaded with marines are enough to handle a pirate ship.The sight of A-101 is a pirate worst's nightmare. With the Albatross inital performance, the Navy plan to retire the older and heavier A-95 Condor.", ignoreEmpty: true, RenderHandler.ColorPalette.White)
				.AddLine(" ")
				.Add("  Starting Difficulty: ")
				.AddLine("Easy", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen)
				.Add("  Unique Feature: ")
				.AddLine("Intimidating Presence, High Operating Cost", ignoreEmpty: true, RenderHandler.ColorPalette.LightBlue)
				.Add("  Playstyle: ")
				.AddLine("Fighter, Passenger Transport", ignoreEmpty: true, RenderHandler.ColorPalette.LightOrange),
			totalPoints = 5000L
		};
		ShipHelper.EquipMilitaryTransportShip(scenario.ship);
		scenario.selectableCharacters = new GameGridScrollableInventory(10, 20);
		scenario.selectableCharacters.AddFastUnchecked(GetDefaultCharacters());
		scenario.selectableItems = new GameGridScrollableInventory(10, 20);
		list.Add(scenario);
		scenario = new Scenario
		{
			ship = DirectoryMaster.Item("utility_ship"),
			richTextDescription = new RichTextBuilder().AddLine("Type-54 Bian Yuan", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen, bold: true, italic: false, underline: true).AddLine("  Utility Ship", ignoreEmpty: true, RenderHandler.ColorPalette.LightBlue).Add("  Designer: ")
				.AddLine("Zhonghua Shipbuilding", ignoreEmpty: true, RenderHandler.ColorPalette.LightOrange)
				.AddLine(" ")
				.AddLine("  A ship designed for industrial operations. Commonly used in mining, salvaging and construction operation.Mass produced by Zhonghua with a streamlined spare part logistic and service network, this made operating and maintning the Type-54 incredibly easy. This made the Type-54 a direct competitor to the Sok-27U which offer similar price and capabilities. Some independant captains took a liking to the Type-54. They found that the ship's hull is rugged and durable and the ship's system is reliable.", ignoreEmpty: true, RenderHandler.ColorPalette.White)
				.AddLine(" ")
				.Add("  Starting Difficulty: ")
				.AddLine("Easy", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen)
				.Add("  Unique Feature: ")
				.AddLine("Rugged Hull, External Slots", ignoreEmpty: true, RenderHandler.ColorPalette.LightBlue)
				.Add("  Playstyle: ")
				.AddLine("Miner, Scavenger, Self-Sufficient", ignoreEmpty: true, RenderHandler.ColorPalette.LightOrange),
			totalPoints = 5000L
		};
		ShipHelper.EquipUtilityShip(scenario.ship);
		scenario.selectableCharacters = new GameGridScrollableInventory(10, 20);
		scenario.selectableCharacters.AddFastUnchecked(GetDefaultCharacters());
		scenario.selectableItems = new GameGridScrollableInventory(10, 20);
		list.Add(scenario);
		scenario = new Scenario
		{
			ship = DirectoryMaster.Item("refugee_ship"),
			richTextDescription = new RichTextBuilder().AddLine("The Bus", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen, bold: true, italic: false, underline: true).AddLine("  Makeshift Transport Ship", ignoreEmpty: true, RenderHandler.ColorPalette.LightBlue).Add("  Designer: ")
				.AddLine("Spacer", ignoreEmpty: true, RenderHandler.ColorPalette.LightBlue)
				.AddLine(" ")
				.AddLine("  Affectionately named by it's crew, The Bus is a type of barely spaceworthy ship weld together by determined spacers in search of a better live. Made from any materal those spacers can lay their hand on. Usually it is made from cargo container, fuel tank and a lot of low grade scrap metal and come in all shapes and sizes. The hull is fragile and but due to its makeshift nature, it is fairly simple to repair. Those makeshift ships transport all kind of people, from political refugee to wanted criminal. This is the cheapest ride one can find. The Spacers have a code of honor between them and a Bus must always help another Bus if met in deep space.", ignoreEmpty: true, RenderHandler.ColorPalette.White)
				.AddLine(" ")
				.Add("  Starting Difficulty: ")
				.AddLine("Hard", ignoreEmpty: true, RenderHandler.ColorPalette.LightRed)
				.Add("  Unique Feature: ")
				.AddLine("Makeshift Hull, Low Operating Cost", ignoreEmpty: true, RenderHandler.ColorPalette.LightBlue)
				.Add("  Playstyle: ")
				.AddLine("Refugee, Passenger Transport", ignoreEmpty: true, RenderHandler.ColorPalette.LightOrange),
			totalPoints = 5000L
		};
		ShipHelper.EquipRefugeeShip(scenario.ship);
		scenario.selectableCharacters = new GameGridScrollableInventory(10, 20);
		scenario.selectableCharacters.AddFastUnchecked(GetDefaultCharacters());
		scenario.selectableItems = new GameGridScrollableInventory(10, 20);
		list.Add(scenario);
		return list;
	}
}
