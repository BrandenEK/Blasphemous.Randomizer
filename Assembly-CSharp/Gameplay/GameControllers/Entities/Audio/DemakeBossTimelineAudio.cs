using System;
using UnityEngine;

namespace Gameplay.GameControllers.Entities.Audio
{
	public class DemakeBossTimelineAudio : DemakeEnemyAudio
	{
		public void PlayAttack()
		{
			Debug.Log("Fake Play Attack");
		}

		public void AttackEvent()
		{
			Debug.Log("Fake AttackEvent");
		}

		public void SetAttackMoveParam()
		{
			Debug.Log("Fake SetAttackMoveParam");
		}
	}
}
