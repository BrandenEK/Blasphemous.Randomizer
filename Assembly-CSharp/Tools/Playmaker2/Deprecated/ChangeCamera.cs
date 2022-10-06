using System;
using Framework.FrameworkCore;
using Framework.Managers;
using HutongGames.PlayMaker;
using UnityEngine;

namespace Tools.Playmaker2.Deprecated
{
	[ActionCategory("Blasphemous Deprecated")]
	[Tooltip("Changes the current main camera.")]
	public class ChangeCamera : FsmStateAction
	{
		public override void OnEnter()
		{
			if (this.action == ChangeCamera.ChangeCameraOptions.SET_CAMERA)
			{
				this.SetCamera();
			}
			else if (this.action == ChangeCamera.ChangeCameraOptions.RETURN_TO_NORMAL)
			{
				this.ReturnToNormal();
			}
		}

		private void SetCamera()
		{
			if (!Core.ready)
			{
				Log.Error("Playmaker", "Framework not initialized yet.", null);
				base.Finish();
				return;
			}
			if (this.camera.Value == null)
			{
				Log.Error("Playmaker", "The selected camera is null.", null);
				base.Finish();
				return;
			}
			Camera component = this.camera.Value.GetComponent<Camera>();
			if (component == null)
			{
				Log.Error("Playmaker", "The selected object has no camera component.", null);
				base.Finish();
				return;
			}
			base.Finish();
		}

		private void ReturnToNormal()
		{
		}

		public ChangeCamera.ChangeCameraOptions action;

		public FsmGameObject camera;

		public enum ChangeCameraOptions
		{
			SET_CAMERA,
			RETURN_TO_NORMAL
		}
	}
}
