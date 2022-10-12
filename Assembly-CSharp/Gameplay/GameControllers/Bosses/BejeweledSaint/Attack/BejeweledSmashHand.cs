using System;
using DG.Tweening;
using FMODUnity;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.Weapon;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.BejeweledSaint.Attack
{
	public class BejeweledSmashHand : Weapon
	{
		public bool IsRaised { get; set; }

		protected SpriteRenderer SpriteRender { get; set; }

		public AttackArea AttackArea { get; private set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			this.WeaponOwner = UnityEngine.Object.FindObjectOfType<BejeweledSaintHead>();
			this.Animator = base.GetComponent<Animator>();
			this.SpriteRender = base.GetComponent<SpriteRenderer>();
			this.AttackArea = base.GetComponentInChildren<AttackArea>();
			PoolManager.Instance.CreatePool(this.impactFx, 1);
		}

		protected override void OnStart()
		{
			base.OnStart();
			this.AttackArea.Entity = this.WeaponOwner;
			this._smashHandHit = new Hit
			{
				AttackingEntity = base.gameObject,
				DamageAmount = this.WeaponOwner.Stats.Strength.Final * 0.5f,
				DamageType = DamageArea.DamageType.Normal,
				Force = 0f,
				HitSoundId = this.HandSmashHitFx
			};
		}

		public void SmashAttack()
		{
			this.Attack(this._smashHandHit);
			PoolManager.Instance.ReuseObject(this.impactFx, this.impactTransform.position + Vector3.up * 2f, Quaternion.identity, false, 1);
		}

		public override void Attack(Hit weapondHit)
		{
			base.GetDamageableEntities();
			base.AttackDamageableEntities(weapondHit);
		}

		public override void OnHit(Hit weaponHit)
		{
		}

		public void AttackAppearing()
		{
			this.SetSmoothYPos(this.MaxHeight, 0.5f, new TweenCallback(this.AnimatorAttack));
		}

		public void Disappear()
		{
			if (DOTween.IsTweening(base.transform, false))
			{
				DOTween.Kill(base.transform, false);
			}
			this.SetSmoothYPos(this.MinHeight, 1f, new TweenCallback(this.OnDissapear));
		}

		private void SetSmoothYPos(float yPos, float time, TweenCallback endCallback)
		{
			base.transform.DOLocalMoveY(yPos, time, false).SetEase(this.AppearingMoveCurve).OnComplete(endCallback).SetId("VerticalMotion");
		}

		public void OnDissapear()
		{
			if (BejeweledSmashHand.OnHandDown != null)
			{
				BejeweledSmashHand.OnHandDown();
			}
		}

		public void AnimatorAttack()
		{
			this.Animator.SetTrigger("ATTACK");
		}

		public void PlaySmash()
		{
			BejeweledSaintHead bejeweledSaintHead = (BejeweledSaintHead)this.WeaponOwner;
			if (bejeweledSaintHead != null)
			{
				bejeweledSaintHead.WholeBoss.Audio.PlayHandStomp();
			}
		}

		public void DoCameraShake()
		{
			if (this.SpriteRender.isVisible)
			{
				Core.Logic.CameraManager.ProCamera2DShake.ShakeUsingPreset("PietyStomp");
			}
		}

		public static Core.SimpleEvent OnHandDown;

		public AnimationCurve AppearingMoveCurve;

		public float MinHeight;

		public float MaxHeight;

		public GameObject impactFx;

		[EventRef]
		public string HandSmashHitFx;

		public Transform impactTransform;

		protected Animator Animator;

		private Hit _smashHandHit;
	}
}
