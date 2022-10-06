using System;
using System.Collections;
using Com.LuisPedroFonseca.ProCamera2D;
using DG.Tweening;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.AnimationBehaviours.Player.Attack;
using Gameplay.GameControllers.Effects.Player.Sparks;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Penitent;
using Gameplay.GameControllers.Penitent.Abilities;
using Gameplay.GameControllers.Penitent.Attack;
using Gameplay.GameControllers.Penitent.Movement;
using Sirenix.OdinInspector;
using UnityEngine;

public class TPOAnimatedFollower : MonoBehaviour
{
	private void Awake()
	{
		this.animator = base.GetComponentInChildren<Animator>();
		this.spriteRenderer = base.GetComponent<SpriteRenderer>();
		LevelManager.OnLevelLoaded += this.LevelManager_OnLevelLoaded;
		this.audioEvents = base.GetComponent<TPOFollowerAudioEvents>();
		PoolManager.Instance.CreatePool(this.deathSparkPrefab, 8);
		DemakeManager demakeManager = Core.DemakeManager;
		demakeManager.OnDemakeLevelCompletion = (Core.SimpleEvent)Delegate.Combine(demakeManager.OnDemakeLevelCompletion, new Core.SimpleEvent(this.OnDemakeVictory));
	}

	private void OnDemakeVictory()
	{
		this.SetVictory(true);
	}

	private void LevelManager_OnLevelLoaded(Level oldLevel, Level newLevel)
	{
		if (newLevel.LevelName.StartsWith("D25"))
		{
			this.ApplyDemakeCameraSettings();
			this.target = Core.Logic.Penitent;
			this.target.SpriteRenderer.enabled = false;
			this.DeactivateTPOAbilities();
			this.LoadMotionProfile();
			this.target.OnAttackBehaviourEnters += this.Target_OnAttackBehaviourEnters;
			this.target.OnAirAttackBehaviourEnters += this.Target_OnAirAttackBehaviourEnters;
			this.target.OnDeath += this.Target_OnDeath;
			this.target.OnDamaged += this.Target_OnDamaged;
			this.target.OnJump += this.Target_OnJump;
			this.swordAnimator = this.target.GetComponentInChildren<SwordAnimatorInyector>();
			this.LoadBloodVfx();
			if (this.swordAnimator)
			{
				this.swordAnimator.SwordAnimator.runtimeAnimatorController = this.demakeSlashAnimatorController;
			}
		}
		else
		{
			LevelManager.OnLevelLoaded -= this.LevelManager_OnLevelLoaded;
		}
	}

	private void Target_OnJump()
	{
		this.audioEvents.PlayJumpFX();
	}

	private void Target_OnDamaged()
	{
		this.damageCoolDown = 0.1f;
		this.attackCoolDown = 0f;
		this.PlayHurt();
	}

	private void Target_OnDeath()
	{
		this.PlayDeath();
	}

	private void DeactivateTPOAbilities()
	{
		if (this.abilityDeactivator)
		{
			this.abilityDeactivator.SetUp();
		}
	}

	private void LoadMotionProfile()
	{
		if (this.target)
		{
			this.motionProfile.Init(this.target.PlatformCharacterController);
		}
	}

	private void LoadBloodVfx()
	{
		if (this.bloodVFXTable && this.target)
		{
			BloodSpawner componentInChildren = this.target.GetComponentInChildren<BloodSpawner>();
			if (!componentInChildren)
			{
				return;
			}
			componentInChildren.bloodVFXTable = this.bloodVFXTable;
			foreach (BloodFXTableElement bloodFXTableElement in componentInChildren.bloodVFXTable.bloodVFXList)
			{
				PoolManager.Instance.CreatePool(bloodFXTableElement.prefab, bloodFXTableElement.poolSize);
			}
		}
	}

