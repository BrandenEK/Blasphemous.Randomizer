using System;
using System.Collections;
using Framework.Inventory;
using Framework.Managers;
using Gameplay.GameControllers.Bosses.Quirce.Attack;
using Gameplay.GameControllers.Penitent;
using Gameplay.GameControllers.Penitent.Abilities;
using UnityEngine;

namespace Tools.Items
{
	public class PenitentLightBeamEffect : ObjectEffect
	{
		protected override bool OnApplyEffect()
		{
			Debug.Log("PENITENT VERTICAL BEAM EFFECT");
			this._owner = Core.Logic.Penitent;
			this._areaSummonAttack = this._owner.GetComponentInChildren<PrayerUse>().lightBeamPrayer;
			Core.Logic.CameraManager.ProCamera2DShake.ShakeUsingPreset("SimpleHit");
			BossAreaSummonAttack areaSummonAttack = this._areaSummonAttack;
			Vector3 position = this._areaSummonAttack.transform.position;
			float final = this._owner.Stats.PrayerStrengthMultiplier.Final;
			GameObject gameObject = areaSummonAttack.SummonAreaOnPoint(position, 0f, final, null);
			gameObject.GetComponent<BossSpawnedAreaAttack>().SetDamage(this.DamageAmount);
			base.StartCoroutine(this.VerticalBeamCoroutine());
			return base.OnApplyEffect();
		}

		private IEnumerator VerticalBeamCoroutine()
		{
			yield return new WaitForSeconds(0.4f);
			this.PushPlayerColor();
			yield return new WaitForSeconds(0.8f);
			this.PopPlayerColor();
			yield break;
		}

		private void PushPlayerColor()
		{
			this.oldMat = this._owner.SpriteRenderer.material;
			this._owner.SpriteRenderer.material = this.penitentBlueTintMaterial;
		}

		private void PopPlayerColor()
		{
			this._owner.SpriteRenderer.material = this.oldMat;
		}

		protected override void OnStart()
		{
			base.OnStart();
		}

		private void Update()
		{
		}

		protected override void OnRemoveEffect()
		{
			base.OnRemoveEffect();
		}

		private Penitent _owner;

		private BossAreaSummonAttack _areaSummonAttack;

		public Material penitentBlueTintMaterial;

		private Material oldMat;

		public int DamageAmount = 1;
	}
}
