using System;
using CreativeSpore.SmartColliders;
using DamageEffect;
using Framework.Managers;
using Gameplay.GameControllers.Combat;
using Gameplay.GameControllers.Enemies.Bishop.AI;
using Gameplay.GameControllers.Enemies.Bishop.Animation;
using Gameplay.GameControllers.Enemies.Bishop.Attack;
using Gameplay.GameControllers.Enemies.Bishop.Audio;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Enemies.Framework.Damage;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Enemies.Framework.Physics;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Bishop
{
	public class Bishop : Enemy, IDamageable
	{
		public BishopAnimatorInyector AnimatorInyector { get; private set; }

		public NPCInputs Inputs { get; private set; }

		public EntityMotionChecker MotionChecker { get; set; }

		public EnemyDamageArea DamageArea { get; set; }

		public BishopAttack Attack { get; private set; }

		public DamageEffectScript DamageEffect { get; private set; }

		public SmartCollider2D SmartCollider { get; private set; }

		public BishopAudio Audio { get; private set; }

		public EntityExecution EntExecution { get; private set; }

		private void OnEnable()
		{
			if (base.Landing)
			{
				base.Landing = !base.Landing;
			}
		}

		protected override void OnAwake()
		{
			base.OnAwake();
			base.Controller = base.GetComponent<PlatformCharacterController>();
			this.AnimatorInyector = base.GetComponentInChildren<BishopAnimatorInyector>();
			base.EnemyBehaviour = base.GetComponent<EnemyBehaviour>();
			this.Inputs = base.GetComponentInChildren<NPCInputs>();
			this.MotionChecker = base.GetComponentInChildren<EntityMotionChecker>();
			this.DamageArea = base.GetComponentInChildren<EnemyDamageArea>();
			this.Attack = base.GetComponentInChildren<BishopAttack>();
			this.DamageEffect = base.GetComponentInChildren<DamageEffectScript>();
			this.SmartCollider = base.GetComponent<SmartCollider2D>();
			this.Audio = base.GetComponentInChildren<BishopAudio>();
			this.EntExecution = base.GetComponentInChildren<EntityExecution>();
		}

		protected override void OnStart()
		{
			base.OnStart();
			base.Target = Core.Logic.Penitent.gameObject;
		}

		protected override void OnUpdate()
		{
			if (base.Target == null)
			{
				base.Target = Core.Logic.Penitent.gameObject;
			}
			base.DistanceToTarget = Vector2.Distance(base.transform.position, base.Target.transform.position);
			bool enable = base.DistanceToTarget <= this.ActivationRange;
			if (this.Status.Dead)
			{
				if (this.DamageArea.DamageAreaCollider.enabled)
				{
					this.DamageArea.DamageAreaCollider.enabled = false;
				}
			}
			else if (base.Landing)
			{
				this.EnablePhysics(enable);
			}
			if (base.Landing)
			{
				return;
			}
			base.Landing = true;
			this.SetPositionAtStart();
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
			if (enable)
			{
				if (!base.Controller.enabled)
				{
					base.Controller.enabled = true;
					base.EnemyBehaviour.StartBehaviour();
					base.Controller.PlatformCharacterPhysics.GravityScale = 1f;
				}
				if (!this.Inputs.enabled)
				{
					this.Inputs.enabled = true;
				}
				if (!this.SmartCollider.EnableCollision2D)
				{
					this.SmartCollider.EnableCollision2D = true;
				}
			}
			else
			{
				if (base.Controller.enabled)
				{
					base.Controller.enabled = false;
					base.EnemyBehaviour.PauseBehaviour();
					base.Controller.PlatformCharacterPhysics.GravityScale = 0f;
				}
				if (this.Inputs.enabled)
				{
					this.Inputs.enabled = false;
				}
				if (this.SmartCollider.EnableCollision2D)
				{
					this.SmartCollider.EnableCollision2D = false;
				}
			}
		}

		public override void SetPositionAtStart()
		{
			base.SetPositionAtStart();
			float groundDist = base.Controller.GroundDist;
			Vector3 position;
			position..ctor(base.transform.position.x, base.transform.position.y - groundDist, base.transform.position.z);
			base.transform.position = position;
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
				this.AnimatorInyector.Death();
				return;
			}
			BishopBehaviour bishopBehaviour = (BishopBehaviour)base.EnemyBehaviour;
			bishopBehaviour.Damage();
			this.DamageEffect.Blink(0f, 0.1f);
			bishopBehaviour.HitDisplacement(hit.AttackingEntity.transform.position);
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
				base.EnemyBehaviour.Execution();
			}
		}

		public Vector3 GetPosition()
		{
			return base.transform.position;
		}
	}
}
