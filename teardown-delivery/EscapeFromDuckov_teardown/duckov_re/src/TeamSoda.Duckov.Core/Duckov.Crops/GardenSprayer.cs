using UnityEngine;

namespace Duckov.Crops;

public class GardenSprayer : MonoBehaviour
{
	[SerializeField]
	private Garden master;

	[SerializeField]
	private Animator animator;

	private void Awake()
	{
		Garden.OnAutoWatersChanged += OnAutoWatersChanged;
		master.OnAutoWatered += OnAutoWatered;
	}

	private void OnDestroy()
	{
		Garden.OnAutoWatersChanged -= OnAutoWatersChanged;
		master.OnAutoWatered -= OnAutoWatered;
	}

	private void Start()
	{
		RefreshAnimatorStatus();
	}

	private void OnAutoWatered(Garden garden, Crop crop)
	{
	}

	private void OnAutoWatersChanged()
	{
		RefreshAnimatorStatus();
	}

	private void RefreshAnimatorStatus()
	{
	}
}
