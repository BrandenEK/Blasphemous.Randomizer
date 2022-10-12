using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class BodyChainLink : MonoBehaviour
{
	private void Awake()
	{
		this.spr = base.GetComponentInChildren<SpriteRenderer>();
		this.spriteChild = this.spr.transform;
	}

	private void Update()
	{
		if (!this.animationStarted)
		{
			if (this.animationCounter >= this.syncOffset)
			{
				this.SecondaryAnimation();
				this.animationStarted = true;
			}
			this.animationCounter += Time.deltaTime;
		}
	}

	private void SecondaryAnimation()
	{
		this.spriteChild.transform.DOLocalMoveY(this.secAnimationAmplitude, this.secAnimationDuration, false).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo);
	}

	public void UpdateChainLink()
	{
		if (this.fixedPoint && this.fixedRotation)
		{
			return;
		}
		Vector2 normalized = (this.targetLink.position - base.transform.position).normalized;
		float z = 57.29578f * Mathf.Atan2(normalized.y, normalized.x);
		if (!this.fixedRotation)
		{
			base.transform.rotation = Quaternion.Euler(0f, 0f, z);
		}
		if (!this.fixedPoint && Vector2.Distance(this.targetLink.position, base.transform.position) > this.distance)
		{
			Vector2 v = this.targetLink.position - normalized * this.distance;
			base.transform.position = v;
		}
	}

	public void UpdateBackwardsChainLink()
	{
		if (this.fixedPoint && this.fixedRotation)
		{
			return;
		}
		Vector2 normalized = (this.previousLink.position - base.transform.position).normalized;
		float z = 180f + 57.29578f * Mathf.Atan2(normalized.y, normalized.x);
		if (!this.fixedRotation)
		{
			base.transform.rotation = Quaternion.Euler(0f, 0f, z);
		}
		if (!this.fixedPoint && Vector2.Distance(this.previousLink.position, base.transform.position) > this.distance)
		{
			Vector2 v = this.previousLink.position - normalized * this.distance * 0.9f;
			base.transform.position = v;
		}
	}

	public void ReverseUpdateChainLink()
	{
		Vector2 normalized = (this.targetLink.position - base.transform.position).normalized;
		float f = 57.29578f * Mathf.Atan2(normalized.y, normalized.x);
		float num = Mathf.Sign(f);
		if (!this.fixedPoint)
		{
			bool flag = this.ClampedLookAt(this.targetLink.position);
		}
		Vector3 a = base.transform.position + base.transform.right * this.distance;
		this.targetLink.parent.position = a - this.targetLink.localPosition;
	}

	public bool ClampedLookAt(Vector2 point)
	{
		bool flag = false;
		float num = 57.29578f * Mathf.Atan2(point.y, point.x);
		float num2 = Mathf.Sign(num - this.baseAngle);
		float num3 = Mathf.Abs(num - this.baseAngle);
		if (num3 > this.limitAngle)
		{
			flag = true;
		}
		Quaternion b = Quaternion.Euler(0f, 0f, (!flag) ? num : (this.baseAngle + num2 * this.limitAngle));
		base.transform.rotation = Quaternion.Slerp(base.transform.rotation, b, this.rotationDamp);
		return flag;
	}

	public void FlipSprite(bool flip)
	{
		this.spr.flipY = flip;
	}

	public void Freeze(bool state)
	{
		this.fixedPoint = state;
		this.fixedRotation = state;
	}

	private void OnDrawGizmosSelected()
	{
		if (!this.targetLink)
		{
			return;
		}
		Gizmos.color = Color.red;
		Gizmos.DrawLine(base.transform.position, this.targetLink.position);
		Gizmos.color = Color.green;
		Vector3 b = (this.targetLink.position - base.transform.position).normalized * this.distance;
		Gizmos.DrawLine(base.transform.position, base.transform.position + b);
	}

	public float DistanceToPoint()
	{
		return Vector2.Distance(this.targetLink.position, base.transform.position);
	}

	public Transform targetLink;

	public Transform previousLink;

	public float distance = 1f;

	public bool fixedPoint;

	public bool fixedRotation;

	private SpriteRenderer spr;

	public float secAnimationDuration = 1f;

	public float secAnimationAmplitude = 1f;

	public float syncOffset;

	private Transform spriteChild;

	private bool animationStarted;

	private float animationCounter;

	public float rotationDamp = 0.5f;

	public float limitAngle = 30f;

	[FoldoutGroup("Debug", 0)]
	public float lastAngle;

	public float baseAngle;
}
