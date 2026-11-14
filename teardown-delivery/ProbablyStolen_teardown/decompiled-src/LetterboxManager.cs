using System;
using System.Collections.Generic;
using EPOOutline;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;
using UnityEngine.UI;

public class LetterboxManager : MonoBehaviour
{
	private class WorldCamState
	{
		public float baseOrtho;

		public float lastApplied = float.NaN;

		public PixelPerfectCamera ppc;

		public bool ppcSuppressed;
	}

	private const float UI_CAMERA_Z = -20000f;

	private const float UI_PLANE_DISTANCE = 500f;

	private const float UI_CAMERA_FAR = 1000f;

	private const int UI_CAMERA_DEPTH = 100;

	private const int BARS_CAMERA_DEPTH = 150;

	private const float DOCK_CAMERA_Z = -40000f;

	private const int DOCK_CAMERA_DEPTH = 200;

	private int gameX;

	private int gameY;

	private int gameW;

	private int gameH;

	private Camera uiCamera;

	private Camera barsCamera;

	private LetterboxBars barsRenderer;

	private Camera dockCamera;

	private Canvas dockCanvas;

	private RectTransform dockRoot;

	private int appliedWidth = -1;

	private int appliedHeight = -1;

	private readonly Dictionary<Camera, WorldCamState> worldCams = new Dictionary<Camera, WorldCamState>();

	private readonly List<Camera> deadCams = new List<Camera>();

	private bool barsActive;

	private readonly List<Canvas> convertedCanvases = new List<Canvas>();

	private int sweepCountdown;

	private const int SWEEP_INTERVAL_FRAMES = 30;

	private static readonly List<Action> deferredActions = new List<Action>();

	public static LetterboxManager Instance { get; private set; }

	public static int GameAreaX
	{
		get
		{
			if (!(Instance != null))
			{
				return 0;
			}
			return Instance.gameX;
		}
	}

	public static int GameAreaY
	{
		get
		{
			if (!(Instance != null))
			{
				return 0;
			}
			return Instance.gameY;
		}
	}

	public static int GameAreaWidth
	{
		get
		{
			if (!(Instance != null))
			{
				return Screen.width;
			}
			return Instance.gameW;
		}
	}

	public static int GameAreaHeight
	{
		get
		{
			if (!(Instance != null))
			{
				return Screen.height;
			}
			return Instance.gameH;
		}
	}

	public static Camera UICamera
	{
		get
		{
			if (!(Instance != null))
			{
				return null;
			}
			return Instance.uiCamera;
		}
	}

	public static Canvas DockCanvas
	{
		get
		{
			if (!(Instance != null) || !Instance.barsActive)
			{
				return null;
			}
			return Instance.dockCanvas;
		}
	}

	public static RectTransform DockRoot
	{
		get
		{
			if (!(Instance != null) || !Instance.barsActive)
			{
				return null;
			}
			return Instance.dockRoot;
		}
	}

