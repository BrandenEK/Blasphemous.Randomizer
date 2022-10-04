using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Inventory;
using Framework.Managers;
using Sirenix.OdinInspector;
using UnityEngine;

public class OssuaryManager : MonoBehaviour
{
	[FoldoutGroup("Debug buttons", 0)]
	[Button("Test CLAIM COLLECTIBLE ITEMS", ButtonSizes.Small)]
	public void ClaimCarriedItems()
	{
		List<string> nonClaimedCollectibles = this.GetNonClaimedCollectibles();
		if (nonClaimedCollectibles.Count > 0)
		{
			base.StartCoroutine(this.ClaimSequenceCoroutine(nonClaimedCollectibles, new Action(this.OnActivationsFinished)));
		}
	}

	private void Start()
	{
		if (this.activationFX != null)
		{
			PoolManager.Instance.CreatePool(this.activationFX, 10);
		}
		foreach (OssuaryItem ossuaryItem in this.ossuaryItems)
		{
			ossuaryItem.gameObject.SetActive(false);
		}
		this.ActivateRetrievedCollectiblesSilently();
		this.anyPendingCollectible = this.IsThereAnyCollectibleNotClaimed();
	}

	public bool IsThereAnyCollectibleNotClaimed()
	{
		return this.GetNonClaimedCollectibles().Count != 0;
	}

