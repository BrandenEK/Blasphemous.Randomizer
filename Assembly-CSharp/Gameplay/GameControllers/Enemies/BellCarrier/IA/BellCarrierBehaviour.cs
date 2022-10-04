using System;
using System.Collections;
using Framework.FrameworkCore;
using Gameplay.GameControllers.Enemies.Framework.IA;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.BellCarrier.IA
{
	public class BellCarrierBehaviour : EnemyBehaviour
	{
		public bool stopWhileChasing { get; set; }

		public bool IsAwaken { get; set; }

		public bool Rising { get; set; }

		public bool IsAwaiting { get; set; }

		public bool IsChasingAfterSeen { get; set; }

		public bool WatchBack { get; set; }

		public bool WallHit { get; set; }

		private IEnumerator ReduceSpeed()
		{
			float currentSpeed = this.MaxSpeed;
			while (currentSpeed > 0f)
			{
				currentSpeed -= 0.3f;
				if (this.isBlocked)
				{
					currentSpeed = 0f;
				}
				this.SetMovementSpeed(currentSpeed);
				yield return null;
			}
			this.SetMovementSpeed(0f);
			this._bellCarrier.Inputs.HorizontalInput = 0f;
			if (Mathf.Abs(this._bellCarrier.Controller.PlatformCharacterPhysics.HSpeed) > 0f)
			{
				this._bellCarrier.Controller.PlatformCharacterPhysics.HSpeed = 0f;
			}
			yield break;
		}

		private IEnumerator Accelerate()
		{
			float t = 0f;
			float currentSpeed = 0f;
			while (t <= 1f)
			{
				t += Time.deltaTime * this.AccelerationFactor;
				currentSpeed = Mathf.Lerp(currentSpeed, this.MaxSpeed, t);
				this.SetMovementSpeed(currentSpeed);
				yield return null;
			}
			this.SetMovementSpeed(this.MaxSpeed);
			yield break;
		}

		private void BellCarrierOnEntityDie()
		{
			this.StopMovement();
			base.BehaviourTree.StopBehaviour();
			this._bellCarrier.AnimatorInyector.Death();
			this._bellCarrier.Audio.PlayDeath();
			this._bellCarrier.OnDeath -= this.BellCarrierOnEntityDie;
		}

		public override void OnAwake()
		{
			base.OnAwake();
			this._bellCarrier = base.GetComponent<BellCarrier>();
		}

		public bool TargetInLine { get; private set; }

		public bool IsTurning()
		{
			return base.TurningAround;
		}

		public override void OnStart()
		{
			base.OnStart();
			this.Entity = this._bellCarrier;
			this._bellCarrier.OnDeath += this.BellCarrierOnEntityDie;
			this._currentTimeChasing = this.MaxTimeChasing;
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			this.CheckCollision();
			if (this.IsAwaiting && this.IsAwaken)
			{
				this._currentIdleTime += Time.deltaTime;
				if (this._currentIdleTime >= this.IdleTimeUntilSleep)
				{
					this._currentIdleTime = 0f;
					this.IsAwaken = false;
					this._bellCarrier.AnimatorInyector.PlayBellHidden();
					this.StopMovement();
				}
			}
			if (base.IsHurt)
			{
				this._bellCarrier.AnimatorInyector.Chasing();
			}
			if (base.IsPlayerHeard())
			{
				this._currentHearingTime += Time.deltaTime;
				if (this._currentHearingTime >= this.MaxHearingTimeUntilDetection && !this.WatchBack)
				{
					this.WatchBack = true;
				}
			}
			else if (!this._bellCarrier.MotionChecker.HitsFloor)
			{
				if (!this.WatchBack)
				{
					this.WatchBack = true;
				}
			}
			else
			{
				if (this.WatchBack)
				{
					this.WatchBack = false;
				}
				this._currentHearingTime = 0f;
			}
			if (base.PlayerHeard || base.PlayerSeen)
			{
				this._currentIdleTime = 0f;
				this.ResetTimeChasing();
			}
		}

		private void CheckCollision()
		{
			int num = (this._bellCarrier.Status.Orientation != EntityOrientation.Right) ? -1 : 1;
			RaycastHit2D[] array = Physics2D.RaycastAll(base.transform.position, Vector2.right * (float)num, 15f, this.RaycastTarget);
			this.TargetInLine = false;
			this.isBlocked = false;
			RaycastHit2D[] array2 = array;
			int num2 = 0;
			if (num2 < array2.Length)
			{
				RaycastHit2D raycastHit2D = array2[num2];
				this.TargetInLine = raycastHit2D.collider.CompareTag("Penitent");
			}
			foreach (RaycastHit2D raycastHit2D2 in array)
			{
				if ((this.BlockLayerMask.value & 1 << raycastHit2D2.collider.gameObject.layer) > 0)
				{
					this.isBlocked = (raycastHit2D2.distance < 1.5f);
					if (this.isBlocked)
					{
						break;
					}
				}
			}
		}

		private void OnDrawGizmos()
		{
			if (this._bellCarrier == null)
			{
				return;
			}
			Gizmos.color = Color.red;
			int num = (this._bellCarrier.Status.Orientation != EntityOrientation.Right) ? -1 : 1;
			Gizmos.DrawLine(base.transform.position, base.transform.position + Vector3.right * (float)num * 15f);
		}

		public override void Idle()
		{
			if (this._stopped)
			{
				this._stopped = !this._stopped;
			}
			this.StopMovement();
			if (!this.VisualSensor.SensorCollider2D.enabled)
			{
				this.VisualSensor.SensorCollider2D.enabled = true;
			}
			if (!this.IsAwaiting)
			{
				this.IsAwaiting = true;
			}
			this._bellCarrier.AnimatorInyector.Stop();
		}

		public void CheckBlocked()
		{
		}

		public override void Chase(Transform targetPosition)
		{
			if (this._bellCarrier == null)
			{
				return;
			}
			if (!this._bellCarrier.IsChasing)
			{
				this._bellCarrier.IsChasing = true;
			}
			if (this._stopped)
			{
				this._stopped = !this._stopped;
			}
			if (base.TurningAround)
			{
				base.TurningAround = !base.TurningAround;
			}
			this._bellCarrier.AnimatorInyector.Chasing();
			if (this.IsAwaiting)
			{
				this.IsAwaiting = !this.IsAwaiting;
			}
			if (this._bellCarrier.BellCarrierBehaviour.Rising)
			{
				this._bellCarrier.BellCarrierBehaviour.Rising = false;
			}
		}

		public void StopChase()
		{
			if (this._bellCarrier.IsChasing)
			{
				this._bellCarrier.IsChasing = false;
				this._currentIdleTime = 0f;
			}
			this.StopMovement();
			this._bellCarrier.AnimatorInyector.Stop();
		}

		public void StopWhileChasing()
		{
			if (this._bellCarrier.Animator.GetCurrentAnimatorStateInfo(0).IsName("TurnAround"))
			{
				return;
			}
			if (this.stopWhileChasing)
			{
				return;
			}
			this.StopMovement();
			this.stopWhileChasing = true;
			this._bellCarrier.AnimatorInyector.PlayStopAnimation();
		}

		public bool ChasingAfterSeen()
		{
			this._currentTimeChasing -= Time.deltaTime;
			if (this._currentTimeChasing <= 0f)
			{
				this.IsChasingAfterSeen = false;
			}
			else
			{
				this.IsChasingAfterSeen = true;
			}
			return this.IsChasingAfterSeen;
		}

		public void ResetTimeChasing()
		{
			this._currentTimeChasing = this.MaxTimeChasing;
			this.IsChasingAfterSeen = false;
		}

		public void Block()
		{
			if (this._bellCarrier.AnimatorInyector.IsInWallCrashAnim || !this._startRun)
			{
				return;
			}
			this._bellCarrier.AnimatorInyector.PlayWallCrushAnimation();
			this._stopped = true;
			this.ResetTimeChasing();
			this.StopMovement();
		}

		public void LookAtTarget(Transform targetPosition)
		{
			if (this._bellCarrier == null)
			{
				return;
			}
			if (this._bellCarrier.Animator.GetCurrentAnimatorStateInfo(0).IsName("StopRun"))
			{
				return;
			}
			if (!this.isBlocked)
			{
				this._bellCarrier.AnimatorInyector.Chasing();
				if (!this._bellCarrier.IsChasing)
				{
					this._bellCarrier.IsChasing = true;
				}
			}
			if (base.TurningAround)
			{
				return;
			}
			EntityOrientation orientation = this.Entity.Status.Orientation;
			if (orientation == EntityOrientation.Left)
			{
				if (this.Entity.transform.position.x <= targetPosition.position.x)
				{
					this.TurnAround();
				}
			}
			else if (this.Entity.transform.position.x > targetPosition.position.x)
			{
				this.TurnAround();
			}
		}

		private void TurnAround()
		{
			this.StopMovement();
			if (!base.TurningAround)
			{
				base.TurningAround = true;
			}
			this._bellCarrier.AnimatorInyector.TurnAround();
		}

		public void Awaken()
		{
			if (this.IsAwaken)
			{
				return;
			}
			this.IsAwaken = true;
			this.Rising = true;
			this._bellCarrier.AnimatorInyector.Awaken();
			this.VisualSensor.SensorCollider2D.enabled = false;
		}

		public override void Damage()
		{
			if (!base.IsHurt)
			{
				base.IsHurt = true;
			}
		}

		public void EnableSensorColliders()
		{
			if (!this.VisualSensor.SensorCollider2D.enabled)
			{
				this.VisualSensor.SensorCollider2D.enabled = true;
			}
			if (!this.HearingSensor.SensorCollider2D.enabled)
			{
				this.VisualSensor.SensorCollider2D.enabled = true;
			}
		}

		public override void StopMovement()
		{
			if (this._bellCarrier == null)
			{
				return;
			}
			if (!this._startRun)
			{
				return;
			}
			this._startRun = false;
			base.StartCoroutine(this.ReduceSpeed());
		}

		public void StartMovement()
		{
			if (this._startRun)
			{
				return;
			}
			this._startRun = true;
			this._currentIdleTime = 0f;
			base.StartCoroutine(this.Accelerate());
			if (base.IsHurt)
			{
				base.IsHurt = !base.IsHurt;
			}
			float horizontalInput = (this._bellCarrier.Status.Orientation != EntityOrientation.Right) ? -1f : 1f;
			this._bellCarrier.Inputs.HorizontalInput = horizontalInput;
		}

		private void SetMovementSpeed(float newSpeed)
		{
			if (this._bellCarrier == null)
			{
				return;
			}
			this._bellCarrier.Controller.MaxWalkingSpeed = newSpeed;
		}

		public override void Attack()
		{
		}

		public override void Wander()
		{
		}

		private BellCarrier _bellCarrier;

		private float _currentHearingTime;

		private float _currentIdleTime;

		private float _currentTimeChasing;

		private bool _lookAtTarget;

		private bool _startRun;

		private bool _stopped;

		public float AccelerationFactor = 2.5f;

		public LayerMask BlockLayers;

		public float IdleTimeUntilSleep = 10f;

		public float MaxHearingTimeUntilDetection = 2f;

		public float MaxSpeed = 3.5f;

		public float MaxTimeChasing = 4f;

		public LayerMask RaycastTarget;
	}
}
