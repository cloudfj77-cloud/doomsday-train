using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewGamePerkTooltip : MonoBehaviour
{
	private NewGamePanelFrame frame;

	private RectTransform rect;

	private Image outer;

	private Image fill;

	private Image border;

	private TextMeshProUGUI nameText;

	private TextMeshProUGUI costText;

	private Image headerRule;

	private RectTransform linesHost;

	private Image lockRule;

	private Image lockIcon;

	private TextMeshProUGUI lockText;

	private readonly List<GameObject> lineObjects = new List<GameObject>();

	public bool IsShown => base.gameObject.activeSelf;

	public static NewGamePerkTooltip Create(NewGamePanelFrame frame, Transform host, Sprite lockSprite)
	{
		GameObject gameObject = new GameObject("PerkTooltip", typeof(RectTransform));
		gameObject.transform.SetParent(host, worldPositionStays: false);
		NewGamePerkTooltip newGamePerkTooltip = gameObject.AddComponent<NewGamePerkTooltip>();
		newGamePerkTooltip.frame = frame;
		newGamePerkTooltip.Build(lockSprite);
		gameObject.SetActive(value: false);
		return newGamePerkTooltip;
	}

	private void Build(Sprite lockSprite)
	{
		rect = base.transform as RectTransform;
		rect.anchorMin = new Vector2(0f, 0f);
		rect.anchorMax = new Vector2(0f, 0f);
		rect.pivot = new Vector2(0f, 0f);
		rect.sizeDelta = new Vector2(512f, 100f);
		float num = 512f;
		float num2 = 2f;
		outer = NewGameUIElements.NewImage(rect, "Outer", 0f, 0f, num, 100f, null, NewGameStyle.PERK_TOOLTIP_OUTER);
		fill = NewGameUIElements.NewImage(rect, "Fill", num2, num2, num - 2f * num2, 100f, null, NewGameStyle.PERK_TOOLTIP_BG);
		border = NewGameUIElements.NewImage(rect, "Border", num2, num2, num - 2f * num2, 100f, frame.frame2Square, NewGameStyle.PERK_TOOLTIP_BORDER_COLOR);
		float num3 = 16f;
		float num4 = 480f;
		nameText = NewGameUIElements.NewText(frame, rect, "Name", num3, num3, num4 - 204f, 28f, FontLoader.FontRole.Lubrifont, 24f, NewGameStyle.PERK_TOOLTIP_NAME, TextAlignmentOptions.MidlineLeft, wrap: false);
		WildNetworkUI.AutoSize(nameText, 14f);
		costText = NewGameUIElements.NewText(frame, rect, "Cost", num3 + num4 - 204f, num3, 204f, 28f, FontLoader.FontRole.Noto_Sans, 18f, NewGameStyle.PERK_TOOLTIP_COST, TextAlignmentOptions.MidlineRight, wrap: false);
		WildNetworkUI.AutoSize(costText, 12f);
		headerRule = NewGameUIElements.NewImage(rect, "HeaderRule", num3, 0f, num4, 2f, null, NewGameStyle.PERK_TOOLTIP_RULE);
		linesHost = NewGameUIElements.NewRect(rect, "Lines", num3, 0f, num4, 0f);
		lockRule = NewGameUIElements.NewImage(rect, "LockRule", num3, 0f, num4, 2f, null, NewGameStyle.PERK_TOOLTIP_RULE);
		lockIcon = NewGameUIElements.NewImage(rect, "LockIcon", num3, 0f, 16f, 18f, lockSprite, Color.white);
		float num5 = num3 + 16f + 8f;
		lockText = NewGameUIElements.NewText(frame, rect, "LockText", num5, 0f, num3 + num4 - num5, 22f, FontLoader.FontRole.Noto_Sans_Normal, 16f, NewGameStyle.PERK_TOOLTIP_LOCKED, TextAlignmentOptions.MidlineLeft, wrap: false);
		WildNetworkUI.AutoSize(lockText, 12f);
	}

	public void Show(StartingPerk perk, StartingPerk lockedBy)
	{
		if (perk == null)
		{
			return;
		}
		base.gameObject.SetActive(value: true);
		nameText.text = perk.GetLocalizedDisplayName();
		costText.text = CostLabel(perk.cost);
		float num = 16f;
		float num2 = 480f;
		float num3 = num;
		num3 += 36f;
		WildNetworkUI.SetRect(headerRule.rectTransform, num, num3, num2, 2f);
		num3 += 10f;
		foreach (GameObject lineObject in lineObjects)
		{
			Object.Destroy(lineObject);
		}
		lineObjects.Clear();
		float num4 = 8f;
		float num5 = num4 + 4f + 8f;
		float width = num2 - num5 - num4;
		float num6 = num4;
		string[] array = (perk.GetLocalizedDescription() ?? "").Split('\n');
		bool flag = false;
		string[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			string text = array2[i].Trim();
			if (text.Length != 0)
			{
				if (flag)
				{
					num6 += 16f;
				}
				flag = true;
				TextMeshProUGUI textMeshProUGUI = NewGameUIElements.NewText(frame, linesHost, "Line", num5, num6, width, 18f, FontLoader.FontRole.Noto_Sans_Normal, 18f, NewGameStyle.PERK_TOOLTIP_LINE, TextAlignmentOptions.TopLeft, wrap: true);
				textMeshProUGUI.text = text;
				float num7 = Mathf.Ceil(textMeshProUGUI.GetPreferredValues(text, width, float.PositiveInfinity).y);
				WildNetworkUI.SetRect(textMeshProUGUI.rectTransform, num5, num6, width, num7);
				Image image = NewGameUIElements.NewImage(linesHost, "Bullet", num4, num6 + Mathf.Floor((num7 - 4f) * 0.5f), 4f, 4f, null, NewGameStyle.PERK_TOOLTIP_BULLET);
				lineObjects.Add(textMeshProUGUI.gameObject);
				lineObjects.Add(image.gameObject);
				num6 += num7;
			}
		}
		float num8 = num6 + num4;
		WildNetworkUI.SetRect(linesHost, num, num3, num2, num8);
		num3 += num8;
		bool flag2 = lockedBy != null;
		lockRule.gameObject.SetActive(flag2);
		lockIcon.gameObject.SetActive(flag2);
		lockText.gameObject.SetActive(flag2);
		if (flag2)
		{
			num3 += 8f;
			WildNetworkUI.SetRect(lockRule.rectTransform, num, num3, num2, 2f);
			num3 += 10f;
			WildNetworkUI.SetRect(lockIcon.rectTransform, num, num3 + 2f, 16f, 18f);
			float num9 = num + 16f + 8f;
			WildNetworkUI.SetRect(lockText.rectTransform, num9, num3, num + num2 - num9, 22f);
			lockText.text = LocHelper.GetLocalizedOrDefault("MainMenu", "new_perk_locked_by", "Locked by perk: {0}", lockedBy.GetLocalizedDisplayName());
			num3 += 22f;
		}
		float num10 = num3 + num;
		float num11 = 512f;
		float num12 = 2f;
		rect.sizeDelta = new Vector2(num11, num10);
		WildNetworkUI.SetRect(outer.rectTransform, 0f, 0f, num11, num10);
		WildNetworkUI.SetRect(fill.rectTransform, num12, num12, num11 - 2f * num12, num10 - 2f * num12);
		WildNetworkUI.SetRect(border.rectTransform, num12, num12, num11 - 2f * num12, num10 - 2f * num12);
		base.transform.SetAsLastSibling();
		Follow();
	}

	public void Hide()
	{
		base.gameObject.SetActive(value: false);
	}

	private static string CostLabel(int cost)
	{
		return ((cost > 0) ? ("-" + cost) : ((cost < 0) ? ("+" + -cost) : "0")) + " " + LocHelper.GetLocalizedPerkTable("perk_points");
	}

	private void Update()
	{
		Follow();
	}

	private void Follow()
	{
		Vector3 tooltipPositionSimple = UIHelper.GetTooltipPositionSimple();
		float num = 25f;
		float num2 = rect.rect.width * base.transform.lossyScale.x;
		float num3 = rect.rect.height * base.transform.lossyScale.y;
		if (tooltipPositionSimple.x + num2 > (float)Screen.width)
		{
			tooltipPositionSimple.x = Mathf.Max(0f, Input.mousePosition.x - num - num2);
		}
		if (tooltipPositionSimple.y + num3 > (float)Screen.height)
		{
			tooltipPositionSimple.y = Mathf.Max(0f, Input.mousePosition.y - num - num3);
		}
		tooltipPositionSimple.z = base.transform.position.z;
		base.transform.position = tooltipPositionSimple;
	}
}
