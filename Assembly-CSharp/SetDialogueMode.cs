using System;
using Framework.Managers;
using HutongGames.PlayMaker;
using UnityEngine;

public class SetDialogueMode : FsmStateAction
{
	public override void OnEnter()
	{
		base.OnEnter();
		if (this.PlaySheathedAnimDirectly.Value)
		{
			this.SetSheathedDirectly();
		}
		else
		{
			Core.Logic.Penitent.Animator.SetBool("IS_DIALOGUE_MODE", this.IsDialogueMode.Value);
		}
		base.Finish();
	}

	private void SetSheathedDirectly()
	{
		Core.Logic.Penitent.Animator.Play(this._sheathedAnim);
		Core.Logic.Penitent.Animator.SetBool("IS_DIALOGUE_MODE", true);
	}

	public FsmBool IsDialogueMode;

	public FsmBool PlaySheathedAnimDirectly;

	private readonly int _sheathedAnim = Animator.StringToHash("SheathedToIdle");
}
