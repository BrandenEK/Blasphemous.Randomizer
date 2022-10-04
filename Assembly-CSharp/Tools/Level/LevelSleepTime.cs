using System;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Entities;

namespace Tools.Level
{
	[Serializable]
	public class LevelSleepTime : PersistentInterface
	{
		public LevelSleepTime(float normal, float heavy, float critical)
		{
			this.Normal = normal;
			this.Heavy = heavy;
			this.Critical = critical;
		}

		public string GetPersistenID()
		{
			throw new NotImplementedException();
		}

		public PersistentManager.PersistentData GetCurrentPersistentState(string dataPath, bool fullSave)
		{
			throw new NotImplementedException();
		}

		public void SetCurrentPersistentState(PersistentManager.PersistentData data, bool isloading, string dataPath)
		{
			throw new NotImplementedException();
		}

		public void ResetPersistence()
		{
		}

		public int GetOrder()
		{
			return 0;
		}

		public float GetHitSleepTime(Hit hit)
		{
			float result = 0f;
			switch (hit.DamageType)
			{
			case DamageArea.DamageType.Normal:
				result = this.Normal;
				break;
			case DamageArea.DamageType.Heavy:
				result = this.Heavy;
				break;
			case DamageArea.DamageType.Critical:
				result = this.Critical;
				break;
			case DamageArea.DamageType.Simple:
				result = this.Normal;
				break;
			case DamageArea.DamageType.Stunt:
				result = this.Heavy;
				break;
			case DamageArea.DamageType.OptionalStunt:
				result = this.Heavy;
				break;
			}
			return result;
		}

		protected float Normal;

		protected float Heavy;

		protected float Critical;
	}
}
