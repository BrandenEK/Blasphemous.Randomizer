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
		Vector3 vector;
		vector..ctor(0f, 0f, -this.oscillationAngle);
		Vector3 vector2;
		vector2..ctor(0f, 0f, this.oscillationAngle);
		this.damageArea = base.GetComponentInChildren<SimpleDamageArea>();
		this.dummyEntity = this.damageArea.GetDummyEntity();
		EntityOrientation orientation = (vector.z >= 0f) ? EntityOrientation.Left : EntityOrientation.Right;
		this.dummyEntity.SetOrientation(orientation, false, false);
		base.transform.rotation = Quaternion.Euler(vector);
		this.tweener = TweenSettingsExtensions.OnStepComplete<Tweener>(TweenSettingsExtensions.SetLoops<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DORotate(base.transform, vector2, this.oscillationTime, 0), this.oscillationCurve), -1, 1), new TweenCallback(this.StepComplete));
	}

	private void Update()
	{
		if (Core.Logic.IsPaused)
		{
			TweenExtensions.Pause<Tweener>(this.tweener);
		}
		else if (!TweenExtensions.IsPlaying(this.tweener))
		{
			TweenExtensions.TogglePause(this.tweener);
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
		Quaternion quaternion = Quaternion.Euler(0f, 0f, this.oscillationAngle);
		Quaternion quaternion2 = Quaternion.Euler(0f, 0f, -this.oscillationAngle);
		Vector3 vector = quaternion * down;
		float num = Mathf.Abs(base.transform.GetChild(0).localPosition.y);
		Gizmos.DrawLine(base.transform.position, base.transform.position + vector * num);
		vector = quaternion2 * down;
		Gizmos.DrawLine(base.transform.position, base.transform.position + vector * num);
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
