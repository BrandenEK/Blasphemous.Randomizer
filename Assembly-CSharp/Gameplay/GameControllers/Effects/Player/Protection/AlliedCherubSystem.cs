using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Framework.Managers;
using Framework.Pooling;
using Framework.Util;
using Gameplay.GameControllers.AlliedCherub;
using Gameplay.GameControllers.Bosses.Amanecidas;
using Gameplay.GameControllers.Bosses.Isidora;
using Gameplay.GameControllers.Bosses.Snake;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Gameplay.GameControllers.Effects.Player.Protection
{
	public class AlliedCherubSystem : PoolObject
	{
		public bool IsCherubDeployed { get; set; }

		public bool IsEnable { get; set; }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<AlliedCherubSystem> OnCherubsDepleted;

		private void Update()
		{
			this.currentCheckEnemyLapse += Time.deltaTime;
			if (this.currentCheckEnemyLapse > this.CheckEnemyLapse)
			{
				this.currentCheckEnemyLapse = 0f;
				Enemy closerEnemy = this.GetCloserEnemy();
				if (closerEnemy == null)
				{
					return;
				}
				GameplayUtils.DrawDebugCross(closerEnemy.transform.position, Color.cyan, 3f);
				UnityEngine.Debug.Log("LOOKING FOR TARGET");
				bool flag = false;
				while (this.ExistMoreEnemiesToCheck())
				{
					if (this.CanAttackEnemy(closerEnemy) && this.LaunchCherubToEnemy(closerEnemy))
					{
						UnityEngine.Debug.Log("FOUND TARGET");
						this.LevelEnemiesChecked.Clear();
						flag = true;
						break;
					}
					UnityEngine.Debug.Log("LOOKING FOR ANOTHER TARGET");
					this.LevelEnemiesChecked.Add(closerEnemy);
					closerEnemy = this.GetCloserEnemy();
					if (closerEnemy == null)
					{
						break;
					}
				}
				if (!flag)
				{
					UnityEngine.Debug.Log("DIDNT FIND ANY TARGET");
					this.currentCheckEnemyLapse = this.CheckEnemyLapse * 0.9f;
					this.LevelEnemies.Clear();
					this.LevelEnemiesChecked.Clear();
					this.FindLevelEnemies();
				}
			}
		}

		private bool ExistMoreEnemiesToCheck()
		{
			return this.LevelEnemiesChecked.Count < this.LevelEnemies.Count;
		}

		private bool CanAttackEnemy(Enemy e)
		{
			if (e is Snake)
			{
				Snake snake = e as Snake;
				return snake.IsCurrentlyDamageable();
			}
			return this.DistanceToEnemy(e) < this.MinDistanceAttack && e.SpriteRenderer != null && e.SpriteRenderer.isVisible && this.CanBeDamaged(e);
		}

		private bool CanBeDamaged(Enemy e)
		{
			bool flag = !e.IsGuarding;
			return e is Amanecidas || flag;
		}

		public override void OnObjectReuse()
		{
			base.OnObjectReuse();
			this.LevelEnemies.Clear();
			this.LevelEnemiesChecked.Clear();
			this.FindLevelEnemies();
		}

		public void DeployCherubs()
		{
			this.IsCherubDeployed = false;
			this.IsEnable = true;
			this._storedCherubOnUse = 0;
			base.StartCoroutine(this.InstantiateCherubCoroutine());
		}

		public bool LaunchCherubToEnemy(Enemy enemy)
		{
			if (this.AvailableCherubs.Count <= 0)
			{
				UnityEngine.Debug.Log("NO AVAILABLE CHERUBS!");
				return false;
			}
			AlliedCherub alliedCherub = this.AvailableCherubs[this.AvailableCherubs.Count - 1];
			if (alliedCherub.Behaviour.CanSeeEnemy(enemy))
			{
				this.AvailableCherubs.Remove(alliedCherub);
				alliedCherub.Behaviour.Attack(enemy);
				UnityEngine.Debug.Log("LAUNCHING CHERUB!");
				if (this.AvailableCherubs.Count == 0)
				{
					this.OnLastCherubLaunched();
				}
				return true;
			}
			UnityEngine.Debug.Log("THEY CAN'T SEE THE ENEMY");
			return false;
		}

		private void OnLastCherubLaunched()
		{
			if (this.OnCherubsDepleted != null)
			{
				this.OnCherubsDepleted(this);
			}
		}

		private IEnumerator InstantiateCherubCoroutine()
		{
			byte i = 0;
			while ((int)i < this.AlliedCherubs.Length)
			{
				this.GetCherub(this.AlliedCherubs[(int)i]);
				yield return new WaitForSeconds(0.1f);
				i += 1;
			}
			yield return new WaitForSeconds(0.75f);
			this.IsCherubDeployed = true;
			yield break;
		}

		private void FindLevelEnemies()
		{
			foreach (Enemy enemy in UnityEngine.Object.FindObjectsOfType<Enemy>())
			{
				if (!enemy.Status.Dead && !(enemy.tag == "CherubCaptor") && !(enemy is HomingBonfire))
				{
					this.LevelEnemies.Add(enemy);
				}
			}
		}

		public float DistanceToEnemy(Enemy enemy)
		{
			return Vector2.Distance(Core.Logic.Penitent.transform.position, enemy.transform.position);
		}

		private Enemy GetCloserEnemy()
		{
			float num = float.MaxValue;
			Enemy result = null;
			Vector3 position = Core.Logic.Penitent.transform.position;
			foreach (Enemy enemy in this.LevelEnemies)
			{
				if (!(enemy == null) && !this.LevelEnemiesChecked.Contains(enemy))
				{
					float num2 = this.DistanceToEnemy(enemy);
					if (num2 < num)
					{
						result = enemy;
						num = num2;
					}
				}
			}
			return result;
		}

		public void DisposeSystem()
		{
			foreach (AlliedCherub alliedCherub in this.AvailableCherubs)
			{
				alliedCherub.Store();
			}
			this.AvailableCherubs.Clear();
		}

		public void SetCherubsPosition()
		{
		}

		private void GetCherub(AlliedCherubSystem.AlliedCherubSlot cherubSlot)
		{
			Vector3 position = Core.Logic.Penitent.transform.position;
			GameObject gameObject;
			if (this.cherubList.Count > this.AlliedCherubs.Length)
			{
				gameObject = this.cherubList[this.cherubList.Count - 1];
				this.cherubList.Remove(gameObject);
				gameObject.SetActive(true);
				gameObject.transform.position = position;
			}
			else
			{
				gameObject = UnityEngine.Object.Instantiate<GameObject>(cherubSlot.AlliedCherub, position, Quaternion.identity);
			}
			AlliedCherub componentInChildren = gameObject.GetComponentInChildren<AlliedCherub>();
			if (componentInChildren == null)
			{
				return;
			}
			componentInChildren.Deploy(cherubSlot, this);
			this.AvailableCherubs.Add(componentInChildren);
		}

		public void StoreCherub(GameObject cherub)
		{
			this.cherubList.Add(cherub);
			this._storedCherubOnUse++;
			cherub.SetActive(false);
			if (this._storedCherubOnUse < this.AlliedCherubs.Length)
			{
				return;
			}
			this.IsEnable = false;
			base.Destroy();
		}

		public AlliedCherubSystem.AlliedCherubSlot[] AlliedCherubs;

		public List<AlliedCherub> AvailableCherubs = new List<AlliedCherub>();

		public List<Enemy> LevelEnemies = new List<Enemy>();

		public List<Enemy> LevelEnemiesChecked = new List<Enemy>();

		private int _storedCherubOnUse;

		public float MinDistanceAttack = 7f;

		public float CheckEnemyLapse = 0.75f;

		private float currentCheckEnemyLapse;

		private List<GameObject> cherubList = new List<GameObject>();

		[Serializable]
		public struct AlliedCherubSlot
		{
			public Vector2 Offset;

			public GameObject AlliedCherub;
		}
	}
}
