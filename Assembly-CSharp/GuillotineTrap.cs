using System;
using DG.Tweening;
using FMODUnity;
using Framework.FrameworkCore;
using Framework.Managers;
using Tools.Level.Actionables;
using UnityEngine;

public class GuillotineTrap : MonoBehaviour
{
	private void Start()
	{
		Vector3 euler = new Vector3(0f, 0f, -this.oscillationAngle);
		Vector3 endValue = new Vector3(0f, 0f, this.oscillationAngle);
		this.damageArea = base.GetComponentInChildren<SimpleDamageArea>();
		this.dummyEntity = this.damageArea.GetDummyEntity();
		EntityOrientation orientation = (euler.z >= 0f) ? EntityOrientation.Left : EntityOrientation.Right;
		this.dummyEntity.SetOrientation(orientation, false, false);
		base.transform.rotation = Quaternion.Euler(euler);
		this.tweener = base.transform.DORotate(endValue, this.oscillationTime, RotateMode.Fast).SetEase(this.oscillationCurve).SetLoops(-1, LoopType.Yoyo).OnStepComplete(new TweenCallback(this.StepComplete));
	}

	private void Update()
	{
		if (Core.Logic.IsPaused)
		{
			this.tweener.Pause<Tweener>();
		}
		else if (!this.tweener.IsPlaying())
		{
			this.tweener.TogglePause();
		}
	}

	private void StepComplete()
	{
		this.PlayMotionAudio();
		EntityOrientation orientation = this.dummyEntity.Status.Orientation;
		this.dummyEntity.SetOrientation((orientation != EntityOrientation.Right) ? EntityOrientation.Right : EntityOrientation.Left, false, false);
	}

	private void PlayMotionAudio()
	{
		if (string.IsNullOrEmpty(this.MotionAudioFx))
		{
			return;
		}
		Core.Audio.PlayOneShot(this.MotionAudioFx, base.transform.position);
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Vector2 down = Vector2.down;
		Quaternion rotation = Quaternion.Euler(0f, 0f, this.oscillationAngle);
		Quaternion rotation2 = Quaternion.Euler(0f, 0f, -this.oscillationAngle);
		Vector3 a = rotation * down;
		float d = Mathf.Abs(base.transform.GetChild(0).localPosition.y);
		Gizmos.DrawLine(base.transform.position, base.transform.position + a * d);
		a = rotation2 * down;
		Gizmos.DrawLine(base.transform.position, base.transform.position + a * d);
	}

	public float oscillationTime;

	public Ease oscillationCurve;

	public float oscillationAngle;

	[EventRef]
	public string MotionAudioFx;

	private SimpleDamageArea damageArea;

	private AreaAttackDummyEntity dummyEntity;

	private Tweener tweener;
}
