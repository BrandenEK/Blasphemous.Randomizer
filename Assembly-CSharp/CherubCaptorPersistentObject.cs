using System;
using System.Collections;
using System.Diagnostics;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.UI;
using Tools.Level.Utils;
using UnityEngine;

public class CherubCaptorPersistentObject : PersistentObject
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<string> OnCherubDestroyed;

	public void OnCherubKilled()
	{
		this.destroyed = true;
		if (this.OnCherubDestroyed != null)
		{
			this.OnCherubDestroyed(this.cherubId);
		}
		this.spawner.DisableCherubSpawn();
		string id = this.cherubId;
		Core.Events.SetFlag(id, true, false);
		this.AddProgressToAC13();
		Camera.main.GetComponent<MonoBehaviour>().StartCoroutine(this.ShowPopUp());
	}

	public override PersistentManager.PersistentData GetCurrentPersistentState(string dataPath, bool fullSave)
	{
		CherubCaptorPersistentObject.CherubSpawnPersistenceData cherubSpawnPersistenceData = base.CreatePersistentData<CherubCaptorPersistentObject.CherubSpawnPersistenceData>();
		cherubSpawnPersistenceData.cherubId = this.cherubId;
		cherubSpawnPersistenceData.destroyed = this.destroyed;
		Debug.Log(string.Format("<color=red>SAVING CHERUB OF ID:{0}. Destroyed = {1}</color>", this.cherubId, this.destroyed));
		return cherubSpawnPersistenceData;
	}

	public override void SetCurrentPersistentState(PersistentManager.PersistentData data, bool isloading, string dataPath)
	{
		CherubCaptorPersistentObject.CherubSpawnPersistenceData cherubSpawnPersistenceData = (CherubCaptorPersistentObject.CherubSpawnPersistenceData)data;
		this.destroyed = cherubSpawnPersistenceData.destroyed;
		if (this.destroyed)
		{
			this.spawner.DisableCherubSpawn();
			this.spawner.DestroySpawnedCherub();
		}
	}

	private void AddProgressToAC13()
	{
		float progress = Core.AchievementsManager.Achievements["AC13"].Progress;
		float num = progress * 38f / 100f;
		int num2 = CherubCaptorPersistentObject.CountRescuedCherubs();
		if ((float)num2 > num)
		{
			Core.AchievementsManager.Achievements["AC13"].AddProgress(2.631579f);
		}
	}

	private IEnumerator ShowPopUp()
	{
		int currentNumberCherubs = CherubCaptorPersistentObject.CountRescuedCherubs();
		yield return new WaitForSeconds(1f);
		UIController.instance.ShowCherubPopUp(currentNumberCherubs + "/" + 38, "event:/Key Event/RelicCollected", 3f, false);
		yield break;
	}

	public static int CountRescuedCherubs()
	{
		int num = 0;
		for (int i = 0; i < 38; i++)
		{
			string id = string.Format("RESCUED_CHERUB_{0}", (i + 1).ToString("00"));
			if (Core.Events.GetFlag(id))
			{
				num++;
			}
		}
		return num;
	}

	public string cherubId;

	public bool destroyed;

	public CherubCaptorSpawnConfigurator spawner;

	public const int TOTAL_NUMBER_OF_CHERUBS_FOR_AC13 = 38;

	private const string RELEASE_CHERUB_SFX = "event:/Key Event/RelicCollected";

	private class CherubSpawnPersistenceData : PersistentManager.PersistentData
	{
		public CherubSpawnPersistenceData(string id) : base(id)
		{
		}

		public string cherubId;

		public bool destroyed;
	}
}
