using System;
using Framework.Managers;
using Gameplay.GameControllers.AlliedCherub.AI;
using Gameplay.GameControllers.Effects.Player.Protection;
using Gameplay.GameControllers.Enemies.BellGhost;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.StateMachine;
using UnityEngine;

namespace Gameplay.GameControllers.AlliedCherub
{
	public class AlliedCherub : Entity
	{
		public Vector3 FlyingPosition { get; private set; }

		public StateMachine StateMachine { get; private set; }

		public AlliedCherubBehaviour Behaviour { get; private set; }

		public Entity Ally { get; private set; }

		public BellGhostFloatingMotion FloatingMotion { get; private set; }

		public AlliedCherubSystem CherubSystem { get; set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			this.StateMachine = base.GetComponent<StateMachine>();
			this.Behaviour = base.GetComponent<AlliedCherubBehaviour>();
			this.FloatingMotion = base.GetComponentInChildren<BellGhostFloatingMotion>();
		}

		protected override void OnStart()
		{
			base.OnStart();
			PoolManager.Instance.CreatePool(this.vfxOnExitPrefab, 2);
		}

		public void Deploy(AlliedCherubSystem.AlliedCherubSlot cherubSlot, AlliedCherubSystem cherubSystem)
		{
			this.FlyingOffset = new Vector2(cherubSlot.Offset.x, cherubSlot.Offset.y);
			this.FlyingPosition = Core.Logic.Penitent.transform.position + this.FlyingOffset;
			this.Ally = Core.Logic.Penitent;
			this.CherubSystem = cherubSystem;
			this.ShowEffect(this.FlyingPosition);
		}

		public void Store()
		{
			if (this.CherubSystem == null)
			{
				return;
			}
			this.ShowEffect(base.transform.position);
			this.CherubSystem.StoreCherub(base.gameObject);
		}

		public void ShowEffect(Vector2 pos)
		{
			if (this.vfxOnExitPrefab != null)
			{
				PoolManager.Instance.ReuseObject(this.vfxOnExitPrefab, pos, base.transform.rotation, false, 1);
			}
		}

		public Vector2 FlyingOffset;

		public GameObject vfxOnExitPrefab;
	}
}
