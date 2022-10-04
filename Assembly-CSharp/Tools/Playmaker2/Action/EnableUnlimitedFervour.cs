using System;
using System.Collections;
using Framework.FrameworkCore;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Disable Penitent Blood Penance.")]
	public class EnableUnlimitedFervour : FsmStateAction
	{
		public override void OnEnter()
		{
			this.keepRefilling = true;
			base.StartCoroutine(this.RefillFervour());
			LevelManager.OnLevelLoaded -= this.OnLevelLoaded;
			LevelManager.OnLevelLoaded += this.OnLevelLoaded;
			base.Finish();
		}

		private IEnumerator RefillFervour()
		{
			while (this.keepRefilling)
			{
				Core.Logic.Penitent.Stats.Fervour.SetToCurrentMax();
				yield return null;
			}
			yield break;
		}

		private void OnLevelLoaded(Level oldLevel, Level newLevel)
		{
			LevelManager.OnLevelLoaded -= this.OnLevelLoaded;
			this.keepRefilling = false;
		}

		private bool keepRefilling = true;
	}
}
