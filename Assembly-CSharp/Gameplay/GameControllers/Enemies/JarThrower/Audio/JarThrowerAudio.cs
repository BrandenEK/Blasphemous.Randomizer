using System;
using Gameplay.GameControllers.Entities.Audio;

namespace Gameplay.GameControllers.Enemies.JarThrower.Audio
{
	public class JarThrowerAudio : EntityAudio
	{
		public void PlayJarExploding()
		{
			base.PlayOneShotEvent("BotijoExplode", EntityAudio.FxSoundCategory.Attack);
		}

		public void PlayThrowJar()
		{
			base.PlayOneShotEvent("BotijeraThrowBotijo", EntityAudio.FxSoundCategory.Attack);
		}

		public void PlayJump()
		{
			base.PlayOneShotEvent("BotijeraJump", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayLanding()
		{
			base.PlayOneShotEvent("BotijeraLanding", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayRun()
		{
			if (this.Owner.SpriteRenderer.isVisible)
			{
				base.PlayOneShotEvent("BotijeraRun", EntityAudio.FxSoundCategory.Motion);
			}
		}

		public void PlayWalk()
		{
			if (this.Owner.SpriteRenderer.isVisible)
			{
				base.PlayOneShotEvent("BotijeraFootsteps", EntityAudio.FxSoundCategory.Motion);
			}
		}

		public void PlayDeath()
		{
			base.PlayOneShotEvent("BotijeraDeath", EntityAudio.FxSoundCategory.Damage);
		}

		private const string JarExplodingEventKey = "BotijoExplode";

		private const string ThrowJarEventKey = "BotijeraThrowBotijo";

		private const string JumpEventKey = "BotijeraJump";

		private const string LandingEventKey = "BotijeraLanding";

		private const string RunEventKey = "BotijeraRun";

		private const string FootstepEventKey = "BotijeraFootsteps";

		private const string DeathEventKey = "BotijeraDeath";
	}
}
