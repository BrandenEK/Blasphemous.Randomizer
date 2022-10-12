using System;
using System.Collections;
using CreativeSpore.SmartColliders;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.AnimationBehaviours.Player.Attack;
using Gameplay.GameControllers.Camera;
using Gameplay.GameControllers.Effects.Entity;
using Gameplay.GameControllers.Effects.NPCs.BloodDecals;
using Gameplay.GameControllers.Effects.Player.Dash;
using Gameplay.GameControllers.Effects.Player.Dust;
using Gameplay.GameControllers.Effects.Player.Sparks;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.StateMachine;
using Gameplay.GameControllers.Penitent.Abilities;
using Gameplay.GameControllers.Penitent.Animator;
using Gameplay.GameControllers.Penitent.Attack;
using Gameplay.GameControllers.Penitent.Audio;
using Gameplay.GameControllers.Penitent.Damage;
using Gameplay.GameControllers.Penitent.InputSystem;
using Gameplay.GameControllers.Penitent.Movement;
using Gameplay.GameControllers.Penitent.Sensor;
using Gameplay.UI;
using Sirenix.OdinInspector;
using Tools.Level;
using UnityEngine;

namespace Gameplay.GameControllers.Penitent
{
	public class Penitent : Entity, IDamageable
	{
		public Penitent()
		{
			this.IsDeadInAir = false;
		}

		public LevelSleepTime LevelSleepTime { get; private set; }

		public bool IsJumping { get; set; }

		public bool JumpFromLadder { get; set; }

		public Dash Dash { get; private set; }

		public DashDustGenerator DashDustGenerator { get; set; }

		public PlatformCharacterInput PlatformCharacterInput { get; private set; }

		public PenitentAudio Audio { get; private set; }

		public event Action<AttackBehaviour> OnAttackBehaviourEnters;

		public event Action OnJump;

		public void OnAttackBehaviour_OnEnter(AttackBehaviour attackBehaviour)
		{
			if (this.OnAttackBehaviourEnters != null)
			{
				this.OnAttackBehaviourEnters(attackBehaviour);
			}
		}

		public event Action<AirAttackBehaviour> OnAirAttackBehaviourEnters;

		public void OnAirAttackBehaviour_OnEnter(AirAttackBehaviour airAttackBehaviour)
		{
			if (this.OnAirAttackBehaviourEnters != null)
			{
				this.OnAirAttackBehaviourEnters(airAttackBehaviour);
			}
		}

		public void IncrementFervour(Hit hit)
		{
			float num = this.Stats.FervourStrength.Final;
			DamageArea.DamageType damageType = hit.DamageType;
			if (damageType != Gameplay.GameControllers.Entities.DamageArea.DamageType.Heavy)
			{
				if (damageType == Gameplay.GameControllers.Entities.DamageArea.DamageType.Critical)
				{
					num *= 5f;
				}
			}
			else
			{
				num *= 2f;
			}
			num *= Core.GuiltManager.GetFervourGainFactor();
			this.Stats.Fervour.Current += num;
		}

		private void OnEntityDead(Entity entity)
		{
			Enemy enemy = entity as Enemy;
			if (enemy)
			{
				this.GetPurge(enemy);
			}
			if (entity as Penitent == null)
			{
				return;
			}
			Core.Events.SetFlag("CHERUB_RESPAWN", true, false);
			this.EnableAbilities(false);
			this.EnableTraits(false);
			this.DamageArea.IncludeEnemyLayer(false);
		}

		private void GetPurge(Enemy enemy)
		{
			if (enemy.Id != "" && "BS01BS03BS04BS05BS06BS12BS13BS14BS16".Contains(enemy.Id))
			{
				Core.Randomizer.Log("GetPurge", 2);
				Core.Randomizer.giveReward(enemy.Id, true);
				return;
			}
			float num = this.Stats.PurgeStrength.Final * enemy.purgePointsWhenDead;
			num *= Core.GuiltManager.GetPurgeGainFactor();
			if (this.IsOnExecution)
			{
				num += num * 0.5f;
			}
			this.Stats.Purge.Current += num;
		}

