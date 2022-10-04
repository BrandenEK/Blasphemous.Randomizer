using System;
using FMODUnity;
using Framework.Managers;
using Gameplay.GameControllers.Environment.AreaEffects;
using Gameplay.GameControllers.Penitent;
using UnityEngine;

namespace Framework.Inventory
{
	public class ToxicCloudEffect : ObjectEffect_Stat
	{
		private PoisonAreaEffect PoisonAreaEffect { get; set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			SpawnManager.OnPlayerSpawn += this.OnPlayerSpawn;
		}

		private void OnPlayerSpawn(Penitent penitent)
		{
			if (this.ToxicCloudPrefab)
			{
				PoolManager.Instance.CreatePool(this.ToxicCloudPrefab, 3);
			}
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			this._currentLapse += Time.deltaTime;
			if (this._currentLapse <= this.InstantiateLapse)
			{
				return;
			}
			this.InstantiateToxicCloud();
			this._currentLapse = 0f;
		}

		protected override bool OnApplyEffect()
		{
			this.InstantiateToxicCloud();
			this._currentLapse = 0f;
			return base.OnApplyEffect();
		}

		private void InstantiateToxicCloud()
		{
			if (!this.ToxicCloudPrefab || !Core.Logic.Penitent)
			{
				return;
			}
			Vector3 position = Core.Logic.Penitent.transform.position + this.OffsetInstantiationPosition;
			PoolManager.ObjectInstance objectInstance = PoolManager.Instance.ReuseObject(this.ToxicCloudPrefab, position, Quaternion.identity, false, 1);
			if (objectInstance == null)
			{
				return;
			}
			this.PoisonAreaEffect = objectInstance.GameObject.GetComponentInChildren<PoisonAreaEffect>();
			this.PoisonAreaEffect.DamageAmount = Core.Logic.Penitent.Stats.PrayerStrengthMultiplier.Final * this.DamageAmount;
			this.PlayAudioEffect();
		}

		private void PlayAudioEffect()
		{
			if (string.IsNullOrEmpty(this.InstantiateToxicCloudFx))
			{
				return;
			}
			Core.Audio.PlaySfx(this.InstantiateToxicCloudFx, 0f);
		}

		private void OnDestroy()
		{
			SpawnManager.OnPlayerSpawn -= this.OnPlayerSpawn;
		}

		public GameObject ToxicCloudPrefab;

		[EventRef]
		public string InstantiateToxicCloudFx;

		public float InstantiateLapse = 2f;

		public Vector2 OffsetInstantiationPosition;

		private float _currentLapse;

		public float DamageAmount = 1f;
	}
}
