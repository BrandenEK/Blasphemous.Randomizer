using System;
using System.Collections;
using FMODUnity;
using Framework.FrameworkCore;
using Framework.Managers;
using Framework.Util;
using Gameplay.GameControllers.Effects.Player.GhostTrail;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Enemies.Framework.Damage;
using Gameplay.GameControllers.Enemies.Projectiles;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.Weapon;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gameplay.GameControllers.Bosses.PietyMonster.ThornProjectile
{
	public class ThornProjectile : Weapon, IDamageable, IProjectileAttack
	{
		public SpriteRenderer Renderer { get; set; }

		public Transform Target { get; set; }

		public GhostTrailGenerator GhostTrail { get; set; }

		public bool IsBroken { get; set; }

		protected EnemyDamageArea DamageArea { get; set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			this._animator = base.GetComponentInChildren<Animator>();
			this.Renderer = base.GetComponentInChildren<SpriteRenderer>();
			this.GhostTrail = base.GetComponentInChildren<GhostTrailGenerator>();
			this.DamageArea = base.GetComponentInChildren<EnemyDamageArea>();
		}

		protected override void OnStart()
		{
			base.OnStart();
			this.AttackArea.OnEnter += this.AttackAreaOnEnter;
		}

		private void AttackAreaOnEnter(object sender, Collider2DParam e)
		{
			if (this.AttackingEntity.Status.Dead)
			{
				return;
			}
			GameObject gameObject = e.Collider2DArg.gameObject;
			if (gameObject.CompareTag("Penitent"))
			{
				Entity componentInParent = gameObject.GetComponentInParent<Entity>();
				if (componentInParent.Status.Unattacable)
				{
					return;
				}
				Hit weapondHit = new Hit
				{
					AttackingEntity = this.AttackingEntity.gameObject,
					DamageType = Gameplay.GameControllers.Entities.DamageArea.DamageType.Normal,
					DamageAmount = this.DamageAmount,
					HitSoundId = this.HitSound
				};
				this.Attack(weapondHit);
			}
			this.Break();
		}

		public void SetOwner(Enemy enemy)
		{
			this.AttackArea.Entity = enemy;
			this.WeaponOwner = enemy;
			this.AttackingEntity = enemy;
			this.GhostTrail.EntityOwner = enemy;
			this.DamageArea.SetOwner(enemy);
		}

		public override void Attack(Hit weapondHit)
		{
			base.GetDamageableEntities();
			base.AttackDamageableEntities(weapondHit);
		}

		private void Break()
		{
			if (this._animator == null)
			{
				return;
			}
			this.StopMovement();
			this._animator.SetTrigger("BREAK");
			if (!this.IsBroken)
			{
				this.IsBroken = true;
			}
			if (this.AttackArea.WeaponCollider.enabled)
			{
				this.AttackArea.WeaponCollider.enabled = false;
			}
			if (this.GhostTrail.EnableGhostTrail)
			{
				this.GhostTrail.EnableGhostTrail = false;
			}
			Core.Audio.PlaySfxOnCatalog("PietatSpitExplosion");
		}

		public void Throw(Vector3 target)
		{
			this._startTime = Time.time;
			this._journeyLength = Vector3.Distance(base.transform.position, target);
			this.ParabolicThrow(this.SetGoal(target));
			if (!this.GhostTrail.EnableGhostTrail)
			{
				this.GhostTrail.EnableGhostTrail = true;
			}
		}

		private Vector3 SetGoal(Vector3 target)
		{
			Vector3 result = target;
			EntityOrientation orientation = this.AttackingEntity.Status.Orientation;
			result.x = ((orientation != EntityOrientation.Left) ? Mathf.Clamp(result.x, this.AttackingEntity.transform.position.x + 3.5f, target.x) : Mathf.Clamp(result.x, target.x, this.AttackingEntity.transform.position.x - 3.5f));
			return result;
		}

		private IEnumerator StraightThrowCoroutine(Vector3 target)
		{
			while (!this.IsBroken)
			{
				float distCovered = (Time.time - this._startTime) * this.StraightThrowSpeed;
				float fracJourney = distCovered / this._journeyLength;
				base.transform.position = Vector3.Lerp(base.transform.position, target, this.SpeedCurve.Evaluate(fracJourney));
				yield return new WaitForEndOfFrame();
			}
			if (!this.IsBroken)
			{
				this.Break();
			}
			yield break;
		}

		private void ParabolicThrow(Vector3 target)
		{
			float num = target.x - base.transform.position.x;
			float num2 = target.y - base.transform.position.y;
			float f = Mathf.Atan((num2 + 7f) / num);
			float num3 = num / Mathf.Cos(f);
			float x = num3 * Mathf.Cos(f);
			float y = num3 * Mathf.Sin(f);
			Rigidbody2D component = base.GetComponent<Rigidbody2D>();
			component.velocity = new Vector2(x, y);
		}

		private IEnumerator ParabolicThrowCoroutine(Vector3 target)
		{
			this.Projectile = base.transform;
			float gravity = Mathf.Abs(Physics2D.gravity.y);
			this.Projectile.position = base.transform.position + new Vector3(0f, 0f, 0f);
			float targetDistance = Vector3.Distance(this.Projectile.position, target);
			float rockVelocity = targetDistance / (Mathf.Sin(2f * this.FiringAngle * 0.017453292f) / gravity);
			float Vx = Mathf.Sqrt(rockVelocity) * Mathf.Cos(this.FiringAngle * 0.017453292f);
			float Vy = Mathf.Sqrt(rockVelocity) * Mathf.Sin(this.FiringAngle * 0.017453292f);
			float flightDuration = targetDistance / Vx;
			this.Projectile.rotation = Quaternion.LookRotation(target - this.Projectile.position);
			this.Renderer.transform.rotation = Quaternion.LookRotation(Vector3.zero);
			float elapseTime = 0f;
			while (elapseTime < flightDuration && !this.IsBroken)
			{
				this.Projectile.Translate(0f, (Vy - gravity * elapseTime) * Time.deltaTime, Vx * Time.deltaTime);
				elapseTime += Time.deltaTime;
				yield return null;
			}
			if (!this.IsBroken)
			{
				this.Break();
			}
			yield break;
		}

		public override void OnHit(Hit weaponHit)
		{
			this.StopMovement();
		}

		public void Damage(Hit hit)
		{
			if (this.IsBroken)
			{
				return;
			}
			Core.Audio.PlaySfxOnCatalog("PietatSpitHit");
			this.Break();
		}

		public Vector3 GetPosition()
		{
			return base.transform.position;
		}

		public void HitsOnFloor()
		{
			if (this.AttackArea.WeaponCollider.enabled)
			{
				this.AttackArea.WeaponCollider.enabled = false;
			}
			this.StopMovement();
			if (this.Renderer.enabled)
			{
				this.Renderer.enabled = false;
			}
			if (this.GhostTrail.EnableGhostTrail)
			{
				this.GhostTrail.EnableGhostTrail = false;
			}
			this._animator.SetTrigger("BREAK");
			Core.Audio.PlaySfxOnCatalog("PietatSpitExplosion");
		}

		public void StopMovement()
		{
			Rigidbody2D component = base.GetComponent<Rigidbody2D>();
			if (component == null)
			{
				return;
			}
			component.velocity = Vector2.zero;
			component.isKinematic = true;
		}

		public bool BleedOnImpact()
		{
			return false;
		}

		public bool SparkOnImpact()
		{
			return false;
		}

		public void SetProjectileWeaponDamage(int damage)
		{
			this.DamageAmount = (float)damage;
		}

		public void SetProjectileWeaponDamage(Projectile projectile, int damage)
		{
		}

		public const float MinHorizontalDistance = 3.5f;

		public AttackArea AttackArea;

		public Enemy AttackingEntity;

		public Transform Projectile;

		public float StraightThrowSpeed = 5f;

		public float FiringAngle = 30f;

		[SerializeField]
		[BoxGroup("Audio", true, false, 0)]
		[EventRef]
		protected string HitSound;

		[FormerlySerializedAs("DamageAmount")]
		[SerializeField]
		[BoxGroup("Damage", true, false, 0)]
		protected float DamageAmount = 15f;

		[Tooltip("Damage factor based on entity damage base amount.")]
		[Range(0f, 1f)]
		public float DamageFactor = 0.5f;

		private float _startTime;

		private float _journeyLength;

		private Animator _animator;

		public AnimationCurve SpeedCurve;
	}
}
