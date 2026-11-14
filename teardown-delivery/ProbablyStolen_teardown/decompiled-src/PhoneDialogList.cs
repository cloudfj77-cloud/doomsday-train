using UnityEngine;

public class PhoneDialogList : MonoBehaviour
{
	private const int RED_IMP_ORDER1_QUANTITY = 8;

	private const int RED_IMP_ORDER1_DELIVERY_DAYS = 1;

	private const int RED_IMP_ORDER2_QUANTITY = 16;

	private const int RED_IMP_ORDER2_DELIVERY_DAYS = 2;

	private const int RED_IMP_UNLOCK_THRESHOLD = 400;

	private const int CHEM_ORDER1_QUANTITY = 8;

	private const int CHEM_ORDER1_DELIVERY_DAYS = 1;

	private const int CHEM_ORDER2_QUANTITY = 16;

	private const int CHEM_ORDER2_DELIVERY_DAYS = 2;

	private const int CHEM_UNLOCK_THRESHOLD = 400;

	private const int MINING_ORDER1_QUANTITY = 6;

	private const int MINING_ORDER1_DELIVERY_DAYS = 1;

	private const int MINING_ORDER2_QUANTITY = 12;

	private const int MINING_ORDER2_DELIVERY_DAYS = 2;

	private const int MINING_UNLOCK_THRESHOLD = 350;

	private const int PHARMA_ORDER1_QUANTITY = 5;

	private const int PHARMA_ORDER1_DELIVERY_DAYS = 1;

	private const int PHARMA_ORDER2_QUANTITY = 9;

	private const int PHARMA_ORDER2_DELIVERY_DAYS = 2;

	private const int PHARMA_UNLOCK_THRESHOLD = 500;

	private const int REV_MERCHANT_DELIVERY_DAYS = 2;

	private const int INTRO_COURIER_DELAY = 1;

	public static Dialogue BugDialog()
	{
		Dialogue dialogue = new Dialogue();
		string title = "[BUG]";
		dialogue.SetText(title, "If you are seeing this, then there is a bug").SetEndAction(delegate
		{
			PhoneUIManager.Instance.OnPhoneDialogEnd();
		});
		return dialogue;
	}

	public static Dialogue PhoneDialogTest()
	{
		Dialogue dialogue = new Dialogue();
		string title = "[Unknown]";
		string title2 = "PLAYER";
		dialogue.SetText(title, "Hello, we know who you are.").NextDialogue().SetText(title, "We are Watching you.")
			.NextDialogue()
			.SetText(title2, "Who is this??")
			.SetEndAction(delegate
			{
				PhoneUIManager.Instance.OnPhoneDialogEnd();
			});
		return dialogue;
	}

	public static Dialogue PhoneDialogSec()
	{
		Dialogue dialogue = new Dialogue();
		string displayName = "Security Phone Operator";
		string title = "PLAYER";
		Dialogue dialogueReport = new Dialogue();
		dialogueReport.SetText(title, "[Report Wanted Criminal]");
		Dialogue dialogueStop = new Dialogue();
		dialogueStop.SetText(title, "[Stop]");
		dialogue.SetText(displayName, "This is Security Department").NextDialogue().SetText(displayName, "How may I help you.")
			.AddChoice(DirectoryMaster.Item("$"), "", "", dialogueReport, delegate
			{
				dialogueReport.NextDialogue().SetText(displayName, "Understand, We will check it out.").NextDialogue()
					.SetText(displayName, "We will let you know tonight how it went.")
					.SetEndAction(delegate
					{
						PlayerStore.Instance.OnCustomerReported();
						PhoneUIManager.Instance.OnPhoneDialogEnd();
					});
			})
			.AddChoice(DirectoryMaster.Item("$"), "", "", dialogueStop, delegate
			{
				dialogueStop.NextDialogue().SetText(displayName, "Do not waste our time.").NextDialogue()
					.SetText(displayName, "You can be fined for this.")
					.SetEndAction(delegate
					{
						PhoneUIManager.Instance.OnPhoneDialogEnd();
					});
			});
		return dialogue;
	}

