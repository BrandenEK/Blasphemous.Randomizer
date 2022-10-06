using System;
using CreativeSpore.SmartColliders;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Combat;
using Gameplay.GameControllers.Effects.Player.Flash;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Enemies.Framework.Physics;
using Sirenix.OdinInspector;
using Tools.Level.Layout;
using UnityEngine;

namespace Gameplay.GameControllers.Entities
{
	[RequireComponent(typeof(EnemyBehaviour))]
	public abstract class Enemy : Entity
	{
		public SpriteRenderer srpiteRenderer { get; private set; }

		public bool IsGuarding { get; set; }

		public bool IsFalling { get; set; }

		public bool IsAttacking { get; set; }

		public bool IsChasing { get; set; }

		public bool Landing { get; set; }

		public bool DebugExecutionActive { get; set; }

		public bool IsStunt { get; set; }

		public float DistanceToTarget { get; set; }

		public GameObject Target { get; set; }

		public int SpawningId { get; set; }

		public EnemyBehaviour EnemyBehaviour { get; set; }

		public PlatformCharacterController Controller { get; set; }

		protected override void OnStart()
		{
			base.OnStart();
			if (this.CanGuard && this.GuardEffect != null)
			{
				PoolManager.Instance.CreatePool(this.GuardEffect, 3);
			}
			this.spriteRenderer = base.GetComponentInChildren<SpriteRenderer>();
			if (this.UseHealthBar)
			{
				Core.UI.AttachHealthBarToEnemy(this);
			}
		}

		public abstract EnemyFloorChecker EnemyFloorChecker();

		public abstract EnemyAttack EnemyAttack();

		public abstract EnemyBumper EnemyBumper();

		protected abstract void EnablePhysics(bool enable = true);

		public override bool BleedOnImpact()
		{
			return this.bleedOnImpact;
		}

		public override bool SparkOnImpact()
		{
			return this.sparksOnImpact;
		}

		protected virtual bool WillDieByHit(Hit h)
		{
			return this.Stats.Life.Current <= h.DamageAmount;
		}

		public virtual void Parry()
		{
		}

		public virtual bool Execution(Hit hit)
		{
			if (this.DebugExecutionActive && !this.IsOnNonExecutionPlatform)
			{
				return true;
			}
			if (!this.IsExecutable)
			{
				return false;
			}
			bool isTriggeredRiposte = Core.Logic.Penitent.ActiveRiposte.IsTriggeredRiposte;
			bool casting = Core.Logic.Penitent.LungeAttack.Casting;
			bool releaseChargedAttack = Core.Logic.Penitent.ReleaseChargedAttack;
			bool flag = isTriggeredRiposte;
			if (hit.DamageAmount >= base.CurrentLife && Core.Logic.ExecutionController.CurrentEnemyCanBeExecuted && hit.DamageType == DamageArea.DamageType.Heavy)
			{
				flag = true;
			}
			if (hit.DamageType == DamageArea.DamageType.Heavy && (casting || releaseChargedAttack) && this.IsAttacking)
			{
				flag = true;
			}
			if (hit.DamageType == DamageArea.DamageType.Stunt)
			{
				flag = true;
			}
			else if (hit.DamageType == DamageArea.DamageType.OptionalStunt)
			{
				if (this.Stats.Life.Current > hit.DamageAmount)
				{
					flag = true;
				}
				else
				{
					EntityExecution component = base.GetComponent<EntityExecution>();
					if (component != null)
					{
						component.DestroyExecution();
					}
				}
			}
			if (flag)
			{
				Core.Metrics.CustomEvent("EXECUTION_SUCCESS", string.Empty, -1f);
				if (this.Stats.Life.Current <= hit.DamageAmount)
				{
					base.Damage(this.Stats.Life.Current - 1f, string.Empty);
				}
				else
				{
					base.Damage(hit.DamageAmount, string.Empty);
				}
				if (isTriggeredRiposte)
				{
					Core.Logic.Penitent.ActiveRiposte.MakeRiposte();
				}
				if (Enemy.OnExecutionFired != null)
				{
					Enemy.OnExecutionFired();
				}
			}
			return flag && !this.IsOnNonExecutionPlatform;
		}

		private bool IsOnNonExecutionPlatform
		{
			get
			{
				LayerMask layerMask;
				if (this.Controller)
				{
					layerMask = this.Controller.SmartPlatformCollider.LayerCollision;
				}
				else
				{
					layerMask = LayerMask.NameToLayer("Floor");
				}
				RaycastHit2D raycastHit2D = Physics2D.Raycast(base.transform.position, -base.transform.up, 2.5f, layerMask);
				return !raycastHit2D.collider || raycastHit2D.collider.GetComponentInChildren<NonExecutionPlatform>() != null;
			}
		}

		public bool IsHitGuarded(Hit hit)
		{
			return this.IsHitOnGuardedSide(hit) && this.IsGuarding && this.CanGuard;
		}

