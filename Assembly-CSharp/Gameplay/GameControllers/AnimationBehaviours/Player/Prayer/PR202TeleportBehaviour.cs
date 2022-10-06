using System;
using System.Collections.Generic;
using DG.Tweening;
using Framework.Managers;
using Gameplay.GameControllers.Penitent;
using Gameplay.UI;
using Tools.Items;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Player.Prayer
{
	public class PR202TeleportBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			this.doTeleport = !this.prohibitedScenes.Exists((string x) => Core.LevelManager.currentLevel.LevelName.StartsWith(x));
			this.doTeleport = (this.doTeleport && !Core.Input.HasBlocker("INTERACTABLE"));
			if (this._penitent == null)
			{
				this._penitent = Core.Logic.Penitent;
			}
			this._penitent.Status.Invulnerable = true;
			Core.Input.SetBlocker("PLAYER_LOGIC", true);
			Sequence sequence = DOTween.Sequence();
			TweenSettingsExtensions.AppendInterval(sequence, this.delay);
			TweenSettingsExtensions.OnComplete<Sequence>(sequence, new TweenCallback(this.SpawnVfx));
			TweenExtensions.Play<Sequence>(sequence);
		}

		private void SpawnVfx()
		{
			PoolManager.Instance.ReuseObject(this.teleportVFX, this._penitent.transform.position, Quaternion.identity, true, 1);
			Core.Logic.Penitent.Shadow.ManuallyControllingAlpha = true;
			Core.Logic.Penitent.Shadow.SetShadowAlpha(0f);
			if (!this.doTeleport)
			{
				this._penitent.Animator.speed = 2f;
			}
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			Core.Input.SetBlocker("PLAYER_LOGIC", false);
			Tween tween = DOTween.To(() => Core.Logic.Penitent.Shadow.GetShadowAlpha(), delegate(float x)
			{
				Core.Logic.Penitent.Shadow.SetShadowAlpha(x);
			}, 1f, 0.2f);
			TweenSettingsExtensions.OnComplete<Tween>(tween, delegate()
			{
				Core.Logic.Penitent.Shadow.ManuallyControllingAlpha = false;
			});
			if (this.doTeleport)
			{
				Core.Events.SetFlag("CHERUB_RESPAWN", true, false);
				Core.SpawnManager.Respawn();
				UIController.instance.HideBossHealth();
				UIController.instance.HideMiriamTimer();
				ChaliceEffect.ShouldUnfillChalice = true;
			}
			Core.Logic.Penitent.Status.Invulnerable = false;
			this._penitent.Animator.speed = 1f;
		}

		public GameObject teleportVFX;

		public float delay = 0.35f;

		private Penitent _penitent;

		private bool doTeleport;

		private List<string> prohibitedScenes = new List<string>
		{
			"D14",
			"D22",
			"D23",
			"D24",
			"D04BZ03S01",
			"D01BZ08S01",
			"D03Z01S06",
			"D07Z01S03"
		};
	}
}
