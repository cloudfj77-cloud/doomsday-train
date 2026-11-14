using System.Collections.Generic;

public class StationGeneration
{
	public static Station GenerateWaystation()
	{
		Room room = new Room("Waystation Entrance", Room.Type.Ship);
		Room room2 = RoomFactory.CreateHydroponic();
		room.FullyConnectToRoom(room2);
		Room room3 = RoomFactory.CreateCrewQuarter(Room.Type.NPC);
		room3.AddRaidersToRoom(EnemyFactory.CreatePremadeRaiderParty());
		room.FullyConnectToRoom(room3);
		Room room4 = RoomFactory.CreateCafeteria();
		room3.FullyConnectToRoom(room4);
		Room room5 = RoomFactory.CreateKitchen(Room.Type.NPC);
		room5.AddRaidersToRoom(EnemyFactory.CreatePremadeRaiderParty2());
		room4.FullyConnectToRoom(room5);
		Room room6 = RoomFactory.CreateCustomRoom("Bridge", null, Room.Type.NPC);
		room6.AddRaidersToRoom(EnemyFactory.CreatePremadeRaiderBoss());
		room4.FullyConnectToRoom(room6);
		Room room7 = RoomFactory.CreateInfirmary();
		room6.FullyConnectToRoom(room7);
		room2.SpawnAttachedLoot();
		room3.SpawnAttachedLoot();
		room4.SpawnAttachedLoot();
		room5.SpawnAttachedLoot();
		room7.SpawnAttachedLoot();
		return new Station(room);
	}

	public static Station GenerateStationOne()
	{
		Room room = new Room("Plundered pastel inc. Outpost Station", Room.Type.Ship);
		Room room2 = RoomFactory.CreateEntrance(Room.Type.NPC);
		room2.FullyConnectToRoom(room);
		Room room3 = RoomFactory.CreateSecurityCheckpoint(Room.Type.NPC);
		room3.FullyConnectToRoom(room2);
		room3.AddRaidersToRoom(EnemyFactory.CreatePremadeJunkers());
		Room room4 = RoomFactory.CreateCustomRoom("Hallway");
		room4.FullyConnectToRoom(room3);
		Room room5 = RoomFactory.CreateHydroponic();
		room5.FullyConnectToRoom(room4);
		Room room6 = RoomFactory.CreateCafeteria();
		room6.FullyConnectToRoom(room4);
		room2.SpawnAttachedLoot();
		room3.SpawnAttachedLoot();
		GraphUtils.TryAcceptAll(room3.floorInventory, DirectoryMaster.Item("pouch_general"));
		room5.SpawnAttachedLoot();
		room6.SpawnAttachedLoot();
		return new Station(room);
	}

	public static Station GenerateStationTwo()
	{
		Room room = new Room("Plundered Davre Food LTD Outpost Station", Room.Type.Ship);
		Dialogue arriveDialogue = new Dialogue();
		arriveDialogue.SetText("", "Your dropship arrive to a Davre Food LTD Outpost Station, This station was originally build as a logistic hub to support the company's operation in the sector. The outpost has clearly been looted, hopefully there is still some thing left.").SetEndAction(delegate
		{
			arriveDialogue.ForceDisable();
		});
		room.dialogues.Add(arriveDialogue);
		Room room2 = RoomFactory.CreateEntrance();
		room2.FullyConnectToRoom(room);
		Room room3 = RoomFactory.CreateSecurityCheckpoint(Room.Type.NPC);
		room3.FullyConnectToRoom(room2);
		room3.AddRaidersToRoom(EnemyFactory.CreatePremadeEasy());
		Room room4 = RoomFactory.CreateCustomRoom("Hallway");
		room4.FullyConnectToRoom(room3);
		Room room5 = RoomFactory.CreateCustomRoom("Service Hallway");
		room5.FullyConnectToRoom(room4);
		Room room6 = RoomFactory.CreateCafeteria(Room.Type.NPC);
		room6.FullyConnectToRoom(room5);
		room6.AddRaidersToRoom(EnemyFactory.CreatePremadeEasy());
		Room room7 = RoomFactory.CreateKitchen();
		room7.FullyConnectToRoom(room6);
		Room room8 = RoomFactory.CreateCustomRoom("Medical Hallway", null, Room.Type.NPC);
		room8.FullyConnectToRoom(room4);
		room8.AddRaidersToRoom(EnemyFactory.CreatePremadeSadistDuoParty());
		Room room9 = RoomFactory.CreateInfirmary();
		room9.FullyConnectToRoom(room8);
		Room room10 = RoomFactory.CreateMedicalStorageRoom();
		room10.FullyConnectToRoom(room8);
		Room room11 = RoomFactory.CreateCustomRoom("Manufacturing Hallway");
		room11.FullyConnectToRoom(room4);
		Room room12 = RoomFactory.CreateHydroponic();
		room12.FullyConnectToRoom(room11);
		Room room13 = RoomFactory.CreateCanningRoom(Room.Type.NPC);
		room13.FullyConnectToRoom(room11);
		room13.AddRaidersToRoom(EnemyFactory.CreatePremadeSadistDuoParty());
		Room room14 = RoomFactory.CreateWorkshop(Room.Type.NPC);
		room14.FullyConnectToRoom(room11);
		room14.AddRaidersToRoom(EnemyFactory.CreatePremadeJunkers());
		room12.SpawnAttachedLoot();
		room13.SpawnAttachedLoot();
		room14.SpawnAttachedLoot();
		GraphUtils.TryAcceptAll(room14.floorInventory, DirectoryMaster.Item("backpack_small_modular"));
		room6.SpawnAttachedLoot();
		room7.SpawnAttachedLoot();
		room9.SpawnAttachedLoot();
		room10.SpawnAttachedLoot();
		return new Station(room);
	}

