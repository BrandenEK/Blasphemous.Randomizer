using System;
using CreativeSpore.SmartColliders;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Penitent;
using Gameplay.GameControllers.Penitent.Damage;
using UnityEngine;

namespace Gameplay.GameControllers.Entities
{
	public class ThrowBack : Ability
	{
		public bool IsThrown { get; private set; }

		protected override void OnStart()
		{
			base.OnStart();
			this.m_smartCollider = Core.Logic.Penitent.GetComponentInChildren<SmartPlatformCollider>();
			this._rigidbody = base.EntityOwner.GetComponent<Rigidbody2D>();
			this._controller = base.EntityOwner.GetComponentInChildren<PlatformCharacterController>();
			this._damageArea = base.EntityOwner.GetComponentInChildren<PenitentDamageArea>();
			PenitentDamageArea damageArea = this._damageArea;
			damageArea.OnDamaged = (PenitentDamageArea.PlayerDamagedEvent)Delegate.Combine(damageArea.OnDamaged, new PenitentDamageArea.PlayerDamagedEvent(this.OnDamaged));
			Core.Events.OnEventLaunched += this.PlayerFallsTrap;
			this._landing = true;
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			if (this._rigidbody == null)
			{
				return;
			}
			this._currentTimeThreshold -= Time.deltaTime;
			if (!this._rigidbody.isKinematic && this.CalculateGroundDist() <= 0f && this._rigidbody.velocity.y <= 0f && (base.EntityOwner.Animator.GetCurrentAnimatorStateInfo(0).IsName("Falling Over") || base.EntityOwner.Animator.GetCurrentAnimatorStateInfo(0).IsName("ThrowbackDesc")))
			{
				if (base.Casting)
				{
					this.StopCastThrow();
				}
				else
				{
					this.ToggleAbilities(true);
					this.StopCastThrowback();
				}
			}
			if (!base.EntityOwner.Status.IsGrounded || this._landing || this._currentTimeThreshold > 0f || !this._rigidbody.isKinematic)
			{
				return;
			}
			this._landing = true;
			Core.Input.SetBlocker("PLAYER_LOGIC", false);
			base.EntityOwner.Animator.Play(this._fallingBackGroundedAnim);
		}

		private void OnTriggerEnter2D(Collider2D obstacle)
		{
			if (this._landing)
			{
				return;
			}
			if ((this.FloorCollisionLayers.value & 1 << obstacle.gameObject.layer) > 0)
			{
				this.StopCastThrow();
			}
			if (obstacle.gameObject.layer != LayerMask.NameToLayer("OneWayDown"))
			{
				return;
			}
			if (this.IsOwnerFalling && this.IsOnCollider(obstacle))
			{
				this.StopCastThrow();
			}
		}

		private void PlayerFallsTrap(string id, string parameter)
		{
			if (id.Equals("PENITENT_KILLED") || parameter.Equals("ABYSS") || parameter.Equals("SPIKES"))
			{
				this.StopCastThrowback();
			}
		}

		private void OnDestroy()
		{
			Core.Events.OnEventLaunched -= this.PlayerFallsTrap;
			if (this._damageArea)
			{
				PenitentDamageArea damageArea = this._damageArea;
				damageArea.OnDamaged = (PenitentDamageArea.PlayerDamagedEvent)Delegate.Remove(damageArea.OnDamaged, new PenitentDamageArea.PlayerDamagedEvent(this.OnDamaged));
			}
		}

		private void OnDamaged(Penitent damaged, Hit hit)
		{
			if (hit.DamageType != DamageArea.DamageType.Heavy || !this._landing)
			{
				return;
			}
			this._hit = hit;
			if (!base.EntityOwner.Status.Dead)
			{
				base.Cast();
			}
			else
			{
				this.ToggleAbilities(false);
				this.CastThrowback();
			}
		}

		protected override void OnCastStart()
		{
			base.OnCastStart();
			this.CastThrowback();
			Core.Logic.Penitent.Parry.StopCast();
		}

		protected override void OnCastEnd(float castingTime)
		{
			base.OnCastEnd(castingTime);
			this.StopCastThrowback();
		}

		private void CastThrowback()
		{
			if (!base.EntityOwner)
			{
				return;
			}
			this._currentTimeThreshold = 0.01f;
			this._landing = false;
			Core.Input.SetBlocker("PLAYER_LOGIC", true);
			base.EntityOwner.Animator.Play(this._fallingOverAnim);
			this.SetRigidbodyType(RigidbodyType2D.Dynamic);
			this.Throw(this.GetGoal(this._hit));
			if (base.EntityOwner.Shadow)
			{
				base.EntityOwner.Shadow.gameObject.SetActive(false);
			}
			this.IsThrown = true;
		}

