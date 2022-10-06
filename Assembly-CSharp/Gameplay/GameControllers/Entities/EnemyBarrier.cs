using System;
using FMODUnity;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Entities
{
	[RequireComponent(typeof(Collider2D))]
	public class EnemyBarrier : Trait
	{
		protected override void OnAwake()
		{
			base.OnAwake();
			this._barrierCollider = base.GetComponent<BoxCollider2D>();
			if (this._barrierCollider == null)
			{
				this._barrierCollider = base.GetComponent<PolygonCollider2D>();
			}
			this._contactHit = new Hit
			{
				AttackingEntity = base.EntityOwner.gameObject,
				DamageAmount = 0f,
				DamageElement = DamageArea.DamageElement.Contact,
				DamageType = DamageArea.DamageType.Normal,
				HitSoundId = this.HitSoundFx,
				Unnavoidable = true
			};
			if (base.EntityOwner != null && base.EntityOwner is Enemy)
			{
				this.behaviour = base.EntityOwner.GetComponent<EnemyBehaviour>();
			}
		}

		protected override void OnStart()
		{
			base.OnStart();
			if (this.HitVfx)
			{
				PoolManager.Instance.CreatePool(this.HitVfx, 1);
			}
			base.EntityOwner.OnDeath += this.OnOwnerDeath;
		}

		private void OnOwnerDeath()
		{
			base.EntityOwner.OnDeath -= this.OnOwnerDeath;
			base.enabled = false;
		}

		private void CheckDeactivation()
		{
			if (this.behaviour != null)
			{
				this._barrierCollider.enabled = this.behaviour.IsPlayerSeen();
			}
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			if (this.activateOnlyIfPlayerSeen)
			{
				this.CheckDeactivation();
			}
			if (this._cdCounter > 0f)
			{
				this._cdCounter -= Time.deltaTime;
				return;
			}
			if (this.UseSpriteRenderer && !base.EntityOwner.SpriteRenderer.isVisible)
			{
				return;
			}
			this._bottomRight = new Vector2(this._barrierCollider.bounds.max.x, this._barrierCollider.bounds.min.y + this.HeightOffset);
			this._bottomLeft = new Vector2(this._barrierCollider.bounds.min.x, this._barrierCollider.bounds.min.y + this.HeightOffset);
			RaycastHit2D raycastHit2D = Physics2D.Raycast(this._bottomRight, Vector2.right, this.RaycastDistance, this.TargetLayer);
			RaycastHit2D raycastHit2D2 = Physics2D.Raycast(this._bottomLeft, -Vector2.right, this.RaycastDistance, this.TargetLayer);
			bool flag = raycastHit2D;
			bool flag2 = raycastHit2D2;
			if (this.OnlyForward)
			{
				if (base.EntityOwner.Status.Orientation == EntityOrientation.Left)
				{
					flag = false;
				}
				else if (base.EntityOwner.Status.Orientation == EntityOrientation.Right)
				{
					flag2 = false;
				}
			}
			if ((flag || flag2) && Core.Logic.Penitent.IsDashing)
			{
				this.StopPlayerDash();
				if (this.HitVfx)
				{
					int num = (!flag2) ? 1 : -1;
					Vector3 vector;
					vector..ctor(this.HitVfxOffset.x * (float)num, this.HitVfxOffset.y, 0f);
					GameObject gameObject = PoolManager.Instance.ReuseObject(this.HitVfx, base.transform.position + vector, Quaternion.identity, false, 1).GameObject;
					gameObject.transform.localScale = new Vector3((float)num, 1f, 1f);
				}
				this._cdCounter = this.cd;
			}
		}

		private void StopPlayerDash()
		{
			Core.Logic.Penitent.Damage(this._contactHit);
		}

		private void OnDrawGizmos()
		{
			if (this._barrierCollider == null)
			{
				this._barrierCollider = base.GetComponent<BoxCollider2D>();
				if (this._barrierCollider == null)
				{
					this._barrierCollider = base.GetComponent<PolygonCollider2D>();
				}
			}
			Vector2 vector;
			vector..ctor(this._barrierCollider.bounds.max.x, this._barrierCollider.bounds.min.y + this.HeightOffset);
			Vector2 vector2;
			vector2..ctor(this._barrierCollider.bounds.min.x, this._barrierCollider.bounds.min.y + this.HeightOffset);
			Gizmos.color = this.gizmoColor;
			Gizmos.DrawRay(vector, Vector2.right * this.RaycastDistance);
			Gizmos.DrawRay(vector2, -Vector2.right * this.RaycastDistance);
		}

		[FoldoutGroup("Raycast Settings", 0)]
		public LayerMask TargetLayer;

		[FoldoutGroup("Raycast Settings", 0)]
		public float HeightOffset;

		[FoldoutGroup("Raycast Settings", 0)]
		public Color gizmoColor;

		private Collider2D _barrierCollider;

		private Vector2 _bottomLeft;

		private Vector2 _bottomRight;

		[FoldoutGroup("Activation settings", 0)]
		private bool activateOnlyIfPlayerSeen;

		[FoldoutGroup("Activation settings", 0)]
		public bool UseSpriteRenderer = true;

		[FoldoutGroup("Hit Settings", 0)]
		public DamageArea.DamageType DamageType;

		[FoldoutGroup("Hit Settings", 0)]
		[EventRef]
		public string HitSoundFx;

		[FoldoutGroup("Hit Settings", 0)]
		public float RaycastDistance = 0.1f;

		[FoldoutGroup("Hit Settings", 0)]
		public bool OnlyForward;

		[FoldoutGroup("Hit Settings", 0)]
		public GameObject HitVfx;

		[FoldoutGroup("Hit Settings", 0)]
		public Vector2 HitVfxOffset;

		private Hit _contactHit;

		private bool _doDamage;

		private float cd = 0.5f;

		private float _cdCounter;

		private EnemyBehaviour behaviour;
	}
}
