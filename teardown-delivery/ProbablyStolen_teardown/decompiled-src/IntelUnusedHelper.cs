using System;
using System.Collections.Generic;

public class IntelUnusedHelper
{
	public const string INTEL_UNIQUE_ID = "INTEL_UNIQUE_ID";

	public const string INTEL_ID_INT = "INTEL_ID_INT";

	public const string INTEL_SUBJECT = "INTEL_SUBJECT";

	public const string INTEL_CONTENT = "INTEL_CONTENT";

	public const string INTEL_AUTHOR = "INTEL_AUTHOR";

	public const string INTEL_TITLE = "INTEL_TITLE";

	public const string SPAWNED_INTEL_TAG_LIST = "SPAWNED_INTEL_TAG_LIST";

	public static List<(string, Action<GameItem>)> intelInit = new List<(string, Action<GameItem>)>
	{
		("p1", InitPastelP1),
		("p2", InitPastelP2),
		("p3", InitPastelP3),
		("n1", InitNeuralN1),
		("w1", InitWatchW1)
	};

	public static void InitIntelItem(GameItem item, int id, string subject, string content, string author, string title)
	{
		item.ModifyTag("INTEL_ID_INT", delegate(TagState state)
		{
			state.Enable().SetInt(id);
		});
		item.ModifyTag("INTEL_SUBJECT", delegate(TagState state)
		{
			state.Enable().SetString(subject);
		});
		item.ModifyTag("INTEL_CONTENT", delegate(TagState state)
		{
			state.Enable().SetString(content);
		});
		item.ModifyTag("INTEL_AUTHOR", delegate(TagState state)
		{
			state.Enable().SetString(author);
		});
		item.ModifyTag("INTEL_TITLE", delegate(TagState state)
		{
			state.Enable().SetString(title);
		});
		item.EnableManufacturer(author);
		PixelWindow contentWindow = DirectoryUtils.CreateIntelWindow(title, author, content);
		item.SetContentWindow(contentWindow);
	}

	public static void InitPastelP1(GameItem item)
	{
		item.SetSpriteAndShape("Items/items_misc", "intel_blue");
		item.name = "Intel Report on pastel #1";
		item.shortDescription = "An intel folder with a non-fungible token certifying its authenticity. This one is a fundamental introduction on pastel.";
		item.unitValue = 200L;
		string title = "pastel, a National Security Risk";
		string content = "pastel deserve its title of megacorp better than any existing corporation.";
		InitIntelItem(item, 1, "pastel", content, "republic_navy", title);
	}

	public static void InitPastelP2(GameItem item)
	{
		item.SetSpriteAndShape("Items/items_misc", "intel_blue");
		item.name = "Intel Report on pastel #2";
		item.shortDescription = "An intel folder with a non-fungible token certifying its authenticity. This one is about pastel's greatest product, the Nutrifruit.";
		item.unitValue = 200L;
		string title = "The Nutrifruit";
		string content = "pastel deserve its title of megacorp better than any existing corporation.";
		InitIntelItem(item, 1, "pastel", content, "republic_navy", title);
	}

	public static void InitPastelP3(GameItem item)
	{
		item.SetSpriteAndShape("Items/items_misc", "intel_blue");
		item.name = "Intel Report on pastel #3";
		item.shortDescription = "An intel folder with a non-fungible token certifying its authenticity. This one is a fundamental introduction on pastel.";
		item.unitValue = 200L;
		string title = "Post Nutrifruit pastel, a transformation";
		string content = "pastel deserve its title of megacorp better than any existing corporation.";
		InitIntelItem(item, 1, "pastel", content, "republic_navy", title);
	}

	public static void InitNeuralN1(GameItem item)
	{
		item.SetSpriteAndShape("Items/items_misc", "intel_blue");
		item.name = "Intel Report on Neural Church #1";
		item.shortDescription = "An intel folder with a Republic Naval Intelligence non-fungible token certifiying its authenticity. This one is a fundamental introduction on the Neural Church.";
		item.unitValue = 200L;
		string title = "Neural Church";
		string content = "The Neural Church is a post-rift religious group who workship a powerful sentient AI as their God.  The AI's primary directive is to assist and improve Human's live. It is said that the AI create personalized nutrition plan to maintain a good mood and energy. It will design medication tailored to a individual's genetic to avoid any side effect. It act as an mental health council and can find the most compatible mate for its members. Those services provided to its members greatly improve their lives. Like many other post-rift religions and cults, the desperate situation of the Subspace make it popular quickly. The R.N.I. is currently not aware of the exact origin of the Neural Church. ";
		InitIntelItem(item, 1, "neural", content, "republic_navy", title);
	}

