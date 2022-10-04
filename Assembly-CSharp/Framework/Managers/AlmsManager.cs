using System;
using Framework.FrameworkCore;
using Tools.DataContainer;
using UnityEngine;

namespace Framework.Managers
{
	public class AlmsManager : GameSystem, PersistentInterface
	{
		public int TearsGiven
		{
			get
			{
				return this._currentTears;
			}
			private set
			{
				this._currentTears = value;
				this.CurentTier = 0;
				foreach (int num in this.Config.GetTearsList())
				{
					if (this._currentTears < num)
					{
						break;
					}
					this.CurentTier++;
				}
			}
		}

		public int CurentTier { get; private set; }

		public AlmsConfigData Config { get; private set; }

		public override void Start()
		{
			this.Config = Resources.Load<AlmsConfigData>("AlmsConfig");
			this.ResetAll();
		}

		public override void AllInitialized()
		{
			Core.Persistence.AddPersistentManager(this);
		}

		public bool IsMaxTier()
		{
			return this.CurentTier >= this.Config.TearsToUnlock.Length;
		}

		public bool CanConsumeTears(int tears)
		{
			return Core.Logic.Penitent.Stats.Purge.Current >= (float)tears;
		}

		public bool ConsumeTears(int tears)
		{
			int curentTier = this.CurentTier;
			if (this.CanConsumeTears(tears))
			{
				Core.Logic.Penitent.Stats.Purge.Current -= (float)tears;
				this.TearsGiven += tears;
			}
			bool flag = curentTier != this.CurentTier;
			if (flag && curentTier < 7 && this.CurentTier >= 7)
			{
				Core.ColorPaletteManager.UnlockAlmsColorPalette();
			}
			return flag;
		}

		public int GetPrieDieuLevel()
		{
			int result = 1;
			if (this.CurentTier >= this.Config.Level3PrieDieus)
			{
				result = 3;
			}
			else if (this.CurentTier >= this.Config.Level2PrieDieus)
			{
				result = 2;
			}
			return result;
		}

		public int GetAltarLevel()
		{
			return this.CurentTier + 1;
		}

		public void DEBUG_SetTearsGiven(int tears)
		{
			this.TearsGiven = tears;
		}

		private void ResetAll()
		{
			this.TearsGiven = 0;
		}

		public string GetPersistenID()
		{
			return "ID_ALMS";
		}

		public void ResetPersistence()
		{
			this.ResetAll();
		}

		public int GetOrder()
		{
			return 0;
		}

		public PersistentManager.PersistentData GetCurrentPersistentState(string dataPath, bool fullSave)
		{
			return new AlmsManager.AlmsPersistenceData
			{
				tears = this.TearsGiven
			};
		}

		public void SetCurrentPersistentState(PersistentManager.PersistentData data, bool isloading, string dataPath)
		{
			AlmsManager.AlmsPersistenceData almsPersistenceData = (AlmsManager.AlmsPersistenceData)data;
			this.TearsGiven = almsPersistenceData.tears;
		}

		public override void OnGUI()
		{
			base.DebugResetLine();
			base.DebugDrawTextLine("AlmsManager -------------------------------------", 10, 1500);
			base.DebugDrawTextLine("TearsGiven: " + this.TearsGiven, 10, 1500);
			base.DebugDrawTextLine("Tier: " + this.CurentTier, 10, 1500);
			base.DebugDrawTextLine(string.Concat(new object[]
			{
				"Num Tiers: ",
				this.Config.TearsToUnlock.Length,
				"  IsMax:",
				this.IsMaxTier()
			}), 10, 1500);
			base.DebugDrawTextLine("PrieDieu Level: " + this.GetPrieDieuLevel(), 10, 1500);
			base.DebugDrawTextLine("Altar Level: " + this.GetAltarLevel(), 10, 1500);
			base.DebugDrawTextLine("Levels:", 10, 1500);
			int num = 1;
			foreach (int num2 in this.Config.GetTearsList())
			{
				string text = string.Concat(new object[]
				{
					"Tier ",
					num,
					"  ",
					num2
				});
				if (num == this.CurentTier)
				{
					text = "--->" + text;
				}
				base.DebugDrawTextLine(text, 10, 1500);
				num++;
			}
		}

		private int _currentTears;

		private const string ALMS_RESOURCE_CONFIG = "AlmsConfig";

		private const string PERSITENT_ID = "ID_ALMS";

		[Serializable]
		public class AlmsPersistenceData : PersistentManager.PersistentData
		{
			public AlmsPersistenceData() : base("ID_ALMS")
			{
			}

			public int tears;
		}
	}
}
