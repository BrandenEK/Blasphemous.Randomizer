using System;
using DG.Tweening;
using Framework.Managers;
using Gameplay.GameControllers.Penitent;
using UnityEngine;

namespace Gameplay.GameControllers.Effects.NPCs
{
	public class FourthFaceEyesController : MonoBehaviour
	{
		private void Start()
		{
			this.startingPos = base.transform.position;
			SpawnManager.OnPlayerSpawn += this.OnPlayerSpawn;
		}

		private void OnPlayerSpawn(Penitent penitent)
		{
			this.target = penitent.transform;
		}

		private void Update()
		{
			if (!this.target)
			{
				return;
			}
			if (Vector2.Distance(this.target.position, base.transform.position) > this.maxDistanceToFollow)
			{
				this.ReturnToStartingPosition();
			}
			else
			{
				this.LookAtTarget();
			}
		}

		private void LookAtTarget()
		{
			if (this.horLookTween != null || this.verLookTween != null)
			{
				return;
			}
			this.LookHorizontally();
			this.LookVertically();
		}

		private void LookHorizontally()
		{
			float endValue;
			if (this.target.position.x > this.startingPos.x)
			{
				float num = this.target.position.x - this.startingPos.x;
				endValue = Mathf.Lerp(this.startingPos.x, this.startingPos.x + this.maxHorizontalDisplacement, num / this.maxDistanceToFollow);
			}
			else
			{
				float num2 = this.startingPos.x - this.target.position.x;
				endValue = Mathf.Lerp(this.startingPos.x, this.startingPos.x - this.maxHorizontalDisplacement, num2 / this.maxDistanceToFollow);
			}
			this.horLookTween = base.transform.DOMoveX(endValue, 0.1f, false).SetEase(Ease.InQuad).OnComplete(delegate
			{
				this.horLookTween = null;
			});
		}

		private void LookVertically()
		{
			float num = this.target.position.y + this.verticalOffset;
			float endValue;
			if (num > this.startingPos.y)
			{
				float num2 = num - this.startingPos.y;
				endValue = Mathf.Lerp(this.startingPos.y, this.startingPos.y + this.maxVerticalDisplacement, num2 / this.maxDistanceToFollow);
			}
			else
			{
				float num3 = this.startingPos.y - num;
				endValue = Mathf.Lerp(this.startingPos.y, this.startingPos.y - this.maxVerticalDisplacement, num3 / this.maxDistanceToFollow);
			}
			this.verLookTween = base.transform.DOMoveY(endValue, 0.1f, false).SetEase(Ease.InQuad).OnComplete(delegate
			{
				this.verLookTween = null;
			});
		}

		private void ReturnToStartingPosition()
		{
			if (this.horLookTween != null || this.verLookTween != null)
			{
				return;
			}
			this.horLookTween = base.transform.DOMoveX(this.startingPos.x, 0.5f, false).SetEase(Ease.InQuad).OnComplete(delegate
			{
				this.horLookTween = null;
			});
			this.verLookTween = base.transform.DOMoveY(this.startingPos.y, 0.5f, false).SetEase(Ease.InQuad).OnComplete(delegate
			{
				this.verLookTween = null;
			});
		}

		public Transform target;

		public float maxDistanceToFollow;

		public float maxHorizontalDisplacement;

		public float maxVerticalDisplacement;

		public float verticalOffset;

		private Vector2 startingPos;

		private Tween horLookTween;

		private Tween verLookTween;
	}
}
