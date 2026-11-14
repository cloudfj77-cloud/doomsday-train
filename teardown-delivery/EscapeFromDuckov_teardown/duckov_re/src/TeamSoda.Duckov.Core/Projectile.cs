using System;
using System.Collections.Generic;
using System.Linq;
using Duckov.Utilities;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;

public class Projectile : MonoBehaviour
{
	public ProjectileContext context;

	public float radius;

	private float traveledDistance;

	private List<RaycastHit> hits;

	private LayerMask hitLayers;

	private Vector3 hitPoint;

	private Vector3 hitNormal;

	private bool dead;

	private bool overMaxDistance;

	[SerializeField]
	private GameObject hitFx;

	private Vector3 direction;

	private Vector3 velocity;

	private float gravity;

	[HideInInspector]
	public List<GameObject> damagedObjects;

	public static Action<Vector3> OnBulletFlyByCharacter;

	private bool flyThroughCharacterSoundPlayed;

	private bool firstFrame = true;

	private Vector3 startPoint;

	private ObjectPool<Projectile> pool;

	[SerializeField]
	private TrailRenderer trail;

	[SerializeField]
	private List<TrailRenderer> otherTrails;

	[FormerlySerializedAs("spin")]
	public Transform randomRotate;

	public bool random;

	public MeshRenderer mesh;

	public string colorName = "_Tint";

	[ColorUsage(true, true)]
	public List<Color> colors = new List<Color>();

	private float traceLerpValue;

	public GameObject explosionFx;

	private bool inited;

	private DamageReceiver _dmgReceiverTemp;

	private float _distanceThisFrame;

	private int _hitCount;

	public void SetPool(ObjectPool<Projectile> _pool)
	{
		pool = _pool;
	}

	private void Release()
	{
		if (pool == null)
		{
			Debug.Log("Destroy");
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else
		{
			pool.Release(this);
		}
	}

	private void Awake()
	{
		if (!inited)
		{
			inited = true;
			Init();
		}
	}

	private void ClearTrails()
	{
		if ((bool)trail)
		{
			trail.Clear();
		}
		if (otherTrails.Count <= 0)
		{
			return;
		}
		foreach (TrailRenderer otherTrail in otherTrails)
		{
			if (otherTrail != null)
			{
				otherTrail.Clear();
			}
		}
	}

	public void Init()
	{
		inited = true;
		damagedObjects = new List<GameObject>();
		damagedObjects.Clear();
		traveledDistance = 0f;
		dead = false;
		overMaxDistance = false;
		flyThroughCharacterSoundPlayed = false;
		firstFrame = true;
		traceLerpValue = 0f;
		hitLayers = (int)GameplayDataSettings.Layers.damageReceiverLayerMask | (int)GameplayDataSettings.Layers.wallLayerMask | (int)GameplayDataSettings.Layers.groundLayerMask | (int)GameplayDataSettings.Layers.blockBulletLayers;
		ClearTrails();
		if ((bool)randomRotate)
		{
			randomRotate.localRotation = Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(0f, 360f));
		}
		if (random)
		{
			Color color = colors.GetRandom();
			if (mesh != null)
			{
				mesh.material.SetColor(colorName, color);
			}
			if ((bool)randomRotate)
			{
				randomRotate.localRotation = Quaternion.Euler(UnityEngine.Random.Range(0f, 360f), UnityEngine.Random.Range(0f, 360f), UnityEngine.Random.Range(0f, 360f));
			}
			if (trail != null)
			{
				color.a = 1f;
				trail.startColor = color;
				color.a = 0f;
				trail.endColor = color;
			}
		}
	}

	private void AddElementsToDamageInfo(ref DamageInfo damageInfo)
	{
		damageInfo.AddElementFactor(ElementTypes.physics, context.element_Physics);
		damageInfo.AddElementFactor(ElementTypes.fire, context.element_Fire);
		damageInfo.AddElementFactor(ElementTypes.poison, context.element_Poison);
		damageInfo.AddElementFactor(ElementTypes.electricity, context.element_Electricity);
		damageInfo.AddElementFactor(ElementTypes.space, context.element_Space);
		damageInfo.AddElementFactor(ElementTypes.ghost, context.element_Ghost);
		damageInfo.AddElementFactor(ElementTypes.ice, context.element_Ice);
	}

