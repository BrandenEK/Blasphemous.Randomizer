using System;
using System.Collections;
using CreativeSpore.SmartColliders;
using DG.Tweening;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Penitent.InputSystem;
using Gameplay.UI;
using Rewired;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Penitent.Abilities
{
	public class WallJump : Trait
	{
		public Player Rewired { get; private set; }

		protected override void OnStart()
		{
			base.OnStart();
			this.Rewired = ReInput.players.GetPlayer(0);
			this.DisabledAbilityWhenUse = base.EntityOwner.GetComponentInChildren<GrabCliffLede>();
			base.EntityOwner.OnDamaged += this.EntityOwnerOnDamaged;
			CameraShakeManager.OnCameraShakeOverthrow = (Core.SimpleEvent)Delegate.Combine(CameraShakeManager.OnCameraShakeOverthrow, new Core.SimpleEvent(this.OnCameraShakeOverthrow));
			this._defaultRayCastDistance = this.Distance;
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			if (base.EntityOwner.Status.IsGrounded)
			{
				this.ResetWallJumpStatus();
				if (this.Distance <= 0f)
				{
					this.Distance = this._defaultRayCastDistance;
				}
			}
			Vector3 v = new Vector3(base.transform.position.x, base.transform.position.y + this.HookHeightFromPivotPoint, base.transform.position.z);
			float d = (base.EntityOwner.Status.Orientation != EntityOrientation.Right) ? -1f : 1f;
			this._wallHit = Physics2D.Raycast(v, Vector2.right * d, this.Distance, this.WallLayerMask);
			if (this.Rewired.GetButton(5) && !this.CharacterController.IsGrounded && this._wallHit.collider != null && !this._stickToWall && this.EndStickCoolDown)
			{
				Core.Logic.Penitent.Audio.SetParametersValuesByWall(this._wallHit.collider);
				this._stickToWall = true;
				this.playerStickedOrientation = Core.Logic.Penitent.Status.Orientation;
				base.EntityOwner.Animator.ResetTrigger("AIR_ATTACK");
				base.EntityOwner.Animator.Play(this._wallClimbContactAnim);
				base.EntityOwner.transform.position = this.GetClimbPosition(this._wallHit.collider);
				Core.Input.SetBlocker("PLAYER_LOGIC", true);
				base.EntityOwner.transform.DOMoveY(base.EntityOwner.transform.position.y - this.GravityDragDistance, this.GravityDragLapse, false).SetEase(Ease.OutSine).OnUpdate(new TweenCallback(this.CheckWallCollider));
			}
			if (this._stickToWall)
			{
				this.Stick();
			}
			if (this.Rewired.GetButtonDown(6) && this._stickToWall && this.EndJumpOffCoolDown && base.EntityOwner.Animator.GetBool("STICK_ON_WALL") && !UIController.instance.IsShowingMenu)
			{
				DOTween.Kill(base.EntityOwner.transform, false);
				this.ToogleAbilities(true);
				this.Detach();
			}
			if (this._jumpOffWall)
			{
				this.JumpOff();
			}
			else
			{
				this._wallJumpTimer = this.CharacterController.JumpingAccTime;
			}
		}

		private void OnDestroy()
		{
			base.EntityOwner.OnDamaged -= this.EntityOwnerOnDamaged;
			CameraShakeManager.OnCameraShakeOverthrow = (Core.SimpleEvent)Delegate.Remove(CameraShakeManager.OnCameraShakeOverthrow, new Core.SimpleEvent(this.OnCameraShakeOverthrow));
		}

		private void OnDrawGizmos()
		{
			if (!this.debugOn)
			{
				return;
			}
			Gizmos.color = Color.red;
			Vector3 vector = new Vector3(base.transform.position.x, base.transform.position.y + this.HookHeightFromPivotPoint, base.transform.position.z);
			float d = (base.EntityOwner.Status.Orientation != EntityOrientation.Right) ? -1f : 1f;
			Gizmos.DrawLine(vector, vector + Vector3.right * d * this.Distance);
		}

		private void EntityOwnerOnDamaged()
		{
			this.UnhangByEvent();
			base.StartCoroutine(this.RestoreRayCastAfterDamageDelay());
		}

		private IEnumerator RestoreRayCastAfterDamageDelay()
		{
			if (this.Distance > 0f)
			{
				yield break;
			}
			yield return new WaitForSeconds(this.RestoreWallJumpAfterDamageLapse);
			this.Distance = this._defaultRayCastDistance;
			yield break;
		}

		private void OnCameraShakeOverthrow()
		{
			if (!this._stickToWall)
			{
				return;
			}
			this.UnhangByEvent();
			base.StartCoroutine(this.ShakeOverthrowPenalty());
		}

		private IEnumerator ShakeOverthrowPenalty()
		{
			float penaltyTime = 0.35f;
			while (penaltyTime > 0f)
			{
				penaltyTime -= Time.deltaTime;
				this._stickToWall = false;
				this._stickCoolDownTimer = this.StickCoolDown;
				base.EntityOwner.Animator.SetBool("STICK_ON_WALL", this._stickToWall);
				this.CharacterController.PlatformCharacterPhysics.Gravity = new Vector3(0f, -9.8f, 0f);
				yield return null;
			}
			this._stickCoolDownTimer = -1f;
			if (this.Distance <= 0f)
			{
				this.Distance = this._defaultRayCastDistance;
			}
			yield break;
		}

		private void UnhangByEvent()
		{
			this.ResetWallJumpStatus();
			this._stickToWall = false;
			this.Distance = 0f;
			this._stickCoolDownTimer = -1f;
			base.EntityOwner.Animator.SetBool("STICK_ON_WALL", false);
			base.EntityOwner.Animator.ResetTrigger("WALLCLIMB_UNHANG");
			Core.Input.SetBlocker("PLAYER_LOGIC", false);
		}

		private void CheckCancelHook()
		{
			if (UIController.instance.IsShowingMenu)
			{
				return;
			}
			if (!this.Rewired.GetButton(65))
			{
				return;
			}
			if (this._unHang)
			{
				return;
			}
			this._unHang = true;
			base.EntityOwner.Animator.SetTrigger("WALLCLIMB_UNHANG");
			base.StartCoroutine(this.UnHang());
		}

		private void ResetWallJumpStatus()
		{
			this._jumpOffWall = false;
			this._unHang = false;
			this._isJumpOffStacked = false;
			this._stickToWall = false;
			this._stickCoolDownTimer = -1f;
			this.CharacterController.PlatformCharacterPhysics.Gravity = new Vector3(0f, -9.8f, 0f);
		}

		private bool EndStickCoolDown
		{
			get
			{
				return this._stickCoolDownTimer < 0f;
			}
		}

		private bool EndJumpOffCoolDown
		{
			get
			{
				return this._jumpOffCoolDownTimer < 0f;
			}
		}

		private void Stick()
		{
			this._jumpOffWall = false;
			this.ToogleAbilities(false);
			this._jumpOffCoolDownTimer -= Time.deltaTime;
			this._stickCoolDownTimer = this.StickCoolDown;
			this._isJumpOffStacked = false;
			this.CharacterController.PlatformCharacterPhysics.Velocity = Vector3.zero;
			this.CharacterController.PlatformCharacterPhysics.VSpeed = 0f;
			this.CharacterController.PlatformCharacterPhysics.Gravity = Vector3.zero;
			this.CharacterController.PlatformCharacterPhysics.Acceleration = Vector3.zero;
			Core.Input.SetBlocker("PLAYER_LOGIC", true);
			Core.Logic.Penitent.SetOrientation(this.playerStickedOrientation, true, false);
			if (this.CanCancelHook)
			{
				this.CheckCancelHook();
			}
		}

		private bool CanCancelHook
		{
			get
			{
				bool flag = base.EntityOwner.Animator.GetCurrentAnimatorStateInfo(0).IsName("WallClimbIdle");
				bool flag2 = base.EntityOwner.Animator.GetCurrentAnimatorStateInfo(0).IsName("WallClimbContact");
				float num = 0f;
				if (flag2)
				{
					num = base.EntityOwner.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
				}
				return flag || (flag2 && num > 0.1f);
			}
		}

		private void Detach()
		{
			this._stickToWall = false;
			this._jumpOffWall = true;
			this.CharacterController.PlatformCharacterPhysics.Gravity = new Vector3(0f, -9.8f, 0f);
			Core.Input.SetBlocker("PLAYER_LOGIC", false);
			this.CharacterController.PlatformCharacterPhysics.Velocity = new Vector2(this.WallJumpSpeed * this.CharacterInput.FHorAxis, this.WallJumpSpeed);
			base.EntityOwner.Animator.SetBool("STICK_ON_WALL", false);
			if (this.DisabledAbilityWhenUse != null)
			{
				base.StartCoroutine(this.DisableAbility());
			}
		}

		private void JumpOff()
		{
			this._jumpOffCoolDownTimer = this.JumpOffCoolDown;
			this._stickCoolDownTimer -= Time.deltaTime;
			if (!this._isJumpOffStacked)
			{
				this._isJumpOffStacked = true;
				float x = (this.playerStickedOrientation != EntityOrientation.Right) ? 1f : -1f;
				this.CharacterInput.Move(x, 0.1f);
				base.EntityOwner.Animator.Play(this._jumpForwardAnim);
				base.EntityOwner.SetOrientation(this.GetReverseOrientation(base.EntityOwner.Status.Orientation), true, false);
			}
			if (this._wallJumpTimer <= 0f)
			{
				return;
			}
			this._wallJumpTimer -= Time.deltaTime;
			this.CharacterController.PlatformCharacterPhysics.Acceleration += base.transform.up * this.WallJumpAcc;
		}

		private EntityOrientation GetReverseOrientation(EntityOrientation currentOrientation)
		{
			return (currentOrientation != EntityOrientation.Left) ? EntityOrientation.Left : EntityOrientation.Right;
		}

		private IEnumerator UnHang()
		{
			yield return new WaitForSeconds(0.35f);
			this._stickToWall = false;
			this.ToogleAbilities(true);
			if (!this._jumpOffWall)
			{
				this._jumpOffWall = true;
			}
			Core.Input.SetBlocker("PLAYER_LOGIC", false);
			this.CharacterController.PlatformCharacterPhysics.Gravity = new Vector3(0f, -9.8f, 0f);
			base.EntityOwner.Animator.SetBool("STICK_ON_WALL", false);
			if (this._unHang)
			{
				this._unHang = !this._unHang;
			}
			yield break;
		}

		private Vector3 GetClimbPosition(Collider2D climbCollider)
		{
			float x = (base.EntityOwner.Status.Orientation != EntityOrientation.Right) ? (climbCollider.bounds.max.x + this.StickDistanceToWall) : (climbCollider.bounds.min.x - this.StickDistanceToWall);
			Vector2 v = new Vector2(x, base.EntityOwner.transform.position.y);
			return v;
		}

		private IEnumerator DisableAbility()
		{
			this.DisabledAbilityWhenUse.enabled = false;
			yield return new WaitForSeconds(0.1f);
			this.DisabledAbilityWhenUse.enabled = true;
			yield break;
		}

		private void CheckWallCollider()
		{
			if (this._wallHit.collider == null)
			{
				DOTween.Kill(base.EntityOwner.transform, false);
			}
		}

		public void ToogleAbilities(bool t)
		{
			for (int i = 0; i < this.ToogledAbilities.Length; i++)
			{
				this.ToogledAbilities[i].enabled = t;
			}
		}

		private readonly int _jumpForwardAnim = Animator.StringToHash("Jump Forward");

		private readonly int _wallClimbContactAnim = Animator.StringToHash("WallClimbContact");

		public PlatformCharacterController CharacterController;

		public PlatformCharacterInput CharacterInput;

		public Ability[] ToogledAbilities;

		private bool _jumpOffWall;

		private GrabCliffLede DisabledAbilityWhenUse;

		private bool _stickToWall;

		private RaycastHit2D _wallHit;

		[BoxGroup("Stick Params", true, false, 0)]
		[SerializeField]
		public LayerMask WallLayerMask;

		[BoxGroup("Stick Params", true, false, 0)]
		[SerializeField]
		public float HookHeightFromPivotPoint = 1f;

		[BoxGroup("Stick Params", true, false, 0)]
		[SerializeField]
		public float Distance = 1f;

		[BoxGroup("Stick Params", true, false, 0)]
		[SerializeField]
		public float StickCoolDown = 1f;

		[BoxGroup("Stick Params", true, false, 0)]
		[SerializeField]
		public float JumpOffCoolDown = 1f;

		[BoxGroup("Stick Params", true, false, 0)]
		[SerializeField]
		public float GravityDragDistance = 0.25f;

		[BoxGroup("Stick Params", true, false, 0)]
		[SerializeField]
		public float GravityDragLapse = 1f;

		[BoxGroup("Jump Params", true, false, 0)]
		[SerializeField]
		public float StickDistanceToWall = 2f;

		[BoxGroup("Jump Params", true, false, 0)]
		[SerializeField]
		public float WallJumpAcc = 2f;

		[BoxGroup("Jump Params", true, false, 0)]
		[SerializeField]
		public float WallJumpSpeed = 2f;

		[BoxGroup("Jump Params", true, false, 0)]
		[SerializeField]
		public float RestoreWallJumpAfterDamageLapse = 0.4f;

		[BoxGroup("Debug", true, false, 0)]
		[SerializeField]
		public bool debugOn;

		private float _wallJumpTimer = -1f;

		private float _stickCoolDownTimer = -1f;

		private float _jumpOffCoolDownTimer = -1f;

		private float _defaultRayCastDistance;

		private EntityOrientation playerStickedOrientation;

		private bool _unHang;

		private bool _isJumpOffStacked;
	}
}
