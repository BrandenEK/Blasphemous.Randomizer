using System;
using System.Linq;
using Framework.Managers;
using Gameplay.GameControllers.Effects.Entity;
using Gameplay.GameControllers.Enemies.BellGhost;
using Gameplay.GameControllers.Enemies.ChainedAngel.AI;
using Gameplay.GameControllers.Enemies.ChainedAngel.Animator;
using Gameplay.GameControllers.Enemies.ChainedAngel.Audio;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Enemies.Framework.Damage;
using Gameplay.GameControllers.Enemies.Framework.Physics;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.StateMachine;
using Gameplay.GameControllers.Penitent;
using Tools.Level.Layout;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.ChainedAngel
{
	public class ChainedAngel : Enemy, IDamageable
	{
		public BodyChainMaster BodyChainMaster { get; private set; }

		public BellGhostFloatingMotion FloatingMotion { get; private set; }

		public VisionCone VisionCone { get; private set; }

		public ChainedAngelBehaviour Behaviour { get; private set; }

		public EnemyDamageArea DamageArea { get; private set; }

		public ColorFlash DamageEffect { get; private set; }

		public ChainedAngelAnimatorInjector AnimatorInjector { get; private set; }

		public StateMachine StateMachine { get; private set; }

		public ChainedAngelAudio Audio { get; private set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			this.BodyChainMaster = base.GetComponentInChildren<BodyChainMaster>();
			this.FloatingMotion = base.GetComponentInChildren<BellGhostFloatingMotion>();
			this.VisionCone = base.GetComponentInChildren<VisionCone>();
			this.Behaviour = base.GetComponentInChildren<ChainedAngelBehaviour>();
			this.DamageArea = base.GetComponentInChildren<EnemyDamageArea>();
			this.DamageEffect = base.GetComponentInChildren<ColorFlash>();
			this.AnimatorInjector = base.GetComponentInChildren<ChainedAngelAnimatorInjector>();
			this.StateMachine = base.GetComponentInChildren<StateMachine>();
			this.Audio = base.GetComponentInChildren<ChainedAngelAudio>();
			this.Behaviour.enabled = false;
			SpawnManager.OnPlayerSpawn += this.OnPlayerSpawn;
		}

		private void OnPlayerSpawn(Penitent penitent)
		{
			this.Behaviour.enabled = true;
			base.Target = penitent.gameObject;
		}

		protected override void OnStart()
		{
			base.OnStart();
			GameObject lowerLink = this.GetLowerLink();
			Vector3 spawnPosition = this.GetSpawnPosition();
			if (this.IsAnchored)
			{
				spawnPosition.y -= 2f;
			}
			lowerLink.transform.position = spawnPosition;
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

		public void Damage(Hit hit)
		{
			this.DamageArea.TakeDamage(hit, false);
			if (this.Status.Dead)
			{
				this.AnimatorInjector.Death();
				return;
			}
			this.DamageEffect.TriggerColorFlash();
			this.SleepTimeByHit(hit);
			if (this.BodyChainMaster.IsAttacking)
			{
				this.BodyChainMaster.Repullo();
			}
		}

		private Vector3 GetSpawnPosition()
		{
			Vector3 position = base.transform.position;
			EnemySpawnPoint enemySpawnPoint = Object.FindObjectsOfType<EnemySpawnPoint>().FirstOrDefault((EnemySpawnPoint x) => x.SpawningId == base.SpawningId);
			if (enemySpawnPoint)
			{
				position = enemySpawnPoint.Position;
			}
			return position;
		}

		public GameObject GetLowerLink()
		{
			GameObject result = null;
			float num = float.MaxValue;
			foreach (BodyChainLink bodyChainLink in this.BodyChainMaster.links)
			{
				if (bodyChainLink.transform.position.y < num)
				{
					num = bodyChainLink.transform.position.y;
					result = bodyChainLink.gameObject;
				}
			}
			return result;
		}

		public Vector3 GetPosition()
		{
			return base.transform.position;
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			SpawnManager.OnPlayerSpawn -= this.OnPlayerSpawn;
		}

		public bool IsAnchored;
	}
}
