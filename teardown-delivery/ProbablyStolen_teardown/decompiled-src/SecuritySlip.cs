using TMPro;
using UnityEngine;

public class SecuritySlip : MonoBehaviour
{
	public TextMeshProUGUI title;

	public TextMeshProUGUI body;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void Init(GameItem item)
	{
		title.text = item.GetTagReadonly("title").GetString();
		body.text = item.GetTagReadonly("body").GetString();
	}
}
