using System;
using CreativeSpore.SmartColliders;
using Framework.Managers;
using Gameplay.GameControllers.Combat;
using Gameplay.GameControllers.Effects.Entity;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Enemies.Framework.Damage;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Enemies.Framework.Physics;
using Gameplay.GameControllers.Enemies.RangedBoomerang.Animator;
using Gameplay.GameControllers.Enemies.RangedBoomerang.Audio;
using Gameplay.GameControllers.Enemies.RangedBoomerang.IA;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Penitent;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.RangedBoomerang
{
	public class RangedBoomerang : Enemy, IDamageable
	{
		public RangedBoomerangBehaviour Behaviour { get; private set; }

		public NPCInputs Input { get; private set; }

		public SmartPlatformCollider Collider { get; private set; }

		public EnemyDamageArea DamageArea { get; private set; }

		public RangedBoomerangAnimatorInyector AnimatorInyector { get; private set; }

		public RangedBoomerangAudio Audio { get; private set; }

		public EnemyAttack Attack { get; private set; }

		public ColorFlash ColorFlash { get; private set; }

		public VisionCone VisionCone { get; private set; }

		public EntityExecution EntExecution { get; private set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			this.Behaviour = base.GetComponentInChildren<RangedBoomerangBehaviour>();
			this.Input = base.GetComponent<NPCInputs>();
			base.Controller = base.GetComponent<PlatformCharacterController>();
			this.Collider = base.GetComponent<SmartPlatformCollider>();
			this.DamageArea = base.GetComponentInChildren<EnemyDamageArea>();
			this.Attack = base.GetComponentInChildren<EnemyAttack>();
			this.AnimatorInyector = base.GetComponentInChildren<RangedBoomerangAnimatorInyector>();
			this.Audio = base.GetComponentInChildren<RangedBoomerangAudio>();
			this.ColorFlash = base.GetComponentInChildren<ColorFlash>();
			this.VisionCone = base.GetComponentInChildren<VisionCone>();
			this.Behaviour.enabled = false;
			this.EntExecution = base.GetComponentInChildren<EntityExecution>();
			SpawnManager.OnPlayerSpawn += this.OnPlayerSpawn;
		}

		private void OnPlayerSpawn(Penitent penitent)
		{
			SpawnManager.OnPlayerSpawn -= this.OnPlayerSpawn;
			base.Target = penitent.gameObject;
		}

		protected override void OnStart()
		{
			base.OnStart();
			this.Status.CastShadow = true;
			this.Behaviour.enabled = true;
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			if (base.Target == null)
			{
				base.Target = Core.Logic.Penitent.gameObject;
			}
			this.Status.IsGrounded = base.Controller.IsGrounded;
			if (!base.Landing)
			{
				base.Landing = true;
				this.SetPositionAtStart();
				base.Controller.PlatformCharacterPhysics.GravityScale = 1f;
			}
		}

		public override void SetPositionAtStart()
		{
			base.SetPositionAtStart();
			float groundDist = base.Controller.GroundDist;
			Vector3 position;
			position..ctor(base.transform.position.x, base.transform.position.y - groundDist, base.transform.position.z);
			base.transform.position = position;
			base.Controller.PlatformCharacterPhysics.GravityScale = 0f;
		}

		public void Damage(Hit hit)
		{
			if (this.Execution(hit))
			{
				this.GetStun(hit);
				return;
			}
			this.DamageArea.TakeDamage(hit, false);
			this.ColorFlash.TriggerColorFlash();
			if (this.Status.Dead)
			{
				this.DamageArea.DamageAreaCollider.enabled = false;
				this.Behaviour.Death();
				return;
			}
			this.SleepTimeByHit(hit);
		}

		public override void GetStun(Hit hit)
		{
			base.GetStun(hit);
			if (base.IsStunt)
			{
				return;
			}
			if (Mathf.Abs(base.Controller.SlopeAngle) < 1f)
			{
				Core.Audio.EventOneShotPanned(hit.HitSoundId, base.transform.position);
				this.Behaviour.Execution();
			}
		}

		public Vector3 GetPosition()
		{
			return base.transform.position;
		}

		public override EnemyFloorChecker EnemyFloorChecker()
		{
			throw new NotImplementedException();
		}

		public override EnemyAttack EnemyAttack()
		{
			throw new NotImplementedException();
		}

		public override EnemyBumper EnemyBumper()
		{
			throw new NotImplementedException();
		}

		protected override void EnablePhysics(bool enable = true)
		{
			throw new NotImplementedException();
		}
	}
}