		public void Respawn()
		{
			Core.Logic.EnemySpawner.Reset();
			Core.Persistence.RestoreStored();
			Core.SpawnManager.Respawn();
		}

		public void CherubRespawn()
		{
			if (this.Cherubs != null)
			{
				PoolManager.Instance.ReuseObject(this.Cherubs, base.transform.position, Quaternion.identity, false, 1);
			}
			UIController.instance.UpdateGuiltLevel(true);
		}

		public bool IsGrabbingCliffLede { get; set; }

		public bool IsClimbingCliffLede { get; set; }

		public EntityOrientation CliffLedeOrientation { get; set; }

		public bool IsClimbingLadder { get; set; }

		public bool IsOnLadder { get; set; }

		public bool StepOnLadder { get; set; }

		public bool IsStickedOnWall { get; set; }

		public bool IsGrabbingLadder { get; set; }

		public bool StartingGoingDownLadders { get; set; }

		public bool CanJumpFromLadder { get; set; }

		public bool IsJumpingOff { get; set; }

		public bool BeginCrouch { get; set; }

		public bool IsCrouchAttacking { get; set; }

		public bool IsCrouched { get; set; }

		public bool WatchBelow { get; set; }

		public bool IsDeadInAir { get; set; }

		public bool DeathEventLaunched { get; set; }

		public bool IsFallingStunt { get; set; }

		public bool IsSmashed { get; set; }

		public EntityOrientation HurtOrientation { get; set; }

		public EntityRumble Rumble { get; private set; }

		public Vector3 RootTargetPosition { get; set; }

		public Vector3 RootMotionDrive { get; set; }

		public PenitentCancelEffect CancelEffect { get; private set; }

		public ParticleSystem ParticleSystem { get; private set; }

		public bool IsDashing { get; set; }

		public bool IsPickingCollectibleItem { get; set; }

		public MotionLerper MotionLerper { get; set; }

		public void Teleport(Vector2 position)
		{
			Penitent penitent = Core.Logic.Penitent;
			penitent.PlatformCharacterController.PlatformCharacterPhysics.Acceleration = Vector3.zero;
			penitent.PlatformCharacterController.PlatformCharacterPhysics.Velocity = Vector3.zero;
			penitent.Physics.EnableColliders(false);
			penitent.Physics.Enable2DCollision(false);
			base.transform.position = position;
			penitent.Physics.EnableColliders(true);
			penitent.Physics.Enable2DCollision(true);
		}

		public void ForceMove(Vector2 position)
		{
			Core.Input.SetBlocker("PLAYER_LOGIC", true);
			base.StartCoroutine(this.ForceMovementAction(position));
			Core.Logic.SetPreviousState();
		}

		private IEnumerator ForceMovementAction(Vector2 targetPosition)
		{
			do
			{
				float num = targetPosition.x - base.transform.position.x;
				if (Mathf.Sign(num) < 0f)
				{
					break;
				}
				Core.Logic.Penitent.PlatformCharacterInput.forceHorizontalMovement = num;
				yield return new WaitForFixedUpdate();
			}
			while (base.transform.position.x < targetPosition.x + 0.2f);
			do
			{
				float num2 = targetPosition.x - base.transform.position.x;
				if (Mathf.Sign(num2) > 0f)
				{
					break;
				}
				Core.Logic.Penitent.PlatformCharacterInput.forceHorizontalMovement = num2;
				yield return new WaitForFixedUpdate();
			}
			while (base.transform.position.x > targetPosition.x - 0.2f);
			yield return new WaitForSeconds(0.5f);
			Core.Logic.Penitent.PlatformCharacterInput.forceHorizontalMovement = 0f;
			yield break;
		}

		public bool IsChargingAttack { get; set; }

		public bool IsAttackCharged { get; set; }

		public bool IsOnExecution { get; set; }

		public bool ReleaseChargedAttack { get; set; }

		public Parry Parry { get; private set; }

