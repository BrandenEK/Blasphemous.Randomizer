using System;
using System.Collections.Generic;
using CreativeSpore.SmartColliders;
using DG.Tweening;
using FMOD.Studio;
using FMODUnity;
using Framework.FrameworkCore;
using Framework.Managers;
using Framework.Util;
using Gameplay.GameControllers.Entities;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace Gameplay.GameControllers.Penitent.Abilities
{
	public class LungeAttack : Ability
	{
		public bool CanHit { get; set; }

		private float GetDamageFactorByLevel()
		{
			if (StringExtensions.IsNullOrWhitespace(base.LastUnlockedSkillId))
			{
				return 1f;
			}
			string lastUnlockedSkillId = base.LastUnlockedSkillId;
			if (lastUnlockedSkillId != null)
			{
				if (lastUnlockedSkillId == "LUNGE_1")
				{
					return this.DamageFactor1;
				}
				if (lastUnlockedSkillId == "LUNGE_2")
				{
					return this.DamageFactor2;
				}
				if (lastUnlockedSkillId == "LUNGE_3")
				{
					return this.DamageFactor3;
				}
			}
			return this.DamageFactor1;
		}

		public float GetLungeSpeedByLevel()
		{
			if (StringExtensions.IsNullOrWhitespace(base.LastUnlockedSkillId))
			{
				return 1f;
			}
			string lastUnlockedSkillId = base.LastUnlockedSkillId;
			if (lastUnlockedSkillId != null)
			{
				if (lastUnlockedSkillId == "LUNGE_1")
				{
					return this.LungeMaxWalkingSpeed;
				}
				if (lastUnlockedSkillId == "LUNGE_2")
				{
					return this.LungeMaxWalkingSpeed2;
				}
				if (lastUnlockedSkillId == "LUNGE_3")
				{
					return this.LungeMaxWalkingSpeed3;
				}
			}
			return this.LungeMaxWalkingSpeed;
		}

		public float GetLungeLapseByLevel()
		{
			if (StringExtensions.IsNullOrWhitespace(base.LastUnlockedSkillId))
			{
				return 1f;
			}
			string lastUnlockedSkillId = base.LastUnlockedSkillId;
			if (lastUnlockedSkillId != null)
			{
				if (lastUnlockedSkillId == "LUNGE_1")
				{
					return this.LungeLapse;
				}
				if (lastUnlockedSkillId == "LUNGE_2")
				{
					return this.LungeLapse2;
				}
				if (lastUnlockedSkillId == "LUNGE_3")
				{
					return this.LungeLapse3;
				}
			}
			return this.LungeLapse;
		}

		protected override void OnStart()
		{
			base.OnStart();
			this._penitent = (Penitent)base.EntityOwner;
			this._playerController = this._penitent.PlatformCharacterController;
			this._attackArea = this._penitent.GetComponentInChildren<AttackArea>();
			this._attackArea.OnEnter += this.AttackAreaOnEnter;
			this._attackArea.OnStay += this.AttackAreaOnStay;
			this.LungeMoveSetting = new Dash.MoveSetting(this.LungeDrag, this.LungeMaxWalkingSpeed);
			this._lungeHit = new Hit
			{
				AttackingEntity = this._penitent.gameObject,
				DamageType = DamageArea.DamageType.Heavy,
				HitSoundId = this.LungeEnemyHitFx
			};
			this._hitEntities = new List<GameObject>();
			this._bottomHits = new RaycastHit2D[2];
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			if (base.Casting)
			{
				bool flag = this.IsGrounded();
				if (!flag)
				{
					this._penitent.PlatformCharacterController.PlatformCharacterPhysics.HSpeed = 0f;
				}
				this._currentLungeLapse += Time.deltaTime;
				if (this._currentLungeLapse <= this.GetLungeLapseByLevel())
				{
					if (flag)
					{
						this.AddLungeForce();
					}
				}
				else
				{
					this.StopLungeForce();
				}
			}
		}

		protected override void OnCastStart()
		{
			base.OnCastStart();
			if (this.OnLungeAttackStart != null)
			{
				this.OnLungeAttackStart();
			}
			base.LastUnlockedSkillId = base.GetLastUnlockedSkill().id;
			Core.Audio.EventOneShotPanned(this.GetLungeFxKeyByLevel, base.transform.position, out this._lungeMovementFx);
			this._playerController.WalkingDrag = this.LungeDrag;
			this._playerController.MaxWalkingSpeed = this.GetLungeSpeedByLevel();
		}

		protected override void OnCastEnd(float castingTime)
		{
			base.OnCastEnd(castingTime);
			this._currentLungeLapse = 0f;
			this.CanHit = false;
			this._playerController.WalkingDrag = this._penitent.Dash.DefaultMoveSetting.Drag;
			this._playerController.MaxWalkingSpeed = this._penitent.Dash.DefaultMoveSetting.Speed;
			if (this._lungeMovementFx.isValid())
			{
				this._lungeMovementFx.stop(1);
				this._lungeMovementFx.release();
			}
			this.ClearHitEntityList();
		}

		public bool IsAvailable
		{
			get
			{
				return base.CanExecuteSkilledAbility() && base.HasEnoughFervour;
			}
		}

		private void AttackAreaOnEnter(object sender, Collider2DParam e)
		{
			this._newEnemyHit = true;
		}

		private void AttackAreaOnStay(object sender, Collider2DParam e)
		{
			if (!this.CanHit)
			{
				return;
			}
			if (base.Casting)
			{
				if (!this._newEnemyHit)
				{
					return;
				}
				GameObject gameObject = e.Collider2DArg.gameObject;
				if (!this._hitEntities.Contains(gameObject))
				{
					this._newEnemyHit = false;
					this._hitEntities.Add(gameObject);
					IDamageable componentInParent = gameObject.GetComponentInParent<IDamageable>();
					this.AttackDamageableEntity(componentInParent);
				}
			}
		}

		private void ClearHitEntityList()
		{
			if (this._hitEntities.Count > 0)
			{
				this._hitEntities.Clear();
			}
		}

		private void AttackDamageableEntity(IDamageable damageable)
		{
			if (damageable == null)
			{
				return;
			}
			float damageFactorByLevel = this.GetDamageFactorByLevel();
			this._lungeHit.DamageAmount = this._penitent.Stats.Strength.Final * damageFactorByLevel;
			Enemy enemy = damageable as Enemy;
			if (enemy != null)
			{
				if (enemy.IsVulnerable)
				{
					enemy.GetStun(this._lungeHit);
				}
				else
				{
					this._penitent.PenitentAttack.CurrentPenitentWeapon.Attack(this._lungeHit);
				}
			}
			else
			{
				this._penitent.PenitentAttack.CurrentPenitentWeapon.Attack(this._lungeHit);
			}
		}

		private bool IsGrounded()
		{
			bool result;
			if (base.EntityOwner.Status.Orientation == EntityOrientation.Left)
			{
				Vector2 vector = base.transform.position - (0.75f * this._myWidth * base.transform.right + Vector2.up * (this._myHeight * 4f));
				Debug.DrawLine(vector, vector - Vector2.up * 1f, Color.yellow);
				result = (Physics2D.LinecastNonAlloc(vector, vector - Vector2.up * 1f, this._bottomHits, this.GroundLayerMask) > 0);
			}
			else
			{
				Vector2 vector2 = base.transform.position + (0.75f * this._myWidth * base.transform.right - Vector2.up * (this._myHeight * 4f));
				Debug.DrawLine(vector2, vector2 - Vector2.up * 1f, Color.yellow);
				result = (Physics2D.LinecastNonAlloc(vector2, vector2 - Vector2.up * 1f, this._bottomHits, this.GroundLayerMask) > 0);
			}
			return result;
		}

		private void AddLungeForce()
		{
			if (this._stopMovementComplete)
			{
				this._stopMovementComplete = !this._stopMovementComplete;
			}
			if (!this._penitent.Status.IsGrounded || this._penitent.HasFlag("FRONT_BLOCKED"))
			{
				return;
			}
			this._playerController.WalkingDrag = this.LungeMoveSetting.Drag;
			this._playerController.MaxWalkingSpeed = this.GetLungeSpeedByLevel();
			this._playerController.SetActionState((this._penitent.Status.Orientation != EntityOrientation.Left) ? 1 : 2, true);
		}

		private void StopLungeForce()
		{
			if (this._stopMovementComplete)
			{
				return;
			}
			this._stopMovementComplete = true;
			DOTween.To(() => this._playerController.WalkingDrag, delegate(float x)
			{
				this._playerController.WalkingDrag = x;
			}, this._penitent.Dash.DefaultMoveSetting.Drag, 1f);
			DOTween.To(() => this._playerController.MaxWalkingSpeed, delegate(float x)
			{
				this._playerController.MaxWalkingSpeed = x;
			}, this._penitent.Dash.DefaultMoveSetting.Speed, 1f);
		}

		public void PlayLungeAnimByLevelReached()
		{
			if (StringExtensions.IsNullOrWhitespace(base.LastUnlockedSkillId))
			{
				return;
			}
			string lastUnlockedSkillId = base.LastUnlockedSkillId;
			int num;
			if (lastUnlockedSkillId != null)
			{
				if (lastUnlockedSkillId == "LUNGE_1")
				{
					num = this.LungAttackAnim;
					goto IL_83;
				}
				if (lastUnlockedSkillId == "LUNGE_2")
				{
					num = this.LungAttackAnimLv2;
					goto IL_83;
				}
				if (lastUnlockedSkillId == "LUNGE_3")
				{
					num = this.LungAttackAnimLv3;
					goto IL_83;
				}
			}
			num = this.LungAttackAnim;
			IL_83:
			base.EntityOwner.Animator.Play(num);
		}

		private string GetLungeFxKeyByLevel
		{
			get
			{
				if (StringExtensions.IsNullOrWhitespace(base.LastUnlockedSkillId))
				{
					return null;
				}
				string empty = string.Empty;
				string lastUnlockedSkillId = base.LastUnlockedSkillId;
				if (lastUnlockedSkillId != null)
				{
					if (lastUnlockedSkillId == "LUNGE_1")
					{
						return this.LungeMovementFxLevel1;
					}
					if (lastUnlockedSkillId == "LUNGE_2")
					{
						return this.LungeMovementFxLevel2;
					}
					if (lastUnlockedSkillId == "LUNGE_3")
					{
						return this.LungeMovementFxLevel3;
					}
				}
				return this.LungeMovementFxLevel1;
			}
		}

		private void OnDestroy()
		{
			if (!this._attackArea)
			{
				return;
			}
			this._attackArea.OnEnter -= this.AttackAreaOnEnter;
			this._attackArea.OnStay -= this.AttackAreaOnStay;
		}

		public Core.SimpleEvent OnLungeAttackStart;

		private Penitent _penitent;

		public Dash.MoveSetting LungeMoveSetting;

		[BoxGroup("Lunge Movement", true, false, 0)]
		public float LungeLapse = 0.5f;

		[BoxGroup("Lunge Movement", true, false, 0)]
		public float LungeLapse2 = 0.6f;

		[BoxGroup("Lunge Movement", true, false, 0)]
		public float LungeLapse3 = 0.7f;

		public float LungeDrag;

		public float LungeMaxWalkingSpeed;

		public float LungeMaxWalkingSpeed2;

		public float LungeMaxWalkingSpeed3;

		[SerializeField]
		[BoxGroup("Audio Attack", true, false, 0)]
		[EventRef]
		public string LungeEnemyHitFx;

		[SerializeField]
		[BoxGroup("Audio Attack", true, false, 0)]
		[EventRef]
		public string LungeMovementFxLevel1;

		[SerializeField]
		[BoxGroup("Audio Attack", true, false, 0)]
		[EventRef]
		public string LungeMovementFxLevel2;

		[SerializeField]
		[BoxGroup("Audio Attack", true, false, 0)]
		[EventRef]
		public string LungeMovementFxLevel3;

		private EventInstance _lungeMovementFx;

		[SerializeField]
		[BoxGroup("Damage Factor", true, false, 0)]
		public float DamageFactor1;

		[SerializeField]
		[BoxGroup("Damage Factor", true, false, 0)]
		public float DamageFactor2;

		[SerializeField]
		[BoxGroup("Damage Factor", true, false, 0)]
		public float DamageFactor3;

		private PlatformCharacterController _playerController;

		private float _currentLungeLapse;

		private bool _stopMovementComplete;

		private AttackArea _attackArea;

		private Hit _lungeHit;

		private bool _newEnemyHit;

		private List<GameObject> _hitEntities;

		[SerializeField]
		[Header("BoxGroup")]
		public float _myWidth;

		[SerializeField]
		[Header("BoxGroup")]
		public float _myHeight;

		private RaycastHit2D[] _bottomHits;

		public LayerMask GroundLayerMask;

		public readonly int LungAttackAnim = Animator.StringToHash("LungeAttack");

		public readonly int LungAttackAnimLv2 = Animator.StringToHash("LungeAttack_Lv2");

		public readonly int LungAttackAnimLv3 = Animator.StringToHash("LungeAttack_Lv3");
	}
}
