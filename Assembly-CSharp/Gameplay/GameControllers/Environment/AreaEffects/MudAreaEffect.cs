using System;
using CreativeSpore.SmartColliders;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Penitent.Abilities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Environment.AreaEffects
{
	public class MudAreaEffect : AreaEffect
	{
		protected override void OnStart()
		{
			base.OnStart();
			if (this.vfxOnEnter != null)
			{
				PoolManager.Instance.CreatePool(this.vfxOnEnter, 2);
			}
			if (this.vfxOnExit != null)
			{
				PoolManager.Instance.CreatePool(this.vfxOnExit, 2);
			}
		}

		protected override void OnEnterAreaEffect(Collider2D other)
		{
			base.OnEnterAreaEffect(other);
			if (this.vfxOnEnter != null)
			{
				this.ShowEffect(new Vector3(other.transform.position.x, base.transform.position.y + this.vfxYOffset, base.transform.position.z), this.vfxOnEnter);
			}
			foreach (GameObject gameObject in this.Population)
			{
				Entity componentInParent = gameObject.GetComponentInParent<Entity>();
				this.Controller = componentInParent.GetComponentInChildren<PlatformCharacterController>();
				this.Dash = componentInParent.GetComponentInChildren<Dash>();
				this.Animator = componentInParent.Animator;
				if (this.Controller != null)
				{
					this._defaultJumpingSpeed = this.Controller.JumpingSpeed;
					this._defaultWalkingAcceleration = this.Controller.WalkingAcc;
					this._defaultMaxWalkingSpeed = this.Dash.DefaultMoveSetting.Speed;
					this._defaultWalkingDrag = this.Dash.DefaultMoveSetting.Drag;
				}
				if (this.Dash)
				{
					this._defaultDashSettings = new Dash.MoveSetting
					{
						Drag = this.Dash.DashDrag,
						Speed = this.Dash.DashMaxWalkingSpeed
					};
				}
			}
		}

		protected override void OnStayAreaEffect()
		{
			base.OnStayAreaEffect();
			this.ApplyMudEffects();
		}

		protected override void OnExitAreaEffect(Collider2D other)
		{
			base.OnExitAreaEffect(other);
			if (this.vfxOnExit != null)
			{
				this.ShowEffect(new Vector3(other.transform.position.x, base.transform.position.y + this.vfxYOffset, base.transform.position.z), this.vfxOnExit);
			}
			this.ApplyDefaultWalkValues();
			this.ApplyDefaultDashValues();
			this.ApplyDefaultAnimationSpeed();
		}

		private void ShowEffect(Vector3 position, GameObject fx)
		{
			PoolManager.Instance.ReuseObject(fx, position, Quaternion.identity, false, 1);
		}

		public override void EnableEffect(bool enableEffect = true)
		{
			base.EnableEffect(enableEffect);
			this.ApplyDefaultWalkValues();
			this.ApplyDefaultDashValues();
			this.ApplyDefaultAnimationSpeed();
		}

		public void ApplyMudEffects()
		{
			if (this.Controller == null)
			{
				return;
			}
			this.Controller.JumpingSpeed = this.JumpingSpeed;
			this.Controller.WalkingDrag = this.WalkingDrag;
			this.Controller.WalkingAcc = this.WalkingAcceleration;
			this.Controller.MaxWalkingSpeed = this.MaxWalkingSpeed;
			if (this.Dash == null)
			{
				return;
			}
			this.Dash.DashMoveSetting.Speed = this.DashSettings.Speed;
			this.Dash.DashMoveSetting.Drag = this.DashSettings.Drag;
			if (this.Animator == null)
			{
				return;
			}
			this.Animator.speed = ((!this.Animator.GetCurrentAnimatorStateInfo(0).IsName("Run")) ? 1f : 0.7f);
		}

		private void ApplyDefaultWalkValues()
		{
			if (this.Controller == null)
			{
				return;
			}
			this.Controller.JumpingSpeed = this._defaultJumpingSpeed;
			this.Controller.WalkingDrag = this._defaultWalkingDrag;
			this.Controller.WalkingAcc = this._defaultWalkingAcceleration;
			this.Controller.MaxWalkingSpeed = this._defaultMaxWalkingSpeed;
		}

		private void ApplyDefaultDashValues()
		{
			if (this.Dash == null)
			{
				return;
			}
			this.Dash.DashMoveSetting = this._defaultDashSettings;
		}

		private void ApplyDefaultAnimationSpeed()
		{
			if (this.Animator == null)
			{
				return;
			}
			this.Animator.speed = 1f;
		}

		private void OnDisable()
		{
			this.ApplyDefaultWalkValues();
			this.ApplyDefaultDashValues();
			this.ApplyDefaultAnimationSpeed();
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.DrawLine(base.transform.position + Vector3.left + Vector3.up * this.vfxYOffset, base.transform.position + Vector3.right + Vector3.up * this.vfxYOffset);
		}

		[FoldoutGroup("Area player's movement settings", 0)]
		public float JumpingSpeed;

		[FoldoutGroup("Area player's movement settings", 0)]
		public float WalkingDrag;

		[FoldoutGroup("Area player's movement settings", 0)]
		public float WalkingAcceleration;

		[FoldoutGroup("Area player's movement settings", 0)]
		public float MaxWalkingSpeed;

		[FoldoutGroup("Area player's movement settings", 0)]
		public Dash.MoveSetting DashSettings;

		private float _defaultJumpingSpeed;

		private float _defaultWalkingDrag;

		private float _defaultWalkingAcceleration;

		private float _defaultMaxWalkingSpeed;

		private Dash.MoveSetting _defaultDashSettings;

		public bool unafectedByRelic;

		protected PlatformCharacterController Controller;

		protected Dash Dash;

		protected Animator Animator;

		public GameObject vfxOnEnter;

		public GameObject vfxOnExit;

		public float vfxYOffset;
	}
}