	private void Update()
	{
		if (dead)
		{
			Release();
			return;
		}
		UpdateMoveAndCheck();
		if (dead)
		{
			if (firstFrame)
			{
				ClearTrails();
			}
			if (context.explosionRange > 0f)
			{
				DamageInfo damageInfo = new DamageInfo(context.fromCharacter);
				AddElementsToDamageInfo(ref damageInfo);
				damageInfo.damageValue = context.explosionDamage;
				damageInfo.damageFactorToZombie = context.damageFactorToZombie;
				damageInfo.fromWeaponItemID = context.fromWeaponItemID;
				damageInfo.armorPiercing = context.armorPiercing;
				damageInfo.damageFactorToZombie = context.damageFactorToZombie;
				ExplosionFxTypes explosionFxTypes = ExplosionFxTypes.normal;
				if (explosionFx != null)
				{
					explosionFxTypes = ExplosionFxTypes.custom;
					UnityEngine.Object.Instantiate(explosionFx, base.transform.position, Quaternion.identity);
				}
				float shakeStrength = 1f;
				if (explosionFxTypes != ExplosionFxTypes.normal)
				{
					shakeStrength = 0f;
				}
				LevelManager.Instance.ExplosionManager.CreateExplosion(base.transform.position, context.explosionRange, damageInfo, explosionFxTypes, shakeStrength);
			}
			Release();
		}
		UpdateFlyThroughSound();
		firstFrame = false;
	}

	private void UpdateFlyThroughSound()
	{
		if (dead || context.team == Teams.player || flyThroughCharacterSoundPlayed || CharacterMainControl.Main == null || velocity.magnitude < 9f)
		{
			return;
		}
		Vector3 lhs = CharacterMainControl.Main.transform.position - base.transform.position;
		lhs.y = 0f;
		if (!(lhs.magnitude > 5f))
		{
			lhs.Normalize();
			if (!(Vector3.Dot(lhs, velocity) > 0f))
			{
				flyThroughCharacterSoundPlayed = true;
				OnBulletFlyByCharacter?.Invoke(base.transform.position);
			}
		}
	}

	public void Init(ProjectileContext _context)
	{
		Init();
		context = _context;
		if (context.traceTarget != null)
		{
			context.ignoreHalfObsticle = true;
			context.critRate = 1f;
		}
		direction = context.direction;
		velocity = context.speed * direction;
		gravity = Mathf.Abs(context.gravity);
		UpdateAimDirection();
	}

