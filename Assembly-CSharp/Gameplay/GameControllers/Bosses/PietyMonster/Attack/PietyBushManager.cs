using System;
using Gameplay.GameControllers.Bosses.PietyMonster.ThornProjectile;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.PietyMonster.Attack
{
	public class PietyBushManager : MonoBehaviour
	{
		public PietyMonster PietyMonster { get; set; }

		public BoxCollider2D Collider { get; set; }

		public int BushDamage { get; set; }

		private void Start()
		{
			this.Collider = base.GetComponent<BoxCollider2D>();
			if (this.PietyMonster == null)
			{
				Debug.Log("A Piety Monster prefab is needed");
			}
			if (this.PietyBushPrefab == null)
			{
				Debug.LogError("A Piety Monster's bush prefab is needed");
			}
		}

		private void Update()
		{
			if (this.PietyMonster != null)
			{
				return;
			}
			this.PietyMonster = UnityEngine.Object.FindObjectOfType<PietyMonster>();
		}

		public void DestroyBushes()
		{
			PietyBush[] array = UnityEngine.Object.FindObjectsOfType<PietyBush>();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].DestroyBush();
			}
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (!other.gameObject.CompareTag("NPC"))
			{
				return;
			}
			ThornProjectile componentInChildren = other.transform.root.GetComponentInChildren<ThornProjectile>();
			if (!componentInChildren)
			{
				return;
			}
			float x = componentInChildren.transform.position.x;
			componentInChildren.HitsOnFloor();
			Vector2 v = new Vector2(x, this.Collider.bounds.max.y);
			if (this.PietyBushPrefab == null)
			{
				return;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.PietyBushPrefab, v, Quaternion.identity);
			PietyBush component = gameObject.GetComponent<PietyBush>();
			if (!component)
			{
				return;
			}
			component.SetOwner(this.PietyMonster);
			component.GetComponent<IDirectAttack>().SetDamage(this.BushDamage);
		}

		public GameObject PietyBushPrefab;
	}
}
