using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScratchInput : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	[SerializeField]
	private RawImage ticketImage;

	public Material brushMaterialTemplate;

	public Camera uiCamera;

	private RenderTexture mask;

	private Material brushMaterialInstance;

	private bool pointerInside;

	private RectTransform rectTransform;

	public static bool isScratching;

	private float revealThreshold = 0.75f;

	private const float BRUSH_RADIUS = 5f;

	private const int COMPLETION_CHECK_INTERVAL = 10;

	private bool hasLastPixel;

	private Vector2Int lastPixel;

	private bool revealed;

	private Texture2D readbackTex;

	private int frameCounter;

	public int slot;

	public int previousX;

	public int previousY;

	[SerializeField]
	private ScratchFlake flakePrefab;

	[SerializeField]
	private RectTransform flakeParent;

	private readonly List<ScratchFlake> flakePool = new List<ScratchFlake>();

	private Vector2 lastScratchPos;

	private bool hasLastPos;

	private static float BrushRadius => 5f * LotteryHelper.scratchRadiusMultiplier;

	private void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
		mask = new RenderTexture(56, 56, 0, RenderTextureFormat.ARGB32);
		mask.filterMode = FilterMode.Point;
		mask.wrapMode = TextureWrapMode.Clamp;
		mask.Create();
		RenderTexture.active = mask;
		GL.Clear(clearDepth: true, clearColor: true, Color.black);
		RenderTexture.active = null;
		brushMaterialInstance = new Material(brushMaterialTemplate);
		Material material = new Material(ticketImage.material);
		material.SetTexture("_MaskTex", mask);
		ticketImage.material = material;
		readbackTex = new Texture2D(56, 56, TextureFormat.R8, mipChain: false);
	}

	private void Update()
	{
		if (Input.GetMouseButtonUp(0))
		{
			hasLastPos = false;
			hasLastPixel = false;
		}
		if (Input.GetMouseButton(0) && pointerInside)
		{
			isScratching = true;
		}
		if (Input.GetMouseButtonUp(0) && !pointerInside)
		{
			isScratching = false;
		}
		if (revealed || !Input.GetMouseButton(0))
		{
			return;
		}
		Camera cam = uiCamera;
		Canvas canvas = ((ticketImage != null) ? ticketImage.canvas : null);
		if (canvas != null)
		{
			Canvas rootCanvas = canvas.rootCanvas;
			cam = ((rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay) ? null : rootCanvas.worldCamera);
		}
		if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, cam, out var localPoint))
		{
			return;
		}
		Rect rect = rectTransform.rect;
		if (localPoint.x < rect.xMin || localPoint.x > rect.xMax || localPoint.y < rect.yMin || localPoint.y > rect.yMax)
		{
			hasLastPixel = false;
			return;
		}
		if (hasLastPos && isScratching && !revealed)
		{
			Vector2 normalized = (localPoint - lastScratchPos).normalized;
			EmitFlakes(localPoint, normalized);
		}
		lastScratchPos = localPoint;
		hasLastPos = true;
		float f = (localPoint.x - rect.xMin) / rect.width * 56f;
		float f2 = (rect.yMax - localPoint.y) / rect.height * 56f;
		int x = Mathf.FloorToInt(f);
		int y = Mathf.FloorToInt(f2);
		OnAnyScratch();
		StrokeTo(x, y);
	}

	private void StrokeTo(int x, int y)
	{
		if (hasLastPixel)
		{
			Vector2 a = lastPixel;
			Vector2 b = new Vector2(x, y);
			int num = Mathf.CeilToInt(Vector2.Distance(a, b) / (BrushRadius * 0.5f));
			for (int i = 1; i <= num; i++)
			{
				if (revealed)
				{
					break;
				}
				Vector2 vector = Vector2.Lerp(a, b, (float)i / (float)num);
				DrawPixel(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y));
			}
		}
		else
		{
			DrawPixel(x, y);
		}
		lastPixel = new Vector2Int(x, y);
		hasLastPixel = true;
	}

	public void OnAnyScratch()
	{
		if (ScratcherUIManager.Instance.currentActiveScratcherItem.IsTag("SCRATCHER_NEW_TAG"))
		{
			ScratcherUIManager.Instance.currentActiveScratcherItem.DisableTag("SCRATCHER_NEW_TAG");
			ScratcherUIManager.Instance.currentActiveScratcherItem.unitValue = 0L;
			ScratcherUIManager.Instance.currentActiveScratcherItem.RemoveGameItemType("LOTTERY");
			LotteryHelper.OnScratcherStarted();
		}
	}

	private void DrawPixel(int x, int y)
	{
		brushMaterialInstance.SetVector("_Coord", new Vector4(x, y, 0f, 0f));
		brushMaterialInstance.SetFloat("_Radius", BrushRadius);
		brushMaterialInstance.SetVector("_TexSize", new Vector4(56f, 56f, 0f, 0f));
		RenderTexture.active = mask;
		GL.PushMatrix();
		GL.LoadOrtho();
		brushMaterialInstance.SetPass(0);
		GL.Begin(4);
		GL.Vertex3(0f, 0f, 0f);
		GL.Vertex3(1f, 0f, 0f);
		GL.Vertex3(0f, 1f, 0f);
		GL.End();
		GL.PopMatrix();
		RenderTexture.active = null;
		if (previousX != x || previousY != y)
		{
			frameCounter++;
		}
		previousX = x;
		previousY = y;
		if (frameCounter % 10 == 0)
		{
			CheckScratchCompletionInner();
		}
		if (frameCounter % 15 == 0)
		{
			AudioManager.Instance.Play2("scratch");
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		pointerInside = true;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		pointerInside = false;
	}

	private void OnDisable()
	{
		isScratching = false;
	}

	private void CheckScratchCompletion()
	{
		RenderTexture.active = mask;
		readbackTex.ReadPixels(new Rect(0f, 0f, 56f, 56f), 0, 0);
		readbackTex.Apply();
		RenderTexture.active = null;
		Color32[] pixels = readbackTex.GetPixels32();
		int num = 0;
		for (int i = 0; i < pixels.Length; i++)
		{
			if (pixels[i].r > 128)
			{
				num++;
			}
		}
		if ((float)num / (float)pixels.Length >= revealThreshold)
		{
			RevealAll();
			ScratcherUIManager.Instance.OnScratchComplete(slot);
		}
	}

	private void CheckScratchCompletionInner()
	{
		RenderTexture.active = mask;
		readbackTex.ReadPixels(new Rect(0f, 0f, 56f, 56f), 0, 0);
		readbackTex.Apply();
		RenderTexture.active = null;
		Color32[] pixels = readbackTex.GetPixels32();
		int num = 0;
		int num2 = 56;
		int num3 = 12;
		int num4 = num3 + 32;
		for (int i = num3; i < num4; i++)
		{
			for (int j = num3; j < num4; j++)
			{
				int num5 = i * num2 + j;
				if (pixels[num5].r > 128)
				{
					num++;
				}
			}
		}
		if ((float)num / 1024f >= revealThreshold)
		{
			RevealAll();
			ScratcherUIManager.Instance.OnScratchComplete(slot);
		}
	}

	public void RevealAll()
	{
		revealed = true;
		RenderTexture.active = mask;
		GL.Clear(clearDepth: true, clearColor: true, Color.white);
		RenderTexture.active = null;
		UIDragHandler.isDraggingAnyUI = false;
		pointerInside = false;
	}

	public void ResetScratch()
	{
		revealed = false;
		hasLastPixel = false;
		RenderTexture.active = mask;
		GL.Clear(clearDepth: true, clearColor: true, Color.black);
		RenderTexture.active = null;
		isScratching = false;
		pointerInside = false;
	}

	public void SetTexture(Texture2D texture)
	{
		ticketImage.texture = texture;
	}

	private ScratchFlake GetFlake()
	{
		foreach (ScratchFlake item in flakePool)
		{
			if (!item.gameObject.activeSelf)
			{
				return item;
			}
		}
		ScratchFlake scratchFlake = Object.Instantiate(flakePrefab, flakeParent);
		flakePool.Add(scratchFlake);
		return scratchFlake;
	}

	private void EmitFlakes(Vector2 localPos, Vector2 dir)
	{
		int num = Random.Range(1, 3);
		for (int i = 0; i < num; i++)
		{
			ScratchFlake flake = GetFlake();
			flake.gameObject.SetActive(value: true);
			flake.transform.localPosition = localPos;
			flake.transform.localRotation = Quaternion.identity;
			flake.transform.localScale = Vector3.one * Random.Range(0.6f, 1f);
			Vector2 dir2 = Quaternion.Euler(0f, 0f, Random.Range(-40f, 40f)) * dir;
			flake.Init(dir2);
		}
	}
}
