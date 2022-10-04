using System;
using System.Collections;
using CreativeSpore.SmartColliders;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Effects.Player.GhostTrail;
using Gameplay.GameControllers.Penitent.Movement;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Penitent.Abilities
{
	public class Dash : Ability
	{
		public bool StandUpAfterDash { get; set; }

		public bool CrouchAfterDash { get; set; }

		public bool StopByDamage { get; private set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			this._playerController = base.EntityOwner.GetComponent<PlatformCharacterController>();
			this.DefaultMoveSetting = new Dash.MoveSetting(this._playerController.WalkingDrag, this._playerController.MaxWalkingSpeed);
			this.DashMoveSetting = new Dash.MoveSetting(this.DashDrag, this.DashMaxWalkingSpeed);
			this._deltaTimeDashing = 0f;
			if (this.cooldownFinishedFX != null)
			{
				PoolManager.Instance.CreatePool(this.cooldownFinishedFX, 1);
			}
		}

		protected override void OnStart()
		{
			base.OnStart();
			if (!this._penitent)
			{
				this._penitent = Core.Logic.Penitent;
			}
			this._penitentCollider = this._playerController.SmartPlatformCollider;
			Vector3 center = this._penitentCollider.Center;
			Vector2 size = this._penitentCollider.Size;
			this._defaultCollisionSkin = new Penitent.CollisionSkin(center, size);
			this._dashCollisionSkin = new Penitent.CollisionSkin(this.DashCollisionCenter, this.DashCollisionSize);
			BoxCollider2D boxCollider2D = (BoxCollider2D)this._penitent.DamageArea.DamageAreaCollider;
			this._defaultDamageColliderSize = boxCollider2D.size;
			this._defaultDamageColliderOffset = boxCollider2D.offset;
			this._dashDamageAreaSize = new Vector2(this._defaultDamageColliderSize.x, this.DamageAreaDashHeight);
			this._dashDamageAreaOffset = new Vector2(this._defaultDamageColliderOffset.x, this.DamageAreaDashYOffset);
			base.EntityOwner.OnDamaged += this.OnDamaged;
			CharacterMotionProfile.OnMotionProfileLoaded += this.OnMotionProfileLoaded;
		}

		private void OnMotionProfileLoaded()
		{
			this.DefaultMoveSetting = new Dash.MoveSetting(this._playerController.WalkingDrag, this._playerController.MaxWalkingSpeed);
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			if (!this._penitent.Status.IsGrounded)
			{
				this.CrouchAfterDash = false;
				base.StopCast();
			}
			if (base.Casting)
			{
				this._deltaTimeDashing += Time.deltaTime;
				this.AddDashForce();
				if (this._deltaTimeDashing >= base.EntityOwner.Stats.DashRide.Final || this._penitent.HasFlag("FRONT_BLOCKED"))
				{
					base.StopCast();
				}
			}
			else
			{
				this._deltaTimeDashing = 0f;
			}
		}

		protected override void OnCastStart()
		{
			base.OnCastStart();
			if (this.OnStartDash != null)
			{
				this.OnStartDash();
			}
			base.SetCooldown(base.EntityOwner.Stats.DashCooldown.Final);
			if (!this._penitent.IsDashing)
			{
				this._penitent.IsDashing = true;
			}
			this._penitent.DamageArea.IncludeEnemyLayer(false);
			this._penitent.DamageArea.EnableEnemyAttack(false);
			this.SetDashSkinCollision();
			if (!this.GhostTrailGenerator.EnableGhostTrail)
			{
				this.GhostTrailGenerator.EnableGhostTrail = true;
			}
			this._penitent.DashDustGenerator.GetStartDashDust();
		}

		protected override void OnCastEnd(float castingTime)
		{
			base.OnCastEnd(castingTime);
			if (this._penitent.IsDashing)
			{
				this._penitent.IsDashing = false;
			}
			this._playerController.WalkingDrag = this.DefaultMoveSetting.Drag;
			this._playerController.MaxWalkingSpeed = this.DefaultMoveSetting.Speed;
			this._isDashDirectionSet = false;
			if (!this.StopByDamage)
			{
				this.SetDefaultSkinCollision();
			}
			else
			{
				base.StartCoroutine(this.DelaySetDefaultCollision());
			}
			this._penitent.DamageArea.IncludeEnemyLayer(true);
			this._penitent.DamageArea.EnableEnemyAttack(true);
			if (this._penitent.Status.Unattacable)
			{
				this._penitent.Status.Unattacable = false;
			}
			if (this.GhostTrailGenerator.EnableGhostTrail)
			{
				this.GhostTrailGenerator.EnableGhostTrail = false;
			}
			if (this.OnFinishDash != null)
			{
				this.OnFinishDash();
			}
		}

		protected override void OnCooldownFinished()
		{
			base.OnCooldownFinished();
			if (this.cooldownFinishedFX != null)
			{
				PoolManager.Instance.ReuseObject(this.cooldownFinishedFX, base.transform.position + this.fxOffset, Quaternion.identity, false, 1);
			}
		}

		private IEnumerator DelaySetDefaultCollision()
		{
			yield return new WaitForSeconds(0.85f);
			this.StopByDamage = false;
			this.SetDefaultSkinCollision();
			yield break;
		}

		public void AddDashForce()
		{
			if (!this._penitent.Status.IsGrounded || this._penitent.HasFlag("FRONT_BLOCKED"))
			{
				return;
			}
			this._playerController.WalkingDrag = this.DashMoveSetting.Drag;
			this._playerController.MaxWalkingSpeed = this.DashMoveSetting.Speed;
			if (!this._isDashDirectionSet)
			{
				this._isDashDirectionSet = true;
				this._dashDirection = this._penitent.PlatformCharacterInput.Rewired.GetAxisRaw(0);
			}
			if (this._dashDirection < 0f)
			{
				this._playerController.SetActionState(eControllerActions.Left, true);
				this._penitent.SetOrientation(EntityOrientation.Left, true, false);
			}
			else if (this._dashDirection > 0f)
			{
				this._playerController.SetActionState(eControllerActions.Right, true);
				this._penitent.SetOrientation(EntityOrientation.Right, true, false);
			}
			else
			{
				this._playerController.SetActionState(eControllerActions.Right, this._penitent.Status.Orientation == EntityOrientation.Right);
				this._playerController.SetActionState(eControllerActions.Left, this._penitent.Status.Orientation == EntityOrientation.Left);
			}
		}

		private void OnDamaged()
		{
			if (!this._penitent.IsDashing)
			{
				return;
			}
			this.StopByDamage = true;
			base.StopCast();
		}

		public void SetDashSkinCollision()
		{
			this._penitentCollider.Center = this._dashCollisionSkin.CenterCollision;
			this._penitentCollider.Size = this._dashCollisionSkin.CollisionSize;
		}

		public void SetDefaultSkinCollision()
		{
			this._penitentCollider.Center = this._defaultCollisionSkin.CenterCollision;
			this._penitentCollider.Size = this._defaultCollisionSkin.CollisionSize;
		}

		public bool IsUpperBlocked
		{
			get
			{
				float num = Mathf.Abs(this._penitent.PlatformCharacterController.SlopeAngle);
				if (num > 1f)
				{
					return false;
				}
				Vector3 position = base.EntityOwner.transform.position;
				position.y += 0.1f;
				Vector2 origin = position;
				Vector2 origin2 = position;
				origin.x = position.x - 0.25f;
				origin2.x = position.x + 0.25f;
				RaycastHit2D hit = Physics2D.Raycast(origin, base.EntityOwner.transform.up, 2.5f, this.UpperBlockLayer);
				RaycastHit2D hit2 = Physics2D.Raycast(origin2, base.EntityOwner.transform.up, 2.5f, this.UpperBlockLayer);
				return hit || hit2;
			}
		}

		private void OnDestroy()
		{
			base.EntityOwner.OnDamaged -= this.OnDamaged;
			CharacterMotionProfile.OnMotionProfileLoaded -= this.OnMotionProfileLoaded;
		}

		public Core.SimpleEvent OnStartDash;

		public Core.SimpleEvent OnFinishDash;

		public GameObject cooldownFinishedFX;

		public Vector2 fxOffset;

		public const float MAX_WALK_SPEED = 5f;

		private Penitent _penitent;

		public float DashDrag;

		public float DashMaxWalkingSpeed;

		[SerializeField]
		protected GhostTrailGenerator GhostTrailGenerator;

		[Tooltip("Mandatoy completion of the dash animation before can dash.")]
		[Range(0f, 1f)]
		public float CompletionBeforeDash;

		private float _deltaTimeDashing;

		private bool _isDashDirectionSet;

		private float _dashDirection;

		public Vector3 DashCollisionCenter;

		public Vector2 DashCollisionSize;

		[FoldoutGroup("Damage Area Dash Boundaries", true, 0)]
		public float DamageAreaDashHeight;

		[FoldoutGroup("Damage Area Dash Boundaries", true, 0)]
		public float DamageAreaDashYOffset;

		private Vector2 _defaultDamageColliderSize;

		private Vector2 _defaultDamageColliderOffset;

		private Vector2 _dashDamageAreaSize;

		private Vector2 _dashDamageAreaOffset;

		private Vector2 _damageCollider;

		private Vector2 _defaultDamageColliderHeight;

		private Penitent.CollisionSkin _defaultCollisionSkin;

		private Penitent.CollisionSkin _dashCollisionSkin;

		private SmartPlatformCollider _penitentCollider;

		public Dash.MoveSetting DefaultMoveSetting;

		public Dash.MoveSetting DashMoveSetting;

		private PlatformCharacterController _playerController;

		public LayerMask UpperBlockLayer;

		[Serializable]
		public struct MoveSetting
		{
			public MoveSetting(float drag, float speed)
			{
				this.Drag = drag;
				this.Speed = speed;
			}

			public float Drag;

			public float Speed;
		}
	}
}
