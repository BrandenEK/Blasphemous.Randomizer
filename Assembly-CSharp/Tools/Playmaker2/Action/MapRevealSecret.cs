using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Reveal a secret from a map.")]
	public class MapRevealSecret : FsmStateAction
	{
		public override void Reset()
		{
			if (this.mapId == null)
			{
				this.mapId = new FsmString();
			}
			this.mapId.Value = string.Empty;
			if (this.secretId == null)
			{
				this.secretId = new FsmString();
			}
			this.secretId.Value = string.Empty;
			if (this.enableSecret == null)
			{
				this.enableSecret = new FsmBool();
			}
			this.enableSecret.Value = true;
		}

		public override void OnEnter()
		{
			string value = (this.mapId == null) ? string.Empty : this.mapId.Value;
			string value2 = (this.secretId == null) ? string.Empty : this.secretId.Value;
			bool flag = this.enableSecret == null || this.enableSecret.Value;
			bool flag2 = this.useMapIdInsteadOfCurrentMap != null && this.useMapIdInsteadOfCurrentMap.Value;
			if (string.IsNullOrEmpty(value))
			{
				base.LogWarning("PlayMaker Action MapRevealSecret - mapId is blank");
			}
			else if (string.IsNullOrEmpty(value2))
			{
				base.LogWarning("PlayMaker Action MapRevealSecret - secretId is blank");
			}
			else
			{
				if (flag2)
				{
					Core.NewMapManager.SetSecret(value, value2, flag);
				}
				else
				{
					Core.NewMapManager.SetSecret(value2, flag);
				}
				base.Finish();
			}
		}

		[RequiredField]
		public FsmString mapId;

		[RequiredField]
		public FsmString secretId;

		public FsmBool enableSecret;

		public FsmBool useMapIdInsteadOfCurrentMap;
	}
}
