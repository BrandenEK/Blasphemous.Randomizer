using System;
using System.Collections;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Penitent;
using UnityEngine;

namespace Gameplay.GameControllers.Effects.Player.Dash
{
	public class DashDustGenerator : Trait
	{
		protected override void OnStart()
		{
			base.OnStart();
			this._penitent = (Penitent)base.EntityOwner;
			if (this.startDashDustPrefab)
			{
				PoolManager.Instance.CreatePool(this.startDashDustPrefab, this.poolsize);
			}
			if (this.stopDashDustPrefab)
			{
				PoolManager.Instance.CreatePool(this.stopDashDustPrefab, this.poolsize);
			}
		}

		public GameObject GetStartDashDust()
		{
			return PoolManager.Instance.ReuseObject(this.startDashDustPrefab, this._penitent.transform.position, Quaternion.identity, false, 1).GameObject;
		}

		public void GetStopDashDust(float delay)
		{
			base.StopAllCoroutines();
			base.StartCoroutine(this.DelayStopDash(delay));
		}

		public void GetStopDashDust()
		{
			PoolManager.Instance.ReuseObject(this.stopDashDustPrefab, this.GetDashDustPosition(), Quaternion.identity, false, 1);
		}

		private IEnumerator DelayStopDash(float d)
		{
			yield return new WaitForSeconds(d);
			this.GetStopDashDust();
			yield break;
		}

		public Vector3 GetDashDustPosition()
		{
			if (this._penitent.DamageArea == null)
			{
				return Vector3.zero;
			}
			float y = this._penitent.DamageArea.DamageAreaCollider.bounds.min.y - this._penitent.PlatformCharacterController.GroundDist;
			float num = (this._penitent.Status.Orientation != EntityOrientation.Right) ? (this._penitent.DamageArea.DamageAreaCollider.bounds.min.x - 0.2f) : (this._penitent.DamageArea.DamageAreaCollider.bounds.max.x + 0.2f);
			this.StopDustSpawnOffsetPos *= ((this._penitent.Status.Orientation != EntityOrientation.Right) ? -1f : 1f);
			return new Vector2(num + this.StopDustSpawnOffsetPos, y);
		}

		private Penitent _penitent;

		public float StopDustSpawnOffsetPos;

		[SerializeField]
		private GameObject startDashDustPrefab;

		[SerializeField]
		private GameObject stopDashDustPrefab;

		[SerializeField]
		private int poolsize = 5;
	}
}
