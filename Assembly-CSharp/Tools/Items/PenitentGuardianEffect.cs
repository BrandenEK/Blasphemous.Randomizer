using System;
using Framework.Inventory;
using Framework.Managers;
using Gameplay.GameControllers.Effects.Player;
using Gameplay.GameControllers.Penitent;
using UnityEngine;

namespace Tools.Items
{
	public class PenitentGuardianEffect : ObjectEffect
	{
		protected override bool OnApplyEffect()
		{
			if (!this.InvulnerableEffect.IsApplied)
			{
				return base.OnApplyEffect();
			}
			this.InstantiateGuardian();
			Core.Logic.CameraManager.ProCamera2DShake.ShakeUsingPreset("SimpleHit");
			return base.OnApplyEffect();
		}

		protected override void OnAwake()
		{
			base.OnAwake();
			SpawnManager.OnPlayerSpawn += this.OnPlayerSpawn;
		}

		private void OnPlayerSpawn(Penitent penitent)
		{
			SpawnManager.OnPlayerSpawn -= this.OnPlayerSpawn;
			this._owner = penitent;
		}

		protected override void OnStart()
		{
			base.OnStart();
			if (this.InvulnerableEffect == null)
			{
				Debug.LogError("Invulnerable effect must be set!");
			}
		}

		private void Update()
		{
			if (!this.InvulnerableEffect || !this.InvulnerableEffect.IsApplied)
			{
				if (this._showLady)
				{
					this._showLady = false;
					Core.Logic.Penitent.DamageArea.PrayerProtectionEnabled = false;
					if (this._owner.Status.Invulnerable)
					{
						this._owner.Status.Invulnerable = false;
					}
				}
				return;
			}
			if (!this._owner.Status.Invulnerable)
			{
				this._showLady = true;
				Core.Logic.Penitent.DamageArea.PrayerProtectionEnabled = true;
				this._owner.Status.Invulnerable = true;
			}
		}

		protected override void OnRemoveEffect()
		{
			base.OnRemoveEffect();
		}

		public void InstantiateGuardian()
		{
			Vector3 position = Core.Logic.Penitent.transform.position;
			position.y += this.YOffset;
			if (this._penitentGuardian == null)
			{
				if (this.PenitentGuardianPrefab != null)
				{
					this._penitentGuardian = UnityEngine.Object.Instantiate<GameObject>(this.PenitentGuardianPrefab, position, Quaternion.identity);
					PenitentGuardian component = this._penitentGuardian.GetComponent<PenitentGuardian>();
					component.SetOrientation(this.currentHit);
				}
			}
			else
			{
				this._penitentGuardian.SetActive(true);
				PenitentGuardian component2 = this._penitentGuardian.GetComponent<PenitentGuardian>();
				if (component2.IsTriggered)
				{
					return;
				}
				component2.SetOrientation(this.currentHit);
				this._penitentGuardian.transform.position = position;
			}
		}

		public void DisableGuardian()
		{
			if (this._penitentGuardian.activeSelf)
			{
				this._penitentGuardian.SetActive(false);
			}
		}

		private GameObject _penitentGuardian;

		public GameObject PenitentGuardianPrefab;

		public ItemTemporalEffect InvulnerableEffect;

		public float YOffset = 1f;

		private Penitent _owner;

		private bool _showLady;
	}
}
