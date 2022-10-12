using System;
using System.Collections;
using CreativeSpore.SmartColliders;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Effects.Player.Dust;
using Gameplay.GameControllers.Effects.Player.GhostTrail;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.Animations;
using Gameplay.GameControllers.Penitent.Audio;
using UnityEngine;

namespace Gameplay.GameControllers.Penitent.Animator
{
	public class PenitentMoveAnimations : EntityAnimationEvents
	{
		private void Awake()
		{
			this._penitent = base.GetComponentInParent<Penitent>();
		}

		private void Start()
		{
			this._stepDustSpawner = this._penitent.GetComponentInChildren<StepDustSpawner>();
			this._pushBackDust = this._penitent.GetComponentInChildren<PushBackDust>();
			this._pushBackSpark = this._penitent.GetComponentInChildren<PushBackSpark>();
			this._throwbackDust = this._penitent.GetComponentInChildren<ThrowbackDust>();
			this._ghostTrailGenerator = this._penitent.GetComponentInChildren<GhostTrailGenerator>();
			this._penitentAudio = this._penitent.Audio;
		}

		public void SetJumpOff(int jumpOff)
		{
			this._penitent.IsJumpingOff = (jumpOff > 0);
		}

		public void GetStepDust()
		{
			if (this._stepDustSpawner == null)
			{
				return;
			}
			StepDustRoot stepDustRoot = this._stepDustSpawner.StepDustRoot;
			Vector2 stepDustPosition = stepDustRoot.transform.position;
			Vector2 vector = stepDustRoot.transform.localPosition;
			stepDustPosition.x = ((this._penitent.Status.Orientation != EntityOrientation.Left) ? stepDustPosition.x : (stepDustPosition.x - vector.x * 2f));
			if (this._penitent.PlatformCharacterController.GroundDist < 0.5f)
			{
				this._stepDustSpawner.GetStepDust(stepDustPosition);
			}
		}

		public void GetPushBackDust()
		{
			if (this._pushBackDust != null)
			{
				this._pushBackDust.TriggerPushBackDust();
			}
		}

		public void GetThrowbackDust()
		{
			if (this._throwbackDust != null)
			{
				this._throwbackDust.TriggerThrowbackDust();
			}
		}

		public void GetPushBackSparks()
		{
			if (this._pushBackSpark && !this._penitent.Dash.StopByDamage)
			{
				this._pushBackSpark.TriggerPushBackSparks();
			}
		}

		public void DisablePhysics()
		{
			this._penitent.Physics.Enable2DCollision(false);
		}

		public void FreezeEntity(DamageArea.DamageType damageType)
		{
			float num = this.freezeTime;
			if (damageType != DamageArea.DamageType.Normal)
			{
				if (damageType == DamageArea.DamageType.Heavy)
				{
					num = this.freezeTime + this.freezeTime * this.freezeTimeFactorPercentage;
				}
			}
			base.StartCoroutine(this._penitent.FreezeAnimator(num));
		}

		public override void Rebound()
		{
			float num = -2.5f;
			if (this._penitent.Status.Orientation == EntityOrientation.Left)
			{
				num *= -1f;
			}
			this._penitent.PlayerHitMotionLerper.distanceToMove = num;
			this._penitent.PlayerHitMotionLerper.TimeTakenDuringLerp = 0.2f;
			Vector3 forwardTangent = this._penitent.GetForwardTangent(base.transform.right, this._penitent.FloorChecker.BottonNormalCollision);
			this._penitent.PlayerHitMotionLerper.StartLerping(forwardTangent.normalized);
		}

		public void SetDashInvulnerable()
		{
			if (!this._penitent.Status.Unattacable)
			{
				this._penitent.Status.Unattacable = true;
			}
		}

		public void SetDashVulnerable()
		{
			if (this._penitent.Status.Unattacable)
			{
				this._penitent.Status.Unattacable = false;
			}
		}

		public void ComboFinisherJump()
		{
			base.StartCoroutine(this.JumpCoroutine());
		}

