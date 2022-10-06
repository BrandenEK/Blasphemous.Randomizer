using System;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.FlyingPortrait.AI
{
	public class FlyingPortraitBehaviour : EnemyBehaviour
	{
		public bool IsAwake { get; set; }

		public override void OnStart()
		{
			base.OnStart();
			this.FlyingPortrait = (FlyingPortrait)this.Entity;
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			if (this.FlyingPortrait.Status.Dead || base.GotParry)
			{
				this.FlyingPortrait.StateMachine.SwitchState<FlyingPortraitDeathState>();
			}
			else if (base.PlayerSeen || base.PlayerHeard)
			{
				this.FlyingPortrait.StateMachine.SwitchState<FlyingPortraitAttackState>();
			}
			else
			{
				this.FlyingPortrait.StateMachine.SwitchState<FlyingPortraitWanderState>();
			}
		}

		public override void Idle()
		{
			throw new NotImplementedException();
		}

		public override void Wander()
		{
			float num = (this.Entity.Status.Orientation != EntityOrientation.Right) ? -1f : 1f;
			base.transform.Translate(Vector2.right * num * Time.deltaTime, 0);
		}

		public override void Chase(Transform targetPosition)
		{
			if (base.IsAttacking)
			{
				return;
			}
			float num = (targetPosition.transform.position.x <= this.Entity.transform.position.x) ? -1f : 1f;
			base.transform.Translate(Vector2.right * num * this.ChasingSpeed * Time.deltaTime, 0);
		}

		public override void Parry()
		{
			base.Parry();
			base.GotParry = true;
			this.FlyingPortrait.AnimatorInjector.ParryReaction();
		}

		public override void Execution()
		{
			base.Execution();
			base.GotParry = true;
			this.FlyingPortrait.gameObject.layer = LayerMask.NameToLayer("Default");
			this.FlyingPortrait.SpriteRenderer.enabled = false;
			this.FlyingPortrait.Attack.gameObject.SetActive(false);
			Core.Logic.Penitent.Audio.PlaySimpleHitToEnemy();
			this.FlyingPortrait.Execution.InstantiateExecution();
			if (this.FlyingPortrait.Execution != null)
			{
				this.FlyingPortrait.Execution.enabled = true;
			}
		}

		public override void Alive()
		{
			base.Alive();
			base.GotParry = false;
			this.FlyingPortrait.gameObject.layer = LayerMask.NameToLayer("Enemy");
			this.FlyingPortrait.SpriteRenderer.enabled = true;
			this.FlyingPortrait.Attack.gameObject.SetActive(true);
			this.FlyingPortrait.CurrentLife = this.FlyingPortrait.Stats.Life.Base / 2f;
			this.FlyingPortrait.AnimatorInjector.Alive();
			if (this.FlyingPortrait.Execution != null)
			{
				this.FlyingPortrait.Execution.enabled = false;
			}
		}

		public override void Attack()
		{
			throw new NotImplementedException();
		}

		public override void Damage()
		{
			throw new NotImplementedException();
		}

		public override void StopMovement()
		{
			throw new NotImplementedException();
		}

		protected FlyingPortrait FlyingPortrait;

		[FoldoutGroup("Attack settings", true, 0)]
		public float AttackCoolDown = 2f;

		[FoldoutGroup("Attack settings", true, 0)]
		public float DistanceAttack = 3f;

		[FoldoutGroup("Attack settings", true, 0)]
		public float ChasingSpeed = 3f;
	}
}
