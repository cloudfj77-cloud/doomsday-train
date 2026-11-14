using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class InteractablePickup : InteractableBase
{
	[SerializeField]
	private DuckovItemAgent itemAgent;

	private Rigidbody rb;

	private Vector3 throwStartPoint;

	private bool destroied;

	public Transform graphicRoot;

	public DuckovItemAgent ItemAgent => itemAgent;

	public static event Action<InteractablePickup, CharacterMainControl> OnPickupSuccess;

	protected override bool IsInteractable()
	{
		return true;
	}

	public void OnInit()
	{
		if ((bool)itemAgent && (bool)itemAgent.Item)
		{
			if (!graphicRoot)
			{
				graphicRoot = base.transform;
			}
			ActiveMarker();
		}
		overrideInteractName = true;
		base.InteractName = itemAgent.Item.DisplayNameRaw;
	}

	public void CreateGraphic()
	{
		if ((bool)itemAgent && (bool)itemAgent.Item)
		{
			ItemGraphicInfo itemGraphicInfo = null;
			if (itemAgent.Item.useSpriteForPickup)
			{
				itemGraphicInfo = UnityEngine.Object.Instantiate(ItemGraphicInfo.spriteGraphicPfb);
				itemGraphicInfo.fallbackSprite.sprite = itemAgent.Item.Icon;
				itemGraphicInfo.transform.SetParent(graphicRoot);
				itemGraphicInfo.transform.localPosition = Vector3.zero;
				itemGraphicInfo.SnapGroundPointToParent();
			}
			else
			{
				itemGraphicInfo = ItemGraphicInfo.CreateAGraphic(itemAgent.Item, graphicRoot, snapGround: true);
			}
			interactMarkerOffset = base.transform.InverseTransformPoint(itemGraphicInfo.transform.position);
		}
	}

	protected override void OnInteractStart(CharacterMainControl character)
	{
		bool flag = character.PickupItem(itemAgent.Item);
		try
		{
			if (flag)
			{
				InteractablePickup.OnPickupSuccess?.Invoke(this, character);
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
		StopInteract();
	}

	public void Throw(Vector3 direction, float randomAngle)
	{
		throwStartPoint = base.transform.position;
		if (!rb)
		{
			rb = base.gameObject.AddComponent<Rigidbody>();
		}
		rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
		rb.constraints = RigidbodyConstraints.FreezeRotation;
		if (direction.magnitude < 0.1f)
		{
			direction = Vector3.zero;
		}
		else
		{
			direction.y = 0f;
			direction.Normalize();
			direction = Quaternion.Euler(0f, UnityEngine.Random.Range(0f - randomAngle, randomAngle) * 0.5f, 0f) * direction;
			direction *= UnityEngine.Random.Range(0.5f, 1f) * 3f;
			direction.y = 2.5f;
		}
		rb.velocity = direction;
		DestroyRigidbody().Forget();
	}

	protected override void OnDestroy()
	{
		destroied = true;
		base.OnDestroy();
	}

	private async UniTaskVoid DestroyRigidbody()
	{
		await UniTask.WaitForSeconds(3);
		if (destroied || !rb)
		{
			return;
		}
		if (rb.velocity.y < -0.2f)
		{
			rb.transform.position = throwStartPoint;
			rb.position = throwStartPoint;
			await UniTask.WaitForSeconds(3);
		}
		if (!destroied && (bool)rb)
		{
			if (rb.velocity.y < -0.2f)
			{
				rb.transform.position = throwStartPoint;
				rb.position = throwStartPoint;
			}
			if ((bool)rb)
			{
				UnityEngine.Object.Destroy(rb);
			}
		}
	}
}
