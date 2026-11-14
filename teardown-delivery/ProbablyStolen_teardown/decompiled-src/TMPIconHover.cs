using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TMPIconHover : MonoBehaviour, IPointerMoveHandler, IEventSystemHandler, IPointerExitHandler
{
	[SerializeField]
	private TMP_Text tmpText;

	[SerializeField]
	private Camera uiCamera;

	public static readonly Dictionary<string, Func<string>> CustomLinks = new Dictionary<string, Func<string>>();

	private string currentHoveredLink;

	private void Awake()
	{
		if (tmpText == null)
		{
			tmpText = GetComponent<TMP_Text>();
		}
	}

	public void OnPointerMove(PointerEventData eventData)
	{
		if (tmpText == null)
		{
			return;
		}
		Camera camera = uiCamera;
		Canvas canvas = ((tmpText != null) ? tmpText.canvas : null);
		if (canvas != null)
		{
			Canvas rootCanvas = canvas.rootCanvas;
			camera = ((rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay) ? null : rootCanvas.worldCamera);
		}
		int num = TMP_TextUtilities.FindIntersectingLink(tmpText, eventData.position, camera);
		if (num == -1)
		{
			if (currentHoveredLink != null)
			{
				ExitLink();
			}
			return;
		}
		TMP_LinkInfo tMP_LinkInfo = tmpText.textInfo.linkInfo[num];
		string linkID = tMP_LinkInfo.GetLinkID();
		if (linkID != currentHoveredLink)
		{
			if (currentHoveredLink != null)
			{
				ExitLink();
			}
			currentHoveredLink = linkID;
			EnterLink(linkID);
		}
		else
		{
			StayLink(linkID);
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (currentHoveredLink != null)
		{
			ExitLink();
		}
	}

	private void OnDisable()
	{
		if (currentHoveredLink != null)
		{
			ExitLink();
		}
	}

	private void EnterLink(string id)
	{
		if (!(TooltipManager._instance == null))
		{
			TooltipManager._instance.SetAndShowTooltip(ResolveLinkText(id), this);
		}
	}

	private static string ResolveLinkText(string id)
	{
		if (CustomLinks.TryGetValue(id, out var value) && value != null)
		{
			try
			{
				return value() ?? "";
			}
			catch (Exception arg)
			{
				Debug.LogError($"[TMPIconHover] Tooltip provider for '{id}' threw: {arg}");
				return "";
			}
		}
		return LocHelper.GetLocalizedUI(id);
	}

	private void StayLink(string id)
	{
	}

	private void ExitLink()
	{
		currentHoveredLink = null;
		if (TooltipManager._instance != null)
		{
			TooltipManager._instance.HideTooltip();
		}
	}
}
