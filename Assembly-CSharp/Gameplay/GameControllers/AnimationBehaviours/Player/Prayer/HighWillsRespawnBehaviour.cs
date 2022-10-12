using System;
using DG.Tweening;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Penitent;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Player.Prayer
{
	public class HighWillsRespawnBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this._penitent == null)
			{
				this._penitent = Core.Logic.Penitent;
			}
			Core.Input.SetBlocker("PLAYER_LOGIC", true);
			Core.Logic.Penitent.SetOrientation(EntityOrientation.Right, true, false);
			PoolManager.Instance.ReuseObject(this.crisantaVFX, this._penitent.transform.position, Quaternion.identity, true, 1);
			Core.Logic.Penitent.Shadow.ManuallyControllingAlpha = true;
			Core.Logic.Penitent.Shadow.SetShadowAlpha(0f);
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			Core.Input.SetBlocker("PLAYER_LOGIC", false);
			Tween t = DOTween.To(() => Core.Logic.Penitent.Shadow.GetShadowAlpha(), delegate(float x)
			{
				Core.Logic.Penitent.Shadow.SetShadowAlpha(x);
			}, 1f, 0.2f);
			t.OnComplete(delegate
			{
				Core.Logic.Penitent.Shadow.ManuallyControllingAlpha = false;
			});
		}

		public GameObject crisantaVFX;

		private Penitent _penitent;
	}
}
