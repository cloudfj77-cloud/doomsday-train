using System;

public class ItemStateModifier : TagState
{
	public enum Operation
	{
		Set,
		Clear,
		Add,
		Mult,
		Enable,
		Disable,
		Toggle,
		SetEnabled,
		Destroy
	}

	public enum Condition
	{
		Any,
		IfEnabled,
		IfDisabled,
		IfExist,
		IfNotExist
	}

	public readonly Operation operation;

	public readonly Condition condition;

	public readonly int priority;

	public new ItemStateModifier SetString(string val)
	{
		base.valueString = val;
		return this;
	}

	public new ItemStateModifier SetInt(int val)
	{
		valueInt = val;
		return this;
	}

	public new ItemStateModifier SetFloat(float val)
	{
		valueFloat = val;
		return this;
	}

	public new ItemStateModifier SetLong(long val)
	{
		valueLong = val;
		return this;
	}

	public new ItemStateModifier SetDouble(double val)
	{
		valueDouble = val;
		return this;
	}

	public new ItemStateModifier SetBool(bool val)
	{
		valueBool = val;
		return this;
	}

	public new ItemStateModifier SetEnabled(bool val)
	{
		valueEnabled = val;
		return this;
	}

	public static ItemStateModifier Create(string key, Operation operation = Operation.Enable, Condition condition = Condition.Any, int priority = 0)
	{
		return new ItemStateModifier(key, key, operation, condition, priority);
	}

	public static ItemStateModifier Create(long keyValue, Operation operation = Operation.Enable, Condition condition = Condition.Any, int priority = 0)
	{
		string text = TagSystem.LongToString(keyValue);
		return new ItemStateModifier(text, text, operation, condition, priority);
	}

	public static ItemStateModifier Create<T>(T keyValue, Operation operation = Operation.Enable, Condition condition = Condition.Any, int priority = 0) where T : Enum
	{
		return new ItemStateModifier(TagSystem.EnumToString(keyValue), TagSystem.EnumToDisplayString(keyValue), operation, condition, priority);
	}

	private ItemStateModifier(string key, string displayKey, Operation operation, Condition condition, int priority)
		: base(key, displayKey)
	{
		this.operation = operation;
		this.condition = condition;
		this.priority = priority;
	}

	private void ApplySet(TagState state)
	{
		state.valueString = base.valueString;
		state.valueInt = valueInt;
		state.valueFloat = valueFloat;
		state.valueLong = valueLong;
		state.valueDouble = valueDouble;
		state.valueBool = valueBool;
	}

	private void ApplyAdd(TagState state)
	{
		state.valueString += base.valueString;
		state.valueInt += valueInt;
		state.valueFloat += valueFloat;
		state.valueLong += valueLong;
		state.valueDouble += valueDouble;
		state.valueBool |= valueBool;
	}

	private void ApplyMult(TagState state)
	{
		state.valueString += base.valueString;
		state.valueInt *= valueInt;
		state.valueFloat *= valueFloat;
		state.valueLong *= valueLong;
		state.valueDouble *= valueDouble;
		state.valueBool &= valueBool;
	}

	public bool Apply(TagSystem system)
	{
		if (base.identifier == null)
		{
			return false;
		}
		if (system == null)
		{
			return false;
		}
		if (condition == Condition.IfEnabled && !system.IsTag(base.identifier))
		{
			return false;
		}
		if (condition == Condition.IfDisabled && system.IsTag(base.identifier))
		{
			return false;
		}
		if (condition == Condition.IfExist && !system.dict.ContainsKey(base.identifier))
		{
			return false;
		}
		if (condition == Condition.IfNotExist && system.dict.ContainsKey(base.identifier))
		{
			return false;
		}
		switch (operation)
		{
		case Operation.Set:
		{
			TagState tag = system.GetTag(base.identifier);
			ApplySet(tag);
			break;
		}
		case Operation.Clear:
			if (!system.dict.ContainsKey(base.identifier))
			{
				return false;
			}
			system.GetTag(base.identifier).Reset();
			break;
		case Operation.Add:
		{
			TagState tag = system.GetTag(base.identifier);
			ApplyAdd(tag);
			break;
		}
		case Operation.Mult:
		{
			TagState tag = system.GetTag(base.identifier);
			ApplyMult(tag);
			break;
		}
		case Operation.Enable:
			if (system.IsTag(base.identifier))
			{
				return false;
			}
			system.EnableTag(base.identifier);
			break;
		case Operation.Disable:
			if (!system.dict.ContainsKey(base.identifier))
			{
				return false;
			}
			system.DisableTag(base.identifier);
			break;
		case Operation.Toggle:
			system.ToggleTag(base.identifier);
			break;
		case Operation.SetEnabled:
			system.GetTag(base.identifier).SetEnabled(valueEnabled);
			break;
		case Operation.Destroy:
			if (!system.dict.ContainsKey(base.identifier))
			{
				return false;
			}
			system.GetTag(base.identifier).Destroy();
			break;
		default:
			return false;
		}
		return true;
	}
}
