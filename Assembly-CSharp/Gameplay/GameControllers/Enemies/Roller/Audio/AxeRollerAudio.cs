using System;
using FMOD.Studio;
using Gameplay.GameControllers.Entities.Audio;

namespace Gameplay.GameControllers.Enemies.Roller.Audio
{
	public class AxeRollerAudio : EntityAudio
	{
		public void PlayDeath()
		{
			base.PlayOneShotEvent("AxeRollerDeath", EntityAudio.FxSoundCategory.Damage);
		}

		public void PlayAttack()
		{
			base.PlayOneShotEvent("AxeRollerAttack", EntityAudio.FxSoundCategory.Attack);
		}

		public void PlayBreath()
		{
			if (!this.Owner.SpriteRenderer.isVisible)
			{
				return;
			}
			base.PlayOneShotEvent("AxeRollerBreath", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayScratch()
		{
			if (!this.Owner.SpriteRenderer.isVisible)
			{
				return;
			}
			base.PlayOneShotEvent("AxeRollerSand", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayPreAttack()
		{
			base.PlayOneShotEvent("AxeRollerPreAttack", EntityAudio.FxSoundCategory.Attack);
		}

		public void PlayReloadAttack()
		{
			if (!this.Owner.SpriteRenderer.isVisible)
			{
				return;
			}
			base.PlayOneShotEvent("AxeRollerReload", EntityAudio.FxSoundCategory.Attack);
		}

		public void PlayRolling()
		{
			if (this._disposeFlag)
			{
				this._rollEventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
				this._rollEventInstance.release();
				this._rollEventInstance = default(EventInstance);
				this._disposeFlag = false;
			}
			base.PlayEvent(ref this._rollEventInstance, "AxeRollerRoll", true);
		}

		public void StopRolling()
		{
			this._disposeFlag = true;
			ParameterInstance parameterInstance;
			this._rollEventInstance.getParameter("End", out parameterInstance);
			if (parameterInstance.isValid())
			{
				parameterInstance.setValue(1f);
			}
		}

		public void PlayAnticipateRoll()
		{
			base.PlayOneShotEvent("AxeRollerPreRoll", EntityAudio.FxSoundCategory.Attack);
		}

		private const string AttackEventKey = "AxeRollerAttack";

		private const string BreathEventKey = "AxeRollerBreath";

		private const string ScratchEventKey = "AxeRollerSand";

		private const string DeathEventKey = "AxeRollerDeath";

		private const string ReloadEventKey = "AxeRollerReload";

		private const string RollEventKey = "AxeRollerRoll";

		private const string PreAttackEventKey = "AxeRollerPreAttack";

		private const string PreRollEventKey = "AxeRollerPreRoll";

		private EventInstance _idleEventInstance;

		private EventInstance _rollEventInstance;

		private bool _disposeFlag;
	}
}
