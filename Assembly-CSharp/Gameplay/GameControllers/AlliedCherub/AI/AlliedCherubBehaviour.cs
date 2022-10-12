using System;
using System.Collections;
using DG.Tweening;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Bosses.Quirce.Attack;
using Gameplay.GameControllers.Bosses.Snake;
using Gameplay.GameControllers.Effects.Entity;
using Gameplay.GameControllers.Entities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.AlliedCherub.AI
{
	public class AlliedCherubBehaviour : MonoBehaviour
	{
		private AlliedCherub AlliedCherub { get; set; }

		public Entity Target { get; private set; }

		private void Awake()
		{
			this.AlliedCherub = base.GetComponent<AlliedCherub>();
			this.spriteEffects = base.GetComponentInChildren<MasterShaderEffects>();
			this.visionCone = base.GetComponentInChildren<VisionCone>();
		}

		private void Start()
		{
			this.AlliedCherub.StateMachine.SwitchState<AlliedCherubIdleState>();
		}

		private void Update()
		{
			if (!this.shooting)
			{
				this.SetOrientation();
			}
			if (this.attackCd > 0f)
			{
				this.attackCd -= Time.deltaTime;
			}
		}

		public bool CanSeeEnemy(Enemy t)
		{
			if (t is Snake)
			{
				Snake snake = t as Snake;
				DamageArea activeDamageArea = snake.GetActiveDamageArea();
				return this.visionCone.CanSeeTarget(activeDamageArea.transform, "Enemy", false);
			}
			return this.visionCone.CanSeeTarget(t.transform, "Enemy", true);
		}

		public void ChasingAlly()
		{
			Vector3 position = Core.Logic.Penitent.transform.position;
			Vector3 entityPos = position + this.AlliedCherub.FlyingOffset;
			float elongation = (!Core.Logic.Penitent.Status.IsGrounded) ? 0.15f : this.ChasingElongation;
			this.ChaseEntity(entityPos, elongation, this.ChasingSpeed);
		}

		public void ChaseEntity(Vector3 entityPos, float elongation, float speed)
		{
			this.AlliedCherub.transform.position = Vector3.SmoothDamp(this.AlliedCherub.transform.position, entityPos + Vector3.up, ref this._velocity, elongation, speed);
		}

		public bool IsInAttackRange(Vector2 v)
		{
			return Vector2.Distance(base.transform.position, v) < this.attackRange;
		}

		public bool CanAttack()
		{
			return this.attackCd <= 0f;
		}

		public bool IsShooting()
		{
			return this.shooting;
		}

		public IEnumerator ShootCoroutine(Collider2D target)
		{
			this.shooting = true;
			Vector2 dir = target.bounds.center - base.transform.position;
			float duration = 0.4f;
			if (dir.magnitude > 2f)
			{
				base.transform.DOLocalMove(base.transform.position + dir.normalized * 2f, duration, false).SetEase(Ease.OutCubic);
			}
			if (this.spriteEffects)
			{
				this.spriteEffects.TriggerColorizeLerp(0f, 1f, duration, null);
			}
			this.SetOrientation(dir);
			yield return new WaitForSeconds(duration);
			if (target != null)
			{
				dir = target.bounds.center - base.transform.position;
			}
			this.railgun.Shoot(base.transform.position, dir.normalized);
			this.shooting = false;
			this.AlliedCherub.Store();
			yield break;
		}

		public void ShootRailgun(Collider2D target)
		{
			if (target != null)
			{
				base.StartCoroutine(this.ShootCoroutine(target));
			}
		}

		private void SetOrientation()
		{
			if (this._velocity.x > 0.1f)
			{
				this.AlliedCherub.SetOrientation(EntityOrientation.Right, true, false);
			}
			else if (this._velocity.x < -0.1f)
			{
				this.AlliedCherub.SetOrientation(EntityOrientation.Left, true, false);
			}
		}

		private void SetOrientation(Vector2 dir)
		{
			if (dir.x > 0f)
			{
				this.AlliedCherub.SetOrientation(EntityOrientation.Right, true, false);
			}
			else
			{
				this.AlliedCherub.SetOrientation(EntityOrientation.Left, true, false);
			}
		}

		public void OnTargetLost()
		{
			this.AlliedCherub.StateMachine.SwitchState<AlliedCherubIdleState>();
		}

		public void Attack(Entity currentTarget)
		{
			this.SetAttackDamage();
			this.Target = currentTarget;
			this.AlliedCherub.StateMachine.SwitchState<AlliedCherubAttackState>();
		}

		public void SetAttackDamage()
		{
			this.railgun.SetDamageStrength(this.CalculateDamageStrength(Core.Logic.Penitent.Stats.PrayerStrengthMultiplier.Final));
			this.railgun.SetDamage(Mathf.RoundToInt(this.cherubBaseDamage));
		}

		private float CalculateDamageStrength(float prayerStrMult)
		{
			return 1f + 0.125f * (prayerStrMult - 1f);
		}

		private void OnEnable()
		{
			this.attackCd = 1f;
			this.spriteEffects.SetColorizeStrength(0f);
		}

		private void OnDisable()
		{
			this.AlliedCherub.StateMachine.SwitchState<AlliedCherubIdleState>();
		}

		[FoldoutGroup("Chasing player", true, 0)]
		public float ChasingElongation = 0.5f;

		[FoldoutGroup("Chasing player", true, 0)]
		public float ChasingSpeed = 5f;

		[FoldoutGroup("Chasing enemy", true, 0)]
		public float ChasingEnemyElongation = 0.25f;

		[FoldoutGroup("Chasing enemy", true, 0)]
		public float ChasingEnemySpeed = 6f;

		private Vector3 _velocity = Vector3.zero;

		public BossInstantProjectileAttack railgun;

		public MasterShaderEffects spriteEffects;

		public float attackRange = 5f;

		public VisionCone visionCone;

		public float cherubBaseDamage = 60f;

		private float attackCd = 1f;

		private bool shooting;
	}
}
