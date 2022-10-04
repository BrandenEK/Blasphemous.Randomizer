using System;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Enemies.Framework.Physics;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Penitent;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Ghost
{
	public class Ghost : Enemy
	{
		protected float GetRemainTimeToNextFlight()
		{
			return UnityEngine.Random.Range(1f, this.maxTimeToNextFlight);
		}

		public override EnemyAttack EnemyAttack()
		{
			throw new NotImplementedException();
		}

		public override EnemyBumper EnemyBumper()
		{
			throw new NotImplementedException();
		}

		public override EnemyFloorChecker EnemyFloorChecker()
		{
			throw new NotImplementedException();
		}

		protected override void EnablePhysics(bool enable)
		{
			throw new NotImplementedException();
		}

		public GhostPath GhostPath
		{
			get
			{
				return this.ghostPath;
			}
		}

		public int CurrentWayPointId { get; set; }

		protected override void OnAwake()
		{
			base.OnAwake();
		}

		protected override void OnStart()
		{
			base.OnStart();
			this._penitent = Core.Logic.Penitent;
			this.ghostSpriteRenderer = base.GetComponentInChildren<SpriteRenderer>();
			this.ghostFlight = base.GetComponentInChildren<GhostFlight>();
			this.entityCurrentState = EntityStates.Idle;
			GhostFlight ghostFlight = this.ghostFlight;
			ghostFlight.OnStopFloating = (Core.SimpleEvent)Delegate.Combine(ghostFlight.OnStopFloating, new Core.SimpleEvent(this.ghost_OnStopFloating));
			GhostFlight ghostFlight2 = this.ghostFlight;
			ghostFlight2.OnLanding = (Core.SimpleEvent)Delegate.Combine(ghostFlight2.OnLanding, new Core.SimpleEvent(this.ghost_OnLanding));
			this.remainTimeToNextFlight = this.GetRemainTimeToNextFlight();
			if (this.ghostPath == null)
			{
				Debug.LogWarning("El fantasma necesita un GhostPath para patrullar");
			}
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			switch (this.entityCurrentState)
			{
			case EntityStates.Wander:
				this.ghostFlight.Landing(false);
				break;
			case EntityStates.Attack:
				this.ghostFlight.Landing(true);
				break;
			case EntityStates.Idle:
				this.ghostFlight.Floating();
				break;
			}
			if (this.ghostSpriteRenderer.transform.position.x >= this._penitent.transform.position.x)
			{
				this.SetOrientation(EntityOrientation.Left, true, false);
			}
			else if (this.ghostSpriteRenderer.transform.position.x < this._penitent.transform.position.x)
			{
				this.SetOrientation(EntityOrientation.Right, true, false);
			}
			this.remainTimeToNextFlight -= Time.deltaTime;
			if (this.remainTimeToNextFlight <= 0f)
			{
				this.remainTimeToNextFlight = this.GetRemainTimeToNextFlight();
				this.ghostFlight.EnableFloating(false);
			}
		}

		private void ghost_OnStopFloating()
		{
			if (this.entityCurrentState == EntityStates.Idle)
			{
				this.ghostFlight.SetTargetPosition(base.transform.position, this.ghostFlight.GetRandomWaypointPosition());
				this.entityCurrentState = EntityStates.Attack;
			}
		}

		private void ghost_OnLanding()
		{
			this.ghostFlight.EnableFloating(true);
			if (this.entityCurrentState != EntityStates.Idle)
			{
				this.entityCurrentState = EntityStates.Idle;
			}
		}

		private Penitent _penitent;

		private SpriteRenderer ghostSpriteRenderer;

		private GhostFlight ghostFlight;

		[SerializeField]
		private GhostPath ghostPath;

		public float maxTimeToNextFlight = 15f;

		protected float remainTimeToNextFlight;
	}
}
