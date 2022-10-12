using System;
using CreativeSpore.SmartColliders;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Effects.Entity;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Enemies.Framework.Damage;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Enemies.Framework.Physics;
using Gameplay.GameControllers.Enemies.Jumper.AI;
using Gameplay.GameControllers.Enemies.Jumper.Animator;
using Gameplay.GameControllers.Enemies.Jumper.Attack;
using Gameplay.GameControllers.Enemies.Jumper.Audio;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Penitent.Attack;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Jumper
{
	public class Jumper : Enemy, IDamageable
	{
		public NPCInputs Inputs { get; private set; }

		public ColorFlash Flash { get; private set; }

		public SmartPlatformCollider Collider { get; private set; }

		public EnemyDamageArea DamageArea { get; set; }

		public JumperAnimator AnimatorInjector { get; private set; }

		public JumperAttack Attack { get; private set; }

		public JumperAudio Audio { get; private set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			this.Inputs = base.GetComponentInChildren<NPCInputs>();
			this.Flash = base.GetComponentInChildren<ColorFlash>();
			base.Controller = base.GetComponentInChildren<PlatformCharacterController>();
			this.Collider = base.GetComponentInChildren<SmartPlatformCollider>();
			base.EnemyBehaviour = base.GetComponentInChildren<JumperBehaviour>();
			this.DamageArea = base.GetComponentInChildren<EnemyDamageArea>();
			this.AnimatorInjector = base.GetComponentInChildren<JumperAnimator>();
			this.Attack = base.GetComponentInChildren<JumperAttack>();
			this.Audio = base.GetComponentInChildren<JumperAudio>();
		}

		protected override void OnStart()
		{
			base.OnStart();
			base.Target = Core.Logic.Penitent.gameObject;
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			if (base.Landing && base.Controller.PlatformCharacterPhysics.GravityScale <= 0f)
			{
				base.Controller.PlatformCharacterPhysics.GravityScale = 3f;
			}
			if (!base.Landing)
			{
				base.Landing = true;
				this.SetPositionAtStart();
			}
			if (base.IsImpaled && !this.Status.Dead)
			{
				this.AnimatorInjector.Death();
			}
		}

		public void Damage(Hit hit)
		{
			if (!this.Status.Dead)
			{
				this.SleepTimeByHit(hit);
				this.DamageArea.TakeDamage(hit, false);
				this.Flash.TriggerColorFlash();
			}
		}

		public Vector3 GetPosition()
		{
			return base.transform.position;
		}

		public void GetSparks(Hit hit)
		{
			if (this.DamageArea == null)
			{
				return;
			}
			PenitentSword penitentSword = (PenitentSword)Core.Logic.Penitent.PenitentAttack.CurrentPenitentWeapon;
			if (penitentSword == null)
			{
				return;
			}
			Bounds bounds = this.DamageArea.DamageAreaCollider.bounds;
			Vector2 position = new Vector2
			{
				x = ((this.Status.Orientation != EntityOrientation.Left) ? bounds.max.x : bounds.min.x),
				y = bounds.max.y
			};
			penitentSword.GetSwordSparks(position);
		}

		public override void SetPositionAtStart()
		{
			base.SetPositionAtStart();
			float groundDist = base.Controller.GroundDist;
			Vector3 position = new Vector3(base.transform.position.x, base.transform.position.y - groundDist, base.transform.position.z);
			base.transform.position = position;
		}

		public override EnemyFloorChecker EnemyFloorChecker()
		{
			throw new NotImplementedException();
		}

		public override EnemyAttack EnemyAttack()
		{
			throw new NotImplementedException();
		}

		public override EnemyBumper EnemyBumper()
		{
			throw new NotImplementedException();
		}

		protected override void EnablePhysics(bool enable = true)
		{
			throw new NotImplementedException();
		}
	}
}
