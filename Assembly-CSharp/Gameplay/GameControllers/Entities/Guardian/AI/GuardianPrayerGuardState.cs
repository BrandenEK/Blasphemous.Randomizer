using System;
using DG.Tweening;
using Framework.Managers;
using Gameplay.GameControllers.Entities.Guardian.Animation;
using Gameplay.GameControllers.Entities.StateMachine;
using Gameplay.GameControllers.Penitent;
using Gameplay.GameControllers.Penitent.Damage;

namespace Gameplay.GameControllers.Entities.Guardian.AI
{
	public class GuardianPrayerGuardState : State
	{
		public override void OnStateInitialize(StateMachine machine)
		{
			base.OnStateInitialize(machine);
			this._guardianPrayer = this.Machine.GetComponentInChildren<GuardianPrayer>();
			PenitentDamageArea.OnHitGlobal = (PenitentDamageArea.PlayerHitEvent)Delegate.Combine(PenitentDamageArea.OnHitGlobal, new PenitentDamageArea.PlayerHitEvent(this.OnMasterHit));
		}

		public override void OnStateEnter()
		{
			base.OnStateEnter();
			this._playerPreviousState = Core.Logic.Penitent.Status.Invulnerable;
			this.SetPlayerInvulnerable(true);
			this.ForwardMovement();
		}

		public override void OnStateExit()
		{
			base.OnStateExit();
			this.SetPlayerInvulnerable(this._playerPreviousState);
		}

		public override void Update()
		{
			base.Update();
		}

		private void ForwardMovement()
		{
			float shieldDistance = this._guardianPrayer.Behaviour.ShieldDistance;
			float actionDirection = this._guardianPrayer.Behaviour.GetActionDirection(shieldDistance);
			this._guardianPrayer.transform.DOMoveX(actionDirection, 0.1f, true).SetEase(Ease.InSine).OnStart(new TweenCallback(this.OnStartForwardMovement)).OnComplete(new TweenCallback(this.OnFinishForwardMovement));
		}

		private void OnStartForwardMovement()
		{
			this.Guard();
		}

		private void OnFinishForwardMovement()
		{
		}

		private void Guard()
		{
			this._guardianPrayer.AnimationHandler.SetAnimatorTrigger(GuardianPrayerAnimationHandler.GuardTrigger);
			this._guardianPrayer.Audio.PlayGuard();
		}

		private void OnMasterHit(Penitent penitent, Hit hit)
		{
			Core.Logic.CameraManager.ProCamera2DShake.ShakeUsingPreset("SimpleHit");
		}

		private void SetPlayerInvulnerable(bool invulnerable)
		{
			Core.Logic.Penitent.Status.Invulnerable = invulnerable;
		}

		public override void Destroy()
		{
			base.Destroy();
			PenitentDamageArea.OnHitGlobal = (PenitentDamageArea.PlayerHitEvent)Delegate.Remove(PenitentDamageArea.OnHitGlobal, new PenitentDamageArea.PlayerHitEvent(this.OnMasterHit));
		}

		private GuardianPrayer _guardianPrayer;

		private bool _playerPreviousState;
	}
}
