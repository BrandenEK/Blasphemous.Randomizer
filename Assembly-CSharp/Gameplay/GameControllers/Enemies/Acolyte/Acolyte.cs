using System;
using CreativeSpore.SmartColliders;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Combat;
using Gameplay.GameControllers.Effects.NPCs.BloodDecals;
using Gameplay.GameControllers.Enemies.Acolyte.Animator;
using Gameplay.GameControllers.Enemies.Acolyte.Audio;
using Gameplay.GameControllers.Enemies.Acolyte.IA;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Enemies.Framework.Damage;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Enemies.Framework.Physics;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Acolyte
{
	public class Acolyte : Enemy, IDamageable
	{
		public Vector3 GetPosition()
		{
			return base.transform.position;
		}

		private void acolyte_OnAttack(object param)
		{
			base.StartCoroutine(base.FreezeAnimator(this.FreezeTime));
		}

		public MotionLerper MotionLerper { get; private set; }

		public AcolyteBehaviour Behaviour { get; private set; }

		public EntityExecution EntExecution { get; set; }

		public bool IsVisible()
		{
			return Entity.IsVisibleFrom(base.SpriteRenderer, Camera.main);
		}

		public AcolyteAnimatorInyector AnimatorInyector { get; set; }

		public AcolyteAudio Audio { get; set; }

		public NPCInputs Inputs { get; set; }

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

		public AttackArea AttackArea { get; private set; }

		public Rigidbody2D Rigidbody { get; private set; }

		public AcolyteAttackAnimations AttackAnimations { get; private set; }

		public VisionCone VisionCone { get; private set; }

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
			this.AnimatorInyector = base.GetComponentInChildren<AcolyteAnimatorInyector>();
			this.Inputs = base.GetComponent<NPCInputs>();
			base.Controller = base.GetComponent<PlatformCharacterController>();
			base.EnemyBehaviour = base.GetComponent<EnemyBehaviour>();
			this.Audio = base.GetComponent<AcolyteAudio>();
			this.MotionLerper = base.GetComponent<MotionLerper>();
			this._bloodDecalPumper = base.GetComponentInChildren<BloodDecalPumper>();
			this._enemyDamageArea = base.GetComponentInChildren<EnemyDamageArea>();
			this._acolyteAttack = base.GetComponentInChildren<AcolyteAttack>();
			this.enemyFloorChecker = base.GetComponentInChildren<EnemyFloorChecker>();
			this._acolyteSmartCollider = base.GetComponent<SmartPlatformCollider>();
			this.Rigidbody = base.GetComponent<Rigidbody2D>();
			this.AttackArea = base.GetComponentInChildren<AttackArea>();
			this.AttackAnimations = base.GetComponentInChildren<AcolyteAttackAnimations>();
			this.VisionCone = base.GetComponentInChildren<VisionCone>();
			this._defaultLayer = LayerMask.NameToLayer("Default");
			this._enemyLayer = LayerMask.NameToLayer("Enemy");
			this._hasEnemyLayer = true;
		}

		protected override void OnStart()
		{
			base.OnStart();
			AcolyteAttack acolyteAttack = this._acolyteAttack;
			acolyteAttack.OnEntityAttack = (Core.GenericEvent)Delegate.Combine(acolyteAttack.OnEntityAttack, new Core.GenericEvent(this.acolyte_OnAttack));
			this.SetOrientation(EntityOrientation.Left, true, false);
			this.entityCurrentState = EntityStates.Idle;
			this.Behaviour = base.GetComponent<AcolyteBehaviour>();
			EnemyFloorChecker enemyFloorChecker = this.enemyFloorChecker;
			enemyFloorChecker.OnTrapFall = (Core.SimpleEvent)Delegate.Combine(enemyFloorChecker.OnTrapFall, new Core.SimpleEvent(this.acolyte_OnTrapFall));
			this.enemyAttack = base.GetComponentInChildren<EnemyAttack>();
			this.enemyBumper = base.GetComponentInChildren<EnemyBumper>();
			this.EntExecution = base.GetComponent<EntityExecution>();
			base.Target = Core.Logic.Penitent.gameObject;
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			if (base.Target == null)
			{
				base.Target = Core.Logic.Penitent.gameObject;
			}
			base.DistanceToTarget = Vector2.Distance(base.transform.position, base.Target.transform.position);
			this.Status.IsVisibleOnCamera = this.IsVisible();
			if (this.Status.Dead)
			{
				this._timeDead += Time.deltaTime;
				this.DisableDamageArea();
				return;
			}
			if (this._timeDead >= 1f && base.HasFlag("GROUNDED"))
			{
				this.EnablePhysics(false);
			}
			if (!this.Status.IsGrounded)
			{
				this.StopMovementLerping();
			}
			if (!base.Landing)
			{
				base.Landing = true;
				this.EnablePhysics(true);
				this.SetPositionAtStart();
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

		public void SetMovementSpeed(float movementSpeed)
		{
			base.Controller.MaxWalkingSpeed = movementSpeed;
		}

		public void StopMovementLerping()
		{
			if (this.MotionLerper.IsLerping)
			{
				this.MotionLerper.StopLerping();
			}
		}

		public override void HitDisplacement(Vector3 enemyPos, DamageArea.DamageType damageType)
		{
			base.HitDisplacement(enemyPos, damageType);
			if (base.Controller.SlopeAngle > 1f)
			{
				return;
			}
			Vector3 vector = base.transform.right;
			vector = ((enemyPos.x < base.transform.position.x) ? vector : (-vector));
			if (this.MotionLerper.IsLerping)
			{
				return;
			}
			this.MotionLerper.TimeTakenDuringLerp = 0.5f;
			this.MotionLerper.distanceToMove = 1.5f;
			this.MotionLerper.StartLerping(vector);
		}

		public RigidbodyType2D RigidbodyType
		{
			get
			{
				return this.Rigidbody.bodyType;
			}
			set
			{
				this.Rigidbody.bodyType = value;
			}
		}

		protected override void EnablePhysics(bool enable = true)
		{
			if (enable)
			{
				if (!base.Controller.enabled)
				{
					base.Controller.enabled = true;
					this.Behaviour.StartBehaviour();
					base.Controller.PlatformCharacterPhysics.GravityScale = 1f;
				}
				if (!this.Inputs.enabled)
				{
					this.Inputs.enabled = true;
				}
				if (!this._acolyteSmartCollider.EnableCollision2D)
				{
					this._acolyteSmartCollider.EnableCollision2D = true;
				}
			}
			else
			{
				if (base.Controller.enabled)
				{
					base.Controller.enabled = false;
					this.Behaviour.PauseBehaviour();
					base.Controller.PlatformCharacterPhysics.GravityScale = 0f;
				}
				if (this.Inputs.enabled)
				{
					this.Inputs.enabled = false;
				}
				if (this._acolyteSmartCollider.EnableCollision2D)
				{
					this._acolyteSmartCollider.EnableCollision2D = false;
				}
			}
		}

		public void EnableEnemyLayer(bool enable = true)
		{
			if (enable && !this._hasEnemyLayer)
			{
				this._hasEnemyLayer = true;
				base.gameObject.layer = this._enemyLayer;
				this._enemyDamageArea.gameObject.layer = this._enemyLayer;
			}
			else if (!enable && this._hasEnemyLayer)
			{
				this._hasEnemyLayer = false;
				base.gameObject.layer = this._defaultLayer;
				this._enemyDamageArea.gameObject.layer = this._defaultLayer;
			}
		}

		public void DisableDamageArea()
		{
			if (this._enemyDamageArea != null)
			{
				this._enemyDamageArea.DamageAreaCollider.enabled = false;
			}
		}

		public void EnableDamageArea()
		{
			if (this._enemyDamageArea != null)
			{
				this._enemyDamageArea.DamageAreaCollider.enabled = true;
			}
		}

		private void acolyte_OnTrapFall()
		{
			this.AnimatorInyector.Dead();
			float amount = this.Stats.Life.Current;
			base.Damage(amount, string.Empty);
		}

		public void Damage(Hit hit)
		{
			if (base.EnemyBehaviour.GotParry)
			{
				hit.HitSoundId = string.Empty;
			}
			if (this.Execution(hit))
			{
				this.GetStun(hit);
				return;
			}
			string id = (hit.DamageType != DamageArea.DamageType.Normal) ? "PenitentHeavyEnemyHit" : "PenitentSimpleEnemyHit";
			Core.Audio.PlaySfxOnCatalog(id);
			if (!base.Animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
			{
				this._enemyDamageArea.TakeDamage(hit, false);
				this.AttackAnimations.ColorFlash.TriggerColorFlash();
				this.SleepTimeByHit(hit);
				this.AnimatorInyector.Damage();
				base.SetOrientationbyHit(hit.AttackingEntity.transform.position);
			}
			if (!this.Behaviour.GotParry)
			{
				return;
			}
			this.Behaviour.GotParry = false;
			this.Behaviour.StartBehaviour();
		}

		public override void Parry()
		{
			base.Parry();
			this.Behaviour.GotParry = true;
			this.StopMovementLerping();
			base.EnemyBehaviour.Parry();
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

		private BloodDecalPumper _bloodDecalPumper;

		private EnemyDamageArea _enemyDamageArea;

		private AcolyteAttack _acolyteAttack;

		private EnemyAI _acolyteAi;

		private SmartPlatformCollider _acolyteSmartCollider;

		private int _defaultLayer;

		private int _enemyLayer;

		public float MaxSpeed = 3.5f;

		public float MinSpeed = 1.4f;

		public float MaxSpeedBeforeFalling;

		private bool _hasEnemyLayer;

		[Range(0f, 1f)]
		public float FreezeTime = 0.1f;

		[Range(0f, 1f)]
		public float FreezeTimeFactor = 0.2f;

		private float _timeDead;
	}
}
