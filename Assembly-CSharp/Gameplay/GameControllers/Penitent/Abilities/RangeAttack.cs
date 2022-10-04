using System;
using FMOD.Studio;
using FMODUnity;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Penitent.Gizmos;
using Rewired;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Penitent.Abilities
{
	public class RangeAttack : Ability
	{
		[FoldoutGroup("Projectile Settings", true, 0)]
		public bool ProjectileIsRunning { get; set; }

		[FoldoutGroup("Projectile Settings", true, 0)]
		public bool RequestProjectile { get; set; }

		protected override void OnStart()
		{
			base.OnStart();
			this._rootMotion = Core.Logic.Penitent.GetComponentInChildren<RootMotionDriver>();
			PoolManager.Instance.CreatePool(this.RangeAttackProjectile, 1);
			PoolManager.Instance.CreatePool(this.RangeExplosion, 1);
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			this._rewired = Core.Logic.Penitent.PlatformCharacterInput.Rewired;
			if (this._rewired == null)
			{
				return;
			}
			this.currentTimeThreshold += Time.deltaTime;
			Penitent penitent = Core.Logic.Penitent;
			if (this.RangeAttackCancelledByAbility(penitent))
			{
				return;
			}
			if (this._rewired.GetButtonDown(57) && !this._pressedKeyDown)
			{
				this._pressedKeyDown = true;
			}
			bool buttonUp = this._rewired.GetButtonUp(57);
			if (Core.Input.InputBlocked)
			{
				this._pressedKeyDown = false;
				return;
			}
			if (!buttonUp || !this._pressedKeyDown)
			{
				return;
			}
			UnlockableSkill lastUnlockedSkill = base.GetLastUnlockedSkill();
			if (lastUnlockedSkill == null || base.EntityOwner.Status.Dead)
			{
				return;
			}
			base.LastUnlockedSkillId = lastUnlockedSkill.id;
			if (base.Casting || this.currentTimeThreshold < this.abilityTimeThreshold || !base.HasEnoughFervour)
			{
				return;
			}
			this.CastRangeAttack();
		}

		private void CastRangeAttack()
		{
			if (base.EntityOwner.Status.IsGrounded)
			{
				base.Cast();
				this._pressedKeyDown = false;
				base.EntityOwner.Animator.Play(RangeAttack.GroundRangeAttackAnim);
			}
			else if (Core.Logic.Penitent.PlatformCharacterController.GroundDist >= 1f)
			{
				base.Cast();
				this._pressedKeyDown = false;
				base.EntityOwner.Animator.Play(RangeAttack.MidAirRangeAttackAnim);
			}
		}

		private bool RangeAttackCancelledByAbility(Penitent player)
		{
			if (this.ProjectileIsRunning || player.PlatformCharacterController.IsClimbing || player.IsClimbingCliffLede || player.IsGrabbingCliffLede || player.PlatformCharacterInput.IsAttacking)
			{
				this.currentTimeThreshold = 0f;
				return true;
			}
			return false;
		}

		protected override void OnCastStart()
		{
			base.OnCastStart();
			Core.Logic.Penitent.Dash.StopCast();
			Core.Audio.EventOneShotPanned(this.FireRangeAttackFx, base.transform.position, out this._rangeAttackFxInstance);
		}

		protected override void OnCastEnd(float castingTime)
		{
			base.OnCastEnd(castingTime);
			if (this._rangeAttackFxInstance.isValid())
			{
				this._rangeAttackFxInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
				this._rangeAttackFxInstance.release();
			}
		}

		public void StopCastRangeAttack()
		{
			base.StopCast();
		}

		public Vector3 GetReverseFirePosition()
		{
			Vector3 position = this._rootMotion.transform.position;
			Vector3 localPosition = this._rootMotion.transform.localPosition;
			float x = position.x - localPosition.x * 2f;
			return new Vector3(x, position.y, 0f);
		}

		public void InstanceProjectile()
		{
			if (this.RangeAttackProjectile == null)
			{
				return;
			}
			Vector3 position = (base.EntityOwner.Status.Orientation != EntityOrientation.Right) ? this.GetReverseFirePosition() : this._rootMotion.transform.position;
			position.y = Core.Logic.Penitent.DamageArea.Center().y + 0.2f;
			PoolManager.Instance.ReuseObject(this.RangeAttackProjectile, position, Quaternion.identity, false, 1);
		}

		public void InstantiateExplosion(Vector3 position)
		{
			PoolManager.Instance.ReuseObject(this.RangeExplosion, position, Quaternion.identity, false, 1);
		}

		public static readonly int GroundRangeAttackAnim = Animator.StringToHash("GroundRangeAttack");

		public static readonly int MidAirRangeAttackAnim = Animator.StringToHash("MidAirRangeAttack");

		private Player _rewired;

		private RootMotionDriver _rootMotion;

		private const float MinHeightActivation = 1f;

		[FoldoutGroup("Projectile Settings", true, 0)]
		public GameObject RangeAttackProjectile;

		[FoldoutGroup("Projectile Settings", true, 0)]
		public GameObject RangeExplosion;

		[SerializeField]
		[BoxGroup("Audio Attack", true, false, 0)]
		[EventRef]
		public string FireRangeAttackFx;

		private EventInstance _rangeAttackFxInstance;

		private bool _pressedKeyDown;

		private float abilityTimeThreshold = 0.2f;

		private float currentTimeThreshold;
	}
}