	public static Dialogue CommissaryPhoneDialog()
	{
		Dialogue dialogue = new Dialogue();
		string title = "Commissary Automated Phone System";
		dialogue.SetText(title, "Hello, you have reached the Commissary Automated Phone System.").SetEndAction(delegate
		{
			PhoneUIManager.Instance.OnPhoneDialogEnd();
			PhoneUIManager.Instance.CloseUI();
			CommissaryUIManager.Instance.OpenUI();
		});
		return dialogue;
	}

	public static Dialogue PhoneWild()
	{
		Dialogue dialogue = new Dialogue();
		string displayName = LocHelper.GetLocalizedName("name_joe_assistant");
		string title = "PLAYER";
		Dialogue dialogueReport = new Dialogue();
		dialogueReport.SetText(title, LocHelper.GetLocalizedDialoguePhone("browse_wild_service"));
		Dialogue dialogueStop = new Dialogue();
		dialogueStop.SetText(title, LocHelper.GetLocalizedDialoguePhone("end_call"));
		Dialogue dialogue2 = dialogue.SetText(displayName, LocHelper.GetLocalizedDialoguePhone("wild1")).NextDialogue();
		dialogue2.SetText(displayName, LocHelper.GetLocalizedDialoguePhone("wild2")).AddChoice(DirectoryMaster.Item("$"), "", "", dialogueReport, delegate
		{
			dialogueReport.NextDialogue().SetText(displayName, LocHelper.GetLocalizedDialoguePhone("wild3")).SetStartAction(delegate
			{
				WildNetworkUI.Open();
				PhoneUIManager.Instance.CloseUI();
				PhoneUIManager.Instance.OnPhoneDialogEnd(playEndSound: false);
			});
		}).AddChoice(DirectoryMaster.Item("$"), "", "", dialogueStop, delegate
		{
			dialogueStop.NextDialogue().SetText(displayName, LocHelper.GetLocalizedDialoguePhone("goodbye")).SetEndAction(delegate
			{
				PhoneUIManager.Instance.OnPhoneDialogEnd();
			});
		});
		if (PhoneUIManager.Instance.isQuickDialCall)
		{
			return dialogue2;
		}
		return dialogue;
	}

	public static Dialogue RedImpRepIntro()
	{
		Dialogue dialogue = new Dialogue();
		string localizedName = LocHelper.GetLocalizedName("name_red_imp_rep");
		dialogue.SetText(localizedName, LocHelper.GetLocalizedDialoguePhone("red_imp_rep_intro1")).NextDialogue().SetText(localizedName, LocHelper.GetLocalizedDialoguePhone("red_imp_rep_intro2"))
			.NextDialogue()
			.SetText(localizedName, LocHelper.GetLocalizedDialoguePhone("red_imp_rep_intro3"))
			.NextDialogue()
			.SetText(localizedName, LocHelper.GetLocalizedDialoguePhone("red_imp_rep_intro4"))
			.NextDialogue()
			.SetText(localizedName, LocHelper.GetLocalizedDialoguePhone("red_imp_rep_intro5"))
			.NextDialogue()
			.SetText(localizedName, LocHelper.GetLocalizedDialoguePhone("red_imp_rep_intro6"))
			.SetEndAction(delegate
			{
				PhoneUIManager.Instance.OnPhoneDialogEnd();
				StorePhoneClient.GetBrewerPhoneClient().phoneState = StorePhoneClient.PhoneState.Regular;
				StorePhoneClient.GetBrewerPhoneClient().UseService();
				PlayerStore.Instance.QueueFuturClient("courrierRedImpFree", 1);
			});
		return dialogue;
	}