		public ChargedAttack ChargedAttack { get; private set; }

		public VerticalAttack VerticalAttack { get; private set; }

		public LungeAttack LungeAttack { get; private set; }

		public PenitentAttack PenitentAttack { get; private set; }

		public RangeAttack RangeAttack { get; private set; }

		public GuardSlide GuardSlide { get; private set; }

		public ActiveRiposte ActiveRiposte { get; private set; }

		private void EnableAbilities(bool enableAbility)
		{
			if (this == null)
			{
				return;
			}
			foreach (Ability ability in base.GetComponentsInChildren<Ability>())
			{
				if (ability.GetType() != typeof(ThrowBack))
				{
					ability.enabled = enableAbility;
				}
			}
		}

		private void EnableTraits(bool enableTraits)
		{
			if (this == null)
			{
				return;
			}
			Trait[] componentsInChildren = base.GetComponentsInChildren<Trait>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = enableTraits;
			}
		}

		public StateMachine StateMachine { get; private set; }

		public DrivePlayer DrivePlayer { get; private set; }

		public GrabCliffLede GrabCliffLede { get; private set; }

		public bool CanClimbLadder { get; set; }

		public bool CanLowerCliff { get; set; }

		public bool IsLadderSliding { get; set; }

		public GrabLadder GrabLadder { get; private set; }

		public bool ReachTopLadder { get; set; }

		public bool ReachBottonLadder { get; set; }

		public PenitentDamageArea DamageArea { get; private set; }

		public BloodDecalPumper BloodDecalPumper { get; private set; }

		public ThrowBack ThrowBack { get; private set; }

		public ColorFlash ColorFlash { get; private set; }

		public CameraManager CameraManager { get; private set; }

		public AttackArea AttackArea { get; private set; }

		public PenitentAttackAnimations PenitentAttackAnimations { get; private set; }

		public SwordSparkSpawner SwordSparkSpawner { get; private set; }

		public StepDustSpawner StepDustSpawner { get; private set; }

		public AnimatorInyector AnimatorInyector { get; private set; }

		public PenitentMoveAnimations PenitentMoveAnimations { get; private set; }

		public PhysicsSwitcher Physics { get; private set; }

		public Rigidbody2D RigidBody { get; private set; }

		public FloorDistanceChecker FloorChecker { get; private set; }

		public CheckTrap TrapChecker { get; private set; }

		public MotionLerper PlayerHitMotionLerper { get; private set; }

		public PlatformCharacterController PlatformCharacterController { get; private set; }

		public override void SetOrientation(EntityOrientation orientation, bool allowFlipRenderer = true, bool searchForRenderer = false)
		{
			base.SetOrientation(orientation, allowFlipRenderer, searchForRenderer);
			this.PlatformCharacterInput.faceRight = (orientation == EntityOrientation.Right);
		}

		public EntityOrientation GetOrientation()
		{
			if (this.PlatformCharacterInput.faceRight)
			{
				return EntityOrientation.Right;
			}
			return EntityOrientation.Left;
		}

		public Healing Healing { get; private set; }

		public PrayerUse PrayerCast { get; private set; }

