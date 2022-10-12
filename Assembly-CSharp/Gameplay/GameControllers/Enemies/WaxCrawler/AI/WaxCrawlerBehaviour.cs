using System;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Enemies.Framework.Damage;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Enemies.WaxCrawler.Animator;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.WaxCrawler.AI
{
	public class WaxCrawlerBehaviour : EnemyBehaviour
	{
		public Transform Target { get; set; }

		public Vector3 Origin { get; set; }

		public bool Awake { get; set; }

		public bool Above { get; set; }

		public bool Below { get; set; }

		public bool Melting { get; set; }

		public bool CanMove { get; set; }

		public bool TargetLost { get; set; }

		public override void OnStart()
		{
			base.OnStart();
			this._waxCrawler = (WaxCrawler)this.Entity;
			this.AnimatorInyector = this._waxCrawler.AnimatorInyector;
			this.AnimatorInyector.AnimatorSpeed(0f);
			this._waxCrawler.SpriteRenderer.enabled = false;
			this._bottomHits = new RaycastHit2D[2];
			this._forwardsHits = new RaycastHit2D[2];
			this._damageArea = this._waxCrawler.DamageArea;
			this._waxCrawler.OnDeath += this.WaxCrawlerOnEntityDie;
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			if (Core.Logic.CurrentState == LogicStates.PlayerDead)
			{
				this._waxCrawler.Attack.EnableAttackArea = false;
			}
			if (this.Entity.Status.Orientation == EntityOrientation.Left)
			{
				Vector2 vector = new Vector2(this._damageArea.DamageAreaCollider.bounds.min.x, this._damageArea.DamageAreaCollider.bounds.center.y);
				Vector2 vector2 = vector;
				Debug.DrawLine(vector2, vector2 - Vector2.up * this.CurrentGroundDetection, Color.yellow);
				base.SensorHitsFloor = (Physics2D.LinecastNonAlloc(vector2, vector2 - Vector2.up * this.CurrentGroundDetection, this._bottomHits, this.BlockLayerMask) > 0);
				Debug.DrawLine(vector2, vector2 - base.transform.right * this.RangeBlockDectection, Color.yellow);
				this.isBlocked = (Physics2D.LinecastNonAlloc(vector2, vector2 - base.transform.right * this.RangeBlockDectection, this._forwardsHits, this.BlockLayerMask) > 0);
			}
			else
			{
				Vector2 vector3 = new Vector2(this._damageArea.DamageAreaCollider.bounds.max.x, this._damageArea.DamageAreaCollider.bounds.center.y);
				Vector2 vector2 = vector3;
				Debug.DrawLine(vector2, vector2 - Vector2.up * this.CurrentGroundDetection, Color.yellow);
				base.SensorHitsFloor = (Physics2D.LinecastNonAlloc(vector2, vector2 - Vector2.up * this.CurrentGroundDetection, this._bottomHits, this.BlockLayerMask) > 0);
				Debug.DrawLine(vector2, vector2 + base.transform.right * this.RangeBlockDectection, Color.yellow);
				this.isBlocked = (Physics2D.LinecastNonAlloc(vector2, vector2 + base.transform.right * this.RangeBlockDectection, this._forwardsHits, this.BlockLayerMask) > 0);
			}
			this.CanMove = (base.SensorHitsFloor && !this.isBlocked);
			if (base.PlayerSeen)
			{
				this.TargetLost = false;
				this._currentTimeChasing = 0f;
			}
			else
			{
				this._currentTimeChasing += Time.deltaTime;
				this.TargetLost = (this._currentTimeChasing >= this.TimeChasing);
			}
		}

		public void Rise()
		{
			if (!this.Awake)
			{
				this.Awake = true;
			}
			if (!this.Melting)
			{
				this.Melting = true;
			}
			this.AnimatorInyector.EnableSpriteRenderer = true;
			this.AnimatorInyector.AnimatorSpeed(1f);
			this.AnimatorInyector.Appear();
		}

		public void Asleep()
		{
			if (this.Awake)
			{
				this.Awake = !this.Awake;
			}
			this._waxCrawler.AnimatorInyector.EnableSpriteRenderer = false;
			this._waxCrawler.Behaviour.Melting = false;
			this._waxCrawler.Behaviour.MoveToOrigin();
		}

		public void Fall()
		{
			if (this.Awake)
			{
				this.Awake = !this.Awake;
			}
		}

		public override void Idle()
		{
			this.AnimatorInyector.AnimatorSpeed(0f);
		}

		public override void Wander()
		{
		}

		public override void Chase(Transform targetPosition)
		{
			if (targetPosition == null)
			{
				return;
			}
			if (!base.IsHurt)
			{
				EntityOrientation orientation = this._waxCrawler.Status.Orientation;
				Vector2 a = (orientation != EntityOrientation.Left) ? Vector2.right : (-Vector2.right);
				base.transform.Translate(a * this.Speed * Time.deltaTime, Space.World);
			}
		}

		public void MoveToOrigin()
		{
			base.transform.position = this.Origin;
		}

		public void Hide()
		{
			this.AnimatorInyector.Hide();
		}

		public override void Attack()
		{
			throw new NotImplementedException();
		}

		public override void Damage()
		{
			throw new NotImplementedException();
		}

		public override void StopMovement()
		{
			throw new NotImplementedException();
		}

		private void WaxCrawlerOnEntityDie()
		{
			if (base.BehaviourTree.isRunning)
			{
				base.BehaviourTree.StopBehaviour();
			}
		}

		public override void LookAtTarget(Vector3 targetPos)
		{
			base.LookAtTarget(targetPos);
		}

		private RaycastHit2D[] _bottomHits;

		private RaycastHit2D[] _forwardsHits;

		private float _currentTimeChasing;

		private EnemyDamageArea _damageArea;

		public WaxCrawler _waxCrawler;

		public WaxCrawlerAnimatorInyector AnimatorInyector;

		public float CurrentGroundDetection = 1f;

		public float RangeBlockDectection = 1f;

		public float Speed = 3f;

		public float TimeChasing = 2f;
	}
}