	public static Dialogue RedImpRepRegular()
	{
		Dialogue dialogue = new Dialogue();
		string displayName = LocHelper.GetLocalizedName("name_red_imp_rep");
		string title = "PLAYER";
		Dialogue order1 = new Dialogue();
		order1.SetText(title, LocHelper.GetLocalizedDialoguePhone("red_imp_order1", 8, 1));
		Dialogue order2 = new Dialogue();
		order2.SetText(title, LocHelper.GetLocalizedDialoguePhone("red_imp_order2", 16, 2));
		Dialogue dialogue2 = new Dialogue();
		dialogue2.SetText(displayName, LocHelper.GetLocalizedDialoguePhone("red_imp_rep3"));
		Dialogue quit = new Dialogue();
		quit.SetText(title, LocHelper.GetLocalizedDialoguePhone("end_call"));
		dialogue.SetText(displayName, LocHelper.GetLocalizedDialoguePhone("red_imp_rep2")).SetNextDialogue(dialogue2);
		dialogue2.AddChoice(DirectoryMaster.Item("$"), "", "", order1, delegate
		{
			StorePhoneClient.GetBrewerPhoneClient().stockType = 0;
			StorePhoneClient.GetBrewerPhoneClient().UseService();
			PlayerStore.Instance.QueueFuturClient("courrierRedImp", 1);
			order1.NextDialogue().SetText(displayName, LocHelper.GetLocalizedDialoguePhone("red_imp_rep4")).SetEndAction(delegate
			{
				PhoneUIManager.Instance.OnPhoneDialogEnd();
			});
		});
		Debug.Log("Amount Spent: " + StorePhoneClient.GetBrewerPhoneClient().amountSpent);
		if (StorePhoneClient.GetBrewerPhoneClient().amountSpent >= 400)
		{
			dialogue2.AddChoice(DirectoryMaster.Item("$"), "", "", order2, delegate
			{
				StorePhoneClient.GetBrewerPhoneClient().stockType = 1;
				StorePhoneClient.GetBrewerPhoneClient().UseService();
				PlayerStore.Instance.QueueFuturClient("courrierRedImp", 2);
				order2.NextDialogue().SetText(displayName, LocHelper.GetLocalizedDialoguePhone("red_imp_rep4")).SetEndAction(delegate
				{
					PhoneUIManager.Instance.OnPhoneDialogEnd();
				});
			});
		}
		dialogue2.AddChoice(DirectoryMaster.Item("$"), "", "", quit, delegate
		{
			quit.NextDialogue().SetText(displayName, LocHelper.GetLocalizedDialoguePhone("end_call")).SetEndAction(delegate
			{
				PhoneUIManager.Instance.OnPhoneDialogEnd();
			});
		});
		if (PhoneUIManager.Instance.isQuickDialCall)
		{
			return dialogue2;
		}
		return dialogue;
	}

	public static Dialogue RedImpRepCooldown()
	{
		Dialogue dialogue = new Dialogue();
		string localizedName = LocHelper.GetLocalizedName("name_red_imp_rep");
		dialogue.SetText(localizedName, LocHelper.GetLocalizedDialoguePhone("red_imp_rep2")).NextDialogue().SetText(localizedName, LocHelper.GetLocalizedDialoguePhone("red_imp_out1"))
			.NextDialogue()
			.SetText(localizedName, LocHelper.GetLocalizedDialoguePhone("red_imp_out2", StorePhoneClient.GetBrewerPhoneClient().currentCooldown))
			.NextDialogue()
			.SetText(localizedName, LocHelper.GetLocalizedDialoguePhone("red_imp_out3"))
			.SetEndAction(delegate
			{
				PhoneUIManager.Instance.OnPhoneDialogEnd();
			});
		return dialogue;
	}

	public static Dialogue ChemistIntro()
	{
		Dialogue dialogue = new Dialogue();
		string localizedName = LocHelper.GetLocalizedName("name_ashish");
		string title = "PLAYER";
		dialogue.SetText(localizedName, LocHelper.GetLocalizedDialoguePhone("ashish_intro1")).NextDialogue().SetText(localizedName, LocHelper.GetLocalizedDialoguePhone("ashish_intro2"))
			.NextDialogue()
			.SetText(localizedName, LocHelper.GetLocalizedDialoguePhone("ashish_intro3"))
			.NextDialogue()
			.SetText(localizedName, LocHelper.GetLocalizedDialoguePhone("ashish_intro4"))
			.NextDialogue()
			.SetText(title, LocHelper.GetLocalizedDialoguePhone("ashish_intro5"))
			.NextDialogue()
			.SetText(localizedName, LocHelper.GetLocalizedDialoguePhone("ashish_intro6"))
			.SetEndAction(delegate
			{
				PhoneUIManager.Instance.OnPhoneDialogEnd();
				StorePhoneClient.GetChemPhoneClient().phoneState = StorePhoneClient.PhoneState.Regular;
				StorePhoneClient.GetChemPhoneClient().UseService();
				PlayerStore.Instance.QueueFuturClient("courrierChemFree", 1);
			});
		return dialogue;
	}