	public static event Action LayoutChanged;

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	private static void Bootstrap()
	{
		if (!(Instance != null))
		{
			GameObject obj = new GameObject("LetterboxManager");
			UnityEngine.Object.DontDestroyOnLoad(obj);
			obj.AddComponent<LetterboxManager>();
		}
	}

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		Instance = this;
		SceneManager.sceneLoaded += OnSceneLoaded;
		Apply();
	}

	private void OnDestroy()
	{
		if (Instance == this)
		{
			SceneManager.sceneLoaded -= OnSceneLoaded;
			Instance = null;
		}
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (dockRoot != null)
		{
			for (int num = dockRoot.childCount - 1; num >= 0; num--)
			{
				UnityEngine.Object.Destroy(dockRoot.GetChild(num).gameObject);
			}
		}
		Apply();
	}

	private void Update()
	{
		if (deferredActions.Count > 0)
		{
			Action[] array = deferredActions.ToArray();
			deferredActions.Clear();
			Action[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i]();
			}
		}
		if (Screen.width != appliedWidth || Screen.height != appliedHeight)
		{
			Apply();
		}
		else if (--sweepCountdown <= 0)
		{
			RefreshWorldCameraList();
			ConvertCanvases();
			sweepCountdown = 30;
		}
	}

	private void LateUpdate()
	{
		MaintainWorldCameras();
	}

	public static void Refresh()
	{
		if (Instance != null)
		{
			Instance.Apply();
		}
	}

	public static void RunNextUpdate(Action action)
	{
		if (action != null)
		{
			deferredActions.Add(action);
		}
	}

	public static void ComputeGameArea(int w, int h, out int x, out int y, out int areaW, out int areaH)
	{
		areaW = w;
		areaH = h;
		if (w * 9 > h * 16)
		{
			areaW = h * 16 / 9;
		}
		else if (w * 9 < h * 16)
		{
			areaH = w * 9 / 16;
		}
		x = (w - areaW) / 2;
		y = (h - areaH) / 2;
	}

	private void Apply()
	{
		int width = Screen.width;
		int height = Screen.height;
		if (width > 0 && height > 0)
		{
			bool num = barsActive;
			ComputeGameArea(width, height, out gameX, out gameY, out gameW, out gameH);
			barsActive = gameW != width || gameH != height;
			EnsureOwnCameras();
			uiCamera.enabled = barsActive;
			barsCamera.enabled = barsActive;
			dockCamera.enabled = barsActive;
			dockCanvas.enabled = barsActive;
			dockCamera.pixelRect = new Rect(0f, 0f, width, height);
			dockCamera.orthographicSize = (float)height / 2f;
			dockCamera.transform.position = new Vector3((float)width / 2f, (float)height / 2f, -40000f);
			Rect area = new Rect((float)gameX / (float)width, (float)gameY / (float)height, (float)gameW / (float)width, (float)gameH / (float)height);
			uiCamera.pixelRect = new Rect(gameX, gameY, gameW, gameH);
			uiCamera.orthographicSize = (float)gameH / 2f;
			uiCamera.transform.position = new Vector3((float)gameX + (float)gameW / 2f, (float)gameY + (float)gameH / 2f, -20000f);
			barsRenderer.SetArea(area);
			appliedWidth = width;
			appliedHeight = height;
			sweepCountdown = 30;
			RefreshWorldCameraList();
			ConvertCanvases();
			MaintainWorldCameras();
			if (num && !barsActive)
			{
				RestoreSuppressedPixelPerfect();
			}
			LetterboxManager.LayoutChanged?.Invoke();
		}
	}

	private void EnsureOwnCameras()
	{
		if (uiCamera == null)
		{
			GameObject gameObject = new GameObject("LetterboxUICamera");
			gameObject.transform.SetParent(base.transform, worldPositionStays: false);
			uiCamera = gameObject.AddComponent<Camera>();
			uiCamera.orthographic = true;
			uiCamera.nearClipPlane = 0.3f;
			uiCamera.farClipPlane = 1000f;
			uiCamera.depth = 100f;
			uiCamera.clearFlags = CameraClearFlags.Depth;
			uiCamera.useOcclusionCulling = false;
			uiCamera.allowMSAA = false;
			uiCamera.allowHDR = false;
		}
		if (barsCamera == null)
		{
			GameObject gameObject2 = new GameObject("LetterboxBarsCamera");
			gameObject2.transform.SetParent(base.transform, worldPositionStays: false);
			gameObject2.transform.localPosition = new Vector3(0f, 0f, -40000f);
			barsCamera = gameObject2.AddComponent<Camera>();
			barsCamera.orthographic = true;
			barsCamera.cullingMask = 0;
			barsCamera.clearFlags = CameraClearFlags.Nothing;
			barsCamera.depth = 150f;
			barsCamera.rect = new Rect(0f, 0f, 1f, 1f);
			barsCamera.useOcclusionCulling = false;
			barsCamera.allowMSAA = false;
			barsCamera.allowHDR = false;
			barsRenderer = gameObject2.AddComponent<LetterboxBars>();
		}
		if (dockCamera == null)
		{
			GameObject gameObject3 = new GameObject("LetterboxDockCamera");
			gameObject3.transform.SetParent(base.transform, worldPositionStays: false);
			dockCamera = gameObject3.AddComponent<Camera>();
			dockCamera.orthographic = true;
			dockCamera.nearClipPlane = 0.3f;
			dockCamera.farClipPlane = 1000f;
			dockCamera.depth = 200f;
			dockCamera.clearFlags = CameraClearFlags.Depth;
			dockCamera.useOcclusionCulling = false;
			dockCamera.allowMSAA = false;
			dockCamera.allowHDR = false;
		}
		if (dockCanvas == null)
		{
			GameObject gameObject4 = new GameObject("LetterboxDockCanvas", typeof(RectTransform));
			gameObject4.transform.SetParent(base.transform, worldPositionStays: false);
			dockCanvas = gameObject4.AddComponent<Canvas>();
			dockCanvas.renderMode = RenderMode.ScreenSpaceCamera;
			dockCanvas.worldCamera = dockCamera;
			dockCanvas.planeDistance = 500f;
			dockCanvas.sortingOrder = -100;
			gameObject4.AddComponent<GraphicRaycaster>();
		}
		if (dockRoot == null)
		{
			GameObject gameObject5 = new GameObject("LetterboxDockRoot", typeof(RectTransform));
			dockRoot = (RectTransform)gameObject5.transform;
			dockRoot.SetParent(dockCanvas.transform, worldPositionStays: false);
			dockRoot.anchorMin = Vector2.zero;
			dockRoot.anchorMax = Vector2.one;
			dockRoot.offsetMin = Vector2.zero;
			dockRoot.offsetMax = Vector2.zero;
			dockRoot.pivot = new Vector2(0f, 1f);
		}
	}

	private void RefreshWorldCameraList()
	{
		Camera[] array = UnityEngine.Object.FindObjectsOfType<Camera>(includeInactive: true);
		foreach (Camera camera in array)
		{
			if (!(camera == uiCamera) && !(camera == barsCamera) && !(camera == dockCamera) && !worldCams.ContainsKey(camera))
			{
				worldCams[camera] = new WorldCamState
				{
					baseOrtho = camera.orthographicSize,
					ppc = camera.GetComponent<PixelPerfectCamera>()
				};
			}
		}
		deadCams.Clear();
		foreach (KeyValuePair<Camera, WorldCamState> worldCam in worldCams)
		{
			if (worldCam.Key == null)
			{
				deadCams.Add(worldCam.Key);
			}
		}
		foreach (Camera deadCam in deadCams)
		{
			worldCams.Remove(deadCam);
		}
	}

	public static float GetIntendedOrthographicSize(Camera cam)
	{
		if (cam == null)
		{
			return 0f;
		}
		if (Instance != null && Instance.worldCams.TryGetValue(cam, out var value) && !float.IsNaN(value.lastApplied) && Mathf.Approximately(cam.orthographicSize, value.lastApplied))
		{
			return value.baseOrtho;
		}
		return cam.orthographicSize;
	}

	public static void SetIntendedOrthographicSize(Camera cam, float size)
	{
		if (!(cam == null))
		{
			if (Instance != null && Instance.worldCams.TryGetValue(cam, out var value))
			{
				float num = size * Instance.CurrentFactor();
				value.baseOrtho = size;
				value.lastApplied = num;
				cam.orthographicSize = num;
			}
			else
			{
				cam.orthographicSize = size;
			}
		}
	}

	private float CurrentFactor()
	{
		if (gameH <= 0)
		{
			return 1f;
		}
		return (float)appliedHeight / (float)gameH;
	}

	private void MaintainWorldCameras()
	{
		bool flag = gameW != appliedWidth || gameH != appliedHeight;
		float num = CurrentFactor();
		foreach (KeyValuePair<Camera, WorldCamState> worldCam in worldCams)
		{
			Camera key = worldCam.Key;
			WorldCamState value = worldCam.Value;
			if (key == null || !key.orthographic)
			{
				continue;
			}
			if (value.ppc != null && value.ppc.enabled)
			{
				if (!flag)
				{
					value.lastApplied = float.NaN;
					continue;
				}
				Debug.Log("LetterboxManager: disabling PixelPerfectCamera on '" + key.name + "' (letterbox active)");
				value.ppc.enabled = false;
				value.ppcSuppressed = true;
				Outliner component = key.GetComponent<Outliner>();
				if (component != null && component.enabled)
				{
					component.enabled = false;
					component.enabled = true;
				}
			}
			float orthographicSize = key.orthographicSize;
			if (float.IsNaN(value.lastApplied) || !Mathf.Approximately(orthographicSize, value.lastApplied))
			{
				value.baseOrtho = orthographicSize;
			}
			float num2 = value.baseOrtho * num;
			if (!Mathf.Approximately(orthographicSize, num2))
			{
				key.orthographicSize = num2;
			}
			value.lastApplied = num2;
		}
	}

	private void RestoreSuppressedPixelPerfect()
	{
		foreach (KeyValuePair<Camera, WorldCamState> worldCam in worldCams)
		{
			WorldCamState value = worldCam.Value;
			if (!value.ppcSuppressed)
			{
				continue;
			}
			value.ppcSuppressed = false;
			if (worldCam.Key == null || value.ppc == null)
			{
				continue;
			}
			if (EmporiumEntry.Instance != null && EmporiumEntry.Instance.pixelPerfectCamera == value.ppc && EmporiumOption.Instance != null)
			{
				EmporiumOption.Instance.ApplyPixelPerfectCamera();
				continue;
			}
			value.ppc.enabled = true;
			Outliner component = worldCam.Key.GetComponent<Outliner>();
			if (component != null && component.enabled)
			{
				component.enabled = false;
				component.enabled = true;
			}
		}
	}

	private static Canvas FindOverlayCanvas()
	{
		RenderHandler current = RenderHandler.current;
		if (!(current != null))
		{
			return null;
		}
		return current.OverlayCanvas;
	}

	private void ConvertCanvases()
	{
		if (uiCamera == null)
		{
			return;
		}
		if (!barsActive)
		{
			foreach (Canvas convertedCanvase in convertedCanvases)
			{
				if (!(convertedCanvase == null) && convertedCanvase.renderMode == RenderMode.ScreenSpaceCamera && (convertedCanvase.worldCamera == uiCamera || convertedCanvase.worldCamera == dockCamera))
				{
					convertedCanvase.renderMode = RenderMode.ScreenSpaceOverlay;
				}
			}
			convertedCanvases.Clear();
			return;
		}
		Canvas canvas = FindOverlayCanvas();
		Canvas[] array = UnityEngine.Object.FindObjectsOfType<Canvas>(includeInactive: true);
		foreach (Canvas canvas2 in array)
		{
			if (!canvas2.isRootCanvas || canvas2 == dockCanvas)
			{
				continue;
			}
			if (canvas2.renderMode == RenderMode.ScreenSpaceOverlay)
			{
				canvas2.renderMode = RenderMode.ScreenSpaceCamera;
				canvas2.worldCamera = ((canvas2 == canvas) ? dockCamera : uiCamera);
				canvas2.planeDistance = 500f;
				convertedCanvases.Add(canvas2);
			}
			else if (canvas2.renderMode == RenderMode.ScreenSpaceCamera)
			{
				if (canvas2 == canvas && canvas2.worldCamera == uiCamera)
				{
					canvas2.worldCamera = dockCamera;
				}
				else if (canvas2 != canvas && canvas2.worldCamera == dockCamera)
				{
					canvas2.worldCamera = uiCamera;
				}
			}
		}
	}
}
