using System.Collections.Generic;
using UnityEngine;

public class ItemBehaviourManager : MonoBehaviour
{
	public static ItemBehaviourManager current { get; private set; }

	public List<ItemUseBehaviour> useBehaviourList { get; } = new List<ItemUseBehaviour>();


	public List<ItemTargetBehaviour> targetBehaviourList { get; } = new List<ItemTargetBehaviour>();


	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
	private void Awake()
	{
		current = this;
		PriorityTrigger.InitBehaviours(useBehaviourList);
		PriorityTrigger.InitBehaviours(targetBehaviourList);
	}

	public bool MayUse(GameItem source)
	{
		foreach (ItemUseBehaviour useBehaviour in useBehaviourList)
		{
			try
			{
				if (!useBehaviour.CheckFilter(source) || !useBehaviour.CheckMay(source))
				{
					continue;
				}
				return true;
			}
			catch
			{
			}
		}
		return false;
	}

	public bool CanUse(GameItem source)
	{
		foreach (ItemUseBehaviour useBehaviour in useBehaviourList)
		{
			try
			{
				if (!useBehaviour.CheckFilter(source) || !useBehaviour.CheckCan(source))
				{
					continue;
				}
				return true;
			}
			catch
			{
			}
		}
		return false;
	}

	public void Use(GameItem source)
	{
		foreach (ItemUseBehaviour useBehaviour in useBehaviourList)
		{
			try
			{
				if (useBehaviour.CheckFilter(source) && useBehaviour.CheckCan(source))
				{
					useBehaviour.OnUse(source);
				}
			}
			catch
			{
			}
		}
	}

	public RichTextBuilder UseTooltip(GameItem source)
	{
		RichTextBuilder richTextBuilder = null;
		foreach (ItemUseBehaviour useBehaviour in useBehaviourList)
		{
			try
			{
				if (!useBehaviour.CheckFilter(source))
				{
					continue;
				}
				RichTextBuilder tooltip = useBehaviour.GetTooltip(source);
				if (tooltip != null)
				{
					if (richTextBuilder == null)
					{
						richTextBuilder = tooltip;
					}
					else
					{
						richTextBuilder.Add(tooltip);
					}
				}
			}
			catch
			{
			}
		}
		return richTextBuilder;
	}

	public bool MayTarget(GameItem source, GameItem target)
	{
		foreach (ItemTargetBehaviour targetBehaviour in targetBehaviourList)
		{
			try
			{
				if (!targetBehaviour.CheckFilter(source, target) || !targetBehaviour.CheckMay(source, target))
				{
					continue;
				}
				return true;
			}
			catch
			{
			}
		}
		return false;
	}

	public bool CanTarget(GameItem source, GameItem target)
	{
		foreach (ItemTargetBehaviour targetBehaviour in targetBehaviourList)
		{
			try
			{
				if (!targetBehaviour.CheckFilter(source, target) || !targetBehaviour.CheckCan(source, target))
				{
					continue;
				}
				return true;
			}
			catch
			{
			}
		}
		return false;
	}

	public void Target(GameItem source, GameItem target)
	{
		foreach (ItemTargetBehaviour targetBehaviour in targetBehaviourList)
		{
			try
			{
				if (targetBehaviour.CheckFilter(source, target) && targetBehaviour.CheckCan(source, target))
				{
					targetBehaviour.OnTarget(source, target);
				}
			}
			catch
			{
			}
		}
	}

	public RichTextBuilder TargetTooltip(GameItem source, GameItem target)
	{
		RichTextBuilder richTextBuilder = null;
		foreach (ItemTargetBehaviour targetBehaviour in targetBehaviourList)
		{
			try
			{
				if (!targetBehaviour.CheckFilter(source, target))
				{
					continue;
				}
				RichTextBuilder tooltip = targetBehaviour.GetTooltip(source, target);
				if (tooltip != null)
				{
					if (richTextBuilder == null)
					{
						richTextBuilder = tooltip;
					}
					else
					{
						richTextBuilder.Add(tooltip);
					}
				}
			}
			catch
			{
			}
		}
		return richTextBuilder;
	}
}