		public virtual bool GuardHit(Hit hit)
		{
			if (!this.IsHitGuarded(hit))
			{
				return false;
			}
			float num = 1f;
			Enemy.GuardSide guard = this.Guard;
			if (guard != Enemy.GuardSide.Front)
			{
				if (guard != Enemy.GuardSide.Back)
				{
					if (guard == Enemy.GuardSide.Both)
					{
						num = Mathf.Sign((hit.AttackingEntity.transform.position - base.transform.position).x);
					}
				}
				else if (this.Status.Orientation == EntityOrientation.Right)
				{
					num *= -1f;
				}
			}
			else if (this.Status.Orientation == EntityOrientation.Left)
			{
				num *= -1f;
			}
			this.InstantiateGuardEffect(num);
			if (this.OnHitGuarded != null)
			{
				this.OnHitGuarded(hit);
			}
			return true;
		}

		private bool IsHitOnGuardedSide(Hit hit)
		{
			Vector3 position = hit.AttackingEntity.transform.position;
			Vector3 position2 = base.transform.position;
			bool result = false;
			Enemy.GuardSide guard = this.Guard;
			if (guard != Enemy.GuardSide.Front)
			{
				if (guard != Enemy.GuardSide.Back)
				{
					if (guard == Enemy.GuardSide.Both)
					{
						result = true;
					}
				}
				else
				{
					if (position.x < position2.x && this.Status.Orientation == EntityOrientation.Right)
					{
						result = true;
					}
					if (position.x >= position2.x && this.Status.Orientation == EntityOrientation.Left)
					{
						result = true;
					}
				}
			}
			else
			{
				if (position.x < position2.x && this.Status.Orientation == EntityOrientation.Left)
				{
					result = true;
				}
				if (position.x >= position2.x && this.Status.Orientation == EntityOrientation.Right)
				{
					result = true;
				}
			}
			return result;
		}

		private void InstantiateGuardEffect(float offsetSign)
		{
			Vector3 vector = this.GuardEffectOffset;
			vector.x *= offsetSign;
			Vector3 position = base.transform.position + vector;
			if (this.GuardEffect)
			{
				GameObject gameObject = PoolManager.Instance.ReuseObject(this.GuardEffect, position, Quaternion.identity, false, 1).GameObject;
				SpriteRenderer componentInChildren = gameObject.GetComponentInChildren<SpriteRenderer>();
				componentInChildren.flipX = (offsetSign < 0f);
			}
		}

		public virtual void SetPositionAtStart()
		{
		}

		public virtual void GetStun(Hit hit)
		{
		}

		public virtual void GetStun()
		{
		}

		private void OnDrawGizmos()
		{
			if (this.showHealthbarGizmo)
			{
				Gizmos.color = Color.red;
				Gizmos.DrawWireCube(base.transform.position + this.healthOffset, new Vector3(1f, 0.125f, 1f));
				Vector2 vector = this.healthOffset;
				vector.x *= -1f;
				Gizmos.color = Color.magenta;
				Gizmos.DrawWireCube(base.transform.position + vector, new Vector3(1f, 0.125f, 1f));
			}
		}

		public static Core.SimpleEvent OnExecutionFired;

		public Core.SimpleEventParam OnHitGuarded;

		[FoldoutGroup("Guard Settings", 0)]
		public bool CanGuard;

		[FoldoutGroup("Guard Settings", 0)]
		public GameObject GuardEffect;

		[FoldoutGroup("Guard Settings", 0)]
		public Vector2 GuardEffectOffset;

		[FoldoutGroup("Guard Settings", 0)]
		public Enemy.GuardSide Guard;

		[FoldoutGroup("Damage Settings", 0)]
		public bool bleedOnImpact = true;

		[FoldoutGroup("Damage Settings", 0)]
		public bool sparksOnImpact = true;

		[BoxGroup("Health Bar", true, false, 0)]
		public bool UseHealthBar = true;

		[BoxGroup("Health Bar", true, false, 0)]
		public Vector2 healthOffset = new Vector2(0f, 2f);

		[BoxGroup("Health Bar", true, false, 0)]
		public bool showHealthbarGizmo = true;

		[Tooltip("Disables the enemy behaviour when is not visible on screen.")]
		public bool DisableBehaviourWhenInvisible;

		public bool IsParryable;

		public bool IsVulnerable;

		public float ActivationRange = 20f;

		[Range(0f, 100f)]
		public float ExecutionChance = 100f;

		public float StuntTime = 5f;

		public float purgePointsWhenDead = 10f;

		public bool DamageByContact = true;

		protected EnemyFloorChecker enemyFloorChecker;

		protected DamagedFlash damagedFlash;

		protected EnemyAttack enemyAttack;

		protected EnemyBumper enemyBumper;

		public enum GuardSide
		{
			Front,
			Back,
			Both
		}
	}
}
