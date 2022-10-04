using System;
using Framework.Managers;
using Gameplay.UI;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Unlocks a Skin.")]
	public class UnlockSkin : FsmStateAction
	{
		public override void OnEnter()
		{
			Core.ColorPaletteManager.UnlockColorPalette(this.skinId.Value, true);
		}

		public override void OnUpdate()
		{
			if (!UIController.instance.IsUnlockActive())
			{
				base.Finish();
			}
		}

		[RequiredField]
		public FsmString skinId;
	}
}
