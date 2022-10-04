using System;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Environment;
using Gameplay.GameControllers.Penitent.Damage;
using UnityEngine;

namespace Gameplay.GameControllers.Penitent.Abilities
{
	public class GrabCliffLede : MonoBehaviour
	{
		private void Awake()
		{
			this.grabCliffLedeCollider = base.GetComponent<BoxCollider2D>();
		}

		private void Start()
		{
			this._penitent = Core.Logic.Penitent;
			PenitentDamageArea damageArea = this._penitent.DamageArea;
			damageArea.OnDamaged = (PenitentDamageArea.PlayerDamagedEvent)Delegate.Combine(damageArea.OnDamaged, new PenitentDamageArea.PlayerDamagedEvent(this.PenitentOnDamaged));
		}

		private void Update()
		{
			this.EnablingCollider();
			if (!this._isGrabbedCliffLede)
			{
				this.remainCooldown -= Time.deltaTime;
			}
			if (!this._grabbedCliffLede || this.remainCooldown > 0f)
			{
				return;
			}
			if (this._penitent.IsGrabbingCliffLede && (!this._grabbedCliffLede.enabled || !this._grabbedCliffLede.gameObject.activeInHierarchy))
			{
				if (!this._grabbedCliffLede.enabled)
				{
					this._penitent.AnimatorInyector.ClimbCliffLede();
				}
				else if (!this._grabbedCliffLede.gameObject.activeInHierarchy)
				{
					this._penitent.AnimatorInyector.ReleaseCliffLede();
					this._penitent.AnimatorInyector.ManualHangOffCliff();
				}
			}
			this._isAirAttacking = false;
			if (!this._penitent.Status.IsGrounded)
			{
				this._isAirAttacking = (this._penitent.Animator.GetCurrentAnimatorStateInfo(0).IsName("Air Attack 1") || this._penitent.Animator.GetCurrentAnimatorStateInfo(0).IsName("Air Attack 2"));
			}
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			if ((this.cliffLedeLayer.value & 1 << other.gameObject.layer) > 0)
			{
				this._grabbedCliffLede = other;
			}
		}

		private void OnTriggerStay2D(Collider2D cliffLedeCollider)
		{
			if ((this.cliffLedeLayer.value & 1 << cliffLedeCollider.gameObject.layer) <= 0 || this._penitent.IsGrabbingCliffLede || this.remainCooldown > 0f)
			{
				return;
			}
			CliffLede component = cliffLedeCollider.GetComponent<CliffLede>();
			if (component != null && !this._penitent.IsJumpingOff && !this._penitent.IsDashing && this._penitent.AnimatorInyector.IsFalling && !this._isGrabbedCliffLede && !this._isAirAttacking)
			{
				if (!component.isClimbable || this._penitent.Status.IsGrounded)
				{
					return;
				}
				this._penitent.AnimatorInyector.UpdateAirAttackingAction();
				if (this._penitent.AnimatorInyector.IsAirAttacking)
				{
					return;
				}
				EntityOrientation orientation = this._penitent.Status.Orientation;
				this._isGrabbedCliffLede = true;
				this.grabCliffLede(component, orientation);
			}
		}

		private void OnTriggerExit2D(Collider2D other)
		{
			if (this._isGrabbedCliffLede)
			{
				this._isGrabbedCliffLede = !this._isGrabbedCliffLede;
			}
		}

		private void PenitentOnDamaged(Penitent damaged, Hit hit)
		{
			if (this._penitent.canClimbCliffLede && hit.DamageType == DamageArea.DamageType.Heavy)
			{
				this._penitent.canClimbCliffLede = false;
			}
		}

		public Bounds CliffColliderBoundaries
		{
			get
			{
				return this._grabbedCliffLede.bounds;
			}
		}

		private void EnablingCollider()
		{
			this.deltaEnabligTimeLapse += Time.deltaTime;
			if (this.deltaEnabligTimeLapse >= this.enabligTimeLapse)
			{
				if (!this.grabCliffLedeCollider.enabled)
				{
					this.grabCliffLedeCollider.enabled = true;
				}
			}
			else if (this.grabCliffLedeCollider.enabled)
			{
				this.grabCliffLedeCollider.enabled = false;
			}
		}

		private void grabCliffLede(CliffLede cliffLede, EntityOrientation playerOrientation)
		{
			this._penitent.IsGrabbingCliffLede = (cliffLede.CliffLedeGrabSideAllowed == playerOrientation);
			this._penitent.CliffLedeOrientation = cliffLede.CliffLedeGrabSideAllowed;
			this._penitent.RootTargetPosition = cliffLede.RootTarget.transform.position;
		}

		public void ReleaseCliffLede()
		{
			this._penitent.IsGrabbingCliffLede = false;
			this.deltaEnabligTimeLapse = 0f;
			this.remainCooldown = this.grabCliffLedeCooldown;
		}

		private void OnDestroy()
		{
			if (this._penitent)
			{
				PenitentDamageArea damageArea = this._penitent.DamageArea;
				damageArea.OnDamaged = (PenitentDamageArea.PlayerDamagedEvent)Delegate.Remove(damageArea.OnDamaged, new PenitentDamageArea.PlayerDamagedEvent(this.PenitentOnDamaged));
			}
		}

		private Penitent _penitent;

		public LayerMask cliffLedeLayer;

		private float deltaEnabligTimeLapse;

		private readonly float enabligTimeLapse = 0.25f;

		private BoxCollider2D grabCliffLedeCollider;

		private Collider2D _grabbedCliffLede;

		private bool _isGrabbedCliffLede;

		private bool _isAirAttacking;

		[SerializeField]
		private float grabCliffLedeCooldown = 0.2f;

		private float remainCooldown;
	}
}
