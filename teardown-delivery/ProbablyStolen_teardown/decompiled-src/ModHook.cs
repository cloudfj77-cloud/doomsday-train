using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class ModHook : MonoBehaviour
{
	public static ModHook Instance { get; private set; }

	public static event Action<ModItemDirectory> OnModItemDirectoryInit;

	public static event Action<StoreClientManager> OnGenerateCustomerVeryEarly;

	public static event Action<StoreClientManager> OnGenerateCustomerEarly;

	public static event Action<StoreClientManager> OnGenerateCustomerNormal;

	public static event Action<StoreClientManager> OnGenerateCustomerLate;

	public static event Action<StoreClientManager> OnGenerateCustomerVeryLate;

	public static event Action OnGameLoadedInit;

	public static event Action OnGameLoadedEarly;

	public static event Action OnGameLoadedNormal;

	public static event Action OnGameLoadedLate;

	public static event Action OnShutterOpenedEarly;

	public static event Action OnShutterOpenedLate;

	public static event Action OnShutterClosedEarly;

	public static event Action OnShutterClosedLate;

	public static event Action OnGoingSleepEarly;

	public static event Action OnGoingSleepLate;

	public static event Action OnWakingUpEarly;

	public static event Action OnWakingUpLate;

	public static event Action OnLeavingStoreEarly;

	public static event Action OnLeavingStoreLate;

	public static event Action OnReturningStoreEarly;

	public static event Action OnReturningStoreLate;

	public static event Action OnHandlingNightlyServicesEarly;

	public static event Action OnHandlingNightlyServicesLate;

	public static event Action<List<GameItem>> OnPlaceInventorInventoryItemEarly;

	public static event Action<List<GameItem>> OnPlaceInventorInventoryItemLate;

	public static event Action<RichTextBuilder, GameItem> OnCreateTooltipEarly;

	public static event Action<RichTextBuilder, GameItem> OnCreateTooltipLate;

	public static event Action<RichTextBuilder, GameItem> OnCreateDevTooltipEarly;

	public static event Action<RichTextBuilder, GameItem> OnCreateDevTooltipLate;

	public static event Action<LoadedMod> OnModAssetsLoaded;

	public static event Action<string> OnModConfigApplied;

	public static event Action OnLootTablesLoaded;

	public static event Action<LootRollContext> OnLootRolled;

	public static event Action<string, PaperUI> OnPaperOpened;

	public static event Action<string> OnNetworkUpgradeUnlocked;

	public static event Action<string> OnNewGameStarted;

	public static event Action<string> OnDifficultyApplied;

	public static event Action<string> OnAfterhourArriveEarly;

	public static event Action<string> OnAfterhourArriveLate;

	public static event Action<string> OnAfterhourLeaveEarly;

	public static event Action<string> OnAfterhourLeaveLate;

	public static event Action<string, string> OnAfterhourEventStarted;

	public static event Action<string, string> OnAfterhourEventEnded;

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		Instance = this;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	private static void SafeInvoke(string hookName, Action handlers)
	{
		if (handlers == null)
		{
			return;
		}
		Delegate[] invocationList = handlers.GetInvocationList();
		for (int i = 0; i < invocationList.Length; i++)
		{
			Action action = (Action)invocationList[i];
			try
			{
				action();
			}
			catch (Exception e)
			{
				LogHandlerError(hookName, action, e);
			}
		}
	}

	private static void SafeInvoke<T>(string hookName, Action<T> handlers, T arg)
	{
		if (handlers == null)
		{
			return;
		}
		Delegate[] invocationList = handlers.GetInvocationList();
		for (int i = 0; i < invocationList.Length; i++)
		{
			Action<T> action = (Action<T>)invocationList[i];
			try
			{
				action(arg);
			}
			catch (Exception e)
			{
				LogHandlerError(hookName, action, e);
			}
		}
	}

	private static void SafeInvoke<T1, T2>(string hookName, Action<T1, T2> handlers, T1 arg1, T2 arg2)
	{
		if (handlers == null)
		{
			return;
		}
		Delegate[] invocationList = handlers.GetInvocationList();
		for (int i = 0; i < invocationList.Length; i++)
		{
			Action<T1, T2> action = (Action<T1, T2>)invocationList[i];
			try
			{
				action(arg1, arg2);
			}
			catch (Exception e)
			{
				LogHandlerError(hookName, action, e);
			}
		}
	}

	private static void SafeInvokeGenerateCustomer(string hookName, Action<StoreClientManager> handlers, StoreClientManager manager)
	{
		if (handlers == null)
		{
			return;
		}
		Delegate[] invocationList = handlers.GetInvocationList();
		for (int i = 0; i < invocationList.Length; i++)
		{
			Action<StoreClientManager> action = (Action<StoreClientManager>)invocationList[i];
			HashSet<StoreClient> before = ((manager?.clientStack != null) ? new HashSet<StoreClient>(manager.clientStack) : null);
			try
			{
				action(manager);
			}
			catch (Exception e)
			{
				LogHandlerError(hookName, action, e);
			}
			try
			{
				ModSourceHelper.MarkNewClients(manager?.clientStack, before, action);
			}
			catch (Exception arg)
			{
				Debug.LogError($"[ModHook] Marking the clients added by a '{hookName}' handler failed: {arg}");
			}
		}
	}

	private static void LogHandlerError(string hookName, Delegate handler, Exception e)
	{
		MethodInfo method = handler.Method;
		Assembly assembly = method.DeclaringType?.Assembly;
		LoadedMod loadedMod = ModSourceHelper.FindModByAssembly(assembly);
		string text = ((loadedMod != null) ? loadedMod.Manifest.Name : (assembly?.GetName().Name ?? "unknown assembly"));
		Debug.LogError($"[ModHook] '{hookName}' handler {method.DeclaringType?.FullName}.{method.Name} from '{text}' threw: {e}");
	}

	internal static void FireOnModItemDirectoryInit(ModItemDirectory directory)
	{
		SafeInvoke("OnModItemDirectoryInit", ModHook.OnModItemDirectoryInit, directory);
	}

	internal static void FireOnGenerateCustomerVeryEarly(StoreClientManager manager)
	{
		SafeInvokeGenerateCustomer("OnGenerateCustomerVeryEarly", ModHook.OnGenerateCustomerVeryEarly, manager);
	}

	internal static void FireOnGenerateCustomerEarly(StoreClientManager manager)
	{
		SafeInvokeGenerateCustomer("OnGenerateCustomerEarly", ModHook.OnGenerateCustomerEarly, manager);
	}

	internal static void FireOnGenerateCustomerNormal(StoreClientManager manager)
	{
		SafeInvokeGenerateCustomer("OnGenerateCustomerNormal", ModHook.OnGenerateCustomerNormal, manager);
	}

	internal static void FireOnGenerateCustomerLate(StoreClientManager manager)
	{
		SafeInvokeGenerateCustomer("OnGenerateCustomerLate", ModHook.OnGenerateCustomerLate, manager);
	}

	internal static void FireOnGenerateCustomerVeryLate(StoreClientManager manager)
	{
		SafeInvokeGenerateCustomer("OnGenerateCustomerVeryLate", ModHook.OnGenerateCustomerVeryLate, manager);
	}

	internal static void FireOnGameLoadedInit()
	{
		SafeInvoke("OnGameLoadedInit", ModHook.OnGameLoadedInit);
	}

	internal static void FireOnGameLoadedEarly()
	{
		SafeInvoke("OnGameLoadedEarly", ModHook.OnGameLoadedEarly);
	}

	internal static void FireOnGameLoadedNormal()
	{
		SafeInvoke("OnGameLoadedNormal", ModHook.OnGameLoadedNormal);
	}

	internal static void FireOnGameLoadedLate()
	{
		SafeInvoke("OnGameLoadedLate", ModHook.OnGameLoadedLate);
	}

	internal static void FireOnShutterOpenedEarly()
	{
		SafeInvoke("OnShutterOpenedEarly", ModHook.OnShutterOpenedEarly);
	}

	internal static void FireOnShutterOpenedLate()
	{
		SafeInvoke("OnShutterOpenedLate", ModHook.OnShutterOpenedLate);
	}

	internal static void FireOnShutterClosedEarly()
	{
		SafeInvoke("OnShutterClosedEarly", ModHook.OnShutterClosedEarly);
	}

	internal static void FireOnShutterClosedLate()
	{
		SafeInvoke("OnShutterClosedLate", ModHook.OnShutterClosedLate);
	}

	internal static void FireOnGoingSleepEarly()
	{
		SafeInvoke("OnGoingSleepEarly", ModHook.OnGoingSleepEarly);
	}

	internal static void FireOnGoingSleepLate()
	{
		SafeInvoke("OnGoingSleepLate", ModHook.OnGoingSleepLate);
	}

	internal static void FireOnWakingUpEarly()
	{
		SafeInvoke("OnWakingUpEarly", ModHook.OnWakingUpEarly);
	}

	internal static void FireOnWakingUpLate()
	{
		SafeInvoke("OnWakingUpLate", ModHook.OnWakingUpLate);
	}

	internal static void FireOnLeavingStoreEarly()
	{
		SafeInvoke("OnLeavingStoreEarly", ModHook.OnLeavingStoreEarly);
	}

	internal static void FireOnLeavingStoreLate()
	{
		SafeInvoke("OnLeavingStoreLate", ModHook.OnLeavingStoreLate);
	}

	internal static void FireOnReturningStoreEarly()
	{
		SafeInvoke("OnReturningStoreEarly", ModHook.OnReturningStoreEarly);
	}

	internal static void FireOnReturningStoreLate()
	{
		SafeInvoke("OnReturningStoreLate", ModHook.OnReturningStoreLate);
	}

	internal static void FireOnHandlingNightlyServicesEarly()
	{
		SafeInvoke("OnHandlingNightlyServicesEarly", ModHook.OnHandlingNightlyServicesEarly);
	}

	internal static void FireOnHandlingNightlyServicesLate()
	{
		SafeInvoke("OnHandlingNightlyServicesLate", ModHook.OnHandlingNightlyServicesLate);
	}

	internal static void FireOnPlaceInventorInventoryItemEarly(List<GameItem> items)
	{
		SafeInvoke("OnPlaceInventorInventoryItemEarly", ModHook.OnPlaceInventorInventoryItemEarly, items);
	}

	internal static void FireOnPlaceInventorInventoryItemLate(List<GameItem> items)
	{
		SafeInvoke("OnPlaceInventorInventoryItemLate", ModHook.OnPlaceInventorInventoryItemLate, items);
	}

	internal static void FireOnCreateTooltipEarly(RichTextBuilder builder, GameItem item)
	{
		SafeInvoke("OnCreateTooltipEarly", ModHook.OnCreateTooltipEarly, builder, item);
	}

	internal static void FireOnCreateTooltipLate(RichTextBuilder builder, GameItem item)
	{
		SafeInvoke("OnCreateTooltipLate", ModHook.OnCreateTooltipLate, builder, item);
	}

	internal static void FireOnCreateDevTooltipEarly(RichTextBuilder builder, GameItem item)
	{
		SafeInvoke("OnCreateDevTooltipEarly", ModHook.OnCreateDevTooltipEarly, builder, item);
	}

	internal static void FireOnCreateDevTooltipLate(RichTextBuilder builder, GameItem item)
	{
		SafeInvoke("OnCreateDevTooltipLate", ModHook.OnCreateDevTooltipLate, builder, item);
	}

	internal static void FireOnModAssetsLoaded(LoadedMod mod)
	{
		SafeInvoke("OnModAssetsLoaded", ModHook.OnModAssetsLoaded, mod);
	}

	internal static void FireOnModConfigApplied(string modId)
	{
		SafeInvoke("OnModConfigApplied", ModHook.OnModConfigApplied, modId);
	}

	internal static void FireOnLootTablesLoaded()
	{
		SafeInvoke("OnLootTablesLoaded", ModHook.OnLootTablesLoaded);
	}

	internal static void FireOnLootRolled(LootRollContext context)
	{
		SafeInvoke("OnLootRolled", ModHook.OnLootRolled, context);
	}

	internal static void FireOnPaperOpened(string bookId, PaperUI ui)
	{
		SafeInvoke("OnPaperOpened", ModHook.OnPaperOpened, bookId, ui);
	}

	internal static void FireOnNetworkUpgradeUnlocked(string upgradeId)
	{
		SafeInvoke("OnNetworkUpgradeUnlocked", ModHook.OnNetworkUpgradeUnlocked, upgradeId);
	}

	internal static void FireOnNewGameStarted(string scenarioId)
	{
		SafeInvoke("OnNewGameStarted", ModHook.OnNewGameStarted, scenarioId);
	}

	internal static void FireOnDifficultyApplied(string difficultyId)
	{
		SafeInvoke("OnDifficultyApplied", ModHook.OnDifficultyApplied, difficultyId);
	}

	internal static void FireOnAfterhourArriveEarly(string locationId)
	{
		SafeInvoke("OnAfterhourArriveEarly", ModHook.OnAfterhourArriveEarly, locationId);
	}

	internal static void FireOnAfterhourArriveLate(string locationId)
	{
		SafeInvoke("OnAfterhourArriveLate", ModHook.OnAfterhourArriveLate, locationId);
	}

	internal static void FireOnAfterhourLeaveEarly(string locationId)
	{
		SafeInvoke("OnAfterhourLeaveEarly", ModHook.OnAfterhourLeaveEarly, locationId);
	}

	internal static void FireOnAfterhourLeaveLate(string locationId)
	{
		SafeInvoke("OnAfterhourLeaveLate", ModHook.OnAfterhourLeaveLate, locationId);
	}

	internal static void FireOnAfterhourEventStarted(string locationId, string eventId)
	{
		SafeInvoke("OnAfterhourEventStarted", ModHook.OnAfterhourEventStarted, locationId, eventId);
	}

	internal static void FireOnAfterhourEventEnded(string locationId, string eventId)
	{
		SafeInvoke("OnAfterhourEventEnded", ModHook.OnAfterhourEventEnded, locationId, eventId);
	}
}
