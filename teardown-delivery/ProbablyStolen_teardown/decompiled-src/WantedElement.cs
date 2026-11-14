using UnityEngine;

public class WantedElement : MonoBehaviour
{
	public string identifier;

	public GameObject cross;

	private void Awake()
	{
		WantedUIManager.Instance.RegisterElement(this);
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void OnEnable()
	{
		if (PlayerStore.Instance.wantedStatus.ContainsKey(identifier) && PlayerStore.Instance.wantedStatus[identifier] == 1)
		{
			cross.SetActive(value: true);
		}
		else
		{
			cross.SetActive(value: false);
		}
	}

	public void OnArrested()
	{
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		StartCoroutine(MetaHelper.WaitAndDo(1.5f, delegate
		{
			if (base.gameObject.activeInHierarchy)
			{
				cross.SetActive(value: true);
				EmporiumAudioPlayer.Instance.cross.Play();
			}
		}));
	}

	private void OnDestroy()
	{
		if (WantedUIManager.Instance != null)
		{
			WantedUIManager.Instance.UnregisterElement(this);
		}
	}
}
