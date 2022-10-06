using System;
using System.Collections;
using Gameplay.UI;
using HutongGames.PlayMaker;
using UnityEngine;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Show a Unlock skills UI")]
	public class UnlockSkillsMenu : FsmStateAction
	{
		public override void OnEnter()
		{
			this.routine = base.StartCoroutine(this.ShowMenu());
		}

		private IEnumerator ShowMenu()
		{
			yield return UIController.instance.ShowUnlockSKillCourrutine();
			base.Finish();
			yield break;
		}

		public override void OnExit()
		{
			base.StopCoroutine(this.routine);
		}

		private Coroutine routine;
	}
}
