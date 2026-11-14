using System.Collections.Generic;
using UnityEngine;

public class RandomGenerator : MonoBehaviour
{
	[Tooltip("可拖拽的Prefab列表（请从Project窗口拖拽预制体文件）")]
	public List<GameObject> prefabs = new List<GameObject>();

	[Tooltip("生成的数量")]
	public int spawnCount = 10;

	[Tooltip("是否随机旋转")]
	public bool randomRotation = true;

	[Tooltip("随机缩放范围 (min, max)")]
	public Vector2 randomScaleRange = new Vector2(1f, 1f);

	[Tooltip("是否放置在几何体表面（从上方投射射线）")]
	public bool spawnOnSurface;

	[Tooltip("是否对齐表面法线（需开启spawnOnSurface）")]
	public bool alignToNormal;

	[Tooltip("将生成的物体作为当前几何体的子物体（方便管理）\n注意：作为子物体后，如果父级缩放改变，物体的世界大小也会改变。若需保持绝对大小不变，建议取消勾选。")]
	public bool attachAsChild;

	[Header("碰撞避免设置")]
	[Tooltip("启用后，生成物体会尝试避免相互穿模")]
	public bool avoidCollisions;

	[Tooltip("检测半径的缩放系数（大于1可增加间距）")]
	public float collisionCheckScale = 1.2f;

	[Tooltip("每个物体的最大位置尝试次数")]
	public int maxPlacementAttempts = 10;

	[Header("减面设置")]
	[Tooltip("合并后自动减面到目标面数比例（0-1）")]
	[Range(0.1f, 1f)]
	public float targetTriangleRatio = 0.5f;

	[Tooltip("是否在合并后自动减面")]
	public bool autoSimplifyAfterCombine;

	private List<GameObject> spawnedObjects = new List<GameObject>();

	[ContextMenu("Generate")]
	public void Generate()
	{
		Debug.Log($"--- 生成参数 ---\nAvoidCollisions: {avoidCollisions}, CollisionCheckScale: {collisionCheckScale}, MaxAttempts: {maxPlacementAttempts}");
		Bounds bounds = GetBounds();
		if (bounds.size == Vector3.zero)
		{
			Debug.LogError("无法获取边界！请确保几何体有Renderer或Collider组件。");
			return;
		}
		if (prefabs == null || prefabs.Count == 0)
		{
			Debug.LogWarning("没有指定任何Prefab，请先拖入Prefab。");
			return;
		}
		for (int i = 0; i < spawnCount; i++)
		{
			GameObject gameObject = prefabs[Random.Range(0, prefabs.Count)];
			if (gameObject == null)
			{
				continue;
			}
			float num = Random.Range(randomScaleRange.x, randomScaleRange.y);
			Vector3 vector = ((!avoidCollisions) ? (spawnOnSurface ? GetRandomPositionOnSurface(bounds) : GetRandomPositionInBounds(bounds)) : FindNonOverlappingPosition(gameObject, bounds, num));
			if (!(vector == Vector3.zero))
			{
				Quaternion quaternion = (randomRotation ? Random.rotation : Quaternion.identity);
				if (spawnOnSurface && alignToNormal && Physics.Raycast(new Ray(vector + Vector3.up * 100f, Vector3.down), out var hitInfo, 200f))
				{
					quaternion = Quaternion.FromToRotation(Vector3.up, hitInfo.normal) * quaternion;
				}
				GameObject gameObject2 = Object.Instantiate(gameObject, vector, quaternion, null);
				gameObject2.transform.localScale = Vector3.one * num;
				if (attachAsChild)
				{
					gameObject2.transform.SetParent(base.transform, worldPositionStays: true);
				}
				spawnedObjects.Add(gameObject2);
			}
		}
	}