	public static Dialogue ChemistRegular()
	{
		Dialogue dialogue = new Dialogue();
		string displayName = LocHelper.GetLocalizedName("name_ashish");
		string title = "PLAYER";
		Dialogue order1 = new Dialogue();
		order1.SetText(title, LocHelper.GetLocalizedDialoguePhone("chem_order1", 8, 1));
		Dialogue order2 = new Dialogue();
		order2.SetText(title, LocHelper.GetLocalizedDialoguePhone("chem_order2", 16, 2));
		Dialogue dialogue2 = new Dialogue();
		dialogue2.SetText(displayName, LocHelper.GetLocalizedDialoguePhone("ashish_regular2"));
		Dialogue quit = new Dialogue();
		quit.SetText(title, LocHelper.GetLocalizedDialoguePhone("end_call"));
		dialogue.SetText(displayName, LocHelper.GetLocalizedDialoguePhone("ashish_regular1")).SetNextDialogue(dialogue2);
		dialogue2.AddChoice(DirectoryMaster.Item("$"), "", "", order1, delegate
		{
			StorePhoneClient.GetChemPhoneClient().stockType = 0;
			StorePhoneClient.GetChemPhoneClient().UseService();
			PlayerStore.Instance.QueueFuturClient("courrierChem", 1);
			order1.NextDialogue().SetText(displayName, LocHelper.GetLocalizedDialoguePhone("ashish_ready")).SetEndAction(delegate
			{
				PhoneUIManager.Instance.OnPhoneDialogEnd();
			});
		});
		if (StorePhoneClient.GetChemPhoneClient().amountSpent >= 400)
		{
			dialogue2.AddChoice(DirectoryMaster.Item("$"), "", "", order2, delegate
			{
				StorePhoneClient.GetChemPhoneClient().stockType = 1;
				StorePhoneClient.GetChemPhoneClient().UseService();
				PlayerStore.Instance.QueueFuturClient("courrierChem", 2);
				order2.NextDialogue().SetText(displayName, LocHelper.GetLocalizedDialoguePhone("ashish_ready")).SetEndAction(delegate
				{
					PhoneUIManager.Instance.OnPhoneDialogEnd();
				});
			});
		}
		dialogue2.AddChoice(DirectoryMaster.Item("$"), "", "", quit, delegate
		{
			quit.NextDialogue().SetText(displayName, LocHelper.GetLocalizedDialoguePhone("goodbye")).SetEndAction(delegate
			{
				PhoneUIManager.Instance.OnPhoneDialogEnd();
			});
		});
		if (PhoneUIManager.Instance.isQuickDialCall)
		{
			return dialogue2;
		}
		return dialogue;
	}

	public static Dialogue ChemistCooldown()
	{
		Dialogue dialogue = new Dialogue();
		string localizedName = LocHelper.GetLocalizedName("name_ashish");
		dialogue.SetText(localizedName, LocHelper.GetLocalizedDialoguePhone("ashish_cooldown1")).NextDialogue().SetText(localizedName, LocHelper.GetLocalizedDialoguePhone("ashish_cooldown2"))
			.NextDialogue()
			.SetText(localizedName, LocHelper.GetLocalizedDialoguePhone("ashish_cooldown3", StorePhoneClient.GetChemPhoneClient().currentCooldown))
			.SetEndAction(delegate
			{
				PhoneUIManager.Instance.OnPhoneDialogEnd();
			});
		return dialogue;
	}

	public static Dialogue MiningIntro()
	{
		Dialogue dialogue = new Dialogue();
		string localizedName = LocHelper.GetLocalizedName("name_caulson");
		string title = "PLAYER";
		dialogue.SetText(localizedName, LocHelper.GetLocalizedDialoguePhone("caulson_intro1")).NextDialogue().SetText(localizedName, LocHelper.GetLocalizedDialoguePhone("caulson_intro2"))
			.NextDialogue()
			.SetText(localizedName, LocHelper.GetLocalizedDialoguePhone("caulson_intro3"))
			.NextDialogue()
			.SetText(localizedName, LocHelper.GetLocalizedDialoguePhone("caulson_intro4"))
			.NextDialogue()
			.SetText(localizedName, LocHelper.GetLocalizedDialoguePhone("caulson_intro5"))
			.NextDialogue()
			.SetText(title, LocHelper.GetLocalizedDialoguePhone("caulson_intro6"))
			.NextDialogue()
			.SetText(localizedName, LocHelper.GetLocalizedDialoguePhone("caulson_intro7"))
			.SetEndAction(delegate
			{
				PhoneUIManager.Instance.OnPhoneDialogEnd();
				StorePhoneClient.GetMiningPhoneClient().phoneState = StorePhoneClient.PhoneState.Regular;
				StorePhoneClient.GetMiningPhoneClient().UseService();
				PlayerStore.Instance.QueueFuturClient("courrierMiningFree", 1);
			});
		return dialogue;
	}

