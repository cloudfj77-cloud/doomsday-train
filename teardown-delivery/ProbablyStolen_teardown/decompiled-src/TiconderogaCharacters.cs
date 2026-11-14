public class TiconderogaCharacters
{
	public static GameCharacterItem CreateAbby()
	{
		Dialogue dialogue = new Dialogue();
		Dialogue dialogue2 = new Dialogue();
		Dialogue abbyDialogueB = new Dialogue();
		Dialogue dialogue3 = new Dialogue();
		GameCharacterItem abby = DirectoryMaster.CharacterItem("merchant");
		abby.name = "Abby";
		abby.SetDialogue(dialogue);
		abby.health = 20;
		abbyDialogueB.SetText(abby.name, "Please...go get...Johnny...in the restaurant...");
		dialogue3.SetText(abby.name, "Oh it's you. Thank you for the help.");
		dialogue.SetText("", "*You rushed to the woman and asked her what's wrong*").NextDialogue().SetText(abby.name, "*The woman mutter a few words between convulsions.* Help...I am having an Immunotosis...attack... Please go ask the kid...Johnny...in the restaurant for help...")
			.AddChoice(DirectoryMaster.Item("question"), "A kid? Shouldn't I go fetch a doctor instead?", "", dialogue2)
			.AddChoice(DirectoryMaster.Item("ok"), "I'll go get him!")
			.AddChoice(DirectoryMaster.Item("cancel"), "I'll see what I can do, but no promises.")
			.SetEndAction(delegate
			{
				abby.SetDialogue(abbyDialogueB);
			});
		dialogue2.SetText("", "No! Just..go get Johnny...");
		return abby;
	}

	public static GameCharacterItem CreateJohnny()
	{
		Dialogue johnnyDialogue = new Dialogue();
		Dialogue dialogue = new Dialogue();
		Dialogue dialogue2 = new Dialogue();
		Dialogue dialogue3 = new Dialogue();
		Dialogue dialogue4 = new Dialogue();
		Dialogue johnnyAbbyDialogue = new Dialogue();
		new Dialogue();
		Dialogue johnnyAbbyAnswer1Dialogue = new Dialogue();
		Dialogue johnnyAbbyAnswer2Dialogue = new Dialogue();
		Dialogue johnnyAbbyAnswer3Dialogue = new Dialogue();
		GameCharacterItem johnny = DirectoryMaster.CharacterItem("merchant");
		johnny.name = "Johnny";
		johnny.SetDialogue(dialogue);
		johnnyAbbyAnswer1Dialogue.SetText(johnny.name, "My organization provide supplies to the people here that are otherwise unavailable, at a profit of course. We deal mostly in medications.").NextDialogue().SetText(johnny.name, "Now that you know our little secret. We will grant you access to our humble shop.")
			.SetEndAction(delegate
			{
				johnny.BarterEnable(TradeSheet.GetJohnnyShop());
				johnnyDialogue.AddChoice(DirectoryMaster.Item("trade"), "I'll like to browse your shop", "", null, delegate
				{
					johnny.BarterShow();
				});
			})
			.SetNextDialogue(johnnyDialogue);
		johnnyAbbyAnswer2Dialogue.SetText(johnny.name, "Because Banjo do not have the medication that she need. Unless you have a paper cut, there not much Banjo can do to help you.").SetNextDialogue(johnnyDialogue);
		johnnyAbbyAnswer3Dialogue.SetText(johnny.name, "She have a condition called Type B Immunotosis. I guess you are not familiar with it. It is a horrible neuro degenerative disease. Depending on what stage the patient is, the symptoms include: pain, headache, convulsions, memory loss, motor function loss and finally death. Poor Abby need to take her med periodically or else she will progress to the next stage. Don't worry stranger, only Type A is contagious.").SetNextDialogue(johnnyDialogue);
		johnnyAbbyDialogue.SetText(johnny.name, "That must be Abby. She run out of meds already? Don't worry I'll send someone for her").NextDialogue().SetText(johnny.name, "*Johnny make a hand signal and another kid that you didn't noticed before suddently step out of the shadow. He run toward the direction of Main Street with a tiny satch on his back.*")
			.NextDialogue()
			.SetText(johnny.name, "Thanks for bringing this to my attention.")
			.SetEndAction(delegate
			{
				johnnyDialogue.AddChoice(DirectoryMaster.Item("question"), "What kind of business are you running here?", "", johnnyAbbyAnswer1Dialogue);
				johnnyDialogue.AddChoice(DirectoryMaster.Item("question"), "Why did Abby ask me to get you instead of a doctor?", "", johnnyAbbyAnswer2Dialogue);
				johnnyDialogue.AddChoice(DirectoryMaster.Item("question"), "What's wrong with Abby?", "", johnnyAbbyAnswer3Dialogue);
				johnnyAbbyDialogue.ForceHide();
			})
			.SetNextDialogue(johnnyDialogue);
		dialogue.SetText("", "*You approch Johnny, a kid who is sitting in the restaurant's corner, eating some Nutrichips. He is carefully eyeing you.*").NextDialogue().SetText(johnny.name, "Hi outsider. I am Johnny. What bring you to Ticonderago?")
			.SetEndAction(delegate
			{
				johnny.SetDialogue(johnnyDialogue);
			})
			.AddChoice(DirectoryMaster.Item("talk"), "We are here to trade.", "", dialogue2)
			.AddChoice(DirectoryMaster.Item("talk"), "We are visiting the station.", "", dialogue3)
			.AddChoice(DirectoryMaster.Item("cancel"), "That's not your business, kid.", "", dialogue4);
		dialogue2.SetText(johnny.name, "Then I guess you already met Joma. Unless you are here to buy food. There isn't much to trade.").SetNextDialogue(johnnyDialogue);
		dialogue3.SetText(johnny.name, "It's big, right? The station, and all of it was loaded with Nutrifruit. Captain Church said there enough food for us for 100 years. Can you believe it?").SetNextDialogue(johnnyDialogue);
		dialogue4.SetText(johnny.name, "Yea, whatever man.").NextDialogue().SetText("", "*Johnny goes back eating his chips*");
		johnnyDialogue.SetText(johnny.name, "Was there anything else?").SetEndAction(delegate
		{
			johnnyDialogue.SetText(johnny.name, "Was there anything else?");
		}).AddChoice(DirectoryMaster.Item("talk"), "There's woman who's having some kind of attack in Main Street who need your help!", "", johnnyAbbyDialogue)
			.AddChoice(DirectoryMaster.Item("cancel"), "'Nevermind.'");
		return johnny;
	}
}
