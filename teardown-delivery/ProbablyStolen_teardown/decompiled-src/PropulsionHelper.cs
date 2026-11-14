using System.Collections.Generic;

public class PropulsionHelper
{
	public const int BASE_SPEED = 10;

	public const int BASE_FUEL_CONSUMPTION = 10;

	public static bool HasEnoughFuel(int distance)
	{
		GameItem activePropulsionSystem = GetActivePropulsionSystem();
		int @int = activePropulsionSystem.GetTagReadonly("PROPULSION_FUEL_DRAIN_INT").GetInt();
		List<GameItem> list = GraphUtils.FindAllChildrenType(activePropulsionSystem, (GameItem item) => item.identifier.Equals("exotic_fuel"));
		if (list.Count <= 0)
		{
			return false;
		}
		if (list[0].unitCount * @int >= distance)
		{
			return true;
		}
		return false;
	}

	public static bool RemoveFuel(int fuelUnitToRemove)
	{
		List<GameItem> list = GraphUtils.FindAllChildrenType(GetActivePropulsionSystem(), (GameItem item) => item.identifier.Equals("exotic_fuel"));
		if (list[0].unitCount < fuelUnitToRemove)
		{
			return false;
		}
		list[0].SetUnitCount(list[0].unitCount - fuelUnitToRemove);
		return true;
	}

	public static void SetPropulsionTarget(GameItem propulsionSystem, int distanceToTarget)
	{
		propulsionSystem.ModifyTag("PROPULSION_PROGRESS_CURRENT_INT", delegate(TagState state)
		{
			state.SetInt(0);
		});
		propulsionSystem.ModifyTag("PROPULSION_DISTANCE_INT", delegate(TagState state)
		{
			state.SetInt(distanceToTarget);
		});
	}

	public static bool UpdatePropulsion(GameItem propulsionSystem)
	{
		bool result = false;
		int propulsionSpeed = GetPropulsionSpeed(propulsionSystem);
		int @int = propulsionSystem.GetTagReadonly("PROPULSION_PROGRESS_CURRENT_INT").GetInt();
		int int2 = propulsionSystem.GetTagReadonly("PROPULSION_DISTANCE_INT").GetInt();
		int newProgress = @int + propulsionSpeed;
		if (newProgress >= int2)
		{
			newProgress = int2;
			result = true;
		}
		propulsionSystem.ModifyTag("PROPULSION_PROGRESS_CURRENT_INT", delegate(TagState state)
		{
			state.SetInt(newProgress);
		});
		return result;
	}

	public static int GetPropulsionSpeed(GameItem propulsionSystem)
	{
		return propulsionSystem.GetTagReadonly("PROPULSION_SPEED_INT").GetInt();
	}

	public static void CreatePropulsionSystemTooltip(RichTextBuilder builder, GameItem item)
	{
		if (item.IsTag("SYSTEM_TAG_PROPULSION"))
		{
			int @int = item.GetTagReadonly("PROPULSION_SPEED_INT").GetInt();
			int int2 = item.GetTagReadonly("PROPULSION_FUEL_DRAIN_INT").GetInt();
			builder.AddLine("Ship Speed: " + @int + " Units/Turn", ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
			builder.AddLine("Fuel Use: " + int2 + " Units/Fuel Cell", ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
			MachineryHelper.AddTooltipModuleBonus(builder, item, LocHelper.GetLocalizedMechanic("mech_module_stat_travel_speed"), LocHelper.GetLocalizedMechanic("mech_module_stat_fuel_consumption"));
		}
	}

	public static GameItem GetActivePropulsionSystem()
	{
		foreach (GameItem item in GraphUtils.FindAllChildrenType<GameItem>(GameMaster.current.shipManager.ship))
		{
			if (item.IsTag("SYSTEM_TAG_PROPULSION") && item.IsTag("IN_SLOT_BOOL"))
			{
				return item;
			}
		}
		return null;
	}
}
