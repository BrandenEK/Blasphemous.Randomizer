using System;
using System.Collections.Generic;
using CreativeSpore.SmartColliders;
using FMODUnity;
using Framework.FrameworkCore;
using Framework.Managers;
using Framework.Util;
using Gameplay.GameControllers.Effects.Player.GhostTrail;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Penitent.InputSystem;
using Rewired;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace Gameplay.GameControllers.Penitent.Abilities
{
	public class VerticalAttack : Ability
	{
		private bool BeamRequiredHeightReached { get; set; }

		protected override void OnStart()
		{
			base.OnStart();
			this._damageableEntities = new List<IDamageable>();
			this._penitent = base.EntityOwner.GetComponent<Penitent>();
			this._rewired = ReInput.players.GetPlayer(0);
			this._defaultAttackAreaOffset = new Vector2(this.VerticalAttackArea.WeaponCollider.offset.x, this.VerticalAttackArea.WeaponCollider.offset.y);
			this._defaultAttackAreaSize = new Vector2(this.VerticalAttackArea.WeaponCollider.bounds.size.x, this.VerticalAttackArea.WeaponCollider.bounds.size.y);
			PoolManager.Instance.CreatePool(this.HardLandingEffectPrefab, 1);
			PoolManager.Instance.CreatePool(this.VerticalAttackBeamEffectPrefab, 1);
			PoolManager.Instance.CreatePool(this.HardLandingEffectUpgradedPrefab, 1);
			this.VerticalAttackArea.OnEnter += this.OnEnterAttackArea;
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			this._canVerticalAttack = (this._penitent.PlatformCharacterController.GroundDist >= this.DistanceThreshold);
			float vspeed = this._penitent.PlatformCharacterController.PlatformCharacterPhysics.VSpeed;
			bool flag = this._penitent.IsClimbingCliffLede || this._penitent.IsGrabbingCliffLede;
			if (!this._penitent.Status.IsGrounded && this._canVerticalAttack && !flag && !this._penitent.IsJumpingOff && vspeed < 0.1f)
			{
				bool flag2 = this._penitent.PlatformCharacterInput.isJoystickDown && this._rewired.GetButtonTimedPress("Attack", this.AttackButtonHoldTime);
				if (flag2 && !base.Casting && base.HasEnoughFervour)
				{
					UnlockableSkill lastUnlockedSkill = base.GetLastUnlockedSkill();
					if (lastUnlockedSkill == null)
					{
						return;
					}
					base.LastUnlockedSkillId = lastUnlockedSkill.id;
					this._ghostTrailTrigered = true;
					this.EnableVerticalAttackCollider(true);
					this.ShowGhostTrail(true);
					base.Animator.SetBool("VERTICAL_ATTACK", true);
					this._initialHorPosition = this._penitent.transform.position.x;
					this.BeamRequiredHeightReached = (this._penitent.PlatformCharacterController.GroundDist >= this.BeamAttackRequiredHeight);
					base.Cast();
				}
			}
			if (base.Animator.GetCurrentAnimatorStateInfo(0).IsName("VerticalAttackStart"))
			{
				this.StopInTheAir();
			}
			else if (base.Animator.GetCurrentAnimatorStateInfo(0).IsName("VerticalAttackFalling"))
			{
				base.Animator.SetBool("VERTICAL_ATTACK", false);
				base.Animator.SetBool("ATTACKING", true);
				this.CharacterController.PlatformCharacterPhysics.Gravity = this._defaultGravity * 5f;
			}
			else if (base.Animator.GetCurrentAnimatorStateInfo(0).IsName("VerticalAttackLanding"))
			{
				this.ShowGhostTrail(false);
				base.Animator.SetBool("ATTACKING", true);
				this._penitent.PlatformCharacterInput.IsAttacking = true;
				if (!this._instantiateExplosion)
				{
					this._instantiateExplosion = true;
					Core.Logic.CameraManager.ProCamera2DShake.ShakeUsingPreset("HardFall");
					this._penitent.Rumble.UsePreset("VerticalAttack");
					this._penitent.AnimatorInyector.ResetStuntByFall();
					this.TriggerVerticalAttack(true);
				}
			}
			else
			{
				if ((!this._ghostTrailTrigered || !this._penitent.Status.IsGrounded) && !base.Animator.GetCurrentAnimatorStateInfo(0).IsName("WallClimbContact"))
				{
					return;
				}
				this._ghostTrailTrigered = false;
				this.ShowGhostTrail(false);
				if (base.Casting)
				{
					base.StopCast();
				}
			}
			if (!base.Casting)
			{
				return;
			}
			Vector3 position = this._penitent.transform.position;
			this._penitent.transform.position = new Vector3(this._initialHorPosition, position.y, position.z);
		}

		protected override void OnCastStart()
		{
			base.OnCastStart();
			this._penitent.Status.Unattacable = true;
			this._penitent.GrabLadder.EnableClimbLadderAbility(false);
			this._instantiateExplosion = false;
		}

		protected override void OnCastEnd(float castingTime)
		{
			base.OnCastEnd(castingTime);
			this._penitent.Status.Unattacable = false;
			base.Animator.SetBool("ATTACKING", false);
			this._penitent.PlatformCharacterInput.IsAttacking = false;
			this._penitent.GrabLadder.EnableClimbLadderAbility(true);
			this.CharacterController.PlatformCharacterPhysics.Gravity = this._defaultGravity;
		}

		private void ShowGhostTrail(bool show = true)
		{
			if (show)
			{
				if (!this.GhostTrailGenerator.EnableGhostTrail)
				{
					this.GhostTrailGenerator.EnableGhostTrail = true;
				}
			}
			else if (this.GhostTrailGenerator.EnableGhostTrail)
			{
				this.GhostTrailGenerator.EnableGhostTrail = false;
			}
		}

		private void StopInTheAir()
		{
			if (this._penitent == null)
			{
				return;
			}
			this._penitent.PlatformCharacterController.PlatformCharacterPhysics.VSpeed = 0f;
			this._penitent.PlatformCharacterController.PlatformCharacterPhysics.HSpeed = 0f;
			this._penitent.PlatformCharacterController.PlatformCharacterPhysics.Acceleration = Vector3.zero;
		}

		private Hit GetHardLandingHit
		{
			get
			{
				return new Hit
				{
					AttackingEntity = base.EntityOwner.gameObject,
					DamageType = DamageArea.DamageType.Heavy,
					DamageAmount = base.EntityOwner.Stats.Strength.Final * this.DamageFactor,
					HitSoundId = this.EnemyImpactSound
				};
			}
		}

		public void TriggerVerticalAttack(bool instantiateExplosion = true)
		{
			this.AttackDamageableEntities(this.GetHardLandingHit);
			this.EnableVerticalAttackCollider(false);
			this.InstantiateExplosion(instantiateExplosion);
		}

		private void InstantiateExplosion(bool instantiate)
		{
			if (!instantiate)
			{
				return;
			}
			if (this.HardLandingEffectPrefab == null || this.VerticalAttackBeamEffectPrefab == null)
			{
				return;
			}
			UnlockableSkill lastUnlockedSkill = base.GetLastUnlockedSkill();
			if (lastUnlockedSkill == null)
			{
				return;
			}
			bool flag = lastUnlockedSkill.id.Equals("VERTICAL_3");
			if (flag && this.BeamRequiredHeightReached)
			{
				PoolManager.Instance.ReuseObject(this.VerticalAttackBeamEffectPrefab, Core.Logic.Penitent.DamageArea.TopCenter, Quaternion.identity, false, 1);
			}
			PoolManager.Instance.ReuseObject((!flag || !this.BeamRequiredHeightReached) ? this.HardLandingEffectPrefab : this.HardLandingEffectUpgradedPrefab, base.EntityOwner.transform.position, Quaternion.identity, false, 1);
		}

		private List<IDamageable> GetDamageableEntities()
		{
			GameObject[] array = this.VerticalAttackArea.OverlappedEntities();
			int num = array.Length;
			byte b = 0;
			while ((int)b < num)
			{
				IDamageable componentInParent = array[(int)b].GetComponentInParent<IDamageable>();
				this._damageableEntities.Add(componentInParent);
				b += 1;
			}
			return this._damageableEntities;
		}

		private void AttackDamageableEntities(Hit hardLandingHit)
		{
			List<IDamageable> damageableEntities = this.GetDamageableEntities();
			int count = damageableEntities.Count;
			if (count <= 0)
			{
				return;
			}
			byte b = 0;
			while ((int)b < count)
			{
				IDamageable damageable = damageableEntities[(int)b];
				if (damageable != null)
				{
					damageable.Damage(hardLandingHit);
				}
				b += 1;
			}
			this._damageableEntities.Clear();
		}

		public void EnableVerticalAttackCollider(bool enable = true)
		{
			if (enable)
			{
				if (!this.VerticalAttackArea.WeaponCollider.enabled)
				{
					this.VerticalAttackArea.WeaponCollider.enabled = true;
				}
				if (!this.VerticalAttackArea.enabled)
				{
					this.VerticalAttackArea.enabled = true;
				}
			}
			else
			{
				if (this.VerticalAttackArea.WeaponCollider.enabled)
				{
					this.VerticalAttackArea.WeaponCollider.enabled = false;
				}
				if (this.VerticalAttackArea.enabled)
				{
					this.VerticalAttackArea.enabled = false;
				}
			}
		}

		public void SetAttackAreaDimensionsBySkill()
		{
			if (this.VerticalAttackArea == null)
			{
				return;
			}
			UnlockableSkill lastUnlockedSkill = base.GetLastUnlockedSkill();
			if (lastUnlockedSkill == null)
			{
				return;
			}
			string id = lastUnlockedSkill.id;
			if (id != null)
			{
				if (id == "VERTICAL_1")
				{
					this.SetDefaultAttackAreaDimensions();
					return;
				}
				if (id == "VERTICAL_2")
				{
					this.VerticalAttackArea.SetOffset(this.AttackAreaDimension[0].AttackAreaOffset);
					this.VerticalAttackArea.SetSize(this.AttackAreaDimension[0].AttackAreaSize);
					return;
				}
				if (id == "VERTICAL_3")
				{
					int num = (!this.BeamRequiredHeightReached) ? 0 : 1;
					this.VerticalAttackArea.SetOffset(this.AttackAreaDimension[num].AttackAreaOffset);
					this.VerticalAttackArea.SetSize(this.AttackAreaDimension[num].AttackAreaSize);
					return;
				}
			}
			this.SetDefaultAttackAreaDimensions();
		}

		public void SetDefaultAttackAreaDimensions()
		{
			if (this.VerticalAttackArea == null)
			{
				return;
			}
			this.VerticalAttackArea.SetOffset(this._defaultAttackAreaOffset);
			this.VerticalAttackArea.SetSize(this._defaultAttackAreaSize);
		}

		private void OnEnterAttackArea(object sender, Collider2DParam e)
		{
			if (!base.Casting || base.EntityOwner.Status.IsGrounded || base.Animator.GetCurrentAnimatorStateInfo(0).IsName("VerticalAttackStart"))
			{
				return;
			}
			this.AttackDamageableEntities(this.GetHardLandingHit);
		}

		public string GetLandingFxEventKey
		{
			get
			{
				if (StringExtensions.IsNullOrWhitespace(base.LastUnlockedSkillId))
				{
					return null;
				}
				string result = string.Empty;
				string lastUnlockedSkillId = base.LastUnlockedSkillId;
				if (lastUnlockedSkillId != null)
				{
					if (!(lastUnlockedSkillId == "VERTICAL_1"))
					{
						if (!(lastUnlockedSkillId == "VERTICAL_2"))
						{
							if (lastUnlockedSkillId == "VERTICAL_3")
							{
								result = this.VerticalLandingFxLevel3;
							}
						}
						else
						{
							result = this.VerticalLandingFxLevel2;
						}
					}
					else
					{
						result = this.VerticalLandingFxLevel1;
					}
				}
				return result;
			}
		}

		private void OnDestroy()
		{
			this.VerticalAttackArea.OnEnter -= this.OnEnterAttackArea;
		}

		public float DamageFactor = 2f;

		public GameObject HardLandingEffectPrefab;

		public GameObject HardLandingEffectUpgradedPrefab;

		public GameObject VerticalAttackBeamEffectPrefab;

		[BoxGroup("Stick Params", true, false, 0)]
		public float BeamAttackRequiredHeight = 5f;

		[Tooltip("Attack Area Dimensions By Skill Upgrade")]
		public VerticalAttack.AttackAreaDimensions[] AttackAreaDimension;

		[SerializeField]
		protected GhostTrailGenerator GhostTrailGenerator;

		private Penitent _penitent;

		private Player _rewired;

		private readonly Vector3 _defaultGravity = new Vector3(0f, -9.8f, 0f);

		[SerializeField]
		[BoxGroup("Trigger AAbility Params", true, false, 0)]
		[Range(0f, 0.5f)]
		private float AttackButtonHoldTime = 0.185f;

		private bool _ghostTrailTrigered;

		private bool _instantiateExplosion;

		private bool _canVerticalAttack;

		private float _initialHorPosition;

		private List<IDamageable> _damageableEntities;

		private Vector2 _defaultAttackAreaSize;

		private Vector2 _defaultAttackAreaOffset;

		public PlatformCharacterController CharacterController;

		public PlatformCharacterInput CharacterInput;

		[FoldoutGroup("Motion", 0)]
		[PropertyTooltip("The decrease factor of the vertical attack first phase")]
		public float DecelerateFactor = 0.5f;

		[FoldoutGroup("Motion", 0)]
		[PropertyTooltip("Max Distance to perform vertical attack")]
		public float RaycastDistance = 3f;

		[FoldoutGroup("Motion", 0)]
		[PropertyTooltip("Minimun distance to perform vertical attack")]
		public float DistanceThreshold = 2f;

		[Tooltip("Floor layers")]
		public LayerMask FloorLayer;

		[SerializeField]
		[FoldoutGroup("Audio Attack", 0)]
		[EventRef]
		public string VerticalLandingFxLevel1;

		[SerializeField]
		[FoldoutGroup("Audio Attack", 0)]
		[EventRef]
		public string VerticalLandingFxLevel2;

		[SerializeField]
		[FoldoutGroup("Audio Attack", 0)]
		[EventRef]
		public string VerticalLandingFxLevel3;

		[SerializeField]
		[FoldoutGroup("Audio Attack", 0)]
		[EventRef]
		public string EnemyImpactSound;

		public AttackArea VerticalAttackArea;

		[Serializable]
		public struct AttackAreaDimensions
		{
			public AttackAreaDimensions(Vector2 attackAreaSize, Vector2 attackAreaOffset)
			{
				this.AttackAreaOffset = attackAreaOffset;
				this.AttackAreaSize = attackAreaSize;
			}

			[SerializeField]
			public Vector2 AttackAreaOffset;

			[SerializeField]
			public Vector2 AttackAreaSize;
		}
	}
}
