using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class GameItem : GameItemFunc, GraphNodeStorage, DirectoryEntry, CombatAbility
{
	public static string ITEM_TYPE_STRING = "ITEM_TYPE_STRING";

	public List<ItemFeature> itemFeatures;

	public List<string> itemTypes;

	private static readonly HashSet<string> warnedMisalignedSprites = new HashSet<string>();

	public const string CONTENT_TABS_STRING = "CONTENT_TABS_STRING";

	public List<GraphNodeStorage> parents
	{
		get
		{
			List<GraphNodeStorage> list = new List<GraphNodeStorage>();
			if (parentInventory != null)
			{
				list.Add(parentInventory);
			}
			return list;
		}
	}

	public virtual List<GraphNodeStorage> children
	{
		get
		{
			List<GraphNodeStorage> list = new List<GraphNodeStorage>();
			if (child != null)
			{
				list.Add(child);
			}
			if (examineWindow != null)
			{
				list.Add(examineWindow);
			}
			return list;
		}
	}

	public GameInventory parentInventory { get; set; }

	public GraphNodeWindow child => contentWindow;

	public GridShape shape { get; set; }

	public GridShape modifiedShape { get; set; }

	public int modifiedXOrigin { get; set; }

	public int modifiedYOrigin { get; set; }

	public TagSystem state { get; set; }

	public TagSystem modifiedState { get; set; }

	public string identifier { get; set; }

	public string identifierName { get; set; }

	public int uniqueId { get; set; }

	public virtual string name { get; set; }

	public string shortDescription { get; set; }

	public string longDescription { get; set; }

	public string flavorText { get; set; }

	public int unitCount { get; set; }

	public long unitValue { get; set; }

	public long lateUnitValue { get; set; }

	public long unitBaseValue { get; set; }

	public long backupUnitValue { get; set; }

	public CombatAbility ability
	{
		get
		{
			return this;
		}
		set
		{
			name = value.name;
			shortDescription = value.shortDescription;
			bonusAccuracy = value.bonusAccuracy;
			triggerOverwatch = value.triggerOverwatch;
			effects = value.effects.ToList();
		}
	}

	public RecipeManager recipeManager { get; set; }

	public string customText { get; set; }

	public PixelWindow contentWindow { get; protected set; }

	public PixelWindow examineWindow { get; protected set; }

	public bool forceDisableActivate { get; set; }

	public bool forceDisableUse { get; set; }

	public bool activateDefault { get; set; }

	public bool isCombatBackpack { get; set; }

	public string spritePath { get; set; }

	public string spriteAtlasPath { get; set; }

	protected bool spriteChanged { get; set; }

	public bool useSpriteFromMod { get; set; }

	public bool isDebugMenu { get; set; }

	public string soundDragStart { get; set; }

	public string soundDragEnd { get; set; }

	public string soundContainerOpen { get; set; }

	public string soundContainerClose { get; set; }

	public string soundSelect { get; set; }

	public string soundExamineOpen { get; set; }

	public string soundToggle { get; set; }

	public string soundActivate { get; set; }

	public string soundUse { get; set; }

	public int bonusAccuracy { get; set; }

	public List<CombatAbilityEffect> effects { get; set; }

	public bool triggerOverwatch { get; set; }

	public bool canUseOutsideCombat { get; set; }

	[Obsolete("GameItem.Create() is deprecated, please use DirectoryMaster.CreateItem() instead.")]
	public static GameItem Create(string spriteAtlasPath, string spritePath, PixelWindow window = null)
	{
		return new GameItemElement(null, spriteAtlasPath, spritePath, window);
	}

	public GameItem(string identifier, PixelWindow window = null)
	{
		soundDragStart = "move_item_default";
		soundDragEnd = null;
		soundContainerOpen = "light_click";
		soundSelect = null;
		soundExamineOpen = null;
		soundToggle = null;
		soundActivate = null;
		soundUse = null;
		state = new TagSystem();
		modifiedState = new TagSystem();
		this.identifier = identifier;
		name = "";
		shortDescription = "";
		longDescription = "";
		flavorText = "";
		spriteAtlasPath = "";
		spritePath = "";
		contentWindow = window;
		contentWindow?.AddParent(this);
		shape = new GridShapeBuilder(1, 1);
		modifiedShape = shape.Clone();
		unitCount = 1;
		unitValue = 0L;
		effects = new List<CombatAbilityEffect>();
		bonusAccuracy = 0;
		triggerOverwatch = false;
		canUseOutsideCombat = true;
		itemFeatures = new List<ItemFeature>();
		itemTypes = new List<string>();
		base.onSlotAddItemFunc = (Action<GameItem, GameInventory, SlotMarker>)Delegate.Combine(base.onSlotAddItemFunc, (Action<GameItem, GameInventory, SlotMarker>)delegate
		{
			if (IsRootedInBackInv())
			{
				if (PlayerStore.Instance.currentClientInstance != null && parentInventory?.GetParentItem() == null)
				{
					StoreClient clientBlueprint = PlayerStore.Instance.currentClientInstance.GetClientBlueprint();
					foreach (string item in clientBlueprint.clientItemFeatureBuying)
					{
						AddItemFeature(ItemFeatureList.SpecialFeature[item]());
					}
					if (clientBlueprint.chargesContrabandMarkup)
					{
						ContrabandHelper.ApplyContrabandFeature(this);
					}
				}
				else if (IsInsideContainerPlacedForSelling())
				{
					ContrabandHelper.ApplyContrabandFeature(this);
					ApplyContrabandFeatureToChildren();
				}
				return;
			}
			RemoveTemporaryClientFeature();
			RemoveContrabandFeature();
			foreach (GameItem item2 in GraphUtils.FindAllChildrenType<GameItem>(this))
			{
				item2.RemoveTemporaryClientFeature();
				item2.RemoveContrabandFeature();
			}
		});
	}

	public void AddClientItemBuyingFeature()
	{
		if (PlayerStore.Instance.currentClientInstance == null || PlayerStore.Instance.currentClientInstance.GetClientBlueprint().clientItemFeatureBuying == null)
		{
			return;
		}
		foreach (string item in PlayerStore.Instance.currentClientInstance.GetClientBlueprint().clientItemFeatureBuying)
		{
			AddItemFeature(ItemFeatureList.SpecialFeature[item]());
		}
	}

	[Obsolete("CloneLinked() is deprecated, please use DirectoryMaster.CreateItem() instead.")]
	public GameItem CloneLinked()
	{
		if (contentWindow != null)
		{
			throw new NotImplementedException("Deep item cloning is not yet implemented, you can only copy the item by itself.");
		}
		if (examineWindow != null)
		{
			throw new NotImplementedException("Deep item cloning is not yet implemented, you can only copy the item by itself.");
		}
		GameItem gameItem = ((identifier != null) ? DirectoryMaster.Item(identifier) : ItemDirectory.CreateEmptyItem());
		gameItem.spritePath = spritePath;
		gameItem.spriteAtlasPath = spriteAtlasPath;
		gameItem.activateDefault = activateDefault;
		gameItem.spriteChanged = true;
		gameItem.unitCount = unitCount;
		gameItem.ability = ability;
		gameItem.state = state.Clone();
		gameItem.modifiedState = modifiedState.Clone();
		gameItem.shape = shape.Clone();
		gameItem.modifiedShape = modifiedShape.Clone();
		gameItem.shortDescription = shortDescription;
		gameItem.longDescription = longDescription;
		gameItem.unitValue = unitValue;
		gameItem.unitBaseValue = unitBaseValue;
		gameItem.lateUnitValue = lateUnitValue;
		gameItem.backupUnitValue = backupUnitValue;
		gameItem.name = name;
		gameItem.customText = customText;
		gameItem.mayInventoryAddItemFunc = base.mayInventoryAddItemFunc;
		gameItem.mayInventoryRemoveItemFunc = base.mayInventoryRemoveItemFunc;
		gameItem.mayStackItemFunc = base.mayStackItemFunc;
		gameItem.maxSlotAddItemFunc = base.maxSlotAddItemFunc;
		gameItem.maxSlotRemoveItemFunc = base.maxSlotRemoveItemFunc;
		gameItem.maxSlotAmountItemFunc = base.maxSlotAmountItemFunc;
		gameItem.minSlotAmountItemFunc = base.minSlotAmountItemFunc;
		gameItem.onSlotAddItemFunc = base.onSlotAddItemFunc;
		gameItem.onSlotRemoveItemFunc = base.onSlotRemoveItemFunc;
		gameItem.onSlotChangeAmountItemFunc = base.onSlotChangeAmountItemFunc;
		gameItem.onChangeAmountItemFunc = base.onChangeAmountItemFunc;
		gameItem.slotPriorityFunc = base.slotPriorityFunc;
		gameItem.slotSearchPassthroughForItemFunc = base.slotSearchPassthroughForItemFunc;
		gameItem.parentItemShapeModifierForItemFunc = base.parentItemShapeModifierForItemFunc;
		gameItem.parentItemSpriteModifierForItemFunc = base.parentItemSpriteModifierForItemFunc;
		gameItem.parentItemStateModifierForItemFunc = base.parentItemStateModifierForItemFunc;
		gameItem.isItemStateValidFunc = base.isItemStateValidFunc;
		gameItem.tooltipRichTextFunc = base.tooltipRichTextFunc;
		gameItem.maySelectSlotItemFunc = base.maySelectSlotItemFunc;
		gameItem.canSelectSlotItemFunc = base.canSelectSlotItemFunc;
		gameItem.mayItemTargetItemFunc = base.mayItemTargetItemFunc;
		gameItem.canItemTargetItemFunc = base.canItemTargetItemFunc;
		gameItem.onItemTargetItemFunc = base.onItemTargetItemFunc;
		gameItem.maySlotItemTargetItemFunc = base.maySlotItemTargetItemFunc;
		gameItem.canSlotItemTargetItemFunc = base.canSlotItemTargetItemFunc;
		gameItem.onSlotItemTargetItemFunc = base.onSlotItemTargetItemFunc;
		gameItem.mayItemTargetSlotItemFunc = base.mayItemTargetSlotItemFunc;
		gameItem.canItemTargetSlotItemFunc = base.canItemTargetSlotItemFunc;
		gameItem.onItemTargetSlotItemFunc = base.onItemTargetSlotItemFunc;
		gameItem.mayActivateSlotItemFunc = base.mayActivateSlotItemFunc;
		gameItem.canActivateSlotItemFunc = base.canActivateSlotItemFunc;
		gameItem.onActivateSlotItemFunc = base.onActivateSlotItemFunc;
		gameItem.mayToggleSlotItemFunc = base.mayToggleSlotItemFunc;
		gameItem.canToggleSlotItemFunc = base.canToggleSlotItemFunc;
		gameItem.onToggleSlotItemFunc = base.onToggleSlotItemFunc;
		gameItem.onConfigureSlotItemFunc = base.onConfigureSlotItemFunc;
		gameItem.onUnloadChildItemFunc = base.onUnloadChildItemFunc;
		gameItem.soundActivate = soundActivate;
		gameItem.soundContainerOpen = soundContainerOpen;
		gameItem.soundDragEnd = soundDragEnd;
		gameItem.soundDragStart = soundDragStart;
		gameItem.soundExamineOpen = soundExamineOpen;
		gameItem.soundSelect = soundSelect;
		gameItem.soundToggle = soundToggle;
		gameItem.soundUse = soundUse;
		return gameItem;
	}

	public GameItem SetSprite(string spriteAtlasPath, string spritePath)
	{
		this.spriteAtlasPath = spriteAtlasPath;
		this.spritePath = spritePath;
		spriteChanged = true;
		return this;
	}

	public GameItem SetSpriteQuick(string spriteAtlasPath, string spritePath)
	{
		this.spriteAtlasPath = spriteAtlasPath;
		this.spritePath = spritePath;
		((GameItemElement)this).ValidatePartialSprite();
		return this;
	}

	public GameItem SetShape(GridShape itemShape)
	{
		shape = itemShape.Clone();
		ValidateShapeState();
		return this;
	}

	public GameItem SetSpriteAndShape(string spriteAtlasPath, string spritePath)
	{
		this.spriteAtlasPath = spriteAtlasPath;
		this.spritePath = spritePath;
		spriteChanged = true;
		SetShape(BuildShapeFromSprite(spriteAtlasPath, spritePath));
		return this;
	}

	public static GridShapeBuilder BuildShapeFromSprite(string spriteAtlasPath, string spritePath)
	{
		Sprite sprite = RenderHandler.LoadFromAtlas(spriteAtlasPath, spritePath);
		Rect rect = sprite.rect;
		int gridSize = UISettings.current.gridSize;
		int mipLevel = (int)Math.Sqrt(gridSize);
		int num = Math.Max(1, (int)rect.width / gridSize);
		int num2 = Math.Max(1, (int)rect.height / gridSize);
		WarnIfSliceNotGridAligned(spriteAtlasPath, spritePath, rect, gridSize);
		byte[] array = new byte[num * num2];
		for (int i = 0; i < num2; i++)
		{
			for (int j = 0; j < num; j++)
			{
				int num3 = i * num + j;
				int x = (int)rect.xMin / gridSize + j;
				int y = (int)rect.yMin / gridSize + (num2 - 1 - i);
				if (sprite.texture.GetPixel(x, y, mipLevel).a > 0f)
				{
					array[num3] = 1;
				}
			}
		}
		return new GridShapeBuilder().SetData(array, num);
	}

	private static void WarnIfSliceNotGridAligned(string spriteAtlasPath, string spritePath, Rect r, int gridSize)
	{
		if ((int)r.xMin % gridSize != 0 || (int)r.yMin % gridSize != 0 || (int)r.width % gridSize != 0 || (int)r.height % gridSize != 0 || !(r.width >= (float)gridSize) || !(r.height >= (float)gridSize))
		{
			string item = spriteAtlasPath + "/" + spritePath;
			if (warnedMisalignedSprites.Add(item))
			{
				Debug.LogWarning("<color=orange>[GameItem] SPRITE NOT ON GRID: '" + spritePath + "' in '" + spriteAtlasPath + "' " + $"(rect x={r.xMin} y={r.yMin} w={r.width} h={r.height}).</color> " + $"Fix the slice in the Sprite Editor: position and size must be multiples of {gridSize}.");
			}
		}
	}

	public GameItem SetSpriteAndShapeFromMod(string modId, string spriteKey)
	{
		LoadedMod loadedMod = ModLoader.Mods.Find((LoadedMod m) => m.Manifest.ID.Equals(modId));
		if (loadedMod == null || !loadedMod.Sprites.TryGetValue(spriteKey, out var value))
		{
			Debug.LogWarning("[GameItem] Mod sprite '" + spriteKey + "' from mod '" + modId + "' not found.");
			return this;
		}
		spriteAtlasPath = modId;
		spritePath = spriteKey;
		useSpriteFromMod = true;
		spriteChanged = true;
		Rect rect = value.rect;
		int gridSize = UISettings.current.gridSize;
		int num = (int)rect.width / gridSize;
		int num2 = (int)rect.height / gridSize;
		byte[] array = new byte[num * num2];
		for (int i = 0; i < num2; i++)
		{
			for (int j = 0; j < num; j++)
			{
				int x = (int)rect.xMin + j * gridSize + gridSize / 2;
				int y = (int)rect.yMin + (num2 - 1 - i) * gridSize + gridSize / 2;
				if (value.texture.GetPixel(x, y).a > 0f)
				{
					array[i * num + j] = 1;
				}
			}
		}
		SetShape(new GridShapeBuilder().SetData(array, num));
		return this;
	}

	public long GetValue(List<GameCharacterItem> party = null, GameCharacterItem vendor = null, bool singleValue = false)
	{
		return ValueModifier(this, party, vendor) * (singleValue ? 1 : unitCount);
	}

	public GameItem SetValue(long num)
	{
		unitValue = num;
		return this;
	}

	private static long ValueModifier(GameItem item, List<GameCharacterItem> party, GameCharacterItem vendor)
	{
		int num = 0;
		num += PowerHelper.GetPowerSourceItemValue(item);
		num += AmenitiesManager.GetAmenitiesItemValue(item);
		num += ArmorHelper.GetArmorItemValue(item);
		num += LiquidContainerHelper.GetLiquidContainerValue(item);
		num += OrganHelper.GetOrganItemValue(item);
		double num2 = 1.0;
		if (item.IsTag(GameCharacterItem.TAG_NOT_PURCHASED))
		{
			num2 = 1.1;
		}
		else if (!item.IsTag(GameCharacterItem.TAG_NOT_PURCHASED) && vendor != null)
		{
			num2 = 0.9;
		}
		double num3 = 1.0;
		double num4 = 1.0;
		if (vendor != null)
		{
			num3 = BarterHelper.GetVendorDiscount(vendor, item);
			if (!item.IsTag(GameCharacterItem.TAG_NOT_PURCHASED))
			{
				num4 = BarterHelper.DoesTraderAcceptThisItemAsPayment(item, vendor);
			}
		}
		double num5 = (double)(item.unitValue + num) * num2 * num3 * num4;
		if (!item.IsTag(GameCharacterItem.TAG_NOT_PURCHASED) && num4 < 0.5)
		{
			num5 = 1.0;
		}
		return (long)num5;
	}

	public GameItem SetUnitCount(int num)
	{
		unitCount = num;
		Validate();
		return this;
	}

	public GameItem SetAmount(int num)
	{
		return SetUnitCount(num);
	}

	public GameItem SetStack(int num)
	{
		return SetUnitCount(num);
	}

	public GameItem SetContentWindow(PixelWindow window)
	{
		contentWindow?.RemoveParent(this);
		contentWindow = window;
		contentWindow?.AddParent(this);
		return this;
	}

	protected void DestroyContentWindow()
	{
		PixelWindow pixelWindow = contentWindow;
		if (pixelWindow == null)
		{
			return;
		}
		foreach (GameItem item in GraphUtils.FindChildrenType<GameItem>(pixelWindow))
		{
			item.Destroy();
		}
		pixelWindow.RemoveParent(this);
		contentWindow = null;
		pixelWindow.Destroy();
	}

	public GameItem SetExamineWindow(PixelWindow window)
	{
		examineWindow?.RemoveParent(this);
		examineWindow = window;
		examineWindow?.AddParent(this);
		return this;
	}

	public GameItem Transform(Action<GameItem> transformFunc)
	{
		transformFunc?.Invoke(this);
		spriteChanged = true;
		return this;
	}

	public bool AddParent(GraphNodeStorage node)
	{
		if (parentInventory != null)
		{
			return false;
		}
		if (!(node is GameInventory))
		{
			return false;
		}
		parentInventory = (GameInventory)node;
		return true;
	}

	public bool RemoveParent(GraphNodeStorage node)
	{
		if (parentInventory != node)
		{
			return false;
		}
		parentInventory = null;
		return true;
	}

	public bool IsAdjacent(GameItem item)
	{
		return parentInventory?.IsAdjacent(parentInventory?.GetIncompleteItemInventorySlot(this), item?.parentInventory?.GetIncompleteItemInventorySlot(item)) ?? false;
	}

	public List<T> FindAdjacentType<T>()
	{
		List<T> list = new List<T>();
		if (parentInventory == null)
		{
			return list;
		}
		SlotMarker incompleteItemInventorySlot = parentInventory.GetIncompleteItemInventorySlot(this);
		foreach (GameItem childItem in parentInventory.childItems)
		{
			if (childItem is T item && parentInventory.IsAdjacent(incompleteItemInventorySlot, childItem.parentInventory?.GetIncompleteItemInventorySlot(childItem)))
			{
				list.Add(item);
			}
		}
		return list;
	}

	public List<(GameItem item, int touchingSides)> GetAdjacentGameItemsWithTouchingSides()
	{
		List<(GameItem, int)> list = new List<(GameItem, int)>();
		if (parentInventory == null)
		{
			return list;
		}
		GridShape itemGridShape = parentInventory.GetIncompleteItemInventorySlot(this).itemGridShape;
		if (itemGridShape == null)
		{
			return list;
		}
		foreach (GameItem childItem in parentInventory.childItems)
		{
			if (childItem == this || childItem.parentInventory == null)
			{
				continue;
			}
			GridShape itemGridShape2 = childItem.parentInventory.GetIncompleteItemInventorySlot(childItem).itemGridShape;
			if (itemGridShape2 != null)
			{
				int num = CountTouchingSides(itemGridShape, itemGridShape2);
				if (num > 0)
				{
					list.Add((childItem, num));
				}
			}
		}
		return list;
	}

	private int CountTouchingSides(GridShape shapeA, GridShape shapeB)
	{
		int num = 0;
		for (int i = shapeA.minY; i <= shapeA.maxY; i++)
		{
			for (int j = shapeA.minX; j <= shapeA.maxX; j++)
			{
				if (shapeA.Get(j, i) == 1)
				{
					if (shapeB.Get(j + 1, i) == 1)
					{
						num++;
					}
					if (shapeB.Get(j - 1, i) == 1)
					{
						num++;
					}
					if (shapeB.Get(j, i + 1) == 1)
					{
						num++;
					}
					if (shapeB.Get(j, i - 1) == 1)
					{
						num++;
					}
				}
			}
		}
		return num;
	}

	public bool MayHaveValidInventorySlot(GameItem item)
	{
		return child?.MayHaveValidInventorySlot(item) ?? false;
	}

	public SlotMarker TryFindOneValidInventorySlot(GameItem item)
	{
		foreach (GraphNodeStorage child in children)
		{
			SlotMarker slotMarker = child.TryFindOneValidInventorySlot(item);
			if (slotMarker != null)
			{
				return slotMarker;
			}
		}
		return null;
	}

	public abstract void Destroy();

	public abstract void Validate();

	public string GetDisplayName(bool preferCustomName = false)
	{
		if (preferCustomName && GeneralHelper.HasCustomName(this))
		{
			return GeneralHelper.GetCustomName(this);
		}
		if (name != null && name.Length > 0)
		{
			return name;
		}
		if (identifier != null && identifier.Length > 0)
		{
			return identifier;
		}
		if (spritePath != null && spritePath.Length > 0)
		{
			return spritePath;
		}
		return "UNKNOWN_ITEM";
	}

	public bool IsTag(string tagName)
	{
		return modifiedState.IsTag(tagName);
	}

	public bool IsOneOfTags(List<string> tags)
	{
		foreach (string tag in tags)
		{
			if (IsTag(tag))
			{
				return true;
			}
		}
		return false;
	}

	public TagState GetTagReadonly(string tagName)
	{
		return modifiedState.GetTag(tagName).Clone();
	}

	public GameItem ModifyTag(string tagName, Action<TagState> action, bool skipValidate = false)
	{
		if (action != null)
		{
			action(state.GetTag(tagName));
			if (!skipValidate)
			{
				ValidateShapeState();
			}
		}
		return this;
	}

	public bool EnableTag(string tagName, bool skipValidate = false)
	{
		if (!state.IsTag(tagName))
		{
			state.EnableTag(tagName);
			if (!skipValidate)
			{
				ValidateShapeState();
			}
			return true;
		}
		return false;
	}

	public bool DisableTag(string tagName, bool skipValidate = false)
	{
		if (state.IsTag(tagName))
		{
			state.DisableTag(tagName);
			if (!skipValidate)
			{
				ValidateShapeState();
			}
			return true;
		}
		return false;
	}

	public bool SetEnableDisableTag(string tagName, bool isEnabled)
	{
		if (isEnabled)
		{
			return EnableTag(tagName);
		}
		return DisableTag(tagName);
	}

	public bool IsTag<T>(T tagName) where T : Enum
	{
		return modifiedState.IsTag(tagName);
	}

	public TagState GetTagReadonly<T>(T tagName) where T : Enum
	{
		return modifiedState.GetTag(tagName).Clone();
	}

	public GameItem ModifyTag<T>(T tagName, Action<TagState> action) where T : Enum
	{
		if (action != null)
		{
			action(state.GetTag(tagName));
			ValidateShapeState();
		}
		return this;
	}

	public bool EnableTag<T>(T tagName) where T : Enum
	{
		if (!state.IsTag(tagName))
		{
			state.EnableTag(tagName);
			ValidateShapeState();
			return true;
		}
		return false;
	}

	public bool DisableTag<T>(T tagName) where T : Enum
	{
		if (state.IsTag(tagName))
		{
			state.DisableTag(tagName);
			ValidateShapeState();
			return true;
		}
		return false;
	}

	public GameItem SetName(string name)
	{
		this.name = name;
		return this;
	}

	public bool MayToggleSlotItem()
	{
		if (parentInventory == null)
		{
			return false;
		}
		bool? flag = null;
		bool? flag2 = null;
		SlotMarker incompleteItemInventorySlot = parentInventory.GetIncompleteItemInventorySlot(this);
		bool? obj = base.mayToggleSlotItemFunc?.InvokeAllReduce(this, parentInventory, incompleteItemInventorySlot, InvokeFuncExtensions.BoolReduceOR);
		bool? flag3 = parentInventory?.mayToggleSlotItemFunc?.InvokeAllReduce(this, parentInventory, incompleteItemInventorySlot, InvokeFuncExtensions.BoolReduceOR);
		flag = flag & obj & flag3;
		if ((flag2 | obj | flag3) == true && flag != false)
		{
			return true;
		}
		return false;
	}

	public bool CanToggleSlotItem()
	{
		if (parentInventory == null)
		{
			return false;
		}
		bool? flag = null;
		bool? flag2 = null;
		SlotMarker incompleteItemInventorySlot = parentInventory.GetIncompleteItemInventorySlot(this);
		bool? obj = base.canToggleSlotItemFunc?.InvokeAllReduce(this, parentInventory, incompleteItemInventorySlot, InvokeFuncExtensions.BoolReduceOR);
		bool? flag3 = parentInventory?.canToggleSlotItemFunc?.InvokeAllReduce(this, parentInventory, incompleteItemInventorySlot, InvokeFuncExtensions.BoolReduceOR);
		flag = flag & obj & flag3;
		if ((flag2 | obj | flag3) == true && flag != false)
		{
			return true;
		}
		return false;
	}

	public void ToggleSlotItem()
	{
		if (parentInventory != null)
		{
			SlotMarker incompleteItemInventorySlot = parentInventory.GetIncompleteItemInventorySlot(this);
			base.onToggleSlotItemFunc?.Invoke(this, parentInventory, incompleteItemInventorySlot);
			parentInventory?.onToggleSlotItemFunc?.Invoke(this, parentInventory, incompleteItemInventorySlot);
			AudioManager.Instance.TryPlayOne(soundToggle);
			OverlayHandler.current.UpdateInventoryStateUI();
		}
	}

	public bool MayConfigureSlotItem()
	{
		if (parentInventory == null)
		{
			return false;
		}
		if (IsTag("CONFIGURE_TAG"))
		{
			return base.onConfigureSlotItemFunc != null;
		}
		return false;
	}

	public bool CanConfigureSlotItem()
	{
		if (MayConfigureSlotItem())
		{
			return GeneralHelper.IsItemEffectivelyOwned(this);
		}
		return false;
	}

	public void ConfigureSlotItem()
	{
		if (parentInventory != null)
		{
			SlotMarker incompleteItemInventorySlot = parentInventory.GetIncompleteItemInventorySlot(this);
			StallWatchdog.Crumb("configure " + identifier);
			base.onConfigureSlotItemFunc?.Invoke(this, parentInventory, incompleteItemInventorySlot);
		}
	}

	public bool MayActivateSlotItem()
	{
		if (forceDisableActivate)
		{
			return false;
		}
		if (parentInventory == null)
		{
			return false;
		}
		if (!GeneralHelper.IsItemEffectivelyOwned(this) && !GeneralHelper.MayInteractNonOwnedActivate(this))
		{
			return false;
		}
		bool? flag = null;
		bool? flag2 = null;
		SlotMarker incompleteItemInventorySlot = parentInventory.GetIncompleteItemInventorySlot(this);
		bool? obj = base.mayActivateSlotItemFunc?.InvokeAllReduce(this, parentInventory, incompleteItemInventorySlot, InvokeFuncExtensions.BoolReduceOR);
		bool? flag3 = parentInventory?.mayActivateSlotItemFunc?.InvokeAllReduce(this, parentInventory, incompleteItemInventorySlot, InvokeFuncExtensions.BoolReduceOR);
		flag = flag & obj & flag3;
		if ((flag2 | obj | flag3) == true && flag != false)
		{
			return true;
		}
		return false;
	}

	public bool CanActivateSlotItem()
	{
		if (parentInventory == null)
		{
			return false;
		}
		if (!GeneralHelper.IsItemEffectivelyOwned(this) && !GeneralHelper.MayInteractNonOwnedActivate(this))
		{
			return false;
		}
		bool? flag = null;
		bool? flag2 = null;
		SlotMarker incompleteItemInventorySlot = parentInventory.GetIncompleteItemInventorySlot(this);
		bool? obj = base.canActivateSlotItemFunc?.InvokeAllReduce(this, parentInventory, incompleteItemInventorySlot, InvokeFuncExtensions.BoolReduceOR);
		bool? flag3 = parentInventory?.canActivateSlotItemFunc?.InvokeAllReduce(this, parentInventory, incompleteItemInventorySlot, InvokeFuncExtensions.BoolReduceOR);
		flag = flag & obj & flag3;
		if ((flag2 | obj | flag3) == true && flag != false)
		{
			return true;
		}
		return false;
	}

	public void ActivateSlotItem()
	{
		if (parentInventory != null)
		{
			SlotMarker incompleteItemInventorySlot = parentInventory.GetIncompleteItemInventorySlot(this);
			StallWatchdog.Crumb("activate " + identifier);
			base.onActivateSlotItemFunc?.Invoke(this, parentInventory, incompleteItemInventorySlot);
			parentInventory?.onActivateSlotItemFunc?.Invoke(this, parentInventory, incompleteItemInventorySlot);
			AudioManager.Instance.TryPlayOne(soundActivate);
			OverlayHandler.current.UpdateInventoryStateUI();
		}
	}

	public bool MaySelectSlotItem()
	{
		if (forceDisableUse)
		{
			return false;
		}
		if (parentInventory == null)
		{
			return false;
		}
		if (!GameMaster.current.InCombat() && !canUseOutsideCombat)
		{
			return false;
		}
		if (!GeneralHelper.IsItemEffectivelyOwned(this))
		{
			return false;
		}
		bool? flag = null;
		bool? flag2 = null;
		SlotMarker incompleteItemInventorySlot = parentInventory.GetIncompleteItemInventorySlot(this);
		bool? obj = base.maySelectSlotItemFunc?.InvokeAllReduce(this, parentInventory, incompleteItemInventorySlot, InvokeFuncExtensions.BoolReduceOR);
		bool? flag3 = parentInventory?.maySelectSlotItemFunc?.InvokeAllReduce(this, parentInventory, incompleteItemInventorySlot, InvokeFuncExtensions.BoolReduceOR);
		flag = flag & obj & flag3;
		if ((flag2 | obj | flag3) == true && flag != false)
		{
			return true;
		}
		return false;
	}

	public bool CanSelectSlotItem()
	{
		if (parentInventory == null)
		{
			return false;
		}
		if (!GeneralHelper.IsItemEffectivelyOwned(this))
		{
			return false;
		}
		bool? flag = null;
		bool? flag2 = null;
		SlotMarker incompleteItemInventorySlot = parentInventory.GetIncompleteItemInventorySlot(this);
		bool? obj = base.canSelectSlotItemFunc?.InvokeAllReduce(this, parentInventory, incompleteItemInventorySlot, InvokeFuncExtensions.BoolReduceOR);
		bool? flag3 = parentInventory?.canSelectSlotItemFunc?.InvokeAllReduce(this, parentInventory, incompleteItemInventorySlot, InvokeFuncExtensions.BoolReduceOR);
		flag = flag & obj & flag3;
		if ((flag2 | obj | flag3) == true && flag != false)
		{
			return true;
		}
		return false;
	}

	public bool MayTarget(GameItem targetItem)
	{
		if (forceDisableUse)
		{
			return false;
		}
		if (!GameMaster.current.InCombat() && !canUseOutsideCombat)
		{
			return false;
		}
		if (!GeneralHelper.IsItemEffectivelyOwned(this))
		{
			return false;
		}
		if (targetItem != null && !GeneralHelper.IsItemEffectivelyOwned(targetItem) && !IsTag("CAN_USE_ON_NON_OWNED_TAG"))
		{
			return false;
		}
		bool? flag = null;
		bool? flag2 = null;
		bool? obj = this?.mayItemTargetItemFunc?.InvokeAllReduce(this, targetItem, InvokeFuncExtensions.BoolReduceOR);
		bool? flag3 = targetItem?.mayItemTargetItemFunc?.InvokeAllReduce(this, targetItem, InvokeFuncExtensions.BoolReduceOR);
		flag = flag & obj & flag3;
		flag2 = flag2 | obj | flag3;
		bool? obj2 = this?.mayThisTargetItemFunc?.InvokeAllReduce(this, targetItem, InvokeFuncExtensions.BoolReduceOR);
		flag3 = targetItem?.mayItemTargetThisFunc?.InvokeAllReduce(this, targetItem, InvokeFuncExtensions.BoolReduceOR);
		flag = flag & obj2 & flag3;
		flag2 = flag2 | obj2 | flag3;
		if (this?.parentInventory != null)
		{
			SlotMarker incompleteItemInventorySlot = parentInventory.GetIncompleteItemInventorySlot(this);
			bool? obj3 = maySlotItemTargetItemFunc?.InvokeAllReduce(this, parentInventory, incompleteItemInventorySlot, targetItem, InvokeFuncExtensions.BoolReduceOR);
			flag3 = parentInventory?.maySlotItemTargetItemFunc?.InvokeAllReduce(this, parentInventory, incompleteItemInventorySlot, targetItem, InvokeFuncExtensions.BoolReduceOR);
			flag = flag & obj3 & flag3;
			flag2 = flag2 | obj3 | flag3;
		}
		if (targetItem?.parentInventory != null)
		{
			SlotMarker incompleteItemInventorySlot2 = targetItem.parentInventory.GetIncompleteItemInventorySlot(targetItem);
			bool? obj4 = targetItem.mayItemTargetSlotItemFunc?.InvokeAllReduce(this, targetItem, targetItem.parentInventory, incompleteItemInventorySlot2, InvokeFuncExtensions.BoolReduceOR);
			flag3 = targetItem.parentInventory?.mayItemTargetSlotItemFunc?.InvokeAllReduce(this, targetItem, targetItem.parentInventory, incompleteItemInventorySlot2, InvokeFuncExtensions.BoolReduceOR);
			flag = flag & obj4 & flag3;
			flag2 = flag2 | obj4 | flag3;
		}
		if (flag2 == true && flag != false)
		{
			return true;
		}
		return false;
	}

	public bool CanTarget(GameItem targetItem)
	{
		if (!GeneralHelper.IsItemEffectivelyOwned(this))
		{
			return false;
		}
		if (targetItem != null && !GeneralHelper.IsItemEffectivelyOwned(targetItem) && !IsTag("CAN_USE_ON_NON_OWNED_TAG"))
		{
			return false;
		}
		bool? flag = null;
		bool? flag2 = null;
		bool? obj = this?.canItemTargetItemFunc?.InvokeAllReduce(this, targetItem, InvokeFuncExtensions.BoolReduceOR);
		bool? flag3 = targetItem?.canItemTargetItemFunc?.InvokeAllReduce(this, targetItem, InvokeFuncExtensions.BoolReduceOR);
		flag = flag & obj & flag3;
		flag2 = flag2 | obj | flag3;
		bool? obj2 = this?.canThisTargetItemFunc?.InvokeAllReduce(this, targetItem, InvokeFuncExtensions.BoolReduceOR);
		flag3 = targetItem?.canItemTargetThisFunc?.InvokeAllReduce(this, targetItem, InvokeFuncExtensions.BoolReduceOR);
		flag = flag & obj2 & flag3;
		flag2 = flag2 | obj2 | flag3;
		if (this?.parentInventory != null)
		{
			SlotMarker incompleteItemInventorySlot = parentInventory.GetIncompleteItemInventorySlot(this);
			bool? obj3 = canSlotItemTargetItemFunc?.InvokeAllReduce(this, parentInventory, incompleteItemInventorySlot, targetItem, InvokeFuncExtensions.BoolReduceOR);
			flag3 = parentInventory?.canSlotItemTargetItemFunc?.InvokeAllReduce(this, parentInventory, incompleteItemInventorySlot, targetItem, InvokeFuncExtensions.BoolReduceOR);
			flag = flag & obj3 & flag3;
			flag2 = flag2 | obj3 | flag3;
		}
		if (targetItem?.parentInventory != null)
		{
			SlotMarker incompleteItemInventorySlot2 = targetItem.parentInventory.GetIncompleteItemInventorySlot(targetItem);
			bool? obj4 = targetItem.canItemTargetSlotItemFunc?.InvokeAllReduce(this, targetItem, targetItem.parentInventory, incompleteItemInventorySlot2, InvokeFuncExtensions.BoolReduceOR);
			flag3 = targetItem.parentInventory?.canItemTargetSlotItemFunc?.InvokeAllReduce(this, targetItem, targetItem.parentInventory, incompleteItemInventorySlot2, InvokeFuncExtensions.BoolReduceOR);
			flag = flag & obj4 & flag3;
			flag2 = flag2 | obj4 | flag3;
		}
		if (flag2 == true && flag != false)
		{
			return true;
		}
		return false;
	}

	public void Target(GameItem targetItem)
	{
		this?.onItemTargetItemFunc?.Invoke(this, targetItem);
		targetItem?.onItemTargetItemFunc?.Invoke(this, targetItem);
		this?.onThisTargetItemFunc?.Invoke(this, targetItem);
		targetItem?.onItemTargetThisFunc?.Invoke(this, targetItem);
		if (this?.parentInventory != null)
		{
			SlotMarker incompleteItemInventorySlot = parentInventory.GetIncompleteItemInventorySlot(this);
			this?.onSlotItemTargetItemFunc?.Invoke(this, this?.parentInventory, incompleteItemInventorySlot, targetItem);
			this?.parentInventory?.onSlotItemTargetItemFunc?.Invoke(this, this?.parentInventory, incompleteItemInventorySlot, targetItem);
		}
		if (targetItem?.parentInventory != null)
		{
			SlotMarker incompleteItemInventorySlot2 = targetItem.parentInventory.GetIncompleteItemInventorySlot(targetItem);
			targetItem?.onItemTargetSlotItemFunc?.Invoke(this, targetItem, targetItem?.parentInventory, incompleteItemInventorySlot2);
			targetItem?.parentInventory?.onItemTargetSlotItemFunc?.Invoke(this, targetItem, targetItem?.parentInventory, incompleteItemInventorySlot2);
		}
		AudioManager.Instance.TryPlayOne(soundUse);
		OverlayHandler.current.UpdateInventoryStateUI();
	}

	public bool MayParentShapeModifier()
	{
		if (parentInventory?.parentItemShapeModifierForItemFunc == null)
		{
			return base.parentItemShapeModifierForItemFunc != null;
		}
		return true;
	}

	public (GridShape, GridShape) GetParentShapeModifier(GameItem newParentItem, SlotMarker newSlot, GridShape newModifiedShape, TagSystem newState)
	{
		if (newParentItem == null)
		{
			return (null, null);
		}
		if (newSlot == null)
		{
			return (null, null);
		}
		if (newModifiedShape == null)
		{
			return (null, null);
		}
		if (newState == null)
		{
			return (null, null);
		}
		GameInventory inventory = newSlot.inventory;
		if (inventory == null)
		{
			return (null, null);
		}
		if (inventory?.parentItemShapeModifierForItemFunc == null && base.parentItemShapeModifierForItemFunc == null)
		{
			return (null, null);
		}
		GridShape item = base.parentItemShapeModifierForItemFunc?.Invoke(this, inventory, newSlot, newModifiedShape, newState, newParentItem);
		GridShape item2 = inventory?.parentItemShapeModifierForItemFunc?.Invoke(this, inventory, newSlot, newModifiedShape, newState, newParentItem);
		return (item, item2);
	}

	public (List<ItemStateModifier>, List<ItemStateModifier>) GetParentStateModifier(GameItem newParentItem, SlotMarker newSlot, GridShape newModifiedShape, TagSystem newState)
	{
		if (newParentItem == null)
		{
			return (null, null);
		}
		if (newSlot == null)
		{
			return (null, null);
		}
		if (newModifiedShape == null)
		{
			return (null, null);
		}
		if (newState == null)
		{
			return (null, null);
		}
		GameInventory inventory = newSlot.inventory;
		if (inventory == null)
		{
			return (null, null);
		}
		if (inventory?.parentItemStateModifierForItemFunc == null && base.parentItemStateModifierForItemFunc == null)
		{
			return (null, null);
		}
		List<ItemStateModifier> item = base.parentItemStateModifierForItemFunc?.Invoke(this, inventory, newSlot, newModifiedShape, newState, newParentItem);
		List<ItemStateModifier> item2 = inventory?.parentItemStateModifierForItemFunc?.Invoke(this, inventory, newSlot, newModifiedShape, newState, newParentItem);
		return (item, item2);
	}

	public bool MayRemove()
	{
		if (parentInventory == null)
		{
			return false;
		}
		bool? flag = null;
		bool? flag2 = null;
		if (parentInventory.IsRemoveLocked())
		{
			return false;
		}
		if (GeneralHelper.IsInsideNonOwnedContainer(this))
		{
			return false;
		}
		bool? obj = base.mayInventoryRemoveItemFunc?.InvokeAllReduce(this, parentInventory, InvokeFuncExtensions.BoolReduceOR);
		bool? flag3 = parentInventory?.mayInventoryRemoveItemFunc?.InvokeAllReduce(this, parentInventory, InvokeFuncExtensions.BoolReduceOR);
		flag = flag & obj & flag3;
		flag2 = flag2 | obj | flag3;
		if (flag != false)
		{
			return true;
		}
		return false;
	}

	public int MaxNumRemove()
	{
		if (parentInventory == null)
		{
			return unitCount;
		}
		SlotMarker incompleteItemInventorySlot = parentInventory.GetIncompleteItemInventorySlot(this);
		if (!parentInventory.CheckRemoveParentShapeModifier(this))
		{
			return 0;
		}
		int num = Math.Max(incompleteItemInventorySlot.inventory.minSlotAmountItemFunc?.InvokeAllReduce(this, incompleteItemInventorySlot.inventory, incompleteItemInventorySlot, InvokeFuncExtensions.BoolReduceMin) ?? 0, base.minSlotAmountItemFunc?.InvokeAllReduce(this, incompleteItemInventorySlot.inventory, incompleteItemInventorySlot, InvokeFuncExtensions.BoolReduceMin) ?? 0);
		int num2 = unitCount - num;
		int num3 = Math.Min(incompleteItemInventorySlot.inventory.maxSlotRemoveItemFunc?.InvokeAllReduce(this, incompleteItemInventorySlot.inventory, incompleteItemInventorySlot, InvokeFuncExtensions.BoolReduceMin) ?? int.MaxValue, base.maxSlotRemoveItemFunc?.InvokeAllReduce(this, incompleteItemInventorySlot.inventory, incompleteItemInventorySlot, InvokeFuncExtensions.BoolReduceMin) ?? int.MaxValue);
		if (num3 < num2)
		{
			num2 = num3;
		}
		return num2;
	}

	public int MaxNumAdd()
	{
		if (parentInventory == null)
		{
			return int.MaxValue;
		}
		SlotMarker incompleteItemInventorySlot = parentInventory.GetIncompleteItemInventorySlot(this);
		incompleteItemInventorySlot.targetItem = this;
		return MaxNumAdd(incompleteItemInventorySlot);
	}

	public int MaxNumAdd(SlotMarker slot)
	{
		if (slot == null)
		{
			return 0;
		}
		if (slot.inventory == null)
		{
			return int.MaxValue;
		}
		int num = Math.Min(slot.inventory.maxSlotAmountItemFunc?.InvokeAllReduce(this, slot.inventory, slot, InvokeFuncExtensions.BoolReduceMax) ?? int.MaxValue, base.maxSlotAmountItemFunc?.InvokeAllReduce(this, slot.inventory, slot, InvokeFuncExtensions.BoolReduceMax) ?? int.MaxValue);
		if (slot.inventory.maxSlotAmountItemFunc == null && base.maxSlotAmountItemFunc == null)
		{
			num = 1;
		}
		int num2 = num - (slot.targetItem?.unitCount ?? 0);
		int num3 = Math.Min(slot.inventory.maxSlotAddItemFunc?.InvokeAllReduce(this, slot.inventory, slot, InvokeFuncExtensions.BoolReduceMax) ?? int.MaxValue, base.maxSlotAddItemFunc?.InvokeAllReduce(this, slot.inventory, slot, InvokeFuncExtensions.BoolReduceMax) ?? int.MaxValue);
		if (num3 < num2)
		{
			num2 = num3;
		}
		return num2;
	}

	public bool MayStack(GameItem otherItem)
	{
		if (otherItem == null)
		{
			return false;
		}
		Func<GameItem, GameItem, bool> func = otherItem.mayStackItemFunc;
		if (func == null || !func.InvokeAllReduce(this, otherItem, InvokeFuncExtensions.BoolReduceOR))
		{
			return false;
		}
		Func<GameItem, GameItem, bool> func2 = base.mayStackItemFunc;
		if (func2 == null || !func2.InvokeAllReduce(this, otherItem, InvokeFuncExtensions.BoolReduceOR))
		{
			return false;
		}
		return true;
	}

	public SlotMarker GetStackSlot(GameItem itemToBeStacked)
	{
		if (parentInventory == null)
		{
			return null;
		}
		if (this == itemToBeStacked)
		{
			return null;
		}
		if (!MayStack(itemToBeStacked))
		{
			return null;
		}
		SlotMarker incompleteItemInventorySlot = parentInventory.GetIncompleteItemInventorySlot(this);
		incompleteItemInventorySlot.item = itemToBeStacked;
		incompleteItemInventorySlot.targetItem = this;
		incompleteItemInventorySlot.numTransfer = MaxNumAdd();
		return incompleteItemInventorySlot;
	}

	public SlotMarker GetIncompleteSlotInInventory()
	{
		if (parentInventory == null)
		{
			return null;
		}
		return parentInventory.GetIncompleteItemInventorySlot(this);
	}

	public GameItem GetSplitItem(int requestedAmount)
	{
		unitCount -= requestedAmount;
		Validate();
		base.onChangeAmountItemFunc?.Invoke(this);
		if (parentInventory != null)
		{
			base.onSlotChangeAmountItemFunc?.Invoke(this, parentInventory, parentInventory.GetIncompleteItemInventorySlot(this));
			parentInventory?.onSlotChangeAmountItemFunc?.Invoke(this, parentInventory, parentInventory.GetIncompleteItemInventorySlot(this));
		}
		GameItem gameItem = DirectoryMaster.Item(identifier);
		gameItem.unitCount = requestedAmount;
		gameItem?.onChangeAmountItemFunc?.Invoke(gameItem);
		if (gameItem?.parentInventory != null)
		{
			gameItem.onSlotChangeAmountItemFunc?.Invoke(gameItem, gameItem.parentInventory, gameItem.parentInventory.GetIncompleteItemInventorySlot(gameItem));
			gameItem.parentInventory?.onSlotChangeAmountItemFunc?.Invoke(gameItem, gameItem.parentInventory, gameItem.parentInventory.GetIncompleteItemInventorySlot(gameItem));
		}
		OverlayHandler.current.UpdateInventoryStateUI();
		return gameItem;
	}

	public bool CheckIsStateValid(GameInventory parentInventory, SlotMarker slot, GridShape tempShape, TagSystem itemState)
	{
		if (parentInventory == null)
		{
			return false;
		}
		bool? flag = null;
		bool? flag2 = null;
		bool? obj = base.isItemStateValidFunc?.Invoke(this, parentInventory, slot, tempShape, itemState);
		bool? flag3 = parentInventory?.isItemStateValidFunc?.Invoke(this, parentInventory, slot, tempShape, itemState);
		flag = flag & obj & flag3;
		flag2 = flag2 | obj | flag3;
		if (flag != false)
		{
			return true;
		}
		return false;
	}

	public bool CheckValidShapeState()
	{
		if (parentInventory == null)
		{
			return true;
		}
		var (tempShape, _, _, tempState) = ComputeModifiedShapeState(null, null, null, null, null);
		return parentInventory.CheckInventorySlotStillValid(this, tempShape, tempState);
	}

	public void SyncModifiedState()
	{
		(GridShape, int, int, TagSystem) tuple = ComputeModifiedShapeState(null, null, null, null, null);
		(modifiedShape, modifiedXOrigin, modifiedYOrigin, modifiedState) = tuple;
	}

	public void ValidateShapeState()
	{
		_ = modifiedShape.globalWidth;
		_ = modifiedShape.globalHeight;
		(GridShape, int, int, TagSystem) tuple = ComputeModifiedShapeState(null, null, null, null, null);
		(modifiedShape, modifiedXOrigin, modifiedYOrigin, modifiedState) = tuple;
		Validate();
		if (parentInventory == null)
		{
			return;
		}
		parentInventory.Validate();
		if (!MayParentShapeModifier())
		{
			return;
		}
		foreach (GameItem item in GraphUtils.FindParentsType<GameItem>(parentInventory))
		{
			item.ValidateShapeState();
		}
		OverlayHandler.current.UpdateInventoryStateUI();
	}

	public (GridShape, int, int, TagSystem) ComputeModifiedShapeState(GridShape addModItem, GridShape addModInv, List<ItemStateModifier> addStateItem, List<ItemStateModifier> addStateInv, GameItem ignoreItem)
	{
		List<GameInventory> list = GraphUtils.FindChildrenType<GameInventory>(this);
		List<GridShape> list2 = new List<GridShape>();
		List<ItemStateModifier> list3 = new List<ItemStateModifier>();
		foreach (GameInventory item7 in list)
		{
			foreach (GameItem childItem in item7.childItems)
			{
				if (childItem != ignoreItem)
				{
					var (gridShape, gridShape2) = childItem.GetParentShapeModifier(this, item7.GetIncompleteItemInventorySlot(childItem), childItem.modifiedShape, childItem.state);
					if (gridShape != null)
					{
						list2.Add(gridShape);
					}
					if (gridShape2 != null)
					{
						list2.Add(gridShape2);
					}
					var (list4, list5) = childItem.GetParentStateModifier(this, item7.GetIncompleteItemInventorySlot(childItem), childItem.modifiedShape, childItem.state);
					if (list4 != null)
					{
						list3.AddRange(list4);
					}
					if (list5 != null)
					{
						list3.AddRange(list5);
					}
				}
			}
		}
		if (addModItem != null)
		{
			list2.Add(addModItem);
		}
		if (addModInv != null)
		{
			list2.Add(addModInv);
		}
		if (addStateItem != null)
		{
			list3.AddRange(addStateItem);
		}
		if (addStateInv != null)
		{
			list3.AddRange(addStateInv);
		}
		GridShapeBuilder gridShapeBuilder = new GridShapeBuilder(shape);
		(int, int) tuple3 = modifiedShape.ForwardTransform(modifiedXOrigin, modifiedYOrigin);
		int item = tuple3.Item1;
		int item2 = tuple3.Item2;
		(int, int) tuple4 = modifiedShape.ForwardTransform(modifiedXOrigin + shape.width - 1, modifiedYOrigin + shape.height - 1);
		int item3 = tuple4.Item1;
		int item4 = tuple4.Item2;
		int minX = Math.Min(item, item3);
		int minY = Math.Min(item2, item4);
		gridShapeBuilder.SetTransform(minX, minY, modifiedShape.flipped, modifiedShape.orientation);
		var (item5, item6) = gridShapeBuilder.MergeWithLocalShapes(list2);
		list3.OrderBy((ItemStateModifier a) => a.priority);
		TagSystem newDict = state.Clone();
		list3.ForEach(delegate(ItemStateModifier s)
		{
			s.Apply(newDict);
		});
		return (gridShapeBuilder, item5, item6, newDict);
	}

	public List<GameItem> GetSiblingItems()
	{
		List<GameItem> list = new List<GameItem>();
		if (child != null && child.parentItems != null)
		{
			foreach (GameItem parentItem in child.parentItems)
			{
				if (parentItem != this)
				{
					list.Add(parentItem);
				}
			}
		}
		return list;
	}

	public GameItem TryAcceptAll(GameItem item)
	{
		GraphUtils.TryAcceptAll(this, item);
		return this;
	}

	public GameItem ExpelAll()
	{
		GraphUtils.FindChildrenType<GameInventory>(this).ForEach(delegate(GameInventory inv)
		{
			inv.ExpelAll();
		});
		return this;
	}

	public GameItem TryUnloadAll()
	{
		return TryUnloadAll(skipPinned: false);
	}

	public GameItem TryUnloadAll(bool skipPinned)
	{
		if (IsTag("IS_UNLOAD_DISABLED_TAG"))
		{
			return this;
		}
		if (contentWindow != null && parentInventory != null)
		{
			GraphUtils.FindChildrenType<GameItem>(contentWindow).ForEach(delegate(GameItem gi)
			{
				if ((!skipPinned || !ItemPinHelper.IsPinned(gi)) && (!skipPinned || gi.MayRemove()) && !HydroponicHelper.IsGrowingSeed(gi) && !gi.IsTag("MODULE_STUCK_TAG") && !GeneralHelper.IsItemLocked(gi))
				{
					if (base.onUnloadChildItemFunc != null)
					{
						base.onUnloadChildItemFunc(this, gi);
					}
					else
					{
						GraphUtils.TryAcceptAll(parentInventory, gi);
					}
				}
			});
		}
		return this;
	}

	public List<GameItem> FindAllChildItems()
	{
		return GraphUtils.FindAllChildrenType<GameItem>(this);
	}

	public List<CraftingRecipe> FindAllRecipes()
	{
		List<CraftingRecipe> list = new List<CraftingRecipe>();
		foreach (GameItem item in FindAllChildItems())
		{
			if (item?.recipeManager != null)
			{
				list.AddRange(item.recipeManager.availableRecipes);
			}
		}
		if (recipeManager != null)
		{
			list.AddRange(recipeManager.availableRecipes);
		}
		return list;
	}

	public CombatAbilityEffect.TargetType GetPrimaryTarget()
	{
		if (effects.Count == 0)
		{
			return CombatAbilityEffect.TargetType.none;
		}
		return effects[0].abilityTargetType;
	}

	public bool IsAlwaysAccurate()
	{
		if (bonusAccuracy == 1000)
		{
			return true;
		}
		return false;
	}

	public void AddAmenities(string type, int value, int use)
	{
		EnableTag("AMENITY_ITEM_TAG");
		SetType("AMENITY_ITEM_TAG");
		ModifyTag("amenities", delegate(TagState state)
		{
			state.Enable().SetInt(value);
		});
		EnableTag(type);
		ModifyTag("amenities_type", delegate(TagState state)
		{
			state.Enable().SetString(type);
		});
		ModifyTag("amenities_use", delegate(TagState state)
		{
			state.Enable().SetInt(use);
		});
	}

	public void EnableProficiency(string proficiencyType)
	{
		EnableTag("proficiency");
		ModifyTag("proficiency", delegate(TagState state)
		{
			state.SetString(proficiencyType);
		});
	}

	public void EnableManufacturer(string manufacturer)
	{
		EnableTag("manufacturer");
		ModifyTag("manufacturer", delegate(TagState state)
		{
			state.SetString(manufacturer);
		});
	}

	public void DisplayManufacturer(RichTextBuilder builder, string manufacturer)
	{
		builder.Add(" ");
		if (manufacturer.Equals("pastel"))
		{
			builder.AddLine(LocHelper.GetLocalizedClientName("corpo_name_pastel"), ignoreEmpty: true, RenderHandler.ColorPalette.VeryLightGreen, bold: false, italic: false, underline: true);
		}
		else if (manufacturer.Equals("carmicheal"))
		{
			builder.AddLine(LocHelper.GetLocalizedClientName("corpo_name_carmicheal"), ignoreEmpty: true, RenderHandler.ColorPalette.LightTurquoise, bold: false, italic: false, underline: true);
		}
		else if (manufacturer.Equals("davre"))
		{
			builder.AddLine(LocHelper.GetLocalizedClientName("corpo_name_davre"), ignoreEmpty: true, RenderHandler.ColorPalette.Green, bold: false, italic: false, underline: true);
		}
		else if (manufacturer.Equals("mnsl"))
		{
			builder.AddLine(LocHelper.GetLocalizedClientName("corpo_name_mnsl"), ignoreEmpty: true, RenderHandler.ColorPalette.Red, bold: false, italic: false, underline: true);
		}
		else if (manufacturer.Equals("eminence"))
		{
			builder.AddLine(LocHelper.GetLocalizedClientName("corpo_name_eminence"), ignoreEmpty: true, RenderHandler.ColorPalette.LightYellow, bold: false, italic: false, underline: true);
		}
		else if (manufacturer.Equals("eninence"))
		{
			builder.AddLine(LocHelper.GetLocalizedClientName("corpo_name_eninence"), ignoreEmpty: true, RenderHandler.ColorPalette.LightYellow, bold: false, italic: false, underline: true);
		}
		else if (manufacturer.Equals("republic"))
		{
			builder.AddLine(LocHelper.GetLocalizedClientName("corpo_name_republic"), ignoreEmpty: true, RenderHandler.ColorPalette.LightBlue, bold: false, italic: false, underline: true);
		}
		else if (manufacturer.Equals("republic_navy"))
		{
			builder.AddLine(LocHelper.GetLocalizedClientName("corpo_name_republic_navy"), ignoreEmpty: true, RenderHandler.ColorPalette.Blue, bold: false, italic: false, underline: true);
		}
		else if (manufacturer.Equals("neural"))
		{
			builder.AddLine(LocHelper.GetLocalizedClientName("corpo_name_neural"), ignoreEmpty: true, RenderHandler.ColorPalette.VeryLightGray, bold: false, italic: false, underline: true);
		}
		else if (manufacturer.Equals("watch"))
		{
			builder.AddLine(LocHelper.GetLocalizedClientName("corpo_name_watch"), ignoreEmpty: true, RenderHandler.ColorPalette.Black, bold: false, italic: false, underline: true);
		}
		else if (manufacturer.Equals("front"))
		{
			builder.AddLine(LocHelper.GetLocalizedClientName("corpo_name_front"), ignoreEmpty: true, RenderHandler.ColorPalette.LightRed, bold: false, italic: false, underline: true);
		}
		else if (manufacturer.Equals("sokolov"))
		{
			builder.AddLine(LocHelper.GetLocalizedClientName("corpo_name_sokolov"), ignoreEmpty: true, RenderHandler.ColorPalette.Red, bold: false, italic: false, underline: true);
		}
		else if (manufacturer.Equals("vision"))
		{
			builder.AddLine(LocHelper.GetLocalizedClientName("corpo_name_vision"), ignoreEmpty: true, RenderHandler.ColorPalette.LightBlue, bold: false, italic: false, underline: true);
		}
		else if (manufacturer.Equals("zhonghua"))
		{
			builder.AddLine(LocHelper.GetLocalizedClientName("corpo_name_zhonghua"), ignoreEmpty: true, RenderHandler.ColorPalette.LightRed, bold: false, italic: false, underline: true);
		}
		else if (manufacturer.Equals("spacer"))
		{
			builder.AddLine(LocHelper.GetLocalizedClientName("corpo_name_spacer"), ignoreEmpty: true, RenderHandler.ColorPalette.LightGray, bold: false, italic: false, underline: true);
		}
		else if (manufacturer.Equals("meridian"))
		{
			builder.AddLine(LocHelper.GetLocalizedClientName("corpo_name_meridian"), ignoreEmpty: true, RenderHandler.ColorPalette.LightRed, bold: false, italic: false, underline: true);
		}
		else if (manufacturer.Equals("echelon"))
		{
			builder.AddLine(LocHelper.GetLocalizedClientName("corpo_name_echelon"), ignoreEmpty: true, RenderHandler.ColorPalette.LightBlue, bold: false, italic: false, underline: true);
		}
		else if (manufacturer.Equals("blueshield"))
		{
			builder.AddLine(LocHelper.GetLocalizedClientName("corpo_name_blueshield"), ignoreEmpty: true, RenderHandler.ColorPalette.Blue, bold: false, italic: false, underline: true);
		}
		else if (manufacturer.Equals("frontier"))
		{
			builder.AddLine(LocHelper.GetLocalizedClientName("corpo_name_frontier"), ignoreEmpty: true, RenderHandler.ColorPalette.Green, bold: false, italic: false, underline: true);
		}
		else if (manufacturer.Equals("ashvins"))
		{
			builder.AddLine(LocHelper.GetLocalizedClientName("corpo_name_ashvins"), ignoreEmpty: true, RenderHandler.ColorPalette.Green, bold: false, italic: false, underline: true);
		}
		else if (manufacturer.Equals("solana"))
		{
			builder.AddLine(LocHelper.GetLocalizedClientName("corpo_name_solana"), ignoreEmpty: true, RenderHandler.ColorPalette.Turquoise, bold: false, italic: false, underline: true);
		}
		else if (manufacturer.Equals("alison"))
		{
			builder.AddLine(LocHelper.GetLocalizedClientName("corpo_name_alison"), ignoreEmpty: true, RenderHandler.ColorPalette.LightBlue, bold: false, italic: false, underline: true);
		}
		else if (manufacturer.Equals("harvey_bee"))
		{
			builder.AddLine(LocHelper.GetLocalizedOrDefault("ClientName", "corpo_name_harvey_bee", "M. Harvey Bee"), ignoreEmpty: true, RenderHandler.ColorPalette.LightBlue, bold: false, italic: false, underline: true);
		}
		else if (manufacturer.Equals("georgio"))
		{
			builder.AddLine(LocHelper.GetLocalizedClientName("name_georgio"), ignoreEmpty: true, RenderHandler.ColorPalette.Red, bold: false, italic: false, underline: true);
		}
	}

	public void SetType(string type)
	{
		EnableTag(type);
		EnableTag(ITEM_TYPE_STRING);
		ModifyTag(ITEM_TYPE_STRING, delegate(TagState state)
		{
			state.SetString(type);
		});
	}

	private void AccumulateFeatureStages(ref long innateMult, ref long marketMult, ref double finalFactor, bool skipEventFeatures = false)
	{
		foreach (ItemFeature itemFeature in itemFeatures)
		{
			if (!itemFeature.isDisabled && (!skipEventFeatures || itemFeature.featureType != ItemFeature.FeatureType.Event))
			{
				switch (itemFeature.GetValueStage())
				{
				case ItemFeature.ValueStage.Market:
					marketMult += itemFeature.GetCurrentValueMod();
					break;
				case ItemFeature.ValueStage.Final:
					finalFactor *= Math.Max(0.0, 1.0 + (double)itemFeature.GetCurrentValueMod() / 100.0);
					break;
				default:
					innateMult += itemFeature.GetCurrentValueMod();
					break;
				}
			}
		}
	}

	private static long ComposeStagedValue(long totalBaseValue, long innateMult, long marketMult, double finalFactor, int miscValue, int marketScaledValue = 0)
	{
		double num = Math.Max(0.0, (double)totalBaseValue * (1.0 + (double)innateMult / 100.0));
		double num2 = Math.Max(0.0, 1.0 + (double)marketMult / 100.0);
		return (long)Math.Round(((num + (double)marketScaledValue) * num2 + (double)miscValue) * finalFactor);
	}

	public long GetNegociatedValue()
	{
		long complexeItemValue = GetComplexeItemValue();
		long totalBaseValue = unitValue + complexeItemValue;
		long innateMult = 0L;
		long marketMult = 0L;
		double finalFactor = 1.0;
		AccumulateFeatureStages(ref innateMult, ref marketMult, ref finalFactor);
		int itemMiscValue = GetItemMiscValue(useMarkup: true);
		long finalValue = ComposeStagedValue(totalBaseValue, innateMult, marketMult, finalFactor, itemMiscValue, WaterHelper.GetContainerPrice(this));
		if (DebugPanel.Instance != null && DebugPanel.Instance.debugPricePanel.activeSelf)
		{
			DebugPanel.Instance.priceDebugTMP2.text = "NegociatedValue\nbase " + totalBaseValue + ", innate " + innateMult + "%, market " + marketMult + "%, misc " + itemMiscValue + ", deal x" + finalFactor.ToString("0.00") + " = " + finalValue;
		}
		return CheckValueSafe(finalValue);
	}

	public long GetDonationCreditValue()
	{
		ItemFeature itemFeature = FindItemFeatureByID("bribeDonation");
		if (itemFeature == null)
		{
			return GetNegociatedValue();
		}
		bool isDisabled = itemFeature.isDisabled;
		itemFeature.isDisabled = true;
		long negociatedValue = GetNegociatedValue();
		itemFeature.isDisabled = isDisabled;
		return negociatedValue;
	}

	public long GetCurrentValue(bool useRetailMarkup = false, bool withChild = true, bool includeEvents = true, bool forceMarkup = false, long extraMarketMod = 0L)
	{
		List<StoreEvent> activeEvents = StoreStation.Instance.storeEventManager.GetActiveEvents();
		long complexeItemValue = GetComplexeItemValue();
		long totalBaseValue = unitValue + complexeItemValue;
		long innateMult = 0L;
		long marketMult = 0L;
		double finalFactor = 1.0;
		if (includeEvents)
		{
			foreach (StoreEvent item in activeEvents)
			{
				for (int i = 0; i < item.negociationDatas.Count; i++)
				{
					if (IsTag(item.negociationDatas[i].itemType))
					{
						marketMult += item.negociationDatas[i].value;
					}
				}
			}
		}
		AccumulateFeatureStages(ref innateMult, ref marketMult, ref finalFactor, skipEventFeatures: true);
		if (((IsApplyMarkUp() && useRetailMarkup) || forceMarkup) && FindItemFeatureByID("retailMarkUp") == null)
		{
			marketMult += PlayerStore.Instance.retailMarkup;
		}
		marketMult += extraMarketMod;
		int itemMiscValue = GetItemMiscValue(useRetailMarkup, withChild, includeEvents, forceMarkup);
		long finalValue = ComposeStagedValue(totalBaseValue, innateMult, marketMult, finalFactor, itemMiscValue, WaterHelper.GetContainerPrice(this));
		if (DebugPanel.Instance != null && DebugPanel.Instance.debugPricePanel.activeSelf)
		{
			DebugPanel.Instance.priceDebugTMP.text = "CurrentValue\nbase " + totalBaseValue + ", innate " + innateMult + "%, market " + marketMult + "%, misc " + itemMiscValue + ", deal x" + finalFactor.ToString("0.00") + " = " + finalValue;
			if (IsRootedInBackInv())
			{
				DebugPanel.Instance.priceDebugTMP.text += "\nIn back inv";
			}
			else
			{
				DebugPanel.Instance.priceDebugTMP.text += "\nNOT In back inv";
			}
			DebugPanel.Instance.priceDebugTMP3.text = "RefreshedValue\n" + GetRefreshedValue();
		}
		return CheckValueSafe(finalValue);
	}

	public static long CheckValueSafe(long finalValue)
	{
		if (finalValue <= 0)
		{
			finalValue = 0L;
		}
		return finalValue;
	}

	public long GetComplexeItemValue()
	{
		return 0L + (long)PowerHelper.GetPowerSourceItemValue(this);
	}

	public long GetRefreshedValue()
	{
		List<StoreEvent> activeEvents = StoreStation.Instance.storeEventManager.GetActiveEvents();
		List<GameItem> list = FindAllChildItems();
		list.Add(this);
		long num = 0L;
		foreach (GameItem item in list)
		{
			long innateMult = 0L;
			long marketMult = 0L;
			double finalFactor = 1.0;
			long complexeItemValue = item.GetComplexeItemValue();
			long totalBaseValue = item.unitValue + complexeItemValue;
			foreach (StoreEvent item2 in activeEvents)
			{
				for (int i = 0; i < item2.negociationDatas.Count; i++)
				{
					if (item.IsTag(item2.negociationDatas[i].itemType))
					{
						marketMult += item2.negociationDatas[i].value;
					}
				}
			}
			item.AccumulateFeatureStages(ref innateMult, ref marketMult, ref finalFactor, skipEventFeatures: true);
			int miscValue = (int)item.lateUnitValue;
			num += CheckValueSafe(ComposeStagedValue(totalBaseValue, innateMult, marketMult, finalFactor, miscValue, WaterHelper.GetContainerPrice(item)));
		}
		return num;
	}

	public long GetBaseValue()
	{
		long complexeItemValue = GetComplexeItemValue();
		int itemMiscValue = GetItemMiscValue(useMarkup: false, withChild: true, includeEvents: false, forceMarkup: false, inheritClientMods: false);
		return CheckValueSafe(unitValue + complexeItemValue + itemMiscValue + WaterHelper.GetContainerPrice(this));
	}

	public int GetItemMiscValue(bool useMarkup = false, bool withChild = true, bool includeEvents = true, bool forceMarkup = false, bool inheritClientMods = true)
	{
		int num = (int)lateUnitValue;
		if (IsTag("IGNORE_CHILD_VALUE"))
		{
			withChild = false;
		}
		if (withChild)
		{
			num += GetCurrentChildValue(useMarkup, includeEvents, forceMarkup, inheritClientMods);
		}
		return num;
	}

	public bool IsApplyMarkUp()
	{
		if (IsRootedInBackInv() && GeneralHelper.IsItemEffectivelyOwned(this) && PlayerStore.Instance.IsCurrentClientAcceptMarkup())
		{
			return true;
		}
		return false;
	}

	public int GetCurrentChildValue(bool useMarkup = false, bool includeEvents = true, bool forceMarkup = false, bool inheritClientMods = true)
	{
		long num = 0L;
		long extraMarketMod = (inheritClientMods ? GetInheritableClientMarketMod() : 0);
		foreach (GameItem item in FindAllChildItems())
		{
			if (!item.IsTag("ITEM_BEHIND_LOCK_TAG"))
			{
				num += item.GetCurrentValue(useMarkup, withChild: false, includeEvents, forceMarkup, extraMarketMod);
			}
		}
		return (int)num;
	}

	public long GetInheritableClientMarketMod()
	{
		long num = 0L;
		foreach (ItemFeature itemFeature in itemFeatures)
		{
			if (!itemFeature.isDisabled && itemFeature.GetValueStage() == ItemFeature.ValueStage.Market && !itemFeature.identifier.Equals("retailMarkUp") && (itemFeature.featureType == ItemFeature.FeatureType.Temporary || itemFeature.featureType == ItemFeature.FeatureType.TemporaryBuying || itemFeature.featureType == ItemFeature.FeatureType.TemporarySelling || itemFeature.featureType == ItemFeature.FeatureType.ClientTemporary))
			{
				num += itemFeature.GetCurrentValueMod();
			}
		}
		return num;
	}

	public void ApplyContrabandFeatureToChildren()
	{
		foreach (GameItem item in FindAllChildItems())
		{
			ContrabandHelper.ApplyContrabandFeature(item);
		}
	}

	public void RemoveContrabandFeature()
	{
		itemFeatures.RemoveAll((ItemFeature feature) => feature.category.Equals("contrabandMarkUp"));
	}

	public void RemoveContrabandFeatureFromChildren()
	{
		foreach (GameItem item in FindAllChildItems())
		{
			item.RemoveContrabandFeature();
		}
	}

	public bool IsInsideContainerPlacedForSelling()
	{
		if (PlayerStore.Instance.currentClientInstance == null)
		{
			return false;
		}
		if (!IsRootedInBackInv())
		{
			return false;
		}
		GameItem gameItem = this;
		GameItem gameItem2 = gameItem.parentInventory?.GetParentItem();
		if (gameItem2 == null)
		{
			return false;
		}
		while (gameItem2 != null)
		{
			gameItem = gameItem2;
			gameItem2 = gameItem.parentInventory?.GetParentItem();
		}
		StoreClient.ClientIntent clientIntent = PlayerStore.Instance.currentClientInstance.GetClientBlueprint().clientIntent;
		if (GeneralHelper.IsItemOwned(gameItem))
		{
			if (clientIntent != StoreClient.ClientIntent.BUY)
			{
				return clientIntent == StoreClient.ClientIntent.SELLNBUY;
			}
			return true;
		}
		return false;
	}

	public bool IsAlreadyContainFeatureWithId(string identifier)
	{
		foreach (ItemFeature itemFeature in itemFeatures)
		{
			if (itemFeature.identifier.Equals(identifier))
			{
				return true;
			}
		}
		return false;
	}

	public ItemFeature FindItemFeatureByID(string id)
	{
		foreach (ItemFeature itemFeature in itemFeatures)
		{
			if (id.Equals(itemFeature.identifier))
			{
				return itemFeature;
			}
		}
		return null;
	}

	public void RemoveItemFeatureByID(string id)
	{
		foreach (ItemFeature item in itemFeatures.ToList())
		{
			if (id.Equals(item.identifier))
			{
				itemFeatures.Remove(item);
				break;
			}
		}
	}

	public ItemFeature FindItemFeatureByCategory(string category)
	{
		foreach (ItemFeature itemFeature in itemFeatures)
		{
			if (category.Equals(itemFeature.category))
			{
				return itemFeature;
			}
		}
		return null;
	}

	public int GetIndexByCategory(string category)
	{
		for (int i = 0; i < itemFeatures.Count; i++)
		{
			if (category.Equals(itemFeatures[i].category))
			{
				return i;
			}
		}
		return -1;
	}

	public ItemFeature GetWeightItemFeature()
	{
		return FindItemFeatureByCategory("weight");
	}

	public void OnAddedToClientArea()
	{
		if (PlayerStore.Instance.currentClientInstance == null)
		{
			return;
		}
		StoreClient clientBlueprint = PlayerStore.Instance.currentClientInstance.GetClientBlueprint();
		StoreClient.ClientIntent clientIntent = clientBlueprint.clientIntent;
		if (!GeneralHelper.IsItemOwned(this) && (clientIntent == StoreClient.ClientIntent.SELL || clientIntent == StoreClient.ClientIntent.SELLNBUY))
		{
			if (HealthData.HasAnyAddiction() && PlayerStore.Instance.healthData.GetTotalAddictionPenalty() != 0)
			{
				AddItemFeature(ItemFeatureList.AddictionBuyPenalty());
			}
			if (StartingPerk.IsAwfulNegotiator())
			{
				AddItemFeature(ItemFeatureList.AwfulNegotiatorBuyPenalty());
			}
			if (clientBlueprint.perfumeInfluence == 1)
			{
				AddItemFeature(ItemFeatureList.NeuroActivePerfumeBuying());
			}
			else if (clientBlueprint.perfumeInfluence == 2)
			{
				AddItemFeature(ItemFeatureList.PheromonePerfumeBuying());
			}
			if (clientBlueprint.buyPriceModifier != 0)
			{
				AddItemFeature(ItemFeatureList.Discount(clientBlueprint.buyPriceModifier));
			}
			StolenHelper.ApplyStolenDiscount(this);
		}
		if (clientBlueprint.onItemAddedToClient != null)
		{
			clientBlueprint.onItemAddedToClient(this);
		}
		if (clientBlueprint.onItemAddedToCounters != null)
		{
			clientBlueprint.onItemAddedToCounters(this);
		}
	}

	public void OnAddedToShowcaseArea()
	{
		if (PlayerStore.Instance.currentClientInstance != null)
		{
			StoreClient clientBlueprint = PlayerStore.Instance.currentClientInstance.GetClientBlueprint();
			if (clientBlueprint.OnItemAddedToShowcase != null)
			{
				clientBlueprint.OnItemAddedToShowcase(this);
			}
		}
	}

	public void OnAddedToWeightedArea(bool ignoreOnItemAddedToWeighted = false)
	{
		if (PlayerStore.Instance.currentClientInstance != null)
		{
			StoreClient clientBlueprint = PlayerStore.Instance.currentClientInstance.GetClientBlueprint();
			StoreClient.ClientIntent clientIntent = clientBlueprint.clientIntent;
			if (GeneralHelper.IsItemOwned(this) && (clientIntent == StoreClient.ClientIntent.BUY || clientIntent == StoreClient.ClientIntent.SELLNBUY))
			{
				if (GeneralHelper.IsItemOwned(this) && (clientIntent == StoreClient.ClientIntent.BUY || clientIntent == StoreClient.ClientIntent.SELLNBUY))
				{
					if (PlayerStore.Instance.IsCurrentClientAcceptMarkup())
					{
						AddItemFeature(ItemFeatureList.RetailMarkUp());
					}
					if (clientBlueprint.perfumeInfluence == 1)
					{
						AddItemFeature(ItemFeatureList.NeuroActivePerfumeSelling());
					}
					else if (clientBlueprint.perfumeInfluence == 2)
					{
						AddItemFeature(ItemFeatureList.PheromonePerfumeSelling());
					}
					if (HealthData.HasAnyAddiction() && PlayerStore.Instance.healthData.GetTotalAddictionPenalty() != 0)
					{
						AddItemFeature(ItemFeatureList.AddictionSellPenalty());
					}
					if (StartingPerk.IsAwfulNegotiator())
					{
						AddItemFeature(ItemFeatureList.AwfulNegotiatorSellPenalty());
					}
					if (clientBlueprint.clientFaction == "FACTION_UPPER_LEVEL" && StoreReputation.IsPerkUnlocked("UL_TIPS"))
					{
						AddItemFeature(ItemFeatureList.Pourboire());
					}
				}
				ContrabandHelper.ApplyContrabandFeature(this);
				ApplyContrabandFeatureToChildren();
				PermitHelper.ApplyRestrictedFeature(this);
			}
			if (!GeneralHelper.IsItemOwned(this) && (clientIntent == StoreClient.ClientIntent.SELL || clientIntent == StoreClient.ClientIntent.SELLNBUY))
			{
				if (HealthData.HasAnyAddiction() && PlayerStore.Instance.healthData.GetTotalAddictionPenalty() != 0)
				{
					AddItemFeature(ItemFeatureList.AddictionBuyPenalty());
				}
				if (StartingPerk.IsAwfulNegotiator())
				{
					AddItemFeature(ItemFeatureList.AwfulNegotiatorBuyPenalty());
				}
				if (clientBlueprint.perfumeInfluence == 1)
				{
					AddItemFeature(ItemFeatureList.NeuroActivePerfumeBuying());
				}
				else if (clientBlueprint.perfumeInfluence == 2)
				{
					AddItemFeature(ItemFeatureList.PheromonePerfumeBuying());
				}
				if (clientBlueprint.buyPriceModifier != 0)
				{
					AddItemFeature(ItemFeatureList.Discount(clientBlueprint.buyPriceModifier));
				}
				StolenHelper.ApplyStolenDiscount(this);
			}
			if (clientBlueprint.onItemAddedToWeighted != null && !ignoreOnItemAddedToWeighted)
			{
				clientBlueprint.onItemAddedToWeighted(this);
			}
			if (clientBlueprint.onItemAddedToCounters != null)
			{
				clientBlueprint.onItemAddedToCounters(this);
			}
		}
		AddFeatureRelatedToEvent();
		AddFeatureRelatedToBuyer();
	}

	public void OnRemovedFromWeightedArea()
	{
		if (itemFeatures.Count != 0 && IsTag("IS_OWNED_TAG"))
		{
			for (int num = itemFeatures.Count - 1; num >= 0; num--)
			{
				if (itemFeatures[num].featureType == ItemFeature.FeatureType.Temporary)
				{
					itemFeatures.RemoveAt(num);
				}
				else if (itemFeatures[num].featureType == ItemFeature.FeatureType.TemporarySelling)
				{
					itemFeatures.RemoveAt(num);
				}
				else if (itemFeatures[num].identifier.Equals("retailMarkUp"))
				{
					itemFeatures.RemoveAt(num);
				}
				else if (itemFeatures[num].identifier.Equals("discountedBuy") || itemFeatures[num].identifier.Equals("discountedBuy20") || itemFeatures[num].identifier.Equals("merchantDiscountBuy"))
				{
					itemFeatures.RemoveAt(num);
				}
				else if (itemFeatures[num].category.Equals("contrabandMarkUp"))
				{
					itemFeatures.RemoveAt(num);
				}
			}
			RemoveContrabandFeatureFromChildren();
		}
		if (PlayerStore.Instance.currentClientInstance != null)
		{
			StoreClient clientBlueprint = PlayerStore.Instance.currentClientInstance.GetClientBlueprint();
			StoreClient.ClientIntent clientIntent = clientBlueprint.clientIntent;
			if (GeneralHelper.IsItemOwned(this) && clientIntent != StoreClient.ClientIntent.BUY)
			{
				_ = 3;
			}
			if (clientBlueprint.onItemRemovedToWeighted != null)
			{
				clientBlueprint.onItemRemovedToWeighted(this);
			}
			if (clientBlueprint.onItemRemovedToCounters != null)
			{
				clientBlueprint.onItemRemovedToCounters(this);
			}
		}
		RemoveFeatureRelatedToEvent();
		RemoveFeatureRelatedToBuyer();
	}

	public void AddFeatureRelatedToBuyer()
	{
	}

	public void RemoveFeatureRelatedToBuyer()
	{
	}

	public void RemoveFeatureRelatedToEvent()
	{
		for (int num = itemFeatures.Count - 1; num >= 0; num--)
		{
			if (itemFeatures[num].featureType == ItemFeature.FeatureType.Event)
			{
				itemFeatures.RemoveAt(num);
			}
		}
	}

	public void RemoveFeatureTemporaryBuying()
	{
		for (int num = itemFeatures.Count - 1; num >= 0; num--)
		{
			if (itemFeatures[num].featureType == ItemFeature.FeatureType.TemporaryBuying)
			{
				itemFeatures.RemoveAt(num);
			}
		}
	}

	public void RemoveFeatureTemporarySelling()
	{
		for (int num = itemFeatures.Count - 1; num >= 0; num--)
		{
			if (itemFeatures[num].featureType == ItemFeature.FeatureType.TemporarySelling)
			{
				itemFeatures.RemoveAt(num);
			}
		}
	}

	public void RemoveFeatureByCategory(string category)
	{
		for (int num = itemFeatures.Count - 1; num >= 0; num--)
		{
			if (itemFeatures[num].category.Equals(category))
			{
				itemFeatures.RemoveAt(num);
			}
		}
	}

	public void AddFeatureRelatedToEvent()
	{
		foreach (StoreEvent activeEvent in StoreStation.Instance.storeEventManager.GetActiveEvents())
		{
			if (activeEvent.IsItemAffectedByThisEvent(this))
			{
				ItemFeature.CreateAndAddFeatureFromEvent(this, activeEvent);
			}
		}
	}

	public void SetGameItemType(string gameItemType)
	{
		EnableTag(gameItemType);
		if (!itemTypes.Contains(gameItemType))
		{
			itemTypes.Add(gameItemType);
		}
	}

	public bool IsGameItemType(string gameItemType)
	{
		return itemTypes.Contains(gameItemType);
	}

	public void RemoveGameItemType(string gameItemType)
	{
		DisableTag(gameItemType);
		if (itemTypes.Contains(gameItemType))
		{
			itemTypes.Remove(gameItemType);
		}
	}

	public void RemoveAllGameItemType()
	{
		foreach (string item in itemTypes.ToList())
		{
			RemoveGameItemType(item);
		}
	}

	public void TransferGameItemTypeFrom(GameItem sourceItem, bool overwrite = true)
	{
		if (overwrite)
		{
			RemoveAllGameItemType();
		}
		foreach (string item in sourceItem.GetGameItemType())
		{
			SetGameItemType(item);
		}
	}

	public List<string> GetGameItemType()
	{
		return itemTypes;
	}

	public bool HaveType(string gameItemType)
	{
		return itemTypes.Contains(gameItemType);
	}

	public void AddItemFeature(ItemFeature itemFeature)
	{
		if (itemFeatures.Any((ItemFeature feature) => feature.identifier.Equals(itemFeature.identifier)))
		{
			return;
		}
		itemFeatures.Add(itemFeature);
		if (itemFeature.IsExposed())
		{
			if (itemFeature.realCondition != null && itemFeature.realCondition.HasOnActivateAction())
			{
				itemFeature.realCondition.ExecuteOnActivateAction(this);
			}
		}
		else if (itemFeature.fakeCondition != null && itemFeature.fakeCondition.HasOnActivateAction())
		{
			itemFeature.fakeCondition.ExecuteOnActivateAction(this);
		}
		itemFeature.parentItemUniqueId = GetUniqueID();
	}

	public void RemoveItemFeature(ItemFeature itemFeature)
	{
		itemFeatures.Remove(itemFeature);
		itemFeature.parentItemUniqueId = -1;
	}

	public void RemoveTemporaryClientFeature()
	{
		itemFeatures.RemoveAll((ItemFeature feature) => feature.featureType == ItemFeature.FeatureType.ClientTemporary);
	}

	public void CloseContentWindow(bool includeChildren = false)
	{
		if (contentWindow != null)
		{
			contentWindow.Hide();
		}
		if (!includeChildren)
		{
			return;
		}
		foreach (GameItem item in GraphUtils.FindAllChildrenType<GameItem>(this))
		{
			if (item.contentWindow != null)
			{
				item.contentWindow.Hide();
			}
		}
	}

	public void RememberContentWindowPositionIfMine(PixelWindow window)
	{
		if (PlayerStore.Instance != null && window != null && window == contentWindow && window.IsVisible())
		{
			PlayerStore instance = PlayerStore.Instance;
			if (instance.contentWindowPositions == null)
			{
				instance.contentWindowPositions = new Dictionary<int, Vector2>();
			}
			PlayerStore.Instance.contentWindowPositions[GetUniqueID()] = window.rectTransform.anchoredPosition;
			RememberContentTabs();
		}
	}

	private List<MachineryTabs> FindContentTabs()
	{
		List<MachineryTabs> list = new List<MachineryTabs>();
		if (contentWindow?.handler == null)
		{
			return list;
		}
		TreeNodeRender[] componentsInChildren = contentWindow.handler.GetComponentsInChildren<TreeNodeRender>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i].element is MachineryTabs item && !list.Contains(item))
			{
				list.Add(item);
			}
		}
		return list;
	}

	public string GetContentTabsSelection()
	{
		List<MachineryTabs> list = FindContentTabs();
		if (list.Count != 0)
		{
			return string.Join(",", list.Select((MachineryTabs t) => t.selected.ToString()));
		}
		return "";
	}

	public void ApplyContentTabsSelection(string selection)
	{
		if (string.IsNullOrEmpty(selection))
		{
			return;
		}
		string[] array = selection.Split(',');
		List<MachineryTabs> list = FindContentTabs();
		for (int i = 0; i < list.Count && i < array.Length; i++)
		{
			if (int.TryParse(array[i], out var result) && result >= 0 && result < list[i].count)
			{
				list[i].Select(result);
			}
		}
	}

	private void RememberContentTabs()
	{
		string selection = GetContentTabsSelection();
		if (selection.Length != 0)
		{
			ModifyTag("CONTENT_TABS_STRING", delegate(TagState state)
			{
				state.Enable().SetString(selection);
			});
		}
	}

	public void RestoreContentTabsIfMine(PixelWindow window)
	{
		if (window != null && window == contentWindow && IsTag("CONTENT_TABS_STRING"))
		{
			ApplyContentTabsSelection(GetTagReadonly("CONTENT_TABS_STRING").GetString());
		}
	}

	public void RestoreContentWindowPosition()
	{
		if (contentWindow != null && PlayerStore.Instance?.contentWindowPositions != null && PlayerStore.Instance.contentWindowPositions.TryGetValue(GetUniqueID(), out var value))
		{
			contentWindow.rectTransform.anchoredPosition = value;
			contentWindow.FixBounds();
		}
	}

	public static GraphNodeStorage GetRootNode(GraphNodeStorage node)
	{
		if (node.parents != null && node.parents.Count != 0)
		{
			return GetRootNode(node.parents[0]);
		}
		return node;
	}

	public bool IsRootedInBackInv()
	{
		if (GetParentWithIdentifier("backInv") != null)
		{
			return true;
		}
		return false;
	}

	public bool IsRootedInSmugglerBay()
	{
		if (GetParentWithIdentifier("smuggler_bay") != null)
		{
			return true;
		}
		return false;
	}

	public GameInventory GetParentWithIdentifier(string targetIdentifier)
	{
		return FindParentWithIdentifier(parentInventory, targetIdentifier);
	}

	public static GameInventory FindParentWithIdentifier(GraphNodeStorage node, string targetIdentifier)
	{
		if (node == null || node.parents == null || node.parents.Count == 0)
		{
			return null;
		}
		if (node is GameInventory gameInventory && gameInventory.identifier == targetIdentifier)
		{
			return gameInventory;
		}
		foreach (GraphNodeStorage parent in node.parents)
		{
			if (parent is GameInventory gameInventory2 && gameInventory2.identifier == targetIdentifier)
			{
				return gameInventory2;
			}
			GameInventory gameInventory3 = FindParentWithIdentifier(parent, targetIdentifier);
			if (gameInventory3 != null)
			{
				return gameInventory3;
			}
		}
		return null;
	}

	public int GetUniqueID()
	{
		if (uniqueId == 0)
		{
			uniqueId = PlayerStore.Instance.GetUniqueID();
		}
		return uniqueId;
	}

	public void RemoveAllFeature()
	{
		foreach (ItemFeature item in itemFeatures.ToList())
		{
			itemFeatures.Remove(item);
		}
	}

	public bool IsConditionActive(string conditionID)
	{
		foreach (ItemFeature itemFeature in itemFeatures)
		{
			if (itemFeature.IsExposed() && itemFeature.realCondition != null && itemFeature.realCondition.identifier.Equals(conditionID))
			{
				return true;
			}
			if (!itemFeature.IsExposed() && itemFeature.fakeCondition != null && itemFeature.fakeCondition.identifier.Equals(conditionID))
			{
				return true;
			}
		}
		return false;
	}

	public bool IsID(string id)
	{
		return identifier.Equals(id);
	}

	public int GetItemSize()
	{
		int num = 0;
		for (int i = 0; i < shape.height; i++)
		{
			for (int j = 0; j < shape.width; j++)
			{
				if (shape.GetLocal(j, i) == 1)
				{
					num++;
				}
			}
		}
		return num;
	}

	public int GetModifiedItemSize()
	{
		int num = 0;
		for (int i = 0; i < modifiedShape.height; i++)
		{
			for (int j = 0; j < modifiedShape.width; j++)
			{
				if (modifiedShape.GetLocal(j, i) == 1)
				{
					num++;
				}
			}
		}
		return num;
	}
}
