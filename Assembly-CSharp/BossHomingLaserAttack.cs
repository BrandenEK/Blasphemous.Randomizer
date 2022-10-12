using System;
using System.Collections;
using System.Diagnostics;
using Framework.FrameworkCore;
using UnityEngine;

public class BossHomingLaserAttack : MonoBehaviour
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<BossHomingLaserAttack> OnAttackFinished;

	private void Awake()
	{
		this.beamLauncher.displayEndAnimation = false;
	}

	public void DelayedTargetedBeam(Transform target, float warningDelay, float duration, EntityOrientation orientation = EntityOrientation.Right, bool forceOrientation = false)
	{
		this._target = target;
		this.beamLauncher.ActivateDelayedBeam(warningDelay, true);
		this.orientation = orientation;
		this.forceOrientation = forceOrientation;
		this.SetRotationToTarget();
		base.StartCoroutine(this.DelayedDeactivation(duration));
	}

	private void SetRotationToTarget()
	{
		Vector2 vector = this._target.transform.position + this.targetOffset - base.transform.position;
		if (this.forceOrientation)
		{
			vector.x = ((this.orientation != EntityOrientation.Right) ? (-Mathf.Abs(vector.x)) : Mathf.Abs(vector.x));
		}
		float num = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
		num = this.ClampTargetAngleAtStart(num);
		base.transform.rotation = Quaternion.Euler(0f, 0f, num);
	}

	public void Clear()
	{
		this.StopBeam();
		this.beamLauncher.ClearAll();
		base.StopAllCoroutines();
	}

	private IEnumerator DelayedDeactivation(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		this.StopBeam();
		yield break;
	}

	public void StopBeam()
	{
		this.beamLauncher.ActivateBeamAnimation(false);
		if (this.OnAttackFinished != null)
		{
			this.OnAttackFinished(this);
		}
	}

	private void Update()
	{
		if (this._target)
		{
			this.UpdateAimingAngle();
		}
	}

	private void UpdateAimingAngle()
	{
		Vector2 vector = this._target.transform.position + this.targetOffset - base.transform.position;
		float z = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
		Quaternion b = Quaternion.Euler(0f, 0f, z);
		z = this.ClampTargetAngle(Quaternion.Slerp(base.transform.rotation, b, this.accuracy).eulerAngles.z);
		base.transform.rotation = Quaternion.Euler(0f, 0f, z);
	}

	private void DrawDebugLine(Color c)
	{
		Vector2 v = this._target.transform.position + this.targetOffset - base.transform.position;
		UnityEngine.Debug.DrawLine(this._target.transform.position, this._target.transform.position + v, c, 2f);
	}

	private float ClampTargetAngleAtStart(float targetAngle)
	{
		if (targetAngle < 0f)
		{
			targetAngle += 360f;
		}
		if (targetAngle > this.lowAngleLimit && targetAngle < this.highAngleLimit)
		{
			targetAngle = this.CloseToBoundary(this.lowAngleLimit, this.highAngleLimit, targetAngle);
		}
		return targetAngle;
	}

	private float CloseToBoundary(float min, float max, float val)
	{
		float num = Mathf.Abs(val - min);
		float num2 = Mathf.Abs(val - max);
		return (num >= num2) ? max : min;
	}

	private float ClampTargetAngle(float targetAngle)
	{
		if (targetAngle < 0f)
		{
			targetAngle += 360f;
		}
		if (targetAngle > this.lowAngleLimit && targetAngle < this.highAngleLimit)
		{
			float z = base.transform.rotation.eulerAngles.z;
			if (z <= this.lowAngleLimit && z > 90f)
			{
				targetAngle = this.lowAngleLimit;
				this.DrawDebugLine(Color.yellow);
			}
			else if ((z <= 90f && z >= 0f) || (z < 0f && z >= this.highAngleLimit))
			{
				targetAngle = this.highAngleLimit;
				this.DrawDebugLine(Color.red);
			}
			else
			{
				this.DrawDebugLine(Color.magenta);
				if (z < 270f)
				{
					targetAngle = this.lowAngleLimit;
				}
				else
				{
					targetAngle = this.highAngleLimit;
				}
			}
		}
		return targetAngle;
	}

	public TileableBeamLauncher beamLauncher;

	private Transform _target;

	public float accuracy = 0.01f;

	public float lowAngleLimit = 240f;

	public float highAngleLimit = 300f;

	public Vector2 targetOffset;

	private EntityOrientation orientation;

	private bool forceOrientation;
}
