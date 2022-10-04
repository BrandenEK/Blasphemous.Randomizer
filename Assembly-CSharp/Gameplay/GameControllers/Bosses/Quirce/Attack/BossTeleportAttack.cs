using System;
using System.Collections;
using System.Diagnostics;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.Quirce.Attack
{
	public class BossTeleportAttack : EnemyAttack
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action OnTeleportInEvent;

		public override void CurrentWeaponAttack()
		{
			base.CurrentWeaponAttack();
		}

		protected override void OnStart()
		{
			base.OnStart();
			this._waitForSeconds = new WaitForSeconds(this.teleportTime);
		}

		public void Use(Transform parentToMove, Transform targetTransform, Vector3 offset)
		{
			this._parentToMove = parentToMove;
			this.OnTeleportOut();
			base.StartCoroutine(this.TeleportCoroutine(parentToMove, targetTransform, offset, new Action(this.OnTeleportIn)));
		}

		private IEnumerator TeleportCoroutine(Transform parentToMove, Transform target, Vector3 offset, Action callback = null)
		{
			yield return this._waitForSeconds;
			parentToMove.position = target.position + offset;
			if (callback != null)
			{
				callback();
			}
			yield break;
		}

		private void OnTeleportOut()
		{
			UnityEngine.Debug.Log("TELEPORT OUT");
			if (this.instantiateOnTeleportOut != null)
			{
				this.InstantiateArea(this.instantiateOnTeleportOut);
			}
		}

		private void OnTeleportIn()
		{
			UnityEngine.Debug.Log("TELEPORT IN");
			if (this.instantiateOnTeleportIn != null)
			{
				this.InstantiateArea(this.instantiateOnTeleportIn);
			}
			if (this.OnTeleportInEvent != null)
			{
				this.OnTeleportInEvent();
			}
		}

		private void InstantiateArea(GameObject toInstantiate)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(toInstantiate, this._parentToMove.position, Quaternion.identity);
			BossSpawnedAreaAttack component = gameObject.GetComponent<BossSpawnedAreaAttack>();
			if (component != null)
			{
				component.SetOwner(base.EntityOwner);
			}
		}

		public float teleportTime;

		public GameObject instantiateOnTeleportOut;

		public GameObject instantiateOnTeleportIn;

		private Transform _parentToMove;

		private Vector3 _targetPoint;

		private Coroutine _currentCoroutine;

		private WaitForSeconds _waitForSeconds;
	}
}
