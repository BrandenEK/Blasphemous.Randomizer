using System;
using System.Collections.Generic;
using System.Diagnostics;
using FMOD.Studio;
using Framework.Managers;
using Gameplay.GameControllers.Entities.Audio;
using Gameplay.GameControllers.Penitent;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.Isidora.Audio
{
	public class IsidoraAudio : EntityAudio
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<IsidoraAudio> OnBarBegins;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<IsidoraAudio> OnNextMarker;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<IsidoraAudio> OnAttackMarker;

		private void Awake()
		{
			this.bossAudioSync = UnityEngine.Object.FindObjectOfType<BossAudioSyncHelper>();
			if (this.bossAudioSync)
			{
				this.bossAudioSync.OnBar += this.NewBarBegins;
				this.bossAudioSync.OnMarker += this.NextMarker;
			}
			this.currentAudioPhase = IsidoraBehaviour.ISIDORA_PHASES.FIRST;
			this.Owner = base.GetComponent<Isidora>();
		}

		public float GetTimeSinceLevelLoad()
		{
			return this.timeSinceLevelLoad;
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			this.timeSinceLevelLoad += Time.deltaTime;
		}

		private void OnDestroy()
		{
			if (this.bossAudioSync)
			{
				this.bossAudioSync.OnBar -= this.NewBarBegins;
			}
		}

		private void NextMarker(string marker)
		{
			if (this.OnNextMarker != null)
			{
				this.OnNextMarker(this);
			}
			if (string.Equals(marker, "Attack"))
			{
				if (this.OnAttackMarker != null)
				{
					this.OnAttackMarker(this);
				}
				this.lastAttackMarker = this.timeSinceLevelLoad;
				this.lastAttackMarkerBar = this.bossAudioSync.LastBar;
			}
			if (string.Equals(marker, "Bridge"))
			{
				this.currentAudioPhase = IsidoraBehaviour.ISIDORA_PHASES.BRIDGE;
			}
			if (string.Equals(marker, "Phase3"))
			{
				this.currentAudioPhase = IsidoraBehaviour.ISIDORA_PHASES.SECOND;
			}
		}

		public void SetIsidoraVoice(bool on)
		{
			this.bossAudioSync.bossfightAudio.SetBossTrackParam("IsidoraVoice", (float)((!on) ? 0 : 1));
		}

		public bool GetIsidoraVoice()
		{
			float num;
			this.bossAudioSync.bossfightAudio.GetBossTrackParam("IsidoraVoice").getValue(out num);
			return num > 0f;
		}

		public void SetSkullsChoir(bool on)
		{
			this.bossAudioSync.bossfightAudio.SetBossTrackParam("SkullChoir", (float)((!on) ? 0 : 1));
		}

		public void SetPhaseBridge()
		{
			this.bossAudioSync.bossfightAudio.SetBossTrackParam("BossPhase", 2f);
		}

		public void SetSecondPhase()
		{
			this.bossAudioSync.bossfightAudio.SetBossTrackParam("BossPhase", 3f);
		}

		private void NewBarBegins()
		{
			this.lastTimeSpanBetweenBars = this.timeSinceLevelLoad - this.lastBarTime;
			this.lastBarTime = this.timeSinceLevelLoad;
			if (this.OnBarBegins != null)
			{
				this.OnBarBegins(this);
			}
		}

		public bool IsLastBarValid()
		{
			return this.bossAudioSync.LastBar % 2 == 1;
		}

		public float GetSingleBarDuration()
		{
			return 2.667f;
		}

		public float GetTimeLeftForCurrentBar()
		{
			return this.GetSingleBarDuration() - (this.timeSinceLevelLoad - this.lastBarTime);
		}

		public float GetTimeUntilNextValidBar()
		{
			float num = this.GetTimeLeftForCurrentBar();
			if (this.IsLastBarValid())
			{
				num += this.GetSingleBarDuration();
			}
			return num;
		}

		public float GetTimeUntilNextAttackAnticipationPeriod()
		{
			return this.lastAttackMarker + 2f * this.GetSingleBarDuration() - this.timeSinceLevelLoad;
		}

		internal void PlayFadeDash()
		{
			this.PlayOneShot_AUDIO("IsidoraFadeSlash", EntityAudio.FxSoundCategory.Attack);
		}

		public void PlayInvisibleDash()
		{
			this.PlayOneShot_AUDIO("IsidoraInvisibleDash", EntityAudio.FxSoundCategory.Attack);
		}

		public void StopMeleeAudios()
		{
			UnityEngine.Debug.Log("<color=red>STOP MELEE AUDIOS</color>");
			this.Stop_AUDIO("IsidoraSlashAnticipation");
			this.Stop_AUDIO("IsidoraSlashAttack");
		}

		public void PlayRisingScytheAnticipationLoopAudio()
		{
			Isidora isidora = this.Owner as Isidora;
			if (isidora.AnimatorInyector.IsScytheOnFire())
			{
				this.Play_AUDIO("IsidoraRisingScytheAnticipationFire");
			}
			else
			{
				this.Play_AUDIO("IsidoraRisingScytheAnticipationNoFire");
			}
		}

		public void PlayRisingScytheSlashAudio()
		{
			Isidora isidora = this.Owner as Isidora;
			if (isidora.AnimatorInyector.IsScytheOnFire())
			{
				this.Play_AUDIO("IsidoraRisingScytheSlashFire");
			}
			else
			{
				this.Play_AUDIO("IsidoraRisingScytheSlashNoFire");
			}
		}

		public void PlayOneShot_AUDIO(string eventId, EntityAudio.FxSoundCategory category = EntityAudio.FxSoundCategory.Attack)
		{
			base.PlayOneShotEvent(eventId, category);
		}

		public void Play_AUDIO(string eventId)
		{
			EventInstance value;
			if (this.eventRefsByEventId.TryGetValue(eventId, out value))
			{
				base.StopEvent(ref value);
				this.eventRefsByEventId.Remove(eventId);
			}
			value = default(EventInstance);
			base.PlayEvent(ref value, eventId, false);
			this.eventRefsByEventId[eventId] = value;
		}

		public void Stop_AUDIO(string eventId)
		{
			EventInstance eventInstance;
			if (!this.eventRefsByEventId.TryGetValue(eventId, out eventInstance))
			{
				return;
			}
			base.StopEvent(ref eventInstance);
			this.eventRefsByEventId.Remove(eventId);
		}

		public void StopAll()
		{
			Penitent penitent = Core.Logic.Penitent;
			penitent.OnDead = (Core.SimpleEvent)Delegate.Remove(penitent.OnDead, new Core.SimpleEvent(this.StopAll));
			foreach (string key in this.eventRefsByEventId.Keys)
			{
				EventInstance eventInstance = this.eventRefsByEventId[key];
				base.StopEvent(ref eventInstance);
			}
			this.eventRefsByEventId.Clear();
		}

		private const string ATTACK_MARKER = "Attack";

		private const string BRIDGE_MARKER = "Bridge";

		private const string PHASE_MARKER = "Phase3";

		public BossAudioSyncHelper bossAudioSync;

		public float lastTimeSpanBetweenBars;

		public float lastBarTime;

		public float lastAttackMarker;

		public int lastAttackMarkerBar;

		public IsidoraBehaviour.ISIDORA_PHASES currentAudioPhase;

		private Dictionary<string, EventInstance> eventRefsByEventId = new Dictionary<string, EventInstance>();

		private const string Isidora_InvisibleDash = "IsidoraInvisibleDash";

		private const string Isidora_SlashAnticipation = "IsidoraSlashAnticipation";

		private const string Isidora_SlashAttack = "IsidoraSlashAttack";

		private const string Isidora_RisingScytheAnticipationFire = "IsidoraRisingScytheAnticipationFire";

		private const string Isidora_RisingScytheAnticipationNoFire = "IsidoraRisingScytheAnticipationNoFire";

		private const string Isidora_RisingScytheSlashFire = "IsidoraRisingScytheSlashFire";

		private const string Isidora_RisingScytheSlashNoFire = "IsidoraRisingScytheSlashNoFire";

		private const string Isidora_FadeSlash = "IsidoraFadeSlash";

		private float timeSinceLevelLoad;
	}
}
