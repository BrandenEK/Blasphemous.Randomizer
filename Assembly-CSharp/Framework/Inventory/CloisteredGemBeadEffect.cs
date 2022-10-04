using System;
using System.Collections;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Penitent;
using Gameplay.GameControllers.Penitent.Animator;
using Gameplay.GameControllers.Penitent.Attack;
using UnityEngine;

namespace Framework.Inventory
{
	public class CloisteredGemBeadEffect : ObjectEffect
	{
		protected override void OnAwake()
		{
			base.OnAwake();
		}

		protected override bool OnApplyEffect()
		{
			this.penitent = Core.Logic.Penitent;
			if (!this.projectileAttack)
			{
				this.projectileAttack = this.penitent.GetComponentInChildren<CloisteredGemProjectileAttack>();
			}
			this.projectileAttack.enabled = true;
			AnimatorInyector animatorInyector = this.penitent.AnimatorInyector;
			animatorInyector.OnAttack = (Core.SimpleEvent)Delegate.Remove(animatorInyector.OnAttack, new Core.SimpleEvent(this.OnAttack));
			AnimatorInyector animatorInyector2 = this.penitent.AnimatorInyector;
			animatorInyector2.OnAttack = (Core.SimpleEvent)Delegate.Combine(animatorInyector2.OnAttack, new Core.SimpleEvent(this.OnAttack));
			this.currentUses = 0;
			return true;
		}

		private void SetProjectileDirection()
		{
			if (this.penitent.IsCrouched)
			{
				this.direction = Vector2.right * (float)((this.penitent.Status.Orientation != EntityOrientation.Right) ? -1 : 1) + Vector2.down * 0.33f;
				this.offset = ((this.penitent.Status.Orientation != EntityOrientation.Right) ? new Vector2(-2f, -0.33f) : new Vector2(2f, -0.33f));
				this.rotation = new Vector3(0f, 0f, (this.penitent.Status.Orientation != EntityOrientation.Right) ? 30f : -30f);
			}
			else if (this.penitent.PlatformCharacterInput.isJoystickUp)
			{
				this.direction = Vector2.up + Vector2.right * ((this.penitent.Status.Orientation != EntityOrientation.Right) ? -0.001f : 0.001f);
				this.offset = ((this.penitent.Status.Orientation != EntityOrientation.Right) ? new Vector2(-0.3f, 2.5f) : new Vector2(0.3f, 2.5f));
				this.rotation = new Vector3(0f, 180f, (this.penitent.Status.Orientation != EntityOrientation.Right) ? 90f : -90f);
			}
			else
			{
				this.direction = Vector2.right * (float)((this.penitent.Status.Orientation != EntityOrientation.Right) ? -1 : 1);
				this.offset = ((this.penitent.Status.Orientation != EntityOrientation.Right) ? new Vector2(-1.8f, 1.2f) : new Vector2(1.8f, 1.2f));
				this.rotation = new Vector3(0f, 0f, 0f);
			}
		}

		private void OnAttack()
		{
			this.SetProjectileDirection();
			base.StartCoroutine(this.WaitAndShoot(0.2f));
			if (this.currentUses >= this.MaxUses)
			{
				this.StopEffect();
			}
		}

		private IEnumerator WaitAndShoot(float waitTime)
		{
			yield return new WaitForSeconds(waitTime);
			if (!this.penitent.Status.Dead && this.projectileAttack)
			{
				this.currentUses++;
				this.projectileAttack.Shoot(this.direction, this.offset, this.rotation, (float)this.DamageAmount);
			}
			yield break;
		}

		protected override void OnRemoveEffect()
		{
			if (this.penitent)
			{
				AnimatorInyector animatorInyector = this.penitent.AnimatorInyector;
				animatorInyector.OnAttack = (Core.SimpleEvent)Delegate.Remove(animatorInyector.OnAttack, new Core.SimpleEvent(this.OnAttack));
			}
			this.projectileAttack = null;
			base.OnRemoveEffect();
		}

		private void StopEffect()
		{
			this.currentUses = 0;
			this.InvObj.RemoveAllObjectEffets();
		}

		public int DamageAmount;

		public int MaxUses = 3;

		private Penitent penitent;

		private CloisteredGemProjectileAttack projectileAttack;

		private Vector2 direction;

		private Vector2 offset;

		private Vector3 rotation;

		private int currentUses;
	}
}
