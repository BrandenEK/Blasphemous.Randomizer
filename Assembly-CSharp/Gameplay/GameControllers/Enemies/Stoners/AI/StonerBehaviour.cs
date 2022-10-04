using System;
using Framework.FrameworkCore;
using Gameplay.GameControllers.Enemies.Framework.IA;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Stoners.AI
{
	public class StonerBehaviour : EnemyBehaviour
	{
		public float CurrentAtackWaitTime { get; set; }

		public bool IsRaised { get; set; }

		public bool IsVisible
		{
			get
			{
				return this._stoners.Status.IsVisibleOnCamera;
			}
		}

		public override void OnAwake()
		{
			base.OnAwake();
			this._stoners = base.GetComponent<Stoners>();
			this.Entity = this._stoners;
		}

		public override void OnStart()
		{
			base.OnStart();
			this._stoners.OnDeath += this.StonersOnEntityDie;
		}

		public void Raise(Vector3 targetPos)
		{
			this._stoners.AnimatorInyector.Raise(targetPos);
		}

		public override void Idle()
		{
		}

		public override void Wander()
		{
			throw new NotImplementedException();
		}

		public override void Chase(Transform targetPosition)
		{
			throw new NotImplementedException();
		}

		public override void Attack()
		{
			this._stoners.AnimatorInyector.Attack();
		}

		public override void Damage()
		{
			throw new NotImplementedException();
		}

		public override void StopMovement()
		{
			throw new NotImplementedException();
		}

		public override void LookAtTarget(Vector3 targetPos)
		{
			if (this.Entity == null)
			{
				return;
			}
			this.Entity.SpriteRenderer.flipX = false;
			if (targetPos.x >= this.Entity.transform.position.x)
			{
				if (this.Entity.Status.Orientation == EntityOrientation.Right)
				{
					return;
				}
				this._stoners.SetOrientation(EntityOrientation.Right, false, false);
				this._stoners.AnimatorInyector.AllowOrientation(true);
			}
			else
			{
				if (this.Entity.Status.Orientation == EntityOrientation.Left)
				{
					return;
				}
				this._stoners.SetOrientation(EntityOrientation.Left, false, false);
				this._stoners.AnimatorInyector.AllowOrientation(true);
			}
		}

		private void StonersOnEntityDie()
		{
			base.BehaviourTree.StopBehaviour();
		}

		private Stoners _stoners;
	}
}
