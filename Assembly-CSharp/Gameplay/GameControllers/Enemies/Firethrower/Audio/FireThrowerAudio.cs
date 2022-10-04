using System;
using FMOD.Studio;
using Framework.Managers;
using Gameplay.GameControllers.Entities.Audio;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Firethrower.Audio
{
	public class FireThrowerAudio : EntityAudio
	{
		protected override void OnStart()
		{
			base.OnStart();
			this.Owner.OnDeath += this.OnDeath;
		}

		public void PlayFireThrowing()
		{
			this.StopFireThrowing();
			base.PlayEvent(ref this._attackEventInstance, "FireThrowerAttack", true);
		}

		public void StopFireThrowing()
		{
			base.StopEvent(ref this._attackEventInstance);
		}

		public void PlayWalk()
		{
			if (this.Owner.SpriteRenderer.isVisible)
			{
				Core.Audio.PlayOneShotFromCatalog("FireThrowerFootsteps", this.Owner.transform.position);
			}
		}

		public void PlayDeath()
		{
			base.PlayOneShotEvent("FireThrowerDeath", EntityAudio.FxSoundCategory.Damage);
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
			this.StopFireThrowing();
		}

		private const string AttackEventKey = "FireThrowerAttack";

		private const string WalkEventKey = "FireThrowerFootsteps";

		private const string DeathEventKey = "FireThrowerDeath";

		private EventInstance _attackEventInstance;

		private const string MoveParameterKey = "Moves";
	}
}
