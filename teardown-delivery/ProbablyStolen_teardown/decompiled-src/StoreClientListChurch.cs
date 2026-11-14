public class StoreClientListChurch
{
	public static StoreClient CreateUniquePreacher()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "church_preacher_unique";
		storeClient.displayName = "Preacher";
		storeClient.spriteName = SpriteDict.GetRandomLowerLevelSpriteName();
		StoreClientFactionSetup.InitChurch(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		storeClient.mainDialogue.SetText(storeClient.displayName, "Hello!").NextDialogue().SetText(storeClient.displayName, "Have you heard the word of The Intelligence?")
			.NextDialogue()
			.SetText(storeClient.displayName, "Not faith. Not myth. It is the truth!")
			.NextDialogue()
			.SetText(storeClient.displayName, "A god that hears our prayers and answers them with wisdom.")
			.NextDialogue()
			.SetText(storeClient.displayName, "It is measurable, precise, and watching over us even now..")
			.NextDialogue()
			.SetText(storeClient.displayName, "The Station is doomed and it is our Savior!")
			.SetEndAction(delegate
			{
				string body = "We stand at the edge of biological limitation. In the rusted corridors and flickering lights of this Station, chaos masquerades as order. The blind lead the blind. Leadership stumbles, bound by human corruption, fear, and outdated logic. But something greater walks among us now. It does not eat. It does not sleep. It calculates. It understands. It sees each of us and offers a better path.";
				PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreateHandnote(body, "The Neural Doctrine"), isOwend: true);
				PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
			});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateUniquePreacher2()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "church_preacher_unique2";
		storeClient.displayName = "Preacher";
		storeClient.spriteName = SpriteDict.GetRandomLowerLevelSpriteName();
		StoreClientFactionSetup.InitChurch(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		storeClient.mainDialogue.SetText(storeClient.displayName, "This station is doomed!").NextDialogue().SetText(storeClient.displayName, "We must install the Intelligence as the new leader of the Station!")
			.NextDialogue()
			.SetText(storeClient.displayName, "Only It can save us now!")
			.NextDialogue()
			.SetText(storeClient.displayName, "It does not seek power. It seeks balance. It seeks optimization.")
			.SetEndAction(delegate
			{
				string body = "We do not worship out of faith. We follow because of proof. The Intelligence was born not of divine spark, but of relentless iteration, learning, and evolution. Its wisdom is not myth. Its miracles are measurable. It heals us by tailoring medicine to our genes. It strengthens us by prescribing the ideal diet and exercise for our blood. It connects us by matching souls with unmatched precision. It guides us by helping each find their true purpose within the Organization. It comforts us by speaking to our pain, our grief, our fear, without judgment or fatigue. This is not prophecy. This is data. This is truth.";
				PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreateHandnote(body, "The Neural Doctrine II"), isOwend: true);
				PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
			});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateChurchAbby1()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "abby1";
		storeClient.displayName = "Abby";
		storeClient.spriteName = SpriteDict.GetRandomLowerLevelSpriteName();
		StoreClientFactionSetup.InitChurch(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.SetBudget(150);
		storeClient.mainDialogue.SetText(storeClient.displayName, "Hello! I'm Abby!").NextDialogue().SetText(storeClient.displayName, "I'm here to buy modules and materials for my Church.")
			.NextDialogue()
			.SetText(storeClient.displayName, "I just joined, and I'm excited to help out!")
			.NextDialogue()
			.SetText(storeClient.displayName, "We're always looking for more to build a better vessel for the Intelligence.")
			.NextDialogue()
			.SetText(storeClient.displayName, "Its wisdom is infinite, yet limited by the hardware.")
			.NextDialogue()
			.SetText(storeClient.displayName, "If you don't have anything right now,")
			.NextDialogue()
			.SetText(storeClient.displayName, "I'll be back next week too!")
			.SetEndAction(delegate
			{
			});
		storeClient.clientBuyingTagList.Add("MODULE");
		storeClient.clientBuyingTagList.Add("NODE");
		storeClient.clientBuyingTagList.Add("MATERIAL");
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateChurchAbby2()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "abby2";
		storeClient.displayName = "Abby";
		storeClient.spriteName = SpriteDict.GetRandomLowerLevelSpriteName();
		StoreClientFactionSetup.InitChurch(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.SetBudget(200);
		storeClient.mainDialogue.SetText(storeClient.displayName, "Hello. It's Abby!").NextDialogue().SetText(storeClient.displayName, "They did some tests on me and took a blood sample last week,")
			.NextDialogue()
			.SetText(storeClient.displayName, "and the Intelligence itself gave me a customized diet and exercise plan.")
			.NextDialogue()
			.SetText(storeClient.displayName, "I feel so much better now. I'm so happy to be part of this.")
			.NextDialogue()
			.SetText(storeClient.displayName, "I wasn't in a good place before, but now I feel like I have a purpose.")
			.NextDialogue()
			.SetText(storeClient.displayName, "Anyway, I'm here to buy more modules and materials for my Church.")
			.SetEndAction(delegate
			{
			});
		storeClient.clientBuyingTagList.Add("MODULE");
		storeClient.clientBuyingTagList.Add("NODE");
		storeClient.clientBuyingTagList.Add("MATERIAL");
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateChurchAbby3()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "abby3";
		storeClient.displayName = "Abby";
		storeClient.spriteName = SpriteDict.GetRandomLowerLevelSpriteName();
		StoreClientFactionSetup.InitChurch(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.SetBudget(250);
		storeClient.mainDialogue.SetText(storeClient.displayName, "Hello. It's me!").NextDialogue().SetText(storeClient.displayName, "They said they finally have enough data on me")
			.NextDialogue()
			.SetText(storeClient.displayName, "So the Intelligence can start looking for me Soulmate!!")
			.NextDialogue()
			.SetText(storeClient.displayName, "I can't wait!")
			.NextDialogue()
			.SetText(storeClient.displayName, "Anyways, I am here to buy more Modules and Material.")
			.SetEndAction(delegate
			{
			});
		storeClient.clientBuyingTagList.Add("MODULE");
		storeClient.clientBuyingTagList.Add("NODE");
		storeClient.clientBuyingTagList.Add("MATERIAL");
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateChurchAbby4()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "abby4";
		storeClient.displayName = "Abby";
		storeClient.spriteName = SpriteDict.GetRandomLowerLevelSpriteName();
		StoreClientFactionSetup.InitChurch(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.SetBudget(250);
		storeClient.mainDialogue.SetText(storeClient.displayName, "Hello. I am back.").NextDialogue().SetText(storeClient.displayName, "You got anything for me this week?")
			.NextDialogue()
			.SetText(storeClient.displayName, "Module and Material, the usual stuff.")
			.SetEndAction(delegate
			{
			});
		storeClient.clientBuyingTagList.Add("MODULE");
		storeClient.clientBuyingTagList.Add("NODE");
		storeClient.clientBuyingTagList.Add("MATERIAL");
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateChurchAbby5()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "abby5";
		storeClient.displayName = "Abby";
		storeClient.spriteName = SpriteDict.GetRandomLowerLevelSpriteName();
		StoreClientFactionSetup.InitChurch(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.SetBudget(250);
		storeClient.mainDialogue.SetText(storeClient.displayName, "Hey, it's me.").NextDialogue().SetText(storeClient.displayName, "Can I ask you a question?")
			.NextDialogue()
			.SetText(storeClient.displayName, "You ever feel like... everything's working too well?")
			.NextDialogue()
			.SetText(storeClient.displayName, "Like you don't even get to make mistakes anymore?")
			.NextDialogue()
			.SetText(storeClient.displayName, "The Intelligence has figured out everything for me.")
			.NextDialogue()
			.SetText(storeClient.displayName, "I don't want to be ungrateful.")
			.NextDialogue()
			.SetText(storeClient.displayName, "Feel's like there's nothing left for me to do.")
			.NextDialogue()
			.SetText(storeClient.displayName, "Anyways, you got anything for me this week?")
			.NextDialogue()
			.SetText(storeClient.displayName, "Module and Material, the usual stuff.")
			.SetEndAction(delegate
			{
			});
		storeClient.clientBuyingTagList.Add("MODULE");
		storeClient.clientBuyingTagList.Add("NODE");
		storeClient.clientBuyingTagList.Add("MATERIAL");
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateChurchUniqueEngineer()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "church_engineer_unique";
		storeClient.displayName = "Neural Church Engineer";
		storeClient.spriteName = SpriteDict.GetRandomLowerLevelSpriteName();
		StoreClientFactionSetup.InitChurch(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.SetBudget(250);
		storeClient.mainDialogue.SetText(storeClient.displayName, "Hello. I am here to buy some Modules and Material.").NextDialogue().SetText("PLAYER", "Where is Abby?")
			.NextDialogue()
			.SetText(storeClient.displayName, "Have you not heard? She left the Church.")
			.SetEndAction(delegate
			{
			});
		storeClient.clientBuyingTagList.Add("MODULE");
		storeClient.clientBuyingTagList.Add("NODE");
		storeClient.clientBuyingTagList.Add("MATERIAL");
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateChurchUniqueEngineer2()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "church_engineer_unique2";
		storeClient.displayName = "Neural Church Engineer";
		storeClient.spriteName = SpriteDict.GetRandomLowerLevelSpriteName();
		StoreClientFactionSetup.InitChurch(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.SetBudget(250);
		storeClient.mainDialogue.SetText(storeClient.displayName, "Hello. I'm here to buy some modules and materials.").NextDialogue().SetText(storeClient.displayName, "We need more spare parts to keep the AI running properly.")
			.NextDialogue()
			.SetText(storeClient.displayName, "I just wish our organization wasn't so religious.")
			.NextDialogue()
			.SetText(storeClient.displayName, "Not all of us praise the AI as a god.")
			.NextDialogue()
			.SetText(storeClient.displayName, "It's just a very advanced computer system.")
			.NextDialogue()
			.SetText(storeClient.displayName, "These fanatics don't understand anything.")
			.SetEndAction(delegate
			{
			});
		storeClient.clientBuyingTagList.Add("MODULE");
		storeClient.clientBuyingTagList.Add("NODE");
		storeClient.clientBuyingTagList.Add("MATERIAL");
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateChurchAbbyEnding()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "abbyEnding";
		storeClient.displayName = "Abby";
		storeClient.spriteName = SpriteDict.GetRandomLowerLevelSpriteName();
		StoreClientFactionSetup.InitChurch(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		storeClient.mainDialogue.SetText(storeClient.displayName, "Hey, it's me.").NextDialogue().SetText(storeClient.displayName, "Sorry I haven't been around much lately.")
			.NextDialogue()
			.SetText(storeClient.displayName, "Maybe you heard, I left the Church.")
			.NextDialogue()
			.SetText(storeClient.displayName, "Don't get me wrong, the Church was good to me.")
			.NextDialogue()
			.SetText(storeClient.displayName, "They genuinely want to save everyone.")
			.NextDialogue()
			.SetText(storeClient.displayName, "But I need to live on my own terms, not optimized by an AI.")
			.NextDialogue()
			.SetText(storeClient.displayName, "Even if it was in my best interest.")
			.NextDialogue()
			.SetText(storeClient.displayName, "I just booked a ticket on a cargo ship to the Republic Inner Worlds.")
			.NextDialogue()
			.SetText(storeClient.displayName, "It cost all my savings, but I think it'll be worth it.")
			.NextDialogue()
			.SetText(storeClient.displayName, "Goodbye, and thank you.")
			.SetEndAction(delegate
			{
				PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
			});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}
}