	public static void InitNeuralN2(GameItem item)
	{
		item.SetSpriteAndShape("Items/items_misc", "intel_blue");
		item.name = "Intel Report on Neural Church #2";
		item.shortDescription = "An intel folder with a Republic Naval Intelligence non-fungible token certifiying its authenticity. This intel is about Neural Church's Spacer Doc.";
		item.unitValue = 200L;
		string title = "Neural Church's Spacer Doc";
		string content = "The Neural Church is a religious group who workship a powerful sentient AI as their God.  The AI's primary directive is to assist and improve Human's live.";
		InitIntelItem(item, 1, "neural", content, "republic_navy", title);
	}

	public static void InitWatchW1(GameItem item)
	{
		item.SetSpriteAndShape("Items/items_misc", "intel_blue");
		item.name = "Intel Report on Neural Watch #1";
		item.shortDescription = "An intel folder with a non-fungible token certifying its authenticity. This one is a fundamental introduction on pastel.";
		item.unitValue = 200L;
		string title = "The Neural Watch";
		string content = "Considered to be a terrorist group. The Neural Watch is a extremist branch of the Neural Church. Just like the Church itself, it is a post-rift faction. The Watch believe that the Neural Church has become weak and that they are not doing enough to propagate their AI God. They believe that uniting all people under their AI God to be the most important thing and are ready resort to violence to reach their goals. The Watch has made attacks with goal to destabilize the Subspace until people turn to their AI God for salvation. The Neural Church itself has publicly rejected The Watch from the beginning and have denounced their actions. Nevertheless some officers of the RNI believe that there is some ties existing between the two groups.";
		InitIntelItem(item, 1, "watch", content, "republic_navy", title);
	}

	public static void InitRepublicR1(GameItem item)
	{
		item.SetSpriteAndShape("Items/items_misc", "intel_blue");
		item.name = "Intel Report on the Republic #1";
		item.shortDescription = "An intel folder with a non-fungible token certifying its authenticity. This one is an introduction on history of the republic.";
		item.unitValue = 200L;
		string title = "The Republic";
		string content = "pastel deserve its title of Megacorp better than any existing corporation.";
		InitIntelItem(item, 1, "republic", content, "republic_navy", title);
	}

	public static void InitRepublicR2(GameItem item)
	{
		item.SetSpriteAndShape("Items/items_misc", "intel_blue");
		item.name = "Intel Report on the Republic #2";
		item.shortDescription = "An intel folder with a non-fungible token certifying its authenticity. This one is an report on the state of the republic post-rift.";
		item.unitValue = 200L;
		string title = "The Republic, Post Rift.";
		string content = "pastel deserve its title of Megacorp better than any existing corporation.";
		InitIntelItem(item, 1, "republic", content, "republic_navy", title);
	}

	public static void InitRepublicR3(GameItem item)
	{
		item.SetSpriteAndShape("Items/items_misc", "intel_blue");
		item.name = "Intel Report on the Republic #3";
		item.shortDescription = "An intel folder with a non-fungible token certifying its authenticity. This one is an introduction on the Republic Naval Intelligence.";
		item.unitValue = 200L;
		string title = "The Republic Navy Intelligence";
		string content = "Nicknamed the Watchful Eyes of the Republic. The Republic Navy Intelligence (R.N.I.) is a Navy Component under the Fleet Forces Command. We are the oldest member of the Republic Intelligence Community. Our mission is gather information on the developments and activities of foreign navies; protecting the Republic's resources and interests; monitoring and countering threats; providing technical, operational, and tactical support to the Republic Navy and its partners;";
		InitIntelItem(item, 1, "republic", content, "republic_navy", title);
	}

