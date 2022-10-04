using System;
using Gameplay.GameControllers.Entities.Animations;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.DrownedCorpse.Animator
{
	public class DrownedCorpseAnimatorInjector : EnemyAnimatorInyector
	{
		protected override void OnStart()
		{
			base.OnStart();
			this._drownedCorpse = (DrownedCorpse)this.OwnerEntity;
			this.AnimEvent_DisableDamageArea();
		}

		public void ShowHelmet()
		{
			if (!this._drownedCorpse)
			{
				return;
			}
			this._drownedCorpse.Helmet.gameObject.SetActive(true);
			this._drownedCorpse.Helmet.transform.position = this.OwnerEntity.transform.position;
		}

		public void HideHelmet()
		{
			if (!this._drownedCorpse)
			{
				return;
			}
			this._drownedCorpse.Helmet.gameObject.SetActive(false);
		}

		public void Awake()
		{
			if (!this._drownedCorpse)
			{
				return;
			}
			this._drownedCorpse.Animator.SetBool(DrownedCorpseAnimatorInjector.Vanish, false);
		}

		public void Run()
		{
			if (!this._drownedCorpse)
			{
				return;
			}
			this._drownedCorpse.Animator.SetBool(DrownedCorpseAnimatorInjector.RunParam, true);
		}

		public void DontRun()
		{
			if (!this._drownedCorpse)
			{
				return;
			}
			this._drownedCorpse.Animator.SetBool(DrownedCorpseAnimatorInjector.RunParam, false);
		}

		public void VanishAfterRun()
		{
			if (!this._drownedCorpse)
			{
				return;
			}
			this._drownedCorpse.Animator.SetBool(DrownedCorpseAnimatorInjector.Vanish, true);
		}

		public void VanishAfterDamage()
		{
			if (!this._drownedCorpse)
			{
				return;
			}
			this._drownedCorpse.Animator.SetTrigger(DrownedCorpseAnimatorInjector.Damage);
			this._drownedCorpse.Animator.SetBool(DrownedCorpseAnimatorInjector.Vanish, true);
		}

		public void AnimEvent_EnableDamageArea()
		{
			if (!this._drownedCorpse)
			{
				return;
			}
			this._drownedCorpse.DamageArea.DamageAreaCollider.enabled = true;
		}

		public void AnimEvent_DisableDamageArea()
		{
			if (!this._drownedCorpse)
			{
				return;
			}
			this._drownedCorpse.DamageArea.DamageAreaCollider.enabled = false;
		}

		public const float VanishAfterRunAnimDuration = 0.75f;

		private DrownedCorpse _drownedCorpse;

		private static readonly int RunParam = Animator.StringToHash("RUN");

		private static readonly int Vanish = Animator.StringToHash("VANISH");

		private static readonly int Damage = Animator.StringToHash("DAMAGE");
	}
}