	public static Dialogue MiningRegular()
	{
		Dialogue dialogue = new Dialogue();
		string displayName = LocHelper.GetLocalizedName("name_caulson");
		string title = "PLAYER";
		Dialogue order1 = new Dialogue();
		order1.SetText(title, LocHelper.GetLocalizedDialoguePhone("mining_order1", 6, 1));
		Dialogue order2 = new Dialogue();
		order2.SetText(title, LocHelper.GetLocalizedDialoguePhone("mining_order2", 12, 2));
		Dialogue quit = new Dialogue();
		quit.SetText(title, LocHelper.GetLocalizedDialoguePhone("end_call"));
		Dialogue dialogue2 = new Dialogue();
		dialogue2.SetText(displayName, LocHelper.GetLocalizedDialoguePhone("caulson_regular2"));
		dialogue.SetText(displayName, LocHelper.GetLocalizedDialoguePhone("caulson_regular1")).SetNextDialogue(dialogue2);
		dialogue2.AddChoice(DirectoryMaster.Item("$"), "", "", order1, delegate
		{
			StorePhoneClient.GetMiningPhoneClient().stockType = 0;
			StorePhoneClient.GetMiningPhoneClient().UseService();
			PlayerStore.Instance.QueueFuturClient("courrierMining", 1);
			order1.NextDialogue().SetText(displayName, LocHelper.GetLocalizedDialoguePhone("caulson_ready")).SetEndAction(delegate
			{
				PhoneUIManager.Instance.OnPhoneDialogEnd();
			});
		});
		if (StorePhoneClient.GetMiningPhoneClient().amountSpent >= 350)
		{
			dialogue2.AddChoice(DirectoryMaster.Item("$"), "", "", order2, delegate
			{
				StorePhoneClient.GetMiningPhoneClient().stockType = 1;
				StorePhoneClient.GetMiningPhoneClient().UseService();
				PlayerStore.Instance.QueueFuturClient("courrierMining", 2);
				order2.NextDialogue().SetText(displayName, LocHelper.GetLocalizedDialoguePhone("caulson_ready")).SetEndAction(delegate
				{
					PhoneUIManager.Instance.OnPhoneDialogEnd();
				});
			});
		}
		dialogue2.AddChoice(DirectoryMaster.Item("$"), "", "", quit, delegate
		{
			quit.NextDialogue().SetText(displayName, LocHelper.GetLocalizedDialoguePhone("goodbye")).SetEndAction(delegate
			{
				PhoneUIManager.Instance.OnPhoneDialogEnd();
			});
		});
		if (PhoneUIManager.Instance.isQuickDialCall)
		{
			return dialogue2;
		}
		return dialogue;
	}

	public static Dialogue MiningCooldown()
	{
		Dialogue dialogue = new Dialogue();
		string localizedName = LocHelper.GetLocalizedName("name_caulson");
		string title = "PLAYER";
		dialogue.SetText(localizedName, LocHelper.GetLocalizedDialoguePhone("caulson_cooldown1")).NextDialogue().SetText(localizedName, LocHelper.GetLocalizedDialoguePhone("caulson_cooldown2"))
			.NextDialogue()
			.SetText(localizedName, LocHelper.GetLocalizedDialoguePhone("caulson_cooldown3", StorePhoneClient.GetMiningPhoneClient().currentCooldown))
			.NextDialogue()
			.SetText(localizedName, LocHelper.GetLocalizedDialoguePhone("caulson_cooldown4"))
			.NextDialogue()
			.SetText(title, LocHelper.GetLocalizedDialoguePhone("caulson_cooldown5"))
			.SetEndAction(delegate
			{
				PhoneUIManager.Instance.OnPhoneDialogEnd();
			});
		return dialogue;
	}

