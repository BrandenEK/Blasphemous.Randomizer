using System;
using Framework.Managers;
using Gameplay.GameControllers.Effects.Entity;
using Gameplay.GameControllers.Effects.Player.GhostTrail;
using Gameplay.GameControllers.Enemies.BellGhost.AI;
using Gameplay.GameControllers.Enemies.BellGhost.Attack;
using Gameplay.GameControllers.Enemies.BellGhost.Audio;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Enemies.Framework.Damage;
using Gameplay.GameControllers.Enemies.Framework.Physics;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.BellGhost
{
	public class BellGhost : Enemy, IDamageable
	{
		public SpriteRenderer Sprite { get; private set; }

		public BellGhostAttack BellGhostAttack { get; private set; }

		public AttackArea AttackArea { get; private set; }

		public MotionLerper MotionLerper { get; private set; }

		public BellGhostFloatingMotion FloatingMotion { get; private set; }

		public BellGhostBehaviour Behaviour { get; private set; }

		public GhostTrailGenerator GhostTrail { get; set; }

		public BellGhostAudio Audio { get; set; }

		public ColorFlash ColorFlash { get; private set; }

		public void Damage(Hit hit)
		{
			this.DamageArea.TakeDamage(hit, false);
			this.ColorFlash.TriggerColorFlash();
			if (this.Status.Dead)
			{
				this.Behaviour.AnimatorInyector.Death();
				this.Audio.Death();
			}
			else
			{
				this.Behaviour.AnimatorInyector.Hurt();
				this.Behaviour.HurtDisplacement(hit.AttackingEntity);
				if (this.Behaviour.bellGhostVariant == BELL_GHOST_TYPES.BULLET)
				{
					this.Audio.HurtVariant();
				}
				else
				{
					this.Audio.Hurt();
				}
			}
			this.SleepTimeByHit(hit);
		}

		public Vector3 GetPosition()
		{
			return base.transform.position;
		}

		protected override void OnAwake()
		{
			base.OnAwake();
			this.MotionLerper = base.GetComponent<MotionLerper>();
			this.Audio = base.GetComponentInChildren<BellGhostAudio>();
			this.Behaviour = base.GetComponent<BellGhostBehaviour>();
			this.Sprite = base.GetComponentInChildren<SpriteRenderer>();
			this.DamageArea = base.GetComponentInChildren<EnemyDamageArea>();
			this.BellGhostAttack = base.GetComponentInChildren<BellGhostAttack>();
			this.FloatingMotion = base.GetComponentInChildren<BellGhostFloatingMotion>();
			this.AttackArea = base.GetComponentInChildren<AttackArea>();
			this.GhostTrail = base.GetComponentInChildren<GhostTrailGenerator>();
			this.ColorFlash = base.GetComponentInChildren<ColorFlash>();
			this.Behaviour.enabled = false;
			this.Behaviour.PauseBehaviour();
		}

		protected override void OnStart()
		{
			base.OnStart();
			if (this.BellGhostAttack)
			{
				BellGhostAttack bellGhostAttack = this.BellGhostAttack;
				bellGhostAttack.OnEntityAttack = (Core.GenericEvent)Delegate.Combine(bellGhostAttack.OnEntityAttack, new Core.GenericEvent(this.bellGhost_OnEntityAttack));
			}
			this.FloatingMotion.IsFloating = true;
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			if (base.Target != null)
			{
				base.DistanceToTarget = Vector2.Distance(base.transform.position, base.Target.transform.position);
			}
			else
			{
				base.Target = Core.Logic.Penitent.gameObject;
			}
			this.Status.IsVisibleOnCamera = this.IsVisible();
			if (base.Target && !this.Behaviour.enabled)
			{
				this.Behaviour.enabled = true;
			}
			this.DamageArea.DamageAreaCollider.enabled = (this.spriteRenderer.color.a > 0.5f);
			if (this.Status.Dead)
			{
				this.DamageArea.DamageAreaCollider.enabled = false;
				return;
			}
			bool flag = base.DistanceToTarget <= this.ActivationRange;
			if (flag)
			{
				this.Behaviour.StartBehaviour();
			}
			else
			{
				this.Behaviour.PauseBehaviour();
			}
		}

		public float TargetDistance
		{
			get
			{
				return Vector2.Distance(base.transform.position, base.Target.transform.position);
			}
		}

		private void bellGhost_OnEntityAttack(object hit)
		{
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			if (this.BellGhostAttack)
			{
				BellGhostAttack bellGhostAttack = this.BellGhostAttack;
				bellGhostAttack.OnEntityAttack = (Core.GenericEvent)Delegate.Remove(bellGhostAttack.OnEntityAttack, new Core.GenericEvent(this.bellGhost_OnEntityAttack));
			}
		}

		public override EnemyAttack EnemyAttack()
		{
			return this.BellGhostAttack;
		}

		public override EnemyBumper EnemyBumper()
		{
			throw new NotImplementedException();
		}

		public override EnemyFloorChecker EnemyFloorChecker()
		{
			throw new NotImplementedException();
		}

		protected override void EnablePhysics(bool enable)
		{
			throw new NotImplementedException();
		}

		public bool IsVisible()
		{
			return Entity.IsVisibleFrom(this.Sprite, Camera.main);
		}

		protected EnemyDamageArea DamageArea;
	}
}
