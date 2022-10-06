using System;
using System.Collections.Generic;
using System.Diagnostics;
using CreativeSpore.SmartColliders;
using FMODUnity;
using Framework.FrameworkCore;
using Framework.Managers;
using Framework.Util;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.Weapon;
using Gameplay.GameControllers.Penitent.Abilities;
using Gameplay.GameControllers.Penitent.InputSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Penitent.Attack
{
	public class PenitentAttack : Attack
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<PenitentAttack> OnAttackTriggered;

		public bool WindowAttackOpen { get; set; }

		private bool HitImpulseTriggered { get; set; }

		public bool IsHeavyAttackPrayerEquipped { get; set; }

		public bool IsRunningCombo { get; private set; }

		public bool IsRunningUpgradedCombo { get; private set; }

		private bool IsDemakeMode
		{
			get
			{
				return Core.GameModeManager.IsCurrentMode(GameModeManager.GAME_MODES.DEMAKE);
			}
		}

		public int CurrentLevel
		{
			get
			{
				return this._currentLevel;
			}
			set
			{
				this._currentLevel = Mathf.Clamp(value, 1, 2);
			}
		}

		public float AttackSpeed
		{
			get
			{
				return this._attackSpeed;
			}
			set
			{
				this._attackSpeed = Mathf.Clamp(value, 1f, 1.5f);
				PenitentSword penitentSword = (PenitentSword)this.CurrentPenitentWeapon;
				penitentSword.SlashAnimator.SetAnimatorSpeed(this._attackSpeed);
			}
		}

		public PlatformCharacterController PenitentController { get; set; }

		public Weapon CurrentPenitentWeapon { get; set; }

		protected override void OnAwake()
		{
			if (this._penitent != null)
			{
				this.motionLerper = this._penitent.MotionLerper;
			}
			this.PenitentController = base.EntityOwner.GetComponent<PlatformCharacterController>();
			this._playerInput = base.EntityOwner.GetComponent<PlatformCharacterInput>();
			this.isAttacking = false;
			this.CurrentPenitentWeapon = base.GetComponent<Weapon>();
			this.CurrentLevel = 1;
		}

		protected override void OnStart()
		{
			if (this.motionLerper != null)
			{
				this.motionLerper.speedCurve = this.speedCurve;
			}
			this._penitent = (Penitent)base.EntityOwner;
			this._penitent.OnDeath += this.PenitentOnEntityDie;
			this._playerAttackArea = base.EntityOwner.GetComponentInChildren<AttackArea>();
			this._playerAttackAreaOriginalHeight = this._playerAttackArea.transform.localPosition.y;
			this._playerAttackAreaCrouchHeight = this._playerAttackAreaOriginalHeight - Mathf.Abs(this._playerAttackAreaOriginalHeight);
			this.SetAttackAreaSize();
			this.SetAttackAreaOffset();
			this._playerAttackArea.OnStay += this.OnStayAttackArea;
		}

		private void SetAttackAreaSize()
		{
			Bounds bounds = this._penitent.AttackArea.WeaponCollider.bounds;
			float num = (!this.IsDemakeMode) ? bounds.size.x : 2f;
			float num2 = (!this.IsDemakeMode) ? bounds.size.y : 2f;
			this._defaultWeaponColliderSize = new Vector2(num, num2);
		}

		private void SetAttackAreaOffset()
		{
			Vector2 offset = this._penitent.AttackArea.WeaponCollider.offset;
			this._defaultWeaponColliderOffset = new Vector2(offset.x, offset.y);
		}

		protected override void OnUpdate()
		{
			if (this.IsAttackTriggered() && this.OnAttackTriggered != null)
			{
				this.OnAttackTriggered(this);
			}
			if (this._penitent.PlatformCharacterController.IsGrounded)
			{
				this._currentImpulses = 0;
			}
			this.AttackTrigger();
			this.CheckHitImpulseFired();
			bool flag = base.EntityOwner.Animator.GetCurrentAnimatorStateInfo(0).IsName("GroundUpwardAttack");
			bool flag2 = base.EntityOwner.Animator.GetCurrentAnimatorStateInfo(0).IsName("AirUpwardAttack");
			if (!this.IsDemakeMode)
			{
				this._playerInput.IsAttacking = (this.IsRunningBasicCombo() || flag);
			}
			this._playerAttackArea.SetLocalHeight((!this._penitent.IsCrouched) ? this._playerAttackAreaOriginalHeight : this._playerAttackAreaCrouchHeight);
			if (this._penitent.Status.IsGrounded && !this._penitent.ReleaseChargedAttack && !flag && !flag2 && !this._penitent.LungeAttack.Casting && !this.IsRunningUpgradedCombo && !this.isHeavyAttacking)
			{
				this._penitent.AttackArea.SetSize(this._defaultWeaponColliderSize);
				this._penitent.AttackArea.SetOffset(this._defaultWeaponColliderOffset);
			}
			this.AttackSpeed = ((this._penitent.Stats.AttackSpeed.Final <= 1f) ? 1f : this._penitent.Stats.AttackSpeed.Final);
		}

		public void ChargeCombo()
		{
			this.comboCharge++;
		}

		public void ResetCombo()
		{
			if (this.comboCharge > 0)
			{
				this.comboCharge = 0;
			}
		}

		private bool IsComboCharged()
		{
			return this.comboCharge >= 2;
		}

		protected bool IsRunningBasicCombo()
		{
			if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
			{
				this.isAttacking = true;
				this._isComboChained = (this.animator.GetCurrentAnimatorStateInfo(0).normalizedTime > this._animationCompletionThreshold && this.IsAttackTriggered() && this.PenitentController.IsGrounded && this._playerAttackArea.IsTargetHit);
				if (this._isComboChained)
				{
					this.animator.Play(this._combo1HashAnim);
				}
			}
			else if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("Attack_Running"))
			{
				this.isAttacking = true;
				this._isComboChained = (this.animator.GetCurrentAnimatorStateInfo(0).normalizedTime > this._animationCompletionThreshold && this.IsAttackTriggered() && this.PenitentController.IsGrounded && this._playerAttackArea.IsTargetHit);
				if (this._isComboChained)
				{
					this.animator.Play(this._combo1HashAnim);
				}
			}
			else if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("Combo_1"))
			{
				this.isAttacking = true;
				this._isComboChained = (this.animator.GetCurrentAnimatorStateInfo(0).normalizedTime > this._animationCompletionThreshold && this.IsAttackTriggered() && this.PenitentController.IsGrounded);
				if (this._isComboChained && this.IsComboCharged() && this._playerAttackArea.IsTargetHit)
				{
					if (!this.IsDemakeMode)
					{
						this.animator.Play(this._combo3HashAnim);
					}
					else
					{
						this.FinishCombo();
					}
				}
				else if ((this._isComboChained && !this.IsComboCharged()) || (this._isComboChained && this.IsComboCharged()))
				{
					this.animator.Play(this._combo2HashAnim);
				}
			}
			else if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("Combo_2"))
			{
				this.isAttacking = true;
				this._isComboChained = (this.animator.GetCurrentAnimatorStateInfo(0).normalizedTime > this._animationCompletionThreshold && this.IsAttackTriggered() && this.PenitentController.IsGrounded);
				if (this._isComboChained)
				{
					this.animator.Play(this._combo1HashAnim);
				}
			}
			else if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("Combo_3"))
			{
				this.isHeavyAttacking = true;
				this.isAttacking = true;
				this.IsRunningCombo = true;
				this.ResetCombo();
				if (this.Combo.IsAvailable)
				{
					this._isComboChained = (this.animator.GetCurrentAnimatorStateInfo(0).normalizedTime > this._animationCompletionThreshold && this.IsAttackTriggered() && this.PenitentController.IsGrounded);
				}
			}
			else if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("Combo_4") || this.animator.GetCurrentAnimatorStateInfo(0).IsName("ComboFinisherDown"))
			{
				this.IsRunningUpgradedCombo = true;
				this.isHeavyAttacking = true;
				this.isAttacking = true;
			}
			else if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("ComboFinisherUp"))
			{
				this.IsRunningUpgradedCombo = true;
				this.isHeavyAttacking = true;
			}
			else
			{
				this.FinishCombo();
			}
			this.animator.SetBool("ATTACKING", this.isAttacking || this.isRangeAttacking);
			return this.isAttacking;
		}

		private void FinishCombo()
		{
			this.comboCharge = 0;
			this.isAttacking = false;
			this.isHeavyAttacking = false;
			this.IsRunningCombo = false;
			this.IsRunningUpgradedCombo = false;
			this._penitent.EntityAttack.IsWeaponBlowingUp = false;
		}

		public void ClearHitEntityList()
		{
			this._hitEntities.Clear();
		}

		public override void CurrentWeaponAttack(DamageArea.DamageType damageType, bool applyDamageTypeMultipliers)
		{
			base.CurrentWeaponAttack(damageType, applyDamageTypeMultipliers);
			if (this.CurrentPenitentWeapon == null || this._penitent == null)
			{
				return;
			}
			float final = this._penitent.Stats.Strength.Final;
			Hit weapondHit = new Hit
			{
				AttackingEntity = this._penitent.gameObject,
				DamageType = ((!this.IsRunningCombo) ? damageType : ((!this.IsFinalComboAvailable || this.IsRunningUpgradedCombo) ? damageType : DamageArea.DamageType.Normal)),
				DamageAmount = final
			};
			weapondHit.DamageType = ((!this.IsHeavyAttackPrayerEquipped) ? weapondHit.DamageType : DamageArea.DamageType.Heavy);
			if (damageType != DamageArea.DamageType.Normal)
			{
				if (damageType != DamageArea.DamageType.Heavy)
				{
					if (damageType == DamageArea.DamageType.Critical)
					{
						weapondHit.Force = 2f;
						weapondHit.HitSoundId = this.CriticalEnemyHit;
					}
				}
				else
				{
					weapondHit.DestroysProjectiles = true;
					weapondHit.HitSoundId = this.HeavyEnemyHit;
					weapondHit.Force = 2f;
					if (applyDamageTypeMultipliers)
					{
						bool @bool = base.EntityOwner.Animator.GetBool("PARRY");
						weapondHit.DamageAmount = ((!@bool) ? (final * this.HeavyAttackMultiplier) : (final * this.ParryMultiplier));
					}
				}
			}
			else
			{
				weapondHit.HitSoundId = this.SimpleEnemyHit;
				weapondHit.DestroysProjectiles = this.PlayerHasSwordHeart;
				weapondHit.Force = 1f;
			}
			weapondHit.DamageAmount *= base.EntityOwner.Stats.DamageMultiplier.Final;
			if (this.CheckCriticalHit())
			{
				weapondHit.DamageAmount *= base.EntityOwner.Stats.CriticalMultiplier.Final;
				weapondHit.DamageType = DamageArea.DamageType.Critical;
			}
			this.CurrentPenitentWeapon.Attack(weapondHit);
		}

		public override void CurrentWeaponAttack(DamageArea.DamageType damageType)
		{
			base.CurrentWeaponAttack(damageType);
			if (!this.CurrentPenitentWeapon || !this._penitent)
			{
				return;
			}
			float final = this._penitent.Stats.Strength.Final;
			Hit weapondHit = new Hit
			{
				AttackingEntity = this._penitent.gameObject,
				DamageType = ((!this.IsRunningCombo) ? damageType : ((!this.IsFinalComboAvailable || this.IsRunningUpgradedCombo) ? damageType : DamageArea.DamageType.Normal))
			};
			weapondHit.DamageType = ((!this.IsHeavyAttackPrayerEquipped) ? weapondHit.DamageType : DamageArea.DamageType.Heavy);
			switch (damageType)
			{
			case DamageArea.DamageType.Normal:
				weapondHit.HitSoundId = this.SimpleEnemyHit;
				weapondHit.DestroysProjectiles = this.PlayerHasSwordHeart;
				weapondHit.Force = 1f;
				weapondHit.DamageAmount = final;
				break;
			case DamageArea.DamageType.Heavy:
			{
				bool @bool = base.EntityOwner.Animator.GetBool("PARRY");
				weapondHit.DamageAmount = ((!@bool) ? (final * this.HeavyAttackMultiplier) : (final * this.ParryMultiplier));
				weapondHit.HitSoundId = this.HeavyEnemyHit;
				weapondHit.DestroysProjectiles = true;
				weapondHit.Force = 2f;
				break;
			}
			case DamageArea.DamageType.Critical:
				weapondHit.HitSoundId = this.CriticalEnemyHit;
				weapondHit.Force = 2f;
				break;
			default:
				weapondHit.DamageAmount = final;
				break;
			}
			weapondHit.DamageAmount *= base.EntityOwner.Stats.DamageMultiplier.Final;
			if (this.CheckCriticalHit())
			{
				weapondHit.DamageAmount *= base.EntityOwner.Stats.CriticalMultiplier.Final;
				weapondHit.DamageType = DamageArea.DamageType.Critical;
			}
			this.CurrentPenitentWeapon.Attack(weapondHit);
		}

		private void AttackTrigger()
		{
			this._deltaAttackTimeThreshold += Time.deltaTime;
			if (this._playerInput.Attack)
			{
				this._deltaAttackTimeThreshold = 0f;
			}
		}

		private void CheckHitImpulseFired()
		{
			this.HitImpulseTriggered = (!this._penitent.Status.IsGrounded && (this._penitent.PlatformCharacterInput.isJoystickDown || this._penitent.PlatformCharacterInput.IsDashButtonHold));
		}

		private bool IsAttackTriggered()
		{
			return this._deltaAttackTimeThreshold <= this._attackTimeThreshold;
		}

		private bool CheckCriticalHit()
		{
			float num = Random.Range(0f, 1f);
			return num <= base.EntityOwner.Stats.CriticalChance.Final;
		}

		public float GetExecutionBonus()
		{
			float result = 0f;
			if (this.Combo.IsAvailable)
			{
				UnlockableSkill getMaxSkill = this.Combo.GetMaxSkill;
				result = ((!getMaxSkill.id.Equals("COMBO_1")) ? this.SecondComboTierExecutionBonus : this.FirstComboTierExecutionBonus);
			}
			return result;
		}

		private bool IsFinalComboAvailable
		{
			get
			{
				if (!this.Combo.IsAvailable)
				{
					return false;
				}
				UnlockableSkill getMaxSkill = this.Combo.GetMaxSkill;
				return getMaxSkill.id.Equals("COMBO_3");
			}
		}

		private void OnStayAttackArea(object sender, Collider2DParam e)
		{
			if (!this.WindowAttackOpen)
			{
				return;
			}
			GameObject gameObject = e.Collider2DArg.gameObject;
			if (!this._hitEntities.Contains(gameObject))
			{
				this._newEnemyHit = false;
				if (this._hitEntities.Count < 1)
				{
					this.HitImpulse();
				}
				this._hitEntities.Add(gameObject);
				if (gameObject.transform.root.GetComponentInChildren<IDamageable>() == null)
				{
					return;
				}
				this.CurrentWeaponAttack(DamageArea.DamageType.Normal, true);
			}
		}

		private void HitImpulse()
		{
			if (!this._penitent.PlatformCharacterController.IsGrounded && (float)this._currentImpulses < base.EntityOwner.Stats.AirImpulses.Final && this.HitImpulseTriggered)
			{
				Vector2 vector;
				vector..ctor(this._penitent.PlatformCharacterController.InstantVelocity.x, 1f);
				this._penitent.PlatformCharacterController.PlatformCharacterPhysics.Velocity = vector * this.airAttackImpulse;
				this._currentImpulses++;
			}
		}

		private void PenitentOnEntityDie()
		{
			this._penitent.Animator.speed = 1f;
			this._hitEntities.Clear();
			this._playerAttackArea.OnStay -= this.OnStayAttackArea;
		}

		public bool PlayerHasSwordHeart
		{
			get
			{
				return Core.InventoryManager.IsSwordEquipped("HE10");
			}
		}

		private readonly int _combo1HashAnim = Animator.StringToHash("Combo_1");

		private readonly int _combo2HashAnim = Animator.StringToHash("Combo_2");

		private readonly int _combo3HashAnim = Animator.StringToHash("Combo_3");

		public Combo Combo;

		[SerializeField]
		[BoxGroup("Prayer Effect", true, false, 0)]
		[Range(0f, 100f)]
		[Tooltip("Life amount percentage drained to an enemy by usage of Distressing Saeta")]
		public float LifeDrainedByPrayerUse;

		[SerializeField]
		[BoxGroup("Audio Attack", true, false, 0)]
		[EventRef]
		public string SimpleEnemyHit;

		[SerializeField]
		[BoxGroup("Audio Attack", true, false, 0)]
		[EventRef]
		public string HeavyEnemyHit;

		[SerializeField]
		[BoxGroup("Audio Attack", true, false, 0)]
		[EventRef]
		public string CriticalEnemyHit;

		private List<GameObject> _hitEntities = new List<GameObject>();

		private bool _newEnemyHit;

		[BoxGroup("Combo Chance by Tier", true, false, 0)]
		public float FirstComboTierExecutionBonus = 3f;

		[BoxGroup("Combo Chance by Tier", true, false, 0)]
		public float SecondComboTierExecutionBonus = 6f;

		[BoxGroup("Air attack", true, false, 0)]
		public float airAttackImpulse = 6f;

		private int _currentImpulses;

		private Penitent _penitent;

		public int comboCharge;

		public const int MAX_COMBO_CHARGE = 2;

		private Vector2 _defaultWeaponColliderSize;

		private Vector2 _defaultWeaponColliderOffset;

		private float _animationCompletionThreshold = 0.8f;

		private float _attackTimeThreshold = 0.15f;

		private float _deltaAttackTimeThreshold;

		private AttackArea _playerAttackArea;

		private float _playerAttackAreaOriginalHeight;

		private float _playerAttackAreaCrouchHeight;

		public float HeavyAttackMultiplier;

		[Range(1f, 10f)]
		public float ParryMultiplier = 2f;

		public PenitentSword.AttackColor AttackColor;

		private int _currentLevel;

		private float _attackSpeed;

		private PlatformCharacterInput _playerInput;

		private bool _isComboChained;
	}
}
