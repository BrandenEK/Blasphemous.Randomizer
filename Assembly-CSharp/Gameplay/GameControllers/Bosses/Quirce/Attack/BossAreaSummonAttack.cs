using System;
using System.Collections;
using System.Collections.Generic;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Entities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.Quirce.Attack
{
	public class BossAreaSummonAttack : EnemyAttack, ISpawnerAttack
	{
		protected override void OnStart()
		{
			base.OnStart();
			if (this.useDifferentRandomAreas)
			{
				foreach (GameObject prefab in this.areaPrefabs)
				{
					PoolManager.Instance.CreatePool(prefab, this.poolSize);
				}
			}
			else
			{
				PoolManager.Instance.CreatePool(this.areaPrefab, this.poolSize);
			}
		}

		public override void CurrentWeaponAttack()
		{
			base.CurrentWeaponAttack();
		}

		public void SummonAreas(Vector3 direction)
		{
			base.StartCoroutine(this.SummonAreasCoroutine(base.transform.position, direction, EntityOrientation.Right));
		}

		public void SummonAreas(Vector3 position, Vector3 direction)
		{
			base.StartCoroutine(this.SummonAreasCoroutine(position, direction, EntityOrientation.Right));
		}

		public void SummonAreas(Vector3 position, Vector3 direction, EntityOrientation orientation)
		{
			base.StartCoroutine(this.SummonAreasCoroutine(position, direction, orientation));
		}

		public void SetDamageStrength(float str)
		{
			this.damageMultiplier = str;
		}

		public GameObject SummonAreaOnPoint(Vector3 point, float angle = 0f, float damageStrength = 1f, Action callbackOnLoopFinish = null)
		{
			return this.InstantiateArea(this.areaPrefab, point, angle, damageStrength, callbackOnLoopFinish);
		}

		public GameObject SummonAreaOnPoint(int areaIndex, Vector3 point, float angle = 0f, float damageStrength = 1f, Action callbackOnLoopFinish = null)
		{
			return this.InstantiateArea(this.areaPrefabs[areaIndex], point, angle, damageStrength, callbackOnLoopFinish);
		}

		public void ClearAll()
		{
			if (this.instantiations == null)
			{
				return;
			}
			for (int i = 0; i < this.instantiations.Count; i++)
			{
				this.instantiations[i].SetActive(false);
			}
		}

		private IEnumerator SummonAreasCoroutine(Vector3 origin, Vector3 dir, EntityOrientation orientation)
		{
			float counter = 0f;
			int areasSummoned = 0;
			Vector3 lastPoint = origin + dir * this.offset;
			bool cancelled = false;
			int currentTotalAreas = this.totalAreas;
			float currentDistanceBetweenAreas = this.distanceBetweenAreas;
			this.lastRandomAreaIndex = -1;
			while (counter < this.seconds)
			{
				float normalizedValue = this.curve.Evaluate(counter / this.seconds);
				if ((float)areasSummoned / (float)currentTotalAreas <= normalizedValue)
				{
					if (this.checkCollisions || cancelled)
					{
						RaycastHit2D[] array = new RaycastHit2D[1];
						bool flag = Physics2D.LinecastNonAlloc(origin, lastPoint, array, this.collisionMask) > 0;
						if (flag)
						{
							Debug.DrawLine(array[0].point, array[0].point + Vector2.up * 0.25f, Color.red, 1f);
							cancelled = true;
						}
					}
					if (!cancelled)
					{
						GameObject gameObject;
						if (this.useDifferentRandomAreas)
						{
							int num = Random.Range(0, this.areaPrefabs.Count);
							if (this.areaPrefabs.Count > 1)
							{
								while (this.lastRandomAreaIndex == num)
								{
									num = Random.Range(0, this.areaPrefabs.Count);
								}
								this.lastRandomAreaIndex = num;
							}
							GameObject toInstantiate = this.areaPrefabs[num];
							gameObject = this.InstantiateArea(toInstantiate, lastPoint, 0f, this.damageMultiplier, null);
							bool flag2 = areasSummoned % 2 == 0;
							float num2 = (!flag2) ? (-this.yDisplacement) : this.yDisplacement;
							num2 += 0.1f;
							BossSpawnedAreaAttack component = gameObject.GetComponent<BossSpawnedAreaAttack>();
							component.transform.position += Vector3.up * num2;
							component.GetComponentInChildren<SpriteRenderer>().sortingOrder = ((!flag2) ? 2 : 0);
						}
						else
						{
							gameObject = this.InstantiateArea(this.areaPrefab, lastPoint, 0f, this.damageMultiplier, null);
						}
						Entity component2 = gameObject.GetComponent<Entity>();
						if (component2 != null)
						{
							component2.SetOrientation(orientation, true, false);
						}
						areasSummoned++;
					}
					lastPoint += dir * currentDistanceBetweenAreas;
				}
				yield return null;
				counter += Time.deltaTime;
			}
			yield break;
		}

		private GameObject InstantiateArea(GameObject toInstantiate, Vector3 point, float angle = 0f, float damageStrength = 1f, Action callbackOnLoopFinish = null)
		{
			Quaternion rotation = Quaternion.Euler(0f, 0f, angle);
			GameObject gameObject = PoolManager.Instance.ReuseObject(toInstantiate, point, rotation, false, 1).GameObject;
			BossSpawnedAreaAttack component = gameObject.GetComponent<BossSpawnedAreaAttack>();
			if (component != null)
			{
				component.SetOwner(base.EntityOwner);
				component.SetDamageStrength(damageStrength);
				if (this.SpawnedAreaAttackDamage > 0)
				{
					component.SetDamage(this.SpawnedAreaAttackDamage);
				}
				component.SetCallbackOnLoopFinish(callbackOnLoopFinish);
			}
			if (this.instantiations == null)
			{
				this.instantiations = new List<GameObject>();
			}
			if (!this.instantiations.Contains(gameObject))
			{
				this.instantiations.Add(gameObject);
			}
			return gameObject;
		}

		public void SetSpawnsDamage(int damage)
		{
			this.SpawnedAreaAttackDamage = damage;
			if (this.useDifferentRandomAreas)
			{
				foreach (GameObject gameObject in this.areaPrefabs)
				{
					IDirectAttack component = gameObject.GetComponent<IDirectAttack>();
					if (component != null)
					{
						component.SetDamage(damage);
					}
				}
			}
			else
			{
				IDirectAttack component2 = this.areaPrefab.GetComponent<IDirectAttack>();
				if (component2 != null)
				{
					component2.SetDamage(damage);
				}
			}
			this.CreateSpawnsHits();
		}

		public void CreateSpawnsHits()
		{
			if (this.useDifferentRandomAreas)
			{
				foreach (GameObject gameObject in this.areaPrefabs)
				{
					IDirectAttack component = gameObject.GetComponent<IDirectAttack>();
					if (component != null)
					{
						component.CreateHit();
					}
				}
			}
			else
			{
				IDirectAttack component2 = this.areaPrefab.GetComponent<IDirectAttack>();
				if (component2 != null)
				{
					component2.CreateHit();
				}
			}
		}

		public AnimationCurve curve;

		public bool useDifferentRandomAreas;

		[HideIf("useDifferentRandomAreas", true)]
		public GameObject areaPrefab;

		[ShowIf("useDifferentRandomAreas", true)]
		public List<GameObject> areaPrefabs;

		[ShowIf("useDifferentRandomAreas", true)]
		public float yDisplacement;

		public int totalAreas;

		public float distanceBetweenAreas;

		public float seconds;

		public float offset = 2f;

		public int poolSize = 3;

		public LayerMask collisionMask;

		public bool checkCollisions = true;

		private float damageMultiplier = 1f;

		public List<GameObject> instantiations;

		private int lastRandomAreaIndex;

		public int SpawnedAreaAttackDamage;
	}
}
