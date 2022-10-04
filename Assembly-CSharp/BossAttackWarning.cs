using System;
using Framework.Managers;
using UnityEngine;

public class BossAttackWarning : MonoBehaviour
{
	private void Start()
	{
		if (this.warningItem != null)
		{
			PoolManager.Instance.CreatePool(this.warningItem, this.poolSize);
		}
	}

	public GameObject ShowWarning(Vector2 pos)
	{
		return PoolManager.Instance.ReuseObject(this.warningItem, pos, Quaternion.identity, false, 1).GameObject;
	}

	public GameObject warningItem;

	public int poolSize = 1;
}
