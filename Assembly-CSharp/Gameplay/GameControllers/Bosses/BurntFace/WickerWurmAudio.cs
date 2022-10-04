using System;
using FMOD.Studio;
using Gameplay.GameControllers.Entities.Audio;

namespace Gameplay.GameControllers.Bosses.BurntFace
{
	public class WickerWurmAudio : EntityAudio
	{
		public void PlayDeath_AUDIO()
		{
			this.StopAlive_AUDIO();
			base.PlayOneShotEvent("BlindBabyDeath", EntityAudio.FxSoundCategory.Damage);
		}

		public void PlayFire_AUDIO()
		{
			base.PlayEvent(ref this._fireEventInstance, "FireLoop", true);
		}

		public void PlayBabyAppear_AUDIO()
		{
			base.PlayOneShotEvent("BabyAppear", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayBite_AUDIO()
		{
			base.PlayOneShotEvent("BlindBabyWurmBite", EntityAudio.FxSoundCategory.Damage);
		}

		public void PlayScorpion1_AUDIO()
		{
			base.PlayOneShotEvent("Scorpion1", EntityAudio.FxSoundCategory.Damage);
		}

		public void PlayScorpion2_AUDIO()
		{
			base.PlayOneShotEvent("Scorpion2", EntityAudio.FxSoundCategory.Damage);
		}

		public void PlayScorpionHit_AUDIO()
		{
			base.PlayOneShotEvent("ScorpionHit", EntityAudio.FxSoundCategory.Damage);
		}

		public void PlayBabyGrab_AUDIO()
		{
			base.PlayOneShotEvent("BlindBabyGrab", EntityAudio.FxSoundCategory.Damage);
		}

		public void PlayCry_AUDIO()
		{
			base.PlayOneShotEvent("BlindBabyCry", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayHit_AUDIO()
		{
			base.PlayOneShotEvent("BlindBabyWurmHit", EntityAudio.FxSoundCategory.Damage);
		}

		public void PlaySnakeLongMove_AUDIO()
		{
			base.PlayOneShotEvent("BlindBabyWurmLongMove", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlaySnakeMove_AUDIO()
		{
			base.PlayOneShotEvent("BlindBabyWurmMove", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayPreAttack_AUDIO()
		{
			base.PlayOneShotEvent("BlindBabyWurmPreAttack", EntityAudio.FxSoundCategory.Attack);
		}

		public void PlayAttack_AUDIO()
		{
			base.PlayOneShotEvent("BlindBabyWurmAttack", EntityAudio.FxSoundCategory.Attack);
		}

		public void StopAlive_AUDIO()
		{
			base.StopEvent(ref this._aliveEventInstance);
		}

		public void PlayAlive_AUDIO()
		{
			base.StopEvent(ref this._aliveEventInstance);
			base.PlayEvent(ref this._aliveEventInstance, "BlindBabyWurmAlive", true);
		}

		public void StopAll()
		{
			base.StopEvent(ref this._aliveEventInstance);
			base.StopEvent(ref this._fireEventInstance);
		}

		private void OnDestroy()
		{
			this.StopAll();
		}

		private const string DeathEventKey = "BlindBabyDeath";

		private const string CryEventKey = "BlindBabyCry";

		private const string WurmFastMoveKey = "BlindBabyWurmMove";

		private const string WurmLongMoveKey = "BlindBabyWurmLongMove";

		private const string WurmAttackKey = "BlindBabyWurmAttack";

		private const string WurmPreAttackKey = "BlindBabyWurmPreAttack";

		private const string WurmAliveKey = "BlindBabyWurmAlive";

		private const string FireKey = "FireLoop";

		private const string WurmHitKey = "BlindBabyWurmHit";

		private const string EndParamKey = "End";

		private const string WurmBiteKey = "BlindBabyWurmBite";

		private const string GrabKey = "BlindBabyGrab";

		private const string Scorpion1Key = "Scorpion1";

		private const string Scorpion2Key = "Scorpion2";

		private const string ScorpionHitKey = "ScorpionHit";

		private const string AppearKey = "BabyAppear";

		private EventInstance _aliveEventInstance;

		private EventInstance _fireEventInstance;
	}
}
