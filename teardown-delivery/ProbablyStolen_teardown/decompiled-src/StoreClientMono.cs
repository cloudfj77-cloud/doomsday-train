using DG.Tweening;
using UnityEngine;

public class StoreClientMono : MonoBehaviour, IClickable
{
	private enum FadeMode
	{
		None,
		MainPath,
		RightPathLeaving
	}

	public SpriteRenderer image;

	private IdleAnimator idleAnimator;

	private const int AUG_STILL_CHANCE = 25;

	private const int AUG_SHAKE_CHANCE = 5;

	private const float AUG_SHAKE_AMPLITUDE = 0.035f;

	private const float AUG_SHAKE_FREQUENCY = 4f;

	private StoreClient idleDecisionClient;

	private bool idleSuppressed;

	private bool idleShakeHorizontal;

	private Vector3 defaultIdleAxis = Vector3.up;

	private float defaultIdleAmplitude;

	private float defaultIdleFrequency;

	private DOTweenPath dOTweenPath;

	public DOTweenPath pathRight;

	private bool ignoreOnLeftFlag;

	[Header("Approach shadow effect")]
	[Tooltip("Tint applied while the client walks in (Papers Please style silhouette).")]
	public Color shadowColor = new Color(0.15f, 0.15f, 0.2f, 1f);

	[Tooltip("Curve mapping path progress (0=edge, 1=at counter) to how much full color shows. Linear by default.")]
	public AnimationCurve fadeInCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	[Tooltip("Fraction of the walk (from the edge) spent as a flat solid-color silhouette before the texture detail appears. Small = a quick flash of solid color.")]
	[Range(0f, 0.99f)]
	public float solidPhaseFraction = 0.66f;

	private FadeMode fadeMode;

	private MaterialPropertyBlock mpb;

	private static readonly int SolidBlendId = Shader.PropertyToID("_SolidBlend");

	private static readonly int SolidColorId = Shader.PropertyToID("_SolidColor");

	private float leaveTimeScale = 1f;

	public const float LIMP_TIME_SCALE = 0.45f;

	private bool walkingIn;

