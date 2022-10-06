using System;
using DG.Tweening;
using Gameplay.GameControllers.Bosses.CommonAttacks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.BurntFace.AI
{
	public class BurntFaceHandBehaviour : MonoBehaviour
	{
		public void ClearAll()
		{
			ShortcutExtensions.DOKill(base.transform, false);
			this.SetMuzzleFlash(false);
			this.rosary.Clear();
			this.homingBallsLauncher.Clear();
			this.targetedBeamAttack.Clear();
			this.rosary.gameObject.SetActive(false);
			this.homingBallsLauncher.gameObject.SetActive(false);
			this.targetedBeamAttack.gameObject.SetActive(false);
		}

		private void Start()
		{
			this.rosary.RegenerateAllBeads();
		}

		public void SetMuzzleFlash(bool on)
		{
			this.muzzleFlashAnimator.SetBool("ACTIVE", on);
		}

		public void MoveToPosition(Vector2 pos, float seconds, Action<BurntFaceHandBehaviour> callback)
		{
			TweenSettingsExtensions.SetEase<Tweener>(TweenSettingsExtensions.OnComplete<Tweener>(ShortcutExtensions.DOMove(base.transform, pos, seconds, false), delegate()
			{
				callback(this);
			}), 28);
		}

		public void SetFlipX(bool flip)
		{
			this.spr.flipX = flip;
		}

		public void Show(float seconds)
		{
			ShortcutExtensions43.DOFade(this.spr, 1f, seconds);
		}

		public void Hide(float seconds)
		{
			this.SetMuzzleFlash(false);
			ShortcutExtensions43.DOFade(this.spr, 0f, seconds);
		}

		public SpriteRenderer spr;

		public Animator muzzleFlashAnimator;

		[FoldoutGroup("Attack references", 0)]
		public BurntFaceRosaryManager rosary;

		[FoldoutGroup("Attack references", 0)]
		public BossHomingLaserAttack targetedBeamAttack;

		[FoldoutGroup("Attack references", 0)]
		public BossStraightProjectileAttack homingBallsLauncher;
	}
}
