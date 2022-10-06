using System;
using System.Collections;
using CreativeSpore.SmartColliders;
using Framework.Managers;
using Framework.Util;
using Gameplay.GameControllers.Penitent;
using Gameplay.GameControllers.Penitent.Abilities;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Player.Jump
{
	public class FallingForwardBehaviour : StateMachineBehaviour
	{
		private Penitent _penitent { get; set; }

		private SmartPlatformCollider Collider { get; set; }

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (!this._penitent)
			{
				this._penitent = Core.Logic.Penitent;
				Dash dash = this._penitent.Dash;
				dash.OnStartDash = (Core.SimpleEvent)Delegate.Combine(dash.OnStartDash, new Core.SimpleEvent(this.OnStartDash));
			}
			this.IsDashing = false;
			if (this.waitForEndOfFrame == null)
			{
				this.waitForEndOfFrame = new WaitForEndOfFrame();
			}
			if (this.Collider == null)
			{
				this.Collider = this._penitent.PlatformCharacterController.SmartPlatformCollider;
			}
			this._defaultColliderSize = this._penitent.DamageArea.DefaultSkinColliderSize;
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateUpdate(animator, stateInfo, layerIndex);
			if (!this.IsSideBlocked() && this._penitent.PlatformCharacterController.PlatformCharacterPhysics.Velocity.y < -3f)
			{
				this.SetColliderHorizontalOffset();
			}
			if (this._penitent.PlatformCharacterController.GroundDist > 1f)
			{
				return;
			}
			RaycastHit2D raycastHit2D = Physics2D.Raycast(Core.Logic.Penitent.transform.position, Vector2.down, 1.5f, this.RayCastLayerDetection);
			if (raycastHit2D && raycastHit2D.transform.GetComponent<Slope>())
			{
				this.SetDefaultCollider();
			}
		}

		private void OnStartDash()
		{
			this.IsDashing = true;
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			float num = Math.Abs(this._penitent.PlatformCharacterController.SlopeAngle);
			if (this._penitent.Status.IsGrounded && num < 1f)
			{
				this.SetDelayDefaultCollider();
			}
			else
			{
				this.SetDefaultCollider();
			}
		}

		private bool IsSideBlocked()
		{
			RaycastHit2D raycastHit2D = Physics2D.Raycast(this.GetRayCastOrigin(1.1f), -Vector2.right, 1.25f, this.RayCastLayerDetection);
			RaycastHit2D raycastHit2D2 = Physics2D.Raycast(this.GetRayCastOrigin(1.1f), Vector2.right, 1.25f, this.RayCastLayerDetection);
			RaycastHit2D raycastHit2D3 = Physics2D.Raycast(this.GetRayCastOrigin(-0.1f), -Vector2.right, 1.25f, this.RayCastLayerDetection);
			RaycastHit2D raycastHit2D4 = Physics2D.Raycast(this.GetRayCastOrigin(-0.1f), Vector2.right, 1.25f, this.RayCastLayerDetection);
			bool flag = raycastHit2D3.collider || raycastHit2D.collider;
			bool flag2 = raycastHit2D4.collider || raycastHit2D2.collider;
			return flag || flag2;
		}

		private void SetColliderHorizontalOffset()
		{
			if (this._colliderRepositioned)
			{
				return;
			}
			this._colliderRepositioned = true;
			this.Collider.Size = new Vector2(this.Collider.Size.x + this.HorizontalCollisionOffset, this.Collider.Size.y);
		}

		private void SetDefaultCollider()
		{
			if (!this._colliderRepositioned)
			{
				return;
			}
			this._colliderRepositioned = false;
			this.Collider.Size = this._defaultColliderSize;
		}

		private void SetDelayDefaultCollider()
		{
			if (!this._colliderRepositioned)
			{
				return;
			}
			Singleton<Core>.Instance.StartCoroutine(this.DefaultColliderCoroutine());
		}

		private IEnumerator DefaultColliderCoroutine()
		{
			yield return this.waitForEndOfFrame;
			if (!this.IsDashing)
			{
				this.SetDefaultCollider();
			}
			else
			{
				this._colliderRepositioned = false;
			}
			yield break;
		}

		private Vector2 GetRayCastOrigin(float heightOffset = 0f)
		{
			Vector3 position = Core.Logic.Penitent.transform.position;
			return new Vector2(position.x, position.y + heightOffset);
		}

		private void OnDestroy()
		{
			if (!this._penitent)
			{
				return;
			}
			Dash dash = this._penitent.Dash;
			dash.OnStartDash = (Core.SimpleEvent)Delegate.Remove(dash.OnStartDash, new Core.SimpleEvent(this.OnStartDash));
		}

		private bool _colliderRepositioned;

		private Vector3 _defaultColliderSize;

		public float HorizontalCollisionOffset = 0.3f;

		private bool IsDashing;

		public LayerMask RayCastLayerDetection;

		private WaitForEndOfFrame waitForEndOfFrame;
	}
}
