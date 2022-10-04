using System;
using System.Collections;
using Framework.FrameworkCore;
using Framework.Inventory;
using Framework.Managers;
using Gameplay.GameControllers.Environment.Traps.FireTrap;
using Gameplay.GameControllers.Penitent;
using UnityEngine;

namespace Tools.Items
{
	public class PR203ElmFireLoopEffect : ObjectEffect
	{
		protected override void OnAwake()
		{
			SpawnManager.OnPlayerSpawn -= this.OnPlayerSpawn;
			SpawnManager.OnPlayerSpawn += this.OnPlayerSpawn;
		}

		private void OnPlayerSpawn(Penitent penitent)
		{
			PoolManager.ObjectInstance objectInstance = PoolManager.Instance.ReuseObject(this.ElmFireLoop, penitent.transform.position + Vector3.up * 1000f, Quaternion.identity, true, 1);
			ElmFireTrapManager componentInChildren = objectInstance.GameObject.GetComponentInChildren<ElmFireTrapManager>();
			componentInChildren.InstantHideElmFireTraps();
		}

		protected override bool OnApplyEffect()
		{
			Vector3 vector = Core.Logic.Penitent.GetPosition();
			vector += ((Core.Logic.Penitent.Status.Orientation != EntityOrientation.Left) ? (Vector3.right * 1.25f) : (Vector3.left * 1.25f));
			PoolManager.ObjectInstance objectInstance = PoolManager.Instance.ReuseObject(this.ElmFireLoop, vector, Quaternion.identity, true, 1);
			int num = (Core.Logic.Penitent.Status.Orientation != EntityOrientation.Left) ? 1 : -1;
			objectInstance.GameObject.transform.localScale = new Vector3((float)num, 1f, 1f);
			ElmFireTrapManager componentInChildren = objectInstance.GameObject.GetComponentInChildren<ElmFireTrapManager>();
			float final = Core.Logic.Penitent.Stats.PrayerStrengthMultiplier.Final;
			componentInChildren.RecursiveSetUpTrapDamage((float)this.BaseLightningDamage * final);
			base.StartCoroutine(this.ElmFireLoopCoroutine(componentInChildren));
			return true;
		}

		private IEnumerator ElmFireLoopCoroutine(ElmFireTrapManager m)
		{
			m.InstantHideElmFireTraps();
			m.ShowElmFireTrapRecursively(m.elmFireTrapNodes[0], this.WaitTimeToShowEachTrap, this.LightningChargeLapse, true);
			yield return new WaitForSeconds(this.WaitTimeToShowEachTrap * (float)this.numTraps);
			m.EnableTraps();
			int realNumPulses = this.NumPulses;
			float addition = Core.Logic.Penitent.Stats.PrayerDurationAddition.Final;
			if (addition > 0f)
			{
				realNumPulses += (int)(this.effectTimeToPulsesRatio * addition);
			}
			for (int i = 0; i < realNumPulses; i++)
			{
				m.elmFireTrapNodes[0].SetCurrentCycleCooldownToMax();
				yield return new WaitForSeconds(this.LightningChargeLapse * (float)this.numTraps);
				if (i < realNumPulses - 1)
				{
					yield return new WaitForSeconds(this.WaitTimeBetweenPulses);
				}
			}
			m.DisableTraps();
			m.HideElmFireTrapRecursively(m.elmFireTrapNodes[0], this.WaitTimeToHideEachTrap);
			yield return new WaitForSeconds(this.WaitTimeToHideEachTrap * (float)this.numTraps);
			yield break;
		}

		public int NumPulses = 3;

		public float WaitTimeToShowEachTrap = 0.1f;

		public float LightningChargeLapse = 0.05f;

		public float WaitTimeBetweenPulses = 0.1f;

		public float WaitTimeToHideEachTrap = 1f;

		public int BaseLightningDamage = 40;

		public GameObject ElmFireLoop;

		private readonly int numTraps = 7;

		private readonly float effectTimeToPulsesRatio = 0.6f;
	}
}
