using System;
using FMOD.Studio;
using Gameplay.GameControllers.Entities.Audio;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Bishop.Audio
{
	public class BishopAudio : EntityAudio
	{
		protected override void OnStart()
		{
			base.OnStart();
			this.Owner.OnDamaged += this.OnDamagedEntity;
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			bool flag = this.Owner.Animator.GetCurrentAnimatorStateInfo(0).IsName("Idle");
			bool flag2 = this.Owner.Animator.GetCurrentAnimatorStateInfo(0).IsName("Chasing");
			if (flag)
			{
				if (this.Owner.SpriteRenderer.isVisible)
				{
					base.PlayEvent(ref this._floatingEventInstance, "BishopFloating", true);
					base.StopEvent(ref this._chasingEventInstance);
					base.UpdateEvent(ref this._floatingEventInstance);
				}
			}
			else
			{
				base.StopEvent(ref this._floatingEventInstance);
			}
			if (flag2)
			{
				base.PlayEvent(ref this._chasingEventInstance, "BishopChasing", true);
				base.StopEvent(ref this._floatingEventInstance);
				base.UpdateEvent(ref this._chasingEventInstance);
			}
			else
			{
				base.StopEvent(ref this._chasingEventInstance);
			}
			if (!this.Owner.SpriteRenderer.isVisible)
			{
				base.StopEvent(ref this._chasingEventInstance);
				base.StopEvent(ref this._floatingEventInstance);
			}
		}

		public void SetAttackParam(float value)
		{
			this.SetMoveParam(this._attackEventInstance, value);
		}

		private void OnDamagedEntity()
		{
			this.StopAttack();
		}

		public void PlayAttack()
		{
			base.PlayEvent(ref this._attackEventInstance, "BishopAttack", true);
		}

		public void StopAttack()
		{
			base.StopEvent(ref this._attackEventInstance);
		}

		public void StopAll()
		{
			this.StopAttack();
			base.StopEvent(ref this._chasingEventInstance);
			base.StopEvent(ref this._floatingEventInstance);
		}

		public void PlayDeath()
		{
			base.PlayOneShotEvent("BishopDeath", EntityAudio.FxSoundCategory.Damage);
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
			this.Owner.OnDamaged -= this.OnDamagedEntity;
			base.StopEvent(ref this._floatingEventInstance);
			base.StopEvent(ref this._chasingEventInstance);
		}

		private const string AttackEventKey = "BishopAttack";

		private const string FloatingEventKey = "BishopFloating";

		private const string DeathEventKey = "BishopDeath";

		private const string ChasingEventKey = "BishopChasing";

		private const string MoveParameterKey = "Moves";

		private EventInstance _chasingEventInstance;

		private EventInstance _floatingEventInstance;

		private EventInstance _attackEventInstance;
	}
}
