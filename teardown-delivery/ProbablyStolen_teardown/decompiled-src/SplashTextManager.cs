using System.Collections;
using UnityEngine;

public class SplashTextManager
{
	public enum TextMode
	{
		Normal,
		Stun,
		Miss,
		Bleed,
		Buffed,
		Debuffed,
		Heal,
		ArmorDamage,
		Damage
	}

	private static SplashTextManager instance = null;

	private static readonly object padlock = new object();

	public static GameObject floatText;

	public static SplashTextManager Instance
	{
		get
		{
			lock (padlock)
			{
				if (instance == null)
				{
					instance = new SplashTextManager();
				}
				return instance;
			}
		}
	}

	public void DisplayFloatText(CombatEntity entity, string text, TextMode textmode = TextMode.Normal)
	{
		TreeNodeRender treeNodeRender = TreeNodeRender.InstantiateText().SetParent(RenderHandler.current.overlayRoot);
		TreeNodeRender treeNodeRender2 = TreeNodeRender.InstantiateText().SetParent(RenderHandler.current.overlayRoot);
		TreeNodeRender treeNodeRender3 = TreeNodeRender.InstantiateText().SetParent(RenderHandler.current.overlayRoot);
		treeNodeRender3.text.text = text;
		treeNodeRender.text.text = text;
		treeNodeRender2.text.text = text;
		if (textmode != 0)
		{
			treeNodeRender3.text.color = RetrieveColor(textmode);
		}
		GameCharacterItem gameCharacterItem = (GameCharacterItem)entity;
		if (gameCharacterItem != null)
		{
			Vector2 rotatorCenteredPosition = gameCharacterItem.GetRotatorCenteredPosition();
			treeNodeRender3.text.enabled = true;
			rotatorCenteredPosition.x += Random.Range(-10, 10);
			rotatorCenteredPosition.y += Random.Range(-10, 10);
			treeNodeRender3.text.rectTransform.position = new Vector3(rotatorCenteredPosition.x, rotatorCenteredPosition.y, treeNodeRender3.text.rectTransform.position.z);
			treeNodeRender3.text.rectTransform.sizeDelta = new Vector2(1500f, 1500f);
			treeNodeRender.text.rectTransform.position = new Vector3(rotatorCenteredPosition.x - 2f, rotatorCenteredPosition.y + 2f, treeNodeRender.text.rectTransform.position.z);
			treeNodeRender.text.rectTransform.sizeDelta = new Vector2(1500f, 1500f);
			treeNodeRender2.text.enabled = true;
			treeNodeRender2.text.rectTransform.position = new Vector3(rotatorCenteredPosition.x + 2f, rotatorCenteredPosition.y - 2f, treeNodeRender2.text.rectTransform.position.z);
			treeNodeRender2.text.rectTransform.sizeDelta = new Vector2(1500f, 1500f);
			treeNodeRender.text.color = RenderHandler.GetColorFromPalette(RenderHandler.ColorPalette.VeryDarkRed);
			treeNodeRender2.text.color = RenderHandler.GetColorFromPalette(RenderHandler.ColorPalette.VeryDarkRed);
			treeNodeRender3.text.StartCoroutine(UpdateSplashText(treeNodeRender3));
			treeNodeRender.text.StartCoroutine(UpdateSplashText(treeNodeRender));
			treeNodeRender2.text.StartCoroutine(UpdateSplashText(treeNodeRender2));
			Object.Destroy(treeNodeRender3.text.gameObject, 1.5f);
			Object.Destroy(treeNodeRender.text.gameObject, 1.5f);
			Object.Destroy(treeNodeRender2.text.gameObject, 1.5f);
		}
	}

