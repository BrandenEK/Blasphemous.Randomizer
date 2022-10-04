using System;
using CreativeSpore.SmartColliders;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Combat;
using Gameplay.GameControllers.Effects.Entity;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Enemies.Framework.Damage;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Enemies.Framework.Physics;
using Gameplay.GameControllers.Enemies.NewFlagellant.AI;
using Gameplay.GameControllers.Enemies.NewFlagellant.Animator;
using Gameplay.GameControllers.Enemies.NewFlagellant.Attack;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.StateMachine;
using Plugins.GhostSprites2D.Scripts.GhostSprites;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.NewFlagellant
{
	public class NewFlagellant : Enemy, IDamageable
	{
		public StateMachine StateMachine { get; private set; }

		public NPCInputs Input { get; private set; }

		public NewFlagellantBehaviour NewFlagellantBehaviour { get; private set; }

		public EntityDisplacement EntityDisplacement { get; set; }

		public NewFlagellantAnimatorInyector AnimatorInyector { get; private set; }

		public MotionLerper MotionLerper { get; set; }

		public EnemyDamageArea DamageArea { get; set; }

		public EntityMotionChecker MotionChecker { get; private set; }

		public VisionCone VisionCone { get; private set; }

		public MasterShaderEffects MasterShaderEffects { get; private set; }

		public bool IsTargetVisible { get; private set; }

		public EntityExecution EntExecution { get; private set; }

		public GhostSprites GhostSprites { get; private set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			this.StateMachine = base.GetComponent<StateMachine>();
			this.EntityDisplacement = base.GetComponent<EntityDisplacement>();
			this.Input = base.GetComponent<NPCInputs>();
			this.NewFlagellantBehaviour = base.GetComponent<NewFlagellantBehaviour>();
			this.AnimatorInyector = base.GetComponentInChildren<NewFlagellantAnimatorInyector>();
			this.MotionLerper = base.GetComponentInChildren<MotionLerper>();
			base.Controller = base.GetComponentInChildren<PlatformCharacterController>();
			this.DamageArea = base.GetComponentInChildren<EnemyDamageArea>();
			this.MotionChecker = base.GetComponentInChildren<EntityMotionChecker>();
			this.VisionCone = base.GetComponentInChildren<VisionCone>();
			this.MasterShaderEffects = base.GetComponentInChildren<MasterShaderEffects>();
			this.EntExecution = base.GetComponentInChildren<EntityExecution>();
			this.GhostSprites = base.GetComponentInChildren<GhostSprites>();
			this.contacts = new Collider2D[3];
		}

		protected override void OnStart()
		{
			base.OnStart();
			this.Status.CastShadow = true;
			this.MAX_SPEED += UnityEngine.Random.Range(-0.2f, 0.1f);
			this.MIN_SPEED += UnityEngine.Random.Range(-0.2f, 0.1f);
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			this.Status.IsGrounded = base.Controller.IsGrounded;
			if (base.Target != null)
			{
				base.DistanceToTarget = Vector2.Distance(base.transform.position, base.Target.transform.position);
			}
			else
			{
				base.Target = Core.Logic.Penitent.gameObject;
			}
			if (base.Landing)
			{
				return;
			}
			base.Landing = true;
			this.SetPositionAtStart();
		}

		public void CheckOverlappingEnemies()
		{
			Collider2D damageAreaCollider = this.DamageArea.DamageAreaCollider;
			int num = damageAreaCollider.OverlapCollider(this.overlapFilter, this.contacts);
			if (num > 0)
			{
				for (int i = 0; i < num; i++)
				{
					if (this.MotionLerper.IsLerping)
					{
						return;
					}
					Enemy componentInChildren = this.contacts[i].GetComponentInChildren<Enemy>();
					if (componentInChildren != null)
					{
						Vector2 v = new Vector2((float)((componentInChildren.transform.position.x >= base.transform.position.x) ? -1 : 1), 0f);
						if ((v.x == 1f && this.Status.Orientation == EntityOrientation.Left) || (v.x == -1f && this.Status.Orientation == EntityOrientation.Right))
						{
							this.NewFlagellantBehaviour.OnBouncedBackByOverlapping();
						}
						this.MotionLerper.StartLerping(v);
					}
				}
			}
		}

		public override void SetPositionAtStart()
		{
			base.SetPositionAtStart();
			float groundDist = base.Controller.GroundDist;
			Vector3 position = new Vector3(base.transform.position.x, base.transform.position.y - groundDist, base.transform.position.z);
			base.transform.position = position;
		}

		public void SetMovementSpeed(float newSpeed)
		{
			base.Controller.MaxWalkingSpeed = newSpeed;
		}

		public void Damage(Hit hit)
		{
			if (this.MotionLerper.IsLerping)
			{
				return;
			}
			this.DamageArea.TakeDamage(hit, false);
			if (this.Status.Dead)
			{
				this.AnimatorInyector.Death();
				return;
			}
			if (this.Execution(hit))
			{
				this.GetStun(hit);
				return;
			}
			if (this.NewFlagellantBehaviour.GotParry)
			{
				this.NewFlagellantBehaviour.GotParry = false;
				this.NewFlagellantBehaviour.ResetParryRecover();
			}
			this.NewFlagellantBehaviour.Damage();
			this.AnimatorInyector.IsParried(false);
			this.MasterShaderEffects.DamageEffectBlink(0f, 0.07f, null);
			this.SleepTimeByHit(hit);
		}

		private void KillByTrap()
		{
			if (this.Status.Dead)
			{
				return;
			}
			this.NewFlagellantBehaviour.StopBehaviour();
			this.Kill();
			this.AnimatorInyector.Death();
		}

		public override void Parry()
		{
			base.Parry();
			this.NewFlagellantBehaviour.Parry();
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
				this.NewFlagellantBehaviour.Execution();
			}
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (!base.Controller.IsGrounded && other.CompareTag("SpikeTrap"))
			{
				this.KillByTrap();
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
			return this.Attack;
		}

		public override EnemyBumper EnemyBumper()
		{
			throw new NotImplementedException();
		}

		protected override void EnablePhysics(bool enable = true)
		{
			throw new NotImplementedException();
		}

		public NewFlagellantAttack Attack;

		public NewFlagellantAttack FastAttack;

		[FoldoutGroup("Overlap fixer settings", 0)]
		public ContactFilter2D overlapFilter;

		[FoldoutGroup("Overlap fixer settings", 0)]
		public Collider2D[] contacts;

		public float MAX_SPEED = 3.5f;

		public float MIN_SPEED = 1.4f;
	}
}
