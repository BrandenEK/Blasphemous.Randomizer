using System;
using DG.Tweening;
using UnityEngine;

public class OssuaryItem : MonoBehaviour
{
	public void ActivateItemSilently()
	{
		base.gameObject.SetActive(true);
	}

	public void DeactivateItemSilently()
	{
		base.gameObject.SetActive(false);
	}

	public void ActivateItem()
	{
		base.gameObject.SetActive(true);
		base.transform.DOPunchScale(Vector3.one, 0.3f, 2, 1f);
	}

	public string itemId;
}