	public static Station GenerateStationThree()
	{
		Room room = new Room("Ticonderoga Makeshift Trading Station", Room.Type.Ship);
		Dialogue arriveDialogue = new Dialogue();
		arriveDialogue.SetText("Ticonderoga Station", "Your dropship arrive to Ticonderoga Station, a derelict super freighter converted into a trading station. You can't help but wonder if the thrusters still works. You see that only half of every lightbulb is powered on.").NextDialogue().SetText("", "You see a girl spying on you behind some crates. She leaves as soon as she realized that she's spotted")
			.SetEndAction(delegate
			{
				arriveDialogue.ForceDisable();
			});
		room.dialogues.Add(arriveDialogue);
		Room room2 = RoomFactory.CreateEntrance();
		room2.FullyConnectToRoom(room);
		Room room3 = RoomFactory.CreateSecurityCheckpoint(Room.Type.NPC);
		room3.FullyConnectToRoom(room2);
		List<GameCharacterItem> list = room3.AddRaidersToRoom(EnemyFactory.CreatePremadeGearedGuards());
		list[0].SetName("Sergent Riggs");
		list[1].SetName("Guardsman Williams");
		Dialogue dialogue = new Dialogue();
		dialogue.SetText("Guardsman Williams", "I am Williams with Ticonderoga Security. If you need anything ask my boss Sergent Riggs.").AddChoice(DirectoryMaster.Item("ok"), "ok.");
		list[1].SetDialogue(dialogue);
		Dialogue guardDialogue = new Dialogue();
		Dialogue dialogue2 = new Dialogue();
		Dialogue guardDialogueTalk = new Dialogue();
		Dialogue foodSupplyDialogue = new Dialogue();
		Dialogue foodExpireDialogue = new Dialogue();
		Dialogue dialogue3 = new Dialogue();
		Dialogue dialogue4 = new Dialogue();
		Dialogue dialogue5 = new Dialogue();
		Dialogue dialogue6 = new Dialogue();
		dialogue2.SetText("Sergent Riggs", "Ticonderago was used to transport Nutrifruit products for pastel before it. When we got Jumped into the Subspace, the incident was just getting started. We explored the Subspace until our fuel run out hoping find a way to go back to civilisation. But we found nothing but emptiness.").NextDialogue().SetText("Sergent Riggs", "Eventually, our fuel run out and we sat still in the Subspace. With the cargo we were transporting we had plenty of food. As time go by we made modification to make the ship more livable. We started to find other ships in the Subspace. The smaller ships didn't have much and most joined our crew in exchange for food and room")
			.NextDialogue()
			.SetText("Sergent Riggs", "With the larger ships, we trade our Nutrifruit cargo for other things we need, specialy energy credit to run our electricals and spare parts to maintain our ship.")
			.AddChoice(DirectoryMaster.Item("ok"), "I see", "", guardDialogueTalk, delegate
			{
				guardDialogueTalk.AddChoice(DirectoryMaster.Item("question"), "How much of the original Nutrifruit cargo do you still have?", "", foodSupplyDialogue);
				guardDialogueTalk.AddChoice(DirectoryMaster.Item("question"), "Won't the food expire at some point?", "", foodExpireDialogue);
			});
		foodSupplyDialogue.SetText("Sergent Riggs", "I am not in charge of the food supply here, but as far as I am concerned, we still have plenty to trade. Did you see the size of the station? It was one of the largest superfreighters to be build and all of it was filled with food.").SetNextDialogue(guardDialogueTalk);
		foodExpireDialogue.SetText("Sergent Riggs", "pastel's food was designed for space travel. If properly stored in temperature and humidity controlled environment, the seal will keep the food fresh for a long long time.").SetNextDialogue(guardDialogueTalk);
		dialogue3.SetText("Sergent Riggs", "Me? Sergent John Riggs. Before the Jump, I was a corporate marine for pastel guarding their ship or stations against pirates. Now I am Head of Security here. Frankly, my live here is better than before. The station is counting on my team to keep them safe.").SetNextDialogue(guardDialogueTalk);
		dialogue4.SetText("Sergent Riggs", "Pirates, raiders... just like before the Jump. They are bolder than before because now, they are desperate.").SetNextDialogue(guardDialogueTalk);
		dialogue5.SetText("Sergent Riggs", "Captain Church is the woman in charge here. She was the captain even before I started working on Ticonderoga. She never lead us astray.").SetNextDialogue(guardDialogueTalk);
		dialogue6.SetText("Sergent Riggs", "Yes, Joma is the one in charge of selling our food reserve to outsiders. But if you are looking for anything else, look around the Market Level.").SetNextDialogue(guardDialogueTalk);
		guardDialogueTalk.SetText("Sergent Riggs", "Yes?").AddChoice(DirectoryMaster.Item("question"), "Can you tell me more about Ticonderoga?", "", dialogue2).AddChoice(DirectoryMaster.Item("question"), "What can you tell about yourself?", "", dialogue3)
			.AddChoice(DirectoryMaster.Item("question"), "What kind of threat do you have to deal with here?", "", dialogue4)
			.AddChoice(DirectoryMaster.Item("question"), "Do you have a leader? Who is in charge here.", "", dialogue5)
			.AddChoice(DirectoryMaster.Item("question"), "You mentioned something about trading?", "", dialogue6)
			.AddChoice(DirectoryMaster.Item("ok"), "Nevermind.");
		guardDialogue.SetText("Security Check Point", "*Your party walk into the room. A pair of heavly armed guards is manning the check point*").SetEndAction(delegate
		{
			guardDialogue.ForceDisable();
		}).NextDialogue()
			.SetText("Sergent Riggs", "Hello, this is Ticonderoga Station. If you are looking for trouble, I suggest to go back on your ship and leave now. Otherwise step inside, we have food, medical services and goods to trade, provide if you have the credits.")
			.AddChoice(DirectoryMaster.Item("ok"), "Understood")
			.AddChoice(DirectoryMaster.Item("question"), "I have some questions.", "", guardDialogueTalk, delegate
			{
			});
		list[0].SetDialogue(guardDialogueTalk);
		room3.dialogues.Add(guardDialogue);
		Room room4 = RoomFactory.CreateCustomRoom("Foyer", null, Room.Type.NPC);
		room4.FullyConnectToRoom(room3);
		GameCharacterItem npc = TiconderogaCharacters.CreateAbby();
		room4.AddNPC(npc);
		Dialogue foyerDialogue = new Dialogue();
		foyerDialogue.SetText("", "").NextDialogue().SetText("", "You see a person in worker's overall sitting against wall. They are having violent convulsions.")
			.SetEndAction(delegate
			{
				foyerDialogue.ForceDisable();
			});
		room4.dialogues.Add(foyerDialogue);
		Room room5 = RoomFactory.CreateCustomRoom("Visitor Hallway");
		room5.FullyConnectToRoom(room4);
		Room room6 = RoomFactory.CreateCustomRoom("pastel inc. Vending Machine Restaurant", null, Room.Type.NPC);
		room6.FullyConnectToRoom(room5);
		GameCharacterItem npc2 = TiconderogaCharacters.CreateJohnny();
		room6.AddNPC(npc2);
		RoomFactory.CreateCustomRoom("Bridge").FullyConnectToRoom(room5);
		Room room7 = RoomFactory.CreateCustomRoom("'Dr.' Banjo's Clinic");
		room7.FullyConnectToRoom(room5);
		Dialogue dialogue7 = new Dialogue();
		Dialogue dialogue8 = new Dialogue();
		Dialogue docDialogueWelcome = new Dialogue();
		GameCharacterItem gameCharacterItem = DirectoryMaster.CharacterItem("merchant");
		room7.AddNPC(gameCharacterItem);
		gameCharacterItem.name = "'Dr.' Banjo";
		gameCharacterItem.BarterEnable(TradeSheet.GetTraderJomaShop());
		gameCharacterItem.SetDialogue(docDialogueWelcome);
		dialogue7.SetText("'Dr.' Banjo'", "Of course I am. My licience might be suspended yes but I assure you there is no one more qualified to pratice medicine on Ticonderoga.").SetNextDialogue(docDialogueWelcome);
		dialogue8.SetText("'Dr.' Banjo'", "Sorry man. We don't have enough meds to trade. Matter of fact, if you have any surplus, go see Trader Joma, we will gladly trade them with you.").SetNextDialogue(docDialogueWelcome);
		docDialogueWelcome.SetText("'Dr.' Banjo", "Hello, welcome to my humble clinic! What can we do for you today?").SetEndAction(delegate
		{
			docDialogueWelcome.SetText("'Dr.' Banjo", "Was there anything else I can you help with today?");
		}).AddChoice(DirectoryMaster.Item("medical_proficiency"), "Medical Assistance")
			.AddChoice(DirectoryMaster.Item("blood_bag"), "'Donate blood'")
			.AddChoice(DirectoryMaster.Item("question"), "Are you even a real doctor?", "", dialogue7)
			.AddChoice(DirectoryMaster.Item("question"), "Do you sell any medical supplies?", "", dialogue8)
			.AddChoice(DirectoryMaster.Item("cancel"), "Nevermind");
		dialogue7.SetNextDialogue(docDialogueWelcome);
		Dialogue rootDialogue = new Dialogue();
		rootDialogue = new Dialogue();
		rootDialogue.SetText("'Dr.' Banjo's Clinic", "*You step into a cramped room with filled with cardboard boxes and medical tools. You see a man wear a labcoat in the back of the room*").SetEndAction(delegate
		{
			rootDialogue.ForceDisable();
		}).SetNextDialogue(docDialogueWelcome);
		room7.dialogues.Add(rootDialogue);
		Room room8 = RoomFactory.CreateCustomRoom("Trader's Joma", null, Room.Type.NPC);
		room8.FullyConnectToRoom(room5);
		Dialogue dialogue9 = new Dialogue();
		Dialogue jomaDialogueMain = new Dialogue();
		Dialogue dialogue10 = new Dialogue();
		Dialogue dialogue11 = new Dialogue();
		GameCharacterItem joma = DirectoryMaster.CharacterItem("merchant");
		room8.AddNPC(joma);
		joma.name = "Joma";
		joma.BarterEnable(TradeSheet.GetTraderJomaShop());
		joma.SetDialogue(jomaDialogueMain);
		dialogue9.SetText("Trader's Joma Shop", "*You step into a tiny room with a counter. A old man come out from the back room to greet you.*").SetNextDialogue(jomaDialogueMain);
		dialogue10.SetText("Joma", "*Joma lean on the counter* Ain't much to talk about. I was the Ticonderago's cargo master before the jump. Now I am in charge of Ticonderago's official trade. Making sure we are getting what we need, at a fair price.").NextDialogue().SetText("Joma", "Anyway, if you need to buy or sell here on Ticonderago, I am your guy.")
			.SetNextDialogue(jomaDialogueMain);
		dialogue11.SetText("Joma", "Right now, we need construction material and parts to keep the ship in one piece, energy credit to power it.").NextDialogue().SetText("Joma", "If you have any of those, I'll give you good prices on them.")
			.NextDialogue()
			.SetText("Joma", "Doc's been telling me that we are low on meds, if you have any, I am wiling to trade those too.")
			.SetNextDialogue(jomaDialogueMain);
		jomaDialogueMain.SetText("Joma", "Hello, looking to buy or sell?").SetEndAction(delegate
		{
			jomaDialogueMain.SetText("Joma", "Need anything else?");
		}).AddChoice(DirectoryMaster.Item("trade"), "Trade", "", null, delegate
		{
			joma.BarterShow();
		})
			.AddChoice(DirectoryMaster.Item("cancel"), "Nevermind")
			.AddChoice(DirectoryMaster.Item("question"), "Can you tell me about yourself?", "", dialogue10)
			.AddChoice(DirectoryMaster.Item("question"), "What does the Ticonderago need?", "", dialogue11);
		room8.dialogues.Add(dialogue9);
		return new Station(room);
	}

