using System;
using Framework.Managers;
using HutongGames.PlayMaker;
using UnityEngine;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[HutongGames.PlayMaker.Tooltip("Sets the camera in free mode and sets a position.")]
	public class CameraModeFree : FsmStateAction
	{
		public override void Reset()
		{
			if (this.Target == null)
			{
				this.Target = new FsmGameObject();
				this.Target.Value = null;
			}
		}

		public override void OnEnter()
		{
			base.OnEnter();
			Vector3 freeCameraPosition = (this.Target == null || !(this.Target.Value != null)) ? Vector3.zero : this.Target.Value.transform.position;
			Core.Cinematics.SetFreeCamera(true);
			Core.Cinematics.SetFreeCameraPosition(freeCameraPosition);
			base.Finish();
		}

		public FsmGameObject Target;
	}
}
