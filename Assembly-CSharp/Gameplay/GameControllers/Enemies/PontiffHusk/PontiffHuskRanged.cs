using System;
using Framework.Managers;
using Gameplay.GameControllers.Effects.Entity;
using Gameplay.GameControllers.Effects.Player.GhostTrail;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Enemies.Framework.Damage;
using Gameplay.GameControllers.Enemies.Framework.Physics;
using Gameplay.GameControllers.Enemies.PontiffHusk.AI;
using Gameplay.GameControllers.Enemies.PontiffHusk.Attack;
using Gameplay.GameControllers.Enemies.PontiffHusk.Audio;
using Gameplay.GameControllers.Entities;
using NodeCanvas.BehaviourTrees;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.PontiffHusk
{
	public class PontiffHuskRanged : Enemy, IDamageable
	{
		public SpriteRenderer Sprite { get; private set; }

		public PontiffHuskRangedAttack PontiffHuskRangedAttack { get; private set; }

		public AttackArea AttackArea { get; private set; }

		public MotionLerper MotionLerper { get; private set; }

		public PontiffHuskFloatingMotion FloatingMotion { get; private set; }

		public PontiffHuskRangedBehaviour Behaviour { get; private set; }

		public GhostTrailGenerator GhostTrail { get; set; }

		public PontiffHuskAudio Audio { get; set; }

		public ColorFlash ColorFlash { get; private set; }

		public BehaviourTreeOwner BehaviourTree { get; private set; }

		public VisionCone VisionCone { get; set; }

		public void Damage(Hit hit)
		{
			if (hit.DamageAmount < 999f)
			{
				hit.DamageAmount = 999f;
			}
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
			this.Behaviour = base.GetComponent<PontiffHuskRangedBehaviour>();
			this.Sprite = base.GetComponentInChildren<SpriteRenderer>();
			this.DamageArea = base.GetComponentInChildren<EnemyDamageArea>();
			this.PontiffHuskRangedAttack = base.GetComponentInChildren<PontiffHuskRangedAttack>();
			this.FloatingMotion = base.GetComponentInChildren<PontiffHuskFloatingMotion>();
			this.AttackArea = base.GetComponentInChildren<AttackArea>();
			this.GhostTrail = base.GetComponentInChildren<GhostTrailGenerator>();
			this.ColorFlash = base.GetComponentInChildren<ColorFlash>();
			this.BehaviourTree = base.GetComponent<BehaviourTreeOwner>();
			this.VisionCone = base.GetComponentInChildren<VisionCone>();
			this.Behaviour.enabled = false;
			this.Behaviour.PauseBehaviour();
		}

		protected override void OnStart()
		{
			base.OnStart();
			if (this.PontiffHuskRangedAttack)
			{
				PontiffHuskRangedAttack pontiffHuskRangedAttack = this.PontiffHuskRangedAttack;
				pontiffHuskRangedAttack.OnEntityAttack = (Core.GenericEvent)Delegate.Combine(pontiffHuskRangedAttack.OnEntityAttack, new Core.GenericEvent(this.PontiffHuskRanged_OnEntityAttack));
			}
			this.FloatingMotion.IsFloating = true;
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			if (!Core.ready)
			{
				return;
			}
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

		private void PontiffHuskRanged_OnEntityAttack(object hit)
		{
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			if (this.PontiffHuskRangedAttack)
			{
				PontiffHuskRangedAttack pontiffHuskRangedAttack = this.PontiffHuskRangedAttack;
				pontiffHuskRangedAttack.OnEntityAttack = (Core.GenericEvent)Delegate.Remove(pontiffHuskRangedAttack.OnEntityAttack, new Core.GenericEvent(this.PontiffHuskRanged_OnEntityAttack));
			}
		}

		public override EnemyAttack EnemyAttack()
		{
			return this.PontiffHuskRangedAttack;
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