	public static Station GenerateStationFour()
	{
		List<Room> list = new List<Room>();
		Room room = new Room("Republic Military Garrison", Room.Type.Ship);
		list.Add(room);
		Room room2 = RoomFactory.CreateSecurityCheckpoint(Room.Type.NPC);
		room2.FullyConnectToRoom(room);
		room2.AddRaidersToRoom(EnemyFactory.CreatePremadeMilitaryTeam());
		list.Add(room2);
		Room room3 = RoomFactory.CreateCustomRoom("Hallway");
		room3.FullyConnectToRoom(room2);
		list.Add(room3);
		Room room4 = RoomFactory.CreateCafeteria();
		room4.FullyConnectToRoom(room3);
		list.Add(room4);
		Room room5 = RoomFactory.CreateBarack(Room.Type.NPC);
		room5.FullyConnectToRoom(room4);
		room5.AddRaidersToRoom(EnemyFactory.CreatePremadeMilitaryTeam());
		list.Add(room5);
		Room room6 = RoomFactory.CreateBarack();
		room6.FullyConnectToRoom(room5);
		list.Add(room6);
		Room room7 = RoomFactory.CreateCustomRoom("Level 2 Hallway");
		room7.FullyConnectToRoom(room3);
		list.Add(room7);
		Room room8 = RoomFactory.CreateBridge(Room.Type.NPC);
		room8.FullyConnectToRoom(room7);
		room8.AddRaidersToRoom(EnemyFactory.CreatePremadeMilitaryTeam());
		list.Add(room8);
		Room room9 = RoomFactory.CreateArmory();
		room9.FullyConnectToRoom(room8);
		list.Add(room9);
		Room room10 = RoomFactory.CreateMedbay();
		room10.FullyConnectToRoom(room7);
		list.Add(room10);
		Room room11 = RoomFactory.CreateOperationTheater();
		room11.FullyConnectToRoom(room10);
		list.Add(room11);
		Room room12 = RoomFactory.CreateCustomRoom("Level 3 Hallway");
		room12.FullyConnectToRoom(room7);
		list.Add(room12);
		Room room13 = RoomFactory.CreateEngineering(Room.Type.NPC);
		room13.FullyConnectToRoom(room12);
		room13.AddRaidersToRoom(EnemyFactory.CreatePremadeMilitaryTeam());
		list.Add(room13);
		Room room14 = RoomFactory.CreatePowerBay();
		room14.FullyConnectToRoom(room13);
		list.Add(room14);
		for (int i = 0; i < list.Count; i++)
		{
			list[i].SpawnAttachedLoot();
		}
		return new Station(room);
	}

