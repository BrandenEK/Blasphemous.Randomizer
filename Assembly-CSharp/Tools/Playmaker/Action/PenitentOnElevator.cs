using System;
using Framework.Managers;
using HutongGames.PlayMaker;
using UnityEngine;

namespace Tools.PlayMaker.Action
{
	[ActionCategory("Blasphemous Action")]
	[HutongGames.PlayMaker.Tooltip("Prepares TPO for riding the elevator.")]
	public class PenitentOnElevator : FsmStateAction
	{
		public override void OnEnter()
		{
			bool value = this.enablePenitentPhysics.Value;
			if (value)
			{
				Core.Logic.Penitent.transform.localPosition = new Vector3(Core.Logic.Penitent.transform.localPosition.x, 0f, Core.Logic.Penitent.transform.localPosition.z);
				Core.Logic.Penitent.PlatformCharacterController.ResetPreviousPosition();
			}
			Core.Logic.Penitent.transform.SetParent((!value) ? this.parentToElement.Value.transform : null);
			Core.Logic.Penitent.Physics.EnablePhysics(value);
			base.Finish();
		}

		public FsmBool enablePenitentPhysics;

		public FsmGameObject parentToElement;
	}
}