		private void StopCastThrowback()
		{
			if (!base.EntityOwner || this._currentTimeThreshold > 0f)
			{
				return;
			}
			this._controller.InstantVelocity = Vector3.zero;
			this._rigidbody.velocity = Vector2.zero;
			this.SetRigidbodyType(RigidbodyType2D.Kinematic);
			if (!base.EntityOwner.Dead)
			{
				base.EntityOwner.Animator.SetBool(ThrowBack.ThrowParam, false);
			}
			if (base.EntityOwner.Shadow)
			{
				base.EntityOwner.Shadow.gameObject.SetActive(true);
			}
			bool isVisible = base.EntityOwner.SpriteRenderer.isVisible;
			if (isVisible)
			{
				Core.Logic.CameraManager.ProCamera2DShake.ShakeUsingPreset("HardFall");
			}
			this.IsThrown = false;
		}

		private void Throw(Vector3 goal)
		{
			float num = goal.x - base.EntityOwner.transform.position.x;
			float num2 = goal.y - base.EntityOwner.transform.position.y;
			float f = Mathf.Atan((num2 + 10f) / num);
			float num3 = num / Mathf.Cos(f);
			float x = num3 * Mathf.Cos(f);
			float y = num3 * Mathf.Sin(f);
			this._rigidbody.velocity = new Vector2(x, y);
		}

		private void SetRigidbodyType(RigidbodyType2D rigidbodyType)
		{
			if (this._rigidbody == null)
			{
				return;
			}
			if (this._rigidbody.bodyType != rigidbodyType)
			{
				this._rigidbody.bodyType = rigidbodyType;
			}
			if (this._rigidbody.bodyType == RigidbodyType2D.Dynamic)
			{
				this._rigidbody.gravityScale = 3f;
			}
		}

		private Vector3 GetGoal(Hit hit)
		{
			GameObject attackingEntity = hit.AttackingEntity;
			Vector3 position = base.EntityOwner.transform.position;
			float y = position.y;
			float num;
			if (hit.ThrowbackDirByOwnerPosition)
			{
				num = ((attackingEntity.transform.position.x < base.EntityOwner.transform.position.x) ? (position.x + this.Distance * hit.Force) : (position.x - this.Distance * hit.Force));
			}
			else
			{
				num = ((attackingEntity.GetComponent<Entity>().Status.Orientation != EntityOrientation.Left) ? (position.x + this.Distance * hit.Force) : (position.x - this.Distance * hit.Force));
			}
			if (base.EntityOwner is Penitent && Core.Logic.Penitent.IsGrabbingCliffLede)
			{
				if (base.EntityOwner.OrientationBeforeHit == EntityOrientation.Right)
				{
					num = ((num >= position.x) ? (position.x - 0.5f) : num);
				}
				else
				{
					num = ((num <= position.x) ? (position.x + 0.5f) : num);
				}
			}
			return new Vector2(num, y);
		}

		private void StopCastThrow()
		{
			if (!base.EntityOwner.Status.Dead)
			{
				base.StopCast();
			}
			else
			{
				this.ToggleAbilities(true);
				this.StopCastThrowback();
			}
		}

		public bool IsOwnerFalling
		{
			get
			{
				return this._rigidbody.velocity.y < 0f;
			}
		}

		private bool IsOnCollider(Collider2D obstacleCollider2D)
		{
			float y = this._damageArea.DamageAreaCollider.bounds.min.y;
			float y2 = obstacleCollider2D.bounds.max.y;
			return y >= y2 - 0.1f;
		}

		protected float CalculateGroundDist()
		{
			float num = float.MaxValue;
			for (int i = 0; i < this.m_smartCollider.BottomCheckPoints.Count; i++)
			{
				SmartRaycastHit smartRaycastHit = this.m_smartCollider.SmartRaycast(base.transform.TransformPoint(this.m_smartCollider.BottomCheckPoints[i]), -base.transform.up, float.MaxValue, this.m_smartCollider.LayerCollision | this.m_smartCollider.OneWayCollisionDown);
				if (smartRaycastHit != null)
				{
					num = Mathf.Min(num, smartRaycastHit.distance - this.m_smartCollider.SkinBottomWidth);
				}
			}
			return num;
		}

		public float Distance = 10f;

		public LayerMask FloorCollisionLayers;

		private Rigidbody2D _rigidbody;

		private Hit _hit;

		private readonly int _fallingOverAnim = Animator.StringToHash("Falling Over");

		private readonly int _fallingBackGroundedAnim = Animator.StringToHash("Grounding Over");

		private PlatformCharacterController _controller;

		private PenitentDamageArea _damageArea;

		private bool _landing;

		private const float TimeThreshold = 0.01f;

		private float _currentTimeThreshold;

		private SmartPlatformCollider m_smartCollider;

		private static readonly int ThrowParam = Animator.StringToHash("THROW");
	}
}
