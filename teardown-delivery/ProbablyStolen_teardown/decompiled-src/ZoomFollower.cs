using UnityEngine;

public class ZoomFollower : MonoBehaviour
{
	public Transform target;

	public float targetNormalScale = 1f;

	[SerializeField]
	private Vector3 baseLocalOffset;

	[SerializeField]
	private Vector3 baseSelfScale = Vector3.one;

	[SerializeField]
	private bool hasBaseline;

	[ContextMenu("Capture Baseline (do this at normal zoom)")]
	private void CaptureBaseline()
	{
		if (!(target == null))
		{
			baseLocalOffset = target.InverseTransformPoint(base.transform.position);
			baseSelfScale = base.transform.localScale;
			hasBaseline = true;
		}
	}

	public void SetTarget(Transform master, float masterNormalScale, float masterCurrentScale)
	{
		target = master;
		targetNormalScale = masterNormalScale;
		if (!(master == null))
		{
			float num = masterCurrentScale / masterNormalScale;
			baseLocalOffset = target.InverseTransformPoint(base.transform.position);
			baseSelfScale = new Vector3(base.transform.localScale.x / num, base.transform.localScale.y / num, base.transform.localScale.z);
			hasBaseline = true;
		}
	}

	private void LateUpdate()
	{
		if (!(target == null) && hasBaseline)
		{
			if (base.transform.parent != target.parent)
			{
				base.transform.SetParent(target.parent, worldPositionStays: true);
				Vector3 localPosition = base.transform.localPosition;
				localPosition.z = 0f;
				base.transform.localPosition = localPosition;
			}
			float num = target.localScale.x / targetNormalScale;
			base.transform.position = target.TransformPoint(baseLocalOffset);
			base.transform.localScale = new Vector3(baseSelfScale.x * num, baseSelfScale.y * num, baseSelfScale.z);
		}
	}
}
