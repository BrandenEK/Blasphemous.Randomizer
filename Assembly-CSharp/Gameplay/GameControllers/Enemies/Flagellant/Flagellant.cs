using System;
using CreativeSpore.SmartColliders;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Combat;
using Gameplay.GameControllers.Effects.NPCs.BloodDecals;
using Gameplay.GameControllers.Enemies.Flagellant.Animator;
using Gameplay.GameControllers.Enemies.Flagellant.Audio;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Enemies.Framework.Damage;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Enemies.Framework.Physics;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Flagellant
{
	public class Flagellant : Enemy, IDamageable
	{
		protected void flagellant_OnTrapFall()
		{
			float amount = this.Stats.Life.Current;
			base.Damage(amount, string.Empty);
			this.AnimatorInyector.Death();
		}

		public EntityExecution EntExecution { get; set; }

		public VisionCone VisionCone { get; private set; }

		public EntityMotionChecker MotionChecker { get; private set; }

		public Rigidbody2D RigidBody { get; private set; }

		public MotionLerper MotionLerper { get; private set; }

		public bool IsVisible()
		{
			return Entity.IsVisibleFrom(base.SpriteRenderer, Camera.main);
		}

		public FlagellantAnimatorInyector AnimatorInyector { get; set; }

		public override EnemyFloorChecker EnemyFloorChecker()
		{
			return this.enemyFloorChecker;
		}

		public override EnemyAttack EnemyAttack()
		{
			return this.enemyAttack;
		}

		public override EnemyBumper EnemyBumper()
		{
			return this.enemyBumper;
		}

		public FlagellantAudio Audio { get; set; }

		public NPCInputs Inputs { get; private set; }

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
			this.MotionChecker = base.GetComponentInChildren<EntityMotionChecker>();
			base.EnemyBehaviour = base.GetComponent<EnemyBehaviour>();
			this.RigidBody = base.GetComponent<Rigidbody2D>();
			base.Controller = base.GetComponent<PlatformCharacterController>();
			this.AnimatorInyector = base.GetComponentInChildren<FlagellantAnimatorInyector>();
			this.Audio = base.GetComponent<FlagellantAudio>();
			this._flagellantMotionLerper = base.GetComponent<MotionLerper>();
			this._bloodDecalPumper = base.GetComponentInChildren<BloodDecalPumper>();
			this._enemyDamageArea = base.GetComponentInChildren<EnemyDamageArea>();
			this._damageAreaCollider = this._enemyDamageArea.GetComponent<BoxCollider2D>();
			this.enemyFloorChecker = base.GetComponentInChildren<EnemyFloorChecker>();
			this.Inputs = base.GetComponent<NPCInputs>();
			this._flagellantSmartCollider = base.GetComponent<SmartPlatformCollider>();
			this.EntExecution = base.GetComponent<EntityExecution>();
			this.VisionCone = base.GetComponentInChildren<VisionCone>();
		}

		protected override void OnStart()
		{
			base.OnStart();
			this.SetOrientation(EntityOrientation.Left, true, false);
			this.entityCurrentState = EntityStates.Idle;
			this.MotionLerper = base.GetComponent<MotionLerper>();
			SmartPlatformCollider flagellantSmartCollider = this._flagellantSmartCollider;
			flagellantSmartCollider.OnSideCollision = (SmartRectCollider2D.OnSideCollisionDelegate)Delegate.Combine(flagellantSmartCollider.OnSideCollision, new SmartRectCollider2D.OnSideCollisionDelegate(this.flagellant_OnSideCollision));
			this.enemyAttack = base.GetComponentInChildren<EnemyAttack>();
			this.enemyBumper = base.GetComponentInChildren<EnemyBumper>();
			EnemyFloorChecker enemyFloorChecker = this.enemyFloorChecker;
			enemyFloorChecker.OnTrapFall = (Core.SimpleEvent)Delegate.Combine(enemyFloorChecker.OnTrapFall, new Core.SimpleEvent(this.flagellant_OnTrapFall));
			base.Target = Core.Logic.Penitent.gameObject;
			this.EntExecution.enabled = false;
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			if (!base.Target)
			{
				base.Target = Core.Logic.Penitent.gameObject;
			}
			base.DistanceToTarget = Vector2.Distance(base.transform.position, base.Target.transform.position);
			this.Status.IsVisibleOnCamera = this.IsVisible();
			if (this.Status.Dead)
			{
				this.timeDead += Time.deltaTime;
				this.DisableDamageArea();
			}
			else if (base.Landing)
			{
				this.EnablePhysics(true);
			}
			if (this.timeDead >= 1f && this.Status.IsGrounded)
			{
				this.EnablePhysics(false);
			}
			if (!base.Landing)
			{
				base.Landing = true;
				this.SetPositionAtStart();
			}
		}

		public override void HitDisplacement(Vector3 enemyPos, DamageArea.DamageType damageType)
		{
			if (base.Controller.SlopeAngle > 1f)
			{
				return;
			}
			float x = base.transform.position.x;
			Vector2 vector = base.transform.right;
			if (enemyPos.x > x)
			{
				vector *= -1f;
			}
			if (this.MotionLerper.IsLerping)
			{
				return;
			}
			this.MotionLerper.TimeTakenDuringLerp = 0.25f;
			this.MotionLerper.distanceToMove = 2f;
			this.MotionLerper.StartLerping(vector);
		}

		public void SetMovementSpeed(float newSpeed)
		{
			base.Controller.MaxWalkingSpeed = newSpeed;
		}

		public override void SetPositionAtStart()
		{
			base.SetPositionAtStart();
			float groundDist = base.Controller.GroundDist;
			Vector3 position;
			position..ctor(base.transform.position.x, base.transform.position.y - groundDist, base.transform.position.z);
			base.transform.position = position;
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
				if (!this._flagellantSmartCollider.EnableCollision2D)
				{
					this._flagellantSmartCollider.EnableCollision2D = true;
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
				if (this._flagellantSmartCollider.EnableCollision2D)
				{
					this._flagellantSmartCollider.EnableCollision2D = false;
				}
			}
		}

		private void flagellant_OnSideCollision(SmartCollision2D collision, GameObject collidedObject)
		{
			if (this.Status.IsVisibleOnCamera)
			{
				base.SetFlag("SIDE_BLOCKED", false);
				for (int i = 0; i < collision.contacts.Length; i++)
				{
					if (collision.contacts[i].normal == -Vector2.right || collision.contacts[i].normal == Vector2.right)
					{
						base.SetFlag("SIDE_BLOCKED", true);
						if (this._flagellantMotionLerper.IsLerping)
						{
							this._flagellantMotionLerper.StopLerping();
							break;
						}
					}
				}
			}
		}

		public void DisableDamageArea()
		{
			if (this._damageAreaCollider)
			{
				this._damageAreaCollider.enabled = false;
			}
		}

		protected void PumpBloodDecal()
		{
		}

		public void Damage(Hit hit)
		{
			if (this.Status.Unattacable)
			{
				return;
			}
			if (this.Execution(hit))
			{
				this.GetStun(hit);
				return;
			}
			this._enemyDamageArea.TakeDamage(hit, false);
			if (this.canBeStunLocked)
			{
				this.AnimatorInyector.DamageImpact();
				base.SetOrientationbyHit(hit.AttackingEntity.transform.position);
			}
			this.SleepTimeByHit(hit);
			this.PumpBloodDecal();
			base.EnemyBehaviour.GotParry = false;
		}

		public override void Parry()
		{
			base.Parry();
			base.EnemyBehaviour.Parry();
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
				base.EnemyBehaviour.Execution();
			}
		}

		private MotionLerper _flagellantMotionLerper;

		private BloodDecalPumper _bloodDecalPumper;

		private EnemyDamageArea _enemyDamageArea;

		private BoxCollider2D _damageAreaCollider;

		private SmartPlatformCollider _flagellantSmartCollider;

		public float MAX_SPEED = 3.5f;

		public float MIN_SPEED = 1.4f;

		public float maxSpeedBeforeFalling;

		public bool canBeStunLocked = true;

		[Range(0f, 1f)]
		public float freezeTime = 0.1f;

		[Range(0f, 1f)]
		public float freezeTimeFactor = 0.2f;

		private float timeDead;
	}
}
