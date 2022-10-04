using System;
using Framework.Managers;
using Gameplay.GameControllers.Effects.Entity;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Enemies.Framework.Damage;
using Gameplay.GameControllers.Enemies.Framework.Physics;
using Gameplay.GameControllers.Enemies.PontiffHusk.AI;
using Gameplay.GameControllers.Enemies.PontiffHusk.Attack;
using Gameplay.GameControllers.Enemies.PontiffHusk.Audio;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.PontiffHusk
{
	public class PontiffHuskMelee : Enemy, IDamageable
	{
		public SpriteRenderer Sprite { get; private set; }

		public PontiffHuskMeleeAttack PontiffHuskMeleeAttack { get; private set; }

		public AttackArea AttackArea { get; private set; }

		public MotionLerper MotionLerper { get; private set; }

		public PontiffHuskFloatingMotion FloatingMotion { get; private set; }

		public PontiffHuskMeleeBehaviour Behaviour { get; private set; }

		public PontiffHuskAudio Audio { get; set; }

		public ColorFlash ColorFlash { get; private set; }

		public void Damage(Hit hit)
		{
			this.DamageArea.TakeDamage(hit, false);
			this.ColorFlash.TriggerColorFlash();
			if (this.Status.Dead)
			{
				bool cut = hit.DamageElement == Gameplay.GameControllers.Entities.DamageArea.DamageElement.Normal;
				this.Behaviour.AnimatorInyector.Death(cut);
				this.Audio.Death(cut);
			}
			else
			{
				this.Behaviour.AnimatorInyector.Hurt();
				this.Behaviour.HurtDisplacement(hit.AttackingEntity);
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
			this.Audio = base.GetComponentInChildren<PontiffHuskAudio>();
			this.Behaviour = base.GetComponent<PontiffHuskMeleeBehaviour>();
			this.Sprite = base.GetComponentInChildren<SpriteRenderer>();
			this.DamageArea = base.GetComponentInChildren<EnemyDamageArea>();
			this.PontiffHuskMeleeAttack = base.GetComponentInChildren<PontiffHuskMeleeAttack>();
			this.FloatingMotion = base.GetComponentInChildren<PontiffHuskFloatingMotion>();
			this.AttackArea = base.GetComponentInChildren<AttackArea>();
			this.ColorFlash = base.GetComponentInChildren<ColorFlash>();
			this.Behaviour.enabled = false;
			this.Behaviour.PauseBehaviour();
		}

		protected override void OnStart()
		{
			base.OnStart();
			if (this.PontiffHuskMeleeAttack)
			{
				PontiffHuskMeleeAttack pontiffHuskMeleeAttack = this.PontiffHuskMeleeAttack;
				pontiffHuskMeleeAttack.OnEntityAttack = (Core.GenericEvent)Delegate.Combine(pontiffHuskMeleeAttack.OnEntityAttack, new Core.GenericEvent(this.PontiffHuskMelee_OnEntityAttack));
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

		private void PontiffHuskMelee_OnEntityAttack(object hit)
		{
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			if (this.PontiffHuskMeleeAttack)
			{
				PontiffHuskMeleeAttack pontiffHuskMeleeAttack = this.PontiffHuskMeleeAttack;
				pontiffHuskMeleeAttack.OnEntityAttack = (Core.GenericEvent)Delegate.Remove(pontiffHuskMeleeAttack.OnEntityAttack, new Core.GenericEvent(this.PontiffHuskMelee_OnEntityAttack));
			}
		}

		public override EnemyAttack EnemyAttack()
		{
			return this.PontiffHuskMeleeAttack;
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

		public EnemyDamageArea DamageArea;
	}
}
