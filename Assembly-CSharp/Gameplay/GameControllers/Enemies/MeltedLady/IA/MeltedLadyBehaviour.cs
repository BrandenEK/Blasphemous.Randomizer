using System;
using System.Collections;
using Framework.FrameworkCore;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Enemies.MeltedLady.Attack;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.MeltedLady.IA
{
	public class MeltedLadyBehaviour : EnemyBehaviour
	{
		[FoldoutGroup("Activation Settings", true, 0)]
		public float DistanceToTarget { get; private set; }

		public float TeleportCooldownLapse { get; set; }

		public bool CanTeleport { get; set; }

		public bool IsInOrigin { get; set; }

		public int CurrentAttackAmount
		{
			get
			{
				return this._currentAttackAmount;
			}
			set
			{
				this._currentAttackAmount = Mathf.Clamp(value, 0, this.AttackAmount);
			}
		}

		public Vector2 OriginPosition { get; private set; }

		public FloatingLady MeltedLady { get; private set; }

		public MeltedLadyTeleportPoint[] TeleportPoints { get; private set; }

		public MeltedLadyTeleportPoint CurrentTeleportPoint { get; private set; }

		public bool Awaken { get; private set; }

		private void SetMaxAttackDistance()
		{
			this.MaxAttackDistance = Mathf.Clamp(this.MaxAttackDistance, this.ActivationDistance, float.MaxValue);
		}

		public override void OnAwake()
		{
			base.OnAwake();
			this.MeltedLady = (FloatingLady)this.Entity;
			this._teleportYield = new WaitForSeconds(this.TeleportInterval);
		}

		public override void OnStart()
		{
			base.OnStart();
			this.MeltedLady.StateMachine.SwitchState<MeltedLadyIdleState>();
			Vector3 position = this.MeltedLady.transform.position;
			this.OriginPosition = new Vector2(position.x, position.y);
			this.CurrentAttackAmount = this.AttackAmount;
			this.TeleportPoints = UnityEngine.Object.FindObjectsOfType<MeltedLadyTeleportPoint>();
			if (this.TeleportPoints.Length < 1)
			{
				Debug.LogError("You have to add at least one teleport point to the scene.");
				this.MeltedLady.gameObject.SetActive(false);
			}
			this.MeltedLady.OnDamaged += this.OnDamaged;
		}

		private void OnDamaged()
		{
			bool flag = this.MeltedLady is InkLady;
			if (flag && this.MeltedLady.IsAttacking)
			{
				return;
			}
			this.MeltedLady.Status.IsHurt = true;
			this.MeltedLady.DamageArea.DamageAreaCollider.enabled = false;
			base.StopCoroutine(this.TeleportCoroutine());
		}

		private void Update()
		{
			if (this.MeltedLady.Target == null)
			{
				return;
			}
			this.DistanceToTarget = Vector2.Distance(this.MeltedLady.transform.position, this.MeltedLady.Target.transform.position);
			if (this.MeltedLady.SpriteRenderer.isVisible)
			{
				this.Floating();
			}
			if (this.MeltedLady.Status.Dead || this.MeltedLady.Status.IsHurt)
			{
				this.MeltedLady.StateMachine.SwitchState<MeltedLadyDeathState>();
			}
			else if (this.DistanceToTarget <= this.ActivationDistance)
			{
				this.MeltedLady.StateMachine.SwitchState<MeltedLadyAttackState>();
			}
			else
			{
				this.MeltedLady.StateMachine.SwitchState<MeltedLadyIdleState>();
			}
		}

		public override void Idle()
		{
			this.StopMovement();
		}

		public override void LookAtTarget(Vector3 targetPos)
		{
			this.MeltedLady.SetOrientation((targetPos.x <= this.MeltedLady.transform.position.x) ? EntityOrientation.Left : EntityOrientation.Right, true, false);
		}

		private void Floating()
		{
			base.transform.position += this.AmplitudeY * (Mathf.Sin(6.2831855f * this.SpeedY * Time.time) - Mathf.Sin(6.2831855f * this.SpeedY * (Time.time - Time.deltaTime))) * base.transform.up;
		}

		public void Teleport()
		{
			base.StartCoroutine(this.TeleportCoroutine());
		}

		private IEnumerator TeleportCoroutine()
		{
			this.CanTeleport = false;
			this.TeleportCooldownLapse = 0f;
			this.MeltedLady.AnimatorInyector.TeleportOut();
			this.MeltedLady.DamageArea.DamageAreaCollider.enabled = false;
			this.MeltedLady.DamageByContact = false;
			yield return this._teleportYield;
			this.TeleportToTarget();
			yield break;
		}

		public void TeleportToTarget()
		{
			this.MeltedLady.transform.position = this.GetAttackPosition();
			this.LookAtTarget(this.MeltedLady.Target.transform.position);
			this.MeltedLady.AnimatorInyector.TeleportIn();
		}

		private Vector2 GetAttackPosition()
		{
			Vector3 v = this.GetNearestTeleportPointToTarget().TeleportPosition;
			this.MeltedLady.Behaviour.IsInOrigin = false;
			float num = Vector2.Distance(this.MeltedLady.Behaviour.OriginPosition, v);
			if (num >= this.MeltedLady.Behaviour.MaxAttackDistance)
			{
				v = this.MeltedLady.Behaviour.OriginPosition;
				this.MeltedLady.Behaviour.IsInOrigin = true;
			}
			return v;
		}

		private MeltedLadyTeleportPoint GetNearestTeleportPointToTarget()
		{
			float num = float.PositiveInfinity;
			MeltedLadyTeleportPoint meltedLadyTeleportPoint = null;
			foreach (MeltedLadyTeleportPoint meltedLadyTeleportPoint2 in this.TeleportPoints)
			{
				if (!meltedLadyTeleportPoint2.Equals(this.CurrentTeleportPoint))
				{
					float num2 = Vector2.Distance(meltedLadyTeleportPoint2.transform.position, this.MeltedLady.Target.transform.position);
					if (num2 < num)
					{
						num = num2;
						meltedLadyTeleportPoint = meltedLadyTeleportPoint2;
					}
				}
			}
			this.CurrentTeleportPoint = meltedLadyTeleportPoint;
			return meltedLadyTeleportPoint;
		}

		public void Chase(Vector3 position)
		{
		}

		public override void Damage()
		{
		}

		public void Death()
		{
		}

		public void ResetCoolDown()
		{
		}

		public void ResetAttackCounter()
		{
			if (this.CurrentAttackAmount < this.AttackAmount)
			{
				this.CurrentAttackAmount = this.AttackAmount;
			}
		}

		public override void Attack()
		{
			this.MeltedLady.AnimatorInyector.Attack();
		}

		public override void StopMovement()
		{
		}

		public override void Wander()
		{
		}

		public override void Chase(Transform targetPosition)
		{
		}

		private void OnDestroy()
		{
			this.MeltedLady.OnDamaged -= this.OnDamaged;
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.magenta;
		}

		[FoldoutGroup("Activation Settings", true, 0)]
		[OnValueChanged("SetMaxAttackDistance", false)]
		public float ActivationDistance;

		[FoldoutGroup("Attack Settings", true, 0)]
		public float MaxAttackDistance;

		[FoldoutGroup("Attack Settings", true, 0)]
		public float AttackCoolDown = 2f;

		[FoldoutGroup("Attack Settings", true, 0)]
		public float TeleportInterval = 0.6f;

		private WaitForSeconds _teleportYield;

		[FoldoutGroup("Attack Settings", true, 0)]
		public float TeleportCooldown = 2f;

		[FoldoutGroup("Attack Settings", true, 0)]
		public float AttackHeight = 2f;

		[FoldoutGroup("Attacks amount", true, 0)]
		public int AttackAmount = 3;

		private int _currentAttackAmount;

		[FoldoutGroup("Attack Settings", true, 0)]
		public float AttackDistance = 2f;

		[FoldoutGroup("Floating Settings", true, 0)]
		public float AmplitudeY = 3f;

		[FoldoutGroup("Floating Settings", true, 0)]
		public float SpeedY = 1f;

		private float _index;

		private float _currentAttackLapse;
	}
}