	private void UpdateMoveAndCheck()
	{
		if (firstFrame)
		{
			startPoint = base.transform.position;
		}
		float num = Time.deltaTime;
		if (num > 0.04f)
		{
			num = 0.04f;
		}
		traceLerpValue = Mathf.MoveTowards(traceLerpValue, 1f, context.traceAbility * num);
		if (context.traceTarget != null)
		{
			Vector3 b = context.traceTarget.characterModel.HelmatSocket.position - base.transform.position;
			b.Normalize();
			direction = Vector3.Lerp(context.direction, b, traceLerpValue);
		}
		else
		{
			velocity.y -= num * gravity;
			direction = velocity.normalized;
		}
		UpdateAimDirection();
		_distanceThisFrame = velocity.magnitude * num;
		if (_distanceThisFrame + traveledDistance > context.distance)
		{
			_distanceThisFrame = context.distance - traveledDistance;
			overMaxDistance = true;
		}
		Vector3 origin = base.transform.position - base.transform.forward * 0.1f;
		if (firstFrame && context.firstFrameCheck)
		{
			origin = context.firstFrameCheckStartPoint;
		}
		hits = Physics.SphereCastAll(origin, radius, direction, _distanceThisFrame + 0.3f, hitLayers, QueryTriggerInteraction.Ignore).ToList();
		int count = hits.Count;
		if (count > 0)
		{
			hits.Sort(delegate(RaycastHit a, RaycastHit raycastHit2)
			{
				float num4 = a.distance - raycastHit2.distance;
				if (num4 > 0f)
				{
					return 1;
				}
				return (num4 < 0f) ? (-1) : 0;
			});
			for (int num2 = 0; num2 < count; num2++)
			{
				RaycastHit raycastHit = hits[num2];
				hitPoint = raycastHit.point;
				if (raycastHit.distance <= 0f)
				{
					hitPoint = raycastHit.collider.transform.position;
				}
				if (damagedObjects.Contains(hits[num2].collider.gameObject) || (context.ignoreHalfObsticle && GameplayDataSettings.LayersData.IsLayerInLayerMask(hits[num2].collider.gameObject.layer, GameplayDataSettings.Layers.halfObsticleLayer)))
				{
					continue;
				}
				damagedObjects.Add(hits[num2].collider.gameObject);
				if (((int)GameplayDataSettings.Layers.damageReceiverLayerMask & (1 << hits[num2].collider.gameObject.layer)) != 0)
				{
					_dmgReceiverTemp = hits[num2].collider.GetComponent<DamageReceiver>();
					if ((context.team == _dmgReceiverTemp.Team && _dmgReceiverTemp.Team != Teams.all) || (_dmgReceiverTemp.isHalfObsticle && context.ignoreHalfObsticle))
					{
						continue;
					}
				}
				else
				{
					_dmgReceiverTemp = null;
				}
				if ((bool)_dmgReceiverTemp)
				{
					bool flag = true;
					if ((bool)_dmgReceiverTemp.health)
					{
						CharacterMainControl characterMainControl = _dmgReceiverTemp.health.TryGetCharacter();
						if ((bool)characterMainControl && context.realFromCharacter == characterMainControl)
						{
							flag = false;
						}
						else if ((bool)characterMainControl && _dmgReceiverTemp.health.TryGetCharacter().Dashing)
						{
							flag = false;
						}
						if (flag && context.critRate > 0.99f && (bool)characterMainControl && context.fromCharacter == CharacterMainControl.Main && characterMainControl.canBeControlVia == context.controlMindType && context.controlMindType != ControlMindTypes.none)
						{
							CharacterMainControl.Main.ControlOtherCharacter(characterMainControl, context.controlMindTime);
						}
					}
					if (!flag)
					{
						continue;
					}
					DamageInfo damageInfo = new DamageInfo(context.fromCharacter);
					damageInfo.damageValue = context.damage;
					damageInfo.damageFactorToZombie = context.damageFactorToZombie;
					if (context.halfDamageDistance > 0f && Vector3.Distance(startPoint, hitPoint) > context.halfDamageDistance)
					{
						float num3 = context.dmgOverDistance;
						if (num3 <= 0f)
						{
							num3 = 0.5f;
						}
						damageInfo.damageValue *= num3;
					}
					damageInfo.critDamageFactor = context.critDamageFactor;
					damageInfo.critRate = context.critRate;
					damageInfo.armorPiercing = context.armorPiercing;
					damageInfo.armorBreak = context.armorBreak;
					AddElementsToDamageInfo(ref damageInfo);
					damageInfo.damagePoint = hitPoint;
					damageInfo.buffChance = context.buffChance;
					damageInfo.buff = context.buff;
					damageInfo.bleedChance = context.bleedChance;
					damageInfo.damageType = DamageTypes.normal;
					damageInfo.fromWeaponItemID = context.fromWeaponItemID;
					damageInfo.damageNormal = raycastHit.normal.normalized;
					_dmgReceiverTemp.Hurt(damageInfo);
					_dmgReceiverTemp.AddBuff(GameplayDataSettings.Buffs.Pain, context.fromCharacter);
					if ((bool)context.fromGunItemSetting)
					{
						context.fromGunItemSetting.TriggerOnHurtEnemyEvent(_dmgReceiverTemp, damageInfo);
					}
					context.penetrate--;
					if (context.penetrate < 0)
					{
						base.transform.position = hitPoint;
						dead = true;
						break;
					}
					continue;
				}
				bool flag2 = false;
				if (GameplayDataSettings.LayersData.IsLayerInLayerMask(hits[num2].collider.gameObject.layer, GameplayDataSettings.Layers.blockBulletLayers))
				{
					BulletBlocker component = hits[num2].collider.gameObject.GetComponent<BulletBlocker>();
					if (component != null)
					{
						flag2 = Team.IsEnemy(context.team, component.team);
						if (component.team == Teams.all)
						{
							flag2 = true;
						}
						if (flag2 && component.checkDirection && Vector3.Angle(-component.transform.forward, direction) > 90f)
						{
							flag2 = false;
						}
						if (!flag2)
						{
							continue;
						}
						if (component.bulletBack)
						{
							traceLerpValue = 0f;
							if (context.traceTarget != null)
							{
								context.traceTarget = context.fromCharacter;
							}
							context.team = component.team;
							traveledDistance = 0f;
							context.direction = -context.direction;
							context.penetrate++;
							direction = -direction;
							velocity.y = 0f;
							velocity = -velocity;
							UnityEngine.Object.Instantiate(GameplayDataSettings.Prefabs.BulletBackFx, hitPoint, Quaternion.LookRotation(raycastHit.normal, Vector3.up));
							context.fromCharacter = component.from;
							context.realFromCharacter = component.from;
							context.damage *= 1.5f;
							context.critRate = 1f;
							damagedObjects.Clear();
							damagedObjects.Add(component.gameObject);
							if ((bool)component.from && component.from.IsMainCharacter)
							{
								GameManager.TimeScaleManager.EnterBulletTime(0.08f);
							}
							break;
						}
					}
				}
				dead = true;
				base.transform.position = hitPoint;
				if (flag2)
				{
					UnityEngine.Object.Instantiate(GameplayDataSettings.Prefabs.BlockBulletFx, hitPoint, Quaternion.LookRotation(raycastHit.normal, Vector3.up));
				}
				else if ((bool)hitFx)
				{
					UnityEngine.Object.Instantiate(hitFx, hitPoint, Quaternion.LookRotation(raycastHit.normal, Vector3.up));
				}
				else
				{
					UnityEngine.Object.Instantiate(GameplayDataSettings.Prefabs.BulletHitObsticleFx, hitPoint, Quaternion.LookRotation(raycastHit.normal, Vector3.up));
				}
				break;
			}
		}
		if (overMaxDistance)
		{
			dead = true;
		}
		if (!dead)
		{
			base.transform.position += direction * _distanceThisFrame;
			traveledDistance += _distanceThisFrame;
		}
	}

	private void UpdateAimDirection()
	{
		base.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
	}
}
