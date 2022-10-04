using System;
using Com.LuisPedroFonseca.ProCamera2D;
using Framework.Managers;
using HutongGames.PlayMaker;
using Sirenix.OdinInspector;

namespace Tools.PlayMaker.Action
{
	public class CameraElasticBoundaries : FsmStateAction
	{
		public override void OnEnter()
		{
			base.OnEnter();
			this.SetCameraElasticBoundariesValues();
			base.Finish();
		}

		private void SetCameraElasticBoundariesValues()
		{
			ProCamera2DNumericBoundaries proCamera2DNumericBoundaries = Core.Logic.CameraManager.ProCamera2DNumericBoundaries;
			if (!proCamera2DNumericBoundaries)
			{
				return;
			}
			if (this.UseHorizontalBoundary.Value)
			{
				proCamera2DNumericBoundaries.HorizontalElasticityDuration = this.HorizontalDurationValue.Value;
			}
			if (this.UseVerticalBoundary.Value)
			{
				proCamera2DNumericBoundaries.VerticalElasticityDuration = this.VerticalDurationValue.Value;
			}
		}

		public FsmBool UseVerticalBoundary;

		[ShowIf("UseVerticalBoundary", true)]
		public FsmFloat VerticalDurationValue;

		public FsmBool UseHorizontalBoundary;

		[ShowIf("UseHorizontalBoundary", true)]
		public FsmFloat HorizontalDurationValue;
	}
}
