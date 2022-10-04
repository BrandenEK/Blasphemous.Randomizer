using System;
using FMODUnity;
using Framework.Managers;
using Gameplay.GameControllers.Penitent;
using UnityEngine;

namespace Framework.Inventory
{
	public class ProtectionDomeEffect : ObjectEffect_Stat
	{
		protected override void OnAwake()
		{
			base.OnAwake();
			SpawnManager.OnPlayerSpawn += this.OnPlayerSpawn;
		}

		private void OnPlayerSpawn(Penitent penitent)
		{
			if (this.ProtectionDomePrefab)
			{
				PoolManager.Instance.CreatePool(this.ProtectionDomePrefab, 1);
			}
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			if (!this.protectionDomeInstance)
			{
				return;
			}
			Penitent penitent = Core.Logic.Penitent;
			if (!this.protectionDomeInstance.activeInHierarchy)
			{
				if (this.setInvulnerable)
				{
					this.setInvulnerable = false;
					penitent.Status.Invulnerable = this.setInvulnerable;
				}
			}
			else
			{
				if (Vector2.Distance(penitent.GetPosition(), this.protectionDomeInstance.transform.position) < this.MaxDistanceFromDome)
				{
					this.setInvulnerable = true;
				}
				else
				{
					this.setInvulnerable = false;
				}
				penitent.Status.Invulnerable = this.setInvulnerable;
			}
		}

		protected override bool OnApplyEffect()
		{
			this.InstantiateProtectionDome();
			return base.OnApplyEffect();
		}

		private void InstantiateProtectionDome()
		{
			if (!this.ProtectionDomePrefab || !Core.Logic.Penitent)
			{
				return;
			}
			Vector3 position = Core.Logic.Penitent.transform.position + Vector3.up * 1.5f;
			this.protectionDomeInstance = PoolManager.Instance.ReuseObject(this.ProtectionDomePrefab, position, Quaternion.identity, false, 1).GameObject;
			if (this.protectionDomeInstance == null)
			{
				return;
			}
			this.PlayAudioEffect();
		}

		private void PlayAudioEffect()
		{
			if (string.IsNullOrEmpty(this.InstantiateProtectionDomeFx))
			{
				return;
			}
			Core.Audio.PlaySfx(this.InstantiateProtectionDomeFx, 0f);
		}

		private void OnDestroy()
		{
			SpawnManager.OnPlayerSpawn -= this.OnPlayerSpawn;
		}

		public GameObject ProtectionDomePrefab;

		[EventRef]
		public string InstantiateProtectionDomeFx;

		public float MaxDistanceFromDome = 2f;

		private GameObject protectionDomeInstance;

		private bool setInvulnerable;
	}
}
