using System;
using System.Collections;
using System.Collections.Generic;
using Framework.FrameworkCore;
using Framework.Inventory;
using Framework.Util;
using Gameplay.UI;
using Gameplay.UI.Widgets;
using UnityEngine;

namespace Framework.Managers
{
	public class DemakeManager : GameSystem
	{
		public override void Initialize()
		{
			GameModeManager gameModeManager = Core.GameModeManager;
			gameModeManager.OnExitDemakeMode = (Core.SimpleEvent)Delegate.Combine(gameModeManager.OnExitDemakeMode, new Core.SimpleEvent(this.OnExitDemakeMode));
			GameModeManager gameModeManager2 = Core.GameModeManager;
			gameModeManager2.OnEnterDemakeMode = (Core.SimpleEvent)Delegate.Combine(gameModeManager2.OnEnterDemakeMode, new Core.SimpleEvent(this.OnEnterDemakeMode));
		}

		private void OnEnterDemakeMode()
		{
			UIController.instance.CanOpenInventory = false;
		}

		private void OnExitDemakeMode()
		{
			UIController.instance.CanOpenInventory = true;
		}

		private void OnBeforeLevelLoad(Level oldLevel, Level newLevel)
		{
			if (newLevel.LevelName.StartsWith("D25") && !Core.GameModeManager.IsCurrentMode(GameModeManager.GAME_MODES.DEMAKE))
			{
				Core.GameModeManager.ChangeMode(GameModeManager.GAME_MODES.DEMAKE);
				Core.PenitenceManager.UseStocksOfHealth = true;
				this.prevMode = GameModeManager.GAME_MODES.DEMAKE;
			}
		}

		private void OnLevelLoaded(Level oldLevel, Level newLevel)
		{
			if (string.IsNullOrEmpty(this.prevLevel))
			{
				this.prevLevel = Core.LevelManager.currentLevel.LevelName;
			}
		}

		public override void Update()
		{
			base.Update();
		}

		public override void Dispose()
		{
		}

		public void StartDemakeRun()
		{
			Singleton<Core>.Instance.StartCoroutine(this.LoadDemakeFadeWhiteCourrutine());
		}

		public void DemakeLevelVictory()
		{
			if (this.OnDemakeLevelCompletion != null)
			{
				this.OnDemakeLevelCompletion();
			}
		}

		public void EndDemakeRun(bool completed, int numSpecialItems)
		{
			Singleton<Core>.Instance.StartCoroutine(this.LoadArcadeFadeCourrutine(completed, numSpecialItems));
		}

		private void OnArcadeLevelLoad(Level newLevel, Level oldLevel)
		{
			LevelManager.OnLevelLoaded -= this.OnArcadeLevelLoad;
			Singleton<Core>.Instance.StartCoroutine(this.WaitBeforeRewards());
		}

		private IEnumerator WaitBeforeRewards()
		{
			Core.Logic.Penitent.Stats.Purge.Current = this.prevPurge;
			this.prevPurge = 0f;
			Core.Logic.Penitent.Stats.Life.Current = this.prevLife;
			this.prevLife = 0f;
			Core.Logic.Penitent.Stats.Fervour.Current = this.prevFervour;
			this.prevFervour = 0f;
			yield return new WaitForSecondsRealtime(1f);
			if (this.grantSkin1)
			{
				this.grantSkin1 = false;
				Core.ColorPaletteManager.UnlockDemake1ColorPalette();
				yield return new WaitForSecondsRealtime(0.05f);
				yield return new WaitUntil(() => !UIController.instance.IsUnlockActive());
			}
			if (this.grantSkin2)
			{
				this.grantSkin2 = false;
				Core.ColorPaletteManager.UnlockDemake2ColorPalette();
				yield return new WaitForSecondsRealtime(0.05f);
				yield return new WaitUntil(() => !UIController.instance.IsUnlockActive());
			}
			if (this.purgeToGrant > 0f)
			{
				Core.Logic.Penitent.Stats.Purge.Current += this.purgeToGrant;
				this.purgeToGrant = 0f;
				if (!this.grantSkin1)
				{
					BaseInventoryObject baseObjectOrTears = Core.InventoryManager.GetBaseObjectOrTears("QI78", InventoryManager.ItemType.Quest);
					UIController.instance.ShowObjectPopUp(UIController.PopupItemAction.GetObejct, baseObjectOrTears.caption, baseObjectOrTears.picture, InventoryManager.ItemType.Quest, 3f, true);
				}
			}
			yield break;
		}