		public FervourPenance Penance { get; private set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			this.AnimatorInyector = base.GetComponent<AnimatorInyector>();
			this.PlayerHitMotionLerper = base.GetComponent<MotionLerper>();
			this.PlatformCharacterInput = base.GetComponent<PlatformCharacterInput>();
			this.PlatformCharacterController = base.GetComponent<PlatformCharacterController>();
			this.Physics = base.GetComponent<PhysicsSwitcher>();
			this.RigidBody = base.GetComponent<Rigidbody2D>();
			this.StateMachine = base.GetComponent<StateMachine>();
			this.ColorFlash = base.GetComponentInChildren<ColorFlash>();
			this.TrapChecker = base.GetComponentInChildren<CheckTrap>();
			this.PenitentMoveAnimations = base.GetComponentInChildren<PenitentMoveAnimations>();
			this.PenitentAttackAnimations = base.GetComponentInChildren<PenitentAttackAnimations>();
			this.StepDustSpawner = base.GetComponentInChildren<StepDustSpawner>();
			this.AttackArea = base.GetComponentInChildren<AttackArea>();
			this.DamageArea = base.GetComponentInChildren<PenitentDamageArea>();
			this.PenitentAttack = base.GetComponentInChildren<PenitentAttack>();
			this.VerticalAttack = base.GetComponentInChildren<VerticalAttack>();
			this.RangeAttack = base.GetComponentInChildren<RangeAttack>();
			this.LungeAttack = base.GetComponentInChildren<LungeAttack>();
			this.SwordSparkSpawner = base.GetComponentInChildren<SwordSparkSpawner>();
			this.GrabCliffLede = base.GetComponentInChildren<GrabCliffLede>();
			this.GrabLadder = base.GetComponentInChildren<GrabLadder>();
			this.Audio = base.GetComponentInChildren<PenitentAudio>();
			this.ChargedAttack = base.GetComponentInChildren<ChargedAttack>();
			this.Dash = base.GetComponentInChildren<Dash>();
			this.FloorChecker = base.GetComponentInChildren<FloorDistanceChecker>();
			this.BloodDecalPumper = base.GetComponentInChildren<BloodDecalPumper>();
			this.MotionLerper = base.GetComponentInChildren<MotionLerper>();
			this.DashDustGenerator = base.GetComponentInChildren<DashDustGenerator>();
			this.Healing = base.GetComponentInChildren<Healing>();
			this.PrayerCast = base.GetComponentInChildren<PrayerUse>();
			this.Penance = base.GetComponentInChildren<FervourPenance>();
			this.Parry = base.GetComponentInChildren<Parry>();
			this.Rumble = base.GetComponentInChildren<EntityRumble>();
			this.GuardSlide = base.GetComponentInChildren<GuardSlide>();
			this.ParticleSystem = base.GetComponentInChildren<ParticleSystem>();
			this.DrivePlayer = base.GetComponentInChildren<DrivePlayer>();
			this.ActiveRiposte = base.GetComponentInChildren<ActiveRiposte>();
			this.ThrowBack = base.GetComponentInChildren<ThrowBack>();
			this.CancelEffect = base.GetComponentInChildren<PenitentCancelEffect>();
			base.Animator.Play("Idle");
			LogicManager.GoToMainMenu = (Core.SimpleEvent)Delegate.Combine(LogicManager.GoToMainMenu, new Core.SimpleEvent(this.GoToMainMenu));
			if (PoolManager.Instance)
			{
				PoolManager.Instance.CreatePool(this.Cherubs.gameObject, 1);
			}
			Entity.Death += this.OnEntityDead;
		}

		internal void OnJumpTrigger(AnimatorInyector animatorInyector)
		{
			if (this.OnJump != null)
			{
				this.OnJump();
			}
		}

		protected override void OnStart()
		{
			base.OnStart();
			this.LevelSleepTime = new LevelSleepTime(this.Normal, this.Heavy, this.Critical);
			this.CameraManager = Core.Logic.CameraManager;
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			this.Status.IsVisibleOnCamera = this.IsVisible();
			if (!this.Status.Dead)
			{
				return;
			}
			if (Core.Logic.CurrentState != LogicStates.PlayerDead)
			{
				Core.Logic.SetState(LogicStates.PlayerDead);
			}
			if (!this.DeathEventLaunched)
			{
				this.DeathEventLaunched = true;
				Core.Logic.PlayerCurrentLife = -1f;
				if (this.OnDead != null)
				{
					this.OnDead();
				}
			}
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			Entity.Death -= this.OnEntityDead;
			LogicManager.GoToMainMenu = (Core.SimpleEvent)Delegate.Remove(LogicManager.GoToMainMenu, new Core.SimpleEvent(this.GoToMainMenu));
		}

		private void GoToMainMenu()
		{
			this.PlatformCharacterInput.ResetInputs();
			Core.Input.SetBlocker("PLAYER_LOGIC", true);
		}

