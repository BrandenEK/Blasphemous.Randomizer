using System;
using CreativeSpore.SmartColliders;
using DamageEffect;
using Framework.Managers;
using Gameplay.GameControllers.Combat;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Enemies.Framework.Damage;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Enemies.Framework.Physics;
using Gameplay.GameControllers.Enemies.Nun.Animator;
using Gameplay.GameControllers.Enemies.Nun.Attack;
using Gameplay.GameControllers.Enemies.Nun.Audio;
using Gameplay.GameControllers.Enemies.Nun.IA;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Nun
{
	public class Nun : Enemy, IDamageable
	{
		public NunBehaviour Behaviour { get; set; }

		public NPCInputs Input { get; set; }

		public SmartPlatformCollider Collider { get; set; }

		public EnemyDamageArea DamageArea { get; private set; }

		public NunAnimatorInyector AnimatorInyector { get; set; }

		public NunAttack Attack { get; private set; }

		public NunAudio Audio { get; private set; }

		public DamageEffectScript DamageEffect { get; set; }

		public EntityExecution EntExecution { get; private set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			this.Behaviour = base.GetComponent<NunBehaviour>();
			this.Input = base.GetComponent<NPCInputs>();
			base.Controller = base.GetComponent<PlatformCharacterController>();
			this.Collider = base.GetComponent<SmartPlatformCollider>();
			this.DamageArea = base.GetComponentInChildren<EnemyDamageArea>();
			this.AnimatorInyector = base.GetComponentInChildren<NunAnimatorInyector>();
			this.Attack = base.GetComponentInChildren<NunAttack>();
			this.Audio = base.GetComponentInChildren<NunAudio>();
			this.DamageEffect = base.GetComponentInChildren<DamageEffectScript>();
			this.EntExecution = base.GetComponentInChildren<EntityExecution>();
		}

		protected override void OnStart()
		{
			base.OnStart();
			base.Target = Core.Logic.Penitent.gameObject;
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			if (base.Target == null)
			{
				base.Target = Core.Logic.Penitent.gameObject;
			}
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
			if (this.Status.Dead)
			{
				this.DamageArea.DamageAreaCollider.enabled = false;
				this.Behaviour.Death();
				return;
			}
			this.DamageEffect.Blink(0f, 0.1f);
			this.SleepTimeByHit(hit);
		}

		public Vector3 GetPosition()
		{
			return base.transform.position;
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
