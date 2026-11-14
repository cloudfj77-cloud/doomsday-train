using DG.Tweening;
using UnityEngine;

public class GenericClientMono : MonoBehaviour
{
	public SpriteRenderer image;

	public string id;

	private IdleAnimator idleAnimator;

	private DOTweenPath dOTweenPath;

	[Header("Approach shadow effect")]
	[Tooltip("Tint applied while the client walks in (Papers Please style silhouette).")]
	public Color shadowColor = new Color(0.15f, 0.15f, 0.2f, 1f);

	[Tooltip("Curve mapping path progress (0=edge, 1=at counter) to how much full color shows. Linear by default.")]
	public AnimationCurve fadeInCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	[Tooltip("Fraction of the walk (from the edge) spent as a flat solid-color silhouette before the texture detail appears. Small = a quick flash of solid color.")]
	[Range(0f, 0.95f)]
	public float solidPhaseFraction = 0.12f;

	private bool fading;

	private MaterialPropertyBlock mpb;

	private static readonly int SolidBlendId = Shader.PropertyToID("_SolidBlend");

	private static readonly int SolidColorId = Shader.PropertyToID("_SolidColor");

	private bool walkingIn;

	private bool ignoreOnLeftFlag;

	private void Awake()
	{
		dOTweenPath = GetComponent<DOTweenPath>();
		idleAnimator = GetComponent<IdleAnimator>();
		base.gameObject.SetActive(value: false);
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (fading && !(image == null) && !(dOTweenPath == null) && dOTweenPath.tween != null)
		{
			ApplyFade(dOTweenPath.tween.ElapsedPercentage());
		}
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

	public void StartArrive()
	{
		base.gameObject.SetActive(value: true);
		image.sprite = SpriteDict.Instance.ShowClientSprite(SpriteDict.GetRandomSecSpriteName());
		EmporiumAudioPlayer.Instance.PlayWalking();
		if (image != null)
		{
			image.color = shadowColor;
			SetSolidBlend(1f);
			fading = true;
		}
		walkingIn = true;
		ignoreOnLeftFlag = true;
		dOTweenPath.DORestart();
		dOTweenPath.tween?.Goto(0f);
		ignoreOnLeftFlag = false;
		dOTweenPath.DOPlay();
	}

	public void StartLeave()
	{
		idleAnimator.Stop();
		EmporiumAudioPlayer.Instance.PlayWalking();
		if (image != null)
		{
			fading = true;
		}
		dOTweenPath.DOPlayBackwards();
	}

	public void OnArrived()
	{
		if (image != null)
		{
			fading = false;
			image.color = Color.white;
			SetSolidBlend(0f);
		}
		bool num = walkingIn;
		walkingIn = false;
		if (num)
		{
			StoreUIManager.Instance.OnGenericArrived(id);
		}
		else
		{
			Debug.Log("[GenericClientMono] OnArrived " + id + " without a dispatch, arrest logic skipped");
		}
		idleAnimator.Play();
	}

	public void OnLeft()
	{
		if (!ignoreOnLeftFlag)
		{
			Debug.Log("[GenericClientMono] OnLeft " + id + " (tween pos " + ((dOTweenPath != null && dOTweenPath.tween != null) ? dOTweenPath.tween.position.ToString("0.00") : "none") + ")");
			StoreUIManager.Instance.OnGenericLeft(id);
			base.gameObject.SetActive(value: false);
		}
	}

	public void SetImmediatlyArrive()
	{
		base.gameObject.SetActive(value: true);
		walkingIn = false;
		SnapWalkToEnd(dOTweenPath, completeIfWalkingIn: false);
		if (image != null)
		{
			fading = false;
			image.color = Color.white;
			SetSolidBlend(0f);
		}
		idleAnimator.Play();
	}

	public static void SnapWalkToEnd(DOTweenPath path, bool completeIfWalkingIn)
	{
		if (path == null)
		{
			return;
		}
		Tween tween = path.tween;
		if (tween != null && tween.IsActive())
		{
			if (completeIfWalkingIn && tween.IsPlaying() && !tween.IsBackwards())
			{
				tween.Complete();
				return;
			}
			tween.Goto(tween.Duration());
			tween.Pause();
		}
	}
}
