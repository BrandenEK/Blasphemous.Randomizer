using System;
using System.Collections;
using CreativeSpore.SmartColliders;
using DG.Tweening;
using DG.Tweening.Core.Surrogates;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Penitent.Gizmos;
using Gameplay.GameControllers.Penitent.Sensor;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Penitent.Abilities
{
	public class GrabLadder : Trait
	{
		public bool IsBottomLadderRepositioning { get; set; }

		public bool IsTopLadderReposition { get; set; }

		public bool StartGoingDown { get; set; }

		public Collider2D CurrentLadderCollider { get; private set; }

		public Vector3 LadderTopPoint { get; private set; }

		protected override void OnStart()
		{
			base.OnStart();
			this._penitent = (Penitent)base.EntityOwner;
			this._penitent.OnDamaged += this.PenitentOnDamaged;
			this._playerController = this._penitent.GetComponent<PlatformCharacterController>();
			this._rootMotionDriver = this._penitent.GetComponentInChildren<RootMotionDriver>();
			this._layerMaskLadder = this._penitent.PlatformCharacterController.ClimbingLayers;
			this._penitent.PlatformCharacterController.ClimbingSpeed = 0f;
			this._climbingLadderLayermask = this._playerController.ClimbingLayers;
			this._ladderLayer = LayerMask.NameToLayer("Ladder");
			this.IsBottomLadderRepositioning = false;
			this._enableClimbLadderAbility = true;
			this._penitent.CanClimbLadder = true;
			FloorDistanceChecker.OnStepLadder = (Core.GenericEvent)Delegate.Combine(FloorDistanceChecker.OnStepLadder, new Core.GenericEvent(this.OnStepLadder));
			this._penitent.OnDeath += this.PenitentOnDeath;
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			if (this.IsBottomLadderRepositioning)
			{
				this.IsBottomLadderRepositioning = false;
			}
			this.StartGoingDown = (this._penitent.StepOnLadder && this._penitent.PlatformCharacterInput.isJoystickDown && !this._penitent.PlatformCharacterController.IsClimbing && this._penitent.Status.IsGrounded);
			bool flag = false;
			if (this.CurrentLadderCollider != null)
			{
				float num = this.DistanceToTopLadder(this._penitent.transform.position);
				flag = (num < this.CurrentLadderCollider.bounds.size.x * this.ladderWidthFactor);
			}
			if (this.StartGoingDown && !this.IsTopLadderReposition)
			{
				this.IsTopLadderReposition = true;
				this.TopLadderReposition();
			}
			bool value = this._penitent.StepOnLadder && flag && this._penitent.CanClimbLadder;
			base.EntityOwner.Animator.SetBool(GrabLadder.StepOnLadderHash, value);
			base.EntityOwner.Animator.SetBool(GrabLadder.IsCollidingLadderHash, this._penitent.IsOnLadder);
			if (!this._penitent.StepOnLadder)
			{
				this.IsTopLadderReposition = false;
			}
			if (this._penitent.PlatformCharacterInput.Rewired.GetButtonDown(65) && !this.IsTakingOffLadder && !Core.Input.InputBlocked)
			{
				this.TakeOffLadder();
			}
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			if ((this._layerMaskLadder.value & 1 << collision.gameObject.layer) <= 0)
			{
				return;
			}
			this._penitent.IsJumpingOff = false;
			this.LadderTopPoint = new Vector3(collision.bounds.center.x, collision.bounds.max.y, 0f);
			this.CurrentLadderCollider = collision;
		}

		private void OnTriggerStay2D(Collider2D collision)
		{
			if ((this._layerMaskLadder.value & 1 << collision.gameObject.layer) <= 0 || !base.enabled)
			{
				return;
			}
			this._penitent.IsOnLadder = true;
			this._penitent.RootMotionDrive = this._rootMotionDriver.transform.position;
			if (this._penitent.RootMotionDrive.y >= this.LadderTopPoint.y)
			{
				if (collision.CompareTag("HasTop"))
				{
					this.SetClimbingSpeed(0f);
				}
				else
				{
					this._penitent.ReachTopLadder = true;
				}
			}
			else
			{
				this._penitent.ReachTopLadder = false;
			}
			if (this._penitent.Status.IsGrounded && !this._penitent.IsClimbingLadder && this._penitent.PlatformCharacterInput.isJoystickUp && !this.IsBottomLadderRepositioning)
			{
				this.IsBottomLadderRepositioning = true;
			}
			this._penitent.ReachBottonLadder = (this.PlayerBottomPointCollider.y < GrabLadder.LadderBottomPointCollider(collision).y);
		}

		private void OnTriggerExit2D(Collider2D collision)
		{
			if ((this._layerMaskLadder.value & 1 << collision.gameObject.layer) <= 0)
			{
				return;
			}
			this._penitent.IsOnLadder = false;
			this._penitent.IsGrabbingLadder = false;
			this._penitent.CanJumpFromLadder = true;
			if (this._penitent.ReachBottonLadder)
			{
				this._penitent.ReachBottonLadder = !this._penitent.ReachBottonLadder;
			}
		}

		public float DistanceToTopLadder(Vector3 playerPos)
		{
			Vector2 b = new Vector2(this.CurrentLadderCollider.bounds.center.x, playerPos.y);
			return Vector2.Distance(playerPos, b);
		}

		public void TopLadderReposition()
		{
			float x = this.CurrentLadderCollider.bounds.center.x;
			float y = this._penitent.transform.position.y;
			Vector3 position = new Vector3(x, y, this._penitent.transform.position.z);
			this._penitent.transform.position = position;
		}

		public void GrabLadderPlayerReposition()
		{
			Vector3 ladderCenteredPosition = this.GetLadderCenteredPosition(this.CurrentLadderCollider);
			DOTween.To(() => this._penitent.transform.position, delegate(Vector3Wrapper x)
			{
				this._penitent.transform.position = x;
			}, ladderCenteredPosition, 0f);
		}

		public float GetClimbingSpeed()
		{
			float result = -1f;
			if (this._penitent != null)
			{
				result = this._penitent.PlatformCharacterController.ClimbingSpeed;
			}
			return result;
		}

		public void SetClimbingSpeed(float climbingSpeed)
		{
			if (this._penitent != null)
			{
				this._penitent.PlatformCharacterController.ClimbingSpeed = climbingSpeed;
			}
		}

		public void EnableClimbLadderAbility(bool enable = true)
		{
			if (enable && !this._enableClimbLadderAbility)
			{
				this._enableClimbLadderAbility = true;
				if (!this._penitent.CanClimbLadder)
				{
					this._penitent.CanClimbLadder = true;
				}
				this._climbingLadderLayermask |= 1 << this._ladderLayer;
				this._playerController.ClimbingLayers = this._climbingLadderLayermask;
			}
			else if (!enable && this._enableClimbLadderAbility)
			{
				this._enableClimbLadderAbility = false;
				if (this._penitent.CanClimbLadder)
				{
					this._penitent.CanClimbLadder = !this._penitent.CanClimbLadder;
				}
				this._climbingLadderLayermask ^= 1 << this._ladderLayer;
				this._playerController.ClimbingLayers = this._climbingLadderLayermask;
			}
		}

		private Vector3 GetLadderCenteredPosition(Collider2D ladderCollider)
		{
			Vector3 position = this._penitent.transform.position;
			Vector3 result = new Vector3(ladderCollider.bounds.center.x, position.y);
			return result;
		}

		private Vector2 PlayerBottomPointCollider
		{
			get
			{
				float x = this._penitent.DamageArea.DamageAreaCollider.bounds.center.x;
				float y = this._penitent.DamageArea.DamageAreaCollider.bounds.min.y;
				return new Vector2(x, y);
			}
		}

		private static Vector2 LadderBottomPointCollider(Collider2D col)
		{
			float x = col.bounds.center.x;
			float y = col.bounds.min.y;
			return new Vector2(x, y);
		}

		private void OnStepLadder(UnityEngine.Object param)
		{
			this.CurrentLadderCollider = (param as Collider2D);
		}

		private void PenitentOnDeath()
		{
			this.EnableClimbLadderAbility(false);
		}

		private void PenitentOnDamaged()
		{
			IEnumerator routine = this.DisabledClimbAbilityLapse(this._penitent.Animator.GetCurrentAnimatorStateInfo(0).length);
			base.StartCoroutine(routine);
		}

		private IEnumerator DisabledClimbAbilityLapse(float lapse)
		{
			this.EnableClimbLadderAbility(false);
			yield return new WaitForSeconds(lapse);
			if (!this._penitent.Status.Dead)
			{
				this.EnableClimbLadderAbility(true);
			}
			yield break;
		}

		private void TakeOffLadder()
		{
			if (!this._penitent.PlatformCharacterController.IsClimbing || this.TakeOffLadderCoolDown <= 0f)
			{
				return;
			}
			this._penitent.PlatformCharacterController.StopClimbing();
			base.StartCoroutine(this.DisabledClimbAbilityLapse(this.TakeOffLadderCoolDown));
		}

		private bool IsTakingOffLadder
		{
			get
			{
				return false || base.EntityOwner.Animator.GetCurrentAnimatorStateInfo(0).IsName("grab_ladder_to_go_down") || base.EntityOwner.Animator.GetCurrentAnimatorStateInfo(0).IsName("release_ladder_to_floor_up");
			}
		}

		private void OnEnable()
		{
			if (this._penitent != null)
			{
				this.EnableClimbLadderAbility(true);
			}
		}

		private void OnDisable()
		{
			if (this._penitent != null)
			{
				this.EnableClimbLadderAbility(false);
			}
		}

		private void OnDestroy()
		{
			FloorDistanceChecker.OnStepLadder = (Core.GenericEvent)Delegate.Remove(FloorDistanceChecker.OnStepLadder, new Core.GenericEvent(this.OnStepLadder));
			if (this._penitent != null)
			{
				this._penitent.OnDeath -= this.PenitentOnDeath;
			}
		}

		private Penitent _penitent;

		private RootMotionDriver _rootMotionDriver;

		private LayerMask _layerMaskLadder;

		private float _maxClimbingSpeed;

		private PlatformCharacterController _playerController;

		[Tooltip("Remain time to start climbing when penitent puts the joystick up")]
		[Range(0f, 0.5f)]
		[FoldoutGroup("Climb Settings", 0)]
		public float timeToStartClimbing = 0.2f;

		public const float MAX_CLIMB_SPEED = 2.25f;

		private LayerMask _climbingLadderLayermask;

		private int _ladderLayer;

		private bool _enableClimbLadderAbility;

		public float LadderRepositionLapse = 0.5f;

		[SerializeField]
		[Range(0f, 1f)]
		[FoldoutGroup("Climb Settings", 0)]
		private float TakeOffLadderCoolDown = 0.5f;

		private Vector2 _ladderBottomPoint;

		[SerializeField]
		[Tooltip("Consider the player repositioned when closer than ladderWidth * ladderWidthFactor")]
		[Range(0f, 1f)]
		private float ladderWidthFactor = 0.2f;

		private static readonly int IsCollidingLadderHash = Animator.StringToHash("IS_COLLIDING_LADDER");

		private static readonly int StepOnLadderHash = Animator.StringToHash("STEP_ON_LADDER");
	}
}
