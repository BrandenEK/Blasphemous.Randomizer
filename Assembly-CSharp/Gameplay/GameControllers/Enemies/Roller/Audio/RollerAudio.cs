using System;
using FMOD.Studio;
using Gameplay.GameControllers.Entities.Audio;

namespace Gameplay.GameControllers.Enemies.Roller.Audio
{
	public class RollerAudio : EntityAudio
	{
		public void PlayDeath()
		{
			base.PlayOneShotEvent("RangedRollerDeath", EntityAudio.FxSoundCategory.Damage);
		}

		public void PlayAttack()
		{
			base.PlayOneShotEvent("RangedRollerAttack", EntityAudio.FxSoundCategory.Attack);
		}

		public void PlayBreath()
		{
			if (!this.Owner.SpriteRenderer.isVisible)
			{
				return;
			}
			base.PlayOneShotEvent("RangedRollerBreath", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayScratch()
		{
			if (!this.Owner.SpriteRenderer.isVisible)
			{
				return;
			}
			base.PlayOneShotEvent("RangedRollerSand", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayPreAttack()
		{
			base.PlayOneShotEvent("RangedRollerPreAttack", EntityAudio.FxSoundCategory.Attack);
		}

		public void PlayReloadAttack()
		{
			if (!this.Owner.SpriteRenderer.isVisible)
			{
				return;
			}
			base.PlayOneShotEvent("RangedRollerReload", EntityAudio.FxSoundCategory.Attack);
		}

		public void PlayRolling()
		{
			if (this._disposeFlag)
			{
				this._rollEventInstance.stop(1);
				this._rollEventInstance.release();
				this._rollEventInstance = default(EventInstance);
				this._disposeFlag = false;
			}
			base.PlayEvent(ref this._rollEventInstance, "RangedRollerRoll", true);
		}

		public void StopRolling()
		{
			this._disposeFlag = true;
			if (!this._rollEventInstance.isValid())
			{
				return;
			}
			ParameterInstance parameterInstance;
			this._rollEventInstance.getParameter("End", ref parameterInstance);
			if (parameterInstance.isValid())
			{
				parameterInstance.setValue(1f);
			}
		}

		private const string AttackEventKey = "RangedRollerAttack";

		private const string BreathEventKey = "RangedRollerBreath";

		private const string ScratchEventKey = "RangedRollerSand";

		private const string DeathEventKey = "RangedRollerDeath";

		private const string ReloadEventKey = "RangedRollerReload";

		private const string RollEventKey = "RangedRollerRoll";

		private const string PreAttackEventKey = "RangedRollerPreAttack";

		private EventInstance _idleEventInstance;

		private EventInstance _rollEventInstance;

		private bool _disposeFlag;
	}
}
