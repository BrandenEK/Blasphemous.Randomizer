using System;
using System.Linq;
using DG.Tweening;
using Framework.FrameworkCore;
using Framework.Managers;
using Framework.Util;
using Gameplay.GameControllers.Bosses.BejeweledSaint.IA;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.Weapon;
using Gameplay.GameControllers.Penitent;
using Gameplay.GameControllers.Penitent.Damage;
using Gameplay.GameControllers.Penitent.Gizmos;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.BejeweledSaint.Attack
{
	public class BejeweledSaintArmAttack : EnemyAttack
	{
		public int AttacksMadeAmount { get; set; }

		public int SucceedAttacksAmount { get; set; }

		private void Awake()
		{
			base.CurrentEnemyWeapon = base.GetComponent<Weapon>();
			base.EntityOwner = this.Owner;
			this._attackArea = base.GetComponentInChildren<AttackArea>();
			PoolManager.Instance.CreatePool(this.fxStaffSlash, 3);
			PoolManager.Instance.CreatePool(this.fxStaffImpact, 3);
		}

		protected override void OnStart()
		{
			base.OnStart();
			BejeweledSaintStaff.OnSucceedHit = (Core.SimpleEvent)Delegate.Combine(BejeweledSaintStaff.OnSucceedHit, new Core.SimpleEvent(this.OnSucceedHit));
			PenitentDamageArea.OnDamagedGlobal = (PenitentDamageArea.PlayerDamagedEvent)Delegate.Combine(PenitentDamageArea.OnDamagedGlobal, new PenitentDamageArea.PlayerDamagedEvent(this.OnDamagedGlobal));
			this._attackArea.OnEnter += this.OnEnterAttackArea;
			this._weaponHit = new Hit
			{
				AttackingEntity = this.Owner.gameObject,
				DamageAmount = this.Owner.Stats.Strength.Final,
				DamageType = this.DamageType,
				Force = this.Force,
				Unnavoidable = true,
				HitSoundId = this.HitSound
			};
		}

		private void OnSucceedHit()
		{
			this.SucceedAttacksAmount++;
		}

		private void OnDamagedGlobal(Penitent damaged, Hit hit)
		{
			if (hit.DamageType == DamageArea.DamageType.Normal)
			{
				return;
			}
			BejeweledSaintBehaviour bejeweledSaintBehaviour = (BejeweledSaintBehaviour)this.Owner.EnemyBehaviour;
			Vector3 position = bejeweledSaintBehaviour.StaffRoot.transform.position;
			Core.Logic.Penitent.SetOrientationbyHit(position);
		}

		private void OnEnterAttackArea(object sender, Collider2DParam e)
		{
			BejeweledSaintBehaviour bejeweledSaintBehaviour = (BejeweledSaintBehaviour)this.Owner.EnemyBehaviour;
			if (bejeweledSaintBehaviour.IsPerformingAttack)
			{
				this.StaffBasicAttack();
			}
		}

		private void SetAttackOrientation()
		{
			BejeweledSaintBehaviour component = this.Owner.GetComponent<BejeweledSaintBehaviour>();
			float x = Core.Logic.Penitent.transform.position.x;
			float x2 = component.StaffRoot.transform.position.x;
			EntityOrientation orientation = (x < x2) ? EntityOrientation.Left : EntityOrientation.Right;
			this.Owner.SetOrientation(orientation, false, false);
		}

		public override void CurrentWeaponAttack()
		{
			base.CurrentWeaponAttack();
			this.SetAttackOrientation();
			base.CurrentEnemyWeapon.Attack(this._weaponHit);
			this.AttacksMadeAmount++;
		}

		public void StaffBasicAttack()
		{
			this.CurrentWeaponAttack();
			this.PlaySlashFX();
			this.PlayImpactFX();
		}

		public void PlaySlashFX()
		{
			GameObject gameObject = PoolManager.Instance.ReuseObject(this.fxStaffSlash, this.angleCastCenter.position + this.angleCastCenter.up * -4.5f, this.angleCastCenter.rotation, false, 1).GameObject;
		}

		public void PlayImpactFX()
		{
			GameObject gameObject = PoolManager.Instance.ReuseObject(this.fxStaffImpact, this.impactTransform.position, Quaternion.identity, false, 1).GameObject;
			if (this.screenshakeOnEnd)
			{
				Core.Logic.CameraManager.ProCamera2DShake.Shake(this.shakeDuration, -this.angleCastCenter.up * this.shakeForce, this.vibrato, 0.01f, 0f, default(Vector3), 0.06f, true);
			}
		}

		public void DefaultArmAngle(float s = 0.9f)
		{
			base.transform.DOLocalRotate(Vector3.zero, s, RotateMode.Fast).SetEase(Ease.InOutCubic);
		}

		public void SetArmAngle()
		{
			Vector3 dir = Core.Logic.Penitent.transform.position - this.angleCastCenter.position;
			Debug.DrawRay(this.angleCastCenter.position, dir, Color.red, 2f);
			float num = 57.29578f * Mathf.Atan2(dir.y, dir.x);
			num += 90f;
			Debug.Log("UNCLAMPED: " + num);
			num = Mathf.Clamp(num, -this.maxArmAngle, this.maxArmAngle);
			Debug.Log("ANGLE:" + num);
			base.transform.DOLocalRotate(new Vector3(0f, 0f, num), 0.4f, RotateMode.Fast).SetEase(Ease.InOutCubic);
		}

		public void QuickAttackMode(bool mode)
		{
			if (mode)
			{
				this.animator.speed = 2f;
			}
			else
			{
				this.animator.speed = 1f;
			}
		}

		public void SetCurrentFailedAttackLimit()
		{
			BejeweledSaintArmAttack.FailedAttackTier failedAttackTier = this.FailedAttackTiers.First<BejeweledSaintArmAttack.FailedAttackTier>();
			this._currentFailedAttacksLimit = UnityEngine.Random.Range(failedAttackTier.MinFailedAttackAmount, failedAttackTier.MaxFailedAttackAmount);
		}

		public bool CanFireSweepAttack
		{
			get
			{
				return this.AttacksMadeAmount - this.SucceedAttacksAmount >= this._currentFailedAttacksLimit;
			}
		}

		public void ResetAttackCounters()
		{
			this.AttacksMadeAmount = 0;
			this.SucceedAttacksAmount = 0;
		}

		public void PlayLiftUpArm()
		{
			BejeweledSaintHead bejeweledSaintHead = (BejeweledSaintHead)this.Owner;
			if (bejeweledSaintHead != null)
			{
				bejeweledSaintHead.WholeBoss.Audio.PlayLiftUpMace();
			}
		}

		public void PlayAttack()
		{
			BejeweledSaintHead bejeweledSaintHead = (BejeweledSaintHead)this.Owner;
			if (bejeweledSaintHead != null)
			{
				bejeweledSaintHead.WholeBoss.Audio.PlaySmashMace();
			}
		}

		private void OnDestroy()
		{
			PenitentDamageArea.OnDamagedGlobal = (PenitentDamageArea.PlayerDamagedEvent)Delegate.Remove(PenitentDamageArea.OnDamagedGlobal, new PenitentDamageArea.PlayerDamagedEvent(this.OnDamagedGlobal));
			BejeweledSaintStaff.OnSucceedHit = (Core.SimpleEvent)Delegate.Remove(BejeweledSaintStaff.OnSucceedHit, new Core.SimpleEvent(this.OnSucceedHit));
			this._attackArea.OnEnter -= this.OnEnterAttackArea;
		}

		public BejeweledSaintArmAttack.FailedAttackTier[] FailedAttackTiers;

		public RootMotionDriver StaffRoot;

		public Enemy Owner;

		public Transform angleCastCenter;

		public Transform impactTransform;

		[FoldoutGroup("FX", 0)]
		public GameObject fxStaffSlash;

		[FoldoutGroup("FX", 0)]
		public GameObject fxStaffImpact;

		[FoldoutGroup("FX", 0)]
		public bool screenshakeOnEnd;

		[ShowIf("screenshakeOnEnd", true)]
		[FoldoutGroup("FX", 0)]
		public int vibrato = 40;

		[ShowIf("screenshakeOnEnd", true)]
		[FoldoutGroup("FX", 0)]
		public float shakeForce;

		[ShowIf("screenshakeOnEnd", true)]
		[FoldoutGroup("FX", 0)]
		public float shakeDuration;

		public float maxArmAngle = 22f;

		private AttackArea _attackArea;

		private Hit _weaponHit;

		private int _currentFailedAttacksLimit;

		[Serializable]
		public struct FailedAttackTier
		{
			[Range(1f, 10f)]
			public int MinFailedAttackAmount;

			[Range(1f, 10f)]
			public int MaxFailedAttackAmount;
		}
	}
}
