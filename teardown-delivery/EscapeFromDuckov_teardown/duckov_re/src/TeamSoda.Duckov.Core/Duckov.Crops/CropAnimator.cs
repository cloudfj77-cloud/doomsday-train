using System;
using UnityEngine;

namespace Duckov.Crops;

public class CropAnimator : MonoBehaviour
{
	[Serializable]
	private struct Stage
	{
		public float progress;

		public float position;

		public float scale;

		public Stage(float progress, float position, float scale)
		{
			this.progress = progress;
			this.position = position;
			this.scale = scale;
		}
	}

	[SerializeField]
	private Crop crop;

	[SerializeField]
	private Transform displayParent;

	[SerializeField]
	private MeshRenderer progressBar;

	[SerializeField]
	private GameObject needWaterIndicator;

	[SerializeField]
	private GameObject canHarvestIndicator;

	[SerializeField]
	private ParticleSystem plantFX;

	[SerializeField]
	private ParticleSystem stageChangeFX;

	[SerializeField]
	private ParticleSystem ripenFX;

	[SerializeField]
	private ParticleSystem waterFX;

	[SerializeField]
	private ParticleSystem harvestFX;

	[SerializeField]
	private ParticleSystem destroyFX;

	[Header("Dirt")]
	[SerializeField]
	private MeshRenderer dirtRenderer;

	[SerializeField]
	private string dirtColorPropertyName = "_TintColor";

	[SerializeField]
	private Color dryColor;

	[SerializeField]
	private Color wetColor;

	[SerializeField]
	private Stage[] stages = new Stage[4]
	{
		new Stage(-0.1f, -0.4f, 0.25f),
		new Stage(0.333f, -0.4f, 0.5f),
		new Stage(0.666f, -0.2f, 0.75f),
		new Stage(0.999f, -0.1f, 1f)
	};

	private MaterialPropertyBlock _propertyBlock;

	private int cachedStage;

	private ParticleSystem PlantFX => plantFX;

	private ParticleSystem StageChangeFX => stageChangeFX;

	private ParticleSystem RipenFX => ripenFX;

	private ParticleSystem WaterFX => waterFX;

	private ParticleSystem HarvestFX => harvestFX;

	private ParticleSystem DestroyFX => destroyFX;

	private MaterialPropertyBlock PropertyBlock
	{
		get
		{
			if (_propertyBlock == null)
			{
				_propertyBlock = new MaterialPropertyBlock();
			}
			return _propertyBlock;
		}
	}

	private void Awake()
	{
		if (crop == null)
		{
			crop = GetComponent<Crop>();
		}
		Crop obj = crop;
		obj.onPlant = (Action<Crop>)Delegate.Combine(obj.onPlant, new Action<Crop>(OnPlant));
		Crop obj2 = crop;
		obj2.onRipen = (Action<Crop>)Delegate.Combine(obj2.onRipen, new Action<Crop>(OnRipen));
		Crop obj3 = crop;
		obj3.onWater = (Action<Crop>)Delegate.Combine(obj3.onWater, new Action<Crop>(OnWater));
		Crop obj4 = crop;
		obj4.onHarvest = (Action<Crop>)Delegate.Combine(obj4.onHarvest, new Action<Crop>(OnHarvest));
		Crop obj5 = crop;
		obj5.onBeforeDestroy = (Action<Crop>)Delegate.Combine(obj5.onBeforeDestroy, new Action<Crop>(OnBeforeDestroy));
	}

	private void Start()
	{
		RefreshDirt();
	}

	private void Update()
	{
		RefreshPosition();
		RefreshProgressBar();
	}

	private void RefreshProgressBar()
	{
		bool watered = crop.Watered;
		bool ripen = crop.Ripen;
		needWaterIndicator.SetActive(!watered);
		canHarvestIndicator.SetActive(ripen);
		bool flag = watered && !ripen;
		progressBar.gameObject.SetActive(flag);
		if (flag)
		{
			PropertyBlock.SetFloat("_Alpha", crop.Watered ? 1f : 0f);
			PropertyBlock.SetFloat("_Progress", crop.Progress);
			progressBar.SetPropertyBlock(PropertyBlock);
		}
	}

	private void RefreshPosition(bool notifyStageChange = true)
	{
		float progress = crop.Progress;
		Stage stage = default(Stage);
		int? num = cachedStage;
		int num2 = 0;
		while (true)
		{
			if (num2 < stages.Length)
			{
				Stage stage2 = stages[num2];
				if (progress < stages[num2].progress)
				{
					stage = stage2;
					cachedStage = num2;
					break;
				}
				num2++;
				continue;
			}
			cachedStage = stages.Length - 1;
			stage = stages[cachedStage];
			break;
		}
		displayParent.localPosition = Vector3.up * stage.position;
		displayParent.localScale = Vector3.one * stage.scale;
		if (notifyStageChange && num.HasValue && num.Value != cachedStage)
		{
			OnStageChange();
		}
	}

	private void OnStageChange()
	{
		FXPool.Play(StageChangeFX, base.transform.position, base.transform.rotation);
		RefreshDirt();
	}

	private void OnWater(Crop crop)
	{
		FXPool.Play(WaterFX, base.transform.position, base.transform.rotation);
		RefreshDirt();
	}

	private void OnRipen(Crop crop)
	{
		FXPool.Play(RipenFX, base.transform.position, base.transform.rotation);
	}

	private void OnHarvest(Crop crop)
	{
		FXPool.Play(HarvestFX, base.transform.position, base.transform.rotation);
	}

	private void OnPlant(Crop crop)
	{
		FXPool.Play(PlantFX, base.transform.position, base.transform.rotation);
		RefreshDirt();
	}

	private void OnBeforeDestroy(Crop crop)
	{
		FXPool.Play(DestroyFX, base.transform.position, base.transform.rotation);
	}

	private void RefreshDirt()
	{
		bool flag = cachedStage <= 0;
		dirtRenderer.gameObject.SetActive(flag);
		if (flag)
		{
			MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
			materialPropertyBlock.SetColor(dirtColorPropertyName, crop.Watered ? wetColor : dryColor);
			dirtRenderer.SetPropertyBlock(materialPropertyBlock);
		}
	}
}
