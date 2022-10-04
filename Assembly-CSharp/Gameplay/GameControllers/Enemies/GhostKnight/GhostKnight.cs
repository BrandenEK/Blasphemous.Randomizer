using System;
using System.Linq;
using Framework.Managers;
using Gameplay.GameControllers.Effects.Entity;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Enemies.Framework.Damage;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Enemies.Framework.Physics;
using Gameplay.GameControllers.Enemies.GhostKnight.AI;
using Gameplay.GameControllers.Enemies.GhostKnight.Audio;
using Gameplay.GameControllers.Entities;
using Plugins.GhostSprites2D.Scripts.GhostSprites;
using Tools.Level.Layout;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.GhostKnight
{
	public class GhostKnight : Enemy, IDamageable
	{
		public MotionLerper MotionLerper { get; private set; }

		public Vector3 StartPoint { get; set; }

		public EnemyDamageArea DamageArea { get; private set; }

		public ColorFlash ColorFlash { get; private set; }

		public GhostKnightAudio Audio { get; private set; }

		public GhostSprites GhostSprites { get; set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			this.MotionLerper = base.GetComponent<MotionLerper>();
		}

		protected override void OnStart()
		{
			base.OnStart();
			base.Target = Core.Logic.Penitent.gameObject;
			this.StartPoint = new Vector2(base.transform.position.x, base.transform.position.y);
			this.DamageArea = base.GetComponentInChildren<EnemyDamageArea>();
			base.EnemyBehaviour = base.GetComponentInChildren<EnemyBehaviour>();
			this.ColorFlash = base.GetComponentInChildren<ColorFlash>();
			this.Audio = base.GetComponentInChildren<GhostKnightAudio>();
			this.GhostSprites = base.GetComponentInChildren<GhostSprites>(true);
			this.EnableAtStart(this.IsOnArena);
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			if (base.Target == null)
			{
				base.Target = Core.Logic.Penitent.gameObject;
			}
			base.DistanceToTarget = Vector2.Distance(base.transform.position, base.Target.transform.position);
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
			if (base.EnemyBehaviour.GotParry)
			{
				hit.HitSoundId = string.Empty;
			}
			if (!this.Status.Dead)
			{
				this.DamageArea.TakeDamage(hit, false);
			}
			base.EnemyBehaviour.Damage();
			base.EnemyBehaviour.GotParry = false;
		}

		public override void Parry()
		{
			base.Parry();
			base.EnemyBehaviour.GotParry = true;
			base.EnemyBehaviour.Parry();
		}

		public Vector3 GetPosition()
		{
			return base.transform.position;
		}

		private bool IsOnArena
		{
			get
			{
				EnemySpawnPoint enemySpawnPoint = UnityEngine.Object.FindObjectsOfType<EnemySpawnPoint>().First((EnemySpawnPoint x) => x.SpawningId == base.SpawningId);
				return enemySpawnPoint != null && enemySpawnPoint.SpawnOnArena;
			}
		}

		private void EnableAtStart(bool enableAtStart)
		{
			GhostKnightBehaviour ghostKnightBehaviour = base.EnemyBehaviour as GhostKnightBehaviour;
			if (!ghostKnightBehaviour)
			{
				return;
			}
			if (enableAtStart)
			{
				ghostKnightBehaviour.Appear(0f);
			}
			else
			{
				ghostKnightBehaviour.Disappear(0f);
			}
		}
	}
}
