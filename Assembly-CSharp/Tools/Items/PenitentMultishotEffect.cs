using System;
using System.Collections;
using Framework.FrameworkCore;
using Framework.Inventory;
using Framework.Managers;
using Gameplay.GameControllers.Bosses.Quirce.Attack;
using Gameplay.GameControllers.Penitent;
using Gameplay.GameControllers.Penitent.Abilities;
using UnityEngine;

namespace Tools.Items
{
	public class PenitentMultishotEffect : ObjectEffect
	{
		protected override bool OnApplyEffect()
		{
			Debug.Log("PENITENT VERTICAL BEAM EFFECT");
			base.StartCoroutine(this.MultiShotCoroutine());
			return base.OnApplyEffect();
		}

		private float CalculateDamageStrength(float prayerStrMult)
		{
			return 1f + 0.35f * (prayerStrMult - 1f);
		}

		private IEnumerator MultiShotCoroutine()
		{
			this._owner = Core.Logic.Penitent;
			this._instantProjectileAttack = this._owner.GetComponentInChildren<PrayerUse>().multishotPrayer;
			this._instantProjectileAttack.SetDamageStrength(this.CalculateDamageStrength(this._owner.Stats.PrayerStrengthMultiplier.Final));
			this._instantProjectileAttack.SetDamage(this.DamageAmount);
			Vector2 dir = Vector2.right * (float)((this._owner.Status.Orientation != EntityOrientation.Right) ? -1 : 1);
			this._instantProjectileAttack.transform.localPosition = dir;
			Vector3 projectilePosition = this._instantProjectileAttack.transform.position;
			this._instantProjectileAttack.Shoot(projectilePosition, dir);
			yield return new WaitForSeconds(0.15f);
			float randomOff = UnityEngine.Random.Range(-1f, 1f) * 1f;
			this._instantProjectileAttack.Shoot(projectilePosition + Vector3.up * randomOff, dir);
			yield return new WaitForSeconds(0.15f);
			randomOff = UnityEngine.Random.Range(-1f, 1f) * 1f;
			this._instantProjectileAttack.Shoot(projectilePosition + Vector3.up * randomOff, dir);
			yield break;
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

		private BossInstantProjectileAttack _instantProjectileAttack;

		public int DamageAmount = 1;
	}
}
