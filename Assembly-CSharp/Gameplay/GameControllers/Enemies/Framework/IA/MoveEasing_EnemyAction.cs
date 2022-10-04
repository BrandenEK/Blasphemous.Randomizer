using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Framework.IA
{
	public class MoveEasing_EnemyAction : EnemyAction
	{
		public EnemyAction StartAction(EnemyBehaviour e, Vector2 _target, float _seconds, Ease _easing, Transform _transformToMove = null, bool _timeScaled = true, Action _tweenUpdateCallback = null, bool _tweenOnX = true, bool _tweenOnY = true, float _overShootOrAmplitude = 1.7f)
		{
			this.point = _target;
			this.seconds = _seconds;
			this.easingCurve = _easing;
			this.transformToMove = _transformToMove;
			this.timeScaled = _timeScaled;
			this.doOnTweenUpdate = _tweenUpdateCallback;
			this.tweenOnX = _tweenOnX;
			this.tweenOnY = _tweenOnY;
			this.overShootOrAmplitude = _overShootOrAmplitude;
			return base.StartAction(e);
		}

		public EnemyAction StartAction(EnemyBehaviour e, float distance, float _seconds, Ease _easing, float _overShootOrAmplitude = 1.7f)
		{
			Vector2 a = e.transform.position;
			Vector2 vector = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));
			this.point = a + vector.normalized * distance;
			Vector2 target = this.point;
			return this.StartAction(e, target, _seconds, _easing, null, true, null, true, true, _overShootOrAmplitude);
		}

		protected override void DoOnStart()
		{
			base.DoOnStart();
		}

		protected override void DoOnStop()
		{
			this.t.Kill(false);
			base.DoOnStop();
		}

		protected override IEnumerator BaseCoroutine()
		{
			Transform transform = this.transformToMove ?? this.owner.transform;
			if (this.tweenOnX && this.tweenOnY)
			{
				this.t = transform.DOMove(this.point, this.seconds, false).SetEase(this.easingCurve);
			}
			else if (this.tweenOnX && !this.tweenOnY)
			{
				this.t = transform.DOMoveX(this.point.x, this.seconds, false).SetEase(this.easingCurve);
			}
			else if (!this.tweenOnX && this.tweenOnY)
			{
				this.t = transform.DOMoveY(this.point.y, this.seconds, false).SetEase(this.easingCurve);
			}
			else
			{
				Debug.LogError("MoveEasing_EnemyAction::BaseCoroutine: tweenOnX or tweenOnY should be set to true!");
			}
			this.t.easeOvershootOrAmplitude = this.overShootOrAmplitude;
			if (this.doOnTweenUpdate != null)
			{
				this.t.OnUpdate(delegate
				{
					this.doOnTweenUpdate();
				});
			}
			this.t.SetUpdate(!this.timeScaled);
			yield return this.t.WaitForCompletion();
			base.FinishAction();
			yield break;
		}

		private float seconds;

		private Ease easingCurve;

		private Transform transformToMove;

		private Vector2 point;

		private bool timeScaled;

		private Action doOnTweenUpdate;

		private Tween t;

		private bool tweenOnX;

		private bool tweenOnY;

		private float overShootOrAmplitude;
	}
}
