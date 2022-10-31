using System;
using System.Collections;
using Framework.EditorScripts.EnemiesBalance;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Entities;
using Sirenix.OdinInspector;
using Tools.Level.Utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace Tools.Level.Layout
{
	[SelectionBase]
	public class EnemySpawnPoint : MonoBehaviour
	{
		public Enemy SpawnedEnemy { get; private set; }

		public bool Consumed { get; set; }

		public int SpawningId { get; private set; }

		public Vector3 Position
		{
			get
			{
				return this.spawnPoint.transform.position;
			}
		}

		public bool HasEnemySpawned { get; private set; }

		public event Action<EnemySpawnPoint, Enemy> OnEnemySpawned;

		public GameObject SelectedEnemy
		{
			get
			{
				return this.selectedEnemy;
			}
		}

		private void Awake()
		{
			this.HasEnemySpawned = false;
			this.SpawningId = base.gameObject.GetHashCode();
			this.preview.enabled = false;
		}

		private void Start()
		{
			if (this.SpawnOnArena)
			{
				this._spawnOnArenaAwaiting = new WaitForSeconds(this.SpawningDelayOnArena);
			}
			if (this.SpawnOnArena && this.SpawnVfx != null)
			{
				PoolManager.Instance.CreatePool(this.SpawnVfx, 1);
			}
		}

		public void CreateEnemy()
		{
			if (this.EnemySpawnDisabled)
			{
				return;
			}
			this.Consumed = Core.Logic.EnemySpawner.IsSpawnerConsumed(base.gameObject.name);
			if (!this.EnablePersistence)
			{
				this.Consumed = false;
			}
			if (this.HasEnemySpawned || !this.selectedEnemy || !this.spawnPoint || this.Consumed)
			{
				return;
			}
			string id = this.selectedEnemy.GetComponentInChildren<Enemy>().Id;
			GameObject gameObject = null;
			if (Core.Randomizer.enemizer != null && (!(Core.LevelManager.currentLevel.LevelName == "D03Z01S02") || !(id == "EN03")))
			{
				gameObject = Core.Randomizer.enemizer.getEnemy(id);
			}
			GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>((gameObject == null) ? this.selectedEnemy : gameObject, this.spawnPoint.position, Quaternion.identity);
			gameObject2.transform.parent = Core.Logic.CurrentLevelConfig.transform;
			Enemy componentInChildren = gameObject2.GetComponentInChildren<Enemy>();
			if (componentInChildren)
			{
				this.SpawnedEnemy = componentInChildren;
				this.SpawnedEnemy.SpawningId = this.SpawningId;
				EnemyBehaviour componentInChildren2 = this.SpawnedEnemy.GetComponentInChildren<EnemyBehaviour>();
				if (componentInChildren2)
				{
					componentInChildren2.EnableBehaviourOnLoad = this.EnableBehaviourOnLoad;
				}
				if (this.OnEnemySpawned != null)
				{
					this.OnEnemySpawned(this, this.SpawnedEnemy);
				}
				this.SetEntityName(this.SpawnedEnemy);
				this.RegisterEnemyEvents();
				if (!this.SpawnEnabledEnemy)
				{
					this.SpawnedEnemy.gameObject.SetActive(false);
				}
			}
			else
			{
				Log.Warning("Level", "Triying to create an enemy whitout entity component.", null);
				Core.Randomizer.Log("Enemy does not have an entity component!");
			}
			EnemyStatsImporter enemyStatsImporter = Core.Logic.CurrentLevelConfig.EnemyStatsImporter;
			if (enemyStatsImporter != null)
			{
				enemyStatsImporter.SetEnemyStats(this.SpawnedEnemy);
			}
			this.HasEnemySpawned = true;
		}

		private void RegisterEnemyEvents()
		{
			this.isEnemyEventsSuscribed = true;
			this.SpawnedEnemy.OnDeath += this.OnEntityDie;
			this.SpawnedEnemy.OnDestroyEntity += this.OnDestroyEntity;
		}

		private void UnregisterEnemyEvents()
		{
			if (this.isEnemyEventsSuscribed && this.SpawnedEnemy)
			{
				this.isEnemyEventsSuscribed = false;
				this.SpawnedEnemy.OnDeath -= this.OnEntityDie;
				this.SpawnedEnemy.OnDestroyEntity -= this.OnDestroyEntity;
			}
		}

		private bool ValidateInput(string entityName)
		{
			return (float)entityName.Length > 3f;
		}

		private void SetEntityName(Entity instance)
		{
			if (instance == null)
			{
				return;
			}
			if (this.ValidateInput(this.EntityName))
			{
				int hashCode = instance.gameObject.GetHashCode();
				instance.gameObject.name = this.EntityName + hashCode;
			}
		}

		private void OnDestroyEntity()
		{
			this.UnregisterEnemyEvents();
		}

		private void OnEntityDie()
		{
			this.UnregisterEnemyEvents();
			this.HasEnemySpawned = false;
			Core.Logic.EnemySpawner.AddConsumedSpawner(this);
		}

		private void OnValidate()
		{
			if (this.selectedEnemy == null && this.preview != null)
			{
				this.preview.sprite = null;
			}
		}

		public void SpawnEnemyOnArena()
		{
			if (this.SpawnVfx)
			{
				PoolManager.Instance.ReuseObject(this.SpawnVfx, new Vector2(this.Position.x, this.Position.y) + this.SpawnEffectOffsetPosition, Quaternion.identity, false, 1);
			}
			base.StartCoroutine(this.SetEnemyActive(true));
		}

		private IEnumerator SetEnemyActive(bool isActive = true)
		{
			yield return this._spawnOnArenaAwaiting;
			if (this.SpawnedEnemy && this.SpawnedEnemy.gameObject)
			{
				this.SpawnedEnemy.gameObject.SetActive(isActive);
			}
			yield break;
		}

		private void SpawnOnArenaValueChanged()
		{
			if (this.SpawnOnArena)
			{
				this.SpawnEnabledEnemy = false;
			}
		}

		private void SpawnEnabledEnemyValueChanged()
		{
			if (this.SpawnEnabledEnemy)
			{
				this.SpawnOnArena = false;
			}
		}

		private void OnDestroy()
		{
			this.UnregisterEnemyEvents();
		}

		private void OnDrawGizmos()
		{
			if (this.EnableInfluenceArea)
			{
				this.DrawCircle(this.InfluenceAreaRadius, Color.magenta);
			}
		}

		private void DrawPreview()
		{
			SpriteRenderer componentInChildren = this.selectedEnemy.GetComponentInChildren<SpriteRenderer>();
			this.preview.sprite = componentInChildren.sprite;
			this.preview.transform.localPosition = componentInChildren.transform.localPosition;
			EnemySpawnConfigurator component = base.GetComponent<EnemySpawnConfigurator>();
			if (component)
			{
				this.preview.flipX = component.facingLeft;
			}
		}

		private void DrawCircle(float radius, Color color)
		{
			Gizmos.color = color;
			float num = 0f;
			float x = radius * Mathf.Cos(num);
			float y = radius * Mathf.Sin(num);
			Vector3 vector = base.transform.position + new Vector3(x, y);
			Vector3 to = vector;
			for (num = 0.1f; num < 6.2831855f; num += 0.1f)
			{
				x = radius * Mathf.Cos(num);
				y = radius * Mathf.Sin(num);
				Vector3 vector2 = base.transform.position + new Vector3(x, y);
				Gizmos.DrawLine(vector, vector2);
				vector = vector2;
			}
			Gizmos.DrawLine(vector, to);
		}

		[FoldoutGroup("Spawning Options", 0)]
		[Tooltip("Spawns the enemy when the level is loaded.")]
		[OnValueChanged("SpawnEnabledEnemyValueChanged", false)]
		public bool SpawnEnabledEnemy = true;

		[FoldoutGroup("Spawning Options", 0)]
		[Tooltip("Delays enemy spawn and instantiate a VFX when is eventually spawned.")]
		[OnValueChanged("SpawnOnArenaValueChanged", false)]
		public bool SpawnOnArena;

		[FoldoutGroup("Spawning Options", 0)]
		[ShowIf("SpawnOnArena", true)]
		public float SpawningDelayOnArena = 0.8f;

		private WaitForSeconds _spawnOnArenaAwaiting;

		[FoldoutGroup("Spawning Options", 0)]
		[ShowIf("SpawnOnArena", true)]
		public GameObject SpawnVfx;

		[FoldoutGroup("Spawning Options", 0)]
		[ShowIf("SpawnOnArena", true)]
		public Vector2 SpawnEffectOffsetPosition;

		[FoldoutGroup("Spawning Options", 0)]
		[Tooltip("Spawns the enemy behaviour enabled.")]
		public bool EnableBehaviourOnLoad = true;

		[FoldoutGroup("Spawning Options", 0)]
		[Tooltip("Enables the enemy spawning persistence.")]
		public bool EnablePersistence = true;

		[FoldoutGroup("Spawning Options", 0)]
		[Tooltip("Disables enemy spawn. Only for Cherubs")]
		public bool EnemySpawnDisabled;

		[SerializeField]
		[BoxGroup("Attached References", true, false, 0)]
		private SpriteRenderer preview;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		[FormerlySerializedAs("enemy")]
		[InlineEditor(InlineEditorModes.LargePreview)]
		private GameObject selectedEnemy;

		private bool isEnemyEventsSuscribed;

		[SerializeField]
		[BoxGroup("Attached References", true, false, 0)]
		private Transform spawnPoint;

		[ValidateInput("ValidateInput", "Name must have more than 3 characters!", InfoMessageType.Error)]
		public string EntityName;

		public bool EnableInfluenceArea;

		[ShowIf("EnableInfluenceArea", true)]
		public float InfluenceAreaRadius = 1f;
	}
}
