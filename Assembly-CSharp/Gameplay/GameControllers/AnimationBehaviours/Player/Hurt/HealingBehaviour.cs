using System;
using Framework.Managers;
using Gameplay.GameControllers.Penitent;
using Gameplay.GameControllers.Penitent.Abilities;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Player.Hurt
{
	public class HealingBehaviour : StateMachineBehaviour
	{
		private Healing HealingAbility { get; set; }

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this._penitent == null)
			{
				this._penitent = Core.Logic.Penitent;
			}
			if (this.HealingAbility == null)
			{
				this.HealingAbility = this._penitent.GetComponentInChildren<Healing>();
			}
			Core.Input.SetBlocker("PLAYER_LOGIC", true);
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			Core.Input.SetBlocker("PLAYER_LOGIC", false);
			this.HealingAbility.StopHeal();
		}

		private Penitent _penitent;
	}
}