	public static Station GenerateStationSarajevo()
	{
		return new Station(new Room("Sarajevo Station", Room.Type.Ship));
	}

	public static Station GenerateStationAmbaAmau()
	{
		Room room = new Room("Amba Amau Debug Station", Room.Type.Ship);
		Dialogue rootDialogue = new Dialogue();
		rootDialogue.SetText("Tutorial AI:", "Hello, welcome to space looter! In this first station you will learn how to loot and fight.").SetEndAction(delegate
		{
			rootDialogue.ForceDisable();
		}).NextDialogue()
			.SetText("Tutorial AI:", "This is a test dialogue...")
			.AddChoice(DirectoryMaster.Item("coin"), "Yes")
			.AddChoice(DirectoryMaster.Item("diversion_item_ability"), "No", "", new Dialogue().SetDummy().ForceDisable());
		room.dialogues.Add(rootDialogue);
		Room room2 = RoomFactory.CreateEntrance(Room.Type.NPC);
		room2.FullyConnectToRoom(room);
		List<GameCharacterItem> GCIs = room2.AddRaidersToRoom(EnemyFactory.CreatePremadeGearedGuards());
		GCIs[0].SetName("Test Trader");
		GCIs[1].SetName("Noob");
		List<GameItem> list = new List<GameItem>();
		list.Add(DirectoryMaster.Item("coin").SetAmount(10));
		list.Add(DirectoryMaster.Item("pill_bottle").TryAcceptAll(DirectoryMaster.Item("bicarsole_pill").SetAmount(6)));
		GCIs[0].BarterEnable(list);
		Dialogue tradeDialogue = new Dialogue();
		tradeDialogue.SetText("Test Trader:", "Do you want to trade?").SetEndAction(delegate
		{
			tradeDialogue.ForceDisable();
		}).AddChoice(DirectoryMaster.Item("coin"), "Yes", "Let's trade!", null, delegate
		{
			GCIs[0].BarterShow();
		})
			.AddChoice(DirectoryMaster.Item("diversion_item_ability"), "No", "", new Dialogue().SetDummy().ForceDisable());
		room2.dialogues.Add(tradeDialogue);
		return new Station(room);
	}

	public static Station ProceduralGenerateWaystation()
	{
		return new Station(new Room("Waystation Entrance", Room.Type.Ship));
	}
}
