using System;
using FMOD.Studio;
using Gameplay.GameControllers.Entities.Audio;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.CrossCrawler.Audio
{
	public class CrossCrawlerAudio : EntityAudio
	{
		protected override void OnUpdate()
		{
			base.OnUpdate();
			if (!this.Owner.SpriteRenderer.isVisible || this.Owner.Status.Dead)
			{
				this.StopAll();
			}
		}

		public void PlayWalk()
		{
			base.PlayEvent(ref this._walkEventInstance, "CrossCrawlerWalk", true);
		}

		public void StopWalk()
		{
			base.StopEvent(ref this._walkEventInstance);
		}

		public void PlayAttack()
		{
			base.PlayEvent(ref this._attackEventInstance, "CrossCrawlerAttack", true);
		}

		public void StopAttack()
		{
			base.StopEvent(ref this._attackEventInstance);
		}

		public void PlayTurnAround()
		{
			this.StopAll();
			base.PlayEvent(ref this._turnEventInstance, "CrossCrawlerTurn", true);
		}

		public void StopTurnAround()
		{
			base.StopEvent(ref this._turnEventInstance);
		}

		public void StopAll()
		{
			this.StopAttack();
			this.StopWalk();
			this.StopTurnAround();
		}

		public void PlayDeath()
		{
			this.StopAll();
			base.PlayOneShotEvent("CrossCrawlerDeath", EntityAudio.FxSoundCategory.Damage);
		}

		public void SetAttackMoveParam(float value)
		{
			this.SetMoveParam(this._attackEventInstance, value);
		}

		public void SetTurnMoveParam(float value)
		{
			this.SetMoveParam(this._turnEventInstance, value);
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

		private void OnDestroy()
		{
			this.StopWalk();
		}

		private const string WalkEventKey = "CrossCrawlerWalk";

		private const string DeathEventKey = "CrossCrawlerDeath";

		private const string AttackEventKey = "CrossCrawlerAttack";

		private const string TurnEventKey = "CrossCrawlerTurn";

		private EventInstance _walkEventInstance;

		private EventInstance _turnEventInstance;

		private EventInstance _chasingEventInstance;

		private EventInstance _attackEventInstance;

		private const string MoveParameterKey = "Moves";
	}
}
