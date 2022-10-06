using System;
using CreativeSpore.SmartColliders;
using Framework.Managers;
using Gameplay.GameControllers.Effects.Entity;
using Gameplay.GameControllers.Enemies.ChainedAngel;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Enemies.Framework.Damage;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Enemies.Framework.Physics;
using Gameplay.GameControllers.Enemies.Processioner.AI;
using Gameplay.GameControllers.Enemies.Processioner.Animator;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Penitent;
using Gameplay.GameControllers.Penitent.Gizmos;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Processioner
{
	public class Processioner : Enemy
	{
		public EnemyDamageArea DamageArea { get; private set; }

		public VisionCone VisionCone { get; private set; }

		public ProcessionerBehaviour Behaviour { get; set; }

		public NPCInputs Input { get; private set; }

		public ProcessionerAnimator ProcessionerAnimator { get; private set; }

		public EntityMotionChecker MotionChecker { get; private set; }

		public ColorFlash ColorFlash { get; private set; }

		public ContactDamage ContactDamage { get; private set; }

		public ChainedAngel ChainedAngel { get; private set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			base.Controller = base.GetComponentInChildren<PlatformCharacterController>();
			this.DamageArea = base.GetComponentInChildren<EnemyDamageArea>();
			this.VisionCone = base.GetComponentInChildren<VisionCone>();
			this.Behaviour = base.GetComponentInChildren<ProcessionerBehaviour>();
			this.Input = base.GetComponentInChildren<NPCInputs>();
			this.ProcessionerAnimator = base.GetComponentInChildren<ProcessionerAnimator>();
			this.MotionChecker = base.GetComponentInChildren<EntityMotionChecker>();
			this.ColorFlash = base.GetComponentInChildren<ColorFlash>();
			this.ContactDamage = base.GetComponentInChildren<ContactDamage>();
			SpawnManager.OnPlayerSpawn += this.OnPlayerSpawn;
		}

		private void OnPlayerSpawn(Penitent penitent)
		{
			SpawnManager.OnPlayerSpawn -= this.OnPlayerSpawn;
			base.Target = penitent.gameObject;
		}

		protected override void OnStart()
		{
			base.OnStart();
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			if (!base.Landing)
			{
				base.Landing = true;
				this.SetPositionAtStart();
				if (this.ChainedAngelComposition)
				{
					this.AddChainedAngel();
				}
			}
			if (this.ChainedAngelComposition)
			{
				this.UpdateChainedAngelPosition();
			}
		}

		public override void SetPositionAtStart()
		{
			base.SetPositionAtStart();
			float groundDist = base.Controller.GroundDist;
			Vector3 position;
			position..ctor(base.transform.position.x, base.transform.position.y - groundDist, base.transform.position.z);
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
		}

		private void AddChainedAngel()
		{
			if (!this.ChainedAngelPrefab)
			{
				return;
			}
			this._rootMotion = base.GetComponentInChildren<RootMotionDriver>();
			GameObject gameObject = Object.Instantiate<GameObject>(this.ChainedAngelPrefab, base.transform.position, Quaternion.identity);
			this.ChainedAngel = gameObject.GetComponentInChildren<ChainedAngel>();
			if (!this.ChainedAngel)
			{
				return;
			}
			this.ChainedAngel.Target = base.Target;
			this.ChainedAngel.Behaviour.enabled = true;
			this._angelLink = this.ChainedAngel.GetLowerLink();
			this._angelLink.transform.position = ((!base.SpriteRenderer.flipX) ? this._rootMotion.transform.position : this._rootMotion.ReversePosition);
			this._angelLink.transform.parent.transform.parent = base.transform;
		}

		private void UpdateChainedAngelPosition()
		{
			if (!this._rootMotion || !this._angelLink)
			{
				return;
			}
			this._angelLink.transform.position = ((!base.SpriteRenderer.flipX) ? this._rootMotion.transform.position : this._rootMotion.ReversePosition);
		}

		private GameObject _angelLink;

		private RootMotionDriver _rootMotion;

		public bool ChainedAngelComposition;

		[ShowIf("ChainedAngelComposition", true)]
		public GameObject ChainedAngelPrefab;
	}
}
