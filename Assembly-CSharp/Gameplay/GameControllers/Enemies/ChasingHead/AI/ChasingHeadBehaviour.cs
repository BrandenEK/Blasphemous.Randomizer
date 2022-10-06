using System;
using DG.Tweening;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Enemies.ChasingHead.Animator;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.ChasingHead.AI
{
	public class ChasingHeadBehaviour : EnemyBehaviour
	{
		public ChasingHeadAnimatorInyector AnimatorInyector { get; private set; }

		public override void OnStart()
		{
			base.OnStart();
			this._chasingHead = (ChasingHead)this.Entity;
			this.AnimatorInyector = this._chasingHead.GetComponentInChildren<ChasingHeadAnimatorInyector>();
			this._clockWise = (Random.value > 0.5f);
			DOTween.To(delegate(float x)
			{
				this._currentAmplitudeX = x;
			}, this._currentAmplitudeX, this.amplitudeX, 1f);
			DOTween.To(delegate(float x)
			{
				this._currentAmplitudeY = x;
			}, this._currentAmplitudeY, this.amplitudeY, 1f);
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			GameObject target = this._chasingHead.Target;
			EntityOrientation orientation = (target.transform.position.x > this._chasingHead.SpriteRenderer.transform.position.x) ? EntityOrientation.Right : EntityOrientation.Left;
			if (!this._chasingHead.Status.Dead)
			{
				this._chasingHead.SetOrientation(orientation, true, false);
			}
			if (this._chasingHead.Status.IsHurt)
			{
				return;
			}
			this.Chase(target.transform);
			this.Floating(this._clockWise);
		}

		public override void Idle()
		{
			throw new NotImplementedException();
		}

		public override void Wander()
		{
			throw new NotImplementedException();
		}

		public override void Chase(Transform targetPosition)
		{
			if (!base.IsChasing)
			{
				base.IsChasing = true;
				this._chasingHead.GhostSprites.EnableGhostTrail = true;
			}
			Vector3 vector;
			vector..ctor(targetPosition.position.x, targetPosition.position.y);
			base.transform.position = Vector3.SmoothDamp(base.transform.position, vector, ref this._velocity, this.ChasingElongation, this.Speed);
		}

		public override void Attack()
		{
			throw new NotImplementedException();
		}

		public override void Damage()
		{
			this.AnimatorInyector.Hurt();
			this.HurtDisplacement(this._chasingHead.DamageArea.LastHit.AttackingEntity, null);
		}

		public void Death()
		{
			this.AnimatorInyector.Death();
			this._chasingHead.DamageArea.DamageAreaCollider.enabled = false;
			this.HurtDisplacement(this._chasingHead.DamageArea.LastHit.AttackingEntity, null);
		}

		public void HurtDisplacement(GameObject attackingEntity, TweenCallback onCompleteCallback)
		{
			float num = (this._chasingHead.Status.Orientation != EntityOrientation.Right) ? this.hurtDisplacement : (-this.hurtDisplacement);
			TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.OnStart<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMoveX(this._chasingHead.transform, base.transform.position.x + num, 0.3f, false), 3), new TweenCallback(this.OnStartDisplacement)), onCompleteCallback);
		}

		private void OnStartDisplacement()
		{
			if (this.OnHurtDisplacement != null)
			{
				this.OnHurtDisplacement();
			}
		}

		public override void StopMovement()
		{
			throw new NotImplementedException();
		}

		public void Floating(bool clockWise = true)
		{
			this._index += Time.deltaTime;
			float num;
			float num2;
			if (clockWise)
			{
				num = this._currentAmplitudeX * Mathf.Sin(this.speedX * this._index);
				num2 = Mathf.Cos(this.speedY * this._index) * this._currentAmplitudeY;
			}
			else
			{
				num = this._currentAmplitudeX * Mathf.Cos(this.speedX * this._index);
				num2 = Mathf.Sin(this.speedY * this._index) * this._currentAmplitudeY;
			}
			this._chasingHead.SpriteRenderer.transform.localPosition = new Vector3(num, num2, 0f);
		}

		public Core.SimpleEvent OnHurtDisplacement;

		private ChasingHead _chasingHead;

		private bool _clockWise;

		private float _index;

		private Vector3 _velocity = Vector3.zero;

		[FoldoutGroup("Floating Motion", true, 0)]
		public float amplitudeX = 10f;

		private float _currentAmplitudeX;

		[FoldoutGroup("Floating Motion", true, 0)]
		public float amplitudeY = 5f;

		private float _currentAmplitudeY;

		[FoldoutGroup("Chasing Motion", true, 0)]
		public float ChasingElongation = 0.5f;

		[FoldoutGroup("Hurt Displacement", true, 0)]
		public float hurtDisplacement = 2f;

		[FoldoutGroup("Chasing Motion", true, 0)]
		public float Speed = 5f;

		[FoldoutGroup("Floating Motion", true, 0)]
		public float speedX = 1f;

		[FoldoutGroup("Floating Motion", true, 0)]
		public float speedY = 2f;
	}
}
