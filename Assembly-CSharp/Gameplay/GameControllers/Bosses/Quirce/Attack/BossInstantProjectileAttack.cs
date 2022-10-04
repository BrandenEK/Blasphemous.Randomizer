using System;
using System.Collections.Generic;
using FMODUnity;
using Framework.FrameworkCore;
using Framework.Managers;
using Framework.Pooling;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Enemies.Framework.Damage;
using Gameplay.GameControllers.Entities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.Quirce.Attack
{
	public class BossInstantProjectileAttack : EnemyAttack, IDirectAttack, ISpawnerAttack
	{
		public AttackArea AttackArea { get; set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			this.AttackArea = base.GetComponentInChildren<AttackArea>();
			this.alreadyInstantiated = new List<DashAttackInstantiations>();
			this.CreateHit();
		}

		protected override void OnStart()
		{
			base.OnStart();
			if (this.beamLauncherToUse != null)
			{
				PoolManager.Instance.CreatePool(this.beamLauncherToUse.gameObject, this.beamLauncherPoolSize);
			}
			foreach (DashAttackInstantiations dashAttackInstantiations in this.objectsToInstantiate)
			{
				if (dashAttackInstantiations.prefabToInstantiate.GetComponent<PoolObject>() != null)
				{
					PoolManager.Instance.CreatePool(dashAttackInstantiations.prefabToInstantiate, this.itemsPoolSize);
				}
			}
			if (this.instantiateOnHit != null)
			{
				PoolManager.Instance.CreatePool(this.instantiateOnHit, this.onHitVfxPoolSize);
			}
		}

		public void CreateHit()
		{
			if (this.dealsDamage)
			{
				this._hit = new Hit
				{
					AttackingEntity = base.EntityOwner.gameObject,
					DamageType = this.DamageType,
					Force = this.Force,
					DamageAmount = (float)this.damage * this._hitStrength,
					DamageElement = this.DamageElement,
					HitSoundId = this.HitSound,
					Unnavoidable = this.unavoidable,
					Unparriable = !this.canBeParried,
					Unblockable = !this.canBeParried,
					forceGuardslide = this.canBeParried
				};
			}
		}

		public void SetDamage(int dmg)
		{
			this.damage = dmg;
			this.CreateHit();
		}

		public void SetDamageStrength(float strength)
		{
			this._hitStrength = strength;
			this.CreateHit();
		}

		public override void CurrentWeaponAttack()
		{
			base.CurrentWeaponAttack();
		}

		public List<GameObject> Shoot(Vector2 origin, Vector2 dir)
		{
			List<GameObject> result = new List<GameObject>();
			this.alreadyInstantiated.Clear();
			if (this.shotSound != string.Empty)
			{
				Core.Audio.PlaySfx(this.shotSound, 0f);
			}
			RaycastHit2D[] array = new RaycastHit2D[1];
			bool flag = Physics2D.LinecastNonAlloc(origin, origin + dir * this.maxRange, array, this.collisionMask) > 0;
			if (flag)
			{
				Debug.DrawLine(array[0].point, array[0].point + Vector2.up * 0.25f, Color.red, 1f);
				this._targetPoint = array[0].point;
			}
			else
			{
				Debug.DrawLine(origin, origin + dir * 10f, Color.red, 1f);
				this._targetPoint = origin + dir * this.maxRange;
			}
			Debug.DrawLine(origin, this._targetPoint, Color.green, 1f);
			if (this.dealsDamage)
			{
				this._lastAttackDir = this._targetPoint - origin;
				this._lastAttackOrigin = origin;
				this._wasBlocked = false;
				this.DamageArea(origin, this._targetPoint, this.areaWidth);
				result = this.DrawAfterDamageArea(origin, dir);
			}
			else
			{
				result = this.DrawCompleteBeam(origin, dir);
			}
			return result;
		}

		private List<GameObject> DrawAfterDamageArea(Vector2 origin, Vector2 dir)
		{
			List<GameObject> list = new List<GameObject>();
			if (this._wasBlocked)
			{
				Vector2 vector = this._blockPoint - origin;
				Vector2 vector2 = -this._lastAttackDir;
				vector2.y *= -1f;
				RaycastHit2D[] array = new RaycastHit2D[1];
				bool flag = Physics2D.LinecastNonAlloc(this._blockPoint, this._blockPoint + vector2 * this.maxRange, array, this.collisionMask) > 0;
				if (flag)
				{
					Debug.DrawLine(array[0].point, array[0].point + Vector2.up * 0.25f, Color.red, 1f);
					this._targetPoint = array[0].point;
				}
				else
				{
					Debug.DrawLine(origin, origin + vector2 * 10f, Color.red, 1f);
					this._targetPoint = origin + vector2 * this.maxRange;
				}
				if (this.beamLauncherToUse != null)
				{
					TileableBeamLauncher component = PoolManager.Instance.ReuseObject(this.beamLauncherToUse.gameObject, Vector3.zero, Quaternion.identity, false, 1).GameObject.GetComponent<TileableBeamLauncher>();
					component.transform.position = origin;
					component.transform.localEulerAngles = new Vector3(0f, 0f, Mathf.Atan2(dir.y, dir.x) * 57.29578f);
					component.TriggerBeamBodyAnim();
					component.maxRange = vector.magnitude;
					Debug.DrawRay(this._blockPoint, vector2, Color.magenta, 10f);
					TileableBeamLauncher component2 = PoolManager.Instance.ReuseObject(this.beamLauncherToUse.gameObject, Vector3.zero, Quaternion.identity, false, 1).GameObject.GetComponent<TileableBeamLauncher>();
					component2.maxRange = array[0].distance;
					component2.transform.position = this._blockPoint;
					component2.transform.localEulerAngles = new Vector3(0f, 0f, Mathf.Atan2(vector2.y, vector2.x) * 57.29578f);
					component2.TriggerBeamBodyAnim();
				}
				foreach (DashAttackInstantiations objectConfig in this.objectsToInstantiate)
				{
					Vector2 p = Vector2.Lerp(this._blockPoint, this._targetPoint, objectConfig.dashMoment);
					list.Add(this.InstantiateObject(objectConfig, p, vector2));
				}
			}
			else
			{
				list = this.DrawCompleteBeam(origin, dir);
			}
			return list;
		}

		private List<GameObject> DrawCompleteBeam(Vector2 origin, Vector2 dir)
		{
			List<GameObject> list = new List<GameObject>();
			Vector2 dir2 = this._targetPoint - origin;
			foreach (DashAttackInstantiations objectConfig in this.objectsToInstantiate)
			{
				Vector2 p = Vector2.Lerp(origin, this._targetPoint, objectConfig.dashMoment);
				list.Add(this.InstantiateObject(objectConfig, p, dir2));
			}
			if (this.beamLauncherToUse != null)
			{
				TileableBeamLauncher component = PoolManager.Instance.ReuseObject(this.beamLauncherToUse.gameObject, Vector3.zero, Quaternion.identity, false, 1).GameObject.GetComponent<TileableBeamLauncher>();
				component.transform.position = origin;
				component.transform.localEulerAngles = new Vector3(0f, 0f, Mathf.Atan2(dir.y, dir.x) * 57.29578f);
				component.TriggerBeamBodyAnim();
				component.maxRange = dir2.magnitude;
			}
			return list;
		}

		private void DamageArea(Vector2 origin, Vector2 end, float width)
		{
			RaycastHit2D[] array = new RaycastHit2D[20];
			Vector2 vector = end - origin;
			Vector2 vector2 = new Vector2(-vector.y, vector.x);
			Vector2 normalized = vector2.normalized;
			Vector2 vector3 = origin + vector.normalized * 0.1f;
			BossInstantProjectileAttack.DrawDebugCross(vector3, Color.cyan, 2f);
			Vector2 size = new Vector2(width, 0.1f);
			Vector2 vector4 = origin + normalized * width / 2f;
			Vector2 vector5 = origin - normalized * width / 2f;
			Debug.DrawLine(vector4, vector4 + vector, Color.green, 1f);
			Debug.DrawLine(vector5, vector5 + vector, Color.green, 1f);
			float angle = Mathf.Atan2(vector.y, vector.x);
			int num = Physics2D.BoxCastNonAlloc(vector3, size, angle, vector, array, vector.magnitude, this.damageMask);
			if (num > 0)
			{
				for (int i = 0; i < num; i++)
				{
					if (this.instantiateOnHit)
					{
						float num2 = Vector2.Distance(array[i].point, origin);
						Vector2 v = origin + vector.normalized * (0.5f + num2);
						GameObject gameObject = PoolManager.Instance.ReuseObject(this.instantiateOnHit, v, Quaternion.identity, false, 1).GameObject;
						gameObject.GetComponentInChildren<SpriteRenderer>().flipX = (vector.x < 0f);
					}
					if (array[i].collider.CompareTag("Penitent"))
					{
						if (this.canBeParried)
						{
							this._hit.OnGuardCallback = new Action<Hit>(this.OnPenitentGuardAttack);
							this._blockPoint = array[i].point;
						}
						Core.Logic.Penitent.Damage(this._hit);
					}
					else
					{
						EnemyDamageArea component = array[i].collider.GetComponent<EnemyDamageArea>();
						if (component)
						{
							((IDamageable)component.OwnerEntity).Damage(this._hit);
						}
						else
						{
							IDamageable damageable = array[i].collider.GetComponentInChildren<IDamageable>() ?? array[i].collider.GetComponentInParent<IDamageable>();
							if (damageable != null)
							{
								damageable.Damage(this._hit);
								if (Mathf.Abs(this.slowTimeDuration) > Mathf.Epsilon)
								{
									Core.Logic.ScreenFreeze.Freeze(0.1f, this.slowTimeDuration, 0f, this.slowTimeCurve);
								}
							}
						}
					}
					BossInstantProjectileAttack.DrawDebugCross(array[i].point, Color.magenta, 2f);
				}
			}
		}

		private void OnPenitentGuardAttack(Hit h)
		{
			Vector2 vector = (Core.Logic.Penitent.Status.Orientation != EntityOrientation.Right) ? Vector2.left : Vector2.right;
			this._wasBlocked = true;
			Core.Logic.ScreenFreeze.Freeze(0.15f, 0.2f, 0f, null);
		}

		private static void DrawDebugCross(Vector2 point, Color c, float seconds)
		{
			float d = 0.6f;
			Debug.DrawLine(point - Vector2.up * d, point + Vector2.up * d, c, seconds);
			Debug.DrawLine(point - Vector2.right * d, point + Vector2.right * d, c, seconds);
		}

		private GameObject InstantiateObject(DashAttackInstantiations objectConfig, Vector2 p, Vector2 dir)
		{
			this.alreadyInstantiated.Add(objectConfig);
			GameObject gameObject;
			if (objectConfig.prefabToInstantiate.GetComponent<PoolObject>())
			{
				gameObject = PoolManager.Instance.ReuseObject(objectConfig.prefabToInstantiate, p, Quaternion.identity, false, 1).GameObject;
			}
			else
			{
				gameObject = UnityEngine.Object.Instantiate<GameObject>(objectConfig.prefabToInstantiate, p, Quaternion.identity);
			}
			if (!objectConfig.keepRotation)
			{
				gameObject.transform.localEulerAngles = new Vector3(0f, 0f, Mathf.Atan2(dir.y, dir.x) * 57.29578f);
			}
			return gameObject;
		}

		public void SetSpawnsDamage(int damage)
		{
			foreach (DashAttackInstantiations dashAttackInstantiations in this.objectsToInstantiate)
			{
				IDirectAttack component = dashAttackInstantiations.prefabToInstantiate.GetComponent<IDirectAttack>();
				if (component != null)
				{
					component.SetDamage(damage);
				}
			}
			this.CreateSpawnsHits();
		}

		public void CreateSpawnsHits()
		{
			foreach (DashAttackInstantiations dashAttackInstantiations in this.objectsToInstantiate)
			{
				IDirectAttack component = dashAttackInstantiations.prefabToInstantiate.GetComponent<IDirectAttack>();
				if (component != null)
				{
					component.CreateHit();
				}
			}
		}

		public LayerMask collisionMask;

		public LayerMask damageMask;

		private Vector3 _targetPoint;

		private Coroutine _currentCoroutine;

		public Vector2 shootOrigin;

		[FoldoutGroup("Instantiations", 0)]
		public GameObject instantiateOnBegin;

		[FoldoutGroup("Instantiations", 0)]
		public GameObject instantiateOnEnd;

		[FoldoutGroup("Instantiations", 0)]
		public GameObject instantiateOnHit;

		[FoldoutGroup("Instantiations", 0)]
		public List<DashAttackInstantiations> objectsToInstantiate;

		public float areaWidth = 3f;

		public float maxRange = 10f;

		[FoldoutGroup("Damage", 0)]
		public bool dealsDamage = true;

		[ShowIf("dealsDamage", true)]
		[FoldoutGroup("Damage", 0)]
		public bool unavoidable;

		[ShowIf("dealsDamage", true)]
		[FoldoutGroup("Damage", 0)]
		public bool canBeParried;

		[ShowIf("dealsDamage", true)]
		[FoldoutGroup("Damage", 0)]
		public int damage;

		[ShowIf("dealsDamage", true)]
		[FoldoutGroup("Damage", 0)]
		public AnimationCurve slowTimeCurve;

		[ShowIf("dealsDamage", true)]
		[FoldoutGroup("Damage", 0)]
		public float slowTimeDuration;

		[EventRef]
		public string shotSound;

		[FoldoutGroup("Instantiations", 0)]
		public TileableBeamLauncher beamLauncherToUse;

		[FoldoutGroup("Instantiations", 0)]
		public int beamLauncherPoolSize = 1;

		[FoldoutGroup("Instantiations", 0)]
		public int itemsPoolSize = 1;

		[FoldoutGroup("Instantiations", 0)]
		public int onHitVfxPoolSize = 1;

		private List<DashAttackInstantiations> alreadyInstantiated;

		private Hit _hit;

		private Vector2 _lastAttackOrigin;

		private Vector2 _lastAttackDir;

		private bool _wasBlocked;

		private Vector2 _blockPoint;

		private float _hitStrength = 1f;
	}
}
