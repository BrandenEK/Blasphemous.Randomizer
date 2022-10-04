using System;
using System.Collections.Generic;
using Framework.FrameworkCore;
using Framework.Managers;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Environment.AreaEffects
{
	[RequireComponent(typeof(Collider2D))]
	public class AreaEffect : MonoBehaviour
	{
		public bool IsPopulated { get; set; }

		private void Awake()
		{
			this.AreaCollider = base.GetComponent<Collider2D>();
			this.levelReady = false;
			LevelManager.OnLevelLoaded += this.LevelManager_OnLevelLoaded;
			this.OnAwake();
		}

		private void LevelManager_OnLevelLoaded(Level oldLevel, Level newLevel)
		{
			this.levelReady = true;
		}

		private void Start()
		{
			this.OnStart();
		}

		private void Update()
		{
			if (this.levelReady)
			{
				this.OnUpdate();
			}
		}

		protected virtual void OnDestroy()
		{
			LevelManager.OnLevelLoaded -= this.LevelManager_OnLevelLoaded;
		}

		protected virtual void OnAwake()
		{
		}

		protected virtual void OnStart()
		{
		}

		protected virtual void OnUpdate()
		{
			if (!this.IsPopulated)
			{
				return;
			}
			this._currentEffectLapse += Time.deltaTime;
			if (this._currentEffectLapse < this.EffectLapse)
			{
				return;
			}
			this._currentEffectLapse = 0f;
			if (!this.IsDisabled)
			{
				this.OnStayAreaEffect();
			}
		}

		protected virtual void OnEnterAreaEffect(Collider2D other)
		{
		}

		protected virtual void OnExitAreaEffect(Collider2D other)
		{
		}

		protected virtual void OnStayAreaEffect()
		{
		}

		public virtual void EnableEffect(bool enableEffect = true)
		{
			this.IsDisabled = !enableEffect;
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			if ((this.AffectedEntities.value & 1 << other.gameObject.layer) <= 0)
			{
				return;
			}
			this.IsPopulated = true;
			this.AddEntityToAreaPopulation(other.transform.gameObject);
			if (!this.IsDisabled)
			{
				this.OnEnterAreaEffect(other);
			}
		}

		private void OnTriggerExit2D(Collider2D other)
		{
			if ((this.AffectedEntities.value & 1 << other.gameObject.layer) <= 0)
			{
				return;
			}
			this.IsPopulated = false;
			this._currentEffectLapse = 0f;
			this.RemoveEntityToAreaPopulation(other.transform.gameObject);
			if (!this.IsDisabled)
			{
				this.OnExitAreaEffect(other);
			}
		}

		private void AddEntityToAreaPopulation(GameObject entity)
		{
			if (!this.Population.Contains(entity))
			{
				this.Population.Add(entity);
			}
		}

		protected void RemoveEntityToAreaPopulation(GameObject entity)
		{
			if (this.Population.Contains(entity))
			{
				this.Population.Remove(entity);
			}
		}

		private void OnDisable()
		{
			this.IsPopulated = false;
			this.Population.Clear();
		}

		protected Collider2D AreaCollider;

		public LayerMask AffectedEntities;

		[FoldoutGroup("Area Settings", true, 0)]
		public float EffectLapse;

		private float _currentEffectLapse;

		[FoldoutGroup("Area Settings", true, 0)]
		[SerializeField]
		public bool IsDisabled;

		protected List<GameObject> Population = new List<GameObject>();

		private bool levelReady;
	}
}
