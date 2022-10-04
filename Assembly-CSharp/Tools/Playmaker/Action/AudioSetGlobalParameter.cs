using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Set a global audio parameter.")]
	public class AudioSetGlobalParameter : FsmStateAction
	{
		public override void Reset()
		{
			if (this.parameterName == null)
			{
				this.parameterName = new FsmString();
				this.parameterName.UseVariable = false;
				this.parameterName.Value = "DANGER";
			}
			if (this.value == null)
			{
				this.value = new FsmFloat(1f);
			}
		}

		public override void OnEnter()
		{
			string key = (this.parameterName == null) ? string.Empty : this.parameterName.Value;
			float num = (this.value == null) ? 0f : this.value.Value;
			Core.Audio.Ambient.SetSceneParam(key, num);
			base.Finish();
		}

		public FsmString parameterName;

		public FsmFloat value;
	}
}
