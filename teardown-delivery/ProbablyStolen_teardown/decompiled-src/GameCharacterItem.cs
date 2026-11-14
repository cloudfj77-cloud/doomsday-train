using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameCharacterItem : GameItemElement, CombatEntity, DirectoryEntry
{
	public enum CombatStates
	{
		IDLE,
		HIPATTACK,
		HURT,
		MELEEATTACK,
		AIMATTACK,
		BLOCK
	}

	public const string SPECIES_TAG = "SPECIES_TAG";

	public const string SPECIES_XENO = "SPECIES_XENO";

	public const string SPECIES_HUMAN = "SPECIES_HUMAN";

	public const string SPECIES_ROBOT = "SPECIES_ROBOT";

	public const string GCI_SEATED_IN_SHIP_TAG = "GCI_SEATED_IN_SHIP_TAG";

	private readonly TreeNodeRender healthBarOutline;

	private readonly TreeNodeRender healthBarBackground;

	private readonly TreeNodeRender healthBar;

	private readonly TreeNodeRender healthIcon;

	private readonly TreeNodeRender moraleBarOutline;

	private readonly TreeNodeRender moraleBarBackground;

	private readonly TreeNodeRender moraleBar;

	private readonly TreeNodeRender moraleIcon;

	public bool isOrganMod;

	public int lowMorale = 20;

	public int lowHealth = 20;

	public bool isDeadLogic;

	public static int stateSize = Enum.GetNames(typeof(CombatStates)).Length;

	public Sprite[] stateBack = new Sprite[stateSize];

	public Sprite[] stateFront = new Sprite[stateSize];

	private bool isLocked;

	public static string TAG_NOT_PURCHASED = "not_purchased";

	public static string TAG_BARTER_INIT = "barter_init";

	public TreeNodeRender characterBackground { get; protected set; }

	public TreeNodeRender characterBackgroundFront { get; protected set; }

	public TreeNodeRender characterFakeBackground { get; protected set; }

	public TreeNodeRender characterFakeBackgroundFront { get; protected set; }

	public bool isCard { get; private set; }

	private GameSlotInventory backpackInventorySlot { get; }

	private GameSlotInventory primaryWeaponSlot { get; }

	private GameSlotInventory secondaryWeaponSlot { get; }

	public GameSlotInventory sideSlot1 { get; }

	public GameSlotInventory sideSlot2 { get; }

	private bool characterDone { get; set; }

	public bool isPlayerControlled { get; set; }

	public bool isDead { get; set; }

	public List<CombatAbilityEffect> abilityEffects { get; set; }

	public CombatAbility currentSelectedAbility { get; set; }

	public List<CombatAbilityEffect> abilityEffectsToBeApplied { get; set; }

	public CombatAI combatAI { get; set; }

	public int reaction
	{
		get
		{
			return GetStat(CombatEntity.EffectType.reaction);
		}
		set
		{
			SetStat(CombatEntity.EffectType.reaction, value);
		}
	}

	public int health
	{
		get
		{
			return GetStat(CombatEntity.EffectType.health);
		}
		set
		{
			SetStat(CombatEntity.EffectType.health, value);
		}
	}

	public int maxHealth
	{
		get
		{
			return GetStat(CombatEntity.EffectType.maxHealth);
		}
		set
		{
			SetStat(CombatEntity.EffectType.maxHealth, value);
		}
	}

	public int action
	{
		get
		{
			return GetStat(CombatEntity.EffectType.action);
		}
		set
		{
			SetStat(CombatEntity.EffectType.action, value);
		}
	}

	public int maxAction
	{
		get
		{
			return GetStat(CombatEntity.EffectType.maxAction);
		}
		set
		{
			SetStat(CombatEntity.EffectType.maxAction, value);
		}
	}

	public int accuracyPercentage
	{
		get
		{
			return GetStat(CombatEntity.EffectType.accuracyPercentage);
		}
		set
		{
			SetStat(CombatEntity.EffectType.accuracyPercentage, value);
		}
	}

	public int damagePercentage
	{
		get
		{
			return GetStat(CombatEntity.EffectType.damagePercentage);
		}
		set
		{
			SetStat(CombatEntity.EffectType.damagePercentage, value);
		}
	}

	public int deathchance
	{
		get
		{
			return GetStat(CombatEntity.EffectType.deathchance);
		}
		set
		{
			SetStat(CombatEntity.EffectType.deathchance, value);
		}
	}

	public int criticalChanceDealing
	{
		get
		{
			return GetStat(CombatEntity.EffectType.criticalChanceDealing);
		}
		set
		{
			SetStat(CombatEntity.EffectType.criticalChanceDealing, value);
		}
	}

	public int criticalChanceReceving
	{
		get
		{
			return GetStat(CombatEntity.EffectType.criticalChanceReceving);
		}
		set
		{
			SetStat(CombatEntity.EffectType.criticalChanceReceving, value);
		}
	}

	public int criticalDamageModifierDealing
	{
		get
		{
			return GetStat(CombatEntity.EffectType.criticalDamageModifierDealing);
		}
		set
		{
			SetStat(CombatEntity.EffectType.criticalDamageModifierDealing, value);
		}
	}

	public int criticalDamageModifierReceving
	{
		get
		{
			return GetStat(CombatEntity.EffectType.criticalDamageModifierReceving);
		}
		set
		{
			SetStat(CombatEntity.EffectType.criticalDamageModifierReceving, value);
		}
	}

	public int priority
	{
		get
		{
			return GetStat(CombatEntity.EffectType.priority);
		}
		set
		{
			SetStat(CombatEntity.EffectType.priority, value);
		}
	}

	public int dodge
	{
		get
		{
			return GetStat(CombatEntity.EffectType.dodge);
		}
		set
		{
			SetStat(CombatEntity.EffectType.dodge, value);
		}
	}

	public int maxMicroNutriment
	{
		get
		{
			return GetTagInt("max_micro_nutriment");
		}
		set
		{
			SetTagInt("max_micro_nutriment", value);
		}
	}

	public int maxCalorie
	{
		get
		{
			return GetTagInt("max_calorie");
		}
		set
		{
			SetTagInt("max_calorie", value);
		}
	}

	public int maxThirst
	{
		get
		{
			return GetTagInt("max_thirst");
		}
		set
		{
			SetTagInt("max_thirst", value);
		}
	}

	public int maxMorale
	{
		get
		{
			return GetTagInt("max_morale");
		}
		set
		{
			SetTagInt("max_morale", value);
		}
	}

	public int currentMicroNutriment
	{
		get
		{
			return GetTagInt("current_micro_nutriment");
		}
		set
		{
			SetTagInt("current_micro_nutriment", value);
		}
	}

	public int currentCalorie
	{
		get
		{
			return GetTagInt("current_calorie");
		}
		set
		{
			SetTagInt("current_calorie", value);
		}
	}

	public int currentThirst
	{
		get
		{
			return GetTagInt("current_thirst");
		}
		set
		{
			SetTagInt("current_thirst", value);
		}
	}

	public int currentMorale
	{
		get
		{
			return GetTagInt("current_morale");
		}
		set
		{
			SetTagInt("current_morale", value);
		}
	}

	public GameGridScrollableInventory barterInventory { get; private set; }

	public GameGridScrollableInventory barterBuyInventory { get; private set; }

	public GameGridScrollableInventory barterSellInventory { get; private set; }

	public long barterStoredScore { get; set; }

	public GameCharacterItem(string identifier, string spriteAtlasPath, string spritePath, string name = "No Name")
		: base(identifier, "Items/characteratlas", "character_card", new PixelWindow(canMove: true))
	{
		this.name = name;
		base.identifierName = identifier;
		EnableTag("humanoid");
		base.contentWindow.SetTitle(name);
		base.examineWindow = new PixelWindow(canMove: true);
		base.examineWindow?.AddParent(this);
		characterBackground = TreeNodeRender.Instantiate().SetParent(base.inner);
		characterBackground.image.sprite = RenderHandler.LoadFromAtlas(spriteAtlasPath, spritePath);
		characterBackground.image.SetNativeSize();
		characterBackground.image.alphaHitTestMinimumThreshold = 0.5f;
		characterBackground.eventHandler = base.background.eventHandler;
		characterBackground.image.raycastTarget = true;
		characterBackground.image.enabled = true;
		characterBackgroundFront = TreeNodeRender.Instantiate().SetParent(characterBackground);
		characterBackgroundFront.image.SetNativeSize();
		characterBackgroundFront.image.alphaHitTestMinimumThreshold = 0.5f;
		characterBackgroundFront.eventHandler = base.background.eventHandler;
		characterBackgroundFront.image.raycastTarget = true;
		characterBackgroundFront.image.enabled = true;
		characterFakeBackground = TreeNodeRender.Instantiate().SetParent(base.fakeInner);
		characterFakeBackground.image.sprite = characterBackground.image.sprite;
		characterFakeBackground.image.SetNativeSize();
		characterFakeBackground.image.enabled = true;
		characterFakeBackgroundFront = TreeNodeRender.Instantiate().SetParent(characterFakeBackground);
		characterFakeBackgroundFront.image.enabled = true;
		healthBarOutline = TreeNodeRender.Instantiate().SetParent(base.inner);
		healthBarBackground = TreeNodeRender.Instantiate().SetParent(base.inner);
		healthBar = TreeNodeRender.Instantiate().SetParent(base.inner);
		healthIcon = TreeNodeRender.Instantiate().SetParent(base.inner);
		moraleBarOutline = TreeNodeRender.Instantiate().SetParent(base.inner);
		moraleBarBackground = TreeNodeRender.Instantiate().SetParent(base.inner);
		moraleBar = TreeNodeRender.Instantiate().SetParent(base.inner);
		moraleIcon = TreeNodeRender.Instantiate().SetParent(base.inner);
		healthBarOutline.image.sprite = RenderHandler.current.healthBarOutline;
		healthBarOutline.image.SetNativeSize();
		healthBarBackground.image.sprite = RenderHandler.current.healthBarBackground;
		healthBarBackground.image.SetNativeSize();
		healthBar.image.sprite = RenderHandler.current.healthBar;
		healthBar.image.SetNativeSize();
		healthIcon.image.sprite = RenderHandler.current.healthIcon;
		healthIcon.image.SetNativeSize();
		moraleBarOutline.image.sprite = RenderHandler.current.moraleBarOutline;
		moraleBarOutline.image.SetNativeSize();
		moraleBarBackground.image.sprite = RenderHandler.current.moraleBarBackground;
		moraleBarBackground.image.SetNativeSize();
		moraleBar.image.sprite = RenderHandler.current.moraleBar;
		moraleBar.image.SetNativeSize();
		moraleIcon.image.sprite = RenderHandler.current.moraleIcon;
		moraleIcon.image.SetNativeSize();
		GridPixelElement gridPixelElement = new GridPixelElement(3, 1);
		base.examineWindow.Attach(gridPixelElement);
		GameGridInventory gameGridInventory = new GameGridInventory(6, 8);
		gameGridInventory.EnableTag("ability");
		gameGridInventory.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 1;
		gameGridInventory.mayInventoryAddItemFunc = (GameItem item, GameInventory inv) => item.IsTag("ability");
		gridPixelElement.Attach(gameGridInventory);
		GameGridScrollableInventory gameGridScrollableInventory = new GameGridScrollableInventory(8, 10, 200);
		gameGridScrollableInventory.EnableTag("ability");
		gameGridScrollableInventory.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 0;
		gameGridScrollableInventory.mayInventoryAddItemFunc = (GameItem item, GameInventory inv) => item.IsTag("ability");
		gridPixelElement.Attach(gameGridScrollableInventory);
		GridPixelElement gridPixelElement2 = new GridPixelElement(1, 2)
		{
			isCentered = true,
			justifyWidth = true
		};
		gridPixelElement.Attach(gridPixelElement2);
		GridPixelElement gridPixelElement3 = new GridPixelElement(3, 1);
		GridPixelElement gridPixelElement4 = new GridPixelElement(3, 1);
		GameSlotInventory element = OrganHelper.CreateLungSlot();
		GameSlotInventory element2 = OrganHelper.CreateHeartSlot();
		GameSlotInventory element3 = OrganHelper.CreateLungSlot(isLeft: false);
		gridPixelElement3.Attach(element);
		gridPixelElement3.Attach(element2);
		gridPixelElement3.Attach(element3);
		GameSlotInventory element4 = OrganHelper.CreateKidneySlot();
		GameSlotInventory element5 = OrganHelper.CreateLiverSlot();
		GameSlotInventory element6 = OrganHelper.CreateKidneySlot(isLeft: false);
		gridPixelElement4.Attach(element4);
		gridPixelElement4.Attach(element5);
		gridPixelElement4.Attach(element6);
		gridPixelElement2.Attach(gridPixelElement3);
		gridPixelElement2.Attach(gridPixelElement4);
		GridPixelElement gridPixelElement5 = new GridPixelElement(2, 1)
		{
			isCentered = true,
			justifyWidth = false
		};
		base.contentWindow.Attach(gridPixelElement5);
		GridPixelElement gridPixelElement6 = new GridPixelElement(1, 4);
		gridPixelElement5.Attach(gridPixelElement6, 0, 0);
		GameSlotInventory gameSlotInventory = (primaryWeaponSlot = new GameSlotInventory("Items/itematlas", "outline_weapon"));
		primaryWeaponSlot.SetTooltipName("[Primary Weapon Slot]");
		gameSlotInventory.mayInventoryAddItemFunc = (GameItem item, GameInventory inv) => item.ability != null;
		gameSlotInventory.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 3;
		gridPixelElement6.Attach(gameSlotInventory, 0, 0);
		gameSlotInventory = (secondaryWeaponSlot = new GameSlotInventory("Items/itematlas", "outline_weapon"));
		secondaryWeaponSlot.SetTooltipName("[Secondary Weapon Slot]");
		gameSlotInventory.mayInventoryAddItemFunc = (GameItem item, GameInventory inv) => item.ability != null;
		gameSlotInventory.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 2;
		gridPixelElement6.Attach(gameSlotInventory, 0, 1);
		gameSlotInventory = (sideSlot1 = new GameSlotInventory(2, 2)
		{
			mayInventoryAddItemFunc = (GameItem item, GameInventory inv) => item.shape.width <= 4 && item.shape.height <= 4 && !item.IsTag("backpack"),
			slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 1
		});
		sideSlot1.SetTooltipName("[Pocket Slot]");
		gridPixelElement6.Attach(gameSlotInventory, 0, 2);
		gameSlotInventory = (sideSlot2 = new GameSlotInventory(2, 2)
		{
			mayInventoryAddItemFunc = (GameItem item, GameInventory inv) => item.shape.width <= 4 && item.shape.height <= 4 && !item.IsTag("backpack"),
			slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 0
		});
		sideSlot2.SetTooltipName("[Pocket Slot]");
		gridPixelElement6.Attach(gameSlotInventory, 0, 3);
		gameSlotInventory = (backpackInventorySlot = new GameSlotInventory("Items/itematlas", "outline_backpack", stealUI: true));
		backpackInventorySlot.SetTooltipName("[Backpack Slot]");
		gameSlotInventory.slotSearchPassthroughForItemFunc = (GameItem _, GameInventory _, SlotMarker _, GameItem _) => true;
		gameSlotInventory.mayInventoryAddItemFunc = (GameItem item, GameInventory inv) => item.IsTag("equippable") && item.IsTag("backpack");
		gridPixelElement5.Attach(gameSlotInventory, 1);
		base.background.eventHandler.mouseClickFunc.Add(delegate
		{
			if (!isLocked && !isCard && GameMaster.current.InRaid())
			{
				GameMaster.current.raidScene.StealCharacterInventory(this);
				return true;
			}
			return false;
		});
		isCard = true;
		characterDone = true;
		ShowBars();
		currentSelectedAbility = null;
		abilityEffects = new List<CombatAbilityEffect>();
		abilityEffectsToBeApplied = new List<CombatAbilityEffect>();
		combatAI = new CombatAI();
		foreach (CombatEntity.EffectType value in Enum.GetValues(typeof(CombatEntity.EffectType)))
		{
			SetStat(value, 0);
		}
		maxAction = 1;
		action = 1;
		health = 100;
		maxHealth = 100;
		accuracyPercentage = 70;
		criticalChanceDealing = 10;
		reaction = 10;
		damagePercentage = 100;
		criticalChanceReceving = 0;
		criticalDamageModifierDealing = 100;
		criticalDamageModifierReceving = 0;
		deathchance = 100;
		priority = 5;
		dodge = 1;
		isDead = false;
		this.name = "default name";
		maxMicroNutriment = 1000;
		currentMicroNutriment = 1000;
		maxCalorie = 1000;
		currentCalorie = 600;
		maxThirst = 1000;
		currentThirst = 1000;
		maxMorale = 100;
		currentMorale = 100;
		Survival.CheckSurvivalEffect(this);
		Validate();
	}

	public GameCharacterItem SetCombatStateSprites(string characterSheetName, int index)
	{
		int num = stateSize * 2 * index;
		for (int i = 0; i < stateSize; i++)
		{
			stateBack[i] = RenderHandler.LoadFromAtlas("Characters/" + characterSheetName, characterSheetName + "_" + (num + i));
			stateFront[i] = RenderHandler.LoadFromAtlas("Characters/" + characterSheetName, characterSheetName + "_" + (num + stateSize + i));
		}
		characterBackground.image.sprite = stateBack[0];
		characterBackground.image.SetNativeSize();
		characterBackgroundFront.image.sprite = stateFront[0];
		characterBackgroundFront.image.SetNativeSize();
		characterFakeBackground.image.sprite = stateBack[0];
		characterFakeBackground.image.SetNativeSize();
		characterFakeBackgroundFront.image.sprite = stateFront[0];
		characterFakeBackgroundFront.image.SetNativeSize();
		if (isCard)
		{
			Uncard();
			Card();
		}
		else
		{
			Card();
			Uncard();
		}
		Validate();
		return this;
	}

	public void ShowBars()
	{
		moraleIcon.image.enabled = true;
		healthIcon.image.enabled = true;
		healthBar.image.enabled = true;
		healthBarBackground.image.enabled = true;
		healthBarOutline.image.enabled = true;
		moraleBar.image.enabled = true;
		moraleBarBackground.image.enabled = true;
		moraleBarOutline.image.enabled = true;
	}

	public void HideBars()
	{
		moraleIcon.image.enabled = false;
		healthIcon.image.enabled = false;
		healthBar.image.enabled = false;
		healthBarBackground.image.enabled = false;
		healthBarOutline.image.enabled = false;
		moraleBar.image.enabled = false;
		moraleBarBackground.image.enabled = false;
		moraleBarOutline.image.enabled = false;
	}

	public GameItem SetCharacterSprite(string spriteAtlasPath, string spritePath)
	{
		characterBackground.image.sprite = RenderHandler.LoadFromAtlas(spriteAtlasPath, spritePath);
		characterBackground.image.SetNativeSize();
		characterFakeBackground.image.sprite = characterBackground.image.sprite;
		characterFakeBackground.image.SetNativeSize();
		return this;
	}

	public override void Validate()
	{
		base.Validate();
		if (characterDone)
		{
			moraleIcon.image.sprite = ((currentMorale < lowMorale) ? RenderHandler.current.moraleIconLow : RenderHandler.current.moraleIcon);
			healthIcon.image.sprite = ((health < lowHealth) ? RenderHandler.current.healthIconLow : RenderHandler.current.healthIcon);
			int num = 54;
			int num2 = 64;
			if (isCard)
			{
				num -= 40;
				num2 -= 40;
			}
			moraleIcon.rectTransform.anchoredPosition = new Vector2(num, -95f);
			healthIcon.rectTransform.anchoredPosition = new Vector2(num2, -96f);
			int num3 = maxMorale / 2;
			int num4 = currentMorale / 2;
			int num5 = maxHealth / 2;
			int num6 = health / 2;
			moraleBar.rectTransform.sizeDelta = new Vector2(moraleBar.rectTransform.sizeDelta.x, num4);
			moraleBarBackground.rectTransform.sizeDelta = new Vector2(moraleBarBackground.rectTransform.sizeDelta.x, num3);
			moraleBarOutline.rectTransform.sizeDelta = new Vector2(moraleBarOutline.rectTransform.sizeDelta.x, num3 + 2);
			moraleBar.rectTransform.anchoredPosition = new Vector2(num + 2, -96f + moraleBar.rectTransform.sizeDelta.y + 4f);
			moraleBarBackground.rectTransform.anchoredPosition = new Vector2(num + 2, -96f + moraleBarOutline.rectTransform.sizeDelta.y + 2f);
			moraleBarOutline.rectTransform.anchoredPosition = new Vector2(num + 1, -96f + moraleBarOutline.rectTransform.sizeDelta.y + 3f);
			healthBar.rectTransform.sizeDelta = new Vector2(healthBar.rectTransform.sizeDelta.x, num6);
			healthBarBackground.rectTransform.sizeDelta = new Vector2(healthBarBackground.rectTransform.sizeDelta.x, num5);
			healthBarOutline.rectTransform.sizeDelta = new Vector2(healthBarOutline.rectTransform.sizeDelta.x, num5 + 2);
			healthBar.rectTransform.anchoredPosition = new Vector2(num2 + 2, -96f + healthBar.rectTransform.sizeDelta.y + 4f);
			healthBarBackground.rectTransform.anchoredPosition = new Vector2(num2 + 2, -96f + healthBarOutline.rectTransform.sizeDelta.y + 2f);
			healthBarOutline.rectTransform.anchoredPosition = new Vector2(num2 + 1, -96f + healthBarOutline.rectTransform.sizeDelta.y + 3f);
			if (health <= 0)
			{
				characterBackground.image.color = new Vector4(0.7f, 0.7f, 0.7f, 0.5f);
				HideBars();
			}
			else
			{
				characterBackground.image.color = new Vector4(1f, 1f, 1f, 1f);
				ShowBars();
			}
		}
	}

	public void AddItem(GameItem item)
	{
		TryFindOneValidInventorySlot(item)?.TryAcceptOnce();
	}

	public void Uncard()
	{
		base.background.gameObject.SetActive(value: false);
		characterBackground.rectTransform.pivot = new Vector2(1f, 1f);
		characterBackground.rectTransform.localScale = new Vector2(-1f, 1f);
		characterBackground.rectTransform.anchoredPosition = new Vector2(-28f, 0f);
		base.contentWindow.Lock();
		isCard = false;
	}

	public void Card()
	{
		Unlock();
		base.background.gameObject.SetActive(value: true);
		base.rotator.rectTransform.anchoredPosition = Vector2.zero;
		base.inner.rectTransform.anchoredPosition = Vector2.zero;
		characterBackground.rectTransform.pivot = new Vector2(0f, 1f);
		characterBackground.rectTransform.localScale = new Vector2(0.8f, 0.8f);
		characterBackground.rectTransform.anchoredPosition = new Vector2(8f, -8f);
		characterFakeBackground.rectTransform.pivot = characterBackground.rectTransform.pivot;
		characterFakeBackground.rectTransform.localScale = characterBackground.rectTransform.localScale;
		characterFakeBackground.rectTransform.anchoredPosition = characterBackground.rectTransform.anchoredPosition;
		GameMaster.current.raidScene.RestoreCharacterInventory(this);
		base.contentWindow.Unlock();
		isCard = true;
	}

	public void Lock()
	{
		isLocked = true;
		GameItem childItem = backpackInventorySlot.childItem;
		if (childItem == null || !childItem.isCombatBackpack)
		{
			backpackInventorySlot.Hide();
		}
	}

	public void Unlock()
	{
		isLocked = false;
		backpackInventorySlot.Show();
	}

	public int GetTagInt(string tag)
	{
		return base.state.GetTag(tag).valueInt;
	}

	public int GetModifiedTagInt(string tag)
	{
		return base.modifiedState.GetTag(tag).valueInt;
	}

	public void SetTagInt(string tag, int value)
	{
		base.state.GetTag(tag).valueInt = value;
	}

	public int GetStat(CombatEntity.EffectType type)
	{
		return base.state.GetTag(type.ToString()).valueInt;
	}

	public int GetModifiedStat(CombatEntity.EffectType type)
	{
		return base.modifiedState.GetTag(type.ToString()).valueInt;
	}

	public void SetStat(CombatEntity.EffectType type, int value)
	{
		string tag = type.ToString();
		base.state.GetTag(tag).valueInt = value;
	}

	public void HealEntity(int healing)
	{
		if (isDead)
		{
			return;
		}
		if (healing + health > maxHealth)
		{
			int netHeal = maxHealth - health;
			ActionQueue.current.Add(new EmptyAction(CombatManager.GetActionQueueDelay()).SetBeginFunc(delegate
			{
				SplashTextManager.Instance.DisplayFloatText(this, netHeal.ToString(), SplashTextManager.TextMode.Heal);
				if (!GameMaster.current.turnManager.isProcessingTurn)
				{
					CombatLog.log(name + " was heal by " + netHeal + " health");
				}
			}));
			health = maxHealth;
			return;
		}
		ActionQueue.current.Add(new EmptyAction(CombatManager.GetActionQueueDelay()).SetBeginFunc(delegate
		{
			SplashTextManager.Instance.DisplayFloatText(this, healing.ToString(), SplashTextManager.TextMode.Heal);
			if (!GameMaster.current.turnManager.isProcessingTurn)
			{
				CombatLog.log(name + " was heal by " + healing + " health");
			}
		}));
		health += healing;
	}

	public bool DamageEntity(int damage, bool isArmorPercing = false)
	{
		int damageToDeal = -damage;
		CancelOverwatch();
		if (!isArmorPercing)
		{
			damageToDeal = HandleArmorItem(damage);
		}
		if (damageToDeal == 0)
		{
			CombatLog.log(name + "'s armor blocked all the damage.");
		}
		else if (health > damageToDeal)
		{
			health -= damageToDeal;
			ActionQueue.current.Add(new CharacterGameAction(this).SetSyncTime(1.0).ModulateColor(delegate(double currentTime)
			{
				float num2 = (Mathf.Cos((float)currentTime * 10f) + 1f) / 2f;
				return new Vector4(1f, num2, num2, 1f);
			}).ModulatePosition((double currentTime) => new Vector2(Mathf.Cos((float)currentTime * 40f) * 5f, 0f))
				.SetBeginFunc(delegate
				{
					CombatLog.log(name + " received " + damageToDeal + " damage.");
					SplashTextManager.Instance.DisplayFloatText(this, damageToDeal.ToString(), SplashTextManager.TextMode.Damage);
				}));
		}
		else if (!isDead)
		{
			ActionQueue.current.Add(new CharacterGameAction(this).SetSyncTime(1.0).ModulateColor(delegate(double currentTime)
			{
				float num = (Mathf.Cos((float)currentTime * 10f) + 1f) / 2f;
				return new Vector4(1f, num, num, 1f);
			}).ModulatePosition((double currentTime) => new Vector2(Mathf.Cos((float)currentTime * 40f) * 5f, 0f))
				.SetBeginFunc(delegate
				{
					CombatLog.log(name + " received " + damageToDeal + " damage.");
					SplashTextManager.Instance.DisplayFloatText(this, damageToDeal.ToString(), SplashTextManager.TextMode.Damage);
				}));
			health = 1;
			if (CheckDeathDoor())
			{
				KillCurrentEntity();
				return true;
			}
		}
		return false;
	}

	public void KillCurrentEntity()
	{
		CombatAbilityEffect effect = DirectoryMaster.CombatAbilityEffect("dead");
		AppliedCombatEffect(effect, null);
		isDeadLogic = true;
		action = 0;
		ActionQueue.current.Add(new EmptyAction(0.5).SetBeginFunc(delegate
		{
			health = 0;
			isDead = true;
			CombatLog.log(name + " Died");
			Validate();
		}));
		if (GameMaster.current.InCombat())
		{
			CombatManager.Instance.EntityDied(this);
		}
	}

	public bool CheckDeathDoor()
	{
		int rng = UnityEngine.Random.Range(0, 100);
		if (deathchance != 100)
		{
			ActionQueue.current.Add(new EmptyAction(0.5).SetBeginFunc(delegate
			{
				CombatLog.log(name + " rolled a " + rng + ", they need " + deathchance + " or more to survive");
				SplashTextManager.Instance.DisplayFloatText(this, "Death's Door", SplashTextManager.TextMode.Bleed);
			}));
		}
		if (rng < deathchance)
		{
			return true;
		}
		ActionQueue.current.Add(new EmptyAction(0.5).SetBeginFunc(delegate
		{
			CombatLog.log(name + " survived, this time...");
		}));
		return false;
	}

	public void resetCurrentAction()
	{
		action = maxAction;
	}

	public CombatAbility GetSelectedAbility()
	{
		return currentSelectedAbility;
	}

	public CombatAbility CastSelectedAbility()
	{
		return GetSelectedAbility();
	}

	public void AppliedStatToEffect(CombatAbilityEffect effect)
	{
		if (effect.affectedStat == CombatEntity.EffectType.damagePercentage)
		{
			effect.value *= damagePercentage + 100;
		}
	}

	public bool OnTurnStart()
	{
		ResetProficiency();
		currentSelectedAbility = null;
		UpdateCombatEffect();
		Survival.UpdateSurvival(this);
		ActionQueue.current.Next();
		if (health <= 0)
		{
			return true;
		}
		return false;
	}

	public void HandlePerkCrafty()
	{
		foreach (CombatAbilityEffect effect in base.effects)
		{
			if (PerkDirectory.craftPerks.Contains(effect.identifier))
			{
				if (effect.identifier.Equals("perk_crafty") && RNG.Roll(1))
				{
					GraphUtils.TryAcceptAll(this, DirectoryMaster.Item("soap"));
				}
				if (effect.identifier.Equals("perk_pyromaniac") && RNG.Roll(1))
				{
					GraphUtils.TryAcceptAll(this, DirectoryMaster.Item("molotov_cocktail"));
				}
			}
		}
	}

	public void ResetProficiency()
	{
		if (IsTag("MEDICAL_PROFICIENCY"))
		{
			SetTagInt("MEDICAL_PROFICIENCY_CURRENT", GetTagInt("MEDICAL_PROFICIENCY"));
		}
		else if (IsTag("TACTICAL_PROFICIENCY"))
		{
			SetTagInt("TACTICAL_PROFICIENCY_CURRENT", GetTagInt("TACTICAL_PROFICIENCY"));
		}
		else if (IsTag("TECHNICAL_PROFICIENCY"))
		{
			SetTagInt("TECHNICAL_PROFICIENCY_CURRENT", GetTagInt("TECHNICAL_PROFICIENCY"));
		}
		else if (IsTag("GHETTO_PROFICIENCY"))
		{
			SetTagInt("GHETTO_PROFICIENCY_CURRENT", GetTagInt("GHETTO_PROFICIENCY"));
		}
	}

	public void CleanUpProficiency()
	{
		ResetProficiency();
		DisableTag("action_free_mode");
	}

	public bool IsAnyAbilitySelected()
	{
		if (currentSelectedAbility != null)
		{
			return true;
		}
		return false;
	}

	public void UpdateCombatEffect()
	{
		for (int num = abilityEffects.Count - 1; num >= 0; num--)
		{
			abilityEffects[num].DecrementDuration();
			AppliedOverTimeEffect(abilityEffects[num]);
		}
	}

	public void ReverseBuffStat(CombatAbilityEffect effect)
	{
		if (effect.IsBuff())
		{
			CombatEntity.EffectType affectedStat = effect.affectedStat;
			if (affectedStat != CombatEntity.EffectType.none)
			{
				SetStat(affectedStat, GetStat(affectedStat) - effect.value);
			}
		}
	}

	public void AppliedOverTimeEffect(CombatAbilityEffect effect)
	{
		if (!isDeadLogic && effect.IsOvertime())
		{
			ResolveEffect(effect, null);
		}
	}

	public bool IsCritialHit(CombatAbilityEffect effect, CombatEntity caster)
	{
		int num = 0;
		if (caster != null)
		{
			num = caster.criticalChanceDealing;
		}
		int num2 = criticalChanceReceving;
		int num3 = num + num2;
		if (UnityEngine.Random.Range(0, 101) < num3)
		{
			ActionQueue.current.Add(new EmptyAction(0.5).SetBeginFunc(delegate
			{
				CombatLog.log(caster.name + " landed a crital hit on " + name + "!");
				SplashTextManager.Instance.DisplayFloatText(this, "Critial!", SplashTextManager.TextMode.Damage);
			}));
			return true;
		}
		return false;
	}

	public void AppliedCombatEffect(CombatAbilityEffect effect, CombatEntity caster)
	{
		if (isDeadLogic)
		{
			return;
		}
		effect.HandlePerkOnEffectApply();
		effect.OnApply(this, caster);
		if (effect.IsInstant())
		{
			ResolveEffect(effect, caster);
		}
		else if ((effect.identifier == "bleed_light" || effect.identifier == "bleed_heavy" || effect.identifier == "red_stim_bleed") && HaveEffect("coagulent"))
		{
			ActionQueue.current.Add(new EmptyAction(0.5).SetBeginFunc(delegate
			{
				CombatLog.log(name + " resisted bleeding!");
			}));
		}
		else if (effect.identifier == "toxin" && HaveEffect("antitoxin"))
		{
			ActionQueue.current.Add(new EmptyAction(0.5).SetBeginFunc(delegate
			{
				CombatLog.log(name + " resisted toxin!");
			}));
		}
		else if (effect.abilityEffectType == CombatAbilityEffect.EffectType.setOverwatch)
		{
			ActionQueue.current.Add(new EmptyAction(0.5).SetBeginFunc(delegate
			{
				CombatLog.log(name + " is set to overwatch " + effect.duration + " turns.");
				SplashTextManager.Instance.DisplayFloatText(caster, "On Overwatch");
			}));
			AddCombatEffect(effect);
		}
		else if (effect.abilityEffectType == CombatAbilityEffect.EffectType.setRetaliate)
		{
			ActionQueue.current.Add(new EmptyAction(0.5).SetBeginFunc(delegate
			{
				CombatLog.log(name + " is set to retaliate " + effect.duration + " turns.");
				SplashTextManager.Instance.DisplayFloatText(caster, "On Retaliation");
			}));
			AddCombatEffect(effect);
		}
		else if (effect.abilityEffectType == CombatAbilityEffect.EffectType.stun)
		{
			ActionQueue.current.Add(new EmptyAction(0.5).SetBeginFunc(delegate
			{
				CombatLog.log(name + " was stunned for " + effect.duration + " turns!");
				SplashTextManager.Instance.DisplayFloatText(this, "Stunned x" + effect.duration, SplashTextManager.TextMode.Stun);
			}));
			CancelOverwatch();
			CancelRetaliate();
			AddCombatEffect(effect);
		}
		else if (effect.abilityEffectType == CombatAbilityEffect.EffectType.berserk)
		{
			ActionQueue.current.Add(new EmptyAction(0.5).SetBeginFunc(delegate
			{
				CombatLog.log(name + " became berserk for " + effect.duration + " turns!");
			}));
			AddCombatEffect(effect);
		}
		else if (effect.IsBuff() || effect.IsOvertime())
		{
			ResolveEffect(effect, caster);
			if (effect.abilityEffectType == CombatAbilityEffect.EffectType.buff)
			{
				if (!effect.isPermanent && !effect.isInvisible)
				{
					ActionQueue.current.Add(new EmptyAction(0.5).SetBeginFunc(delegate
					{
						SplashTextManager.Instance.DisplayFloatText(this, "Buffed x" + effect.duration, SplashTextManager.TextMode.Buffed);
					}));
				}
			}
			else if (effect.abilityEffectType == CombatAbilityEffect.EffectType.debuff && !effect.isPermanent && !effect.isInvisible)
			{
				ActionQueue.current.Add(new EmptyAction(0.5).SetBeginFunc(delegate
				{
					SplashTextManager.Instance.DisplayFloatText(this, "Debuffed x" + effect.duration, SplashTextManager.TextMode.Debuffed);
				}));
			}
			AddCombatEffect(effect);
		}
		PostAppliedEffect();
	}

	public void PostAppliedEffect()
	{
		EffectHelper.UpdateBleedingEffect(this);
	}

	public void AddCombatEffect(CombatAbilityEffect effect)
	{
		if (abilityEffects.Any((CombatAbilityEffect x) => x.identifier == effect.identifier))
		{
			abilityEffects.First((CombatAbilityEffect item) => item.identifier == effect.identifier).duration += effect.duration;
		}
		else
		{
			abilityEffects.Add(effect);
		}
	}

	public void ResolveEffect(CombatAbilityEffect effect, CombatEntity caster)
	{
		if (isDeadLogic)
		{
			return;
		}
		CombatEntity.EffectType affectedStat = effect.affectedStat;
		if (affectedStat != CombatEntity.EffectType.none)
		{
			if (affectedStat == CombatEntity.EffectType.health && effect.value < 0)
			{
				bool isArmorPercing = effect.IsArmorPercing();
				int num = effect.value;
				if (IsCritialHit(effect, caster))
				{
					num *= 2;
				}
				DamageEntity(num, isArmorPercing);
			}
			else if (affectedStat == CombatEntity.EffectType.health && effect.value > 0)
			{
				HealEntity(effect.value);
			}
			else
			{
				SetStat(affectedStat, GetStat(affectedStat) + effect.value);
			}
		}
		if (effect.abilityEffectType == CombatAbilityEffect.EffectType.callback)
		{
			effect.OnUpdate(this, null);
		}
	}

	public void OnCombatStart()
	{
	}

	public void SelectAbility(CombatAbility combatAbility)
	{
		currentSelectedAbility = combatAbility;
	}

	public bool HasEffect(CombatAbilityEffect effect)
	{
		foreach (CombatAbilityEffect abilityEffect in abilityEffects)
		{
			if (effect.id == abilityEffect.id)
			{
				return true;
			}
		}
		return false;
	}

	public bool HasEffectType(CombatAbilityEffect.EffectType effectType)
	{
		foreach (CombatAbilityEffect abilityEffect in abilityEffects)
		{
			if (abilityEffect.abilityEffectType == effectType)
			{
				return true;
			}
		}
		return false;
	}

	public int getTeam()
	{
		if (isPlayerControlled)
		{
			return 0;
		}
		return 1;
	}

	public int getAdverseTeam()
	{
		if (isPlayerControlled)
		{
			return 1;
		}
		return 0;
	}

	public bool IsStunned()
	{
		if (HasEffectType(CombatAbilityEffect.EffectType.stun))
		{
			return true;
		}
		return false;
	}

	public bool IsEntityLostOfAgency()
	{
		if (IsStunned() || HasEffectType(CombatAbilityEffect.EffectType.berserk))
		{
			return true;
		}
		return false;
	}

	public void CancelOverwatch()
	{
		if (!HasEffectType(CombatAbilityEffect.EffectType.setOverwatch))
		{
			return;
		}
		for (int num = abilityEffects.Count - 1; num >= 0; num--)
		{
			if (abilityEffects[num].abilityEffectType == CombatAbilityEffect.EffectType.setOverwatch)
			{
				abilityEffects.RemoveAt(num);
				ActionQueue.current.Add(new EmptyAction(0.5).SetBeginFunc(delegate
				{
					CombatLog.log(name + "'s Overwatch is cancelled");
					SplashTextManager.Instance.DisplayFloatText(this, "Overwatch Cancelled", SplashTextManager.TextMode.Debuffed);
				}));
			}
		}
	}

	public void CancelRetaliate()
	{
		if (!HasEffectType(CombatAbilityEffect.EffectType.setRetaliate))
		{
			return;
		}
		for (int num = abilityEffects.Count - 1; num >= 0; num--)
		{
			if (abilityEffects[num].abilityEffectType == CombatAbilityEffect.EffectType.setRetaliate)
			{
				abilityEffects.RemoveAt(num);
				ActionQueue.current.Add(new EmptyAction(0.5).SetBeginFunc(delegate
				{
					CombatLog.log(name + "'s Retaliation is cancelled");
					SplashTextManager.Instance.DisplayFloatText(this, "Retaliation Cancelled", SplashTextManager.TextMode.Debuffed);
				}));
			}
		}
	}

	public int RemoveEffectByID(string identifier)
	{
		int num = 0;
		for (int num2 = abilityEffects.Count - 1; num2 >= 0; num2--)
		{
			if (abilityEffects[num2].identifier.Equals(identifier))
			{
				abilityEffects.RemoveAt(num2);
				num++;
			}
		}
		return num;
	}

	public bool HaveEffect(string identifier)
	{
		for (int num = abilityEffects.Count - 1; num >= 0; num--)
		{
			if (abilityEffects[num].identifier.Equals(identifier))
			{
				return true;
			}
		}
		return false;
	}

	public bool HaveEffects(string[] identifiers)
	{
		for (int num = abilityEffects.Count - 1; num >= 0; num--)
		{
			for (int i = 0; i < identifiers.Length; i++)
			{
				if (abilityEffects[num].identifier.Equals(identifiers[i]))
				{
					return true;
				}
			}
		}
		return false;
	}

	public bool IsBleeding()
	{
		if (HaveEffect("bleed_light") || HaveEffect("bleed_heavy"))
		{
			return true;
		}
		return false;
	}

	public CombatAbility GetEntityWeaponAbility()
	{
		if (!primaryWeaponSlot.childItems.Any())
		{
			return null;
		}
		return primaryWeaponSlot.childItems[0]?.ability;
	}

	public void EntityTurnDone()
	{
		foreach (CombatAbilityEffect abilityEffect in abilityEffects)
		{
			if (abilityEffect.duration == 0)
			{
				ReverseBuffStat(abilityEffect);
				abilityEffect.OnExpire(this, null);
			}
		}
		abilityEffects.RemoveAll((CombatAbilityEffect effect) => effect.duration <= 0);
	}

	public void ForceUpdateEffect()
	{
		foreach (CombatAbilityEffect abilityEffect in abilityEffects)
		{
			abilityEffect.DecrementDuration();
			if (abilityEffect.duration == 0)
			{
				ReverseBuffStat(abilityEffect);
				abilityEffect.OnExpire(this, null);
			}
		}
		abilityEffects.RemoveAll((CombatAbilityEffect effect) => effect.duration <= 0);
	}

	public void ForceUpdateThisEffect(CombatAbilityEffect effect)
	{
		effect.DecrementDuration();
		if (effect.duration == 0)
		{
			ReverseBuffStat(effect);
			effect.OnExpire(this, null);
			abilityEffects.Remove(effect);
		}
	}

	public int RemoveEffectByTag(CombatAbilityEffect.EffectTag tag)
	{
		int num = 0;
		for (int num2 = abilityEffects.Count - 1; num2 >= 0; num2--)
		{
			if (abilityEffects[num2].IsTag(tag))
			{
				ReverseBuffStat(abilityEffects[num2]);
				abilityEffects.RemoveAt(num2);
				num++;
			}
		}
		return num;
	}

	public int RemoveEffect(CombatAbilityEffect effect)
	{
		int num = 0;
		for (int num2 = abilityEffects.Count - 1; num2 >= 0; num2--)
		{
			if (abilityEffects[num2] == effect)
			{
				abilityEffects.RemoveAt(num2);
				num++;
			}
		}
		return num;
	}

	public bool HaveEffect(CombatAbilityEffect.EffectTag tag)
	{
		for (int num = abilityEffects.Count - 1; num >= 0; num--)
		{
			if (abilityEffects[num].IsTag(tag))
			{
				return true;
			}
		}
		return false;
	}

	public CombatAbilityEffect GetEffect(string identifier)
	{
		for (int num = abilityEffects.Count - 1; num >= 0; num--)
		{
			if (abilityEffects[num].identifier.Equals(identifier))
			{
				return abilityEffects[num];
			}
		}
		return null;
	}

	public CombatAbilityEffect GetEffectByType(CombatAbilityEffect.EffectType type)
	{
		for (int num = abilityEffects.Count - 1; num >= 0; num--)
		{
			if (abilityEffects[num].abilityEffectType.Equals(type))
			{
				return abilityEffects[num];
			}
		}
		return null;
	}

	public void AddLoot(List<GameItem> items)
	{
		foreach (GameItem item in items)
		{
			if (GraphUtils.TryAcceptAll(this, item) > 0)
			{
				Debug.Log(("Not all loot could be inserted in room", item.GetDisplayName(), name));
			}
		}
	}

	public List<GameItem> GetEquippedArmorItem()
	{
		List<GameItem> list = new List<GameItem>();
		if (sideSlot1.childItems.Any() && sideSlot1.childItems[0].IsTag("armor_item"))
		{
			list.Add(sideSlot1.childItems[0]);
		}
		if (sideSlot2.childItems.Any() && sideSlot2.childItems[0].IsTag("armor_item"))
		{
			list.Add(sideSlot2.childItems[0]);
		}
		if (backpackInventorySlot.childItems.Any() && backpackInventorySlot.childItems[0].IsTag("armor_item"))
		{
			list.Add(backpackInventorySlot.childItems[0]);
		}
		return list;
	}

	public int HandleArmorItem(int damage)
	{
		return ArmorHelper.DistributeDamageWithEfficiency(GetEquippedArmorItem(), damage, this);
	}

	public (int, int) GetTotalCurrentArmorValue()
	{
		List<GameItem> equippedArmorItem = GetEquippedArmorItem();
		int num = 0;
		int num2 = 0;
		foreach (GameItem item in equippedArmorItem)
		{
			if (item.GetTagReadonly("current_armor_value").GetInt() > 0)
			{
				num2 = item.GetTagReadonly("current_armor_value").GetInt() + num2;
			}
			if (item.GetTagReadonly("current_max_armor_value").GetInt() > 0)
			{
				num = item.GetTagReadonly("current_max_armor_value").GetInt() + num;
			}
		}
		return (num2, num);
	}

	public int GetEffectModifier(CombatEntity.EffectType type)
	{
		int num = 0;
		foreach (CombatAbilityEffect abilityEffect in abilityEffects)
		{
			if (abilityEffect.affectedStat == type)
			{
				num += abilityEffect.value;
			}
		}
		return num;
	}

	public GameCharacterItem SetDialogue(Dialogue dialogue)
	{
		if (dialogue == null)
		{
			return this;
		}
		base.onActivateSlotItemFunc = delegate
		{
			GameMaster.current.raidManager.ForceStartDialogue(dialogue);
		};
		base.activateDefault = true;
		base.mayActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => !dialogue.IsDisabled();
		base.canActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		return this;
	}

	public void BarterShow()
	{
		OverlayHandler.current.ShowBarter(this);
	}

	public bool IsMerchant()
	{
		return barterInventory != null;
	}

	private void BarterInit()
	{
		if (barterInventory != null)
		{
			return;
		}
		barterInventory = new GameGridScrollableInventory(12, 20);
		barterBuyInventory = new GameGridScrollableInventory(8, 12, 400, 0.4f);
		barterSellInventory = new GameGridScrollableInventory(8, 12, 400, 0.4f);
		barterInventory.alwaysStack = true;
		barterInventory.mayInventoryAddItemFunc = (GameItem item, GameInventory _) => item.IsTag(TAG_NOT_PURCHASED) ? true : false;
		barterBuyInventory.mayInventoryAddItemFunc = barterInventory.mayInventoryAddItemFunc;
		barterInventory.onSlotAddItemEndFunc = delegate(GameItem item, GameInventory _, SlotMarker _)
		{
			item.EnableTag(TAG_NOT_PURCHASED);
			item.contentWindow?.Lock();
			if (!item.IsTag(TAG_BARTER_INIT))
			{
				item.EnableTag(TAG_BARTER_INIT);
				Func<GameItem, GameInventory, bool> itemPreviousAddFunc = item.mayInventoryAddItemFunc;
				item.mayInventoryAddItemFunc = (GameItem i, GameInventory inv) => i.IsTag(TAG_NOT_PURCHASED) ? (inv.IsTag(TAG_NOT_PURCHASED) ? true : false) : (itemPreviousAddFunc?.InvokeAllReduce(i, inv, InvokeFuncExtensions.BoolReduceOR) ?? true);
				item.mayStackItemFunc = (GameItem otherItem, GameItem _) => item?.identifier == otherItem?.identifier;
			}
		};
		barterBuyInventory.onSlotAddItemEndFunc = barterInventory.onSlotAddItemEndFunc;
		barterSellInventory.onSlotAddItemEndFunc = delegate(GameItem item, GameInventory _, SlotMarker _)
		{
			item.DisableTag(TAG_NOT_PURCHASED);
			item.contentWindow?.Unlock();
		};
		barterInventory.EnableTag(TAG_NOT_PURCHASED);
		barterBuyInventory.EnableTag(TAG_NOT_PURCHASED);
	}

	public GameCharacterItem BarterEnable(List<GameItem> items = null)
	{
		BarterInit();
		barterInventory.ExpelAll();
		barterBuyInventory.ExpelAll();
		if (items != null)
		{
			barterInventory.AddFastUnchecked(items);
		}
		barterInventory.Validate();
		return this;
	}

	public GameCharacterItem BarterAdd(List<GameItem> items)
	{
		BarterInit();
		barterInventory.AddFastUnchecked(items);
		barterInventory.Validate();
		return this;
	}

	public GameCharacterItem BarterClear()
	{
		BarterInit();
		barterInventory.ExpelAll();
		barterBuyInventory.ExpelAll();
		barterInventory.Validate();
		return this;
	}

	public int BarterGetStock(string identifier)
	{
		if (barterInventory == null)
		{
			return 0;
		}
		int num = (from item in GraphUtils.FindAllChildrenType<GameItem>(barterInventory)
			where item.identifier == identifier
			select item.unitCount).DefaultIfEmpty(0).Sum();
		int num2 = (from item in GraphUtils.FindAllChildrenType<GameItem>(barterBuyInventory)
			where item.identifier == identifier
			select item.unitCount).DefaultIfEmpty(0).Sum();
		return num + num2;
	}

	public void SetSpecies(string species)
	{
		EnableTag(species);
		EnableTag("SPECIES_TAG");
		ModifyTag("SPECIES_TAG", delegate(TagState state)
		{
			state.SetString(species);
		});
	}
}
