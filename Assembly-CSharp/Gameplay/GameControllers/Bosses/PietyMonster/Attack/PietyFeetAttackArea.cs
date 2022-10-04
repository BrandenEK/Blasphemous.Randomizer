using System;
using Gameplay.GameControllers.Bosses.PietyMonster.IA;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.PietyMonster.Attack
{
	[RequireComponent(typeof(Collider2D))]
	public class PietyFeetAttackArea : MonoBehaviour
	{
		private void Awake()
		{
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			if ((this.TargetLayerMask.value & 1 << other.gameObject.layer) <= 0)
			{
				return;
			}
			if (!this.PietyMonsterBehaviour.TargetOnRange)
			{
				this.PietyMonsterBehaviour.TargetOnRange = true;
			}
		}

		private void OnTriggerExit2D(Collider2D other)
		{
			if ((this.TargetLayerMask.value & 1 << other.gameObject.layer) <= 0)
			{
				return;
			}
			if (this.PietyMonsterBehaviour.TargetOnRange)
			{
				this.PietyMonsterBehaviour.TargetOnRange = false;
			}
		}

		public LayerMask TargetLayerMask;

		public PietyMonsterBehaviour PietyMonsterBehaviour;
	}
}
