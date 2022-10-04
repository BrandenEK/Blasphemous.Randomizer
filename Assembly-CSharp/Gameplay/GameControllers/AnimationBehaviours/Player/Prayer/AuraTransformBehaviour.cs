using System;
using Framework.Managers;
using Gameplay.GameControllers.Penitent;
using Gameplay.GameControllers.Penitent.Abilities;
using Rewired;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Player.Prayer
{
	public class AuraTransformBehaviour : StateMachineBehaviour
	{
		private protected Player Rewired { protected get; private set; }

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this._penitent == null)
			{
				this._penitent = Core.Logic.Penitent;
				this._prayer = this._penitent.GetComponentInChildren<PrayerUse>();
				this.Rewired = ReInput.players.GetPlayer(0);
			}
			Core.Input.SetBlocker("PLAYER_LOGIC", true);
			this._cancel = false;
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			Core.Input.SetBlocker("PLAYER_LOGIC", false);
			this._penitent.Audio.StopPrayerCast();
		}

		private Penitent _penitent;

		private PrayerUse _prayer;

		private bool _cancel;
	}
}
