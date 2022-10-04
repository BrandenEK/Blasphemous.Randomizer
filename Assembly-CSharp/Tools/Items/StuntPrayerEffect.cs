using System;
using Framework.Inventory;
using Framework.Managers;
using Gameplay.GameControllers.Bosses.Quirce.Attack;
using Gameplay.GameControllers.Penitent;
using Gameplay.GameControllers.Penitent.Abilities;
using UnityEngine;

namespace Tools.Items
{
	public class StuntPrayerEffect : ObjectEffect
	{
		protected override bool OnApplyEffect()
		{
			Debug.Log("STUNT PRAYER EFFECT");
			this._owner = Core.Logic.Penitent;
			this._areaSummonAttack = this._owner.GetComponentInChildren<PrayerUse>().stuntPrayer;
			Core.Logic.CameraManager.ProCamera2DShake.ShakeUsingPreset("SimpleHit");
			BossAreaSummonAttack areaSummonAttack = this._areaSummonAttack;
			Vector3 position = this._areaSummonAttack.transform.position;
			float final = this._owner.Stats.PrayerStrengthMultiplier.Final;
			areaSummonAttack.SummonAreaOnPoint(position, 0f, final, null);
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
		private GameObject attack;

		private Penitent _owner;

		private BossAreaSummonAttack _areaSummonAttack;
	}
}
