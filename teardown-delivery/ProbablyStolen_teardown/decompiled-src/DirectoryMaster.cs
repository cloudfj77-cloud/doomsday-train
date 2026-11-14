using System;
using System.Collections.Generic;
using System.Linq;

public class DirectoryMaster
{
	private static Dictionary<Type, List<object>> singletonInstance;

	private static Dictionary<Type, List<object>> directories
	{
		get
		{
			if (singletonInstance == null)
			{
				singletonInstance = new Dictionary<Type, List<object>>();
			}
			return singletonInstance;
		}
	}

	public static bool AddDirectory<T>(Directory<T> directory) where T : DirectoryEntry
	{
		if ((object)directory == null)
		{
			Debug.LogWarning("Trying to add a null Directory in DirectoryMaster, ignoring duplicated directory.");
			return false;
		}
		if (!directories.ContainsKey(typeof(T)))
		{
			directories.TryAdd(typeof(T), new List<object>());
		}
		List<object> list = directories[typeof(T)];
		foreach (object item in list)
		{
			if (item.GetType() == directory.GetType())
			{
				Debug.LogWarning("Trying to add a duplicate Directory '" + directory.GetType()?.ToString() + "' in DirectoryMaster, ignoring duplicated directory.");
				return false;
			}
		}
		list.Add(directory);
		return true;
	}

	public static void Reset()
	{
		singletonInstance = null;
	}

	public static List<string> GetDictionaryClassNameList<T>() where T : DirectoryEntry
	{
		List<string> list = new List<string>();
		if (directories.ContainsKey(typeof(T)))
		{
			foreach (object item in directories[typeof(T)])
			{
				if (item is Directory<T>)
				{
					list.Add(item.GetType().Name);
				}
			}
		}
		return list;
	}

	public static List<string> GetIdentifierList<T>(string dictionaryClassName = null) where T : DirectoryEntry
	{
		List<string> list = new List<string>();
		if (directories.ContainsKey(typeof(T)))
		{
			foreach (object item in directories[typeof(T)])
			{
				if ((dictionaryClassName == null || !(item.GetType().Name != dictionaryClassName)) && item is Directory<T> directory)
				{
					list.AddRange(directory.factoryDictionary.Keys);
				}
			}
		}
		return list;
	}

	public static T Create<T>(string identifier) where T : DirectoryEntry
	{
		if (identifier == null)
		{
			return default(T);
		}
		if (directories.ContainsKey(typeof(T)))
		{
			foreach (object item in directories[typeof(T)])
			{
				if (item is Directory<T> directory)
				{
					T val = directory.Create(identifier);
					if (val != null)
					{
						return val;
					}
				}
			}
		}
		return default(T);
	}

	public static bool Has<T>(string identifier) where T : DirectoryEntry
	{
		if (identifier == null)
		{
			return false;
		}
		if (directories.ContainsKey(typeof(T)))
		{
			foreach (object item in directories[typeof(T)])
			{
				if (item is Directory<T> directory && directory.Has(identifier))
				{
					return true;
				}
			}
		}
		return false;
	}

	public static Func<T> GetFactory<T>(string identifier) where T : DirectoryEntry
	{
		if (identifier == null)
		{
			return null;
		}
		if (directories.ContainsKey(typeof(T)))
		{
			foreach (object item in directories[typeof(T)])
			{
				if (item is Directory<T> { factoryDictionary: not null } directory && directory.factoryDictionary.TryGetValue(identifier, out var value))
				{
					return value;
				}
			}
		}
		return null;
	}

	public static bool Remove<T>(string identifier) where T : DirectoryEntry
	{
		if (identifier == null)
		{
			return false;
		}
		if (directories.ContainsKey(typeof(T)))
		{
			foreach (object item in directories[typeof(T)])
			{
				if (item is Directory<T> directory && directory.Remove(identifier))
				{
					return true;
				}
			}
		}
		return false;
	}

	public static T Create<T, K>(K identifier) where T : DirectoryEntry where K : Enum
	{
		if (identifier == null)
		{
			return default(T);
		}
		if (directories.ContainsKey(typeof(T)))
		{
			foreach (object item in directories[typeof(T)])
			{
				if (item is Directory<T> directory)
				{
					T val = directory.Create(identifier);
					if (val != null)
					{
						return val;
					}
				}
			}
		}
		return default(T);
	}

	public static bool Has<T, K>(K identifier) where T : DirectoryEntry where K : Enum
	{
		if (identifier == null)
		{
			return false;
		}
		if (directories.ContainsKey(typeof(T)))
		{
			foreach (object item in directories[typeof(T)])
			{
				if (item is Directory<T> directory && directory.Has(identifier))
				{
					return true;
				}
			}
		}
		return false;
	}

	public static bool Remove<T, K>(K identifier) where T : DirectoryEntry where K : Enum
	{
		if (identifier == null)
		{
			return false;
		}
		if (directories.ContainsKey(typeof(T)))
		{
			foreach (object item in directories[typeof(T)])
			{
				if (item is Directory<T> directory && directory.Remove(identifier))
				{
					return true;
				}
			}
		}
		return false;
	}

	public static GameCharacterItem CharacterItem(string identifier)
	{
		if (identifier != null)
		{
			GameCharacterItem gameCharacterItem = Create<GameCharacterItem>(identifier);
			if (gameCharacterItem != null)
			{
				List<GameItem> list = GraphUtils.FindAllChildrenType<GameItem>(gameCharacterItem);
				if (list.Any())
				{
					Debug.LogWarning("CharacterItem '" + identifier + "' created with DirectoryMaster contains child items, this should not happen as it might cause problems during saving and loading! Removing all child items as a fix...");
					foreach (GameItem item in list)
					{
						item.Destroy();
					}
				}
				return gameCharacterItem;
			}
		}
		Debug.LogWarning("Could not find GameCharacterItem identifier '" + identifier + "' when attempting to create GameCharacterItem! Creating dummy GameCharacterItem instead...");
		return CharacterItemDirectory.CreateEmptyCharacterItem(identifier);
	}

