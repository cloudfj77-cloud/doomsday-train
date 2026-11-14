using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Duckov.Utilities;
using ItemStatsSystem;
using UnityEngine;
using UnityEngine.Events;

namespace Duckov;

public class LotteryBox : InteractableBase
{
	[Serializable]
	private class ItemTypeID
	{
		[ItemTypeID]
		public int id;
	}

	[SerializeField]
	private CostTaker costTaker;

	[SerializeField]
	private Transform graphicsParent;

	[SerializeField]
	private float openDelay = 1f;

	[SerializeField]
	private float rollingTime = 5f;

	[SerializeField]
	private float rollingStepBegin = 0.5f;

	[SerializeField]
	private float rollingStepEnd = 0.1f;

	[SerializeField]
	private string openSFX = "SFX/Special/lotterybox_open";

	[SerializeField]
	private string closeSFX = "SFX/Special/lotterybox_close";

	[SerializeField]
	private RandomContainer<ItemTypeID> candidates;

	[SerializeField]
	private SodaPointLight light;

	[SerializeField]
	private Renderer glow;

	public UnityEvent onBegin;

	public UnityEvent onRollBegin;

	public UnityEvent onRollStep;

	public UnityEvent onShowResult;

	public UnityEvent onPickup;

	public UnityEvent onEnd;

	private bool inProgress;

	private Item result;

	private bool confirmed;

	private bool interactable;

	private Dictionary<int, ItemGraphicInfo> graphicsCache = new Dictionary<int, ItemGraphicInfo>();

	protected override void OnInteractStart(CharacterMainControl interactCharacter)
	{
		if (interactCharacter.IsMainCharacter)
		{
			confirmed = true;
		}
	}

	protected override bool IsInteractable()
	{
		return interactable;
	}

	public void Begin()
	{
		if (!inProgress)
		{
			Process(base.destroyCancellationToken).Forget();
		}
	}

	public async UniTask Process(CancellationToken cancellationToken)
	{
		interactable = false;
		inProgress = true;
		costTaker.gameObject.SetActive(value: false);
		Display(candidates.entries[0].value.id);
		onBegin?.Invoke();
		AudioManager.Post(openSFX);
		await UniTask.WaitForSeconds(openDelay, ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken);
		SetColor(Color.white * 0.6f);
		onRollBegin?.Invoke();
		await RollingAnimation(cancellationToken);
		await ShowResult(cancellationToken);
		onShowResult?.Invoke();
		interactable = true;
		if (result != null)
		{
			int num = result.Quality;
			if (num > 6)
			{
				num = 6;
			}
			DisplayQuality q = (DisplayQuality)num;
			Color shadowColor = GameplayDataSettings.UIStyle.GetDisplayQualityLook(q).shadowColor;
			SetColor(shadowColor);
		}
		await WaitForConfirm(cancellationToken);
		if (result != null && !ItemUtilities.SendToPlayerCharacter(result, dontMerge: true))
		{
			result.Drop(CharacterMainControl.Main, createRigidbody: true);
		}
		onPickup?.Invoke();
		AudioManager.Post(closeSFX);
		SetColor(Color.black);
		HideAllCachedGraphics();
		interactable = false;
		inProgress = false;
		costTaker.gameObject.SetActive(value: true);
		onEnd?.Invoke();
	}

	private async UniTask WaitForConfirm(CancellationToken cancellationToken)
	{
		confirmed = false;
		await UniTask.WaitUntil(() => confirmed, PlayerLoopTiming.Update, cancellationToken);
	}

	private async UniTask ShowResult(CancellationToken cancellationToken)
	{
		ItemTypeID typeID = candidates.GetRandom();
		result = await ItemAssetsCollection.InstantiateAsync(typeID.id);
		overrideInteractName = true;
		_overrideInteractNameKey = result.DisplayNameRaw;
		Display(typeID.id);
	}

	private async UniTask RollingAnimation(CancellationToken cancellationToken)
	{
		float timer = 0f;
		_ = rollingStepBegin;
		int count = candidates.entries.Count;
		int displayingIndex = UnityEngine.Random.Range(0, count);
		while (timer < rollingTime)
		{
			ItemTypeID value = candidates.entries[displayingIndex].value;
			Display(value.id);
			onRollStep?.Invoke();
			float t = timer / rollingTime;
			float stepTime = Mathf.Lerp(rollingStepBegin, rollingStepEnd, t);
			float num = rollingTime - timer;
			bool needBreak = false;
			if (stepTime > num)
			{
				stepTime = num;
				needBreak = true;
			}
			await UniTask.WaitForSeconds(stepTime, ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken);
			if (!needBreak)
			{
				timer += stepTime;
				displayingIndex++;
				displayingIndex %= count;
				continue;
			}
			break;
		}
	}

	private void SetColor(Color color)
	{
		if ((bool)light)
		{
			light.LightColor = color * 6f;
		}
		if ((bool)glow)
		{
			glow.material.SetColor("_EmissionColor", light.LightColor * 2f);
		}
	}

	private void Display(int id)
	{
		HideAllCachedGraphics();
		Get(id).gameObject.SetActive(value: true);
	}

	private void HideAllCachedGraphics()
	{
		foreach (KeyValuePair<int, ItemGraphicInfo> item in graphicsCache)
		{
			item.Value.gameObject.SetActive(value: false);
		}
	}

	private ItemGraphicInfo Get(int typeID)
	{
		if (graphicsCache.TryGetValue(typeID, out var value))
		{
			return value;
		}
		ItemGraphicInfo value2 = ItemGraphicInfo.CreateAGraphic(ItemAssetsCollection.GetPrefab(typeID), graphicsParent);
		graphicsCache.Add(typeID, value2);
		return value2;
	}
}
