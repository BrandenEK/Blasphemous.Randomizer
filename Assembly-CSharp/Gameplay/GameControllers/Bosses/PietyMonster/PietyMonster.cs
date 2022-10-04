using System;
using CreativeSpore.SmartColliders;
using DamageEffect;
using FMODUnity;
using Framework.Managers;
using Gameplay.GameControllers.Bosses.PietyMonster.Animation;
using Gameplay.GameControllers.Bosses.PietyMonster.Attack;
using Gameplay.GameControllers.Bosses.PietyMonster.IA;
using Gameplay.GameControllers.Bosses.PietyMonster.Sound;
using Gameplay.GameControllers.Effects.Player.Dust;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Enemies.Framework.Damage;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Enemies.Framework.Physics;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.EntityGizmos;
using Gameplay.GameControllers.Penitent;
using Gameplay.GameControllers.Penitent.InputSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.PietyMonster
{
	public class PietyMonster : Enemy, IDamageable
	{
		public PietyMonsterAnimatorInyector AnimatorInyector { get; private set; }

		public NPCInputs Inputs { get; private set; }

		public PietyMonsterBehaviour PietyBehaviour { get; private set; }

		public BossBodyBarrier BodyBarrier { get; private set; }

		public DamageEffectScript DamageEffect { get; set; }

		public EnemyDamageArea DamageArea { get; set; }

		public PietyMonsterAudio Audio { get; set; }

		public EntityRootMotion SpitingMouth { get; set; }

		public PietyAnimatorBridge AnimatorBridge { get; set; }

		public StepDustSpawner DustSpawner { get; set; }

		public SmartPlatformCollider Collider { get; private set; }

		public EntityRumble Rumble { get; private set; }

		public PietyRootsManager PietyRootsManager { get; set; }

		public bool CanMove { get; set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			this.DamageArea = base.GetComponentInChildren<EnemyDamageArea>();
			this.Rumble = base.GetComponentInChildren<EntityRumble>();
			this.AnimatorInyector = base.GetComponentInChildren<PietyMonsterAnimatorInyector>();
			this.AnimatorBridge = base.GetComponentInChildren<PietyAnimatorBridge>();
			this.Inputs = base.GetComponent<NPCInputs>();
			base.EnemyBehaviour = base.GetComponentInChildren<EnemyBehaviour>();
			this.PietyBehaviour = (PietyMonsterBehaviour)base.EnemyBehaviour;
			this.BodyBarrier = base.GetComponentInChildren<BossBodyBarrier>();
			this.DamageEffect = base.GetComponentInChildren<DamageEffectScript>();
			this.Audio = base.GetComponentInChildren<PietyMonsterAudio>();
			base.Controller = base.GetComponent<PlatformCharacterController>();
			this.SpitingMouth = base.GetComponentInChildren<EntityRootMotion>();
			this.PietyRootsManager = UnityEngine.Object.FindObjectOfType<PietyRootsManager>();
			this.DustSpawner = base.GetComponentInChildren<StepDustSpawner>();
			this.Collider = base.GetComponent<SmartPlatformCollider>();
		}

		protected override void OnStart()
		{
			base.OnStart();
			SpawnManager.OnPlayerSpawn += this.OnPlayerSpawn;
			this.PietyBehaviour.PauseBehaviour();
		}

		private void OnPlayerSpawn(Penitent penitent)
		{
			base.Target = Core.Logic.Penitent.gameObject;
			SpawnManager.OnPlayerSpawn -= this.OnPlayerSpawn;
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			if (base.Target != null)
			{
				base.DistanceToTarget = Vector2.Distance(base.transform.position, base.Target.transform.position);
			}
			bool enable = base.DistanceToTarget <= this.ActivationRange;
			if (!this.Status.Dead)
			{
				this.EnablePiety(enable);
			}
		}

		public override EnemyFloorChecker EnemyFloorChecker()
		{
			return null;
		}

		public override EnemyAttack EnemyAttack()
		{
			return this.PietyBehaviour.StompAttack;
		}

		public override EnemyBumper EnemyBumper()
		{
			return null;
		}

		private void EnablePiety(bool enable = true)
		{
			this.EnablePhysics(enable);
		}

		protected override void EnablePhysics(bool enable = true)
		{
			if (this.Collider == null)
			{
				return;
			}
			if (enable)
			{
				if (!this.Collider.enabled)
				{
					this.Collider.enabled = true;
				}
				if (!base.Controller.enabled)
				{
					base.Controller.enabled = true;
				}
			}
			else
			{
				if (this.Collider.enabled)
				{
					this.Collider.enabled = false;
				}
				if (base.Controller.enabled)
				{
					base.Controller.enabled = false;
				}
			}
		}

		private void EnableBehaviour(bool enable = true)
		{
			if (this.PietyBehaviour == null)
			{
				return;
			}
			if (enable)
			{
				if (!this.PietyBehaviour.enabled)
				{
					this.PietyBehaviour.enabled = true;
				}
				this.PietyBehaviour.StartBehaviour();
			}
			else
			{
				if (this.PietyBehaviour.enabled)
				{
					this.PietyBehaviour.enabled = false;
				}
				this.PietyBehaviour.PauseBehaviour();
			}
		}

		public void Damage(Hit hit)
		{
			if (this.Status.Dead || Core.Logic.Penitent.Stats.Life.Current <= 0f)
			{
				return;
			}
			if (this.WillDieByHit(hit))
			{
				hit.HitSoundId = this.finalHit;
			}
			this.DamageArea.TakeDamage(hit, false);
			if (this.Status.Dead)
			{
				Core.Logic.ScreenFreeze.Freeze(0.01f, 2f, 0f, this.slowTimeCurve);
				Core.Logic.CameraManager.ScreenEffectsManager.ScreenModeBW(0.5f);
			}
			else
			{
				this.DamageEffect.Blink(0f, 0.1f);
				this.SleepTimeByHit(hit);
			}
		}

		public Vector3 GetPosition()
		{
			return base.transform.position;
		}

		public AnimationCurve slowTimeCurve;

		[SerializeField]
		[BoxGroup("Audio", true, false, 0)]
		[EventRef]
		private string finalHit;

		public PietyMonster.BossStatus CurrentBossStatus;

		public enum BossStatus
		{
			First,
			Second,
			Third,
			Forth,
			Fifth,
			Sixth
		}
	}
}
