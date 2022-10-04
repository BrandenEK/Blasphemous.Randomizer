using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Change the current map.")]
	public class MapChange : FsmStateAction
	{
		public override void Reset()
		{
			if (this.mapId == null)
			{
				this.mapId = new FsmString();
			}
			this.mapId.Value = string.Empty;
		}

		public override void OnEnter()
		{
			string text = (this.mapId == null) ? string.Empty : this.mapId.Value;
			if (string.IsNullOrEmpty(text))
			{
				base.LogWarning("PlayMaker Action MapChange - mapId is blank");
			}
			else
			{
				Core.NewMapManager.SetCurrentMap(text);
				base.Finish();
			}
		}

		[RequiredField]
		public FsmString mapId;
	}
}
