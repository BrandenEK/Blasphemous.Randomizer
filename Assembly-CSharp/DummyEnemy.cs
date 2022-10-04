using System;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Enemies.Framework.Physics;
using Gameplay.GameControllers.Entities;
using UnityEngine;

public class DummyEnemy : Enemy, IDamageable
{
	public void Damage(Hit hit)
	{
	}

	public override EnemyAttack EnemyAttack()
	{
		throw new NotImplementedException();
	}

	public override EnemyBumper EnemyBumper()
	{
		throw new NotImplementedException();
	}

	public override EnemyFloorChecker EnemyFloorChecker()
	{
		throw new NotImplementedException();
	}

	public Vector3 GetPosition()
	{
		throw new NotImplementedException();
	}

	protected override void EnablePhysics(bool enable = true)
	{
		throw new NotImplementedException();
	}
}