	private void ActivateRetrievedCollectiblesSilently()
	{
		using (List<string>.Enumerator enumerator = OssuaryManager.GetAlreadyRetrievedCollectibles().GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				string item = enumerator.Current;
				this.ossuaryItems.Find((OssuaryItem x) => x.itemId == item).ActivateItemSilently();
			}
		}
		if (this.IsOssuaryComplete())
		{
			this.CheckActivationLists();
			Core.AchievementsManager.GrantAchievement("AC19");
		}
	}

	private static void DeactivateRetrievedCollectiblesSilently()
	{
		List<string> alreadyRetrievedCollectibles = OssuaryManager.GetAlreadyRetrievedCollectibles();
		OssuaryManager ossuaryManager = UnityEngine.Object.FindObjectOfType<OssuaryManager>();
		using (List<string>.Enumerator enumerator = alreadyRetrievedCollectibles.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				string item = enumerator.Current;
				ossuaryManager.ossuaryItems.Find((OssuaryItem x) => x.itemId == item).DeactivateItemSilently();
			}
		}
		ossuaryManager.alreadyClaimedRewards = 0;
	}

	private void CheckActivationLists()
	{
		foreach (GameObject gameObject in this.activateIfOssuaryComplete)
		{
			gameObject.SetActive(true);
		}
		foreach (GameObject gameObject2 in this.deactivateIfOssuaryComplete)
		{
			gameObject2.SetActive(false);
		}
	}

	private bool IsOssuaryComplete()
	{
		using (List<OssuaryItem>.Enumerator enumerator = this.ossuaryItems.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (!enumerator.Current.gameObject.activeInHierarchy)
				{
					return false;
				}
			}
		}
		return true;
	}

	private static List<string> GetAlreadyRetrievedCollectibles()
	{
		List<string> list = new List<string>();
		foreach (Framework.Inventory.CollectibleItem collectibleItem in Core.InventoryManager.GetCollectibleItemOwned())
		{
			if (collectibleItem.ClaimedInOssuary)
			{
				list.Add(collectibleItem.id);
			}
		}
		return list;
	}

	public static int CountAlreadyRetrievedCollectibles()
	{
		return OssuaryManager.GetAlreadyRetrievedCollectibles().Count;
	}

	public static void ResetAlreadyRetrievedCollectibles()
	{
		IEnumerator enumerator = Enum.GetValues(typeof(OSSUARY_GROUPS)).GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				object arg = enumerator.Current;
				string id = arg + "_OSSUARY_FLAG";
				Core.Events.SetFlag(id, false, false);
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
		OssuaryManager.DeactivateRetrievedCollectiblesSilently();
		OssuaryManager.UnclaimInInventory(OssuaryManager.GetAlreadyRetrievedCollectibles());
	}

	private List<string> GetNonClaimedCollectibles()
	{
		List<string> list = new List<string>();
		foreach (Framework.Inventory.CollectibleItem collectibleItem in Core.InventoryManager.GetCollectibleItemOwned())
		{
			if (!collectibleItem.ClaimedInOssuary)
			{
				list.Add(collectibleItem.id);
			}
		}
		return list;
	}

	private void OnActivationsFinished()
	{
		List<string> nonClaimedCollectibles = this.GetNonClaimedCollectibles();
		this.ClaimInInventory(nonClaimedCollectibles);
		this.CheckGroupCompletion();
	}

	private void CheckGroupCompletion()
	{
		Array values = Enum.GetValues(typeof(OSSUARY_GROUPS));
		OssuaryManager.OssuaryGroupStatus[] array = new OssuaryManager.OssuaryGroupStatus[values.Length];
		this.pendingRewards = 0;
		this.alreadyClaimedRewards = values.Length;
		int num = 0;
		for (int i = 0; i < values.Length; i++)
		{
			array[i] = new OssuaryManager.OssuaryGroupStatus((OSSUARY_GROUPS)values.GetValue(i));
			if (array[i].isCompleted)
			{
				num++;
			}
		}
		if (num == values.Length)
		{
			Core.Events.LaunchEvent(this.CheckRewardsEvent, string.Empty);
			return;
		}
		this.alreadyClaimedRewards = num;
		for (int j = 0; j < values.Length; j++)
		{
			if (!array[j].isCompleted && this.CheckGroup(array[j].group))
			{
				Core.Events.SetFlag(array[j].flag, true, false);
				this.pendingRewards++;
			}
		}
		Core.Events.LaunchEvent(this.CheckRewardsEvent, string.Empty);
	}

	private bool CheckGroup(OSSUARY_GROUPS g)
	{
		List<OssuaryItem> items = this.ossuaryGroups.Find((OssuaryItemGroups x) => x.groupId == g).items;
		if (items == null)
		{
			Debug.LogFormat("Ossuary Group {0} has no items!", new object[]
			{
				g
			});
			return false;
		}
		using (List<OssuaryItem>.Enumerator enumerator = items.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (!enumerator.Current.gameObject.activeInHierarchy)
				{
					return false;
				}
			}
		}
		return true;
	}

	private void ClaimInInventory(List<string> ids)
	{
		foreach (string idCollectibleItem in ids)
		{
			Core.InventoryManager.GetCollectibleItem(idCollectibleItem).ClaimedInOssuary = true;
		}
	}

	private static void UnclaimInInventory(List<string> ids)
	{
		foreach (string idCollectibleItem in ids)
		{
			Core.InventoryManager.GetCollectibleItem(idCollectibleItem).ClaimedInOssuary = false;
		}
	}

	private OssuaryItem GetOssuaryItem(string id)
	{
		return this.ossuaryItems.Find((OssuaryItem x) => x.itemId == id);
	}

	private IEnumerator ClaimSequenceCoroutine(List<string> ids, Action callback)
	{
		float timeBetweenActivations = 0.25f;
		int num;
		for (int counter = 0; counter < ids.Count; counter = num + 1)
		{
			OssuaryItem ossuaryItem = this.GetOssuaryItem(ids[counter]);
			ossuaryItem.ActivateItem();
			this.ActivationEffect(ossuaryItem.transform.position);
			yield return new WaitForSeconds(timeBetweenActivations);
			num = counter;
		}
		callback();
		yield break;
	}

	private void ActivationEffect(Vector2 pos)
	{
		if (this.activationFX != null)
		{
			PoolManager.Instance.ReuseObject(this.activationFX, pos, Quaternion.identity, false, 1);
		}
	}

	private const string collectiblePath = "Inventory/CollectibleItem/";

	[FoldoutGroup("Main", 0)]
	public List<OssuaryItem> ossuaryItems;

	[FoldoutGroup("Activation Lists", 0)]
	public List<GameObject> activateIfOssuaryComplete;

	[FoldoutGroup("Activation Lists", 0)]
	public List<GameObject> deactivateIfOssuaryComplete;

	[FoldoutGroup("Groups", 0)]
	public List<OssuaryItemGroups> ossuaryGroups;

	[FoldoutGroup("Effects", 0)]
	public GameObject activationFX;

	[FoldoutGroup("Playmaker properties", 0)]
	public bool anyPendingCollectible;

	[FoldoutGroup("Playmaker properties", 0)]
	public int pendingRewards;

	[FoldoutGroup("Playmaker properties", 0)]
	public int alreadyClaimedRewards;

	[FoldoutGroup("Playmaker properties", 0)]
	public string CheckRewardsEvent;

	private struct OssuaryGroupStatus
	{
		public OssuaryGroupStatus(OSSUARY_GROUPS g)
		{
			this.group = g;
			this.flag = string.Format("{0}_OSSUARY_FLAG", g.ToString());
			this.isCompleted = Core.Events.GetFlag(this.flag);
		}

		public OSSUARY_GROUPS group;

		public string flag;

		public bool isCompleted;
	}
}
