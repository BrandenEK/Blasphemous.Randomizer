using System;
using System.Collections.Generic;
using Tools.Level.Actionables;
using UnityEngine;

namespace Gameplay.GameControllers.Environment
{
	public class AshPlatformFightManager : MonoBehaviour
	{
		private void Update()
		{
			if (this.activated)
			{
				this.CheckIndex();
			}
		}

		public void Activate()
		{
			this.currentCounter = 0f;
			this.currentIndex = 0;
			this.activated = true;
		}

		public void Deactivate()
		{
			this.activated = false;
			foreach (AshPlatformFightManager.AshPlatformsByIndex ashPlatformsByIndex in this.platforms)
			{
				if (ashPlatformsByIndex.platform.showing)
				{
					ashPlatformsByIndex.platform.Hide(1f, 0.8f);
				}
			}
		}

		private void CheckIndex()
		{
			if (this.currentCounter < this.timeBetweenIndexes)
			{
				this.currentCounter += Time.deltaTime;
			}
			else
			{
				this.currentCounter = 0f;
				this.currentIndex = (this.currentIndex + 1) % this.maxIndex;
				this.ActivateAllWithIndex(this.currentIndex);
			}
		}

		private void ActivateAllWithIndex(int i)
		{
			foreach (AshPlatformFightManager.AshPlatformsByIndex ashPlatformsByIndex in this.platforms)
			{
				if (ashPlatformsByIndex.index == i && (!this.heightLimitOn || ashPlatformsByIndex.platform.transform.position.y < base.transform.position.y + this.heightLimit))
				{
					ashPlatformsByIndex.platform.Show();
					ashPlatformsByIndex.platform.Hide(this.platformActivationTime, 0.8f);
				}
			}
		}

		public void DeactivateRecursively(AshPlatform a)
		{
			List<GameObject> targets = a.GetTargets();
			foreach (GameObject gameObject in targets)
			{
				if (gameObject == null)
				{
					break;
				}
				AshPlatform component = gameObject.GetComponent<AshPlatform>();
				if (a != null)
				{
					this.DeactivateRecursively(component);
				}
			}
			a.Hide(0f, 0.8f);
		}

		public void DeactivateGroup(AshPlatform a)
		{
			this.DeactivateRecursively(a);
			a.gameObject.SetActive(false);
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.cyan;
			Gizmos.DrawLine(base.transform.position - Vector2.right * 10f + Vector2.up * this.heightLimit, base.transform.position + Vector2.right * 10f + Vector2.up * this.heightLimit);
			foreach (AshPlatformFightManager.AshPlatformsByIndex ashPlatformsByIndex in this.platforms)
			{
				switch (ashPlatformsByIndex.index)
				{
				case 0:
					Gizmos.color = Color.red;
					break;
				case 1:
					Gizmos.color = Color.blue;
					break;
				case 2:
					Gizmos.color = Color.green;
					break;
				case 3:
					Gizmos.color = Color.yellow;
					break;
				}
				Gizmos.DrawCube(ashPlatformsByIndex.platform.transform.position + Vector3.right, new Vector3(1f, 0.5f, 1f));
			}
			for (int i = 0; i < 4; i++)
			{
				switch (i)
				{
				case 0:
					Gizmos.color = Color.red;
					break;
				case 1:
					Gizmos.color = Color.blue;
					break;
				case 2:
					Gizmos.color = Color.green;
					break;
				case 3:
					Gizmos.color = Color.yellow;
					break;
				}
				Vector2 vector = base.transform.position + Vector2.down * 6f - Vector2.right * 2f + Vector2.right * (float)i;
				Gizmos.DrawSphere(vector, 0.25f);
			}
		}

		public float heightLimit = 5f;

		public bool heightLimitOn = true;

		public List<AshPlatformFightManager.AshPlatformsByIndex> platforms;

		public float timeBetweenIndexes = 3f;

		public int maxIndex = 5;

		public float currentCounter;

		public int currentIndex = -1;

		public float platformActivationTime = 2f;

		public bool activated;

		[Serializable]
		public struct AshPlatformsByIndex
		{
			public int index;

			public AshPlatform platform;
		}
	}
}
