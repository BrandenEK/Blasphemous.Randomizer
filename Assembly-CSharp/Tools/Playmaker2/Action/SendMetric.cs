using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Sends metric data to the cloud")]
	public class SendMetric : FsmStateAction
	{
		public override void OnEnter()
		{
			Core.Metrics.CustomEvent(this.metricId.Value, this.metricDefinition.Value, -1f);
			base.Finish();
		}

		public FsmString metricId;

		public FsmString metricDefinition;
	}
}