	[Button("Test death sparks", 0)]
	private void PlayDeathSparks()
	{
		float num = 25f;
		float num2 = 1.5f;
		Vector3 up = Vector3.up;
		for (int i = 0; i < 8; i++)
		{
			GameObject gameObject = PoolManager.Instance.ReuseObject(this.deathSparkPrefab, base.transform.position + up, Quaternion.identity, false, 1).GameObject;
			Vector2 vector = Quaternion.Euler(0f, 0f, (float)(45 * i)) * Vector2.right;
			TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(gameObject.transform, gameObject.transform.position + vector * num, num2, false), 7);
		}
	}

	private void ApplyDemakeCameraSettings()
	{
		Object.FindObjectOfType<ProCamera2DPixelPerfect>().PixelsPerUnit = 16f;
	}

	private void Target_OnAirAttackBehaviourEnters(AirAttackBehaviour obj)
	{
		this.PlayAirAttack();
	}

	private void Target_OnAttackBehaviourEnters(AttackBehaviour obj)
	{
		this.PlayAttack();
		this.attackCoolDown = 0.15f;
	}

	private void Update()
	{
		float deltaTime = Time.deltaTime;
		this.CheckAttackCooldown(deltaTime);
		this.CheckDamageCooldown(deltaTime);
	}

	private void LateUpdate()
	{
		if (this.target)
		{
			this.CopyTargetPosition();
			this.CopyTargetOrientation();
			this.CopyTargetGrounded();
			this.CopyTargetSpeed();
			this.CopyDash();
		}
	}

	private void CheckAttackCooldown(float dt)
	{
		if (!this.target)
		{
			return;
		}
		this.attackCoolDown -= dt;
		bool isAttacking = this.attackCoolDown > 0f;
		this.target.PenitentAttack.IsAttacking = isAttacking;
		this.target.PlatformCharacterInput.IsAttacking = isAttacking;
	}

	private void CheckDamageCooldown(float dt)
	{
		if (!this.target)
		{
			return;
		}
		this.damageCoolDown -= dt;
		bool isHurt = this.damageCoolDown > 0f;
		this.target.Status.IsHurt = isHurt;
	}

	private void CopyDash()
	{
		this.SetDashing(this.target.IsDashing);
	}

	private void CopyTargetSpeed()
	{
		float num = Math.Abs(this.target.PlatformCharacterController.PlatformCharacterPhysics.HSpeed);
		this.SetRun(num > 0.5f);
	}

	private void CopyTargetGrounded()
	{
		if (this.target.PlatformCharacterController.IsGrounded)
		{
			this.SetGrounded();
		}
		else
		{
			this.SetOnTheAir();
		}
	}

	private void CopyTargetOrientation()
	{
		base.transform.position = this.target.GetPosition();
	}

	private void CopyTargetPosition()
	{
		this.spriteRenderer.flipX = (this.target.Status.Orientation != EntityOrientation.Right);
	}

	private void PlayAttack()
	{
		this.animator.SetTrigger(this._attackAnimHash);
		this.audioEvents.PlayAttackFX();
	}

	private void PlayHurt()
	{
		this.animator.SetTrigger(this._hurtAnimHash);
		this.audioEvents.PlayHurtFX();
	}

	private void PlayDeath()
	{
		this.animator.SetTrigger(this._deathAnimHash);
		this.audioEvents.PlayDeathFX();
	}

	private void PlayAirAttack()
	{
		this.LandingAttack();
		this.animator.SetTrigger(this._airAttackAnimHash);
		this.audioEvents.PlayAttackFX();
	}

	private void LandingAttack()
	{
		if (!this.target)
		{
			return;
		}
		bool flag = this.target.PlatformCharacterController.PlatformCharacterPhysics.VSpeed < 0f;
		bool flag2 = this.target.PlatformCharacterController.GroundDist < 2f;
		if (this.TargetIsAirAttacking && flag2 && flag)
		{
			base.StartCoroutine(this.AirAttackCoroutine());
		}
	}

	private IEnumerator AirAttackCoroutine()
	{
		yield return new WaitForSeconds(0.1f);
		this.target.PenitentAttack.CurrentWeaponAttack(DamageArea.DamageType.Normal);
		yield break;
	}

	private void SetOnTheAir()
	{
		this.animator.SetBool(this._jumpAnimHash, true);
	}

	private void SetVictory(bool victory)
	{
		this.animator.SetBool(this._victoryAnimHash, victory);
	}

	private void SetDashing(bool dashing)
	{
		this.animator.SetBool(this._dashAnimHash, dashing);
	}

	private void SetGrounded()
	{
		this.animator.SetBool(this._jumpAnimHash, false);
	}

	private void SetRun(bool isRunning)
	{
		this.animator.SetBool(this._runAnimHash, isRunning);
	}

	private bool TargetIsAirAttacking
	{
		get
		{
			return this.target && this.target.Animator.GetCurrentAnimatorStateInfo(0).IsName("Air Attack 1");
		}
	}

	private void OnDestroy()
	{
		if (this.target)
		{
			this.target.OnAttackBehaviourEnters -= this.Target_OnAttackBehaviourEnters;
			this.target.OnAirAttackBehaviourEnters -= this.Target_OnAirAttackBehaviourEnters;
			this.target.OnDeath -= this.Target_OnDeath;
			this.target.OnJump -= this.Target_OnJump;
			this.target.OnDamaged -= this.Target_OnDamaged;
		}
		DemakeManager demakeManager = Core.DemakeManager;
		demakeManager.OnDemakeLevelCompletion = (Core.SimpleEvent)Delegate.Remove(demakeManager.OnDemakeLevelCompletion, new Core.SimpleEvent(this.OnDemakeVictory));
		LevelManager.OnLevelLoaded -= this.LevelManager_OnLevelLoaded;
	}

	private Animator animator;

	private SpriteRenderer spriteRenderer;

	private readonly int _upwardAttackAnimHash = Animator.StringToHash("GroundUpwardAttack");

	private readonly int _airupwardAttackAnimHash = Animator.StringToHash("AirUpwardAttack");

	private readonly int _jumpAnimHash = Animator.StringToHash("Air");

	private readonly int _runAnimHash = Animator.StringToHash("Run");

	private readonly int _attackAnimHash = Animator.StringToHash("GroundAttack");

	private readonly int _airAttackAnimHash = Animator.StringToHash("AirAttack");

	private readonly int _dashAnimHash = Animator.StringToHash("Dash");

	private readonly int _hurtAnimHash = Animator.StringToHash("Hurt");

	private readonly int _deathAnimHash = Animator.StringToHash("Death");

	private readonly int _victoryAnimHash = Animator.StringToHash("Victory");

	private const float HorSpeedThreshold = 0.5f;

	private const float AttackDuration = 0.15f;

	private float attackCoolDown;

	private const float DamageBlockDuration = 0.1f;

	private float damageCoolDown;

	private Penitent target;

	private TPOFollowerAudioEvents audioEvents;

	private SwordAnimatorInyector swordAnimator;

	public RuntimeAnimatorController demakeSlashAnimatorController;

	public GameObject deathSparkPrefab;

	[SerializeField]
	private AbilityDeactivator abilityDeactivator;

	[SerializeField]
	private BloodVFXTable bloodVFXTable;

	[SerializeField]
	private CharacterMotionProfile motionProfile;
}
