using System;
using Framework.Managers;
using Gameplay.GameControllers.Effects.Player;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Prayers
{
	public class PenitentGuardianHaloProtectionBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this._penitentGuardian == null)
			{
				this._penitentGuardian = animator.GetComponent<PenitentGuardian>();
			}
			this._penitentGuardian.IsTriggered = true;
			Core.Logic.Penitent.Audio.PrayerInvincibility();
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			this._penitentGuardian.IsTriggered = false;
			this._penitentGuardian.FadeOut();
			animator.gameObject.SetActive(false);
		}

		private PenitentGuardian _penitentGuardian;
	}
}
