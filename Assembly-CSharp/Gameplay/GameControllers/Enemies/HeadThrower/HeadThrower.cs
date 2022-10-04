using System;
using Framework.Managers;
using Gameplay.GameControllers.Effects.Entity;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Enemies.Framework.Damage;
using Gameplay.GameControllers.Enemies.Framework.Physics;
using Gameplay.GameControllers.Enemies.HeadThrower.AI;
using Gameplay.GameControllers.Enemies.HeadThrower.Audio;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.HeadThrower
{
	public class HeadThrower : Enemy, IDamageable
	{
		public HeadThrowerAudio Audio { get; private set; }

		public EnemyDamageArea DamageArea { get; private set; }

		public ColorFlash ColorFlash { get; private set; }

		public void Damage(Hit hit)
		{
			this.DamageArea.TakeDamage(hit, false);
			base.EnemyBehaviour.Damage();
			this.ColorFlash.TriggerColorFlash();
			this.SleepTimeByHit(hit);
		}

		public Vector3 GetPosition()
		{
			return base.transform.position;
		}

		protected override void OnStart()
		{
			base.OnStart();
			this.Audio = base.GetComponentInChildren<HeadThrowerAudio>();
			this.DamageArea = base.GetComponentInChildren<EnemyDamageArea>();
			base.EnemyBehaviour = base.GetComponentInChildren<HeadThrowerBehaviour>();
			this.ColorFlash = base.GetComponentInChildren<ColorFlash>();
			base.Target = Core.Logic.Penitent.gameObject;
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			this.SetPositionAtStart();
			if (base.Target == null)
			{
				base.Target = Core.Logic.Penitent.gameObject;
			}
		}

		public override void SetPositionAtStart()
		{
			base.SetPositionAtStart();
			if (base.Landing)
			{
				return;
			}
			float distance = Physics2D.Raycast(base.transform.position, -Vector2.up, 5f, base.EnemyBehaviour.BlockLayerMask).distance;
			Vector3 position = base.transform.position;
			position.y -= distance;
			base.transform.position = position;
			base.Landing = true;
		}

		public override EnemyFloorChecker EnemyFloorChecker()
		{
			throw new NotImplementedException();
		}

		public override EnemyAttack EnemyAttack()
		{
			throw new NotImplementedException();
		}

		public override EnemyBumper EnemyBumper()
		{
			throw new NotImplementedException();
		}

		protected override void EnablePhysics(bool enable = true)
		{
			throw new NotImplementedException();
		}
	}
}
