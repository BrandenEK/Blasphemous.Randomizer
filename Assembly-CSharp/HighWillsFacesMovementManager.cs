using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class HighWillsFacesMovementManager : MonoBehaviour
{
	private void Start()
	{
		this.leftHWStartPos = this.LeftHW.transform.position;
		this.middleHWStartPos = this.MiddleHW.transform.position;
		this.rightHWStartPos = this.RightHW.transform.position;
		this.leftHWMinHorSeparation = this.LeftHW.transform.position.x - this.MiddleHW.transform.position.x;
		this.rightHWMinHorSeparation = this.RightHW.transform.position.x - this.MiddleHW.transform.position.x;
		this.leftHWTargetHorSeparation = this.leftHWMinHorSeparation;
		this.rightHWTargetHorSeparation = this.rightHWMinHorSeparation;
		this.StartLeftHWUpwardsMovement();
		this.StartMiddleHWDownwardsMovement();
		this.StartRightHWUpwardsMovement();
	}

	private void Update()
	{
		float num = this.LeftHW.transform.position.x - this.MiddleHW.transform.position.x;
		if (!Mathf.Approximately(num, this.leftHWTargetHorSeparation) && !this.leftHWHorMoving)
		{
			float num2 = this.LeftHW.transform.position.x + (this.leftHWTargetHorSeparation - num);
			this.leftHWHorMoving = true;
			TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMoveX(this.LeftHW.transform, num2, 0.2f, false), 7), delegate()
			{
				this.leftHWHorMoving = false;
			});
		}
		float num3 = this.RightHW.transform.position.x - this.MiddleHW.transform.position.x;
		if (!Mathf.Approximately(num3, this.rightHWTargetHorSeparation) && !this.rightHWHorMoving)
		{
			float num4 = this.RightHW.transform.position.x + (this.rightHWTargetHorSeparation - num3);
			this.rightHWHorMoving = true;
			TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMoveX(this.RightHW.transform, num4, 0.2f, false), 7), delegate()
			{
				this.rightHWHorMoving = false;
			});
		}
	}

	[Button(0)]
	public void ResetFaces()
	{
		this.leftHWTargetHorSeparation = this.leftHWMinHorSeparation;
		this.rightHWTargetHorSeparation = this.rightHWMinHorSeparation;
		ShortcutExtensions.DOKill(this.LeftHW.transform, false);
		ShortcutExtensions.DOKill(this.MiddleHW.transform, false);
		ShortcutExtensions.DOKill(this.RightHW.transform, false);
		this.LeftHW.transform.position = this.leftHWStartPos;
		this.MiddleHW.transform.position = this.middleHWStartPos;
		this.RightHW.transform.position = this.rightHWStartPos;
		this.StartLeftHWUpwardsMovement();
		this.StartMiddleHWDownwardsMovement();
		this.StartRightHWUpwardsMovement();
	}

	public void SeparateLeftFace(float portion)
	{
		this.leftHWTargetHorSeparation = (this.LeftHWMaxHorSeparation - this.leftHWMinHorSeparation) * portion + this.leftHWMinHorSeparation;
	}

	public void SeparateRightFace(float portion)
	{
		this.rightHWTargetHorSeparation = (this.RightHWMaxHorSeparation - this.rightHWMinHorSeparation) * portion + this.rightHWMinHorSeparation;
	}

	[Button(0)]
	public void SeparateLeftFaceToMax()
	{
		this.SeparateLeftFace(1f);
	}

	[Button(0)]
	public void SeparateRightFaceToMax()
	{
		this.SeparateRightFace(1f);
	}

	[Button(0)]
	public void SeparateLeftFaceToMin()
	{
		this.SeparateLeftFace(0f);
	}

	[Button(0)]
	public void SeparateRightFaceToMin()
	{
		this.SeparateRightFace(0f);
	}

	[Button(0)]
	public void SeparateFacesToMax()
	{
		this.SeparateLeftFaceToMax();
		this.SeparateRightFaceToMax();
	}

	[Button(0)]
	public void SeparateFacesToMin()
	{
		this.SeparateLeftFaceToMin();
		this.SeparateRightFaceToMin();
	}

	public void StartLeftHWUpwardsMovement()
	{
		this.StartHWVerMovement(this.LeftHW, this.LeftHWUpwardsMov, new TweenCallback(this.StartLeftHWDownwardsMovement));
	}

	public void StartLeftHWDownwardsMovement()
	{
		this.StartHWVerMovement(this.LeftHW, this.LeftHWDownwardsMov, new TweenCallback(this.StartLeftHWUpwardsMovement));
	}

	public void StartMiddleHWUpwardsMovement()
	{
		this.StartHWVerMovement(this.MiddleHW, this.MiddleHWUpwardsMov, new TweenCallback(this.StartMiddleHWDownwardsMovement));
	}

	public void StartMiddleHWDownwardsMovement()
	{
		this.StartHWVerMovement(this.MiddleHW, this.MiddleHWDownwardsMov, new TweenCallback(this.StartMiddleHWUpwardsMovement));
	}

	public void StartRightHWUpwardsMovement()
	{
		this.StartHWVerMovement(this.RightHW, this.RightHWUpwardsMov, new TweenCallback(this.StartRightHWDownwardsMovement));
	}

	public void StartRightHWDownwardsMovement()
	{
		this.StartHWVerMovement(this.RightHW, this.RightHWDownwardsMov, new TweenCallback(this.StartRightHWUpwardsMovement));
	}

	private void StartHWVerMovement(GameObject hw, HighWillsFacesMovementManager.FaceMovement fm, TweenCallback onComplete)
	{
		float num = hw.transform.position.y + fm.Length;
		TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMoveY(hw.transform, num, fm.Time, false), fm.Ease), onComplete);
	}

	public GameObject LeftHW;

	public GameObject MiddleHW;

	public GameObject RightHW;

	public float LeftHWMaxHorSeparation;

	public float RightHWMaxHorSeparation;

	public HighWillsFacesMovementManager.FaceMovement LeftHWUpwardsMov;

	public HighWillsFacesMovementManager.FaceMovement MiddleHWUpwardsMov;

	public HighWillsFacesMovementManager.FaceMovement RightHWUpwardsMov;

	public HighWillsFacesMovementManager.FaceMovement LeftHWDownwardsMov;

	public HighWillsFacesMovementManager.FaceMovement MiddleHWDownwardsMov;

	public HighWillsFacesMovementManager.FaceMovement RightHWDownwardsMov;

	private float leftHWMinHorSeparation;

	private float rightHWMinHorSeparation;

	private Vector3 leftHWStartPos;

	private Vector3 middleHWStartPos;

	private Vector3 rightHWStartPos;

	private float leftHWTargetHorSeparation;

	private float rightHWTargetHorSeparation;

	private bool leftHWHorMoving;

	private bool rightHWHorMoving;

	[Serializable]
	public struct FaceMovement
	{
		public Ease Ease;

		public float Length;

		public float Time;
	}
}
