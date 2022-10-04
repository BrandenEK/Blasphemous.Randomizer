using System;
using Framework.Util;
using Gameplay.GameControllers.Entities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Environment.Traps.FireTrap
{
	[RequireComponent(typeof(AttackArea))]
	public class BeamAttack : MonoBehaviour
	{
		private void OnEnable()
		{
			if (this.BeamLauncher)
			{
				this.SetColliderWidth(this.BeamLauncher.maxRange);
			}
		}

		private void Awake()
		{
			this._attackArea = base.GetComponent<AttackArea>();
			this._attackArea.OnStay += this.AttackAreaOnStay;
			if (!this.BeamLauncher)
			{
				Debug.LogError("The Beam launcher member is not set!");
			}
		}

		private void Update()
		{
			if (this.isAttackWindowOpen && this.UseAttackCooldown && this.currentAttackCd > 0f)
			{
				this.currentAttackCd -= Time.deltaTime;
			}
		}

		private void SetColliderWidth(float width)
		{
			BoxCollider2D boxCollider2D = (BoxCollider2D)this._attackArea.WeaponCollider;
			boxCollider2D.size = new Vector2(width, boxCollider2D.size.y);
		}

		private void AttackAreaOnStay(object sender, Collider2DParam e)
		{
			IDamageable componentInParent = e.Collider2DArg.GetComponentInParent<IDamageable>();
			this.Attack(componentInParent);
		}

		private void Attack(IDamageable damageable)
		{
			if (!this.isAttackWindowOpen || damageable == null)
			{
				return;
			}
			if (this.UseAttackCooldown)
			{
				if (this.currentAttackCd <= 0f)
				{
					this.currentAttackCd = this.TimeBetweenTicks;
					damageable.Damage(this.lightningHit);
				}
			}
			else
			{
				damageable.Damage(this.lightningHit);
			}
		}

		public void OpenAttackWindow()
		{
			this.isAttackWindowOpen = true;
		}

		public void CloseAttackWindow()
		{
			this.isAttackWindowOpen = false;
		}

		private void OnDestroy()
		{
			if (this._attackArea)
			{
				this._attackArea.OnEnter -= this.AttackAreaOnStay;
			}
		}

		private AttackArea _attackArea;

		public TileableBeamLauncher BeamLauncher;

		[SerializeField]
		public Hit lightningHit;

		public bool UseAttackCooldown;

		[ShowIf("UseAttackCooldown", true)]
		public float TimeBetweenTicks = 0.3f;

		private bool isAttackWindowOpen;

		private float currentAttackCd;
	}
}
