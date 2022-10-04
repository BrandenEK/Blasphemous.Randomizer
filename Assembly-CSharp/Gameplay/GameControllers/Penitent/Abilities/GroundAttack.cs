using System;
using System.Collections;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Gameplay.GameControllers.Penitent.Abilities
{
	public class GroundAttack : LegacyAbility
	{
		protected override void OnAbilityStart()
		{
			Debug.Log("Ground Attack");
			this.entity.Animator.SetTrigger("ATTACK");
			base.StartCoroutine(this.DamageDelay());
		}

		private IEnumerator DamageDelay()
		{
			yield return new WaitForSeconds(this.damageDelay);
			Entity[] affectedEntities = this.affectedArea.GetTouchedEntities();
			for (int i = 0; i < affectedEntities.Length; i++)
			{
				affectedEntities[i].Damage(this.entity.Stats.Strength.Base, string.Empty);
			}
			yield break;
		}

		[SerializeField]
		private float damageDelay = 1f;

		[SerializeField]
		private CollisionSensor affectedArea;
	}
}
