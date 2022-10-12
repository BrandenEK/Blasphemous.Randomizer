using System;
using FMOD.Studio;
using Gameplay.GameControllers.Entities.Audio;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.MasterAnguish.Audio
{
	public class ElderBrotherAudio : EntityAudio
	{
		protected override void OnUpdate()
		{
			base.OnUpdate();
			if (this.Owner.Status.Dead)
			{
				this.StopAll();
			}
		}

		public void PlayDeath()
		{
			this.StopAll();
			base.PlayOneShotEvent("ElderBrotherDeath", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayJump()
		{
			this.StopAll();
			if (this.Owner.SpriteRenderer.isVisible)
			{
				base.PlayOneShotEvent("ElderBrotherJump", EntityAudio.FxSoundCategory.Motion);
			}
		}

		public void PlayDummyJump()
		{
			this.StopAll();
			base.PlayOneShotEvent("ElderBrotherJump", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayJumpLanding()
		{
			this.StopAll();
			base.PlayOneShotEvent("ElderBrotherLanding", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayAttack()
		{
			this.StopAttack();
			base.PlayEvent(ref this._attackEventInstance, "ElderBrotherAttack", false);
		}

		public void PlayAttackMove2()
		{
			this.SetAttackMoveParam(1f);
		}

		public void PlayAttackMove3()
		{
			this.SetAttackMoveParam(2f);
		}

		public void StopAttack()
		{
			base.StopEvent(ref this._attackEventInstance);
		}

		private void SetAttackMoveParam(float value)
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

		public void StopAll()
		{
			this.StopAttack();
		}

		private const string JumpKey = "ElderBrotherJump";

		private const string LandingKey = "ElderBrotherLanding";

		private const string DeathKey = "ElderBrotherDeath";

		private const string AttackEventKey = "ElderBrotherAttack";

		private const string MoveParameterKey = "Moves";

		private EventInstance _attackEventInstance;
	}
}
