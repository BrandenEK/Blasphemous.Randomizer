using System;
using Framework.FrameworkCore;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Framework.IA
{
	public class CliffSensor : MonoBehaviour
	{
		private void Awake()
		{
			this.entity = base.GetComponentInParent<Entity>();
		}

		private void Start()
		{
		}

		private void Update()
		{
			this.entity.Status.IsOnCliffLede = (this.isOnCliffLede(this.rayCasts) && this.IsFacingCliffLede());
		}

		private void FixedUpdate()
		{
			if (this.IsNearbyCliff() && !this.entity.HasFlag("NEARBY_CLIFF"))
			{
				this.entity.SetFlag("NEARBY_CLIFF", true);
			}
			else if (!this.IsNearbyCliff() && this.entity.HasFlag("NEARBY_CLIFF"))
			{
				this.entity.SetFlag("NEARBY_CLIFF", false);
			}
		}

		private bool IsNearbyCliff()
		{
			this.hitRight = Physics2D.Raycast(base.transform.position, Vector2.right, this.cliffScopeDetection, this.targetLayer);
			this.hitLeft = Physics2D.Raycast(base.transform.position, -Vector2.right, this.cliffScopeDetection, this.targetLayer);
			this.rayCasts = new RaycastHit2D[]
			{
				this.hitRight,
				this.hitLeft
			};
			Debug.DrawRay(base.transform.position, Vector2.right * this.cliffScopeDetection, Color.blue);
			Debug.DrawRay(base.transform.position, -Vector2.right * this.cliffScopeDetection, Color.red);
			if (this.hitRight.collider != null)
			{
				this.hitSide = EntityOrientation.Right;
			}
			if (this.hitLeft.collider != null)
			{
				this.hitSide = EntityOrientation.Left;
			}
			return this.hitLeft || this.hitRight;
		}

		private bool isOnCliffLede(RaycastHit2D[] rayCasts)
		{
			bool result = false;
			if (this.IsNearbyCliff())
			{
				byte b = 0;
				while ((int)b < rayCasts.Length)
				{
					if (rayCasts[(int)b].collider != null)
					{
						this.currentDistanceToCliffLede = rayCasts[(int)b].distance;
						if (this.currentDistanceToCliffLede <= this.vanishingPointDistance)
						{
							result = true;
							break;
						}
					}
					b += 1;
				}
			}
			return result;
		}

		public bool IsFacingCliffLede()
		{
			return this.entity.Status.Orientation == this.hitSide;
		}

		[Range(0f, 3f)]
		public float cliffScopeDetection;

		private float currentDistanceToCliffLede;

		private Entity entity;

		private RaycastHit2D hitLeft;

		private RaycastHit2D hitRight;

		private EntityOrientation hitSide;

		private RaycastHit2D[] rayCasts;

		public LayerMask targetLayer;

		[Tooltip("Minimum distance to which it will be allowed to attack advancing.")]
		[Range(0f, 1f)]
		public float vanishingPointDistance;
	}
}