	public void DisplayFloatText(string text, TextMode textmode = TextMode.Normal)
	{
		TreeNodeRender treeNodeRender = TreeNodeRender.InstantiateText().SetParent(RenderHandler.current.overlayRoot);
		TreeNodeRender treeNodeRender2 = TreeNodeRender.InstantiateText().SetParent(RenderHandler.current.overlayRoot);
		TreeNodeRender treeNodeRender3 = TreeNodeRender.InstantiateText().SetParent(RenderHandler.current.overlayRoot);
		treeNodeRender3.text.text = text;
		treeNodeRender.text.text = text;
		treeNodeRender2.text.text = text;
		if (textmode != 0)
		{
			treeNodeRender3.text.color = RetrieveColor(textmode);
		}
		Vector2 vector = new Vector2(LetterboxManager.GameAreaX + LetterboxManager.GameAreaWidth / 4, LetterboxManager.GameAreaY + LetterboxManager.GameAreaHeight / 4 * 3);
		treeNodeRender3.text.enabled = true;
		vector.x += Random.Range(-10, 10);
		vector.y += Random.Range(-10, 10);
		treeNodeRender3.text.rectTransform.position = new Vector3(vector.x, vector.y, treeNodeRender3.text.rectTransform.position.z);
		treeNodeRender3.text.rectTransform.sizeDelta = new Vector2(1500f, 1500f);
		treeNodeRender.text.rectTransform.position = new Vector3(vector.x - 2f, vector.y + 2f, treeNodeRender.text.rectTransform.position.z);
		treeNodeRender.text.rectTransform.sizeDelta = new Vector2(1500f, 1500f);
		treeNodeRender2.text.enabled = true;
		treeNodeRender2.text.rectTransform.position = new Vector3(vector.x + 2f, vector.y - 2f, treeNodeRender2.text.rectTransform.position.z);
		treeNodeRender2.text.rectTransform.sizeDelta = new Vector2(1500f, 1500f);
		treeNodeRender.text.color = RenderHandler.GetColorFromPalette(RenderHandler.ColorPalette.VeryDarkRed);
		treeNodeRender2.text.color = RenderHandler.GetColorFromPalette(RenderHandler.ColorPalette.VeryDarkRed);
		treeNodeRender3.text.StartCoroutine(UpdateSplashText(treeNodeRender3));
		treeNodeRender.text.StartCoroutine(UpdateSplashText(treeNodeRender));
		treeNodeRender2.text.StartCoroutine(UpdateSplashText(treeNodeRender2));
		Object.Destroy(treeNodeRender3.text.gameObject, 1.5f);
		Object.Destroy(treeNodeRender.text.gameObject, 1.5f);
		Object.Destroy(treeNodeRender2.text.gameObject, 1.5f);
	}

	private IEnumerator UpdateSplashText(TreeNodeRender node)
	{
		int count = 0;
		while (count < 150)
		{
			if (count > 50)
			{
				Color color = node.text.color;
				color.a -= 0.01f;
				node.text.color = color;
			}
			count++;
			yield return new WaitForSeconds(0.01f);
		}
	}

	public static Color RetrieveColor(TextMode textmode)
	{
		return textmode switch
		{
			TextMode.Stun => RenderHandler.GetColorFromPalette(RenderHandler.ColorPalette.LightYellow), 
			TextMode.Miss => RenderHandler.GetColorFromPalette(RenderHandler.ColorPalette.VeryLightGray), 
			TextMode.Bleed => RenderHandler.GetColorFromPalette(RenderHandler.ColorPalette.VeryLightRed), 
			TextMode.Buffed => RenderHandler.GetColorFromPalette(RenderHandler.ColorPalette.LightOrange), 
			TextMode.Debuffed => RenderHandler.GetColorFromPalette(RenderHandler.ColorPalette.Gray), 
			TextMode.Heal => RenderHandler.GetColorFromPalette(RenderHandler.ColorPalette.LightGreen), 
			TextMode.ArmorDamage => RenderHandler.GetColorFromPalette(RenderHandler.ColorPalette.VeryLightCyan), 
			TextMode.Damage => RenderHandler.GetColorFromPalette(RenderHandler.ColorPalette.VeryLightRed), 
			_ => RenderHandler.GetColorFromPalette(RenderHandler.ColorPalette.VeryLightRed), 
		};
	}
}
