using System;
using Gameplay.GameControllers.Enemies.GhostKnight.AI;
using Gameplay.GameControllers.Enemies.GhostKnight.Attack;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.GhostKnight.Animator
{
	public class GhostKnightAnimatorBridge : MonoBehaviour
	{
		public GhostKnightAttack GhostAttack { get; private set; }

		public GhostKnight GhostKnight { get; private set; }

		private void Awake()
		{
			this.GhostAttack = base.transform.root.GetComponentInChildren<GhostKnightAttack>();
			this.GhostKnight = this.GhostAttack.GetComponentInParent<GhostKnight>();
		}

		public void PlayStartAttack()
		{
			this.GhostKnight.Audio.StartAttack();
		}

		public void Attack()
		{
			if (this.GhostKnight == null)
			{
				return;
			}
			this.GhostKnight.EnemyBehaviour.Attack();
		}

		public void SwordHit(DamageArea.DamageType damageType)
		{
			if (this.GhostAttack == null)
			{
				return;
			}
			this.GhostAttack.CurrentWeaponAttack(damageType);
		}

		public void Dissappear()
		{
			if (this.GhostKnight == null)
			{
				return;
			}
			GhostKnightBehaviour componentInChildren = this.GhostKnight.GetComponentInChildren<GhostKnightBehaviour>();
			componentInChildren.Disappear(componentInChildren.TimeBecomeInVisible);
		}

		public void DestroyEnemy()
		{
			UnityEngine.Object.Destroy(this.GhostKnight.gameObject);
		}
	}
}
