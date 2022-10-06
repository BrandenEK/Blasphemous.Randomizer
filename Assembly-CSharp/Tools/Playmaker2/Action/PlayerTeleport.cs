using System;
using Framework.Managers;
using HutongGames.PlayMaker;
using UnityEngine;

namespace Tools.PlayMaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Instantly teleports the player to the specific GO position.")]
	public class PlayerTeleport : FsmStateAction
	{
		public override void OnEnter()
		{
			GameObject gameObject = Core.Logic.Penitent.gameObject;
			gameObject.SetActive(false);
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.teleportTarget);
			gameObject.transform.position = ownerDefaultTarget.transform.position;
			gameObject.SetActive(true);
			base.Finish();
		}

		[RequiredField]
		public FsmOwnerDefault teleportTarget;
	}
}
