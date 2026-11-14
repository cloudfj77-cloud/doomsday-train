using Cysharp.Threading.Tasks;
using Duckov.UI.Animations;
using Duckov.Utilities;
using ItemStatsSystem;
using ItemStatsSystem.Items;
using SodaCraft.Localizations;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Duckov.UI;

public class IslandClosureView : MonoBehaviour, IClosureView, IPointerClickHandler, IEventSystemHandler
{
	[SerializeField]
	private FadeGroup mainFadeGroup;

	[SerializeField]
	private float timeSpacing = 0.25f;

	[Header("Title")]
	[SerializeField]
	private GameObject titleGroup;

	[SerializeField]
	private TextMeshProUGUI titleText;

	[SerializeField]
	[LocalizationKey("Default")]
	private string evacuatedTitleTextKey = "UI_Closure_Escaped";

	[SerializeField]
	private Color evacuatedTitleTextColor = Color.white;

	[SerializeField]
	[LocalizationKey("Default")]
	private string failedTitleTextKey = "UI_Closure_Dead";

	[SerializeField]
	private Color failedTitleTextColor = Color.red;

	[Header("Kill")]
	[SerializeField]
	private GameObject killCountGroup;

	[SerializeField]
	private TextMeshProUGUI killCountText;

	[SerializeField]
	private PunchReceiver killCountPunch;

	[SerializeField]
	private GameObject bossCountGroup;

	[SerializeField]
	private TextMeshProUGUI bossCountText;

	[SerializeField]
	private PunchReceiver bossCountPunch;

	[Header("Items")]
	[SerializeField]
	private GameObject itemValueGroup;

	[SerializeField]
	private TextMeshProUGUI itemValueText;

	[SerializeField]
	private PunchReceiver itemValuePunch;

	[SerializeField]
	private GameObject buffItemsGroup;

	[SerializeField]
	private IslandClosure_ItemEntry entryTemplate;

	private PrefabPool<IslandClosure_ItemEntry> pool;

	[Header("Interaction")]
	[SerializeField]
	private Button confirmButton;

	private bool confirmed;

	private bool clicked;

	private string EvacuatedTitleText => evacuatedTitleTextKey.ToPlainText();

	private string FailedTitleText => failedTitleTextKey.ToPlainText();

	private PrefabPool<IslandClosure_ItemEntry> Pool
	{
		get
		{
			if (pool == null)
			{
				pool = new PrefabPool<IslandClosure_ItemEntry>(entryTemplate);
			}
			return pool;
		}
	}

	private void Awake()
	{
		ClosureView.OverrideClosureView = this;
		confirmButton.onClick.AddListener(OnConfirmButtonClicked);
	}

	private void OnDestroy()
	{
		if (ClosureView.OverrideClosureView as Object == this)
		{
			ClosureView.OverrideClosureView = null;
		}
	}

	private void OnConfirmButtonClicked()
	{
		confirmed = true;
	}

	public async UniTask Task(bool evacuated)
	{
		Setup(evacuated);
		await mainFadeGroup.ShowAndReturnTask();
		await BlackScreen.HideAndReturnTask(null, 0f, 0.1f);
		await ShowTitle();
		await UniTask.WaitForSeconds(timeSpacing);
		await ShowKillCount();
		await UniTask.WaitForSeconds(timeSpacing);
		await ShowBossKillCount();
		await UniTask.WaitForSeconds(timeSpacing);
		await ShowItemsValue();
		await UniTask.WaitForSeconds(timeSpacing);
		await ShowBuffItems();
		confirmButton.gameObject.SetActive(value: true);
		await WaitForConfirm();
		await BlackScreen.ShowAndReturnTask(null, 0f, 0.1f);
		await mainFadeGroup.HideAndReturnTask();
	}

	private async UniTask WaitForConfirm()
	{
		confirmed = false;
		await UniTask.WaitUntil(() => confirmed);
	}

	private void Setup(bool evacuated)
	{
		Pool.ReleaseAll();
		titleGroup.SetActive(value: false);
		killCountGroup.SetActive(value: false);
		bossCountGroup.SetActive(value: false);
		itemValueGroup.SetActive(value: false);
		buffItemsGroup.SetActive(value: false);
		confirmButton.gameObject.SetActive(value: false);
		if (evacuated)
		{
			titleText.color = evacuatedTitleTextColor;
			titleText.text = EvacuatedTitleText;
		}
		else
		{
			titleText.color = failedTitleTextColor;
			titleText.text = FailedTitleText;
		}
		SetupKillCounts();
		int num = CalculateItemValue();
		itemValueText.text = $"{num}";
	}

	private void SetupKillCounts()
	{
		RogueClosureManager instance = RogueClosureManager.Instance;
		if (instance == null)
		{
			killCountText.text = "?";
			bossCountText.text = "?";
		}
		else
		{
			killCountText.text = $"{instance?.enemyKilledCount}";
			bossCountText.text = $"{instance?.bossKilledCount}";
		}
	}

	private int CalculateItemValue()
	{
		int num = 0;
		CharacterMainControl main = CharacterMainControl.Main;
		if (main == null)
		{
			return num;
		}
		Item characterItem = main.CharacterItem;
		if (characterItem == null)
		{
			return num;
		}
		foreach (Slot slot in characterItem.Slots)
		{
			if (slot != null && !(slot.Content == null))
			{
				num += slot.Content.Value;
			}
		}
		foreach (Item item in characterItem.Inventory)
		{
			if (!(item == null))
			{
				num += item.Value;
			}
		}
		PetProxy instance = PetProxy.Instance;
		if (instance != null && instance.Inventory != null)
		{
			foreach (Item item2 in instance.Inventory)
			{
				if (!(item2 == null))
				{
					num += item2.Value;
				}
			}
		}
		return num;
	}

	private async UniTask ShowTitle()
	{
		titleGroup.SetActive(value: true);
	}

	private async UniTask ShowKillCount()
	{
		killCountGroup.SetActive(value: true);
		killCountPunch.Punch();
	}

	private async UniTask ShowBossKillCount()
	{
		bossCountGroup.SetActive(value: true);
		bossCountPunch.Punch();
	}

	private async UniTask ShowItemsValue()
	{
		itemValueGroup.SetActive(value: true);
		itemValuePunch.Punch();
	}

	private async UniTask ShowBuffItems()
	{
		buffItemsGroup.SetActive(value: true);
		CharacterMainControl main = CharacterMainControl.Main;
		if (main == null)
		{
			return;
		}
		Item item = main.CharacterItem;
		if (item == null)
		{
			return;
		}
		await UniTask.WaitForSeconds(timeSpacing);
		float spacing = 0.2f;
		clicked = false;
		foreach (Item item2 in item.Inventory)
		{
			if (!(item2 == null) && item2.Tags.Contains("PassiveProp"))
			{
				Pool.Get().Show(item2).Forget();
				if (!clicked)
				{
					await UniTask.WaitForSeconds(spacing);
				}
			}
		}
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		clicked = true;
	}
}
