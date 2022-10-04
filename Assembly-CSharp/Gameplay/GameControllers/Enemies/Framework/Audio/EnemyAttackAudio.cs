using System;
using Framework.Managers;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Framework.Audio
{
	public abstract class EnemyAttackAudio : MonoBehaviour
	{
		public abstract void SetFxIdBySurface(string surface);

		protected FMODAudioManager audioManager;
	}
}
