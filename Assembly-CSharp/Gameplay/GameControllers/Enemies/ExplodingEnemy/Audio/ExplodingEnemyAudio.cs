using System;
using Gameplay.GameControllers.Entities.Audio;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.ExplodingEnemy.Audio
{
	public class ExplodingEnemyAudio : EntityAudio
	{
		public void Appear()
		{
			bool flag = false;
			if (Camera.main != null)
			{
				Vector3 vector = Camera.main.WorldToViewportPoint(this.Owner.transform.position);
				vector.x = Mathf.Clamp01(vector.x);
				vector.y = Mathf.Clamp01(vector.y);
				flag = (vector.x > 0f && vector.x < 1f && vector.y > 0f && vector.y < 1f);
			}
			if (flag)
			{
				base.PlayOneShotEvent("ExplodingEnemyAppear", EntityAudio.FxSoundCategory.Motion);
			}
		}

		public void PlayDeath()
		{
			if (!this.Owner.SpriteRenderer.isVisible)
			{
				return;
			}
			base.PlayOneShotEvent("ExplodingEnemyDeath", EntityAudio.FxSoundCategory.Damage);
		}

		public void PlayDeathExplosion()
		{
			if (!this.Owner.SpriteRenderer.isVisible)
			{
				return;
			}
			base.PlayOneShotEvent("ExplodingEnemyExplode", EntityAudio.FxSoundCategory.Damage);
		}

		public void PlayWalk()
		{
			if (!this.Owner.SpriteRenderer.isVisible)
			{
				return;
			}
			base.PlayOneShotEvent("ExplodingEnemyWalk", EntityAudio.FxSoundCategory.Motion);
		}

		private const string AppearEventKey = "ExplodingEnemyAppear";

		private const string WalkEventKey = "ExplodingEnemyWalk";

		private const string DeathEventKey = "ExplodingEnemyDeath";

		private const string ExplodingEventKey = "ExplodingEnemyExplode";
	}
}
