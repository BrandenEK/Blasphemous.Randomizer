using System;
using CreativeSpore.SmartColliders;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Combat;
using Gameplay.GameControllers.Effects.Entity;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Enemies.Framework.Damage;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Enemies.Framework.Physics;
using Gameplay.GameControllers.Enemies.ShieldMaiden.Animator;
using Gameplay.GameControllers.Enemies.ShieldMaiden.Audio;
using Gameplay.GameControllers.Enemies.ShieldMaiden.IA;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Penitent;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.ShieldMaiden
{
	public class ShieldMaiden : Enemy, IDamageable
	{
		public ShieldMaidenBehaviour Behaviour { get; set; }

		public NPCInputs Input { get; set; }

		public SmartPlatformCollider Collider { get; set; }

		public EnemyDamageArea DamageArea { get; private set; }

		public ShieldMaidenAnimatorInyector AnimatorInyector { get; set; }

		public EnemyAttack Attack { get; set; }

		public ColorFlash ColorFlash { get; set; }

		public EntityExecution EntExecution { get; set; }

		public VisionCone VisionCone { get; set; }

		public ShieldMaidenAudio Audio { get; private set; }

		public MotionLerper MotionLerper { get; private set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			this.Behaviour = base.GetComponent<ShieldMaidenBehaviour>();
			this.Input = base.GetComponent<NPCInputs>();
			base.Controller = base.GetComponent<PlatformCharacterController>();
			this.Collider = base.GetComponent<SmartPlatformCollider>();
			this.DamageArea = base.GetComponentInChildren<EnemyDamageArea>();
			this.Attack = base.GetComponentInChildren<EnemyAttack>();
			this.AnimatorInyector = base.GetComponentInChildren<ShieldMaidenAnimatorInyector>();
			this.ColorFlash = base.GetComponentInChildren<ColorFlash>();
			this.EntExecution = base.GetComponentInChildren<EntityExecution>();
			this.Audio = base.GetComponentInChildren<ShieldMaidenAudio>();
			this.VisionCone = base.GetComponentInChildren<VisionCone>();
			this.MotionLerper = base.GetComponentInChildren<MotionLerper>();
			this.contacts = new Collider2D[10];
		}

		protected override void OnStart()
		{
			base.OnStart();
			this.Status.CastShadow = true;
			this.Status.IsGrounded = true;
			SpawnManager.OnPlayerSpawn += this.OnPlayerSpawn;
		}

		private void OnPlayerSpawn(Penitent penitent)
		{
			base.Target = Core.Logic.Penitent.gameObject;
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			if (base.Target == null)
			{
				base.Target = Core.Logic.Penitent.gameObject;
			}
			if (!base.Landing)
			{
				base.Landing = true;
				this.SetPositionAtStart();
				base.Controller.PlatformCharacterPhysics.GravityScale = 1f;
			}
		}

		private void LateUpdate()
		{
			this.CheckOverlappingEnemies();
		}

		public bool IsOverlappingOtherEnemies()
		{
			Collider2D damageAreaCollider = this.DamageArea.DamageAreaCollider;
			int num = damageAreaCollider.OverlapCollider(this.filter, this.contacts);
			if (num > 0)
			{
				for (int i = 0; i < num; i++)
				{
					Enemy componentInChildren = this.contacts[i].GetComponentInChildren<Enemy>();
					if (componentInChildren != null)
					{
						return true;
					}
				}
			}
			return false;
		}

		public void CheckOverlappingEnemies()
		{
			Collider2D damageAreaCollider = this.DamageArea.DamageAreaCollider;
			int num = damageAreaCollider.OverlapCollider(this.filter, this.contacts);
			if (num > 0)
			{
				for (int i = 0; i < num; i++)
				{
					if (this.MotionLerper.IsLerping)
					{
						return;
					}
					Enemy componentInChildren = this.contacts[i].GetComponentInChildren<Enemy>();
					if (componentInChildren != null)
					{
						Vector2 v = new Vector2((float)((componentInChildren.transform.position.x >= base.transform.position.x) ? -1 : 1), 0f);
						if ((v.x == 1f && this.Status.Orientation == EntityOrientation.Left) || (v.x == -1f && this.Status.Orientation == EntityOrientation.Right))
						{
							this.Behaviour.OnBouncedBackByOverlapping();
						}
						this.MotionLerper.StartLerping(v);
					}
				}
			}
		}

		public override void SetPositionAtStart()
		{
			base.SetPositionAtStart();
			float groundDist = base.Controller.GroundDist;
			Vector3 position = new Vector3(base.transform.position.x, base.transform.position.y - groundDist, base.transform.position.z);
			base.transform.position = position;
			base.Controller.PlatformCharacterPhysics.GravityScale = 0f;
		}

		public override void Parry()
		{
			base.Parry();
			this.Behaviour.OnParry();
		}

		public void Damage(Hit hit)
		{
			if (this.GuardHit(hit))
			{
				this.SleepTimeByHit(hit);
				this.Audio.PlayHitShield();
				this.Behaviour.OnShieldHit();
				return;
			}
			if (this.Execution(hit))
			{
				this.GetStun(hit);
				return;
			}
			this.DamageArea.TakeDamage(hit, false);
			if (this.Status.Dead)
			{
				this.DamageArea.DamageAreaCollider.enabled = false;
				this.Behaviour.Death();
			}
			this.Behaviour.Damage();
			this.ColorFlash.TriggerColorFlash();
			this.SleepTimeByHit(hit);
		}

		public override void GetStun(Hit hit)
		{
			base.GetStun(hit);
			if (base.IsStunt)
			{
				return;
			}
			if (Mathf.Abs(base.Controller.SlopeAngle) < 1f)
			{
				this.AnimatorInyector.StopAll();
				Core.Audio.EventOneShotPanned(hit.HitSoundId, base.transform.position);
				this.Behaviour.Execution();
			}
		}

		public Vector3 GetPosition()
		{
			return base.transform.position;
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

		private new void OnDestroy()
		{
			SpawnManager.OnPlayerSpawn -= this.OnPlayerSpawn;
		}

		[FoldoutGroup("Overlap fixer settings", 0)]
		public ContactFilter2D filter;

		public Collider2D[] contacts;
	}
}
