using System;
using System.Text.RegularExpressions;
using CreativeSpore.SmartColliders;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using Gameplay.UI.Others.UIGameLogic;
using UnityEngine;

namespace Gameplay.GameControllers.Penitent.Damage
{
	[RequireComponent(typeof(BoxCollider2D))]
	public class PenitentDamageArea : DamageArea
	{
		public bool IsFallingForwardResized { get; set; }

		public Vector2 DefaultSkinColliderCenter { get; private set; }

		public Vector2 DefaultSkinColliderSize { get; private set; }

		public bool PrayerProtectionEnabled { get; set; }

		public bool IsIncludeEnemyLayer { get; private set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			this._damageAreaCollider = base.GetComponent<BoxCollider2D>();
			this._damageAreaOriginalOffset = this._damageAreaCollider.offset;
			this._damageAreaOriginalSize = this._damageAreaCollider.size;
			this._penitent = base.GetComponentInParent<Penitent>();
		}

		protected override void OnStart()
		{
			base.OnStart();
			SmartPlatformCollider smartPlatformCollider = this._penitent.PlatformCharacterController.SmartPlatformCollider;
			this.DefaultSkinColliderCenter = new Vector2(smartPlatformCollider.Center.x, smartPlatformCollider.Center.y);
			this.DefaultSkinColliderSize = new Vector2(smartPlatformCollider.Size.x, smartPlatformCollider.Size.y);
			this._enemyAttackAreaLayerValue = LayerMask.NameToLayer("Gate");
			this._enableEnemyAttack = true;
			this._logicManager = Core.Logic;
			MotionLerper motionLerper = this._penitent.MotionLerper;
			motionLerper.OnLerpStop = (Core.SimpleEvent)Delegate.Combine(motionLerper.OnLerpStop, new Core.SimpleEvent(this.OnLerpStop));
			MotionLerper motionLerper2 = this._penitent.MotionLerper;
			motionLerper2.OnLerpStart = (Core.SimpleEvent)Delegate.Combine(motionLerper2.OnLerpStart, new Core.SimpleEvent(this.OnLerpStart));
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			this.DeltaRecoverTime += Time.deltaTime;
			this.ResizeDamageArea();
			if (this._penitent.Status.Dead && this._damageAreaCollider.enabled && !this._penitent.Animator.GetBool(PenitentDamageArea.Throw))
			{
				this._damageAreaCollider.enabled = false;
			}
			if (!this._penitent.MotionLerper.IsLerping)
			{
				return;
			}
			if (!this._penitent.FloorChecker.IsGrounded || this._penitent.FloorChecker.IsSideBlocked)
			{
				this._penitent.MotionLerper.StopLerping();
			}
		}

		private void ResizeDamageArea()
		{
			if (this._penitent.AnimatorInyector.IsJumpingForward && this.IsFallingForwardResized && this._penitent.PlatformCharacterController.GroundDist >= 1.5f)
			{
				this.SetTopSmallDamageArea();
				return;
			}
			if ((this._penitent.IsCrouched || this._penitent.Dash.CrouchAfterDash || this._penitent.IsDashing || this._penitent.LungeAttack.Casting) && !this._damageAreaResized)
			{
				this.SetBottomSmallDamageArea();
			}
			else if (!this._penitent.IsCrouched && !this._penitent.Dash.CrouchAfterDash && !this._penitent.IsDashing && !this._penitent.LungeAttack.Casting && this._damageAreaResized)
			{
				this.SetDefaultDamageArea();
			}
		}

		private void SetBottomSmallDamageArea()
		{
			this._damageAreaResized = true;
			this._damageAreaCollider.size = new Vector2(this._damageAreaCollider.size.x + this._damageAreaCollider.size.x * this.damageAreaIncrementalXSizeFactor, this._damageAreaCollider.size.y / 2f);
			this._damageAreaCollider.offset = new Vector2(this._damageAreaCollider.offset.x + this._damageAreaCollider.offset.x * this.damageAreaIncrementalXOffsetFactor, this._damageAreaCollider.size.y / 2f);
		}

		private void SetTopSmallDamageArea()
		{
			this._damageAreaResized = true;
			Vector2 size;
			size..ctor(this._damageAreaCollider.size.x, 0.6f);
			Vector2 offset;
			offset..ctor(this._damageAreaCollider.offset.x, 1.1f);
			this._damageAreaCollider.size = size;
			this._damageAreaCollider.offset = offset;
		}

