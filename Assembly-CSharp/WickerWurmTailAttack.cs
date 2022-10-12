using System;
using System.Collections;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class WickerWurmTailAttack : MonoBehaviour
{
	public void ShowTail(Vector2 point, bool rightSide, float delay)
	{
		this.pathfollower.spline = this.pathConfigShow.spline;
		this.tailMaster.chainMode = BodyChainMaster.CHAIN_UPDATE_MODES.NORMAL;
		if (!this.pathfollower || !this.pathfollower.spline)
		{
			return;
		}
		this.pathfollower.spline.transform.localScale = new Vector3((float)((!rightSide) ? -1 : 1), this.pathfollower.spline.transform.localScale.y, 1f);
		this.pathfollower.currentCounter = 0f;
		this.pathfollower.spline.gameObject.transform.position = point;
		this.pathfollower.movementCurve = this.pathConfig.curve;
		this.pathfollower.duration = this.pathConfig.duration;
		this.pathfollower.followActivated = true;
		this.pathfollower.OnMovingToNextPoint += this.OnNextPointOnSight;
		this.retractDelay = delay;
	}

	public void TailAttack(Vector2 point, bool rightSide, float delay)
	{
		this.pathfollower.spline = this.pathConfig.spline;
		this.tailMaster.chainMode = BodyChainMaster.CHAIN_UPDATE_MODES.NORMAL;
		this.pathfollower.spline.transform.localScale = new Vector3((float)((!rightSide) ? -1 : 1), this.pathfollower.spline.transform.localScale.y, 1f);
		this.pathfollower.currentCounter = 0f;
		this.pathfollower.spline.gameObject.transform.position = point;
		this.pathfollower.movementCurve = this.pathConfig.curve;
		this.pathfollower.duration = this.pathConfig.duration;
		this.pathfollower.followActivated = true;
		this.pathfollower.OnMovementCompleted += this.OnTailAttackCompleted;
		this.pathfollower.OnMovingToNextPoint += this.OnNextPointOnSight;
		this.retractDelay = delay;
	}

	private void OnNextPointOnSight(Vector2 obj)
	{
		Vector2 vector = obj - this.tailMaster.transform.position;
		this.tailMaster.transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(vector.y, vector.x) * 57.29578f);
	}

	[Button("Debug_TestShow", ButtonSizes.Small)]
	public void TestShow()
	{
		this.ShowTail(base.transform.position, true, 0f);
	}

	[Button("Debug_TestAttac", ButtonSizes.Small)]
	public void TestAttack()
	{
		this.TailAttack(base.transform.position, true, 0f);
	}

	private void OnTailAttackCompleted()
	{
		this.pathfollower.OnMovementCompleted -= this.OnTailAttackCompleted;
		this.pathfollower.OnMovingToNextPoint -= this.OnNextPointOnSight;
		base.StartCoroutine(this.RetractTail());
	}

	private IEnumerator RetractTail()
	{
		yield return new WaitForSeconds(this.retractDelay);
		this.tailMaster.chainMode = BodyChainMaster.CHAIN_UPDATE_MODES.BACKWARDS;
		this.tailEnd.transform.DOMoveY(this.tailEnd.transform.position.y - 20f * this.pathConfig.spline.transform.localScale.y, 2f, false).SetEase(Ease.InOutQuad);
		yield break;
	}

	public BlindBabyPoints.WickerWurmPathConfig pathConfigShow;

	public BlindBabyPoints.WickerWurmPathConfig pathConfig;

	public SplineFollower pathfollower;

	public BodyChainLink tailEnd;

	public BodyChainMaster tailMaster;

	private float retractDelay;

	public float showTailCounter;
}
