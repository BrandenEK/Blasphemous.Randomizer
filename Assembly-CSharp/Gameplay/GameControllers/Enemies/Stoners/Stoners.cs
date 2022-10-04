using System;
using Framework.Managers;
using Gameplay.GameControllers.Effects.NPCs.BloodDecals;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Enemies.Framework.Damage;
using Gameplay.GameControllers.Enemies.Framework.Physics;
using Gameplay.GameControllers.Enemies.Stoners.AI;
using Gameplay.GameControllers.Enemies.Stoners.Animator;
using Gameplay.GameControllers.Enemies.Stoners.Attack;
using Gameplay.GameControllers.Enemies.Stoners.Audio;
using Gameplay.GameControllers.Enemies.Stoners.Rock;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Stoners
{
	public class Stoners : Enemy, IDamageable
	{
		public EnemyDamageArea StonersDamageArea { get; set; }

		public BloodDecalPumper BloodDecalPumper { get; set; }

		public StonerBehaviour StonerBehaviour { get; set; }

		public RockPool RockPool { get; set; }

		public SpawnPoint RockSpawnPoint { get; set; }

		public StonersAttack Attack { get; set; }

		public StonersAudio Audio { get; set; }

		public void Damage(Hit hit)
		{
			this.StonersDamageArea.TakeDamage(hit, false);
			if (this.Status.Dead)
			{
				this.Audio.Death();
			}
			else
			{
				this.Audio.Damage();
			}
			this.AnimatorInyector.Hurt();
			this.SleepTimeByHit(hit);
			this.PumpBloodDecal();
		}

		public Vector3 GetPosition()
		{
			return base.transform.position;
		}

		protected override void OnAwake()
		{
			base.OnAwake();
			this.AnimatorInyector = base.GetComponentInChildren<StonerAnimatorInyector>();
			this.StonersDamageArea = base.GetComponentInChildren<EnemyDamageArea>();
			this.BloodDecalPumper = base.GetComponentInChildren<BloodDecalPumper>();
			this.StonerBehaviour = base.GetComponentInChildren<StonerBehaviour>();
			this.RockPool = base.GetComponentInChildren<RockPool>();
			this.RockSpawnPoint = base.GetComponentInChildren<SpawnPoint>();
			this.Attack = base.GetComponentInChildren<StonersAttack>();
			this.Audio = base.GetComponentInChildren<StonersAudio>();
		}

		protected override void OnStart()
		{
			base.OnStart();
			this.Status.IsVisibleOnCamera = this.IsVisible();
			base.Target = Core.Logic.Penitent.gameObject;
			this.StonersDamageArea.DamageAreaCollider.enabled = false;
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			this.Status.IsVisibleOnCamera = this.IsVisible();
			base.DistanceToTarget = Vector2.Distance(base.transform.position, base.Target.transform.position);
			bool flag = base.DistanceToTarget <= this.ActivationRange;
			if (flag)
			{
				if (this.DisableBehaviourWhenInvisible)
				{
					this.StonerBehaviour.StartBehaviour();
				}
			}
			else if (this.DisableBehaviourWhenInvisible)
			{
				this.StonerBehaviour.PauseBehaviour();
			}
		}

		public override EnemyFloorChecker EnemyFloorChecker()
		{
			throw new NotImplementedException();
		}

		public override EnemyAttack EnemyAttack()
		{
			return this.Attack;
		}

		public override EnemyBumper EnemyBumper()
		{
			throw new NotImplementedException();
		}

		protected override void EnablePhysics(bool enable)
		{
			throw new NotImplementedException();
		}

		public bool IsVisible()
		{
			return Entity.IsVisibleFrom(this.spriteRenderer, Camera.main);
		}

		protected void PumpBloodDecal()
		{
		}

		public StonerAnimatorInyector AnimatorInyector;

		[Range(0f, 1f)]
		public float freezeTime = 0.1f;

		[Range(0f, 1f)]
		public float freezeTimeFactor = 0.2f;
	}
}
