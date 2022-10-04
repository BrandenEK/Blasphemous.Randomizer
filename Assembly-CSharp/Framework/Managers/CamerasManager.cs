using System;
using Com.LuisPedroFonseca.ProCamera2D;
using Gameplay.GameControllers.Camera;
using UnityEngine;

namespace Framework.Managers
{
	public class CamerasManager : GameSystem
	{
		public override void OnGUI()
		{
			base.DebugResetLine();
			base.DebugDrawTextLine("CameraManager -------------------------------------", 10, 1500);
			ProCamera2DNumericBoundaries proCamera2DNumericBoundaries = Core.Logic.CameraManager.ProCamera2DNumericBoundaries;
			base.DebugDrawTextLine("--Camera  SIZE:" + Core.Logic.CameraManager.ProCamera2D.GameCamera.orthographicSize.ToString(), 10, 1500);
			base.DebugDrawTextLine("--Boundaries use " + proCamera2DNumericBoundaries.UseNumericBoundaries.ToString(), 10, 1500);
			this.ShowBounday("Top", proCamera2DNumericBoundaries.UseTopBoundary, proCamera2DNumericBoundaries.TopBoundary);
			this.ShowBounday("Bottom", proCamera2DNumericBoundaries.UseBottomBoundary, proCamera2DNumericBoundaries.BottomBoundary);
			this.ShowBounday("Left", proCamera2DNumericBoundaries.UseLeftBoundary, proCamera2DNumericBoundaries.LeftBoundary);
			this.ShowBounday("Right", proCamera2DNumericBoundaries.UseRightBoundary, proCamera2DNumericBoundaries.RightBoundary);
			CameraPlayerOffset cameraPlayerOffset = Core.Logic.CameraManager.CameraPlayerOffset;
			string text = "NULL";
			if (cameraPlayerOffset.PlayerTarget != null)
			{
				if (cameraPlayerOffset.PlayerTarget.TargetTransform != null)
				{
					text = cameraPlayerOffset.PlayerTarget.TargetTransform.name + " ";
				}
				text = text + "Offset X:" + cameraPlayerOffset.PlayerTarget.TargetOffset.x.ToString();
				text = text + " Y:" + cameraPlayerOffset.PlayerTarget.TargetOffset.y.ToString();
			}
			base.DebugDrawTextLine("--Target " + text, 10, 1500);
			text = "Default X:" + cameraPlayerOffset.DefaultTargetOffset.x.ToString();
			text = text + " Y:" + cameraPlayerOffset.DefaultTargetOffset.y.ToString();
			base.DebugDrawTextLine("--Offset " + text, 10, 1500);
			Vector2 overallOffset = Core.Logic.CameraManager.ProCamera2D.OverallOffset;
			base.DebugDrawTextLine("--Overall Offset X:" + overallOffset.x.ToString() + "  Y:" + overallOffset.y.ToString(), 10, 1500);
		}

		private void ShowBounday(string name, bool use, float value)
		{
			base.DebugDrawTextLine(string.Concat(new string[]
			{
				"Bound ",
				name,
				": ",
				use.ToString(),
				" (",
				value.ToString(),
				")"
			}), 10, 1500);
		}
	}
}
