using System;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Environment;
using UnityEngine;

namespace Gameplay.GameControllers.Effects.Player.Dust
{
	public class StepDustSpawner : MonoBehaviour
	{
		public StepDustRoot StepDustRoot { get; set; }

		public bool EntityRaiseDust { get; set; }

		public StepDust.StepDustType CurrentStepDustSpawn { get; set; }

		private void Start()
		{
			this.levelEffectsStore = Core.Logic.CurrentLevelConfig.LevelEffectsStore;
			this.CreateDustPool();
			if (this.Owner == null)
			{
				Debug.LogError("StepDust Spanwer needs an owner!");
			}
			else
			{
				this.StepDustRoot = this.Owner.GetComponentInChildren<StepDustRoot>();
			}
		}

		private void CreateDustPool()
		{
			byte b = 0;
			while ((int)b < this.stepsDust.Length)
			{
				PoolManager.Instance.CreatePool(this.stepsDust[(int)b].gameObject, 3);
				b += 1;
			}
		}

		private void OnDestroy()
		{
		}

		public StepDust GetStepDust(Vector2 stepDustPosition)
		{
			if (Core.GameModeManager.IsCurrentMode(GameModeManager.GAME_MODES.DEMAKE))
			{
				return null;
			}
			GameObject stepDustfromProperties = this.GetStepDustfromProperties(this.CurrentStepDustSpawn);
			StepDust component = PoolManager.Instance.ReuseObject(stepDustfromProperties, stepDustPosition, Quaternion.identity, false, 1).GameObject.GetComponent<StepDust>();
			component.Owner = this.Owner;
			component.transform.parent = this.levelEffectsStore.transform;
			component.SetSpriteOrientation(this.Owner.Status.Orientation);
			return component;
		}

		public void StoreStepDust(StepDust stepDust)
		{
			if (stepDust)
			{
				stepDust.gameObject.SetActive(false);
			}
		}

		private GameObject GetStepDustfromProperties(StepDust.StepDustType stepDustType)
		{
			StepDust stepDust = null;
			byte b = 0;
			while ((int)b < this.stepsDust.Length)
			{
				if (this.stepsDust[(int)b].stepDustType == stepDustType)
				{
					stepDust = this.stepsDust[(int)b];
				}
				b += 1;
			}
			return stepDust.gameObject;
		}

		private LevelEffectsStore levelEffectsStore;

		public Entity Owner;

		public StepDust[] stepsDust;

		private const int dustPoolSize = 3;
	}
}
