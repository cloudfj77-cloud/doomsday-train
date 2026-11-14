using System.Reflection;
using UnityEngine;

public class BulletRandomDirection : MonoBehaviour
{
	private static readonly FieldInfo DirectionField = typeof(Projectile).GetField("direction", BindingFlags.Instance | BindingFlags.NonPublic);

	private static readonly FieldInfo VelocityField = typeof(Projectile).GetField("velocity", BindingFlags.Instance | BindingFlags.NonPublic);

	[SerializeField]
	private Vector2 firstRandomTimeRange = new Vector2(0.3f, 0.8f);

	[SerializeField]
	private Vector2 randomTimeRange = new Vector2(0.3f, 0.8f);

	[SerializeField]
	private Vector2 angleRange = new Vector2(60f, 120f);

	[SerializeField]
	private bool reverseDirectionEveryTime;

	[SerializeField]
	private bool halveFirstRandomAngle;

	private Projectile projectile;

	private float timer;

	private float nextTurnTime;

	private int lastTurnDirection;

	private bool waitingForLaunch;

	private bool firstTurn;

	private void Awake()
	{
		projectile = GetComponent<Projectile>();
	}

	private void OnEnable()
	{
		timer = 0f;
		lastTurnDirection = 0;
		waitingForLaunch = true;
		firstTurn = true;
	}

	private void Update()
	{
		if (projectile == null)
		{
			return;
		}
		if (waitingForLaunch)
		{
			if (projectile.context.speed <= 0f)
			{
				return;
			}
			waitingForLaunch = false;
			timer = 0f;
			ResetNextTurnTime(firstRandomTimeRange);
		}
		timer += Time.deltaTime;
		if (!(timer < nextTurnTime))
		{
			timer = 0f;
			RotateRandomDirection();
			ResetNextTurnTime(randomTimeRange);
		}
	}

	private void RotateRandomDirection()
	{
		float minInclusive = Mathf.Min(angleRange.x, angleRange.y);
		float maxInclusive = Mathf.Max(angleRange.x, angleRange.y);
		float num = Random.Range(minInclusive, maxInclusive);
		if (firstTurn && halveFirstRandomAngle)
		{
			num *= 0.5f;
		}
		num *= (float)GetNextTurnDirection();
		firstTurn = false;
		RotateProjectileDirectionOnXZ(num);
	}

	private void RotateProjectileDirectionOnXZ(float angle)
	{
		if (DirectionField == null || VelocityField == null)
		{
			return;
		}
		Quaternion quaternion = Quaternion.AngleAxis(angle, Vector3.up);
		Vector3 vector = (Vector3)DirectionField.GetValue(projectile);
		Vector3 vector2 = (Vector3)VelocityField.GetValue(projectile);
		Vector3 vector3 = vector2;
		vector3.y = 0f;
		if (vector3.sqrMagnitude > 0.0001f)
		{
			vector3 = quaternion * vector3;
			vector2 = new Vector3(vector3.x, vector2.y, vector3.z);
			vector = vector2.normalized;
		}
		else
		{
			Vector3 vector4 = vector;
			vector4.y = 0f;
			if (vector4.sqrMagnitude <= 0.0001f)
			{
				vector4 = base.transform.forward;
				vector4.y = 0f;
			}
			vector4 = quaternion * vector4.normalized;
			vector = new Vector3(vector4.x, vector.y, vector4.z).normalized;
			vector2 = vector * projectile.context.speed;
		}
		ProjectileContext context = projectile.context;
		Vector3 vector5 = vector;
		vector5.y = 0f;
		if (vector5.sqrMagnitude > 0.0001f)
		{
			context.direction = vector5.normalized;
			projectile.context = context;
		}
		DirectionField.SetValue(projectile, vector);
		VelocityField.SetValue(projectile, vector2);
		base.transform.rotation = Quaternion.LookRotation(vector, Vector3.up);
	}

	private int GetNextTurnDirection()
	{
		if (reverseDirectionEveryTime)
		{
			if (lastTurnDirection == 0)
			{
				lastTurnDirection = ((!(Random.value < 0.5f)) ? 1 : (-1));
				return lastTurnDirection;
			}
			lastTurnDirection = -lastTurnDirection;
		}
		else
		{
			lastTurnDirection = ((!(Random.value < 0.5f)) ? 1 : (-1));
		}
		return lastTurnDirection;
	}

	private void ResetNextTurnTime(Vector2 timeRange)
	{
		float num = Mathf.Max(0f, Mathf.Min(timeRange.x, timeRange.y));
		float maxInclusive = Mathf.Max(num, Mathf.Max(timeRange.x, timeRange.y));
		nextTurnTime = Random.Range(num, maxInclusive);
	}
}
