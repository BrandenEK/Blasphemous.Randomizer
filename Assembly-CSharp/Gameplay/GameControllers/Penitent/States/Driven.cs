using System;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Entities.StateMachine;
using Gameplay.GameControllers.Penitent.Abilities;

namespace Gameplay.GameControllers.Penitent.States
{
	public class Driven : State
	{
		public override void OnStateEnter()
		{
			base.OnStateEnter();
			this.StopCastAbilities();
			this.SetInvincible(true);
			Core.Logic.Penitent.PlatformCharacterInput.ResetInputs();
			Core.Logic.Penitent.PlatformCharacterInput.ResetActions();
		}

		public override void Update()
		{
			base.Update();
			if (!Core.Logic.Penitent.Status.IsGrounded)
			{
				return;
			}
			Core.Logic.Penitent.AnimatorInyector.enabled = false;
			Core.Input.SetBlocker("PLAYER_LOGIC", true);
		}

		public override void OnStateExit()
		{
			base.OnStateExit();
			Core.Input.SetBlocker("PLAYER_LOGIC", false);
			Core.Logic.Penitent.AnimatorInyector.enabled = true;
			this.SetInvincible(false);
		}

		private void StopCastAbilities()
		{
			Ability[] componentsInChildren = Core.Logic.Penitent.GetComponentsInChildren<Ability>();
			foreach (Ability ability in componentsInChildren)
			{
				if (!(ability is DrivePlayer))
				{
					ability.StopCast();
				}
			}
		}

		private void SetInvincible(bool enableDamageArea = true)
		{
			Core.Logic.Penitent.Status.Unattacable = enableDamageArea;
		}
	}
}
