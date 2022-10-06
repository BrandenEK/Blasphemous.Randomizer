using System;
using System.Collections.Generic;
using FMOD.Studio;
using Framework.Managers;
using Gameplay.GameControllers.Entities.Audio;
using Gameplay.GameControllers.Penitent;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.Amanecidas.Audio
{
	public class AmanecidasAudio : EntityAudio
	{
		public void PlayShield_AUDIO(float percentage)
		{
			this.Play_AUDIO("AmanecidasShield");
			this.SetShieldParam(this.eventRefsByEventId["AmanecidasShield"], percentage);
		}

		private void SetShieldParam(EventInstance eventInstance, float value)
		{
			try
			{
				ParameterInstance parameterInstance;
				eventInstance.getParameter("ShieldEnergy", ref parameterInstance);
				parameterInstance.setValue(value);
			}
			catch (Exception ex)
			{
				Debug.LogWarning(ex.Message + ex.StackTrace);
			}
		}

		public void PlayShieldDestroy_AUDIO()
		{
			base.PlayOneShotEvent("AmanecidasShieldDestroy", EntityAudio.FxSoundCategory.Attack);
		}

		public void PlayShieldRecharge_AUDIO()
		{
			base.PlayOneShotEvent("AmanecidasShieldRecharge", EntityAudio.FxSoundCategory.Attack);
		}

		public void PlayShieldExplosion_AUDIO()
		{
			base.PlayOneShotEvent("AmanecidasShieldExplosion", EntityAudio.FxSoundCategory.Attack);
		}

		public void PlayHeadExplosion_AUDIO()
		{
			base.PlayOneShotEvent("AmanecidasHeadExplosion", EntityAudio.FxSoundCategory.Attack);
		}

		public void PlayAttackCharge_AUDIO()
		{
			base.PlayOneShotEvent("AmanecidasAttackCharge", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayEnergyCharge_AUDIO()
		{
			base.PlayOneShotEvent("AmanecidasEnergyCharge", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayDeath_AUDIO()
		{
			base.PlayOneShotEvent("AmanecidasDeath", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayMoveFast_AUDIO()
		{
			base.PlayOneShotEvent("AmanecidasMoveFast", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayPain_AUDIO()
		{
			base.PlayOneShotEvent("AmanecidasPain", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayRecover_AUDIO()
		{
			base.PlayOneShotEvent("AmanecidasRecover", EntityAudio.FxSoundCategory.Motion);
		}

		public void StopShieldRecharge_AUDIO()
		{
			this.Stop_AUDIO("AmanecidasShieldRecharge");
		}

		public void PlayGroundPiston_AUDIO()
		{
			base.PlayOneShotEvent("GroundPiston", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayGroundAttack_AUDIO()
		{
			base.PlayOneShotEvent("AmanecidasGroundAttack", EntityAudio.FxSoundCategory.Attack);
		}

		public void PlayArrowCharge_AUDIO()
		{
			base.PlayOneShotEvent("AmanecidasNeedleCharge", EntityAudio.FxSoundCategory.Attack);
		}

		public void PlayArrowFire_AUDIO()
		{
			base.PlayOneShotEvent("AmanecidasNeedleFire", EntityAudio.FxSoundCategory.Attack);
		}

		public void PlayArrowFireFast_AUDIO()
		{
			base.PlayOneShotEvent("AmanecidasNeedleFireFast", EntityAudio.FxSoundCategory.Attack);
		}

		public void PlayHorizontalPreattack_AUDIO()
		{
			base.PlayOneShotEvent("AmanecidasHorizontalPreattack", EntityAudio.FxSoundCategory.Attack);
		}

		public void PlayVerticalPreattack_AUDIO()
		{
			base.PlayOneShotEvent("AmanecidasVerticalPreattack", EntityAudio.FxSoundCategory.Attack);
		}

		public void PlayLaserPreattack_AUDIO()
		{
			base.PlayOneShotEvent("AmanecidasLaserShotPreattack", EntityAudio.FxSoundCategory.Attack);
		}

		public void PlayArrowHitsGround_AUDIO()
		{
			base.PlayOneShotEvent("AmanecidasNeedleHitGround", EntityAudio.FxSoundCategory.Attack);
		}

		public void PlayRayFire_AUDIO()
		{
			base.PlayOneShotEvent("AmanecidasRayFire", EntityAudio.FxSoundCategory.Attack);
		}

		public void PlayAxeHitsGround_AUDIO()
		{
			base.PlayOneShotEvent("AmanecidasAxeHitGround", EntityAudio.FxSoundCategory.Attack);
		}

		public void PlayAxeThrowPreattack_AUDIO()
		{
			base.PlayOneShotEvent("AmanecidasAxeThrowPreattack", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayAxeThrow_AUDIO()
		{
			base.PlayOneShotEvent("AmanecidasAxeThrow", EntityAudio.FxSoundCategory.Attack);
		}

		public void PlayAxeAttack_AUDIO()
		{
			base.PlayOneShotEvent("AmanecidasAxeAttack", EntityAudio.FxSoundCategory.Attack);
		}

		public void PlayAxeSmashPreattack_AUDIO()
		{
			base.PlayOneShotEvent("AmanecidasAxeSmashPreattack", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayAxeSmash_AUDIO()
		{
			base.PlayOneShotEvent("AmanecidasAxeSmash", EntityAudio.FxSoundCategory.Attack);
		}

		public void PlaySwordDashPreattack_AUDIO()
		{
			base.PlayOneShotEvent("AmanecidasSwordDashPreattack", EntityAudio.FxSoundCategory.Attack);
		}

		public void PlaySwordDash_AUDIO()
		{
			base.PlayOneShotEvent("AmanecidasSwordDash", EntityAudio.FxSoundCategory.Attack);
		}

		public void PlaySwordPreattack_AUDIO()
		{
			base.PlayOneShotEvent("AmanecidasSwordPreattack", EntityAudio.FxSoundCategory.Attack);
		}

		public void PlaySwordAttack_AUDIO()
		{
			base.PlayOneShotEvent("AmanecidasSwordAttack", EntityAudio.FxSoundCategory.Attack);
		}

		public void PlaySwordFirePreattack_AUDIO()
		{
			base.PlayOneShotEvent("AmanecidasSwordFirePreattack", EntityAudio.FxSoundCategory.Attack);
		}

		public void PlayBeamDashPreattack_AUDIO()
		{
			base.PlayOneShotEvent("RayDashPreattack", EntityAudio.FxSoundCategory.Attack);
		}

		public void PlayDashCharge_AUDIO()
		{
			this.Play_AUDIO("AmanecidasDashCharge");
		}

		public void StopDashCharge_AUDIO()
		{
			this.Stop_AUDIO("AmanecidasDashCharge");
		}

		public void PlayLaudesChangeWeapon_AUDIO()
		{
			base.PlayOneShotEvent("LaudesChangeWeapon", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayOneShot_AUDIO(string eventId, EntityAudio.FxSoundCategory category)
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

		private Dictionary<string, EventInstance> eventRefsByEventId = new Dictionary<string, EventInstance>();

		private const string Amanecidas_SHIELD = "AmanecidasShield";

		private const string Amanecidas_SHIELD_DESTROY = "AmanecidasShieldDestroy";

		private const string Amanecidas_SHIELD_RECHARGE = "AmanecidasShieldRecharge";

		private const string Amanecidas_SHIELD_EXPLOSION = "AmanecidasShieldExplosion";

		private const string Amanecidas_HEAD_EXPLOSION = "AmanecidasHeadExplosion";

		private const string Amanecidas_ATTACK_CHARGE = "AmanecidasAttackCharge";

		private const string Amanecidas_ENERGY_CHARGE = "AmanecidasEnergyCharge";

		private const string Amanecidas_DEATH = "AmanecidasDeath";

		private const string Amanecidas_MOVE_FAST = "AmanecidasMoveFast";

		private const string Amanecidas_TELEPORT_IN = "AmanecidasTeleportIn";

		private const string Amanecidas_TELEPORT_OUT = "AmanecidasTeleportOut";

		private const string Amanecidas_TURN = "AmanecidasTurn";

		private const string Amanecidas_PAIN = "AmanecidasPain";

		private const string Amanecidas_RECOVER = "AmanecidasRecover";

		private const string Amanecidas_RECHARGE = "AmanecidasRecharge";

		private const string Amanecidas_WEAPON_SPAWN = "AmanecidasWeaponSpawn";

		private const string Amanecidas_TORNADO = "AmanecidasTornado";

		private const string Amanecidas_GROUND_PISTON = "GroundPiston";

		private const string Amanecidas_GROUND_ATTACK = "AmanecidasGroundAttack";

		private const string Amanecidas_FIRE_SHOT = "AmanecidasFireShot";

		private const string Amanecidas_ARROW_CHARGE = "AmanecidasNeedleCharge";

		private const string Amanecidas_ARROW_FIRE = "AmanecidasNeedleFire";

		private const string Amanecidas_ARROW_FIRE_FAST = "AmanecidasNeedleFireFast";

		private const string Amanecidas_ARROW_HOR_PREATTACK = "AmanecidasHorizontalPreattack";

		private const string Amanecidas_ARROW_VER_PREATTACK = "AmanecidasVerticalPreattack";

		private const string Amanecidas_ARROW_LASER_PREATTACK = "AmanecidasLaserShotPreattack";

		private const string Amanecidas_ARROWS_HITS_GROUND = "AmanecidasNeedleHitGround";

		private const string Amanecidas_RAY_FIRE = "AmanecidasRayFire";

		private const string Amanecidas_SMALL_RAY_FIRE = "AmanecidasSmallRayFire";

		private const string Amanecidas_AXE_BACK = "AmanecidasAxeBack";

		private const string Amanecidas_AXE_THROW_PREATTACK = "AmanecidasAxeThrowPreattack";

		private const string Amanecidas_AXE_THROW = "AmanecidasAxeThrow";

		private const string Amanecidas_AXE_ATTACK = "AmanecidasAxeAttack";

		private const string Amanecidas_AXE_SMASH_PREATTACK = "AmanecidasAxeSmashPreattack";

		private const string Amanecidas_AXE_SMASH = "AmanecidasAxeSmash";

		private const string Amanecidas_AXE_HITS_GROUND = "AmanecidasAxeHitGround";

		private const string Amanecidas_AXE_SPIN = "AmanecidasAxeSpin";

		private const string Amanecidas_SWORD_DASH_PREATTACK = "AmanecidasSwordDashPreattack";

		private const string Amanecidas_SWORD_DASH = "AmanecidasSwordDash";

		private const string Amanecidas_SWORD_PREATTACK = "AmanecidasSwordPreattack";

		private const string Amanecidas_SWORD_ATTACK = "AmanecidasSwordAttack";

		private const string Amanecidas_SWORD_FIRE_PREATTACK = "AmanecidasSwordFirePreattack";

		private const string Amanecidas_SWORD_SPIN_PROJECTILE = "AmanecidasSpinProyectile";

		private const string Amanecidas_LANCE_DASH_PREATTACK = "AmanecidasLanceDashPreattack";

		private const string Amanecidas_LANCE_SPIKE_APPEAR = "SpikeAppear";

		private const string Amanecidas_LANCE_HITS_GROUND = "SpikeHitGround";

		private const string Amanecidas_LANCE_SPIKE_DESTROY = "SpikeDestroy";

		private const string Amanecidas_RAY_DASH_PREATTACK = "RayDashPreattack";

		private const string Amanecidas_DASH_CHARGE = "AmanecidasDashCharge";

		private const string Amanecidas_LAUDES_CHANGE_WEAPON = "LaudesChangeWeapon";

		private const string shieldDamageKey = "ShieldEnergy";
	}
}
