using TMPro;
using UnityEngine;

public class RecordUIManager : MonoBehaviour
{
	public GameObject UIPanel;

	public TextMeshProUGUI officerNoteTMP;

	public TextMeshProUGUI suspectedCrimeTMP;

	public TextMeshProUGUI officerNameTMP;

	public TextMeshProUGUI recommandActionTMP;

	public TextMeshProUGUI activeOperationTMP;

	public TextMeshProUGUI evidenceLevelTMP;

	public TextMeshProUGUI projectSentanceTMP;

	public static RecordUIManager Instance { get; private set; }

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
	}

	public void OpenUI()
	{
		UIPanel.SetActive(value: true);
		EmporiumAudioPlayer.Instance.Play();
		Init();
	}

	public void CloseUI()
	{
		EmporiumAudioPlayer.Instance.Play();
		UIPanel.SetActive(value: false);
	}

	public void Init()
	{
		SecData secData = PlayerStore.Instance.secData;
		officerNoteTMP.text = secData.note;
		suspectedCrimeTMP.text = secData.GetSuspectedCrimeDisplay();
		officerNameTMP.text = secData.GetCaseOfficer(secData.GetTotalCrimeValue());
		recommandActionTMP.text = secData.GetAction();
		activeOperationTMP.text = secData.GetOperation();
		evidenceLevelTMP.text = secData.GetEvidenceLevelDisplay();
		projectSentanceTMP.text = secData.GetProjectedSentence();
	}
}
