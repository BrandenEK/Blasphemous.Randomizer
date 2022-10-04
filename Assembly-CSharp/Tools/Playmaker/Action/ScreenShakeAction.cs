using System;
using Com.LuisPedroFonseca.ProCamera2D;
using Framework.Managers;
using HutongGames.PlayMaker;
using UnityEngine;

namespace Tools.PlayMaker.Action
{
	public class ScreenShakeAction : FsmStateAction
	{
		public override void OnEnter()
		{
			ProCamera2DShake proCamera2DShake = Core.Logic.CameraManager.ProCamera2DShake;
			if (proCamera2DShake == null)
			{
				Debug.LogError("The ProCamera2D component needs to have the Shake plugin enabled.");
			}
			if (ProCamera2D.Instance != null && proCamera2DShake != null)
			{
				proCamera2DShake.ShakeUsingPreset("PietyStomp");
			}
			base.Fsm.Event(this.finishEvent);
			base.Finish();
		}

		[RequiredField]
		public FsmEvent finishEvent;
	}
}
