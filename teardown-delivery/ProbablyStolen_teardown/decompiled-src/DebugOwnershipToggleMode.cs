using TMPro;
using UnityEngine;

public class DebugOwnershipToggleMode : MonoBehaviour
{
	private const string LABEL_TEXT = "Toggle Item Ownership";

	private const float CURSOR_OFFSET_X = 18f;

	private TextMeshProUGUI label;

	private RectTransform labelRect;

	private Canvas canvas;

	public static DebugOwnershipToggleMode Instance { get; private set; }

	public static void Activate(TMP_FontAsset font)
	{
		if (!(Instance != null))
		{
			GameObject obj = new GameObject("DebugOwnershipToggleMode", typeof(RectTransform));
			Canvas canvas = obj.AddComponent<Canvas>();
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			canvas.sortingOrder = 950;
			CustomUIManager.AddScreenScaler(obj);
			DebugOwnershipToggleMode debugOwnershipToggleMode = obj.AddComponent<DebugOwnershipToggleMode>();
			debugOwnershipToggleMode.canvas = canvas;
			debugOwnershipToggleMode.BuildLabel(font);
			Instance = debugOwnershipToggleMode;
			Debug.Log("[Debug] Toggle Item Ownership mode ON: left-click an item to flip ownership, right-click to exit.");
		}
	}

	public static void Deactivate()
	{
		if (!(Instance == null))
		{
			Object.Destroy(Instance.gameObject);
			Instance = null;
			Debug.Log("[Debug] Toggle Item Ownership mode OFF.");
		}
	}

	private void BuildLabel(TMP_FontAsset font)
	{
		GameObject gameObject = new GameObject("label", typeof(RectTransform));
		gameObject.transform.SetParent(base.transform, worldPositionStays: false);
		labelRect = (RectTransform)gameObject.transform;
		labelRect.pivot = new Vector2(0f, 0.5f);
		labelRect.sizeDelta = new Vector2(320f, 60f);
		CanvasGroup canvasGroup = gameObject.AddComponent<CanvasGroup>();
		canvasGroup.blocksRaycasts = false;
		canvasGroup.interactable = false;
		label = gameObject.AddComponent<TextMeshProUGUI>();
		if (font != null)
		{
			label.font = font;
		}
		label.fontSize = 16f;
		label.fontStyle = FontStyles.Bold;
		label.color = Color.yellow;
		label.alignment = TextAlignmentOptions.MidlineLeft;
		label.enableWordWrapping = false;
		label.raycastTarget = false;
		label.text = "Toggle Item Ownership";
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(1))
		{
			Deactivate();
			return;
		}
		GameItem gameItem = HoveredItem();
		UpdateLabel(gameItem);
		if (Input.GetMouseButtonDown(0) && gameItem != null)
		{
			bool flag = !GeneralHelper.IsItemOwned(gameItem);
			GeneralHelper.SetItemOwned(gameItem, flag);
			Debug.Log("[Debug] " + gameItem.name + " (" + gameItem.identifier + ") -> " + (flag ? "OWNED" : "NOT OWNED"));
		}
	}

	private static GameItem HoveredItem()
	{
		if (StoreUIManager.Instance != null && StoreUIManager.Instance.hoveringStoreUI)
		{
			return null;
		}
		if (ItemMouseDragHandler.current != null && ItemMouseDragHandler.current.IsDraggingItem)
		{
			return null;
		}
		GameItem gameItem = RenderHandler.RaycastElement<GameItem>(Input.mousePosition);
		if (gameItem == null || gameItem.isDebugMenu)
		{
			return null;
		}
		return gameItem;
	}

	private void UpdateLabel(GameItem hovered)
	{
		if (hovered == null)
		{
			label.text = "Toggle Item Ownership";
		}
		else
		{
			string text = (GeneralHelper.IsItemOwned(hovered) ? "<color=#8FE38F>Owned</color>" : "<color=#FF7A7A>Not owned</color>");
			label.text = "Toggle Item Ownership\n" + hovered.name + ": " + text;
		}
		float num = ((canvas != null) ? canvas.scaleFactor : 1f);
		Vector3 position = Input.mousePosition + new Vector3(18f * num, 0f, 0f);
		position.z = labelRect.position.z;
		labelRect.position = position;
	}

	private void OnDestroy()
	{
		if (Instance == this)
		{
			Instance = null;
		}
	}
}
