using System;
using System.Collections;
using System.Collections.Generic;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Enemies.CowardTrapper.Attack;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Penitent;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.CowardTrapper.AI
{
	public class CowardTrapperBehaviour : EnemyBehaviour
	{
		public CowardTrapper CowardTrapper { get; private set; }

		public bool IsRunAway { get; set; }

		public bool ReverseDirection { get; set; }

		public override void OnAwake()
		{
			base.OnAwake();
			this.CowardTrapper = (CowardTrapper)this.Entity;
			this.CowardTrapper.OnDeath += this.OnDeath;
			SpawnManager.OnPlayerSpawn += this.OnPlayerSpawn;
		}

		private void OnDeath()
		{
			this.CowardTrapper.OnDeath -= this.OnDeath;
		}

		public override void OnStart()
		{
			base.OnStart();
			this._currentTimeRunning = this.TimeRunning;
			this._currentTimeAwaiting = this.TimeAwaiting;
			this._currentTrapInterval = this.SpawningTrapInterval;
			this._runningCoroutine = this.RunAwayCoroutine();
		}

		private void OnPlayerSpawn(Penitent penitent)
		{
			this.isActive = true;
			SpawnManager.OnPlayerSpawn -= this.OnPlayerSpawn;
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			if (!this.isActive || this.CowardTrapper.Status.Dead)
			{
				return;
			}
			this._currentTimeAwaiting -= Time.deltaTime;
			if (this.CowardTrapper.DistanceToTarget <= this.ActivationDistance && !this.IsRunAway && !this.IsAwaiting && this.CanSeeTarget)
			{
				this.CowardTrapper.AnimatorInjector.Scared();
			}
			if (this.IsBlocked && this.IsRunAway)
			{
				this.StopMovement();
			}
			this.SpawnTraps();
		}

		private IEnumerator RunAwayCoroutine()
		{
			this.IsRunAway = true;
			this._runningDir = this.GetRunningDirection;
			while (this._currentTimeRunning > 0f)
			{
				this._currentTimeRunning -= Time.deltaTime;
				this.RunAway(this._runningDir);
				if (this.IsBlocked || this.Entity.Status.Dead)
				{
					if (this.IsBlocked)
					{
						this.ReverseOrientation();
					}
					this.StopMovement();
					yield break;
				}
				yield return null;
			}
			this.StopMovement();
			yield break;
		}

		public void StartRun()
		{
			base.StartCoroutine(this.RunAwayCoroutine());
		}

		public void RunAway(Vector2 dir)
		{
			this.CowardTrapper.Input.HorizontalInput = dir.x;
			this.Entity.Status.Orientation = ((dir.x <= 0f) ? EntityOrientation.Left : EntityOrientation.Right);
			this.Entity.SpriteRenderer.flipX = (this.Entity.Status.Orientation == EntityOrientation.Left);
		}

		private Vector2 GetRunningDirection
		{
			get
			{
				Vector2 vector = (this.CowardTrapper.Target.transform.position.x <= this.CowardTrapper.transform.position.x) ? Vector2.right : Vector2.left;
				if (!this.ReverseDirection)
				{
					return vector;
				}
				this.ReverseDirection = false;
				return vector * -1f;
			}
		}

		public new bool IsBlocked
		{
			get
			{
				return this.CowardTrapper.MotionChecker.HitsBlock || !this.CowardTrapper.MotionChecker.HitsFloor || this.CowardTrapper.MotionChecker.HitsPatrolBlock;
			}
		}

		public bool IsAwaiting
		{
			get
			{
				return this._currentTimeAwaiting > 0f;
			}
		}

		public bool CanSeeTarget
		{
			get
			{
				return this.CowardTrapper.VisionCone.CanSeeTarget(this.CowardTrapper.Target.transform, "Penitent", false);
			}
		}

		public override void StopMovement()
		{
			this.CowardTrapper.Input.HorizontalInput = 0f;
			this.CowardTrapper.AnimatorInjector.StopRun();
			this.CowardTrapper.Controller.PlatformCharacterPhysics.HSpeed = 0f;
			this.IsRunAway = false;
			this._currentTimeRunning = this.TimeRunning;
			this._currentTimeAwaiting = this.TimeAwaiting;
			this._currentTrapInterval = this.SpawningTrapInterval;
			if (!this.IsBlocked)
			{
				return;
			}
			this.ReverseDirection = true;
			this.LookAtTarget(this.CowardTrapper.Target.transform.position);
		}

		private void SpawnTraps()
		{
			if (!this.IsRunAway || !this.CanSpawnTrap || this.IsOnOneWayDownPlatform)
			{
				return;
			}
			this._currentTrapInterval -= Time.deltaTime;
			if (this._currentTrapInterval >= 0f)
			{
				return;
			}
			this._currentTrapInterval = this.SpawningTrapInterval;
			CowardTrap component = this.CowardTrapper.Attack.SummonAreaOnPoint(base.transform.position, 0f, 1f, null).GetComponent<CowardTrap>();
			component.SetOwner(this.CowardTrapper);
			this.AddTrap(component);
		}

		private bool IsOnOneWayDownPlatform
		{
			get
			{
				return Physics2D.Raycast(base.transform.position, -base.transform.up, 2f, this.OneWayDownLayer);
			}
		}

		public void AddTrap(CowardTrap trap)
		{
			if (!this._cowardTraps.Contains(trap))
			{
				this._cowardTraps.Add(trap);
			}
		}

		public void RemoveTrap(CowardTrap trap)
		{
			if (this._cowardTraps.Contains(trap))
			{
				this._cowardTraps.Remove(trap);
			}
		}

		private bool CanSpawnTrap
		{
			get
			{
				return this._cowardTraps.Count < this.MaxTrapsAmount;
			}
		}

		private new void ReverseOrientation()
		{
			EntityOrientation orientation = this.Entity.Status.Orientation;
			EntityOrientation orientation2 = (orientation != EntityOrientation.Left) ? EntityOrientation.Left : EntityOrientation.Right;
			this.Entity.SetOrientation(orientation2, true, false);
		}

		public override void Idle()
		{
			throw new NotImplementedException();
		}

		public override void Wander()
		{
			throw new NotImplementedException();
		}

		public override void Chase(Transform targetPosition)
		{
		}

		public override void Attack()
		{
			throw new NotImplementedException();
		}

		public override void Damage()
		{
			throw new NotImplementedException();
		}

		public LayerMask OneWayDownLayer;

		public float ActivationDistance = 6f;

		public float SpawningTrapInterval = 1f;

		public int MaxTrapsAmount = 3;

		private List<CowardTrap> _cowardTraps = new List<CowardTrap>();

		private float _currentTrapInterval;

		private bool isActive;

		public float TimeRunning = 4f;

		public float TimeAwaiting = 1f;

		private float _currentTimeRunning;

		private float _currentTimeAwaiting;

		private Vector2 _runningDir;

		private IEnumerator _runningCoroutine;
	}
}
