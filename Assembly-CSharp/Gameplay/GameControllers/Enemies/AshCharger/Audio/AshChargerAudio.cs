using System;
using FMOD.Studio;
using Gameplay.GameControllers.Entities.Audio;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.AshCharger.Audio
{
	public class AshChargerAudio : EntityAudio
	{
		protected override void OnStart()
		{
			base.OnStart();
			this.Owner.OnDeath += this.OnDeath;
		}

		public void Walk()
		{
			if (!this.Owner.SpriteRenderer.isVisible)
			{
				return;
			}
		}

		public void Death()
		{
		}

		public void PlayGuardHit()
		{
		}

		public void PlayAttack()
		{
			this.StopAttack();
		}

		public void StopAttack()
		{
			base.StopEvent(ref this._attackEventInstance);
		}

		public void SetAttackMoveParam(float value)
		{
			this.SetMoveParam(this._attackEventInstance, value);
		}

		private void SetMoveParam(EventInstance eventInstance, float value)
		{
			try
			{
				ParameterInstance parameterInstance;
				eventInstance.getParameter("Moves", out parameterInstance);
				parameterInstance.setValue(value);
			}
			catch (Exception ex)
			{
				Debug.LogWarning(ex.Message + ex.StackTrace);
			}
		}

		private void OnDeath()
		{
			this.Owner.OnDeath -= this.OnDeath;
			this.StopAttack();
		}

		public const string WalkStepEventKey = "FernandoFootsteps";

		public const string DeathEventKey = "FernandoDeath";

		public const string GuardEventKey = "FernandoBackHit";

		private const string AttackEventKey = "FernandoAttack";

		private const string MoveParameterKey = "Moves";

		private EventInstance _attackEventInstance;
	}
}
