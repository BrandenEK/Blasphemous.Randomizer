using System;
using System.Collections;
using Com.LuisPedroFonseca.ProCamera2D;
using Framework.Managers;
using Gameplay.UI.Widgets;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Camera
{
	public class CameraNumericBoundaries : MonoBehaviour
	{
		private void OnDestroy()
		{
			base.StopCoroutine("EnableElastic");
		}

		private void SetBoundariesAndMoveToPlayer()
		{
			this.SetBoundaries();
			ProCamera2DNumericBoundaries component = Camera.main.GetComponent<ProCamera2DNumericBoundaries>();
			component.VerticalElasticityDuration = 0.01f;
			component.HorizontalElasticityDuration = 0.01f;
			component.UseElasticBoundaries = false;
			Core.Logic.CameraManager.ProCamera2D.MoveCameraInstantlyToPosition(Core.Logic.Penitent.transform.position);
		}

		public void SetBoundariesOnLevelLoad()
		{
			this.SetBoundariesAndMoveToPlayer();
			base.StartCoroutine(this.EnableElastic());
		}

		public void SetBoundariesForcingPosition()
		{
			this.SetBoundariesAndMoveToPlayer();
			base.StartCoroutine(this.EnableElasticCancelling());
		}

		public void SetBoundaries()
		{
			ProCamera2DNumericBoundaries component = Camera.main.GetComponent<ProCamera2DNumericBoundaries>();
			component.UseNumericBoundaries = this.UseCameraBoundaries;
			component.UseTopBoundary = (this.UseCameraBoundaries && this.UseTopBoundary);
			component.TopBoundary = this.TopBoundary;
			component.UseBottomBoundary = (this.UseCameraBoundaries && this.UseBottomBoundary);
			component.BottomBoundary = this.BottomBoundary;
			component.UseLeftBoundary = (this.UseCameraBoundaries && this.UseLeftBoundary);
			component.LeftBoundary = this.LeftBoundary;
			component.UseRightBoundary = (this.UseCameraBoundaries && this.UseRightBoundary);
			component.RightBoundary = this.RightBoundary;
		}

		[Button(ButtonSizes.Small)]
		public void SavePosition()
		{
			this.originPos = base.transform.position;
		}

		[Button(ButtonSizes.Small)]
		public void CenterHere()
		{
			this.TopBoundary = base.transform.position.y + 5.625f;
			this.BottomBoundary = base.transform.position.y - 5.625f;
			this.LeftBoundary = base.transform.position.x - 10f;
			this.RightBoundary = base.transform.position.x + 10f;
		}

		[Button(ButtonSizes.Small)]
		public void CenterKeepSize()
		{
			this.TopBoundary -= this.originPos.y;
			this.BottomBoundary -= this.originPos.y;
			this.LeftBoundary -= this.originPos.x;
			this.RightBoundary -= this.originPos.x;
			this.TopBoundary += base.transform.position.y;
			this.BottomBoundary += base.transform.position.y;
			this.LeftBoundary += base.transform.position.x;
			this.RightBoundary += base.transform.position.x;
			this.originPos = base.transform.position;
		}

		private IEnumerator EnableElastic()
		{
			yield return new WaitForSeconds(1f);
			if (FadeWidget.instance.IsActive)
			{
				yield return new WaitForSeconds(2f);
			}
			ProCamera2DNumericBoundaries boundaries = Camera.main.GetComponent<ProCamera2DNumericBoundaries>();
			boundaries.UseElasticBoundaries = true;
			this.ResetElasticitySeconds();
			yield break;
		}

		private IEnumerator EnableElasticCancelling()
		{
			yield return new WaitForSeconds(1f);
			ProCamera2DNumericBoundaries boundaries = Camera.main.GetComponent<ProCamera2DNumericBoundaries>();
			boundaries.UseElasticBoundaries = true;
			boundaries.VerticalElasticityDuration = 0f;
			boundaries.HorizontalElasticityDuration = 0f;
			yield break;
		}

		public void ResetElasticitySeconds()
		{
			ProCamera2DNumericBoundaries component = Camera.main.GetComponent<ProCamera2DNumericBoundaries>();
			component.VerticalElasticityDuration = 2f;
			component.HorizontalElasticityDuration = 2f;
		}

		private void OnDrawGizmosSelected()
		{
			if (!this.UseCameraBoundaries)
			{
				return;
			}
			Gizmos.color = this.gizmoColor;
			Gizmos.DrawWireSphere(this.originPos, 0.25f);
			if (this.UseTopBoundary)
			{
				Gizmos.DrawLine(new Vector2(this.RayLengh, this.TopBoundary), new Vector2(-this.RayLengh, this.TopBoundary));
			}
			if (this.UseBottomBoundary)
			{
				Gizmos.DrawLine(new Vector2(this.RayLengh, this.BottomBoundary), new Vector2(-this.RayLengh, this.BottomBoundary));
			}
			if (this.UseLeftBoundary)
			{
				Gizmos.DrawLine(new Vector2(this.LeftBoundary, this.RayLengh), new Vector2(this.LeftBoundary, -this.RayLengh));
			}
			if (this.UseRightBoundary)
			{
				Gizmos.DrawLine(new Vector2(this.RightBoundary, this.RayLengh), new Vector2(this.RightBoundary, -this.RayLengh));
			}
		}

		private void OnDrawGizmos()
		{
			if (!this.UseCameraBoundaries)
			{
				return;
			}
			if (!this.drawGizmosUnselected)
			{
				return;
			}
			Gizmos.color = this.gizmoColor;
			Gizmos.DrawWireSphere(this.originPos, 0.25f);
			if (this.UseTopBoundary)
			{
				Gizmos.DrawLine(new Vector2(this.RayLengh, this.TopBoundary), new Vector2(-this.RayLengh, this.TopBoundary));
			}
			if (this.UseBottomBoundary)
			{
				Gizmos.DrawLine(new Vector2(this.RayLengh, this.BottomBoundary), new Vector2(-this.RayLengh, this.BottomBoundary));
			}
			if (this.UseLeftBoundary)
			{
				Gizmos.DrawLine(new Vector2(this.LeftBoundary, this.RayLengh), new Vector2(this.LeftBoundary, -this.RayLengh));
			}
			if (this.UseRightBoundary)
			{
				Gizmos.DrawLine(new Vector2(this.RightBoundary, this.RayLengh), new Vector2(this.RightBoundary, -this.RayLengh));
			}
		}

		public bool notSetOnLevelLoad;

		public bool UseCameraBoundaries;

		public float RayLengh = 50f;

		public Vector2 originPos;

		[BoxGroup("Boundaries Values", true, false, 0)]
		public bool UseTopBoundary;

		[ShowIf("UseTopBoundary", true)]
		[BoxGroup("Boundaries Values", true, false, 0)]
		public float TopBoundary = 10f;

		[BoxGroup("Boundaries Values", true, false, 0)]
		public bool UseBottomBoundary = true;

		[ShowIf("UseBottomBoundary", true)]
		[BoxGroup("Boundaries Values", true, false, 0)]
		public float BottomBoundary = -10f;

		[BoxGroup("Boundaries Values", true, false, 0)]
		public bool UseLeftBoundary;

		[ShowIf("UseLeftBoundary", true)]
		[BoxGroup("Boundaries Values", true, false, 0)]
		public float LeftBoundary = -10f;

		[BoxGroup("Boundaries Values", true, false, 0)]
		public bool UseRightBoundary;

		[ShowIf("UseRightBoundary", true)]
		[BoxGroup("Boundaries Values", true, false, 0)]
		public float RightBoundary = 10f;

		[BoxGroup("Debug", true, false, 0)]
		public Color gizmoColor = Color.white;

		[BoxGroup("Debug", true, false, 0)]
		public bool drawGizmosUnselected;
	}
}
