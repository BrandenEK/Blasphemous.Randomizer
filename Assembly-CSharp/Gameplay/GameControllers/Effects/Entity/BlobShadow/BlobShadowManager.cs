using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.GameControllers.Effects.Entity.BlobShadow
{
	public class BlobShadowManager : MonoBehaviour
	{
		public GameObject GetBlowShadow(Vector3 position)
		{
			GameObject gameObject;
			if (this.blobShadowList.Count > 0)
			{
				gameObject = this.blobShadowList[this.blobShadowList.Count - 1];
				if (!gameObject.activeSelf)
				{
					gameObject.SetActive(true);
				}
				this.blobShadowList.Remove(gameObject);
			}
			else
			{
				gameObject = UnityEngine.Object.Instantiate<GameObject>(this.blobShadowPrefab, position, Quaternion.identity);
			}
			if (gameObject.transform.parent != base.transform)
			{
				gameObject.transform.parent = base.transform;
			}
			return gameObject;
		}

		public void StoreBlobShadow(GameObject blobShadow)
		{
			if (!this.blobShadowList.Contains(blobShadow))
			{
				this.blobShadowList.Add(blobShadow);
				blobShadow.gameObject.SetActive(false);
			}
		}

		[SerializeField]
		protected GameObject blobShadowPrefab;

		private List<GameObject> blobShadowList = new List<GameObject>();
	}
}
