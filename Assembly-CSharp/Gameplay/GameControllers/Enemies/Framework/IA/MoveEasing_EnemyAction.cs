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
			Vector2 vector = e.transform.position;
			Vector2 vector2;
			vector2..ctor(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
			this.point = vector + vector2.normalized * distance;
			Vector2 target = this.point;
			return this.StartAction(e, target, _seconds, _easing, null, true, null, true, true, _overShootOrAmplitude);
		}

		protected override void DoOnStart()
		{
			base.DoOnStart();
		}

		protected override void DoOnStop()
		{
			TweenExtensions.Kill(this.t, false);
			base.DoOnStop();
		}

		protected override IEnumerator BaseCoroutine()
		{
			Transform transform = this.transformToMove ?? this.owner.transform;
			if (this.tweenOnX && this.tweenOnY)
			{
				this.t = TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(transform, this.point, this.seconds, false), this.easingCurve);
			}
			else if (this.tweenOnX && !this.tweenOnY)
			{
				this.t = TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMoveX(transform, this.point.x, this.seconds, false), this.easingCurve);
			}
			else if (!this.tweenOnX && this.tweenOnY)
			{
				this.t = TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMoveY(transform, this.point.y, this.seconds, false), this.easingCurve);
			}
			else
			{
				Debug.LogError("MoveEasing_EnemyAction::BaseCoroutine: tweenOnX or tweenOnY should be set to true!");
			}
			this.t.easeOvershootOrAmplitude = this.overShootOrAmplitude;
			if (this.doOnTweenUpdate != null)
			{
				TweenSettingsExtensions.OnUpdate<Tween>(this.t, delegate()
				{
					this.doOnTweenUpdate();
				});
			}
			TweenSettingsExtensions.SetUpdate<Tween>(this.t, !this.timeScaled);
			yield return TweenExtensions.WaitForCompletion(this.t);
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
