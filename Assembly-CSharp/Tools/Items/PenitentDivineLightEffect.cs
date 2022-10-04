using System;
using Framework.Inventory;
using Framework.Managers;
using Gameplay.GameControllers.Bosses.Quirce.Attack;
using Gameplay.GameControllers.Penitent;
using Gameplay.GameControllers.Penitent.Abilities;
using UnityEngine;

namespace Tools.Items
{
	public class PenitentDivineLightEffect : ObjectEffect
	{
		protected override bool OnApplyEffect()
		{
			Debug.Log("PENITENT DIVINE LIGHT EFFECT");
			this._owner = Core.Logic.Penitent;
			this._areaSummonAttack = this._owner.GetComponentInChildren<PrayerUse>().divineLightPrayer;
			this._areaSummonAttack.SetDamageStrength(this._owner.Stats.PrayerStrengthMultiplier.Final);
			this._areaSummonAttack.SummonAreas(Vector2.right);
			this._areaSummonAttack.SummonAreas(Vector2.left);
			Core.Logic.CameraManager.ProCamera2DShake.ShakeUsingPreset("SimpleHit");
			return base.OnApplyEffect();
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

		[SerializeField]
		private int beansCount = 4;

		[SerializeField]
		private float randMaxX = 10f;

		[SerializeField]
		private float randMaxY = 5f;

		public int DamageAmount = 1;

		private Penitent _owner;

		private BossAreaSummonAttack _areaSummonAttack;
	}
}
