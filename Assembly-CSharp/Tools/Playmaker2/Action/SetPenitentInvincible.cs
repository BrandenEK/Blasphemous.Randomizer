using System;
using System.Collections;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Enables or disables the Invincibility of Penitent.")]
	public class SetPenitentInvincible : FsmStateAction
	{
		public override void OnEnter()
		{
			if (this.isInvincible != null && this.isInvincible.Value)
			{
				this.currentLevelName = Core.LevelManager.currentLevel.LevelName;
				base.StartCoroutine(this.InvencibilityCoroutine());
			}
			base.Finish();
		}

		private IEnumerator InvencibilityCoroutine()
		{
			while (this.currentLevelName.Equals(Core.LevelManager.currentLevel.LevelName))
			{
				yield return null;
				Core.Logic.Penitent.Stats.Life.SetToCurrentMax();
			}
			yield break;
		}

		public FsmBool isInvincible;

		private string currentLevelName;
	}
}
