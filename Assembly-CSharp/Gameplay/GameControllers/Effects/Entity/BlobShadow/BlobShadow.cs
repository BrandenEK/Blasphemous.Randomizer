using System;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using Tools.Level.Layout;
using UnityEngine;

namespace Gameplay.GameControllers.Effects.Entity.BlobShadow
{
	[RequireComponent(typeof(SpriteRenderer))]
	public class BlobShadow : MonoBehaviour
	{
		public Entity Owner
		{
			get
			{
				return this._entity;
			}
		}

		private void Awake()
		{
			this.blobSpriteRenderer = base.GetComponent<SpriteRenderer>();
		}

		private void Start()
		{
			if (this._entity == null)
			{
				Debug.LogError("Blob shadow needs an entity to works");
			}
			else
			{
				this._currentLevel = Core.Logic.CurrentLevelConfig;
				this.SetLevelColor(this._currentLevel.levelShadowColor);
				this.SetHidden(this._currentLevel.hideShadows);
				this._blobHeight = this.blobSpriteRenderer.bounds.extents.y;
				this._bottomHits = new RaycastHit2D[1];
				this._isScaleReduce = false;
				this._entityShadow = this._entity.GetComponentInChildren<EntityShadow>();
				this._entity.OnDeath += this.EntityOnEntityDie;
			}
		}

		private void Update()
		{
			if (this._entity == null)
			{
				return;
			}
			if (this._entity.SpriteRenderer.isVisible)
			{
				this.CheckCliffBorder();
			}
			if (!this.ManuallyControllingAlpha)
			{
				this.SetShadowAlpha((!this._entity.Status.IsGrounded || this._entity.Status.Dead || !this._entity.Status.CastShadow) ? 0f : 1f);
			}
			if (this._isOnFloor)
			{
				this.SetRotation(this._entity.SlopeAngle);
				this.ReduceScale(false);
			}
			else
			{
				this.ReduceScale(true);
			}
		}

		private void LateUpdate()
		{
			if (this._entity == null)
			{
				return;
			}
			if (this._entity.Status.Dead)
			{
				return;
			}
			this.SetPosition();
		}

		public void SetEntity(Entity entity)
		{
			if (this._entity != null)
			{
				return;
			}
			this._entity = entity;
			this._entityCollider = this._entity.EntityDamageArea.DamageAreaCollider;
			this.Owner.Shadow = this;
			this.Owner.Status.CastShadow = true;
		}

		private void SetPosition()
		{
			if (this._entityCollider == null)
			{
				return;
			}
			Vector2 vector;
			vector..ctor(this._entityCollider.bounds.center.x, this._entityCollider.bounds.min.y);
			base.transform.position = vector;
		}

		public void SetShadowAlpha(float alpha)
		{
			float a = Mathf.Clamp01(alpha);
			Color color = this.blobSpriteRenderer.color;
			color.a = a;
			this.blobSpriteRenderer.color = color;
		}

		public float GetShadowAlpha()
		{
			return this.blobSpriteRenderer.color.a;
		}

		private void SetRotation(float slopeAngle)
		{
			if (slopeAngle >= 5f)
			{
				base.transform.eulerAngles = new Vector3(0f, 0f, 10f);
			}
			else if (slopeAngle <= -5f)
			{
				base.transform.eulerAngles = new Vector3(0f, 0f, -10f);
			}
			else
			{
				base.transform.rotation = Quaternion.identity;
			}
		}

		private void ReduceScale(bool reduce = true)
		{
			if (reduce && !this._isScaleReduce)
			{
				this._isScaleReduce = true;
				Vector3 localScale;
				localScale..ctor(0.5f, 0.5f, 1f);
				base.transform.localScale = localScale;
			}
			else if (!reduce && this._isScaleReduce)
			{
				this._isScaleReduce = false;
				Vector3 localScale2;
				localScale2..ctor(1f, 1f, 1f);
				base.transform.localScale = localScale2;
			}
		}

		private void SetHidden(bool hidden)
		{
			this.blobSpriteRenderer.enabled = !hidden;
		}

		public void SetLevelColor(Color color)
		{
			if (this.blobSpriteRenderer.color != color)
			{
				this.blobSpriteRenderer.color = color;
			}
		}

		private void EntityOnEntityDie()
		{
			this._entity = null;
			this.SetShadowAlpha(0f);
			Core.Logic.CurrentLevelConfig.BlobShadowManager.StoreBlobShadow(base.gameObject);
		}

		private void CheckCliffBorder()
		{
			if (this._entity == null || this._entityShadow == null)
			{
				return;
			}
			Vector2 vector;
			vector..ctor(this._entityCollider.bounds.min.x + this._entityShadow.ShadowXOffset, this._entityCollider.bounds.min.y + this._entityShadow.ShadowYOffset + this._blobHeight);
			Debug.DrawLine(vector, vector - Vector2.up * 0.5f, Color.white);
			bool flag = Physics2D.LinecastNonAlloc(vector, vector - Vector2.up * 1f, this._bottomHits, this.floorLayer) > 0;
			Vector2 vector2;
			vector2..ctor(this._entityCollider.bounds.max.x - this._entityShadow.ShadowXOffset, this._entityCollider.bounds.min.y + this._entityShadow.ShadowYOffset + this._blobHeight);
			Debug.DrawLine(vector2, vector2 - Vector2.up * 0.5f, Color.white);
			bool flag2 = Physics2D.LinecastNonAlloc(vector2, vector2 - Vector2.up * 1f, this._bottomHits, this.floorLayer) > 0;
			this._isOnFloor = (flag2 && flag);
		}

		private float _blobHeight;

		private RaycastHit2D[] _bottomHits;

		private LevelInitializer _currentLevel;

		[SerializeField]
		private Entity _entity;

		private Collider2D _entityCollider;

		private EntityShadow _entityShadow;

		private bool _isOnFloor;

		private bool _isScaleReduce;

		private const float ShadowRotation = 10f;

		private SpriteRenderer blobSpriteRenderer;

		public LayerMask floorLayer;

		public bool ManuallyControllingAlpha;
	}
}
