using System;
using DG.Tweening;
using Framework.FrameworkCore;
using Gameplay.GameControllers.Enemies.DrownedCorpse.AI.DrownedCorpseStates;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.StateMachine;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.DrownedCorpse.AI
{
	public class DrownedCorpseBehaviour : EnemyBehaviour
	{
		private bool Vanished { get; set; }

		public DrownedCorpse DrownedCorpse { get; set; }

		private StateMachine StateMachine { get; set; }

		private VisionCone visionCone { get; set; }

		public void Awaken()
		{
		}

		public void SleepForever()
		{
		}

		public override void OnAwake()
		{
			base.OnAwake();
			this.StateMachine = this.Entity.GetComponent<StateMachine>();
			this.visionCone = this.Entity.GetComponentInChildren<VisionCone>();
			this._bottomHits = new RaycastHit2D[2];
		}

		public override void OnStart()
		{
			base.OnStart();
			this.DrownedCorpse = (DrownedCorpse)this.Entity;
			this.ResetSleepTime();
			this.SwitchToState(DrownedCorpseBehaviour.CorpseState.Sleep);
			if (UnityEngine.Random.value > 0.5f)
			{
				this.LookAtTarget(base.transform.position - Vector3.right);
			}
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			DrownedCorpseBehaviour.CorpseState currentCorpseState = this.CurrentCorpseState;
			if (currentCorpseState != DrownedCorpseBehaviour.CorpseState.Sleep)
			{
				if (currentCorpseState == DrownedCorpseBehaviour.CorpseState.Chase)
				{
					this.Chase(this.DrownedCorpse.Target.transform);
				}
			}
			else
			{
				this.Sleep();
			}
		}

		private void SwitchToState(DrownedCorpseBehaviour.CorpseState targetState)
		{
			this.CurrentCorpseState = targetState;
			DrownedCorpseBehaviour.CorpseState currentCorpseState = this.CurrentCorpseState;
			if (currentCorpseState != DrownedCorpseBehaviour.CorpseState.Sleep)
			{
				if (currentCorpseState == DrownedCorpseBehaviour.CorpseState.Chase)
				{
					this.StateMachine.SwitchState<DrownedCorpseChaseState>();
				}
			}
			else
			{
				this.StateMachine.SwitchState<DrownedCorpseSleepState>();
			}
		}

		public bool CanWalk()
		{
			return !this._isSpawning;
		}

		public void Sleep()
		{
			this.currentChasingTimeAfterLostTarget = UnityEngine.Random.Range(this.MinChasingTime, this.MaxChasingTime);
			this.sleepTime -= Time.deltaTime;
			if (this.TargetCanBeVisible() && this.sleepTime < 0f && this.CanSeeTarget())
			{
				this.SwitchToState(DrownedCorpseBehaviour.CorpseState.Chase);
			}
		}

		public override void Chase(Transform targetPosition)
		{
			if (!this.DrownedCorpse.MotionChecker.HitsFloor || this.DrownedCorpse.MotionChecker.HitsBlock)
			{
				this.StopMovement();
				this.VanishAfterRun();
			}
			if (this.TargetIsLost(targetPosition.position))
			{
				this.currentChasingTimeAfterLostTarget -= Time.deltaTime;
				if (this.currentChasingTimeAfterLostTarget > 0f)
				{
					return;
				}
				this.VanishAfterRun();
			}
		}

		private void VanishAfterRun()
		{
			this.ResetSleepTime();
			this.DrownedCorpse.AnimatorInyector.VanishAfterRun();
			this.SwitchToState(DrownedCorpseBehaviour.CorpseState.Sleep);
		}

		private bool TargetIsLost(Vector3 position)
		{
			bool result = false;
			if (this.CurrentCorpseState != DrownedCorpseBehaviour.CorpseState.Chase)
			{
				return false;
			}
			if (this.DrownedCorpse.Controller.PlatformCharacterPhysics.HSpeed > 0.1f)
			{
				result = (position.x < this.DrownedCorpse.transform.position.x);
			}
			else if (this.DrownedCorpse.Controller.PlatformCharacterPhysics.HSpeed < 0f)
			{
				result = (position.x > this.DrownedCorpse.transform.position.x);
			}
			return result;
		}

		public bool TargetCanBeVisible()
		{
			base.GetTarget();
			if (!this.DrownedCorpse.Target)
			{
				return false;
			}
			Vector2 vector = this.DrownedCorpse.Target.transform.position - this.DrownedCorpse.transform.position;
			float num = Mathf.Abs(vector.y);
			float num2 = Mathf.Abs(vector.x);
			return num <= this.MaxVisibleHeight && num2 < this.ActivationDistance;
		}

		public bool CanSeeTarget()
		{
			return !(this.visionCone == null) && this.visionCone.CanSeeTarget(this.DrownedCorpse.Target.transform, "Penitent", false);
		}

		public override void Idle()
		{
		}

		public override void Wander()
		{
		}

		public override void Damage()
		{
			this.VanishByHit();
			this.DrownedCorpse.AnimatorInyector.VanishAfterDamage();
		}

		public void OnGuarded()
		{
		}

		private void VanishByHit()
		{
			this.ResetSleepTime();
			this.SwitchToState(DrownedCorpseBehaviour.CorpseState.Sleep);
		}

		public void Death()
		{
			this.StopMovement();
		}

		public override void StopMovement()
		{
			this.DrownedCorpse.Input.HorizontalInput = 0f;
			this.DrownedCorpse.Controller.PlatformCharacterPhysics.HSpeed = 0f;
		}

		public void StopMovement(float elapse)
		{
			this.DrownedCorpse.Input.HorizontalInput = 0f;
			DOTween.To(delegate(float x)
			{
				this.DrownedCorpse.Controller.PlatformCharacterPhysics.HSpeed = x;
			}, this.DrownedCorpse.Controller.PlatformCharacterPhysics.HSpeed, 0f, elapse);
		}

		public override void LookAtTarget(Vector3 targetPos)
		{
			if (targetPos.x > this.DrownedCorpse.transform.position.x)
			{
				if (this.DrownedCorpse.Status.Orientation != EntityOrientation.Right)
				{
					this.DrownedCorpse.SetOrientation(EntityOrientation.Right, true, false);
				}
			}
			else if (this.DrownedCorpse.Status.Orientation != EntityOrientation.Left)
			{
				this.DrownedCorpse.SetOrientation(EntityOrientation.Left, true, false);
			}
			this.SetColliderScale();
		}

		private void ResetSleepTime()
		{
			this.sleepTime = UnityEngine.Random.Range(this.minSleepTime, this.maxSleepTime);
		}

		public override void Attack()
		{
		}

		private void SetColliderScale()
		{
			this.DrownedCorpse.EntityDamageArea.DamageAreaCollider.transform.localScale = new Vector3((float)((this.DrownedCorpse.Status.Orientation != EntityOrientation.Right) ? -1 : 1), 1f, 1f);
		}

		[SerializeField]
		[FoldoutGroup("Motion Settings", true, 0)]
		private RaycastHit2D[] _bottomHits;

		private readonly bool _isSpawning;

		[FoldoutGroup("Activation Settings", true, 0)]
		public float ActivationDistance;

		private float currentChasingTimeAfterLostTarget;

		public DrownedCorpseBehaviour.CorpseState CurrentCorpseState;

		public float DistanceToTarget;

		[FoldoutGroup("Motion Settings", true, 0)]
		public LayerMask GroundLayerMask;

		[SerializeField]
		[FoldoutGroup("Activation Settings", true, 0)]
		private float MaxChasingTime = 0.5f;

		[SerializeField]
		[FoldoutGroup("Activation Settings", true, 0)]
		private float MinChasingTime = 0.2f;

		[SerializeField]
		[FoldoutGroup("Sleep Settings", true, 0)]
		private float maxSleepTime = 16f;

		[SerializeField]
		[FoldoutGroup("Sleep Settings", true, 0)]
		private float minSleepTime = 8f;

		[FoldoutGroup("Activation Settings", true, 0)]
		public float MaxTimeAwaitingBeforeChase;

		public float MaxVisibleHeight = 2f;

		private float sleepTime;

		[FoldoutGroup("Activation Settings", true, 0)]
		public bool startAwaken = true;

		public enum CorpseState
		{
			Sleep,
			Chase
		}
	}
}
