using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GameItemElement : GameItem, PixelElement, TooltipListener
{
	private readonly TreeNodeRender stackCountText;

	private readonly TreeNodeRender stackCountTextBackground;

	private readonly List<TreeNodeRender> pinOverlayNodes = new List<TreeNodeRender>();

	private readonly TreeNodeRender fakeStackCountText;

	private readonly TreeNodeRender fakeStackCountTextBackground;

	private readonly List<TreeNodeRender> fakePinOverlayNodes = new List<TreeNodeRender>();

	private bool fakePinOverlayVisible;

	private TreeNodeRender markOverlayNode;

	private TreeNodeRender fakeMarkOverlayNode;

	private bool markHiddenWithBackground;

	private TreeNodeRender[] playingFrameNodes;

	private TreeNodeRender[] fakePlayingFrameNodes;

	private readonly List<TreeNodeRender> spriteMods = new List<TreeNodeRender>();

	private readonly List<TreeNodeRender> fakeSpriteMods = new List<TreeNodeRender>();

	private readonly List<TreeNodeRender> innateSpriteMods = new List<TreeNodeRender>();

	private readonly List<TreeNodeRender> innateFakeSpriteMods = new List<TreeNodeRender>();

	private int appliedMinX = int.MinValue;

	private int appliedMinY = int.MinValue;

	private int appliedOrientation = -1;

	private int appliedGridSize = -1;

	private bool appliedFlipped;

	private bool validateLock;

	private static readonly List<ItemSpriteModifier> noSpriteModifiers = new List<ItemSpriteModifier>();

	private static readonly List<TreeNodeRender> innerNodeBuffer = new List<TreeNodeRender>();

	private bool destroyed;

	public bool canResize => false;

	public int widthPixels => UISettings.current.gridSize * base.modifiedShape.width;

	public int heightPixels => UISettings.current.gridSize * base.modifiedShape.height;

	public TreeNodeRender handler { get; private set; }

	public RectTransform rectTransform => middle.rectTransform;

	public TreeNodeRender inner { get; private set; }

	public TreeNodeRender fakeInner { get; private set; }

	public TreeNodeRender middle { get; private set; }

	public TreeNodeRender rotator { get; private set; }

	public TreeNodeRender background { get; private set; }

	public TreeNodeRender fakeMiddle { get; private set; }

	public TreeNodeRender fakeRotator { get; private set; }

	public TreeNodeRender fakeBackground { get; private set; }

	public GridShapeBuilder tempShape { get; private set; }

	public GameItemElement(string identifier, string spriteAtlasPath, string spritePath, PixelWindow window = null)
		: base(identifier, window)
	{
		Sprite sprite = RenderHandler.LoadFromAtlas(spriteAtlasPath, spritePath);
		handler = TreeNodeRender.Instantiate(this, "Item " + spritePath);
		middle = TreeNodeRender.Instantiate().SetParent(handler);
		rotator = TreeNodeRender.Instantiate().SetParent(middle);
		background = TreeNodeRender.Instantiate().SetParent(rotator);
		stackCountTextBackground = TreeNodeRender.InstantiateText().SetParent(middle);
		stackCountText = TreeNodeRender.InstantiateText().SetParent(middle);
		stackCountText.rectTransform.sizeDelta = new Vector2(1000f, 16f);
		stackCountText.text.color = RenderHandler.current.textColorRed;
		stackCountText.text.fontStyle = FontStyles.Normal;
		stackCountTextBackground.rectTransform.sizeDelta = new Vector2(1000f, 16f);
		stackCountTextBackground.rectTransform.anchoredPosition = new Vector2(0.5f, -1f);
		stackCountTextBackground.text.color = RenderHandler.current.textColorDarkRed;
		stackCountTextBackground.text.fontStyle = FontStyles.Bold;
		background.image.sprite = sprite;
		background.image.SetNativeSize();
		background.image.alphaHitTestMinimumThreshold = 1E-05f;
		background.InitEventHandler();
		background.image.enabled = true;
		handler.InitEventHandler();
		handler.image.raycastTarget = false;
		fakeMiddle = TreeNodeRender.Instantiate().SetParent(handler);
		fakeRotator = TreeNodeRender.Instantiate().SetParent(fakeMiddle);
		fakeBackground = TreeNodeRender.Instantiate().SetParent(fakeRotator);
		fakeStackCountTextBackground = TreeNodeRender.InstantiateText().SetParent(fakeMiddle);
		fakeStackCountText = TreeNodeRender.InstantiateText().SetParent(fakeMiddle);
		fakeStackCountText.rectTransform.sizeDelta = stackCountText.rectTransform.sizeDelta;
		fakeStackCountText.text.color = stackCountText.text.color;
		fakeStackCountText.text.fontStyle = stackCountText.text.fontStyle;
		fakeStackCountTextBackground.rectTransform.sizeDelta = stackCountTextBackground.rectTransform.sizeDelta;
		fakeStackCountTextBackground.rectTransform.anchoredPosition = stackCountTextBackground.rectTransform.anchoredPosition;
		fakeStackCountTextBackground.text.color = stackCountTextBackground.text.color;
		fakeStackCountTextBackground.text.fontStyle = stackCountTextBackground.text.fontStyle;
		fakeBackground.image.sprite = background.image.sprite;
		fakeBackground.image.SetNativeSize();
		fakeBackground.InitEventHandler();
		inner = TreeNodeRender.Instantiate().SetParent(rotator);
		fakeInner = TreeNodeRender.Instantiate().SetParent(fakeRotator);
		tempShape = new GridShapeBuilder(base.shape);
		SetSpriteAndShape(spriteAtlasPath, spritePath);
		Validate();
	}

	public int WidthPixelsClamp(int width)
	{
		return widthPixels;
	}

	public int HeightPixelsClamp(int height)
	{
		return heightPixels;
	}

	public void ResizePixels(int width, int height)
	{
	}

	public int XClamp(int x)
	{
		return x;
	}

	public int YClamp(int y)
	{
		return y;
	}

	public void UpdateRectTransform()
	{
		int gridSize = UISettings.current.gridSize;
		if (appliedMinX != base.modifiedShape.minX || appliedMinY != base.modifiedShape.minY || appliedOrientation != base.modifiedShape.orientation || appliedFlipped != base.modifiedShape.flipped || appliedGridSize != gridSize)
		{
			appliedMinX = base.modifiedShape.minX;
			appliedMinY = base.modifiedShape.minY;
			appliedOrientation = base.modifiedShape.orientation;
			appliedFlipped = base.modifiedShape.flipped;
			appliedGridSize = gridSize;
			ApplyRectTransform(gridSize);
		}
		fakeRotator.rectTransform.localScale = rotator.rectTransform.localScale;
		fakeRotator.rectTransform.anchorMin = rotator.rectTransform.anchorMin;
		fakeRotator.rectTransform.anchorMax = rotator.rectTransform.anchorMax;
		fakeRotator.rectTransform.pivot = rotator.rectTransform.pivot;
		fakeRotator.rectTransform.localEulerAngles = rotator.rectTransform.localEulerAngles;
		fakeMiddle.rectTransform.anchoredPosition = middle.rectTransform.anchoredPosition;
	}

	private void ApplyRectTransform(int gridSize)
	{
		rotator.rectTransform.localScale = new Vector3((!base.modifiedShape.flipped) ? 1 : (-1), 1f, 1f);
		rotator.rectTransform.anchorMin = new Vector2(0f, 1f);
		rotator.rectTransform.anchorMax = new Vector2(0f, 1f);
		float num = (base.modifiedShape.flipped ? 1 : 0);
		switch (base.modifiedShape.orientation)
		{
		case 0:
			rotator.rectTransform.pivot = new Vector2(num, 1f);
			break;
		case 1:
			rotator.rectTransform.pivot = new Vector2(1f - num, 1f);
			break;
		case 2:
			rotator.rectTransform.pivot = new Vector2(1f - num, 0f);
			break;
		default:
			rotator.rectTransform.pivot = new Vector2(num, 0f);
			break;
		}
		rotator.rectTransform.localEulerAngles = 90 * base.modifiedShape.orientation * Vector3.forward;
		middle.rectTransform.anchoredPosition = new Vector2(base.modifiedShape.minX * gridSize, -base.modifiedShape.minY * gridSize);
	}

	public override void Validate()
	{
		ValidateSelect();
	}

	private static List<ItemSpriteModifier> SortedByPriority(List<ItemSpriteModifier> mods)
	{
		if (mods == null)
		{
			return noSpriteModifiers;
		}
		if (mods.Count < 2)
		{
			return mods;
		}
		return mods.OrderBy((ItemSpriteModifier a) => a.priority).ToList();
	}

	public Sprite ResolveSpriteByName(string name)
	{
		if (base.useSpriteFromMod)
		{
			LoadedMod loadedMod = ModLoader.Mods.Find((LoadedMod m) => m.Manifest.ID == base.spriteAtlasPath);
			if (loadedMod != null && loadedMod.Sprites.TryGetValue(name, out var value))
			{
				return value;
			}
			Debug.LogWarning("Mod sprite '" + name + "' from mod '" + base.spriteAtlasPath + "' not found. Loading dummy sprite.");
			return RenderHandler.unknownSprite;
		}
		return RenderHandler.LoadFromAtlas(base.spriteAtlasPath, name);
	}

	private Sprite ResolveItemSprite()
	{
		return ResolveSpriteByName(base.spritePath);
	}

	public void ValidatePartialSprite()
	{
		Sprite sprite = ResolveItemSprite();
		background.image.sprite = sprite;
		background.image.SetNativeSize();
		fakeBackground.image.sprite = background.image.sprite;
		fakeBackground.image.SetNativeSize();
	}

	public void ApplyAnimationFrame(Sprite frame)
	{
		if (!(frame == null))
		{
			background.image.sprite = frame;
			fakeBackground.image.sprite = frame;
		}
	}

	public void HandleModSprite()
	{
		Debug.Log("Handling mod sprite");
		int num = 0;
		foreach (ItemSpriteModifier item in SortedByPriority(base.getItemSpriteModifierForItemFunc?.Invoke(this)))
		{
			UpdateModSprite(num, item.sprite, item.position, item.orientation, item.isFlipped, item);
			num++;
		}
	}

	public void ValidateSelect(bool updateBackground = true)
	{
		if (validateLock)
		{
			return;
		}
		validateLock = true;
		try
		{
			ValidateSelectLocked(updateBackground);
		}
		finally
		{
			validateLock = false;
		}
	}

	private void ValidateSelectLocked(bool updateBackground)
	{
		if (base.spriteChanged)
		{
			Sprite sprite = ResolveItemSprite();
			background.image.sprite = sprite;
			background.image.SetNativeSize();
			fakeBackground.image.sprite = background.image.sprite;
			fakeBackground.image.SetNativeSize();
			base.spriteChanged = false;
		}
		if (updateBackground)
		{
			ToggleBackground(base.unitCount > 0);
			ToggleFakeBackground(enabled: false, UISettings.current.dragOpacity);
		}
		int gridSize = UISettings.current.gridSize;
		Vector2 sizeDelta = new Vector2(base.modifiedShape.width * gridSize, base.modifiedShape.height * gridSize);
		background.rectTransform.anchoredPosition = new Vector2(base.modifiedXOrigin * gridSize, base.modifiedYOrigin * gridSize);
		fakeBackground.rectTransform.anchoredPosition = new Vector2(base.modifiedXOrigin * gridSize, base.modifiedYOrigin * gridSize);
		middle.rectTransform.sizeDelta = sizeDelta;
		rotator.rectTransform.sizeDelta = sizeDelta;
		handler.rectTransform.anchoredPosition = Vector2.zero;
		fakeMiddle.rectTransform.sizeDelta = sizeDelta;
		fakeRotator.rectTransform.sizeDelta = sizeDelta;
		if (base.parentInventory != null)
		{
			UpdateRectTransform();
		}
		string text = base.customText;
		if (text == null)
		{
			text = "";
		}
		if (text.Length < 1 && base.unitCount > 1)
		{
			text = base.unitCount.ToString() ?? "";
		}
		stackCountText.text.text = text;
		stackCountText.text.enabled = text.Length > 0 && !base.isDebugMenu;
		stackCountTextBackground.text.text = stackCountText.text.text;
		stackCountTextBackground.text.enabled = text.Length > 0 && !base.isDebugMenu;
		fakeStackCountText.text.text = stackCountText.text.text;
		fakeStackCountText.text.enabled = false;
		fakeStackCountTextBackground.text.text = stackCountTextBackground.text.text;
		fakeStackCountTextBackground.text.enabled = false;
		UpdatePinOverlay();
		UpdateMarkOverlay();
		UpdatePlayingFrameOverlay();
		int num = 0;
		List<GameInventory> list = GraphUtils.FindChildrenType<GameInventory>(this);
		new List<GridShape>();
		foreach (GameInventory item in list)
		{
			foreach (GameItem childItem in item.childItems)
			{
				foreach (ItemSpriteModifier item2 in SortedByPriority(childItem.parentItemSpriteModifierForItemFunc?.Invoke(childItem, item, item.GetIncompleteItemInventorySlot(childItem), childItem.modifiedShape, childItem.modifiedState, this)))
				{
					UpdateModSprite(num, item2.sprite, item2.position, item2.orientation, item2.isFlipped, item2);
					num++;
				}
				foreach (ItemSpriteModifier item3 in SortedByPriority(item.parentItemSpriteModifierForItemFunc?.Invoke(childItem, item, item.GetIncompleteItemInventorySlot(childItem), childItem.modifiedShape, childItem.modifiedState, this)))
				{
					UpdateModSprite(num, item3.sprite, item3.position, item3.orientation, item3.isFlipped, item3);
					num++;
				}
			}
		}
		for (int i = num; i < spriteMods.Count; i++)
		{
			spriteMods[i].gameObject.SetActive(value: false);
			fakeSpriteMods[i].gameObject.SetActive(value: false);
		}
		if (num > 0 || base.parentInventory?.parentItemSpriteModifierForItemFunc != null || base.parentItemSpriteModifierForItemFunc != null)
		{
			GraphUtils.FindParentsType<GameItem>(base.parentInventory)?.ForEach(delegate(GameItem n)
			{
				n.Validate();
			});
		}
		HandleInnateModSprite();
		SpriteAnimationHelper.SyncSpriteAnimation(this);
	}

	public IEnumerable<TreeNodeRender> GetActiveInnateSpriteMods()
	{
		return innateSpriteMods.Where((TreeNodeRender m) => m != null && m.gameObject.activeSelf);
	}

	public void HandleInnateModSprite()
	{
		int num = 0;
		foreach (ItemSpriteModifier item in SortedByPriority(base.getItemSpriteModifierForItemFunc?.Invoke(this)))
		{
			UpdateInnateModSprite(num, item.sprite, item.position, item.orientation, item.isFlipped, item);
			num++;
		}
		for (int i = num; i < innateSpriteMods.Count; i++)
		{
			innateSpriteMods[i].gameObject.SetActive(value: false);
			innateFakeSpriteMods[i].gameObject.SetActive(value: false);
		}
	}

	private void UpdateModSprite(int i, Sprite modSprite, Vector2 modPosition, int modOrientation, bool modFlip, ItemSpriteModifier mod)
	{
		while (i >= spriteMods.Count)
		{
			TreeNodeRender treeNodeRender = TreeNodeRender.Instantiate().SetParent(rotator);
			treeNodeRender.eventHandler = background.eventHandler;
			treeNodeRender.image.raycastTarget = true;
			treeNodeRender.image.alphaHitTestMinimumThreshold = 1E-05f;
			TreeNodeRender treeNodeRender2 = TreeNodeRender.Instantiate().SetParent(fakeRotator);
			spriteMods.Add(treeNodeRender);
			fakeSpriteMods.Add(treeNodeRender2);
			treeNodeRender.gameObject.SetActive(value: false);
			treeNodeRender2.gameObject.SetActive(value: false);
		}
		spriteMods[i].image.sprite = modSprite;
		spriteMods[i].image.SetNativeSize();
		RectTransform rectTransform = spriteMods[i].rectTransform;
		rectTransform.localScale = new Vector3((!modFlip) ? 1 : (-1), 1f, 1f);
		rectTransform.anchorMin = new Vector2(0f, 1f);
		rectTransform.anchorMax = new Vector2(0f, 1f);
		float num = (modFlip ? 1 : 0);
		modOrientation = (modOrientation % 4 + 4) % 4;
		switch (modOrientation)
		{
		case 0:
			rectTransform.pivot = new Vector2(num, 1f);
			break;
		case 1:
			rectTransform.pivot = new Vector2(1f - num, 1f);
			break;
		case 2:
			rectTransform.pivot = new Vector2(1f - num, 0f);
			break;
		default:
			rectTransform.pivot = new Vector2(num, 0f);
			break;
		}
		rectTransform.localEulerAngles = 90 * modOrientation * Vector3.forward;
		rectTransform.anchoredPosition = modPosition + background.rectTransform.anchoredPosition;
		spriteMods[i].image.enabled = true;
		spriteMods[i].gameObject.SetActive(value: true);
		fakeSpriteMods[i].image.sprite = modSprite;
		fakeSpriteMods[i].image.SetNativeSize();
		fakeSpriteMods[i].image.enabled = false;
		fakeSpriteMods[i].rectTransform.localScale = rectTransform.localScale;
		fakeSpriteMods[i].rectTransform.anchorMin = rectTransform.anchorMin;
		fakeSpriteMods[i].rectTransform.anchorMax = rectTransform.anchorMax;
		fakeSpriteMods[i].rectTransform.pivot = rectTransform.pivot;
		fakeSpriteMods[i].rectTransform.localEulerAngles = rectTransform.localEulerAngles;
		fakeSpriteMods[i].rectTransform.anchoredPosition = modPosition + background.rectTransform.anchoredPosition;
		fakeSpriteMods[i].gameObject.SetActive(value: true);
	}

	private void UpdateInnateModSprite(int i, Sprite modSprite, Vector2 modPosition, int modOrientation, bool modFlip, ItemSpriteModifier mod)
	{
		while (i >= innateSpriteMods.Count)
		{
			TreeNodeRender treeNodeRender = TreeNodeRender.Instantiate().SetParent(rotator);
			treeNodeRender.eventHandler = background.eventHandler;
			treeNodeRender.image.raycastTarget = true;
			treeNodeRender.image.alphaHitTestMinimumThreshold = 1E-05f;
			TreeNodeRender treeNodeRender2 = TreeNodeRender.Instantiate().SetParent(fakeRotator);
			innateSpriteMods.Add(treeNodeRender);
			innateFakeSpriteMods.Add(treeNodeRender2);
			treeNodeRender.gameObject.SetActive(value: false);
			treeNodeRender2.gameObject.SetActive(value: false);
		}
		innateSpriteMods[i].image.color = mod.color;
		innateSpriteMods[i].image.sprite = modSprite;
		innateSpriteMods[i].image.SetNativeSize();
		RectTransform rectTransform = innateSpriteMods[i].rectTransform;
		rectTransform.localScale = new Vector3((!modFlip) ? 1 : (-1), 1f, 1f);
		rectTransform.anchorMin = new Vector2(0f, 1f);
		rectTransform.anchorMax = new Vector2(0f, 1f);
		float num = (modFlip ? 1 : 0);
		modOrientation = (modOrientation % 4 + 4) % 4;
		switch (modOrientation)
		{
		case 0:
			rectTransform.pivot = new Vector2(num, 1f);
			break;
		case 1:
			rectTransform.pivot = new Vector2(1f - num, 1f);
			break;
		case 2:
			rectTransform.pivot = new Vector2(1f - num, 0f);
			break;
		default:
			rectTransform.pivot = new Vector2(num, 0f);
			break;
		}
		rectTransform.localEulerAngles = 90 * modOrientation * Vector3.forward;
		rectTransform.anchoredPosition = modPosition + background.rectTransform.anchoredPosition;
		innateSpriteMods[i].image.enabled = true;
		innateSpriteMods[i].gameObject.SetActive(value: true);
		innateFakeSpriteMods[i].image.color = mod.color;
		innateFakeSpriteMods[i].image.sprite = modSprite;
		innateFakeSpriteMods[i].image.SetNativeSize();
		innateFakeSpriteMods[i].image.enabled = false;
		innateFakeSpriteMods[i].rectTransform.localScale = rectTransform.localScale;
		innateFakeSpriteMods[i].rectTransform.anchorMin = rectTransform.anchorMin;
		innateFakeSpriteMods[i].rectTransform.anchorMax = rectTransform.anchorMax;
		innateFakeSpriteMods[i].rectTransform.pivot = rectTransform.pivot;
		innateFakeSpriteMods[i].rectTransform.localEulerAngles = rectTransform.localEulerAngles;
		innateFakeSpriteMods[i].rectTransform.anchoredPosition = modPosition + background.rectTransform.anchoredPosition;
		innateFakeSpriteMods[i].gameObject.SetActive(value: true);
	}

	public void UpdateFakeReal(int numPickedUp)
	{
		int num = base.unitCount - numPickedUp;
		ToggleBackground(num > 0);
		stackCountText.text.text = num.ToString() ?? "";
		stackCountText.text.enabled = num > 1 && !base.isDebugMenu;
		stackCountTextBackground.text.text = stackCountText.text.text;
		stackCountTextBackground.text.enabled = num > 1 && !base.isDebugMenu;
		ToggleFakeBackground(numPickedUp > 0, UISettings.current.dragOpacity);
		fakeStackCountText.text.text = numPickedUp.ToString() ?? "";
		fakeStackCountText.text.enabled = numPickedUp > 1;
		fakeStackCountTextBackground.text.text = fakeStackCountText.text.text;
		fakeStackCountTextBackground.text.enabled = numPickedUp > 1;
	}

	private void UpdatePinOverlay()
	{
		if (base.isDebugMenu || !ItemPinHelper.IsPinned(this))
		{
			foreach (TreeNodeRender pinOverlayNode in pinOverlayNodes)
			{
				pinOverlayNode.image.enabled = false;
			}
			{
				foreach (TreeNodeRender fakePinOverlayNode in fakePinOverlayNodes)
				{
					fakePinOverlayNode.image.enabled = false;
				}
				return;
			}
		}
		int gridSize = UISettings.current.gridSize;
		int num = 0;
		for (int i = base.modifiedShape.minY; i <= base.modifiedShape.maxY; i++)
		{
			for (int j = base.modifiedShape.minX; j <= base.modifiedShape.maxX; j++)
			{
				if (base.modifiedShape.Get(j, i) != 0)
				{
					Vector2 position = new Vector2((j - base.modifiedShape.minX) * gridSize, -(i - base.modifiedShape.minY) * gridSize);
					PlacePinOverlayNode(pinOverlayNodes, num, middle, position, gridSize, visible: true);
					PlacePinOverlayNode(fakePinOverlayNodes, num, fakeMiddle, position, gridSize, fakePinOverlayVisible);
					num++;
				}
			}
		}
		for (int k = num; k < pinOverlayNodes.Count; k++)
		{
			pinOverlayNodes[k].image.enabled = false;
		}
		for (int l = num; l < fakePinOverlayNodes.Count; l++)
		{
			fakePinOverlayNodes[l].image.enabled = false;
		}
	}

	private static void PlacePinOverlayNode(List<TreeNodeRender> pool, int index, TreeNodeRender parent, Vector2 position, int gridSize, bool visible)
	{
		TreeNodeRender treeNodeRender;
		if (index < pool.Count)
		{
			treeNodeRender = pool[index];
		}
		else
		{
			treeNodeRender = TreeNodeRender.Instantiate().SetParent(parent);
			treeNodeRender.gameObject.name = "PinOverlay";
			treeNodeRender.rectTransform.anchorMin = new Vector2(0f, 1f);
			treeNodeRender.rectTransform.anchorMax = new Vector2(0f, 1f);
			treeNodeRender.rectTransform.pivot = new Vector2(0f, 1f);
			Sprite overlaySprite = ItemPinHelper.GetOverlaySprite();
			treeNodeRender.image.sprite = overlaySprite;
			treeNodeRender.image.color = ((overlaySprite != null) ? Color.white : ((Color)RenderHandler.GetColorFromPalette(RenderHandler.ColorPalette.LightOrange)));
			treeNodeRender.image.raycastTarget = false;
			pool.Add(treeNodeRender);
		}
		treeNodeRender.rectTransform.SetAsLastSibling();
		treeNodeRender.rectTransform.sizeDelta = new Vector2(gridSize, gridSize);
		treeNodeRender.rectTransform.anchoredPosition = position;
		treeNodeRender.image.enabled = visible;
	}

	private void UpdateMarkOverlay()
	{
		Color? color = (base.isDebugMenu ? null : AnimalMarkHelper.GetMarkColor(this));
		if (!color.HasValue)
		{
			if (markOverlayNode != null)
			{
				markOverlayNode.image.enabled = false;
			}
			if (fakeMarkOverlayNode != null)
			{
				fakeMarkOverlayNode.image.enabled = false;
			}
		}
		else
		{
			int px = Mathf.Max(1, UISettings.current.gridSize / 16);
			markOverlayNode = PlaceMarkOverlayNode(markOverlayNode, middle, color.Value, px, !markHiddenWithBackground);
			fakeMarkOverlayNode = PlaceMarkOverlayNode(fakeMarkOverlayNode, fakeMiddle, color.Value, px, fakePinOverlayVisible);
		}
	}

	private static TreeNodeRender PlaceMarkOverlayNode(TreeNodeRender node, TreeNodeRender parent, Color color, int px, bool visible)
	{
		if (node == null)
		{
			node = TreeNodeRender.Instantiate().SetParent(parent);
			node.gameObject.name = "MarkOverlay";
			node.rectTransform.anchorMin = new Vector2(0f, 1f);
			node.rectTransform.anchorMax = new Vector2(0f, 1f);
			node.rectTransform.pivot = new Vector2(0f, 1f);
			node.image.sprite = null;
			node.image.raycastTarget = false;
		}
		node.rectTransform.SetAsLastSibling();
		node.rectTransform.sizeDelta = new Vector2(5 * px, 5 * px);
		node.rectTransform.anchoredPosition = new Vector2(2 * px, -2 * px);
		node.image.color = color;
		node.image.enabled = visible;
		return node;
	}

	public void RefreshPlayingFrame()
	{
		UpdatePlayingFrameOverlay();
	}

	private void UpdatePlayingFrameOverlay()
	{
		Color? color = (base.isDebugMenu ? null : CassetteHelper.GetPlayingFrameColor(this));
		if (!color.HasValue)
		{
			SetFrameNodesEnabled(playingFrameNodes, enabled: false);
			SetFrameNodesEnabled(fakePlayingFrameNodes, enabled: false);
			return;
		}
		int num = Mathf.Max(1, UISettings.current.gridSize / 16);
		float width = 2 * num;
		playingFrameNodes = PlaceFrameNodes(playingFrameNodes, middle, color.Value, width, !markHiddenWithBackground);
		fakePlayingFrameNodes = PlaceFrameNodes(fakePlayingFrameNodes, fakeMiddle, color.Value, width, fakePinOverlayVisible);
	}

	private static void SetFrameNodesEnabled(TreeNodeRender[] nodes, bool enabled)
	{
		if (nodes != null)
		{
			for (int i = 0; i < nodes.Length; i++)
			{
				nodes[i].image.enabled = enabled;
			}
		}
	}

	private static TreeNodeRender[] PlaceFrameNodes(TreeNodeRender[] nodes, TreeNodeRender parent, Color color, float width, bool visible)
	{
		if (nodes == null)
		{
			nodes = new TreeNodeRender[4];
			for (int i = 0; i < nodes.Length; i++)
			{
				nodes[i] = TreeNodeRender.Instantiate().SetParent(parent);
				nodes[i].gameObject.name = "PlayingFrame";
				nodes[i].image.sprite = null;
				nodes[i].image.raycastTarget = false;
			}
		}
		PlaceFrameEdge(nodes[0], new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, width));
		PlaceFrameEdge(nodes[1], new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, width));
		PlaceFrameEdge(nodes[2], new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(0f, 0.5f), new Vector2(width, -2f * width));
		PlaceFrameEdge(nodes[3], new Vector2(1f, 0f), new Vector2(1f, 1f), new Vector2(1f, 0.5f), new Vector2(width, -2f * width));
		TreeNodeRender[] array = nodes;
		foreach (TreeNodeRender obj in array)
		{
			obj.rectTransform.SetAsLastSibling();
			obj.image.color = color;
			obj.image.enabled = visible;
		}
		return nodes;
	}

	private static void PlaceFrameEdge(TreeNodeRender node, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 sizeDelta)
	{
		node.rectTransform.anchorMin = anchorMin;
		node.rectTransform.anchorMax = anchorMax;
		node.rectTransform.pivot = pivot;
		node.rectTransform.sizeDelta = sizeDelta;
		node.rectTransform.anchoredPosition = Vector2.zero;
	}

	public void ToggleBackground(bool enabled, float opacity = 1f, float lightness = 1f)
	{
		inner.gameObject.GetComponentsInChildren(innerNodeBuffer);
		if (enabled)
		{
			Vector4 vector = new Vector4(lightness, lightness, lightness, opacity);
			background.image.color = vector;
			foreach (TreeNodeRender spriteMod in spriteMods)
			{
				spriteMod.image.color = vector;
			}
			foreach (TreeNodeRender innateSpriteMod in innateSpriteMods)
			{
				innateSpriteMod.image.color = vector;
			}
			foreach (TreeNodeRender item in innerNodeBuffer)
			{
				item.image.color = vector;
			}
		}
		background.image.enabled = enabled;
		foreach (TreeNodeRender spriteMod2 in spriteMods)
		{
			spriteMod2.image.enabled = enabled;
		}
		foreach (TreeNodeRender innateSpriteMod2 in innateSpriteMods)
		{
			innateSpriteMod2.image.enabled = enabled;
		}
		foreach (TreeNodeRender item2 in innerNodeBuffer)
		{
			item2.image.enabled = enabled;
		}
		markHiddenWithBackground = !enabled;
		UpdateMarkOverlay();
		UpdatePlayingFrameOverlay();
	}

	public void ToggleFakeBackground(bool enabled, Vector4 color)
	{
		if (enabled)
		{
			fakeMiddle.SetParent(RenderHandler.current.overlayRoot, worldPositionStays: true);
		}
		else
		{
			fakeMiddle.SetParent(handler, worldPositionStays: true);
		}
		fakePinOverlayVisible = enabled;
		UpdatePinOverlay();
		UpdateMarkOverlay();
		UpdatePlayingFrameOverlay();
		fakeInner.gameObject.GetComponentsInChildren(innerNodeBuffer);
		if (enabled)
		{
			fakeBackground.image.color = color;
			foreach (TreeNodeRender fakeSpriteMod in fakeSpriteMods)
			{
				fakeSpriteMod.image.color = color;
			}
			foreach (TreeNodeRender item in innerNodeBuffer)
			{
				item.image.color = color;
			}
		}
		fakeBackground.image.enabled = enabled;
		foreach (TreeNodeRender fakeSpriteMod2 in fakeSpriteMods)
		{
			fakeSpriteMod2.image.enabled = enabled;
		}
		foreach (TreeNodeRender innateFakeSpriteMod in innateFakeSpriteMods)
		{
			innateFakeSpriteMod.image.enabled = enabled;
		}
		foreach (TreeNodeRender item2 in innerNodeBuffer)
		{
			item2.image.enabled = enabled;
		}
	}

	public void ToggleFakeBackground(bool enabled, float opacity = 1f, float lightness = 1f)
	{
		Vector4 color = new Vector4(lightness, lightness, lightness, opacity);
		ToggleFakeBackground(enabled, color);
	}

	public bool Attach(PixelElement element)
	{
		throw new NotImplementedException();
	}

	public bool Detach(PixelElement element)
	{
		throw new NotImplementedException();
	}

	public void GridFlipX()
	{
		fakeRotator.rectTransform.pivot = new Vector2(1f - fakeRotator.rectTransform.pivot.x, fakeRotator.rectTransform.pivot.y);
		fakeRotator.rectTransform.localScale = new Vector3(0f - fakeRotator.rectTransform.localScale.x, 1f, 1f);
		tempShape.flipped = !tempShape.flipped;
	}

	public void GridFlipY()
	{
		fakeRotator.rectTransform.pivot = new Vector2(fakeRotator.rectTransform.pivot.x, 1f - fakeRotator.rectTransform.pivot.y);
		fakeRotator.rectTransform.localScale = new Vector3(0f - fakeRotator.rectTransform.localScale.x, 1f, 1f);
		fakeRotator.rectTransform.localEulerAngles += 180f * Vector3.forward;
		tempShape.flipped = !tempShape.flipped;
		tempShape.SetOrientation((tempShape.orientation + 2) % 4);
	}

	public void GridRotCCW()
	{
		if (tempShape.flipped)
		{
			fakeRotator.rectTransform.pivot = new Vector2(1f - fakeRotator.rectTransform.pivot.y, fakeRotator.rectTransform.pivot.x);
		}
		else
		{
			fakeRotator.rectTransform.pivot = new Vector2(fakeRotator.rectTransform.pivot.y, 1f - fakeRotator.rectTransform.pivot.x);
		}
		fakeRotator.rectTransform.localEulerAngles += 90f * Vector3.forward;
		tempShape.SetOrientation((tempShape.orientation + 1) % 4);
	}

	public void GridRotCW()
	{
		if (tempShape.flipped)
		{
			fakeRotator.rectTransform.pivot = new Vector2(fakeRotator.rectTransform.pivot.y, 1f - fakeRotator.rectTransform.pivot.x);
		}
		else
		{
			fakeRotator.rectTransform.pivot = new Vector2(1f - fakeRotator.rectTransform.pivot.y, fakeRotator.rectTransform.pivot.x);
		}
		fakeRotator.rectTransform.localEulerAngles -= 90f * Vector3.forward;
		tempShape.SetOrientation((tempShape.orientation + 3) % 4);
	}

	public void GridRot180()
	{
		fakeRotator.rectTransform.pivot = new Vector2(1f - fakeRotator.rectTransform.pivot.x, 1f - fakeRotator.rectTransform.pivot.y);
		fakeRotator.rectTransform.localEulerAngles += 180f * Vector3.forward;
		tempShape.SetOrientation((tempShape.orientation + 2) % 4);
	}

	public void FlipX()
	{
		var (x, y) = GetCenteredPosition();
		GridFlipX();
		SetCenteredPosition(x, y);
	}

	public void FlipY()
	{
		var (x, y) = GetCenteredPosition();
		GridFlipY();
		SetCenteredPosition(x, y);
	}

	public void RotCCW()
	{
		var (x, y) = GetCenteredPosition();
		GridRotCCW();
		SetCenteredPosition(x, y);
	}

	public void RotCW()
	{
		var (x, y) = GetCenteredPosition();
		GridRotCW();
		SetCenteredPosition(x, y);
	}

	public void Rot180()
	{
		var (x, y) = GetCenteredPosition();
		GridRot180();
		SetCenteredPosition(x, y);
	}

	public (float, float) GetCenteredPosition()
	{
		float num;
		float num2;
		if (tempShape.orientation % 2 != 0)
		{
			float y = fakeMiddle.rectTransform.sizeDelta.y;
			float x = fakeMiddle.rectTransform.sizeDelta.x;
			num = y;
			num2 = x;
		}
		else
		{
			float x2 = fakeMiddle.rectTransform.sizeDelta.x;
			float x = fakeMiddle.rectTransform.sizeDelta.y;
			num = x2;
			num2 = x;
		}
		return (fakeMiddle.rectTransform.anchoredPosition.x + num / 2f, fakeMiddle.rectTransform.anchoredPosition.y - num2 / 2f);
	}

	public void SetCenteredPosition(float x, float y)
	{
		float num;
		float num2;
		if (tempShape.orientation % 2 != 0)
		{
			float y2 = fakeMiddle.rectTransform.sizeDelta.y;
			float x2 = fakeMiddle.rectTransform.sizeDelta.x;
			num = y2;
			num2 = x2;
		}
		else
		{
			float x3 = fakeMiddle.rectTransform.sizeDelta.x;
			float x2 = fakeMiddle.rectTransform.sizeDelta.y;
			num = x3;
			num2 = x2;
		}
		fakeMiddle.rectTransform.anchoredPosition = new Vector2(x - num / 2f, y + num2 / 2f);
	}

	public void MovePosition(float x, float y)
	{
		fakeMiddle.rectTransform.position += new Vector3(x, y, 0f);
	}

	public Vector2 GetFakeRotatorCenteredPosition()
	{
		return fakeRotator.rectTransform.TransformPoint(fakeRotator.rectTransform.rect.center);
	}

	public Vector2 GetRotatorCenteredPosition()
	{
		return rotator.rectTransform.TransformPoint(rotator.rectTransform.rect.center);
	}

	public override void Destroy()
	{
		if (!destroyed)
		{
			destroyed = true;
			if (base.parentInventory != null)
			{
				base.parentInventory.Expel(this);
			}
			if (base.contentWindow != null)
			{
				base.contentWindow.Hide();
			}
			if (base.uniqueId != 0)
			{
				PlayerStore.Instance?.contentWindowPositions?.Remove(base.uniqueId);
			}
			DestroyContentWindow();
			if (fakeMiddle != null)
			{
				UnityEngine.Object.Destroy(fakeMiddle.gameObject);
			}
			UnityEngine.Object.Destroy(handler.gameObject);
		}
	}

	public bool IsDestroyed()
	{
		if (!destroyed)
		{
			return handler == null;
		}
		return true;
	}

	public bool HasTooltip()
	{
		if (IsTag("IS_TOOLTIP_DISABLED_TAG"))
		{
			return false;
		}
		return true;
	}

	private RichTextBuilder GetTooltipBasic()
	{
		int num = PlantGeneticsHelper.GetTooltipWidth(this, 40);
		if (IsTag("MACHINE_MANUAL_TAG"))
		{
			num = (int)((float)num * 2f);
		}
		if (IsTag("FEED_DISPENSER_TAG"))
		{
			num = (int)((float)num * 1.5f);
		}
		RichTextBuilder richTextBuilder = new RichTextBuilder(num);
		GeneralHelper.CreateGeneralItemTooltip(richTextBuilder, this);
		if (GeneralHelper.HasCustomName(this))
		{
			richTextBuilder.AddLine(GeneralHelper.GetCustomName(this) + " (" + GetDisplayName() + ")", ignoreEmpty: true, RenderHandler.ColorPalette.White, bold: true);
		}
		else
		{
			richTextBuilder.AddLine(GetDisplayName(), ignoreEmpty: true, RenderHandler.ColorPalette.White, bold: true);
		}
		if (IsTag("manufacturer"))
		{
			DisplayManufacturer(richTextBuilder, GetTagReadonly("manufacturer").GetString());
		}
		if (base.forceDisableActivate || base.forceDisableUse)
		{
			richTextBuilder.AddLine(LocHelper.GetLocalizedMechanic("mech_tooltip_disabled"), ignoreEmpty: true, RenderHandler.ColorPalette.Red, bold: true, italic: false, underline: true);
		}
		StolenHelper.CreateStolenItemTooltip(richTextBuilder, this);
		ItemPinHelper.CreatePinnedTooltip(richTextBuilder, this);
		CommonTooltipHelper.CreateItemTypeTooltip(richTextBuilder, this);
		ToggleAppearanceHelper.CreateToggleAppearanceTooltip(richTextBuilder, this);
		ContrabandHelper.CreateContrabandItemTooltip(richTextBuilder, this);
		if (base.parentInventory != null && base.parentInventory.IsInsertLocked() && base.parentInventory.IsRemoveLocked())
		{
			richTextBuilder.AddLine(LocHelper.GetLocalizedMechanic("mech_tooltip_locked_machine"), ignoreEmpty: true, RenderHandler.ColorPalette.Red);
		}
		richTextBuilder.AddEmptyLine(14);
		if (PlayerStore.Instance.devMode)
		{
			if (IsTag("ITEM_HIDDEN_TAG"))
			{
				richTextBuilder.Add(LocHelper.GetLocalizedMechanic("mech_tooltip_hidden_dev"), RenderHandler.ColorPalette.Black);
			}
			if (IsTag("IMPORTANT_TAG"))
			{
				richTextBuilder.Add(LocHelper.GetLocalizedMechanic("mech_tooltip_important_dev"), RenderHandler.ColorPalette.Red);
			}
			if (PlayerStore.Instance.IsEligibleForRepo(this))
			{
				richTextBuilder.Add(LocHelper.GetLocalizedMechanic("mech_tooltip_repo_dev"), RenderHandler.ColorPalette.Yellow);
			}
			richTextBuilder.AddLine(base.uniqueId + " Unique Id");
			richTextBuilder.AddLine(base.spriteAtlasPath + " " + base.spritePath);
			richTextBuilder.AddLine("");
		}
		if (GeneralHelper.IsItemLocked(this))
		{
			richTextBuilder.AddLine(LocHelper.GetLocalizedMechanic("mech_tooltip_locked"), ignoreEmpty: true, RenderHandler.ColorPalette.Red);
		}
		long value = GetValue(new List<GameCharacterItem>(), OverlayHandler.current.currentBarterCharacter);
		long value2 = GetValue(new List<GameCharacterItem>(), OverlayHandler.current.currentBarterCharacter, singleValue: true);
		int @int = GetTagReadonly("BOUGHT_PRICE_TAG").GetInt();
		if (GetCurrentValue(useRetailMarkup: false, withChild: true, includeEvents: true, forceMarkup: false, 0L) != 0L)
		{
			if (value != value2)
			{
				richTextBuilder.AddLine(LocHelper.GetLocalizedMechanic("mech_tooltip_unit_barter", value2), ignoreEmpty: true, RenderHandler.ColorPalette.LightTurquoise);
				richTextBuilder.AddLine(LocHelper.GetLocalizedMechanic("mech_tooltip_total_barter", value), ignoreEmpty: true, RenderHandler.ColorPalette.LightTurquoise);
			}
			else if (GetCurrentValue(useRetailMarkup: false, withChild: true, includeEvents: true, forceMarkup: false, 0L) != 0L)
			{
				richTextBuilder.AddLine(LocHelper.GetLocalizedMechanic("mech_tooltip_est_value", GetCurrentValue(useRetailMarkup: false, withChild: true, includeEvents: true, forceMarkup: false, 0L), GetBaseValue()), ignoreEmpty: true, RenderHandler.ColorPalette.LightTurquoise);
			}
			if (@int != 0)
			{
				richTextBuilder.AddLine(LocHelper.GetLocalizedMechanic("mech_tooltip_bought_at", @int), ignoreEmpty: true, RenderHandler.ColorPalette.LightTurquoise);
			}
			BarterHelper.CreateDiscountedBarteringItemTooltip(richTextBuilder, this);
			BarterHelper.CreateTaggedItemAsPaymentTooltip(richTextBuilder, this);
		}
		ModInternal.CreateTooltipEarly(richTextBuilder, this);
		if (PlayerStore.Instance.devMode)
		{
			ModInternal.CreateDevTooltipEarly(richTextBuilder, this);
		}
		AppraisalHelper.CreateAppraisalTooltip(richTextBuilder, this);
		AppraisalRankHelper.CreateCertificateTooltip(richTextBuilder, this);
		PowerHelper.CreatePowerSourceItemTooltip(richTextBuilder, this);
		EquipmentHelper.CreateEquipmentTooltip(richTextBuilder, this);
		InspectableHelper.CreateInspectableTooltip(richTextBuilder, this);
		InsCigaretteHelper.CreateDebugTooltip(richTextBuilder, this);
		InsStampHelper.CreateDebugTooltip(richTextBuilder, this);
		InsInjectorHelper.CreateDebugTooltip(richTextBuilder, this);
		RibwichHelper.CreateDebugTooltip(richTextBuilder, this);
		AppHandHelper.CreateDebugTooltip(richTextBuilder, this);
		TechnicianBackpackHelper.CreateComplexeEquipmentTooltip(richTextBuilder, this);
		TechnicianBackpackHelper.CreateComplexeModuleTooltip(richTextBuilder, this);
		TechnicianBackpackHelper.CreateComplexeSystemTooltip(richTextBuilder, this);
		UseCountHelper.CreateUseCountItemTooltip(richTextBuilder, this);
		ToolboxHelper.CreateToolboxTooltip(richTextBuilder, this);
		DebrisHelper.CreateTooltip(richTextBuilder, this);
		ReactorHelper.CreateTooltip(richTextBuilder, this);
		StorageExpansionHelper.CreateTooltip(richTextBuilder, this);
		MachineStorageCoordinator.CreateTooltip(richTextBuilder, this);
		HopperHelper.CreateTooltip(richTextBuilder, this);
		LouOperativeHelper.CreateLockerTooltip(richTextBuilder, this);
		OrganHelper.CreateOrganItemTooltip(richTextBuilder, this);
		MachineDesequencer.CreateMachineDesequencerTooltip(richTextBuilder, this);
		MachineBlender.CreateTooltip(richTextBuilder, this);
		MachineHeatingPlate.CreateTooltip(richTextBuilder, this);
		MachineChemFinisher.CreateTooltip(richTextBuilder, this);
		MachineAlarm.CreateAlarmTooltip(richTextBuilder, this);
		MachineProjector.CreateProjectorTooltip(richTextBuilder, this);
		MachineDealMaker.CreateTooltip(richTextBuilder, this);
		MachineMoistureFarm.CreateTooltip(richTextBuilder, this);
		MachinePurifier.CreateTooltip(richTextBuilder, this);
		MachineBottlePrinter.CreateTooltip(richTextBuilder, this);
		VendingMachineHelper.CreateTooltip(richTextBuilder, this);
		VendingFountainHelper.CreateTooltip(richTextBuilder, this);
		ScavHelper.CreateTooltip(richTextBuilder, this);
		HusbandryHelper.CreateVacuumRobotTooltip(richTextBuilder, this);
		MachineryHelper.CreateMachineryTooltip(richTextBuilder, this);
		MachineProgressHelper.CreateMachineTooltip(richTextBuilder, this);
		MachineHydroponic.CreateMachineTooltip(richTextBuilder, this);
		MachineDnaManipulator.CreateMachineTooltip(richTextBuilder, this);
		MachineTurboBoosterAdv.CreateMachineTooltip(richTextBuilder, this);
		CurrentConverterHelper.CreateTooltip(richTextBuilder, this);
		HypertechBatteryHelper.CreateTooltip(richTextBuilder, this);
		MindReprogrammerHelper.CreateTooltip(richTextBuilder, this);
		PropulsionHelper.CreatePropulsionSystemTooltip(richTextBuilder, this);
		MachineryHelper.CreateTrashPileTooltip(richTextBuilder, this);
		CanteenHelper.CreateCanteenTooltip(richTextBuilder, this);
		WaterFeatureHelper.CreateWaterFeatureTooltip(richTextBuilder, this);
		WaterHelper.CreateWaterContainerTooltip(richTextBuilder, this);
		IngotPurityHelper.CreateIngotTooltip(richTextBuilder, this);
		DurabilityHelper.CreateIngotTooltip(richTextBuilder, this);
		GunHelper.CreateTooltip(richTextBuilder, this);
		GunModHelper.CreateTooltip(richTextBuilder, this);
		LockHelper.CreateLockerContainerTooltip(richTextBuilder, this);
		LotteryHelper.CreateLotteryTicketTooltip(richTextBuilder, this);
		SecuredContainerHelper.CreateKeyItemTip(richTextBuilder, this);
		SecuredContainerHelper.CreateSecuredContainerToolTip(richTextBuilder, this);
		HydroponicHelper.CreateSeedItemTooltip(richTextBuilder, this);
		PlantGeneticsHelper.CreateSeedGeneticsTooltip(richTextBuilder, this);
		PlantGeneticsHelper.CreateDiskTooltip(richTextBuilder, this);
		MachineProgressHelper.CreateSourceItemDefaultTooltip(richTextBuilder, this);
		LabelerHelper.CreateLabeledItemTooltip(richTextBuilder, this);
		ModuleHelper.CreateModuleTooltip(richTextBuilder, this);
		HusbandryHelper.CreateItemTooltip(richTextBuilder, this);
		HusbandryHelper.CreateButcherTallyTooltip(richTextBuilder, this);
		LotteryHelper.CreateGamblersCoinTooltip(richTextBuilder, this);
		AnimalMarkHelper.CreateTooltip(richTextBuilder, this);
		IngesterHelper.CreateIngesterTooltip(richTextBuilder, this);
		FoodItemHelper.CreateFoodItemTooltip(richTextBuilder, this);
		AnimalCage.CreateTooltip(richTextBuilder, this);
		MachineFridge.CreateTooltip(richTextBuilder, this);
		MachineFeedDispenser.CreateTooltip(richTextBuilder, this);
		MachineAutoButcher.CreateTooltip(richTextBuilder, this);
		AquacultureHelper.CreateTooltip(richTextBuilder, this);
		MachineRAS.CreateTooltip(richTextBuilder, this);
		GutterflowPelletHelper.CreateTooltip(richTextBuilder, this);
		IndustrialFishFeedHelper.CreateTooltip(richTextBuilder, this);
		AugScanner.CreateTooltip(richTextBuilder, this);
		WineHelper.CreateTooltip(richTextBuilder, this);
		BlackLampHelper.CreateBlackLampTooltip(richTextBuilder, this);
		ModInternal.CreateTooltipLate(richTextBuilder, this);
		if (PlayerStore.Instance.devMode)
		{
			ModInternal.CreateDevTooltipLate(richTextBuilder, this);
		}
		if (!base.shortDescription.Equals(""))
		{
			richTextBuilder.AddLine(base.shortDescription, ignoreEmpty: true, RenderHandler.ColorPalette.White);
		}
		if (!base.flavorText.Equals(""))
		{
			richTextBuilder.AddLine(" " + base.flavorText, ignoreEmpty: true, RenderHandler.ColorPalette.Gray, bold: false, italic: true);
		}
		ContainerHelper.CreateContainerTooltip(richTextBuilder, this);
		ScratcherHelper.CreateScratcherTooltip(richTextBuilder, this);
		LotteryHelper.CreateLotteryRedeemTooltip(richTextBuilder, this);
		if (base.identifier.Equals("food_stamp"))
		{
			richTextBuilder.AddLine(LocHelper.GetLocalizedMechanic("mech_tooltip_food_stamp"), ignoreEmpty: true, RenderHandler.ColorPalette.LightBlue);
		}
		if (IsTag("IS_ITEM_TOGGLE_ON_TAG"))
		{
			richTextBuilder.AddLine(LocHelper.GetLocalizedUI("ui_rep_active"), ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
		}
		return richTextBuilder;
	}

	public virtual RichText GetTooltip()
	{
		RichTextBuilder richTextBuilder = GetTooltipBasic();
		if (base.parentInventory != null && base.tooltipRichTextFunc != null)
		{
			richTextBuilder = base.tooltipRichTextFunc(this, base.parentInventory, base.parentInventory.GetIncompleteItemInventorySlot(this), base.modifiedShape, base.modifiedState) ?? richTextBuilder;
		}
		ModSourceHelper.CreateTooltip(richTextBuilder, this);
		return richTextBuilder.Build();
	}

	public virtual RichText GetTooltipAdvanced()
	{
		RichTextBuilder richTextBuilder = GetTooltipBasic();
		if (base.parentInventory != null && base.tooltipRichTextFunc != null)
		{
			richTextBuilder = base.tooltipRichTextFunc(this, base.parentInventory, base.parentInventory.GetIncompleteItemInventorySlot(this), base.modifiedShape, base.modifiedState) ?? richTextBuilder;
		}
		ModSourceHelper.CreateTooltip(richTextBuilder, this);
		return richTextBuilder.Build();
	}
}
