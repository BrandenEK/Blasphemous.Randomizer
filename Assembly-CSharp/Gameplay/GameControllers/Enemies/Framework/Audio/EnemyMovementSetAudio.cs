using System;
using Framework.Managers;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Framework.Audio
{
	public abstract class EnemyMovementSetAudio : MonoBehaviour
	{
		public abstract void SetFxIdByFloor(string floorTag);

		protected FMODAudioManager audioManager;

		public LayerMask floorLayerMask;
	}
}