		private void SetDefaultDamageArea()
		{
			this._damageAreaResized = false;
			this._damageAreaCollider.size = this._damageAreaOriginalSize;
			this._damageAreaCollider.offset = this._damageAreaOriginalOffset;
		}

		private void OnCollisionEnter2D(Collision2D collision)
		{
			if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy") && collision.contacts.Length > 0)
			{
				ContactPoint2D contactPoint2D = collision.contacts[0];
				if ((double)Vector3.Dot(contactPoint2D.normal, Vector3.up) > 0.5)
				{
					Debug.Log("collision was from below");
				}
			}
		}

		public override void TakeDamage(Gameplay.GameControllers.Entities.Hit hit, bool force = false)
		{
			if (Core.PenitenceManager.UseStocksOfHealth)
			{
				hit.DamageAmount = PlayerHealthPE02.StocksDamage;
			}
			base.TakeDamage(hit, force);
			this.RaiseHitEvent(hit);
			if (!force && !this.CanTakeHit(hit))
			{
				return;
			}
			if (this.DeltaRecoverTime < this.RecoverTime)
			{
				return;
			}
			this.DeltaRecoverTime = 0f;
			base.LastHit = hit;
			this.CameraShake();
			this.TriggerLevelSleepTime(hit);
			switch (hit.DamageType)
			{
			case DamageArea.DamageType.Normal:
				this._penitent.Rumble.UsePreset("NormalDamage");
				if (this._penitent.AnimatorInyector.IsAirAttacking || this._penitent.AnimatorInyector.IsJumpingForward)
				{
					this._penitent.PenitentMoveAnimations.GetPushBackSparks();
					if (this._penitent.Status.Dead)
					{
						this._penitent.Animator.Play(this._airHurtAnimHash, 0, 0f);
					}
				}
				else if (!this._penitent.Status.Dead)
				{
					this.SetDamageAnimation(hit.DamageType, hit.AttackingEntity.transform.position);
				}
				break;
			case DamageArea.DamageType.Heavy:
				this._penitent.Rumble.UsePreset("HeavyDamage");
				this.SetDamageAnimation(hit.DamageType, hit.AttackingEntity.transform.position);
				break;
			case DamageArea.DamageType.Critical:
				break;
			case DamageArea.DamageType.Simple:
				this._penitent.Rumble.UsePreset("NormalDamage");
				this.SetDamageAnimation(hit.DamageType, hit.AttackingEntity.transform.position);
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			this.RaiseDamageEvent(hit);
		}

		private bool CanTakeHit(Gameplay.GameControllers.Entities.Hit hit)
		{
			bool result;
			if (this.PrayerProtectionEnabled || this._penitent.Status.Invulnerable)
			{
				result = false;
			}
			else if (this._penitent.Status.Unattacable)
			{
				result = hit.Unnavoidable;
			}
			else
			{
				result = !this._penitent.Status.IsHurt;
			}
			return result;
		}

		public void EnableEnemyAttack(bool enable = true)
		{
			if (enable && !this._enableEnemyAttack)
			{
				this._enableEnemyAttack = true;
				this.enemyAttackAreaLayer |= 1 << this._enemyAttackAreaLayerValue;
			}
			else if (!enable && this._enableEnemyAttack)
			{
				this._enableEnemyAttack = false;
				this.enemyAttackAreaLayer ^= 1 << this._enemyAttackAreaLayerValue;
			}
		}

		public void IncludeEnemyLayer(bool include = true)
		{
		}

		private void RaiseDamageEvent(Gameplay.GameControllers.Entities.Hit hit)
		{
			float num = hit.DamageAmount;
			if (!Core.PenitenceManager.UseStocksOfHealth)
			{
				num = this._penitent.GetReducedDamage(hit);
			}
			if (Core.LevelManager.currentLevel.LevelName.Equals("D24Z01S01") && Core.Logic.Penitent.CurrentLife - num <= 0f)
			{
				num = Core.Logic.Penitent.CurrentLife - 1f;
				Core.Logic.ScreenFreeze.Freeze(0.05f, 0.5f, 0f, null);
				Core.Logic.Penitent.Physics.Enable2DCollision(false);
			}
			this.CheckPenitentKilledEvent(num, hit.AttackingEntity);
			int num2 = Mathf.CeilToInt(num);
			this._penitent.Damage((float)num2, hit.HitSoundId);
			this._logicManager.PlayerCurrentLife = this._penitent.Stats.Life.Current;
			if (this.OnDamaged != null)
			{
				this.OnDamaged(this._penitent, hit);
			}
			if (PenitentDamageArea.OnDamagedGlobal != null)
			{
				PenitentDamageArea.OnDamagedGlobal(this._penitent, hit);
			}
		}

		private void RaiseHitEvent(Gameplay.GameControllers.Entities.Hit hit)
		{
			if (PenitentDamageArea.OnHitGlobal != null)
			{
				PenitentDamageArea.OnHitGlobal(this._penitent, hit);
			}
		}

		private void CheckPenitentKilledEvent(float damage, GameObject attacker)
		{
			if (!attacker)
			{
				return;
			}
			if (this._penitent.Status.Dead)
			{
				return;
			}
			string text = Regex.Replace(attacker.name, " \\([1-9]\\)", string.Empty);
			text = text.Replace("(Clone)", string.Empty);
			if (Core.Logic.Penitent.CurrentLife - damage <= 0f)
			{
				Core.Metrics.CustomEvent("PLAYER_DEATH", text, -1f);
			}
			Core.Metrics.HeatmapEvent("PLAYER_DEATH", this._penitent.transform.position);
		}

		private void TriggerLevelSleepTime(Gameplay.GameControllers.Entities.Hit hit)
		{
			this._penitent.PenitentAttackAnimations.LevelSleepTime(0.1f);
		}

		private void CameraShake()
		{
			if (this._penitent.CameraManager.ProCamera2DShake)
			{
				this._penitent.CameraManager.ProCamera2DShake.ShakeUsingPreset("SimpleHit");
			}
		}

		public void SetDamageAnimation(DamageArea.DamageType damageType, Vector3 enemyPosition)
		{
			if (this._penitent.AnimatorInyector.IsHoldingChargeAttack && damageType != DamageArea.DamageType.Heavy)
			{
				return;
			}
			if (damageType != DamageArea.DamageType.Simple)
			{
				this._penitent.HurtOrientation = this._penitent.SetOrientationbyHit(enemyPosition);
			}
			this._penitent.AnimatorInyector.PlayerGetDamage(damageType);
		}

		public void HitDisplacement(Vector3 enemyPos)
		{
			if (!this._penitent.Status.IsGrounded || base.LastHit.forceGuardslide || this._penitent.Status.Dead)
			{
				return;
			}
			if (!this._penitent.HasFlag("SIDE_BLOCKED"))
			{
				this.HitDisplacementForce(enemyPos);
			}
		}

		private void OnLerpStop()
		{
			this.IncludeEnemyLayer(true);
		}

		private void OnLerpStart()
		{
			this.IncludeEnemyLayer(false);
		}

		private void HitDisplacementForce(Vector3 enemyPos)
		{
			bool flag = Core.GameModeManager.IsCurrentMode(GameModeManager.GAME_MODES.DEMAKE);
			Vector3 dir = (enemyPos.x <= base.transform.position.x) ? base.transform.right : (-base.transform.right);
			if (this._penitent.MotionLerper.IsLerping || this._penitent.SlopeAngle > 1f)
			{
				return;
			}
			this._penitent.MotionLerper.TimeTakenDuringLerp = ((!flag) ? 0.85f : 0.15f);
			this._penitent.MotionLerper.distanceToMove = ((!flag) ? 2.75f : 1f);
			this._penitent.MotionLerper.StartLerping(dir);
		}

		public static PenitentDamageArea.PlayerDamagedEvent OnDamagedGlobal;

		public static PenitentDamageArea.PlayerHitEvent OnHitGlobal;

		public PenitentDamageArea.PlayerDamagedEvent OnDamaged;

		private Penitent _penitent;

		private Vector2 _damageAreaOriginalOffset;

		private Vector2 _damageAreaOriginalSize;

		[Range(0f, 0.5f)]
		public float damageAreaIncrementalXSizeFactor = 0.2f;

		[Range(0f, 0.5f)]
		public float damageAreaIncrementalXOffsetFactor = 0.2f;

		private bool _enableEnemyAttack;

		private int _enemyAttackAreaLayerValue;

		private bool _damageAreaResized;

		private LogicManager _logicManager;

		[Tooltip("Invulnerability lapse after ground hurt animation ends.")]
		public float InvulnerabilityLapse = 0.5f;

		private BoxCollider2D _damageAreaCollider;

		private readonly int _airHurtAnimHash = Animator.StringToHash("Hurt In The Air");

		private static readonly int Throw = Animator.StringToHash("THROW");

		public delegate void PlayerDamagedEvent(Penitent damaged, Gameplay.GameControllers.Entities.Hit hit);

		public delegate void PlayerHitEvent(Penitent penitent, Gameplay.GameControllers.Entities.Hit hit);
	}
}
