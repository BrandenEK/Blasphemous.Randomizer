using System;
using Framework.Managers;
using Tools.Level.Layout;
using UnityEngine;

namespace Gameplay.GameControllers.Entities.Animations
{
	public class EntityAnimationEvents : MonoBehaviour
	{
		private void Start()
		{
			this.OnStart();
		}

		public bool AnimationsFXReady { get; set; }

		protected virtual void OnStart()
		{
			this.currentLevel = Core.Logic.CurrentLevelConfig;
		}

		private void Update()
		{
			this.OnUpdate();
		}

		protected virtual void OnUpdate()
		{
		}

		public virtual void Rebound()
		{
		}

		protected LevelInitializer currentLevel;
	}
}