		public void ShowTutorial(string tutorialId)
		{
			if (!Core.TutorialManager.IsTutorialUnlocked(tutorialId))
			{
				base.StartCoroutine(Core.TutorialManager.ShowTutorial(tutorialId, true));
			}
		}

		public void ShowTutorial(string id, float delay)
		{
			base.StartCoroutine(this.ShowTutorialDelayedCoroutine(delay, id));
		}

		private IEnumerator ShowTutorialDelayedCoroutine(float delay, string id)
		{
			yield return new WaitForSeconds(delay);
			this.ShowTutorial(id);
			yield break;
		}

		public bool IsVisible()
		{
			return Entity.IsVisibleFrom(base.SpriteRenderer, Camera.main);
		}

		public void SetInVisible(bool invisible = true)
		{
			Color white = Color.white;
			white.a = ((!invisible) ? 0f : 1f);
			base.SpriteRenderer.color = white;
		}

		public void Damage(Hit hit)
		{
			if (!this.DamageArea)
			{
				return;
			}
			if (this.LungeAttack.Casting && hit.DamageElement == Gameplay.GameControllers.Entities.DamageArea.DamageElement.Contact)
			{
				return;
			}
			if (this.IsOnExecution || this.IsPickingCollectibleItem || this.GuardSlide.Casting)
			{
				return;
			}
			if (this.Parry.IsOnParryChance && hit.DamageType == Gameplay.GameControllers.Entities.DamageArea.DamageType.Normal && !hit.forceGuardslide)
			{
				this.Parry.IsOnParryChance = false;
				if (!this.Parry.CheckParry(hit))
				{
					this.DamageArea.TakeDamage(hit, false);
					return;
				}
			}
			else if (this.Parry.IsOnParryChance && !hit.Unblockable && (hit.DamageType == Gameplay.GameControllers.Entities.DamageArea.DamageType.Heavy || hit.forceGuardslide))
			{
				PenitentSword penitentSword = (PenitentSword)Core.Logic.Penitent.PenitentAttack.CurrentPenitentWeapon;
				Enemy enemy = (!(hit.AttackingEntity != null)) ? null : hit.AttackingEntity.GetComponent<Enemy>();
				if (!hit.CheckOrientationsForGuardslide || penitentSword.IsEnemySameDirection(enemy))
				{
					this.GuardSlide.CastSlide(hit);
					return;
				}
				this.DamageArea.TakeDamage(hit, false);
				return;
			}
			else
			{
				this.DamageArea.TakeDamage(hit, false);
			}
		}

		public Vector3 GetPosition()
		{
			return base.transform.position;
		}

		public Vector2 GetVelocity()
		{
			return this.PlatformCharacterController.InstantVelocity;
		}

		public Core.SimpleEvent OnDead;

		[BoxGroup("Sleep Time By Hit Type", true, false, 0)]
		[SerializeField]
		[Range(0f, 1f)]
		public float Normal;

		[BoxGroup("Sleep Time By Hit Type", true, false, 0)]
		[SerializeField]
		[Range(0f, 1f)]
		public float Heavy;

		[BoxGroup("Sleep Time By Hit Type", true, false, 0)]
		[SerializeField]
		[Range(0f, 1f)]
		public float Critical;

		public bool obtainsFervour = true;

		public bool GuiltDrop = true;

		public bool AllowEquipSwords = true;

		[SerializeField]
		protected GameObject Cherubs;

		public bool cliffLedeClimbingStarted;

		public bool canClimbCliffLede;

		public Vector2 jumpOffRoot;

		public bool isJumpOffReady;

		public bool startedJumpOff;

		private float fadeInTime;

		private bool fadeIn;

		public PIDI_2DReflection reflections;

		public struct CollisionSkin
		{
			public CollisionSkin(Vector3 centerCollision, Vector2 collisionSize)
			{
				this.CenterCollision = centerCollision;
				this.CollisionSize = collisionSize;
			}

			public Vector3 CenterCollision;

			public Vector2 CollisionSize;
		}
	}
}