		private IEnumerator LoadDemakeFadeWhiteCourrutine()
		{
			yield return FadeWidget.instance.FadeCoroutine(new Color(0f, 0f, 0f, 0f), Color.white, 2f, true, null);
			UIController.instance.HideIntroDemakeWidget();
			this.prevSlot = PersistentManager.GetAutomaticSlot();
			this.prevLevel = Core.LevelManager.currentLevel.LevelName;
			this.prevMode = Core.GameModeManager.GetCurrentGameMode();
			this.prevPurge = Core.Logic.Penitent.Stats.Purge.Current;
			this.prevLife = Core.Logic.Penitent.Stats.Life.Current;
			this.prevFervour = Core.Logic.Penitent.Stats.Fervour.Current;
			Core.SpawnManager.SetCurrentToCustomSpawnData(false);
			PersistentManager.SetAutomaticSlot(8);
			Core.GameModeManager.ChangeMode(GameModeManager.GAME_MODES.DEMAKE);
			Core.Persistence.SaveGame(false);
			Core.Logic.Penitent.Stats.Life.SetPermanentBonus(0f);
			Core.Logic.Penitent.Stats.Life.SetToCurrentMax();
			Core.Logic.Penitent.Stats.Flask.SetToCurrentMax();
			Core.Logic.Penitent.Stats.Purge.Current = 0f;
			Core.Logic.Penitent.Stats.Fervour.Current = 0f;
			Core.Logic.Penitent.Stats.Strength.ResetBonus();
			Core.Logic.Penitent.Stats.Strength.ResetUpgrades();
			Core.PenitenceManager.DeactivateCurrentPenitence();
			Core.PenitenceManager.UseStocksOfHealth = true;
			Core.InventoryManager.RemoveEquipableObjects();
			this.oldShakeValue = Core.Logic.CameraManager.ProCamera2DShake.enabled;
			Core.Logic.CameraManager.ProCamera2DShake.enabled = false;
			Core.SpawnManager.PrepareForSpawnFromEditor();
			Core.LevelManager.ChangeLevel("D25Z01S01", false, false, false, new Color?(Color.white));
			yield break;
		}

		private IEnumerator LoadArcadeFadeCourrutine(bool completed, int numSpecialItems)
		{
			if (completed)
			{
				yield return FadeWidget.instance.FadeCoroutine(new Color(0f, 0f, 0f, 0f), Color.white, 2f, true, null);
			}
			else
			{
				yield return FadeWidget.instance.FadeCoroutine(Color.black, Color.white, 2f, true, null);
			}
			this.purgeToGrant = Core.Logic.Penitent.Stats.Purge.Current;
			this.grantSkin2 = Core.Events.GetFlag("DEMAKE_GBSKIN");
			PersistentManager.SetAutomaticSlot(this.prevSlot);
			Core.Persistence.LoadGameWithOutRespawn(this.prevSlot);
			Core.GameModeManager.ChangeMode(this.prevMode);
			Core.Logic.CameraManager.ProCamera2DShake.enabled = this.oldShakeValue;
			Core.Persistence.SaveGame(false);
			LevelManager.OnLevelLoaded += this.OnArcadeLevelLoad;
			if (completed)
			{
				if (numSpecialItems == this.purgeRewardsByIndex.Count - 1 && !Core.ColorPaletteManager.IsColorPaletteUnlocked("PENITENT_DEMAKE"))
				{
					this.grantSkin1 = true;
					this.purgeToGrant = 0f;
				}
				else
				{
					this.purgeToGrant += (float)this.purgeRewardsByIndex[numSpecialItems];
				}
			}
			Core.SpawnManager.SpawnFromCustom(true, new Color?(Color.white));
			yield break;
		}

		public List<int> purgeRewardsByIndex = new List<int>
		{
			100,
			200,
			500,
			1000,
			2000,
			3000
		};

		public const string DEMAKE_DISTRICT = "D25";

		private const string FIRST_DEMAKE_SCENE = "D25Z01S01";

		public const string INTRO_DEMAKE_ID = "DEMAKE_INTRO";

		public const string INTRO_DEMAKE_EVENT = "event:/Background Layer/DemakePressStartScreen";

		public const string ARCADE_MUSIC_EVENT = "event:/SFX/DEMAKE/ArcadeMusic";

		public const string SKIN2_FLAG = "DEMAKE_GBSKIN";

		private const int DEMAKE_SLOT = 8;

		private int prevSlot;

		private GameModeManager.GAME_MODES prevMode;

		private string prevLevel;

		private bool oldShakeValue;

		private float prevPurge;

		private float prevLife;

		private float prevFervour;

		private float purgeToGrant;

		private bool grantSkin1;

		private bool grantSkin2;

		public Core.SimpleEvent OnDemakeLevelCompletion;
	}
}