	public static StoreClientMono Instance { get; private set; }

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Object.Destroy(this);
		}
		else
		{
			Instance = this;
		}
		dOTweenPath = GetComponent<DOTweenPath>();
		idleAnimator = GetComponent<IdleAnimator>();
		if (idleAnimator != null)
		{
			defaultIdleAxis = idleAnimator.axis;
			defaultIdleAmplitude = idleAnimator.amplitude;
			defaultIdleFrequency = idleAnimator.frequency;
		}
		base.gameObject.SetActive(value: false);
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (image == null || fadeMode == FadeMode.None)
		{
			return;
		}
		float closeness;
		if (fadeMode == FadeMode.MainPath)
		{
			if (dOTweenPath == null || dOTweenPath.tween == null)
			{
				return;
			}
			closeness = dOTweenPath.tween.ElapsedPercentage();
		}
		else
		{
			if (pathRight == null || pathRight.tween == null)
			{
				return;
			}
			closeness = 1f - pathRight.tween.ElapsedPercentage();
		}
		ApplyFade(closeness);
	}

	private void ApplyFade(float closeness)
	{
		image.color = Color.Lerp(shadowColor, Color.white, fadeInCurve.Evaluate(closeness));
		float solidBlend = ((solidPhaseFraction > 0f) ? Mathf.Clamp01(1f - closeness / solidPhaseFraction) : 0f);
		SetSolidBlend(solidBlend);
	}

	private void SetSolidBlend(float blend)
	{
		if (!(image == null))
		{
			if (mpb == null)
			{
				mpb = new MaterialPropertyBlock();
			}
			image.GetPropertyBlock(mpb);
			mpb.SetFloat(SolidBlendId, blend);
			mpb.SetColor(SolidColorId, shadowColor);
			image.SetPropertyBlock(mpb);
		}
	}

	public void SetLimpOnLeave(float timeScale = 0.45f)
	{
		leaveTimeScale = timeScale;
	}

	private void ApplyLeaveTimeScale(DOTweenPath path)
	{
		if (path != null && path.tween != null)
		{
			path.tween.timeScale = leaveTimeScale;
		}
	}

	public void FreezeIdle()
	{
		if (idleAnimator != null)
		{
			idleAnimator.Freeze();
		}
	}

	public void StartLeavingStore()
	{
		if (dOTweenPath != null)
		{
			idleAnimator.Stop();
			EmporiumAudioPlayer.Instance.PlayWalking();
			if (image != null)
			{
				fadeMode = FadeMode.MainPath;
			}
			dOTweenPath.DOPlayBackwards();
			ApplyLeaveTimeScale(dOTweenPath);
		}
	}

	public void StartLeavingStoreRight()
	{
		if (pathRight != null)
		{
			idleAnimator.Stop();
			EmporiumAudioPlayer.Instance.PlayWalking();
			dOTweenPath.DOComplete();
			if (image != null)
			{
				fadeMode = FadeMode.RightPathLeaving;
			}
			pathRight.DORestart();
			pathRight.DOPlay();
			ApplyLeaveTimeScale(pathRight);
		}
	}

	public void StartApprochingStore()
	{
		Debug.Log("Approching Store");
		EmporiumAudioPlayer.Instance.PlayWalking();
		if (image != null)
		{
			image.color = shadowColor;
			SetSolidBlend(1f);
			fadeMode = FadeMode.MainPath;
		}
		walkingIn = true;
		ignoreOnLeftFlag = true;
		leaveTimeScale = 1f;
		ApplyLeaveTimeScale(pathRight);
		ApplyLeaveTimeScale(dOTweenPath);
		pathRight.DORewind();
		dOTweenPath.DORestart();
		dOTweenPath.tween?.Goto(0f);
		ignoreOnLeftFlag = false;
		dOTweenPath.DOPlay();
	}

	public void SetImmediatlyArrive()
	{
		StoreUIManager.Instance.DisplayClient();
		ignoreOnLeftFlag = true;
		if (pathRight != null)
		{
			pathRight.DORewind();
		}
		ignoreOnLeftFlag = false;
		GenericClientMono.SnapWalkToEnd(dOTweenPath, completeIfWalkingIn: true);
		if (image != null)
		{
			fadeMode = FadeMode.None;
			image.color = Color.white;
			SetSolidBlend(0f);
		}
		PlayIdleForCurrentClient();
	}

	private void PlayIdleForCurrentClient()
	{
		StoreClient storeClient = ((PlayerStore.Instance != null && PlayerStore.Instance.currentClientInstance != null) ? PlayerStore.Instance.currentClientInstance.GetClientBlueprint() : null);
		if (storeClient != idleDecisionClient)
		{
			idleDecisionClient = storeClient;
			bool flag = storeClient?.isAug ?? false;
			if (storeClient != null && storeClient.augIdleTell != 0)
			{
				idleSuppressed = storeClient.augIdleTell == StoreClient.AugIdleTell.Freeze;
				idleShakeHorizontal = storeClient.augIdleTell == StoreClient.AugIdleTell.Seizure;
			}
			else
			{
				idleSuppressed = flag && RNG.Roll(25);
				idleShakeHorizontal = flag && !idleSuppressed && RNG.Roll(5);
			}
		}
		if (idleSuppressed)
		{
			idleAnimator.Freeze();
			return;
		}
		if (idleShakeHorizontal)
		{
			idleAnimator.axis = Vector3.right;
			idleAnimator.amplitude = 0.035f;
			idleAnimator.frequency = 4f;
		}
		else
		{
			idleAnimator.axis = defaultIdleAxis;
			idleAnimator.amplitude = defaultIdleAmplitude;
			idleAnimator.frequency = defaultIdleFrequency;
		}
		idleAnimator.Play();
	}

	public void OnLeft()
	{
		if (ignoreOnLeftFlag)
		{
			return;
		}
		Debug.Log("Client Left");
		PlayerStore.Instance.ClientFlagDown();
		StoreUIManager.Instance.HideClient();
		if (StoreUIManager.Instance.isShootingMode)
		{
			StoreUIManager.Instance.ExitShootingMode();
		}
		if (StoreUIManager.Instance.isMagnifyingMode)
		{
			StoreUIManager.Instance.ExitMagnifyingMode();
		}
		if (PlayerStore.Instance.storeClientManager.clientStack.Count == 0)
		{
			TutorialUIManager.Instance.NoMoreCustomer();
		}
		if (!PlayerStore.Instance.callNextWhenPossible)
		{
			return;
		}
		PlayerStore.Instance.callNextWhenPossible = false;
		if (PlayerStore.Instance.storeState == PlayerStore.StoreState.OPEN)
		{
			PlayerStore.Instance.ClientFlagUp();
			StoreUIManager.Instance.StartCoroutine(MetaHelper.WaitAndDo(0.5f, delegate
			{
				PlayerStore.Instance.TryCallNextClient();
				PlayerStore.Instance.ClientFlagDown();
			}));
		}
	}

	public void OnArrived()
	{
		bool num = walkingIn;
		walkingIn = false;
		if (num)
		{
			Debug.Log("Client Arrived");
			PlayerStore.Instance.isClientArrived = true;
			StoreSecButton.Instance.ResetLight();
			PlayerStore.Instance.ClientFlagDown();
			StoreUIManager.Instance.OnNextClientArrived();
		}
		if (image != null)
		{
			fadeMode = FadeMode.None;
			image.color = Color.white;
			SetSolidBlend(0f);
		}
		PlayIdleForCurrentClient();
	}

	public void OnClick()
	{
		if (PlayerStore.Instance.currentClientInstance != null && !StoreUIManager.Instance.isShootingMode && !StoreUIManager.Instance.isMagnifyingMode)
		{
			StoreClient.ClientIntent clientIntent = PlayerStore.Instance.currentClientInstance.GetClientBlueprint().clientIntent;
			if (clientIntent != StoreClient.ClientIntent.DIALOGUE && clientIntent != StoreClient.ClientIntent.INSPECTION && clientIntent != StoreClient.ClientIntent.SPECIAL && clientIntent != StoreClient.ClientIntent.RENT)
			{
				NegociationUIManager.Instance.ToggleUI();
			}
		}
	}

	public bool IsMoving()
	{
		if (dOTweenPath != null && dOTweenPath.tween != null && dOTweenPath.tween.IsPlaying())
		{
			return true;
		}
		if (pathRight != null && pathRight.tween != null && pathRight.tween.IsPlaying())
		{
			return true;
		}
		return false;
	}
}
