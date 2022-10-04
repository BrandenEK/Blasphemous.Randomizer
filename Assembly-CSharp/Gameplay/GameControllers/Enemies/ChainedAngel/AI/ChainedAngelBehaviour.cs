using System;
using Framework.FrameworkCore;
using Gameplay.GameControllers.Enemies.Framework.IA;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.ChainedAngel.AI
{
	public class ChainedAngelBehaviour : EnemyBehaviour
	{
		public ChainedAngel ChainedAngel { get; private set; }

		public override void OnStart()
		{
			base.OnStart();
			this.ChainedAngel = (ChainedAngel)this.Entity;
			this.ChainedAngel.StateMachine.SwitchState<ChainedAngelIdleState>();
			this.ChainedAngel.OnDeath += this.OnDeath;
		}

		private void OnDeath()
		{
			this.ChainedAngel.OnDeath -= this.OnDeath;
			this.ChainedAngel.StateMachine.enabled = false;
			this.ChainedAngel.BodyChainMaster.ForceStopAttack();
		}

		public bool CanSeeTarget
		{
			get
			{
				return this.ChainedAngel.VisionCone.CanSeeTarget(this.ChainedAngel.Target.transform, "Penitent", false);
			}
		}

		public override void LookAtTarget(Vector3 targetPos)
		{
			if (targetPos.x > this.ChainedAngel.transform.position.x)
			{
				if (this.ChainedAngel.Status.Orientation == EntityOrientation.Right)
				{
					return;
				}
				this.ChainedAngel.SetOrientation(EntityOrientation.Right, true, false);
			}
			else
			{
				if (this.ChainedAngel.Status.Orientation == EntityOrientation.Left)
				{
					return;
				}
				this.ChainedAngel.SetOrientation(EntityOrientation.Left, true, false);
			}
		}

		public override void Idle()
		{
			throw new NotImplementedException();
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
			this.ChainedAngel.BodyChainMaster.SnakeAttack(this.AttackOffsetPosition, null);
			this.ChainedAngel.Audio.PlayAttack();
		}

		public override void Damage()
		{
			throw new NotImplementedException();
		}

		public override void StopMovement()
		{
			throw new NotImplementedException();
		}

		public float AttackLapse = 2f;

		public Vector2 AttackOffsetPosition;
	}
}
