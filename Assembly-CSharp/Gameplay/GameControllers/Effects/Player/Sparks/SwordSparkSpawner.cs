using System;
using System.Collections.Generic;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Environment;
using UnityEngine;

namespace Gameplay.GameControllers.Effects.Player.Sparks
{
	public class SwordSparkSpawner : Trait
	{
		public SwordSpark.SwordSparkType CurrentSwordSparkSpawningType { get; set; }

		private void AttackAreaPositioning()
		{
			Bounds bounds = this.AttackArea.bounds;
			base.transform.position = bounds.center;
		}

		protected SwordSpark GetCurrentSwordSpark(SwordSpark.SwordSparkType currentSwordSparkType)
		{
			SwordSpark result = null;
			if (this.SwordSparks.Length > 0)
			{
				byte b = 0;
				while ((int)b < this.SwordSparks.Length)
				{
					if (this.SwordSparks[(int)b].sparkType == currentSwordSparkType)
					{
						result = this.SwordSparks[(int)b];
						break;
					}
					b += 1;
				}
			}
			return result;
		}

		protected SwordSpark GetCurrentSwordSparkFromPool(SwordSpark.SwordSparkType currentSwordSparkType)
		{
			SwordSpark swordSpark = null;
			if (this._swordSparkPool.Count > 0)
			{
				for (int i = 0; i < this._swordSparkPool.Count; i++)
				{
					if (this._swordSparkPool[i].sparkType == currentSwordSparkType && !this._swordSparkPool[i].gameObject.activeSelf)
					{
						swordSpark = this._swordSparkPool[i];
						this._swordSparkPool.Remove(swordSpark);
						break;
					}
				}
			}
			return swordSpark;
		}

		protected override void OnStart()
		{
			this._levelEffectsStore = Core.Logic.CurrentLevelConfig.GetComponentInChildren<LevelEffectsStore>();
		}

		protected override void OnUpdate()
		{
			this.AttackAreaPositioning();
		}

		public SwordSpark GetSwordSpark(Vector3 position)
		{
			SwordSpark swordSpark;
			GameObject gameObject;
			if (this._swordSparkPool.Count > 0)
			{
				swordSpark = this.GetCurrentSwordSparkFromPool(this.CurrentSwordSparkSpawningType);
				if (swordSpark)
				{
					gameObject = swordSpark.gameObject;
					gameObject.SetActive(true);
					gameObject.transform.position = position;
				}
				else
				{
					swordSpark = this.GetCurrentSwordSpark(this.CurrentSwordSparkSpawningType);
					gameObject = Object.Instantiate<GameObject>(swordSpark.gameObject, position, Quaternion.identity);
				}
			}
			else
			{
				swordSpark = this.GetCurrentSwordSpark(this.CurrentSwordSparkSpawningType);
				if (!swordSpark)
				{
					return null;
				}
				gameObject = Object.Instantiate<GameObject>(swordSpark.gameObject, position, Quaternion.identity);
			}
			gameObject.transform.parent = this._levelEffectsStore.transform;
			return swordSpark;
		}

		public void StoreSwordSpark(SwordSpark swordSpark)
		{
			if (this._swordSparkPool != null)
			{
				swordSpark.gameObject.SetActive(false);
				this._swordSparkPool.Add(swordSpark);
			}
		}

		public void DrainSwordSparkPool()
		{
			if (this._swordSparkPool.Count > 0)
			{
				this._swordSparkPool.Clear();
			}
		}

		public BoxCollider2D AttackArea;

		private LevelEffectsStore _levelEffectsStore;

		public SwordSpark[] SwordSparks;

		private readonly List<SwordSpark> _swordSparkPool = new List<SwordSpark>();
	}
}
