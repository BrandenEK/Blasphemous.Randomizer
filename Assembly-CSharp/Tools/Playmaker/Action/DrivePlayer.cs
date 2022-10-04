using System;
using Framework.FrameworkCore;
using Framework.Managers;
using HutongGames.PlayMaker;
using UnityEngine;

namespace Tools.PlayMaker.Action
{
	public class DrivePlayer : FsmStateAction
	{
		public override void OnEnter()
		{
			base.OnEnter();
			if (this.Destination.Value == null)
			{
				Core.Logic.Penitent.SetOrientation(this.FinalPlayerOrientation, true, false);
			}
			else
			{
				Vector3 position = this.Destination.Value.transform.position;
				Core.Logic.Penitent.DrivePlayer.OnStopMotion += this.OnStopMotion;
				Core.Logic.Penitent.DrivePlayer.MoveToPosition(position, this.FinalPlayerOrientation);
			}
		}

		private void OnStopMotion()
		{
			Core.Logic.Penitent.DrivePlayer.OnStopMotion -= this.OnStopMotion;
			base.Finish();
		}

		public FsmGameObject Destination;

		public EntityOrientation FinalPlayerOrientation;
	}
}
