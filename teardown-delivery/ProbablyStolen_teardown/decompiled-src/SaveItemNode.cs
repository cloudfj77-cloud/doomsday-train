using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveItemNode : ISerializationCallbackReceiver
{
	public string identifier;

	public int unitCount;

	public long uuid;

	public TagSystem itemState;

	public GridShapeBuilder itemShape;

	public GridShapeBuilder itemModifiedShape;

	public string itemType;

	public string name;

	public string shortDescription;

	public string flavorText;

	public int uniqueId;

	public long unitBaseValue;

	public long unitValue;

	public long lateUnitValue;

	public long backupUnitValue;

	public string spriteAtlasPath;

	public string spritePath;

	public List<string> itemTypes = new List<string>();

	public List<long> childItems = new List<long>();

	public List<int> childItemInventoryNode = new List<int>();

	[SerializeField]
	private List<string> _keys = new List<string>();

	[SerializeField]
	private List<TagState> _values = new List<TagState>();

	public GameItem tempLink;

	public void OnBeforeSerialize()
	{
		_keys.Clear();
		_values.Clear();
		foreach (KeyValuePair<string, TagState> item in itemState.dict)
		{
			TagState value = item.Value;
			if (value.valueEnabled || value.valueInt != 0 || value.valueLong != 0L || value.valueFloat != 0f || value.valueDouble != 0.0 || value.valueBool || !string.IsNullOrEmpty(value.GetString()))
			{
				_keys.Add(item.Key);
				_values.Add(value);
			}
		}
	}

	public void OnAfterDeserialize()
	{
		itemState = new TagSystem();
		for (int i = 0; i < Math.Min(_keys.Count, _values.Count); i++)
		{
			itemState.dict.Add(_keys[i], _values[i]);
		}
	}
}