		private IEnumerator JumpCoroutine()
		{
			float js = this._penitent.PlatformCharacterController.JumpingSpeed;
			float jat = this._penitent.PlatformCharacterController.JumpingAccTime;
			this._penitent.PlatformCharacterController.JumpingSpeed *= 1.6f;
			this._penitent.PlatformCharacterController.JumpingAccTime *= 1f;
			this._penitent.PlatformCharacterController.SetActionState(eControllerActions.Jump, true);
			yield return new WaitForSeconds(1f);
			this._penitent.PlatformCharacterController.JumpingSpeed = js;
			this._penitent.PlatformCharacterController.JumpingAccTime = jat;
			this._penitent.PlatformCharacterController.SetActionState(eControllerActions.Jump, false);
			yield break;
		}

		public void ResizeFallingDamageArea(int resize)
		{
			if (!this._penitent)
			{
				return;
			}
			this._penitent.DamageArea.IsFallingForwardResized = (resize > 0);
		}

		public void RaiseStopDust()
		{
			if (this._penitent == null)
			{
				return;
			}
			this._penitent.DashDustGenerator.GetStopDashDust();
		}

		public void EnabledGhostTrail(AttackAnimationsEvents.Activation activation)
		{
			if (this._ghostTrailGenerator == null)
			{
				return;
			}
			this._ghostTrailGenerator.EnableGhostTrail = (activation == AttackAnimationsEvents.Activation.True);
		}

		public void PlayFootStep()
		{
			this._penitentAudio.PlayFootStep();
			if (this.OnStep != null)
			{
				this.OnStep(this._penitent.GetPosition());
			}
		}

		public void PlayRunStop()
		{
			this._penitentAudio.PlayRunStopSound();
		}

		public void PlayLanding()
		{
			this._penitentAudio.PlayLandingSound();
		}

		public void PlayLandingRunning()
		{
			this._penitentAudio.PlayLandingForward();
		}

		public void PlayJump()
		{
			this._penitentAudio.PlayJumpSound();
		}

		public void PlayDash()
		{
			this._penitentAudio.PlayDashSound();
		}

		public void PlayClimbLadder()
		{
			this._penitentAudio.ClimbLadder();
		}

		public void GrabCliffLede()
		{
			this._penitentAudio.GrabCliffLede();
		}

		public void ClimbCliffLede()
		{
			this._penitentAudio.ClimbCliffLede();
		}

		public void JumpOff()
		{
			this._penitentAudio.JumpOff();
		}

		public void PlaySlidingLadder()
		{
			this._penitentAudio.SlidingLadderLanding();
		}

		public void PlaySlidingLadderLanding()
		{
			this._penitentAudio.SlidingLadderLanding();
		}

		public void PlayStickToWall()
		{
			this._penitentAudio.PlayStickToWall();
		}

		public void PlayUnHangFromWall()
		{
			this._penitentAudio.PlayUnHangFromWall();
		}

		public void PlayHardLanding()
		{
			this._penitentAudio.PlayHardLanding();
		}

		public void PlayIdleSword()
		{
			this._penitentAudio.PlayIdleModeSword();
		}

		public void PlayStartDialogue()
		{
			if (!Core.Logic.Penitent.IsVisible())
			{
				return;
			}
			this._penitentAudio.PlayStartDialogue();
		}

		public void PlayEndDialogue()
		{
			this._penitentAudio.PlayEndDialogue();
		}

		public Core.SimpleEventParam OnStep;

		private Penitent _penitent;

		private PenitentAudio _penitentAudio;

		[Header("Freze Times")]
		[Range(0f, 1f)]
		public float freezeTime = 0.2f;

		[Range(0f, 1f)]
		public float freezeTimeFactorPercentage = 0.2f;

		private StepDustSpawner _stepDustSpawner;

		private PushBackDust _pushBackDust;

		private PushBackSpark _pushBackSpark;

		private ThrowbackDust _throwbackDust;

		private GhostTrailGenerator _ghostTrailGenerator;
	}
}
