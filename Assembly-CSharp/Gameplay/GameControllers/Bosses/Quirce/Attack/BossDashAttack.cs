using System;
using System.Collections.Generic;
using System.Diagnostics;
using Framework.Managers;
using Framework.Pooling;
using Framework.Util;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.Weapon;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.Quirce.Attack
{
	public class BossDashAttack : EnemyAttack, IDirectAttack, ISpawnerAttack, IPaintAttackCollider
	{
		public AttackArea AttackArea { get; set; }

		public void SetRotatingFunction(BossDashAttack.RotatingFunction f)
		{
			this.currentRotatingFunction = f;
		}

		public Hit BossDashHit
		{
			get
			{
				return this._hit;
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event BossDashAttack.OnDashAdvancedFunction OnDashAdvancedEvent;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<BossDashAttack> OnDashBlockedEvent;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action OnDashFinishedEvent;

		protected override void OnStart()
		{
			base.OnStart();
			this.AttackArea = base.GetComponentInChildren<AttackArea>();
			base.EntityOwner.OnDeath += this.OnDeath;
			if (this.dealsDamage)
			{
				this.CreateHit();
			}
			this.CheckPools();
			this.AttachShowScriptIfNeeded();
		}

		private void CheckPools()
		{
			foreach (DashAttackInstantiations dashAttackInstantiations in this.objectsToInstantiate)
			{
				PoolObject component = dashAttackInstantiations.prefabToInstantiate.GetComponent<PoolObject>();
				if (component)
				{
					PoolManager.Instance.CreatePool(component.gameObject, 1);
				}
			}
			foreach (DashAttackInstantiations dashAttackInstantiations2 in this.instantiateOnCollision)
			{
				PoolObject component2 = dashAttackInstantiations2.prefabToInstantiate.GetComponent<PoolObject>();
				if (component2)
				{
					PoolManager.Instance.CreatePool(component2.gameObject, 1);
				}
			}
		}

		public override void CurrentWeaponAttack()
		{
			base.CurrentWeaponAttack();
		}

		public void DashToPoint(Transform parentToMove, Vector2 point, float offset = 0f)
		{
			if (this._currentCoroutine != null)
			{
				base.StopCoroutine(this._currentCoroutine);
			}
			if (this.dealsDamage)
			{
				this.currentlyDealsDamage = true;
				this.AttackArea.OnEnter += this.OnPenitentEntersArea;
				this.CheckInitialArea();
			}
			this._parentToMove = parentToMove;
			this._targetPoint = point;
			Vector3 vector = this._targetPoint - parentToMove.transform.position;
			this._dashDir = vector;
			this._screenShakeDir = vector;
			this.willStopOnCollision = false;
			if (this.checkCollisions)
			{
				RaycastHit2D[] array = new RaycastHit2D[1];
				bool flag = Physics2D.LinecastNonAlloc(parentToMove.transform.position, this._targetPoint, array, this.collisionMask) > 0;
				if (flag)
				{
					Debug.DrawLine(array[0].point, array[0].point + Vector2.up * 0.25f, Color.red, 1f);
					this._targetPoint = array[0].point - vector.normalized * offset;
					this.willStopOnCollision = true;
				}
			}
			if (this.dashAlongGround)
			{
				RaycastHit2D[] array2 = new RaycastHit2D[1];
				Vector2 vector2 = parentToMove.transform.position + Vector3.up * 0.1f;
				bool flag2 = Physics2D.LinecastNonAlloc(vector2, vector2 - Vector2.up, array2, this.collisionMask) > 0;
				if (flag2)
				{
					base.transform.position = array2[0].point;
					this._targetPoint.y = base.transform.position.y;
				}
			}
			Debug.DrawLine(this._parentToMove.position, this._targetPoint, Color.red);
			if (this.rotateTowardsDirection && this.currentRotatingFunction != null)
			{
				this.currentRotatingFunction(parentToMove, this._targetPoint);
			}
			this._currentCoroutine = base.StartCoroutine(GameplayUtils.LerpMoveWithCurveCoroutine(this._parentToMove, this._parentToMove.position, this._targetPoint, this.curve, this.dashDuration, new Action<Transform>(this.OnDashFinished), new Action<float>(this.OnDashAdvanced)));
		}

		public void Dash(Transform parentToMove, Vector2 direction, float distance, float offset = 0f, bool updateHit = false)
		{
			if (updateHit)
			{
				this.CreateHit();
			}
			if (this._currentCoroutine != null)
			{
				base.StopCoroutine(this._currentCoroutine);
				this.CheckInitialArea();
			}
			if (this.dealsDamage)
			{
				this.currentlyDealsDamage = true;
				this.AttackArea.OnEnter += this.OnPenitentEntersArea;
			}
			this._parentToMove = parentToMove;
			this._targetPoint = parentToMove.position + direction.normalized * distance;
			if (this.dashAlongGround)
			{
				RaycastHit2D[] array = new RaycastHit2D[1];
				Vector2 vector = parentToMove.transform.position + Vector3.up * 0.1f;
				bool flag = Physics2D.LinecastNonAlloc(vector, vector - Vector2.up, array, this.collisionMask) > 0;
				if (flag)
				{
					base.transform.position = array[0].point;
				}
			}
			this.willStopOnCollision = false;
			if (this.checkCollisions)
			{
				RaycastHit2D[] array2 = new RaycastHit2D[1];
				bool flag2 = Physics2D.LinecastNonAlloc(parentToMove.transform.position + Vector3.up * 0.1f, this._targetPoint, array2, this.collisionMask) > 0;
				if (flag2)
				{
					this.willStopOnCollision = true;
					this._targetPoint = array2[0].point - direction * offset;
				}
			}
			this._dashDir = direction;
			this._screenShakeDir = direction;
			Debug.DrawLine(this._parentToMove.position, this._targetPoint, Color.red, 15f);
			if (this.rotateTowardsDirection && this.currentRotatingFunction != null)
			{
				this.currentRotatingFunction(parentToMove, this._targetPoint);
			}
			float seconds = this.dashDuration;
			if (this.useSpeed)
			{
				float num = Vector2.Distance(this._parentToMove.position, this._targetPoint);
				seconds = num / this.speed;
			}
			this.alreadyInstantiated = new List<DashAttackInstantiations>();
			this._currentCoroutine = base.StartCoroutine(GameplayUtils.LerpMoveWithCurveCoroutine(this._parentToMove, this._parentToMove.position, this._targetPoint, this.curve, seconds, new Action<Transform>(this.OnDashFinished), new Action<float>(this.OnDashAdvanced)));
		}

		public void StopDash(Transform parentToMove, bool launchFinishedCallback = true)
		{
			if (this._currentCoroutine != null)
			{
				base.StopCoroutine(this._currentCoroutine);
			}
			if (launchFinishedCallback)
			{
				this.OnDashFinished(parentToMove);
			}
		}

		public Vector3 GetTargetPoint()
		{
			return this._targetPoint;
		}

		private void OnPenitentEntersArea(object sender, Collider2DParam e)
		{
			GameObject gameObject = e.Collider2DArg.gameObject;
			if (this.stopsOnBlock)
			{
				this._hit.OnGuardCallback = new Action<Hit>(this.OnDashGuarded);
			}
			gameObject.GetComponentInParent<IDamageable>().Damage(this._hit);
		}

		private void CheckInitialArea()
		{
			if (this.dealsDamageOnStart)
			{
				base.GetComponentInChildren<Weapon>().Attack(this._hit);
			}
		}

		private void OnDashGuarded(Hit h)
		{
			Debug.Log("<color=cyan>DASH GUARDED</color>");
			if (this.OnDashBlockedEvent != null)
			{
				this.OnDashBlockedEvent(this);
			}
			this.StopDash(this._parentToMove, true);
		}

		private void OnDashAdvanced(float nvalue)
		{
			foreach (DashAttackInstantiations dashAttackInstantiations in this.objectsToInstantiate)
			{
				if (dashAttackInstantiations.dashMoment <= nvalue && !this.alreadyInstantiated.Contains(dashAttackInstantiations))
				{
					this.InstantiateObject(dashAttackInstantiations);
				}
			}
			if (this.OnDashAdvancedEvent != null)
			{
				this.OnDashAdvancedEvent(nvalue);
			}
		}

		public void OnDashFinished(Transform parentToMove)
		{
			if (parentToMove == null)
			{
				return;
			}
			parentToMove.rotation = Quaternion.identity;
			if (this.dealsDamage && this.AttackArea != null)
			{
				this.currentlyDealsDamage = false;
				this.AttackArea.OnEnter -= this.OnPenitentEntersArea;
			}
			if (this.screenshakeOnEnd)
			{
				Core.Logic.CameraManager.ProCamera2DShake.Shake(this.shakeDuration, this._screenShakeDir * this.shakeForce, this.vibrato, 0.1f, 0f, default(Vector3), 0.06f, false);
			}
			if (this.willStopOnCollision && this.instantiateOnCollision != null && this.instantiateOnCollision.Count > 0)
			{
				this.InstantiateCollisionEffect();
			}
			if (this.OnDashFinishedEvent != null)
			{
				this.OnDashFinishedEvent();
			}
		}

		private void InstantiateCollisionEffect()
		{
			foreach (DashAttackInstantiations objectConfig in this.instantiateOnCollision)
			{
				this.InstantiateObject(objectConfig);
			}
		}

		private void InstantiateObject(DashAttackInstantiations objectConfig)
		{
			this.alreadyInstantiated.Add(objectConfig);
			PoolObject component = objectConfig.prefabToInstantiate.GetComponent<PoolObject>();
			GameObject gameObject;
			if (component != null)
			{
				gameObject = PoolManager.Instance.ReuseObject(component.gameObject, this._parentToMove.position + objectConfig.offset, Quaternion.identity, false, 1).GameObject;
			}
			else
			{
				gameObject = Object.Instantiate<GameObject>(objectConfig.prefabToInstantiate, this._parentToMove.position + objectConfig.offset, Quaternion.identity);
			}
			if (!objectConfig.keepRotation)
			{
				gameObject.transform.localEulerAngles = new Vector3(0f, 0f, Mathf.Atan2(this._dashDir.y, this._dashDir.x) * 57.29578f);
			}
			BossSpawnedAreaAttack component2 = gameObject.GetComponent<BossSpawnedAreaAttack>();
			if (component2 != null)
			{
				component2.SetOwner(base.EntityOwner);
			}
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(this._targetPoint, 0.25f);
			Gizmos.DrawWireSphere(this._targetPoint, 5f);
		}

		public void CreateHit()
		{
			this._hit = new Hit
			{
				AttackingEntity = base.EntityOwner.gameObject,
				DamageAmount = (float)this.damage,
				DamageType = this.DamageType,
				DamageElement = this.DamageElement,
				Unnavoidable = this.unavoidable,
				HitSoundId = this.HitSound,
				Unblockable = this.unblockable,
				Force = this.Force
			};
		}

		public void SetDamage(int damage)
		{
			if (damage < 0)
			{
				return;
			}
			this.damage = damage;
			this.CreateHit();
		}

		private void OnDeath()
		{
			base.EntityOwner.OnDeath -= this.OnDeath;
			if (this._parentToMove != null)
			{
				this.StopDash(this._parentToMove, true);
			}
		}

		public void SetSpawnsDamage(int damage)
		{
			foreach (DashAttackInstantiations dashAttackInstantiations in this.objectsToInstantiate)
			{
				IDirectAttack component = dashAttackInstantiations.prefabToInstantiate.GetComponent<IDirectAttack>();
				if (component != null)
				{
					component.SetDamage(damage);
				}
			}
			this.CreateSpawnsHits();
		}

		public void CreateSpawnsHits()
		{
			foreach (DashAttackInstantiations dashAttackInstantiations in this.objectsToInstantiate)
			{
				IDirectAttack component = dashAttackInstantiations.prefabToInstantiate.GetComponent<IDirectAttack>();
				if (component != null)
				{
					component.CreateHit();
				}
			}
		}

		public bool IsCurrentlyDealingDamage()
		{
			return this.currentlyDealsDamage;
		}

		public void AttachShowScriptIfNeeded()
		{
		}

		public AnimationCurve curve;

		public float dashDuration;

		public bool checkCollisions = true;

		public bool dashAlongGround;

		public LayerMask collisionMask;

		private Transform _parentToMove;

		private Vector3 _targetPoint;

		private Coroutine _currentCoroutine;

		[FoldoutGroup("Graphics", 0)]
		public bool rotateTowardsDirection;

		[FoldoutGroup("Instantiations", 0)]
		public List<DashAttackInstantiations> objectsToInstantiate;

		[ShowIf("checkCollisions", true)]
		[FoldoutGroup("Instantiations", 0)]
		public List<DashAttackInstantiations> instantiateOnCollision;

		[FoldoutGroup("Damage", 0)]
		public bool dealsDamage = true;

		[ShowIf("dealsDamage", true)]
		[FoldoutGroup("Damage", 0)]
		public bool unavoidable;

		[ShowIf("dealsDamage", true)]
		[FoldoutGroup("Damage", 0)]
		public bool unblockable;

		[ShowIf("dealsDamage", true)]
		[FoldoutGroup("Damage", 0)]
		public bool stopsOnBlock = true;

		[ShowIf("dealsDamage", true)]
		[FoldoutGroup("Damage", 0)]
		public bool dealsDamageOnStart;

		[ShowIf("dealsDamage", true)]
		[FoldoutGroup("Damage", 0)]
		public int damage;

		[FoldoutGroup("Screenshake", 0)]
		public bool screenshakeOnEnd;

		[FoldoutGroup("Screenshake", 0)]
		[ShowIf("screenshakeOnEnd", true)]
		public int vibrato = 40;

		[FoldoutGroup("Screenshake", 0)]
		[ShowIf("screenshakeOnEnd", true)]
		public float shakeForce;

		[FoldoutGroup("Screenshake", 0)]
		[ShowIf("screenshakeOnEnd", true)]
		public float shakeDuration;

		public bool useSpeed;

		[ShowIf("useSpeed", true)]
		public float speed;

		private List<DashAttackInstantiations> alreadyInstantiated;

		private Hit _hit;

		private BossDashAttack.RotatingFunction currentRotatingFunction;

		private bool willStopOnCollision;

		private bool currentlyDealsDamage;

		private Vector2 _screenShakeDir;

		private Vector2 _dashDir;

		public delegate void OnDashAdvancedFunction(float value);

		public delegate void RotatingFunction(Transform parentToRotate, Vector3 point);
	}
}
