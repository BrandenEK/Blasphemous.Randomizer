using System;
using System.Collections.Generic;
using System.Linq;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using Tools.Level.Layout;
using UnityEngine;

namespace Framework.Audio
{
	public class AudioLoader : MonoBehaviour
	{
		private void Awake()
		{
			this.AddCatalogItems();
		}

		private void Start()
		{
			this.RegisterCatalogs();
		}

		private void RegisterCatalogs()
		{
			if (this.AudioCatalogs.Length <= 0)
			{
				return;
			}
			for (int i = 0; i < this.AudioCatalogs.Length; i++)
			{
				if (this.AudioCatalogs[i] != null)
				{
					Core.Audio.RegisterCatalog(this.AudioCatalogs[i]);
				}
			}
		}

		private void AddCatalogItems()
		{
			List<FMODAudioCatalog> list = this.AudioCatalogs.ToList<FMODAudioCatalog>();
			FMODAudioCatalog[] enemiesAudioCatalogs = Core.Audio.EnemiesAudioCatalogs;
			IEnumerable<Type> levelEnemyTypes = this.GetLevelEnemyTypes();
			foreach (Type type in levelEnemyTypes)
			{
				foreach (FMODAudioCatalog fmodaudioCatalog in enemiesAudioCatalogs)
				{
					if (!(fmodaudioCatalog.Owner == null))
					{
						if (fmodaudioCatalog.Owner.GetType() == type && !list.Contains(fmodaudioCatalog))
						{
							list.Add(fmodaudioCatalog);
							if (fmodaudioCatalog.HasAssociatedCatalog && !list.Contains(fmodaudioCatalog.AssociatedCatalog))
							{
								list.Add(fmodaudioCatalog.AssociatedCatalog);
							}
							break;
						}
					}
				}
			}
			this.AudioCatalogs = list.ToArray();
		}

		private IEnumerable<Type> GetLevelEnemyTypes()
		{
			List<Type> list = new List<Type>();
			EnemySpawnPoint[] array = Object.FindObjectsOfType<EnemySpawnPoint>();
			foreach (EnemySpawnPoint enemySpawnPoint in array)
			{
				if (!(enemySpawnPoint.SelectedEnemy == null))
				{
					Type type = enemySpawnPoint.SelectedEnemy.GetComponentInChildren<Enemy>().GetType();
					if (!list.Contains(type))
					{
						list.Add(type);
					}
				}
			}
			return list;
		}

		public FMODAudioCatalog[] AudioCatalogs;
	}
}