	public static Dialogue PharmaIntro()
	{
		Dialogue dialogue = new Dialogue();
		string localizedName = LocHelper.GetLocalizedName("name_dale");
		string title = "PLAYER";
		dialogue.SetText(localizedName, LocHelper.GetLocalizedDialoguePhone("dale_intro1")).NextDialogue().SetText(localizedName, LocHelper.GetLocalizedDialoguePhone("dale_intro2"))
			.NextDialogue()
			.SetText(localizedName, LocHelper.GetLocalizedDialoguePhone("dale_intro3"))
			.NextDialogue()
			.SetText(localizedName, LocHelper.GetLocalizedDialoguePhone("dale_intro4"))
			.NextDialogue()
			.SetText(localizedName, LocHelper.GetLocalizedDialoguePhone("dale_intro5"))
			.NextDialogue()
			.SetText(localizedName, LocHelper.GetLocalizedDialoguePhone("dale_intro6"))
			.NextDialogue()
			.SetText(localizedName, LocHelper.GetLocalizedDialoguePhone("dale_intro7"))
			.NextDialogue()
			.SetText(title, LocHelper.GetLocalizedDialoguePhone("dale_intro8"))
			.NextDialogue()
			.SetText(localizedName, LocHelper.GetLocalizedDialoguePhone("dale_intro9"))
			.SetEndAction(delegate
			{
				PhoneUIManager.Instance.OnPhoneDialogEnd();
				StorePhoneClient.GetPharmaPhoneClient().phoneState = StorePhoneClient.PhoneState.Regular;
				StorePhoneClient.GetPharmaPhoneClient().UseService();
				PlayerStore.Instance.QueueFuturClient("courrierPharmaFree", 1);
			});
		return dialogue;
	}

	public static Dialogue PharmaRegular()
	{
		Dialogue dialogue = new Dialogue();
		string displayName = LocHelper.GetLocalizedName("name_dale");
		string title = "PLAYER";
		Dialogue order1 = new Dialogue();
		order1.SetText(title, LocHelper.GetLocalizedDialoguePhone("pharma_order1", 5, 1));
		Dialogue order2 = new Dialogue();
		order2.SetText(title, LocHelper.GetLocalizedDialoguePhone("pharma_order2", 9, 2));
		Dialogue dialogue2 = new Dialogue();
		dialogue2.SetText(displayName, LocHelper.GetLocalizedDialoguePhone("dale_regular3"));
		Dialogue quit = new Dialogue();
		quit.SetText(title, LocHelper.GetLocalizedDialoguePhone("end_call"));
		dialogue.SetText(displayName, LocHelper.GetLocalizedDialoguePhone("dale_regular1")).NextDialogue().SetText(displayName, LocHelper.GetLocalizedDialoguePhone("dale_regular2"))
			.SetNextDialogue(dialogue2);
		dialogue2.AddChoice(DirectoryMaster.Item("$"), "", "", order1, delegate
		{
			StorePhoneClient.GetPharmaPhoneClient().UseService();
			StorePhoneClient.GetPharmaPhoneClient().stockType = 0;
			PlayerStore.Instance.QueueFuturClient("courrierPharma", 1);
			order1.NextDialogue().SetText(displayName, LocHelper.GetLocalizedDialoguePhone("dale_ready")).SetEndAction(delegate
			{
				PhoneUIManager.Instance.OnPhoneDialogEnd();
			});
		});
		if (StorePhoneClient.GetPharmaPhoneClient().amountSpent >= 500)
		{
			dialogue2.AddChoice(DirectoryMaster.Item("$"), "", "", order2, delegate
			{
				StorePhoneClient.GetPharmaPhoneClient().UseService();
				StorePhoneClient.GetPharmaPhoneClient().stockType = 1;
				PlayerStore.Instance.QueueFuturClient("courrierPharma", 2);
				order2.NextDialogue().SetText(displayName, LocHelper.GetLocalizedDialoguePhone("dale_ready")).SetEndAction(delegate
				{
					PhoneUIManager.Instance.OnPhoneDialogEnd();
				});
			});
		}
		dialogue2.AddChoice(DirectoryMaster.Item("$"), "", "", quit, delegate
		{
			quit.NextDialogue().SetText(displayName, LocHelper.GetLocalizedDialoguePhone("goodbye")).SetEndAction(delegate
			{
				PhoneUIManager.Instance.OnPhoneDialogEnd();
			});
		});
		if (PhoneUIManager.Instance.isQuickDialCall)
		{
			return dialogue2;
		}
		return dialogue;
	}

