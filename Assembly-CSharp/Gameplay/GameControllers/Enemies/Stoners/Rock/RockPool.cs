using System;
using System.Collections.Generic;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Stoners.Rock
{
	public class RockPool : MonoBehaviour
	{
		public GameObject GetRock(Vector3 position)
		{
			GameObject gameObject;
			if (this.Rocks.Count > 0)
			{
				gameObject = this.Rocks[this.Rocks.Count - 1];
				this.Rocks.Remove(gameObject);
				gameObject.SetActive(true);
				gameObject.transform.position = position;
				gameObject.transform.rotation = Quaternion.identity;
			}
			else
			{
				gameObject = UnityEngine.Object.Instantiate<GameObject>(this.Rock, position, Quaternion.identity);
			}
			StonersRock componentInChildren = gameObject.GetComponentInChildren<StonersRock>();
			componentInChildren.AttackArea.Entity = this.EnemyOwner;
			return gameObject;
		}

		public void StoreRock(GameObject rock)
		{
			if (this.Rocks.Contains(rock))
			{
				return;
			}
			this.Rocks.Add(rock);
			rock.SetActive(false);
		}

		private void OnDestroy()
		{
			if (this.Rocks.Count > 0)
			{
				this.Rocks.Clear();
			}
		}

		public Enemy EnemyOwner;

		public GameObject Rock;

		private readonly List<GameObject> Rocks = new List<GameObject>();
	}
}
