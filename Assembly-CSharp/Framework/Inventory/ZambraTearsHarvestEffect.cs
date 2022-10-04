using System;
using System.Collections;
using Framework.Managers;
using Gameplay.GameControllers.Penitent;
using Gameplay.GameControllers.Penitent.Prayers;
using UnityEngine;

namespace Framework.Inventory
{
	public class ZambraTearsHarvestEffect : ObjectEffect
	{
		private ZambraTearsHarvestPrayer TearsHarvestPrayer { get; set; }

		private bool IsRunningEffect { get; set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			SpawnManager.OnPlayerSpawn += this.OnPlayerSpawn;
		}

		private void OnPlayerSpawn(Penitent penitent)
		{
			this.TearsHarvestPrayer = penitent.GetComponentInChildren<ZambraTearsHarvestPrayer>();
			SpawnManager.OnPlayerSpawn -= this.OnPlayerSpawn;
		}

		protected override bool OnApplyEffect()
		{
			if (!this.IsRunningEffect && this.TearsHarvestPrayer)
			{
				Debug.Log("ZAMBRA Effect Applied");
				this.IsRunningEffect = true;
				this.TearsHarvestPrayer.EnableEffect();
				base.StartCoroutine(this.DisableCoroutine(this.Duration));
			}
			return true;
		}

		private IEnumerator DisableCoroutine(float delay)
		{
			yield return new WaitForSeconds(delay);
			this.IsRunningEffect = false;
			if (this.TearsHarvestPrayer)
			{
				this.TearsHarvestPrayer.DisableEffect();
			}
			Debug.Log("ZAMBRA Effect disable");
			yield break;
		}

		protected override void OnRemoveEffect()
		{
			base.OnRemoveEffect();
		}

		public float Duration = 2f;
	}
}
