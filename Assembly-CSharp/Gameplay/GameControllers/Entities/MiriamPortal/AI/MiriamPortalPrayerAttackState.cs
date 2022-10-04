using System;
using DG.Tweening;
using Framework.Managers;
using Gameplay.GameControllers.Entities.MiriamPortal.Animation;
using Gameplay.GameControllers.Entities.StateMachine;
using UnityEngine;

namespace Gameplay.GameControllers.Entities.MiriamPortal.AI
{
	public class MiriamPortalPrayerAttackState : State
	{
		public override void OnStateInitialize(StateMachine machine)
		{
			base.OnStateInitialize(machine);
			this._miriamPortalPrayer = this.Machine.GetComponentInChildren<MiriamPortalPrayer>();
		}

		public override void OnStateEnter()
		{
			base.OnStateEnter();
			GameObject portalShatteringVfx = this._miriamPortalPrayer.Behaviour.PortalShatteringVfx;
			Vector3 position = this._miriamPortalPrayer.transform.position;
			PoolManager.ObjectInstance objectInstance = PoolManager.Instance.ReuseObject(portalShatteringVfx, position, Quaternion.identity, false, 1);
			objectInstance.GameObject.GetComponent<SimpleVFX>().SetOrientation(this._miriamPortalPrayer.Status.Orientation, false);
			Core.Logic.ScreenFreeze.Freeze(0.05f, 0.2f, 0f, null);
			this.ForwardMovement();
		}

		public override void OnStateExit()
		{
			base.OnStateExit();
			this._miriamPortalPrayer.transform.DOKill(false);
		}

		public override void Update()
		{
			base.Update();
		}

		private void ForwardMovement()
		{
			Vector2 actionDirection = this._miriamPortalPrayer.Behaviour.GetActionDirection(true);
			Vector3 position = this._miriamPortalPrayer.transform.position;
			float num = Mathf.Abs(actionDirection.y - position.y);
			float num2 = 0.1f;
			float descendingTime = Mathf.Lerp(0.1f, 0.5f, num / this._miriamPortalPrayer.Behaviour.MaxDistanceToHitGround);
			this._miriamPortalPrayer.transform.DOMoveY(position.y + 1f, num2, false).SetEase(Ease.InQuad).OnComplete(delegate
			{
				this._miriamPortalPrayer.transform.DOMoveY(actionDirection.y, descendingTime, false).SetEase(Ease.InQuad).OnComplete(delegate
				{
					this.FinishAttack(actionDirection);
				});
			});
			this._miriamPortalPrayer.transform.DOMoveX(actionDirection.x, num2 + descendingTime, false).SetEase(Ease.OutQuad).OnStart(new TweenCallback(this.StartAttack));
			Sequence sequence = DOTween.Sequence();
			sequence.AppendInterval(descendingTime);
			sequence.AppendCallback(delegate
			{
				this.ResumeAnimation();
			});
			sequence.Play<Sequence>();
		}

		private void StartAttack()
		{
			this._miriamPortalPrayer.AnimationHandler.SetAnimatorTrigger(MiriamPortalPrayerAnimationHandler.AttackTrigger);
			this._miriamPortalPrayer.Audio.PlayAttack();
			this._miriamPortalPrayer.GhostTrail.EnableGhostTrail = true;
		}

		private void ResumeAnimation()
		{
			this._miriamPortalPrayer.Animator.speed = 1f;
		}

		private void FinishAttack(Vector2 actionDirection)
		{
			this._miriamPortalPrayer.transform.DOMoveY(actionDirection.y - 0.3f, 1f, false).SetEase(Ease.InQuad);
			this._miriamPortalPrayer.Behaviour.CheckAndSpawnLandingVfx();
			this._miriamPortalPrayer.GhostTrail.EnableGhostTrail = false;
			this.ResumeAnimation();
		}

		private MiriamPortalPrayer _miriamPortalPrayer;
	}
}
