using System;
using FMOD.Studio;
using FMODUnity;
using Framework.Managers;
using Framework.Util;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.Weapon;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Nun.Attack
{
	public class OilPuddle : Weapon
	{
		private Animator Animator { get; set; }

		protected override void OnStart()
		{
			base.OnStart();
			this.Animator = base.GetComponentInChildren<Animator>();
			this._attackArea.OnEnter += this.AttackAreaOnEnter;
			this._oilPuddleHit = new Hit
			{
				AttackingEntity = base.gameObject,
				DamageAmount = this.Damage,
				DamageType = DamageArea.DamageType.Normal,
				Force = 0f,
				HitSoundId = this.HitSoundId,
				Unnavoidable = true
			};
		}

		private void AttackAreaOnEnter(object sender, Collider2DParam e)
		{
			this.Attack(this._oilPuddleHit);
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			this._currentBoilingTime += Time.deltaTime;
			if (this._currentBoilingTime < this.BoilingTime || this._vanish)
			{
				return;
			}
			this._vanish = true;
			this.Animator.SetTrigger("VANISH");
			Core.Audio.PlaySfx(this.VanishSoundId, 0f);
			this.StopBubbles();
		}

		public override void Attack(Hit weapondHit)
		{
			base.GetDamageableEntities();
			base.AttackDamageableEntities(weapondHit);
		}

		public override void OnHit(Hit weaponHit)
		{
		}

		public void SetOwner(Entity owner)
		{
			if (this.WeaponOwner == null)
			{
				this.WeaponOwner = owner;
			}
			if (this._attackArea == null)
			{
				this._attackArea = base.GetComponentInChildren<AttackArea>();
			}
			this._attackArea.Entity = this.WeaponOwner;
		}

		public override void OnObjectReuse()
		{
			base.OnObjectReuse();
			this._vanish = false;
			this._currentBoilingTime = 0f;
			this.PlayBubbles();
		}

		public void Dissappear()
		{
			base.Destroy();
		}

		public void PlayBubbles()
		{
			if (this._oilPuddleBubble.isValid())
			{
				return;
			}
			this._oilPuddleBubble = Core.Audio.CreateEvent(this.AppearSoundId, default(Vector3));
			this._oilPuddleBubble.start();
		}

		public void StopBubbles()
		{
			if (!this._oilPuddleBubble.isValid())
			{
				return;
			}
			this._oilPuddleBubble.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
			this._oilPuddleBubble.release();
			this._oilPuddleBubble = default(EventInstance);
		}

		[FoldoutGroup("Attack Settings", true, 0)]
		public float BoilingTime = 5f;

		[FoldoutGroup("Attack Settings", true, 0)]
		public float Damage = 5f;

		[FoldoutGroup("Audio", true, 0)]
		[EventRef]
		public string HitSoundId;

		[FoldoutGroup("Audio", true, 0)]
		[EventRef]
		public string AppearSoundId;

		[FoldoutGroup("Audio", true, 0)]
		[EventRef]
		public string VanishSoundId;

		private float _currentBoilingTime;

		private bool _vanish;

		private AttackArea _attackArea;

		private Hit _oilPuddleHit;

		private EventInstance _oilPuddleBubble;
	}
}