	public static GameItem Item(string identifier, bool isOwned = true)
	{
		if (identifier != null)
		{
			GameItem gameItem = Create<GameItem>(identifier);
			if (gameItem != null)
			{
				List<GameItem> list = GraphUtils.FindAllChildrenType<GameItem>(gameItem);
				if (list.Any())
				{
					Debug.LogWarning("GameItem '" + identifier + "' created with DirectoryMaster contains child items, this should not happen as it might cause problems during saving and loading! Removing all child items as a fix...");
					foreach (GameItem item in list)
					{
						item.Destroy();
					}
				}
				GeneralHelper.SetItemOwned(gameItem, isOwned);
				if (gameItem.uniqueId == 0)
				{
					gameItem.uniqueId = PlayerStore.Instance.GetUniqueID();
				}
				PermitHelper.OnNewItemCreated(gameItem);
				return gameItem;
			}
		}
		Debug.LogWarning("Could not find GameItem identifier '" + identifier + "' when attempting to create GameItem! Creating dummy GameItem instead...");
		return ItemDirectory.CreateEmptyItem(identifier);
	}

	public static CombatAbility CombatAbility(string identifier)
	{
		if (identifier != null)
		{
			CombatAbility combatAbility = Create<CombatAbility>(identifier);
			if (combatAbility != null)
			{
				return combatAbility;
			}
		}
		Debug.LogWarning("Could not find CombatAbility identifier '" + identifier + "' when attempting to create CombatAbility! Creating dummy CombatAbility instead...");
		return CombatAbilityDirectory.CreateEmptyCombatAbility(identifier);
	}

	public static CombatAbilityEffect CombatAbilityEffect(string identifier)
	{
		if (identifier != null)
		{
			CombatAbilityEffect combatAbilityEffect = Create<CombatAbilityEffect>(identifier);
			if (combatAbilityEffect != null)
			{
				return combatAbilityEffect;
			}
		}
		Debug.LogWarning("Could not find CombatAbilityEffect identifier '" + identifier + "' when attempting to create CombatAbilityEffect! Creating dummy CombatAbilityEffect instead...");
		return CombatAbilityEffectDirectory.CreateEmptyCombatAbilityEffect(identifier);
	}

	public static GameCharacterItem CharacterItem<K>(K identifier) where K : Enum
	{
		K val;
		if (identifier != null)
		{
			GameCharacterItem gameCharacterItem = Create<GameCharacterItem, K>(identifier);
			if (gameCharacterItem != null)
			{
				List<GameItem> list = GraphUtils.FindAllChildrenType<GameItem>(gameCharacterItem);
				if (list.Any())
				{
					val = identifier;
					Debug.LogWarning("CharacterItem '" + val?.ToString() + "' created with DirectoryMaster contains child items, this should not happen as it might cause problems during saving and loading! Removing all child items as a fix...");
					foreach (GameItem item in list)
					{
						item.Destroy();
					}
				}
				return gameCharacterItem;
			}
		}
		val = identifier;
		Debug.LogWarning("Could not find GameCharacterItem identifier '" + val?.ToString() + "' when attempting to create GameCharacterItem! Creating dummy GameCharacterItem instead...");
		return CharacterItemDirectory.CreateEmptyCharacterItem(identifier);
	}

	public static GameItem Item<K>(K identifier) where K : Enum
	{
		K val;
		if (identifier != null)
		{
			GameItem gameItem = Create<GameItem, K>(identifier);
			if (gameItem != null)
			{
				List<GameItem> list = GraphUtils.FindAllChildrenType<GameItem>(gameItem);
				if (list.Any())
				{
					val = identifier;
					Debug.LogWarning("GameItem '" + val?.ToString() + "' created with DirectoryMaster contains child items, this should not happen as it might cause problems during saving and loading! Removing all child items as a fix...");
					foreach (GameItem item in list)
					{
						item.Destroy();
					}
				}
				return gameItem;
			}
		}
		val = identifier;
		Debug.LogWarning("Could not find GameItem identifier '" + val?.ToString() + "' when attempting to create GameItem! Creating dummy GameItem instead...");
		return ItemDirectory.CreateEmptyItem(identifier);
	}

	public static CombatAbility CombatAbility<K>(K identifier) where K : Enum
	{
		if (identifier != null)
		{
			CombatAbility combatAbility = Create<CombatAbility, K>(identifier);
			if (combatAbility != null)
			{
				return combatAbility;
			}
		}
		K val = identifier;
		Debug.LogWarning("Could not find CombatAbility identifier '" + val?.ToString() + "' when attempting to create CombatAbility! Creating dummy CombatAbility instead...");
		return CombatAbilityDirectory.CreateEmptyCombatAbility(identifier);
	}

	public static CombatAbilityEffect CombatAbilityEffect<K>(K identifier) where K : Enum
	{
		if (identifier != null)
		{
			CombatAbilityEffect combatAbilityEffect = Create<CombatAbilityEffect, K>(identifier);
			if (combatAbilityEffect != null)
			{
				return combatAbilityEffect;
			}
		}
		K val = identifier;
		Debug.LogWarning("Could not find CombatAbilityEffect identifier '" + val?.ToString() + "' when attempting to create CombatAbilityEffect! Creating dummy CombatAbilityEffect instead...");
		return CombatAbilityEffectDirectory.CreateEmptyCombatAbilityEffect(identifier);
	}
}
