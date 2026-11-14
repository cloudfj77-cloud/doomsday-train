using System;

public class SecuredContainerHelper
{
	public const string LOCK_TYPE = "LOCK_TYPE";

	public const string UNLOCK_TYPE = "UNLOCK_TYPE";

	public const string UNLOCK_BREACHING = "UNLOCK_BREACHING";

	public const string UNLOCK_DURABILITY = "UNLOCK_DURABILITY";

	public const string LOCK_TYPE_MECHANICAL = "LOCK_TYPE_MECHANICAL";

	public const string LOCK_TYPE_ELECTRONIC = "LOCK_TYPE_ELECTRONIC";

	public const string LOCK_DIFFICULTY = "LOCK_DIFFICULTY";

	public const string LOCK_COMPANY_ACCESS = "LOCK_COMPANY_ACCESS";

	public static void InitSecuredContainer(GameItem item, string lockType, int difficulty, bool startLocked = true, string company = "none")
	{
		if (startLocked)
		{
			item.contentWindow.Lock();
		}
		item.ModifyTag("LOCK_DIFFICULTY", delegate(TagState state)
		{
			state.Enable().SetInt(difficulty);
		});
		item.ModifyTag("LOCK_TYPE", delegate(TagState state)
		{
			state.Enable().SetString(lockType);
		});
		if (!company.Equals("none"))
		{
			item.ModifyTag("LOCK_COMPANY_ACCESS", delegate(TagState state)
			{
				state.Enable().SetString(company);
			});
			item.EnableManufacturer(company);
		}
	}

	public static void InitKey(GameItem item, string unlockType, int difficulty, bool isBreaching, string company = "none", int durability = -1)
	{
		item.ModifyTag("LOCK_DIFFICULTY", delegate(TagState state)
		{
			state.Enable().SetInt(difficulty);
		});
		item.ModifyTag("UNLOCK_TYPE", delegate(TagState state)
		{
			state.Enable().SetString(unlockType);
		});
		item.ModifyTag("LOCK_COMPANY_ACCESS", delegate(TagState state)
		{
			state.Enable().SetString(company);
		});
		item.ModifyTag("UNLOCK_BREACHING", delegate(TagState state)
		{
			state.Enable().SetBool(isBreaching);
		});
		if (durability != -1)
		{
			item.ModifyTag("UNLOCK_DURABILITY", delegate(TagState state)
			{
				state.Enable().SetInt(durability);
			});
		}
		item.maySelectSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		item.canSelectSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => true;
		item.mayThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(item.mayThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => CanUnlock(t, s)));
		item.canThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(item.canThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => CanUnlock(t, s)));
		item.onThisTargetItemFunc = (Action<GameItem, GameItem>)Delegate.Combine(item.onThisTargetItemFunc, (Action<GameItem, GameItem>)delegate(GameItem s, GameItem t)
		{
			TryUnlock(t, s);
		});
		if (!company.Equals("none"))
		{
			item.EnableManufacturer(company);
		}
	}

	public static bool CanUnlock(GameItem container, GameItem key)
	{
		if (!container.IsTag("LOCK_TYPE"))
		{
			return false;
		}
		string @string = key.GetTagReadonly("UNLOCK_TYPE").GetString();
		int @int = key.GetTagReadonly("LOCK_DIFFICULTY").GetInt();
		string string2 = key.GetTagReadonly("LOCK_COMPANY_ACCESS").GetString();
		bool @bool = key.GetTagReadonly("UNLOCK_BREACHING").GetBool();
		int int2 = key.GetTagReadonly("UNLOCK_DURABILITY").GetInt();
		string string3 = container.GetTagReadonly("LOCK_TYPE").GetString();
		int int3 = container.GetTagReadonly("LOCK_DIFFICULTY").GetInt();
		string string4 = container.GetTagReadonly("LOCK_COMPANY_ACCESS").GetString();
		if (key.IsTag("UNLOCK_DURABILITY") && int2 < 0)
		{
			return false;
		}
		if (!@string.Equals(string3))
		{
			return false;
		}
		if (@int < int3)
		{
			return false;
		}
		if (!@bool && !string2.Equals(string4))
		{
			return false;
		}
		return true;
	}

	public static bool TryUnlock(GameItem container, GameItem key)
	{
		if (CanUnlock(container, key))
		{
			if (key.IsTag("UNLOCK_DURABILITY"))
			{
				int keyDurability = key.GetTagReadonly("UNLOCK_DURABILITY").GetInt() - 1;
				if (keyDurability <= 0)
				{
					key.Destroy();
				}
				else
				{
					key.ModifyTag("UNLOCK_DURABILITY", delegate(TagState state)
					{
						state.Enable().SetInt(keyDurability);
					});
				}
			}
			container.contentWindow.Unlock();
			return true;
		}
		return false;
	}

	public static void CreateSecuredContainerToolTip(RichTextBuilder builder, GameItem container)
	{
		if (!container.IsTag("LOCK_TYPE"))
		{
			return;
		}
		string @string = container.GetTagReadonly("LOCK_TYPE").GetString();
		int @int = container.GetTagReadonly("LOCK_DIFFICULTY").GetInt();
		container.GetTagReadonly("LOCK_COMPANY_ACCESS").GetString();
		builder.AddLine("Required Access Level: " + @int, ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
		if (@string.Equals("LOCK_TYPE_ELECTRONIC"))
		{
			builder.AddLine("Electronic Lock", ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
		}
		else
		{
			builder.AddLine("Mechanical Lock", ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
		}
		if (container.contentWindow != null)
		{
			if (container.contentWindow.isLocked)
			{
				builder.AddLine("Locked", ignoreEmpty: true, RenderHandler.ColorPalette.LightRed);
			}
			else
			{
				builder.AddLine("Unlocked", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
			}
		}
	}

	public static void CreateKeyItemTip(RichTextBuilder builder, GameItem key)
	{
		if (key.IsTag("UNLOCK_TYPE"))
		{
			key.GetTagReadonly("UNLOCK_TYPE").GetString();
			key.GetTagReadonly("LOCK_DIFFICULTY").GetInt();
			key.GetTagReadonly("LOCK_COMPANY_ACCESS").GetString();
			key.GetTagReadonly("UNLOCK_BREACHING").GetBool();
			int @int = key.GetTagReadonly("UNLOCK_DURABILITY").GetInt();
			if (key.IsTag("UNLOCK_DURABILITY"))
			{
				builder.AddLine("Durability: " + @int, ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
			}
		}
	}
}