	private Vector3 FindNonOverlappingPosition(GameObject prefab, Bounds bounds, float scaleFactor)
	{
		float num = GetPrefabCollisionRadius(prefab, scaleFactor) * collisionCheckScale;
		Vector3 result = Vector3.zero;
		for (int i = 0; i < maxPlacementAttempts; i++)
		{
			Vector3 vector = (spawnOnSurface ? GetRandomPositionOnSurface(bounds) : GetRandomPositionInBounds(bounds));
			bool flag = false;
			foreach (GameObject spawnedObject in spawnedObjects)
			{
				if (!(spawnedObject == null))
				{
					float objectCollisionRadius = GetObjectCollisionRadius(spawnedObject);
					float num2 = Vector3.Distance(vector, spawnedObject.transform.position);
					float num3 = num + objectCollisionRadius;
					if (num2 < num3)
					{
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				return vector;
			}
			result = vector;
		}
		Debug.Log("无法找到完全无重叠的位置，将接受轻微重叠。");
		return result;
	}

	private float GetPrefabCollisionRadius(GameObject prefab, float scaleFactor)
	{
		Collider[] componentsInChildren = prefab.GetComponentsInChildren<Collider>();
		if (componentsInChildren.Length != 0)
		{
			Bounds bounds = componentsInChildren[0].bounds;
			for (int i = 1; i < componentsInChildren.Length; i++)
			{
				bounds.Encapsulate(componentsInChildren[i].bounds);
			}
			return bounds.extents.magnitude * scaleFactor;
		}
		Renderer[] componentsInChildren2 = prefab.GetComponentsInChildren<Renderer>();
		if (componentsInChildren2.Length != 0)
		{
			Bounds bounds2 = componentsInChildren2[0].bounds;
			for (int j = 1; j < componentsInChildren2.Length; j++)
			{
				bounds2.Encapsulate(componentsInChildren2[j].bounds);
			}
			return bounds2.extents.magnitude * scaleFactor;
		}
		return 0.5f * scaleFactor;
	}

	private float GetObjectCollisionRadius(GameObject obj)
	{
		Collider[] componentsInChildren = obj.GetComponentsInChildren<Collider>();
		if (componentsInChildren.Length != 0)
		{
			Bounds bounds = componentsInChildren[0].bounds;
			for (int i = 1; i < componentsInChildren.Length; i++)
			{
				bounds.Encapsulate(componentsInChildren[i].bounds);
			}
			return bounds.extents.magnitude;
		}
		Renderer[] componentsInChildren2 = obj.GetComponentsInChildren<Renderer>();
		if (componentsInChildren2.Length != 0)
		{
			Bounds bounds2 = componentsInChildren2[0].bounds;
			for (int j = 1; j < componentsInChildren2.Length; j++)
			{
				bounds2.Encapsulate(componentsInChildren2[j].bounds);
			}
			return bounds2.extents.magnitude;
		}
		return 0.5f;
	}

	private Bounds GetBounds()
	{
		Renderer component = GetComponent<Renderer>();
		if (component != null)
		{
			return component.bounds;
		}
		Collider component2 = GetComponent<Collider>();
		if (component2 != null)
		{
			return component2.bounds;
		}
		return new Bounds(base.transform.position, Vector3.zero);
	}

	private Vector3 GetRandomPositionInBounds(Bounds bounds)
	{
		return new Vector3(Random.Range(bounds.min.x, bounds.max.x), Random.Range(bounds.min.y, bounds.max.y), Random.Range(bounds.min.z, bounds.max.z));
	}

	private Vector3 GetRandomPositionOnSurface(Bounds bounds)
	{
		Vector2 vector = new Vector2(Random.Range(bounds.min.x, bounds.max.x), Random.Range(bounds.min.z, bounds.max.z));
		float num = bounds.max.y + 10f;
		if (Physics.Raycast(new Ray(new Vector3(vector.x, num, vector.y), Vector3.down), out var hitInfo, num + 10f))
		{
			return hitInfo.point;
		}
		return GetRandomPositionInBounds(bounds);
	}

	[ContextMenu("Clear Generated Objects")]
	public void ClearGeneratedObjects()
	{
		for (int num = base.transform.childCount - 1; num >= 0; num--)
		{
			Object.DestroyImmediate(base.transform.GetChild(num).gameObject);
		}
		spawnedObjects.Clear();
	}
}