	public static void InitRepublicR4(GameItem item)
	{
		item.SetSpriteAndShape("Items/items_misc", "intel_blue");
		item.name = "Intel Report on the Republic #4";
		item.shortDescription = "An intel folder with a non-fungible token certifying its authenticity. This one is classified briefing on the Republic Special Warfare Forces.";
		item.unitValue = 200L;
		string title = "The Republic Navy Intelligence";
		string content = "Nicknamed the Dagger of the Republic. The Special Warfare Forces is a unified command that oversee the Republic's special forces from all branches of the armed forces. Working closely with the Republic's Intelligence Community, they are relied upon during hostage crisis, counter-terror operation, anti-pirate operation, direct action and special reconnaissance.";
		InitIntelItem(item, 1, "republic", content, "republic_navy", title);
	}

	public static GameItem SpawnRandomNewIntel()
	{
		GameItem gameItem = DirectoryMaster.Item("intel_item");
		List<string> list = MetaHelper.RetrieveListInWorldTagSystem("SPAWNED_INTEL_TAG_LIST");
		if (list.Count == 0)
		{
			int randomInt = RNG.GetRandomInt(0, intelInit.Count - 1);
			intelInit[randomInt].Item2(gameItem);
			MetaHelper.AppendStringInWorldTagSystem("SPAWNED_INTEL_TAG_LIST", intelInit[randomInt].Item1);
			return gameItem;
		}
		List<(string, Action<GameItem>)> list2 = new List<(string, Action<GameItem>)>();
		foreach (var item in intelInit)
		{
			if (!list.Contains(item.Item1))
			{
				list2.Add(item);
			}
		}
		if (list2.Count == 0)
		{
			return gameItem;
		}
		int randomInt2 = RNG.GetRandomInt(0, list2.Count - 1);
		list2[randomInt2].Item2(gameItem);
		MetaHelper.AppendStringInWorldTagSystem("SPAWNED_INTEL_TAG_LIST", list2[randomInt2].Item1);
		return gameItem;
	}

	public static void SpawnDebugSupplyCrate()
	{
		GameGridInventory invElement = EmporiumEntry.Instance.invElement;
		GraphUtils.TryAcceptAll(invElement, DirectoryMaster.Item("sec_box"));
		GraphUtils.TryAcceptAll(invElement, DirectoryMaster.Item("evidence_box"));
		GraphUtils.TryAcceptAll(invElement, DirectoryMaster.Item("sec_keycard"));
		GraphUtils.TryAcceptAll(invElement, DirectoryMaster.Item("cmd_keycard"));
	}

	public static GameItem CreateTodoList()
	{
		string title = "Todo List";
		string body = "\n-Visit the Inventor\n-Acquire a Furnace to smelt Ingot from scrap metal\n-Visit the Supermarket\n";
		string signature = "";
		GameItem gameItem = DirectoryMaster.Item("tutorial_page");
		gameItem.SetSpriteAndShape("Items/itematlas", "page2");
		gameItem.onActivateSlotItemFunc = delegate
		{
			PaperUIManager.Instance.OpenUI(title, body, signature, 2);
		};
		gameItem.name = title;
		return gameItem;
	}

	public static GameItem CreateNoteSupplyCrate()
	{
		string text = "Mentor's Note: Marketplace Requests";
		string body = " \n Factions and individuals will publicly post time sensitive item bounties on Marketplace Requests. Sometime during shortages to keep their organization running.\n Fulfilling a Request will also boost your reputation with that faction.\n \n \n ";
		string signature = "-The Mentor";
		GameItem gameItem = DirectoryMaster.Item("tutorial_page");
		IntelHelper.InitTutorialPage(gameItem, text, body, signature);
		gameItem.name = text;
		return gameItem;
	}

	public static GameItem CreateMentorNote3()
	{
		string text = "Mentor's Note : Rent and The Landlord";
		string body = " The Landlord will come to collect the rent every 7 days. With the current market, your rent is likely to increase every week. \n Make sure you have the credits in hand, otherwise you will be evicted, and that's over for you. If you don't have the rent, the Landlord might be lenient on your first time and might collect it next week along with that's week rents";
		string signature = "-The Mentor";
		GameItem gameItem = DirectoryMaster.Item("tutorial_page");
		IntelHelper.InitTutorialPage(gameItem, text, body, signature);
		gameItem.name = text;
		return gameItem;
	}
}
