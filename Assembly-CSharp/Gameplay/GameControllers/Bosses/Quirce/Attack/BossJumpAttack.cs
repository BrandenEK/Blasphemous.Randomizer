using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using BezierSplines;
using Framework.Managers;
using Framework.Pooling;
using Framework.Util;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Entities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.Quirce.Attack
{
	public class BossJumpAttack : EnemyAttack, IDirectAttack
	{
		public AttackArea AttackArea { get; set; }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<Vector2> OnJumpAdvancedEvent;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action OnJumpLanded;

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
		}

		public override void CurrentWeaponAttack()
		{
			base.CurrentWeaponAttack();
		}

		protected override void OnStart()
		{
			base.OnStart();
			this.AttackArea = base.GetComponentInChildren<AttackArea>();
			if (this.dealsDamage)
			{
				this.CreateHit();
			}
			this.CheckPools();
		}

		public bool IsInsideWall(Vector2 p)
		{
			RaycastHit2D[] array = new RaycastHit2D[1];
			p += Vector2.up;
			int num = Physics2D.LinecastNonAlloc(p + Vector2.right * 0.1f, p, array, this.groundLayerMask);
			return num > 0;
		}

		public Vector2 GetPointOutOfWall(Vector2 targetPoint, Vector2 dir, float skinWidth)
		{
			int num = 0;
			bool flag = false;
			Vector2 vector = Vector2.zero;
			RaycastHit2D[] array = new RaycastHit2D[1];
			targetPoint += Vector2.up * 2f;
			while (!flag && num < 100)
			{
				num++;
				int num2 = Physics2D.LinecastNonAlloc(targetPoint + dir * skinWidth, targetPoint, array, this.groundLayerMask);
				if (num2 > 0)
				{
					vector = array[0].point;
					Debug.DrawLine(vector, targetPoint, Color.red, 6f);
					targetPoint += dir * skinWidth;
				}
				else
				{
					vector += dir * skinWidth;
					Debug.DrawLine(vector, targetPoint, Color.green, 6f);
					flag = true;
				}
			}
			return vector;
		}

		public void Use(Transform parentToMove, Vector3 targetPoint)
		{
			this._parentToMove = parentToMove;
			this.OnJumped();
			if (this.IsInsideWall(targetPoint))
			{
				targetPoint = this.GetPointOutOfWall(targetPoint, Vector2.right * Mathf.Sign(parentToMove.transform.position.x - targetPoint.x), 0.5f);
			}
			RaycastHit2D[] array = new RaycastHit2D[1];
			int num = Physics2D.LinecastNonAlloc(targetPoint + Vector2.up * 2f, targetPoint + Vector2.down * 10f, array, this.groundLayerMask);
			if (num > 0)
			{
				Debug.DrawLine(targetPoint + Vector3.up, targetPoint + Vector3.down * 10f, Color.green);
				targetPoint = array[0].point;
				this._lastPoint = targetPoint;
				Vector3 vector = this.jumpCurve.GetControlPoint(2) - this.jumpCurve.GetControlPoint(3);
				this.jumpCurve.SetControlPoint(3, parentToMove.InverseTransformPoint(targetPoint));
				this.jumpCurve.SetControlPoint(2, parentToMove.InverseTransformPoint(targetPoint) + vector);
				base.StartCoroutine(this.JumpCoroutine(parentToMove, new Action(this.OnLanded)));
			}
			else
			{
				Debug.DrawLine(targetPoint + Vector3.up, targetPoint + Vector3.down * 10f, Color.red);
				GameplayUtils.DrawDebugCross(targetPoint + Vector3.down * 10f, Color.red, 5f);
				GameplayUtils.DrawDebugCross(targetPoint + Vector3.up * 2f, Color.yellow, 5f);
				GameObject gameObject = new GameObject("DEBUG_JUMP_ERROR");
				gameObject.transform.position = targetPoint;
				Debug.LogError("COULDNT JUMP, THERES NO FLOOR");
				this.OnLanded();
			}
		}

		public void StopJump()
		{
			this.forceJumpEnd = true;
			this.AttackArea.OnEnter -= this.OnPenitentEntersArea;
		}

		private IEnumerator JumpCoroutine(Transform parentToMove, Action callback = null)
		{
			this.alreadyInstantiated = new List<DashAttackInstantiations>();
			this.forceJumpEnd = false;
			float counter = 0f;
			Vector3 originPos = parentToMove.position;
			while (counter < this.moveSeconds && !this.forceJumpEnd)
			{
				float normalized = counter / this.moveSeconds;
				this.OnJumpAdvanced(normalized);
				float eased = this.easingCurve.Evaluate(normalized);
				parentToMove.position = originPos + parentToMove.InverseTransformPoint(this.jumpCurve.GetPoint(eased));
				Debug.DrawLine(base.transform.position, parentToMove.position + Vector3.up * 0.1f, Color.green);
				counter += Time.deltaTime;
				yield return null;
			}
			this.OnJumpAdvanced(1f);
			parentToMove.position = originPos + parentToMove.InverseTransformPoint(this.jumpCurve.GetPoint(1f));
			if (callback != null)
			{
				callback();
			}
			yield break;
		}

		private void OnJumpAdvanced(float nvalue)
		{
			if (this.OnJumpAdvancedEvent != null)
			{
				this.OnJumpAdvancedEvent(this.GetCurveDirection(nvalue));
			}
			foreach (DashAttackInstantiations dashAttackInstantiations in this.objectsToInstantiate)
			{
				if (dashAttackInstantiations.dashMoment <= nvalue && !this.alreadyInstantiated.Contains(dashAttackInstantiations))
				{
					this.InstantiateObject(dashAttackInstantiations);
				}
			}
		}

		private Vector2 GetCurveDirection(float n)
		{
			return this.jumpCurve.GetDirection(n);
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
			BossSpawnedAreaAttack component2 = gameObject.GetComponent<BossSpawnedAreaAttack>();
			if (component2 != null)
			{
				component2.SetOwner(base.EntityOwner);
			}
		}

		private void OnJumped()
		{
			if (this.dealsDamage)
			{
				this.AttackArea.OnEnter += this.OnPenitentEntersArea;
			}
		}

		private void OnLanded()
		{
			if (this.instantiateOnEnd != null)
			{
				this.InstantiateArea(this.instantiateOnEnd);
			}
			if (this.screenshakeOnEnd)
			{
				Core.Logic.CameraManager.ProCamera2DShake.Shake(this.shakeDuration, Vector3.down * this.shakeForce, this.vibrato, 0.2f, 0f, default(Vector3), 0f, false);
			}
			if (this.OnJumpLanded != null)
			{
				this.OnJumpLanded();
			}
			if (this.ShockWaveOnEnd)
			{
				Core.Logic.CameraManager.ShockwaveManager.Shockwave(base.transform.position, 0.8f, 0.3f, 1.8f);
			}
			if (this.dealsDamage)
			{
				this.AttackArea.OnEnter -= this.OnPenitentEntersArea;
			}
		}

		private void InstantiateArea(GameObject toInstantiate)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(toInstantiate, this._parentToMove.position, Quaternion.identity);
			BossSpawnedAreaAttack component = gameObject.GetComponent<BossSpawnedAreaAttack>();
			if (component != null)
			{
				component.SetOwner(base.EntityOwner);
			}
		}

		private void OnPenitentEntersArea(object sender, Collider2DParam e)
		{
			Debug.Log("ON PENITENT ENTERS THIS OBJECT AREA: " + base.gameObject.name);
			GameObject gameObject = e.Collider2DArg.gameObject;
			this._hit.OnGuardCallback = new Action<Hit>(this.OnGuard);
			gameObject.GetComponentInParent<IDamageable>().Damage(this._hit);
		}

		private void OnGuard(Hit h)
		{
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.red;
			Gizmos.DrawSphere(this._lastPoint, 0.25f);
		}

		public void CreateHit()
		{
			this._hit = new Hit
			{
				AttackingEntity = base.EntityOwner.gameObject,
				DamageType = this.DamageType,
				Force = this.Force,
				DamageAmount = (float)this.damage,
				HitSoundId = this.HitSound,
				Unnavoidable = this.unavoidable
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

		public BezierSpline jumpCurve;

		public AnimationCurve easingCurve;

		public LayerMask groundLayerMask;

		public float moveSeconds;

		public GameObject instantiateOnStart;

		public GameObject instantiateOnEnd;

		[FoldoutGroup("Instantiations", 0)]
		public List<DashAttackInstantiations> objectsToInstantiate;

		private List<DashAttackInstantiations> alreadyInstantiated;

		[FoldoutGroup("Damage", 0)]
		public bool dealsDamage = true;

		[ShowIf("dealsDamage", true)]
		[FoldoutGroup("Damage", 0)]
		public bool unavoidable;

		[ShowIf("dealsDamage", true)]
		[FoldoutGroup("Damage", 0)]
		public int damage;

		[FoldoutGroup("FX", 0)]
		public bool screenshakeOnEnd;

		[FoldoutGroup("FX", 0)]
		public bool ShockWaveOnEnd = true;

		[ShowIf("screenshakeOnEnd", true)]
		[FoldoutGroup("FX", 0)]
		public int vibrato = 40;

		[ShowIf("screenshakeOnEnd", true)]
		[FoldoutGroup("FX", 0)]
		public float shakeForce;

		[ShowIf("screenshakeOnEnd", true)]
		[FoldoutGroup("FX", 0)]
		public float shakeDuration;

		private Transform _parentToMove;

		private Coroutine _currentCoroutine;

		private Vector2 _lastPoint;

		private Hit _hit;

		private bool forceJumpEnd;
	}
}
