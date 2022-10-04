using System;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.PietyMonster.Animation
{
	[RequireComponent(typeof(BoxCollider2D))]
	public class BossBodyBarrier : MonoBehaviour
	{
		public BoxCollider2D BossBodyBarrierCollider { get; private set; }

		public bool AvoidCollision { get; set; }

		private void Awake()
		{
			this.BossBodyBarrierCollider = base.GetComponent<BoxCollider2D>();
		}

		private void Start()
		{
			this._enemy = base.GetComponentInParent<Enemy>();
			if (this._enemy)
			{
				this._enemy.OnDeath += this.EnemyOnEntityDie;
			}
		}

		public void EnableCollider()
		{
			if (!this.BossBodyBarrierCollider.enabled)
			{
				this.BossBodyBarrierCollider.enabled = true;
			}
		}

		public void DisableCollider()
		{
			if (this.BossBodyBarrierCollider.enabled)
			{
				this.BossBodyBarrierCollider.enabled = false;
			}
		}

		private void EnemyOnEntityDie()
		{
			this.DisableCollider();
		}

		private void OnDestroy()
		{
			if (this._enemy)
			{
				this._enemy.OnDeath -= this.EnemyOnEntityDie;
			}
		}

		private Enemy _enemy;

		private Collider2D _targetCollider;

		private float _targetColliderWidth;

		private Entity _targetEntity;

		private Transform _targetTransform;

		public LayerMask TargetLayerMask;
	}
}
