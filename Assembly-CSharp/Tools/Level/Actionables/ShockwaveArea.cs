using System;
using FMODUnity;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.Weapon;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Tools.Level.Actionables
{
	public class ShockwaveArea : Weapon
	{
		public AttackArea AttackArea { get; set; }

		public override void OnObjectReuse()
		{
			base.OnObjectReuse();
			this.damageArea.transform.localScale = Vector3.one;
			this.counter = 0f;
			this._animator = base.GetComponentInChildren<Animator>();
			this.Activate();
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			this.counter += Time.deltaTime;
			if (this.counter >= this.duration)
			{
				base.Destroy();
			}
		}

		private void ShockwaveEffect()
		{
			Core.Logic.CameraManager.ShockwaveManager.Shockwave(base.transform.position, this.duration, 0.2f, 0.7f);
		}

		protected virtual void Activate()
		{
			this.PlayAreaSound();
			this._animator.SetTrigger(ShockwaveArea.ActivateParam);
		}

		public void PlayAreaSound()
		{
			if (string.IsNullOrEmpty(this.shockwaveSound))
			{
				return;
			}
			Core.Audio.PlayOneShot(this.shockwaveSound, base.transform.position);
		}

		public override void Attack(Hit weapondHit)
		{
			base.GetDamageableEntities();
			base.AttackDamageableEntities(weapondHit);
		}

		public override void OnHit(Hit weaponHit)
		{
			Debug.Log("ON AREA HIT");
		}

		public void SetOwner(Entity owner)
		{
			this.WeaponOwner = owner;
			if (this.AttackArea == null)
			{
				this.AttackArea = base.GetComponentInChildren<AttackArea>();
			}
			this.AttackArea.Entity = owner;
		}

		public GameObject damageArea;

		public float radius;

		public float duration = 1.2f;

		private float counter;

		[SerializeField]
		[BoxGroup("Audio", true, false, 0)]
		[EventRef]
		protected string shockwaveSound;

		public float maxAudioDistance = 20f;

		public float minAudioDistance = 7f;

		private Animator _animator;

		private static readonly int ActivateParam = Animator.StringToHash("ACTIVATE");
	}
}
