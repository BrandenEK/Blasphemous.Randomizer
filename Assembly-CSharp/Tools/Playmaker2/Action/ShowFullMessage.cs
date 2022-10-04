using System;
using System.Collections;
using Gameplay.UI;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Play fullmessage on screen.")]
	public class ShowFullMessage : FsmStateAction
	{
		public override void OnEnter()
		{
			base.StartCoroutine(this.ShowCourrutine());
		}

		private IEnumerator ShowCourrutine()
		{
			yield return UIController.instance.ShowFullMessageCourrutine(this.type, this.totalTime, this.fadeInTime, this.fadeOutTime);
			base.Finish();
			yield break;
		}

		public UIController.FullMensages type;

		public float totalTime = 2f;

		public float fadeInTime = 0.5f;

		public float fadeOutTime = 0.5f;
	}
}
