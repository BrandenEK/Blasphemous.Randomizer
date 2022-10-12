using System;
using DG.Tweening;
using Framework.Pooling;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Entities
{
	public class MoveOnStart : PoolObject
	{
		private bool showSpriteRendererAttribute
		{
			get
			{
				return this.FlipHorizontalDirectionByRenderer || this.FlipVerticalDirectionByRenderer;
			}
		}

		private void Start()
		{
			this.startPos = this.motionObject.localPosition;
		}

		public override void OnObjectReuse()
		{
			base.OnObjectReuse();
			this.motionObject.localPosition = this.startPos;
			if (this.FlipHorizontalDirectionByRenderer && this.SpriteRenderer.flipX)
			{
				this.HorizontalDistanceToMove = -this.HorizontalDistanceToMove;
			}
			if (this.FlipVerticalDirectionByRenderer && this.SpriteRenderer.flipY)
			{
				this.VerticalDistanceToMove = -this.VerticalDistanceToMove;
			}
			float endValue = this.motionObject.position.x + this.HorizontalDistanceToMove;
			if (this.UseAnimationCurveForHorizontalMovement)
			{
				this.motionObject.DOMoveX(endValue, this.MoveDuration, false).SetEase(this.HorizontalMovementAnimationCurve);
			}
			else
			{
				this.motionObject.DOMoveX(endValue, this.MoveDuration, false).SetEase(this.HorizontalMovementEase);
			}
			float endValue2 = this.motionObject.position.y + this.VerticalDistanceToMove;
			if (this.UseAnimationCurveForVerticalMovement)
			{
				this.motionObject.DOMoveY(endValue2, this.MoveDuration, false).SetEase(this.VerticalMovementAnimationCurve);
			}
			else
			{
				this.motionObject.DOMoveY(endValue2, this.MoveDuration, false).SetEase(this.VerticalMovementEase);
			}
		}

		[Header("Motion Params")]
		[Tooltip("The transform used to move the game object")]
		[SerializeField]
		private Transform motionObject;

		[Tooltip("The time taken to move from the start to finish positions during the movement")]
		public float MoveDuration;

		[Tooltip("Should the horizontal movement direction flip according to the Sprite Renderer?")]
		public bool FlipHorizontalDirectionByRenderer;

		[Tooltip("Should the vertical movement direction flip according to the Sprite Renderer?")]
		public bool FlipVerticalDirectionByRenderer;

		[ShowIf("showSpriteRendererAttribute", true)]
		[Tooltip("SpriteRenderer that is checked in order to flip the movement directions")]
		public SpriteRenderer SpriteRenderer;

		[Tooltip("How far the object should move when along the X axis")]
		public float HorizontalDistanceToMove;

		[Tooltip("Should the horizontal movement use a custom Animation Curve or a predefined Ease?")]
		public bool UseAnimationCurveForHorizontalMovement;

		[HideIf("UseAnimationCurveForHorizontalMovement", true)]
		[Tooltip("The Ease to use in the horizontal movement")]
		public Ease HorizontalMovementEase;

		[ShowIf("UseAnimationCurveForHorizontalMovement", true)]
		[Tooltip("The AnimationCurve to use in the horizontal movement")]
		public AnimationCurve HorizontalMovementAnimationCurve;

		[Tooltip("How far the object should move when along the Y axis")]
		public float VerticalDistanceToMove;

		[Tooltip("Should the vertical movement use a custom animation curve or a predefined ease?")]
		public bool UseAnimationCurveForVerticalMovement;

		[HideIf("UseAnimationCurveForVerticalMovement", true)]
		[Tooltip("The Ease to use in the vertical movement")]
		public Ease VerticalMovementEase;

		[ShowIf("UseAnimationCurveForVerticalMovement", true)]
		[Tooltip("The AnimationCurve to use in the horizontal movement")]
		public AnimationCurve VerticalMovementAnimationCurve;

		private Vector2 startPos;
	}
}
