using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class CustomUIDebugWindows
{
	private class CustomerEntry
	{
		public string id;

		public string displayName;

		public string spriteName;

		public string faction;

		public int cooldown;

		public bool factoryFailed;
	}

	private const string SPRITE_VIEWER_ID = "debug_sprite_dict";

	private const string ALL_GROUPS = "All";

	private const int GRID_COLUMNS = 2;

	private const float SPRITE_WIDTH = 288f;

	private const float SPRITE_HEIGHT = 211f;

	private const float LABEL_HEIGHT = 18f;

	private const float APPLY_BUTTON_HEIGHT = 24f;

	private const string CUSTOMER_VIEWER_ID = "debug_customer_dict";

	private const int CUSTOMER_GRID_COLUMNS = 3;

	private const float PORTRAIT_WIDTH = 192f;

	private const float PORTRAIT_HEIGHT = 141f;

	private static string filter = "";

	private static string group = "All";

	private static float uiScale = 1f;

	private static string customerFilter = "";

	private static string customerFaction = "All";

	private static float customerUiScale = 1f;

	private static List<CustomerEntry> customerEntries;

	public static void OpenSpriteDictViewer()
	{
		if (CustomUIManager.Instance == null)
		{
			Debug.LogError("[CustomUI] No CustomUIManager, cannot open the SpriteDict viewer");
			return;
		}
		CustomUIWindow window = CustomUIManager.Instance.GetWindow("debug_sprite_dict");
		Vector2? vector = ((window != null && window.IsAlive) ? new Vector2?(window.Rect.anchoredPosition) : null);
		CustomUIBuilder customUIBuilder = CustomUIManager.Instance.CreateWindow("debug_sprite_dict", "SpriteDict Viewer", "rear").SetSize(650f, 620f).SetDraggable(draggable: true)
			.SetCloseOnEscape();
		if (SpriteDict.Instance == null || SpriteDict.Instance.spriteDictionary == null)
		{
			customUIBuilder.AddLabel("SpriteDict is not loaded in this scene.");
			customUIBuilder.Center().Show();
			return;
		}
		List<KeyValuePair<string, Sprite>> list = SpriteDict.Instance.spriteDictionary.OrderBy((KeyValuePair<string, Sprite> p) => SplitKey(p.Key).prefix, StringComparer.OrdinalIgnoreCase).ThenBy((KeyValuePair<string, Sprite> p) => SplitKey(p.Key).number).ThenBy((KeyValuePair<string, Sprite> p) => p.Key, StringComparer.OrdinalIgnoreCase)
			.ToList();
		List<string> groups = BuildGroups(list);
		int initialIndex = Mathf.Max(0, groups.IndexOf(group));
		List<KeyValuePair<string, Sprite>> list2 = list.Where(Matches).ToList();
		customUIBuilder.BeginRow();
		customUIBuilder.AddInput("filter by key...", filter, delegate(string v)
		{
			filter = v;
			OpenSpriteDictViewer();
		});
		customUIBuilder.AddDropdown(groups, initialIndex, delegate(int i)
		{
			group = groups[i];
			OpenSpriteDictViewer();
		});
		customUIBuilder.AddButton(ScaleLabel(), ToggleScale, "scale_button");
		customUIBuilder.End();
		customUIBuilder.AddLabel($"{list2.Count} of {list.Count} sprites, hover for key, click to copy");
		customUIBuilder.BeginScroll(490f);
		customUIBuilder.BeginGrid(2, 288f, 253f, 6f);
		foreach (KeyValuePair<string, Sprite> item in list2)
		{
			string key = item.Key;
			customUIBuilder.BeginColumn(2f);
			customUIBuilder.AddImage(item.Value, 288f, 211f);
			MakeSpriteCellInteractive(customUIBuilder.Last, key, item.Value);
			customUIBuilder.AddLabel(key);
			CenterCellLabel(customUIBuilder.Last);
			customUIBuilder.AddButton("Apply to customer", delegate
			{
				ApplyToCurrentCustomer(key);
			});
			customUIBuilder.End();
		}
		customUIBuilder.End();
		customUIBuilder.End();
		if (vector.HasValue)
		{
			customUIBuilder.SetPosition(vector.Value);
		}
		else
		{
			customUIBuilder.Center();
		}
		ApplyScale(customUIBuilder.Show());
	}

	private static string ScaleLabel()
	{
		if (!(uiScale >= 2f))
		{
			return "Scale: 1x";
		}
		return "Scale: 2x";
	}

	private static void ToggleScale()
	{
		uiScale = ((uiScale >= 2f) ? 1f : 2f);
		CustomUIWindow window = CustomUIManager.Instance.GetWindow("debug_sprite_dict");
		ApplyScale(window);
		window?.Get("scale_button")?.SetText(ScaleLabel());
	}

	private static void ApplyScale(CustomUIWindow window)
	{
		if (window != null && window.Rect != null)
		{
			window.Rect.localScale = Vector3.one * uiScale;
		}
	}

	private static void ApplyToCurrentCustomer(string key)
	{
		if (PlayerStore.Instance == null || PlayerStore.Instance.currentClientInstance == null)
		{
			Debug.LogWarning("[CustomUI] No customer at the store right now, sprite not applied");
			return;
		}
		StoreClientMono storeClientMono = ((StoreUIManager.Instance != null) ? StoreUIManager.Instance.clientGameObject : null);
		if (storeClientMono != null && storeClientMono.image != null)
		{
			storeClientMono.image.sprite = SpriteDict.Instance.GetSprite(key);
		}
		StoreClient clientBlueprint = PlayerStore.Instance.currentClientInstance.GetClientBlueprint();
		if (clientBlueprint != null)
		{
			clientBlueprint.spriteName = key;
		}
		Debug.Log("[CustomUI] Customer sprite set to '" + key + "'");
	}

	private static void MakeSpriteCellInteractive(CustomUIElement element, string key, Sprite sprite)
	{
		if (element != null && !(element.image == null))
		{
			element.image.raycastTarget = true;
			element.SetTooltip($"{key} ({sprite.rect.width}x{sprite.rect.height})");
			Button button = element.root.AddComponent<Button>();
			button.targetGraphic = element.image;
			button.transition = Selectable.Transition.None;
			button.onClick.AddListener(delegate
			{
				GUIUtility.systemCopyBuffer = key;
				Debug.Log("[CustomUI] Sprite key copied to clipboard: " + key);
			});
		}
	}

	private static void CenterCellLabel(CustomUIElement element)
	{
		if (element != null && !(element.text == null))
		{
			element.text.alignment = TextAlignmentOptions.Center;
			element.text.enableWordWrapping = false;
			element.text.fontSizeMax = element.text.fontSize;
			element.text.fontSizeMin = 1f;
			element.text.enableAutoSizing = true;
			element.SetPreferredSize(288f, 18f);
		}
	}

	private static bool Matches(KeyValuePair<string, Sprite> pair)
	{
		if (group != "All" && SplitKey(pair.Key).prefix != group)
		{
			return false;
		}
		if (string.IsNullOrEmpty(filter))
		{
			return true;
		}
		return pair.Key.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0;
	}

	private static List<string> BuildGroups(List<KeyValuePair<string, Sprite>> all)
	{
		List<string> list = all.Select((KeyValuePair<string, Sprite> p) => SplitKey(p.Key).prefix).Distinct().OrderBy((string g) => g, StringComparer.OrdinalIgnoreCase)
			.ToList();
		list.Insert(0, "All");
		return list;
	}

	private static (string prefix, int number) SplitKey(string key)
	{
		int num = key.Length;
		while (num > 0 && char.IsDigit(key[num - 1]))
		{
			num--;
		}
		string item = key.Substring(0, num);
		int result;
		int item2 = ((num < key.Length && int.TryParse(key.Substring(num), out result)) ? result : 0);
		return (prefix: item, number: item2);
	}

	public static void OpenCustomerDictViewer()
	{
		if (CustomUIManager.Instance == null)
		{
			Debug.LogError("[CustomUI] No CustomUIManager, cannot open the customer dict viewer");
			return;
		}
		CustomUIWindow window = CustomUIManager.Instance.GetWindow("debug_customer_dict");
		Vector2? vector = ((window != null && window.IsAlive) ? new Vector2?(window.Rect.anchoredPosition) : null);
		CustomUIBuilder customUIBuilder = CustomUIManager.Instance.CreateWindow("debug_customer_dict", "Customer Dict Viewer", "rear").SetSize(680f, 640f).SetDraggable(draggable: true)
			.SetCloseOnEscape();
		List<CustomerEntry> list = GetCustomerEntries();
		List<string> factions = BuildFactionGroups(list);
		int initialIndex = Mathf.Max(0, factions.IndexOf(customerFaction));
		List<CustomerEntry> list2 = list.Where(MatchesCustomer).ToList();
		customUIBuilder.BeginRow();
		customUIBuilder.AddInput("filter by id or name...", customerFilter, delegate(string v)
		{
			customerFilter = v;
			OpenCustomerDictViewer();
		});
		customUIBuilder.AddDropdown(factions, initialIndex, delegate(int i)
		{
			customerFaction = factions[i];
			OpenCustomerDictViewer();
		});
		customUIBuilder.AddButton(CustomerScaleLabel(), ToggleCustomerScale, "customer_scale_button");
		customUIBuilder.End();
		customUIBuilder.AddLabel($"{list2.Count} of {list.Count} customers, hover for details, click portrait to copy id");
		customUIBuilder.AddLabel(BuildStackText(), "customer_stack_label");
		customUIBuilder.BeginScroll(490f);
		customUIBuilder.BeginGrid(3, 192f, 209f, 6f);
		foreach (CustomerEntry item in list2)
		{
			string id = item.id;
			customUIBuilder.BeginColumn(2f);
			customUIBuilder.AddImage(ResolveCustomerSprite(item), 192f, 141f);
			MakeCustomerCellInteractive(customUIBuilder.Last, item);
			customUIBuilder.AddLabel(id);
			CenterCustomerLabel(customUIBuilder.Last);
			customUIBuilder.AddLabel(item.factoryFailed ? "(factory failed)" : item.displayName);
			CenterCustomerLabel(customUIBuilder.Last);
			customUIBuilder.AddButton("Queue next", delegate
			{
				QueueCustomer(id);
			});
			customUIBuilder.End();
		}
		customUIBuilder.End();
		customUIBuilder.End();
		if (vector.HasValue)
		{
			customUIBuilder.SetPosition(vector.Value);
		}
		else
		{
			customUIBuilder.Center();
		}
		CustomUIWindow customUIWindow = customUIBuilder.Show();
		if (customUIWindow != null && customUIWindow.Rect != null)
		{
			customUIWindow.Rect.localScale = Vector3.one * customerUiScale;
		}
	}

	private static string CustomerScaleLabel()
	{
		if (!(customerUiScale >= 2f))
		{
			return "Scale: 1x";
		}
		return "Scale: 2x";
	}

	private static void ToggleCustomerScale()
	{
		customerUiScale = ((customerUiScale >= 2f) ? 1f : 2f);
		CustomUIWindow window = CustomUIManager.Instance.GetWindow("debug_customer_dict");
		if (window != null && window.Rect != null)
		{
			window.Rect.localScale = Vector3.one * customerUiScale;
		}
		window?.Get("customer_scale_button")?.SetText(CustomerScaleLabel());
	}

	private static void QueueCustomer(string identifier)
	{
		if (PlayerStore.Instance == null || PlayerStore.Instance.storeClientManager == null)
		{
			Debug.LogWarning("[CustomUI] No StoreClientManager, customer not queued");
			return;
		}
		StoreClient storeClient = StoreClientListDict.CreateStoreClient(identifier);
		if (storeClient == null)
		{
			Debug.LogWarning("[CustomUI] Could not create customer '" + identifier + "'");
			return;
		}
		PlayerStore.Instance.storeClientManager.AddNextClient(storeClient);
		Debug.Log("[CustomUI] Queued customer '" + identifier + "' as next");
		RefreshStackLabel();
	}

	private static void RefreshStackLabel()
	{
		CustomUIManager.Instance.GetWindow("debug_customer_dict")?.Get("customer_stack_label")?.SetText(BuildStackText());
	}

	private static string BuildStackText()
	{
		if (PlayerStore.Instance == null || PlayerStore.Instance.storeClientManager == null)
		{
			return "Today's stack: no StoreClientManager in this scene";
		}
		List<StoreClient> clientStack = PlayerStore.Instance.storeClientManager.clientStack;
		if (clientStack == null || clientStack.Count == 0)
		{
			return "Today's stack: empty";
		}
		return $"Today's stack ({clientStack.Count}): " + string.Join(", ", from c in clientStack
			where c != null
			select c.identifier);
	}

	private static List<CustomerEntry> GetCustomerEntries()
	{
		if (customerEntries != null)
		{
			return customerEntries;
		}
		customerEntries = new List<CustomerEntry>();
		foreach (KeyValuePair<string, Func<StoreClient>> item in StoreClientListDict.storeClientDict)
		{
			CustomerEntry customerEntry = new CustomerEntry
			{
				id = item.Key
			};
			try
			{
				StoreClient storeClient = item.Value();
				customerEntry.displayName = storeClient.displayName;
				customerEntry.faction = (string.IsNullOrEmpty(storeClient.clientFaction) ? "NONE" : storeClient.clientFaction);
				customerEntry.cooldown = storeClient.cooldown;
				customerEntry.spriteName = ((!string.IsNullOrEmpty(storeClient.spriteName)) ? storeClient.spriteName : ((storeClient.possibleSprites != null && storeClient.possibleSprites.Count > 0) ? storeClient.possibleSprites[0] : ""));
			}
			catch (Exception ex)
			{
				customerEntry.displayName = "";
				customerEntry.faction = "NONE";
				customerEntry.spriteName = "";
				customerEntry.factoryFailed = true;
				Debug.LogWarning("[CustomUI] Customer factory '" + item.Key + "' threw: " + ex.Message);
			}
			customerEntries.Add(customerEntry);
		}
		customerEntries = customerEntries.OrderBy((CustomerEntry e) => e.faction, StringComparer.OrdinalIgnoreCase).ThenBy((CustomerEntry e) => e.id, StringComparer.OrdinalIgnoreCase).ToList();
		return customerEntries;
	}

	private static Sprite ResolveCustomerSprite(CustomerEntry entry)
	{
		if (string.IsNullOrEmpty(entry.spriteName))
		{
			return null;
		}
		if (SpriteDict.Instance == null || SpriteDict.Instance.spriteDictionary == null)
		{
			return null;
		}
		SpriteDict.Instance.spriteDictionary.TryGetValue(entry.spriteName, out var value);
		return value;
	}

	private static void MakeCustomerCellInteractive(CustomUIElement element, CustomerEntry entry)
	{
		if (element != null && !(element.image == null))
		{
			element.image.raycastTarget = true;
			element.SetTooltip("id: " + entry.id + "\nname: " + entry.displayName + "\nfaction: " + entry.faction + "\n" + $"cooldown: {entry.cooldown}\n" + "sprite: " + (string.IsNullOrEmpty(entry.spriteName) ? "(none)" : entry.spriteName));
			Button button = element.root.AddComponent<Button>();
			button.targetGraphic = element.image;
			button.transition = Selectable.Transition.None;
			button.onClick.AddListener(delegate
			{
				GUIUtility.systemCopyBuffer = entry.id;
				Debug.Log("[CustomUI] Customer id copied to clipboard: " + entry.id);
			});
		}
	}

	private static void CenterCustomerLabel(CustomUIElement element)
	{
		if (element != null && !(element.text == null))
		{
			element.text.alignment = TextAlignmentOptions.Center;
			element.text.enableWordWrapping = false;
			element.text.fontSizeMax = element.text.fontSize;
			element.text.fontSizeMin = 1f;
			element.text.enableAutoSizing = true;
			element.SetPreferredSize(192f, 18f);
		}
	}

	private static bool MatchesCustomer(CustomerEntry entry)
	{
		if (customerFaction != "All" && entry.faction != customerFaction)
		{
			return false;
		}
		if (string.IsNullOrEmpty(customerFilter))
		{
			return true;
		}
		if (entry.id.IndexOf(customerFilter, StringComparison.OrdinalIgnoreCase) >= 0)
		{
			return true;
		}
		if (entry.displayName != null)
		{
			return entry.displayName.IndexOf(customerFilter, StringComparison.OrdinalIgnoreCase) >= 0;
		}
		return false;
	}

	private static List<string> BuildFactionGroups(List<CustomerEntry> all)
	{
		List<string> list = all.Select((CustomerEntry e) => e.faction).Distinct().OrderBy((string f) => f, StringComparer.OrdinalIgnoreCase)
			.ToList();
		list.Insert(0, "All");
		return list;
	}
}
