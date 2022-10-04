using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Marks a Boss Rush Course As Unlocked.")]
	public class MarkCourseAsUnlocked : FsmStateAction
	{
		public override void OnEnter()
		{
			string id = this.courseId.Value + "_UNLOCKED";
			Core.Events.SetFlag(id, true, true);
			base.Finish();
		}

		public FsmString courseId;
	}
}
