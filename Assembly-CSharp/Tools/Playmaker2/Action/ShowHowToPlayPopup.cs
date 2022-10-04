using System;
using System.Collections;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Show the popup in screen and block player.")]
	public class ShowHowToPlayPopup : FsmStateAction
	{
		public override void Reset()
		{
			this.popupId = new FsmString();
			this.popupId.UseVariable = false;
			this.popupId.Value = string.Empty;
			this.blockPlayer = new FsmBool();
			this.blockPlayer.UseVariable = false;
			this.blockPlayer.Value = true;
		}

		public override void OnEnter()
		{
			string id = (this.popupId == null) ? string.Empty : this.popupId.Value;
			bool block = this.blockPlayer == null || this.blockPlayer.Value;
			base.StartCoroutine(this.ShowCourrutine(id, block));
		}

		private IEnumerator ShowCourrutine(string id, bool block)
		{
			yield return Core.TutorialManager.ShowTutorial(id, block);
			base.Finish();
			yield break;
		}

		public FsmString popupId;

		public FsmBool blockPlayer;
	}
}
