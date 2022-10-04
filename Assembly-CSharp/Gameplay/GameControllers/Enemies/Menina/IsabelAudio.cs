using System;
using Gameplay.GameControllers.Entities.Audio;

namespace Gameplay.GameControllers.Enemies.Menina
{
	public class IsabelAudio : MeninaAudio
	{
		public override void PlayDeath()
		{
			base.PlayOneShotEvent("IsabelDeath", EntityAudio.FxSoundCategory.Damage);
		}

		public override void PlayRightLeg()
		{
			base.PlayOneShotEvent("IsabelFootsteps", EntityAudio.FxSoundCategory.Motion);
		}

		public override void PlayIdle()
		{
			if (this.Owner.SpriteRenderer.isVisible)
			{
				base.PlayOneShotEvent("IsabelIdle", EntityAudio.FxSoundCategory.Motion);
			}
		}

		public override void PlayAttack()
		{
			base.PlayOneShotEvent("IsabelAttack", EntityAudio.FxSoundCategory.Attack);
		}

		private const string IdleEventKey = "IsabelIdle";

		private const string DeathEventKey = "IsabelDeath";

		private const string AttackEventKey = "IsabelAttack";

		private const string FootstepsEventKey = "IsabelFootsteps";
	}
}
