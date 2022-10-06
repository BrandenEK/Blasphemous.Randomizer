using System;
using Framework.Managers;
using UnityEngine;

namespace Tools.Level.Layout
{
	public class PopPenitentFromGeo : MonoBehaviour
	{
		private void Awake()
		{
			this.boxCollider = base.GetComponent<BoxCollider2D>();
		}

		private void Start()
		{
			this.checkToPopUpPenitent = this.ShouldCheckToPopUpPenitent();
			this.raycastHits = new RaycastHit2D[1];
		}

		private void OnEnable()
		{
			this.currentSecondsOnScene = 0f;
		}

		private bool ShouldCheckToPopUpPenitent()
		{
			return this.boxCollider && (LayerMask.LayerToName(base.gameObject.layer).Equals("Floor") || LayerMask.LayerToName(base.gameObject.layer).Equals("Wall"));
		}

		private void FixedUpdate()
		{
			if (!Core.ready || !Core.Logic.Penitent || !this.checkToPopUpPenitent)
			{
				return;
			}
			if (this.currentSecondsOnScene < 5f)
			{
				this.currentSecondsOnScene += Time.deltaTime;
				return;
			}
			if (!this.boxCollider.OverlapPoint(Core.Logic.Penitent.GetPosition() + Vector3.up * 0.5f))
			{
				return;
			}
			if (this.boxCollider.size.x >= this.boxCollider.size.y)
			{
				Vector2 popUpPoint = this.GetPopUpPoint();
				if (this.RaycastToCheckForGeo(popUpPoint, Vector2.up))
				{
					this.PopPenitentSideways();
				}
				else
				{
					Core.Logic.Penitent.Teleport(popUpPoint);
				}
			}
			else
			{
				Vector2 popUpSidewaysPoint = this.GetPopUpSidewaysPoint();
				Vector2 vector = Core.Logic.Penitent.GetPosition();
				Vector2 dir = popUpSidewaysPoint - vector;
				if (this.RaycastToCheckForGeo(popUpSidewaysPoint, dir))
				{
					this.PopPenitentUp();
				}
				else
				{
					Core.Logic.Penitent.Teleport(popUpSidewaysPoint);
				}
			}
		}

		private Vector2 GetPopUpPoint()
		{
			float num = this.boxCollider.transform.position.y + this.boxCollider.offset.y;
			float num2 = num + this.boxCollider.size.y / 2f + 0.05f;
			return new Vector2(Core.Logic.Penitent.transform.position.x, num2);
		}

		private Vector2 GetPopUpSidewaysPoint()
		{
			float num = this.boxCollider.transform.position.x + this.boxCollider.offset.x;
			float num2 = num;
			if (Core.Logic.Penitent.transform.position.x < num)
			{
				num2 -= this.boxCollider.size.x / 2f + 0.5f;
			}
			else
			{
				num2 += this.boxCollider.size.x / 2f + 0.5f;
			}
			return new Vector2(num2, Core.Logic.Penitent.transform.position.y);
		}

		private bool RaycastToCheckForGeo(Vector2 point, Vector2 dir)
		{
			LayerMask layerMask = LayerMask.GetMask(new string[]
			{
				"Wall",
				"Floor"
			});
			if (Physics2D.RaycastNonAlloc(point, dir, this.raycastHits, 0.1f, layerMask) > 0)
			{
				Debug.DrawLine(point, this.raycastHits[0].point, Color.cyan, 5f);
				return true;
			}
			return false;
		}

		private void PopPenitentUp()
		{
			Vector2 popUpPoint = this.GetPopUpPoint();
			Core.Logic.Penitent.Teleport(popUpPoint);
		}

		private void PopPenitentSideways()
		{
			Vector2 popUpSidewaysPoint = this.GetPopUpSidewaysPoint();
			Core.Logic.Penitent.Teleport(popUpSidewaysPoint);
		}

		private const float MIN_SECONDS_BEFORE_POPUP = 5f;

		private const float V_THRESHOLD_FOR_CHECKING = 0.5f;

		private const float Y_OFFSET_TO_POP_UP = 0.05f;

		private const float X_OFFSET_TO_POP_SIDEWAYS = 0.5f;

		private BoxCollider2D boxCollider;

		private bool checkToPopUpPenitent;

		private float currentSecondsOnScene;

		private RaycastHit2D[] raycastHits;
	}
}
