using System;
using Framework.FrameworkCore;
using Gameplay.GameControllers.Effects.Player.GhostTrail;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Penitent.Gizmos;
using Maikel.StatelessFSM;
using Plugins.Maikel.StateMachine;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.Quirce.AI
{
	public class QuirceSwordBehaviour : Entity
	{
		protected override void OnStart()
		{
			base.OnStart();
			this.ghostTrail = base.GetComponentInChildren<GhostTrailGenerator>();
			this._results = new RaycastHit2D[5];
			this.stIdle = new QuirceSwordSt_Idle();
			this.stSpinning = new QuirceSwordSt_Spinning();
			this.stSpinToPoint = new QuirceSwordSt_SpinToPoint();
			this._fsm = new StateMachine<QuirceSwordBehaviour>(this, this.stIdle, null, null);
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			this._fsm.DoUpdate();
		}

		public void SetAutoFollow(bool follow)
		{
			this.doFollow = follow;
		}

		public void GoToPoint(Vector2 point)
		{
			this._targetPosition = point;
			this._fsm.ChangeState(this.stSpinToPoint);
		}

		public void SetVisible(bool visible)
		{
			base.Animator.SetBool("VISIBLE", visible);
		}

		public void SetReversed(bool reversed)
		{
			this._inverted = reversed;
		}

		public void SetSpinning(bool spin)
		{
			this._fsm.ChangeState((!spin) ? this.stIdle : this.stSpinning);
			this._instantiationTimer = this.secondsBetweenInstances;
			base.Animator.SetBool("SPIN", spin);
		}

		public void SetGhostTrail(bool active)
		{
			this.ghostTrail.EnableGhostTrail = active;
		}

		public bool IsCloseToPoint()
		{
			return Vector2.Distance(base.transform.position, this._targetPosition) < 0.2f;
		}

		public void ReturnToSpinFollow()
		{
			this._fsm.ChangeState(this.stSpinning);
		}

		public void SetSpeedFactor(float spdFactor)
		{
			this._oldSmoothTranslationFactor = this.smoothTranslationFactor;
			this.smoothTranslationFactor = spdFactor;
		}

		public void SetNormalSpeed()
		{
			this.smoothTranslationFactor = this._oldSmoothTranslationFactor;
		}

		public void ResetRotation()
		{
			base.transform.rotation = Quaternion.identity;
		}

		public void CheckCollision()
		{
			if (this._instantiationTimer > 0f)
			{
				this._instantiationTimer -= Time.deltaTime;
			}
			else
			{
				this._instantiationTimer = this.secondsBetweenInstances;
				Vector2 vector = base.transform.position + this.collisionPoint;
				if (Physics2D.Raycast(vector, Vector2.down, this.filter, this._results, this.collisionRadius) > 0)
				{
					Vector2 point = this._results[0].point;
					Object.Instantiate<GameObject>(this.sparksPrefab, point, Quaternion.identity);
				}
			}
		}

		public void UpdateFloatingOffset()
		{
			float num = Mathf.Sin(this.floatingFrequency * Time.time) * this.floatingAmplitude;
			this._floatingOffset = new Vector2(0f, num);
		}

		public void SetTargetPosition()
		{
			if (this.rootMotion == null)
			{
				return;
			}
			if (this.ownerEntity.Status.Orientation == EntityOrientation.Right)
			{
				this._targetPosition = this.rootMotion.transform.position;
			}
			else
			{
				this._targetPosition = this.rootMotion.ReversePosition;
			}
		}

		public void SetTargetRotation()
		{
			if (this.rootMotion == null)
			{
				return;
			}
			if (this.ownerEntity.Status.Orientation == EntityOrientation.Right)
			{
				this._targetAngle = this.rootMotion.transform.localEulerAngles.z;
				this._targetAngle += (float)(90 * ((!this._inverted) ? 0 : 1));
			}
			else
			{
				this._targetAngle = -this.rootMotion.transform.localEulerAngles.z;
				this._targetAngle += (float)(90 * ((!this._inverted) ? -1 : -2));
			}
		}

		public void ApplyPosition()
		{
			this._followPosition = this._targetPosition + this._floatingOffset;
			if (this._inverted)
			{
				this._followPosition += Vector2.up * this.invertedYOffset;
			}
			base.transform.position = Vector3.Lerp(base.transform.position, this._followPosition, this.smoothTranslationFactor);
		}

		public void ApplyRotation()
		{
			Quaternion quaternion = Quaternion.Euler(0f, 0f, this._targetAngle);
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, quaternion, this.smoothRotationFactor);
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawLine(base.transform.position + this.collisionPoint, base.transform.position + this.collisionPoint + Vector3.down * this.collisionRadius);
		}

		[FoldoutGroup("References", 0)]
		public RootMotionDriver rootMotion;

		[FoldoutGroup("References", 0)]
		public Entity ownerEntity;

		[FoldoutGroup("Floating settings", 0)]
		public float floatingFrequency;

		[FoldoutGroup("Floating settings", 0)]
		public float floatingAmplitude;

		[FoldoutGroup("Follow settings", 0)]
		public float invertedYOffset;

		[FoldoutGroup("Follow settings", 0)]
		public float smoothTranslationFactor;

		[FoldoutGroup("Follow settings", 0)]
		public float smoothRotationFactor;

		[FoldoutGroup("Follow settings", 0)]
		public bool doFollow = true;

		[FoldoutGroup("VFX", 0)]
		public GameObject sparksPrefab;

		[FoldoutGroup("VFX", 0)]
		public Vector2 collisionPoint;

		[FoldoutGroup("VFX", 0)]
		public float collisionRadius;

		[FoldoutGroup("VFX", 0)]
		public ContactFilter2D filter;

		[FoldoutGroup("VFX", 0)]
		public float secondsBetweenInstances = 0.5f;

		public GhostTrailGenerator ghostTrail;

		private bool _inverted;

		private EntityOrientation _orientation;

		private float _targetAngle;

		private Vector2 _targetPosition;

		private Vector2 _followPosition;

		private Vector2 _floatingOffset;

		private RaycastHit2D[] _results;

		private float _instantiationTimer;

		private StateMachine<QuirceSwordBehaviour> _fsm;

		private State<QuirceSwordBehaviour> stIdle;

		private State<QuirceSwordBehaviour> stSpinning;

		private State<QuirceSwordBehaviour> stSpinToPoint;

		private float _oldSmoothTranslationFactor;
	}
}
