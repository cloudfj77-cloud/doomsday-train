using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class CreditsUI
{
	private struct TeamRow
	{
		public string roleKey;

		public string name;

		public string roleEnglish;

		public string Role
		{
			get
			{
				if (roleEnglish != null)
				{
					return LocHelper.GetLocalizedOrDefault("MainMenu", roleKey, roleEnglish);
				}
				return LocHelper.GetLocalizedMainMenu(roleKey);
			}
		}

		public TeamRow(string roleKey, string name, string roleEnglish = null)
		{
			this.roleKey = roleKey;
			this.name = name;
			this.roleEnglish = roleEnglish;
		}
	}

	private struct CreditGroup
	{
		public string labelKey;

		public string label;

		public string[] names;

		public string Label
		{
			get
			{
				if (labelKey != null)
				{
					return LocHelper.GetLocalizedOrDefault("MainMenu", labelKey, label);
				}
				return label;
			}
		}

		public CreditGroup(string labelKey, string label, params string[] names)
		{
			this.labelKey = labelKey;
			this.label = label;
			this.names = names;
		}
	}

	public const string WINDOW_ID = "credits";

	private static CustomUIWindow window;

	private static CreditsFrame frame;

	private static readonly TeamRow[] TEAM = new TeamRow[10]
	{
		new TeamRow("credit_lead", "Bei Ning Pan"),
		new TeamRow("lead_art", "Elena Nazaire"),
		new TeamRow("character_artist", "MirrorFloria"),
		new TeamRow("credit_artist", "Yan \"Nucleose\" Rodriguex"),
		new TeamRow("credit_artist", "Tappyowo"),
		new TeamRow("composer", "A.L.I.S.O.N"),
		new TeamRow("composer", "M. Harvey Bee"),
		new TeamRow("sound_designer", "Vasiliy \"Mushroomer\" Kashnikov"),
		new TeamRow("sound_designer", "Sam \"Fauxless\" Dalwood"),
		new TeamRow("ui_designer", "Zakhareykin Sergey", "UI Designer")
	};

	private static readonly string[] ACKNOWLEDGEMENT_KEYS = new string[3] { "credit_inspo", "credit_zapsplat", "thanks_bowen" };

	private const string COMMUNITY = "Maslak, Alma Pons, AdrianNumbers, aleksandermg, Cahyo Melki, Ceurelian, Ch3kh0v, Chloroplaste, D E A N, DkRule, Eleanora, Grub, Honeydew81, Honeypot, Jack, Jmanis Catharg, Kingcat, l0v3rm4n69420, Lusatia, McVirusSempai, Ninija159, Persil, Qay, Resident_Neutral, Swat, TTX515, WREN_PL";

	private const string PUBLISHER = "tinyBuild";

	private const string LOC_AGENCY = "From the Void";

	private static readonly CreditGroup[] PUBLISHING = new CreditGroup[32]
	{
		new CreditGroup("credit_pub_ceo", "CEO", "Alex Nichiporchik"),
		new CreditGroup("credit_pub_cfo", "Chief Financial Officer", "Giasone Salati"),
		new CreditGroup("credit_pub_exec_assistant", "Executive Assistant", "Annette Patent"),
		new CreditGroup("credit_pub_bizdev_director", "Director of Business Development and Partner Licensing", "Corey Caplan"),
		new CreditGroup("credit_pub_sales_director", "Sales Director", "Michael Kuzmin"),
		new CreditGroup("credit_pub_head_publishing", "Head of Publishing", "Artem Bochkarev"),
		new CreditGroup("credit_pub_exec_producer", "Executive Producer", "Mike Rafiienko", "Anton Pavlov", "Dmitry Yashanov"),
		new CreditGroup("credit_pub_producer", "Producer", "Vladimir Tolmachev", "Aleksei Glebov", "Artyom Safarov"),
		new CreditGroup("credit_pub_associate_producer", "Associate Producer", "Nadya Zhuk"),
		new CreditGroup("credit_pub_senior_marketing_manager", "Senior Marketing Manager", "Anton Daty"),
		new CreditGroup("credit_pub_marketing_manager", "Marketing Manager", "Sergey Smirnov", "Arnaud Richard"),
		new CreditGroup("credit_pub_head_influencer", "Head of Influencer Management", "Koen Rebel"),
		new CreditGroup("credit_pub_senior_influencer_manager", "Senior Influencer Manager", "Vera Lubbers"),
		new CreditGroup("credit_pub_senior_community_manager", "Senior Community Manager", "George Kulko", "James Croucher"),
		new CreditGroup("credit_community_manager", "Community Manager", "Tina Benoit"),
		new CreditGroup("credit_pub_social_media_manager", "Social Media Marketing Manager", "Shunise Wise", "Francesca Falcini", "Tori Gerbeshi", "Anna Bienek"),
		new CreditGroup("credit_pub_user_acquisition_manager", "User Acquisition Manager", "Artem Peganov"),
		new CreditGroup("credit_pub_porting_producer", "Porting Producer", "Aleksandra Akimova"),
		new CreditGroup("credit_pub_liveops_producer", "LiveOps Producer", "Alex Leoveanu"),
		new CreditGroup("credit_pub_head_dev_services", "Head of Development Services", "Artem Chernyshev"),
		new CreditGroup("credit_pub_release_manager", "Release Manager", "Artem Pozhilenkov", "Bradley Manning"),
		new CreditGroup("credit_pub_localization_manager", "Localization Manager", "Lisa Sidorova", "Alina Aliabieva"),
		new CreditGroup("credit_pub_discord_manager", "Discord Manager", "Jacky Motta"),
		new CreditGroup("credit_pub_support_manager", "Customer Support Manager", "Kimon Sklavounos", "Oleksandr Striuk"),
		new CreditGroup("credit_pub_qa_manager", "QA Manager", "Tiberiu Cristea", "George Popa"),
		new CreditGroup("credit_pub_head_media", "Head of Media Production", "Igor Surov"),
		new CreditGroup("credit_pub_media_project_manager", "Media Project Manager", "Christina Osipova"),
		new CreditGroup("credit_pub_trailer_producer", "Trailer Producer", "Nikita Varnakov", "Alexander Isaev"),
		new CreditGroup("credit_pub_video_editor", "Video Editor", "Eugene Chataev"),
		new CreditGroup("credit_pub_graphic_designer", "Graphic Designer", "Tatiana Kurguzova"),
		new CreditGroup("credit_pub_head_hr", "Head of HR", "Olga Barlet"),
		new CreditGroup("credit_pub_sysadmin_devops", "System Administrator / DevOps Engineer", "Valerii Zotov")
	};

	private static readonly CreditGroup[] LOCALIZATION = new CreditGroup[10]
	{
		new CreditGroup("credit_loc_director", "Director & Project Manager", "Marc Eybert-Guillon"),
		new CreditGroup(null, "Simplified Chinese", "Lin Feiyang \"Luminous\"", "Ma Mofan \"Sirius Wolf\""),
		new CreditGroup(null, "Japanese", "Yuri Tanaka", "Lina Mitsufuji"),
		new CreditGroup(null, "Russian", "Dmitry Kornyukhov", "Arty Ra"),
		new CreditGroup(null, "German", "Gesine Preusser", "Mareica Hauck", "Johannes Irmer", "Christina Milner", "Nadine Leonhardt"),
		new CreditGroup(null, "French", "Lucie Teulières", "Sarah Deville", "Aurélie Perrin"),
		new CreditGroup(null, "Polish", "Wojciech Brudziński", "Michał Piwowarczyk", "Małgorzata Duszka"),
		new CreditGroup(null, "Spanish", "Yasmina Casado González", "Keru Gesé Ponce", "Curri Barceló"),
		new CreditGroup("credit_community_manager", "Community Manager", "Laureline Denis-Venuat"),
		new CreditGroup("credit_team_mascot", "Team Mascot", "Indy")
	};

	public static bool IsOpen
	{
		get
		{
			if (window != null && window.IsAlive)
			{
				return window.IsVisible;
			}
			return false;
		}
	}

	public static void Open()
	{
		if (window != null && window.IsAlive)
		{
			window.Show();
			return;
		}
		if (CustomUIManager.Instance == null)
		{
			Debug.LogError("[CreditsUI] No CustomUIManager in the scene, cannot open the credits");
			return;
		}
		if (CustomUIManager.Instance.creditsPrefab == null)
		{
			Debug.LogError("[CreditsUI] CustomUIManager.creditsPrefab is not assigned. Run Build > Generate Credits UI Prefab in the menu scene.");
			return;
		}
		window = CustomUIManager.Instance.CreateWindow("credits", null, "menu", CustomUIManager.Instance.creditsPrefab).SetCloseOnEscape().OnClose(OnWindowClosed)
			.GetWindow();
		frame = ((window.root != null) ? window.root.GetComponent<CreditsFrame>() : null);
		if (frame == null)
		{
			Debug.LogError("[CreditsUI] creditsPrefab has no CreditsFrame on its root; regenerate it");
			window.Close();
			return;
		}
		frame.headerTitle.text = LocHelper.GetLocalizedMainMenu("credit");
		frame.backButton.onClick.AddListener(NewGameUIElements.PlayClick);
		BuildSections();
		window.Show();
		if (frame.scroll != null)
		{
			frame.scroll.verticalNormalizedPosition = 1f;
		}
		Canvas.ForceUpdateCanvases();
		if (frame.scroll != null)
		{
			frame.scroll.verticalNormalizedPosition = 1f;
		}
	}

	public static void Close()
	{
		if (window != null && window.IsAlive)
		{
			window.Close();
			return;
		}
		window = null;
		frame = null;
	}

	private static void OnWindowClosed()
	{
		window = null;
		frame = null;
	}

	private static void BuildSections()
	{
		NewGameUIElements.ClearChildren(frame.content);
		SectionTitle(LocHelper.GetLocalizedMainMenu("dev_team"));
		Rule();
		RectTransform rows = Rows("TeamRows");
		TeamRow[] tEAM = TEAM;
		for (int i = 0; i < tEAM.Length; i++)
		{
			TeamRow teamRow = tEAM[i];
			TeamRowElement(rows, teamRow.Role, teamRow.name);
		}
		SectionTitle(LocHelper.GetLocalizedMainMenu("credit_ack"));
		Rule();
		string[] array = new string[ACKNOWLEDGEMENT_KEYS.Length];
		for (int j = 0; j < ACKNOWLEDGEMENT_KEYS.Length; j++)
		{
			array[j] = LocHelper.GetLocalizedMainMenu(ACKNOWLEDGEMENT_KEYS[j]);
		}
		Paragraph("Acknowledgement", string.Join("\n", array), 22f, TextAlignmentOptions.TopLeft);
		SectionTitle(LocHelper.GetLocalizedMainMenu("credit_community"));
		Rule();
		Paragraph("Community", "Maslak, Alma Pons, AdrianNumbers, aleksandermg, Cahyo Melki, Ceurelian, Ch3kh0v, Chloroplaste, D E A N, DkRule, Eleanora, Grub, Honeydew81, Honeypot, Jack, Jmanis Catharg, Kingcat, l0v3rm4n69420, Lusatia, McVirusSempai, Ninija159, Persil, Qay, Resident_Neutral, Swat, TTX515, WREN_PL", 22f, TextAlignmentOptions.TopLeft);
		Paragraph("CommunityNote", LocHelper.GetLocalizedMainMenu("credit_community_note"), 24f, TextAlignmentOptions.Top);
		CompanySection(LocHelper.GetLocalizedOrDefault("MainMenu", "credit_publishing", "Publishing"), "tinyBuild", PUBLISHING);
		CompanySection(LocHelper.GetLocalizedOrDefault("MainMenu", "credit_localization", "Localization"), "From the Void", LOCALIZATION);
	}

	private static void CompanySection(string title, string company, CreditGroup[] groups)
	{
		SectionTitle(title);
		Rule();
		Paragraph("Company", company, 24f, TextAlignmentOptions.Top);
		for (int i = 0; i < groups.Length; i++)
		{
			CreditGroup creditGroup = groups[i];
			RectTransform rows = Rows("GroupRows");
			for (int j = 0; j < creditGroup.names.Length; j++)
			{
				TeamRowElement(rows, (j == 0) ? creditGroup.Label : "", creditGroup.names[j]);
			}
		}
	}

	private static void SectionTitle(string label)
	{
		TextMeshProUGUI textMeshProUGUI = NewGameUIElements.NewText(frame, frame.content, "SectionTitle", 0f, 0f, 900f, 38f, FontLoader.FontRole.Noto_Sans_Normal, 28f, NewGameStyle.CREDITS_SECTION_TITLE, TextAlignmentOptions.Center, wrap: false);
		textMeshProUGUI.fontStyle = FontStyles.UpperCase;
		textMeshProUGUI.text = label;
		textMeshProUGUI.gameObject.AddComponent<LayoutElement>().preferredHeight = 38f;
	}

	private static void Rule()
	{
		NewGameUIElements.NewImage(frame.content, "Rule", 0f, 0f, 900f, 2f, null, NewGameStyle.SEPARATOR).gameObject.AddComponent<LayoutElement>().preferredHeight = 2f;
	}

	private static RectTransform Rows(string name)
	{
		RectTransform rectTransform = NewGameUIElements.NewRect(frame.content, name, 0f, 0f, 900f, 0f);
		VerticalLayoutGroup verticalLayoutGroup = rectTransform.gameObject.AddComponent<VerticalLayoutGroup>();
		verticalLayoutGroup.spacing = 8f;
		verticalLayoutGroup.childAlignment = TextAnchor.UpperLeft;
		verticalLayoutGroup.childControlWidth = true;
		verticalLayoutGroup.childControlHeight = true;
		verticalLayoutGroup.childForceExpandWidth = true;
		verticalLayoutGroup.childForceExpandHeight = false;
		return rectTransform;
	}

	private static void TeamRowElement(RectTransform rows, string role, string name)
	{
		RectTransform rectTransform = NewGameUIElements.NewRect(rows, "Row", 0f, 0f, 900f, 33f);
		rectTransform.gameObject.AddComponent<LayoutElement>().preferredHeight = 33f;
		TextMeshProUGUI textMeshProUGUI = NewGameUIElements.NewText(frame, rectTransform, "Role", 0f, 0f, 450f, 33f, FontLoader.FontRole.Noto_Sans_Normal, 24f, NewGameStyle.CREDITS_TEXT, TextAlignmentOptions.MidlineLeft, wrap: false);
		textMeshProUGUI.text = role;
		WildNetworkUI.AutoSize(textMeshProUGUI, 12f);
		TextMeshProUGUI textMeshProUGUI2 = NewGameUIElements.NewText(frame, rectTransform, "Name", 450f, 0f, 450f, 33f, FontLoader.FontRole.Noto_Sans_Normal, 24f, NewGameStyle.CREDITS_TEXT, TextAlignmentOptions.MidlineLeft, wrap: false);
		textMeshProUGUI2.text = name;
		WildNetworkUI.AutoSize(textMeshProUGUI2, 12f);
	}

	private static void Paragraph(string name, string body, float size, TextAlignmentOptions alignment)
	{
		NewGameUIElements.NewText(frame, frame.content, name, 0f, 0f, 900f, 0f, FontLoader.FontRole.Noto_Sans_Normal, size, NewGameStyle.CREDITS_TEXT, alignment, wrap: true).text = body;
	}
}
