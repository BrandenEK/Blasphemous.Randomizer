using System;
using FMOD.Studio;
using Gameplay.GameControllers.Entities.Audio;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Nun.Audio
{
	public class NunAudio : EntityAudio
	{
		protected override void OnUpdate()
		{
			base.OnUpdate();
			if (this.Owner.SpriteRenderer.isVisible && !this.Owner.Status.Dead)
			{
				this.PlayBotafumeiro();
				this.UpdateBotafumeiro();
			}
			else
			{
				this.StopBotafumeiro();
			}
		}

		public void PlayWalk()
		{
			base.PlayOneShotEvent("NunWalk", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayAttack()
		{
			base.PlayEvent(ref this._attackEventInstance, "NunAttack", true);
		}

		public void StopAttack()
		{
			base.StopEvent(ref this._attackEventInstance);
		}

		public void PlayTurnAround()
		{
			base.PlayEvent(ref this._turnEventInstance, "NunTurn", true);
		}

		public void StopTurnAround()
		{
			base.StopEvent(ref this._turnEventInstance);
		}

		public void PlayBotafumeiro()
		{
			base.PlayEvent(ref this._botafumeiroEventInstance, "NunBotafumeiro", true);
		}

		public void StopBotafumeiro()
		{
			base.StopEvent(ref this._botafumeiroEventInstance);
		}

		public void UpdateBotafumeiro()
		{
			base.UpdateEvent(ref this._botafumeiroEventInstance);
		}

		public void PlayDeath()
		{
			base.PlayOneShotEvent("NunDeath", EntityAudio.FxSoundCategory.Damage);
			if (!this._attackEventInstance.isValid())
			{
				return;
			}
			this._attackEventInstance.stop(1);
			this._attackEventInstance.release();
		}

		public void SetAttackParam(float value)
		{
			this.SetMoveParam(this._attackEventInstance, value);
		}

		public void SetMoveParam(EventInstance eventInstance, float value)
		{
			try
			{
				ParameterInstance parameterInstance;
				eventInstance.getParameter("Moves", ref parameterInstance);
				parameterInstance.setValue(value);
			}
			catch (Exception ex)
			{
				Debug.LogWarning(ex.Message + ex.StackTrace);
			}
		}

		private void OnDestroy()
		{
			this.StopBotafumeiro();
		}

		private const string WalkEventKey = "NunWalk";

		private const string DeathEventKey = "NunDeath";

		private const string AttackEventKey = "NunAttack";

		private const string TurnEventKey = "NunTurn";

		private const string OilPuddleEventKey = "OilPuddle";

		private const string VanishOilPuddleEventKey = "OilDisappear";

		private const string NunBotafumeiroEventKey = "NunBotafumeiro";

		private EventInstance _walkEventInstance;

		private EventInstance _turnEventInstance;

		private EventInstance _chasingEventInstance;

		private EventInstance _attackEventInstance;

		private EventInstance _floatingEventInstance;

		private EventInstance _botafumeiroEventInstance;

		private const string MoveParameterKey = "Moves";
	}
}
