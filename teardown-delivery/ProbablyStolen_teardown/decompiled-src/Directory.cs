using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Directory<T> : MonoBehaviour where T : DirectoryEntry
{
	public Dictionary<string, Func<T>> factoryDictionary { get; protected set; }

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
	private void Awake()
	{
		factoryDictionary = new Dictionary<string, Func<T>>();
		if (DirectoryMaster.AddDirectory(this))
		{
			InitDirectory();
		}
	}

	private void OnApplicationQuit()
	{
		DirectoryMaster.Reset();
	}

	public T Create(string identifier)
	{
		if (!factoryDictionary.ContainsKey(identifier))
		{
			return default(T);
		}
		T val = factoryDictionary[identifier]();
		if (val != null)
		{
			val.identifier = identifier;
			val.identifierName = TagSystem.StringToDisplayString(identifier);
		}
		return val;
	}

	public T Create<K>(K identifierValue) where K : Enum
	{
		string text = TagSystem.EnumToString(identifierValue);
		if (!factoryDictionary.ContainsKey(text))
		{
			return default(T);
		}
		T val = factoryDictionary[text]();
		if (val != null)
		{
			val.identifier = text;
			val.identifierName = TagSystem.EnumToDisplayString(identifierValue);
		}
		return val;
	}

	public abstract T Empty();

	protected abstract void InitDirectory();

	public virtual bool Add(string identifier, Func<T> factoryFunc)
	{
		if (identifier == null || factoryFunc == null)
		{
			return false;
		}
		if (DirectoryMaster.Remove<T>(identifier))
		{
			Debug.LogWarning("Trying to add " + typeof(T)?.ToString() + " with identifier '" + identifier + "' when it already exists in DirectoryMaster, ignoring new entry.");
		}
		return factoryDictionary.TryAdd(identifier, factoryFunc);
	}

	public virtual bool Set(string identifier, Func<T> factoryFunc)
	{
		if (identifier == null || factoryFunc == null)
		{
			return false;
		}
		if (DirectoryMaster.Remove<T>(identifier))
		{
			Debug.LogWarning("Trying to set " + typeof(T)?.ToString() + " with identifier '" + identifier + "' when it already exists in DirectoryMaster, replacing old entry with new entry.");
		}
		Remove(identifier);
		return factoryDictionary.TryAdd(identifier, factoryFunc);
	}

	public virtual bool Has(string identifier)
	{
		if (identifier == null)
		{
			return false;
		}
		return factoryDictionary.ContainsKey(identifier);
	}

	public virtual bool Remove(string identifier)
	{
		if (identifier == null)
		{
			return false;
		}
		return factoryDictionary.Remove(identifier);
	}

	public virtual bool Add<K>(K identifierValue, Func<T> factoryFunc) where K : Enum
	{
		if (identifierValue == null || factoryFunc == null)
		{
			return false;
		}
		string identifier = TagSystem.EnumToString(identifierValue);
		return Add(identifier, factoryFunc);
	}

	public virtual bool Set<K>(K identifierValue, Func<T> factoryFunc) where K : Enum
	{
		if (identifierValue == null || factoryFunc == null)
		{
			return false;
		}
		string identifier = TagSystem.EnumToString(identifierValue);
		return Set(identifier, factoryFunc);
	}

	public virtual bool Has<K>(K identifierValue) where K : Enum
	{
		if (identifierValue == null)
		{
			return false;
		}
		string identifier = TagSystem.EnumToString(identifierValue);
		return Has(identifier);
	}

	public virtual bool Remove<K>(K identifierValue) where K : Enum
	{
		if (identifierValue == null)
		{
			return false;
		}
		string identifier = TagSystem.EnumToString(identifierValue);
		return Remove(identifier);
	}
}