	public static Dialogue PharmaCooldown()
	{
		Dialogue dialogue = new Dialogue();
		string localizedName = LocHelper.GetLocalizedName("name_dale");
		string title = "PLAYER";
		dialogue.SetText(localizedName, LocHelper.GetLocalizedDialoguePhone("dale_cooldown1")).NextDialogue().SetText(localizedName, LocHelper.GetLocalizedDialoguePhone("dale_cooldown2", StorePhoneClient.GetPharmaPhoneClient().currentCooldown))
			.NextDialogue()
			.SetText(title, LocHelper.GetLocalizedDialoguePhone("dale_cooldown3"))
			.SetEndAction(delegate
			{
				PhoneUIManager.Instance.OnPhoneDialogEnd();
			});
		return dialogue;
	}

	public static Dialogue GenericCooldown(string displayName)
	{
		Dialogue dialogue = new Dialogue();
		dialogue.SetText(displayName, LocHelper.GetLocalizedDialoguePhone("generic_cooldown")).SetEndAction(delegate
		{
			PhoneUIManager.Instance.OnPhoneDialogEnd();
			PhoneUIManager.Instance.CloseUI();
		});
		return dialogue;
	}

	public static Dialogue RevMerchantPhoneDialog()
	{
		Dialogue dialogue = new Dialogue();
		string displayName = LocHelper.GetLocalizedName("name_odin");
		string title = "PLAYER";
		if (StoreReputation.IsPerkUnlocked("REV_BLACKLISTED"))
		{
			dialogue.SetText(displayName, LocHelper.GetLocalizedDialoguePhone("odin_blacklisted1")).NextDialogue().SetText(displayName, LocHelper.GetLocalizedDialoguePhone("odin_blacklisted2"))
				.NextDialogue()
				.SetText(displayName, LocHelper.GetLocalizedDialoguePhone("odin_blacklisted3"))
				.SetEndAction(delegate
				{
					PhoneUIManager.Instance.OnPhoneDialogEnd();
					StorePhoneClient.GetRevMerchantClient().phoneState = StorePhoneClient.PhoneState.NotResponding;
				});
			return dialogue;
		}
		if (StoreStation.Instance.storeEventManager.IsEventActive("crackdown"))
		{
			dialogue.SetText(displayName, LocHelper.GetLocalizedDialoguePhone("odin_crackdown1")).NextDialogue().SetText(displayName, LocHelper.GetLocalizedDialoguePhone("odin_crackdown2"))
				.SetEndAction(delegate
				{
					PhoneUIManager.Instance.OnPhoneDialogEnd();
				});
			return dialogue;
		}
		Dialogue yes = new Dialogue();
		yes.SetText(title, LocHelper.GetLocalizedDialoguePhone("odin_yes"));
		Dialogue no = new Dialogue();
		no.SetText(title, LocHelper.GetLocalizedDialoguePhone("odin_no"));
		dialogue.SetText(displayName, LocHelper.GetLocalizedDialoguePhone("odin_regular1")).AddChoice(DirectoryMaster.Item("$"), "", "", yes, delegate
		{
			StorePhoneClient.GetRevMerchantClient().UseService();
			PlayerStore.Instance.QueueFuturClient("wanted4Normal", 2);
			yes.NextDialogue().SetText(displayName, LocHelper.GetLocalizedDialoguePhone("odin_hold")).NextDialogue()
				.SetText(displayName, LocHelper.GetLocalizedDialoguePhone("odin_dropby", 2))
				.SetEndAction(delegate
				{
					PhoneUIManager.Instance.OnPhoneDialogEnd();
				});
		}).AddChoice(DirectoryMaster.Item("$"), "", "", no, delegate
		{
			no.NextDialogue().SetText(displayName, LocHelper.GetLocalizedDialoguePhone("odin_goodbye")).SetEndAction(delegate
			{
				PhoneUIManager.Instance.OnPhoneDialogEnd();
			});
		});
		return dialogue;
	}
}
