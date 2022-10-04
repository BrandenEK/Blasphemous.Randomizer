using System;
using System.Collections.Generic;
using Framework.FrameworkCore;
using Framework.Managers;
using UnityEngine;

namespace Gameplay.GameControllers.Effects.Player.Dust
{
	public class WallClimbDust : Trait
	{
		public void TriggerDust()
		{
			if (base.EntityOwner == null)
			{
				return;
			}
			this.FlipSpriteRenderer();
			for (int i = 0; i < this.WallClimbDustAnimators.Length; i++)
			{
				this.WallClimbDustAnimators[i].Play(this._wallClimbDustAnim, 0, 0f);
			}
			this.InstantiateClimbDust();
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			this.EnableAnimators(!base.EntityOwner.Status.IsGrounded);
		}

		public void FlipSpriteRenderer()
		{
			if (base.EntityOwner == null)
			{
				return;
			}
			for (int i = 0; i < this.SpriteRenderers.Length; i++)
			{
				if (base.EntityOwner.Status.Orientation == EntityOrientation.Left && !this.SpriteRenderers[i].flipX)
				{
					this.SpriteRenderers[i].flipX = true;
				}
				else if (base.EntityOwner.Status.Orientation == EntityOrientation.Right && this.SpriteRenderers[i].flipX)
				{
					this.SpriteRenderers[i].flipX = false;
				}
			}
		}

		private void EnableAnimators(bool e = true)
		{
			if (this._enableAnimators == e)
			{
				return;
			}
			for (int i = 0; i < this.WallClimbDustAnimators.Length; i++)
			{
				this.WallClimbDustAnimators[i].enabled = e;
			}
			this._enableAnimators = e;
		}

		public void InstantiateClimbDust()
		{
			if (this.WallClimbDustPrefab == null)
			{
				return;
			}
			GameObject gameObject;
			if (this._climbDustList.Count > 0)
			{
				gameObject = this._climbDustList[this._climbDustList.Count - 1];
				this._climbDustList.Remove(gameObject);
				gameObject.SetActive(true);
				gameObject.transform.position = base.EntityOwner.transform.position;
			}
			else
			{
				gameObject = UnityEngine.Object.Instantiate<GameObject>(this.WallClimbDustPrefab, base.EntityOwner.transform.position, Quaternion.identity);
			}
			if (gameObject != null)
			{
				gameObject.GetComponent<SpriteRenderer>().flipX = (base.EntityOwner.Status.Orientation == EntityOrientation.Left);
			}
		}

		public void StoreClimbDust(GameObject cd)
		{
			if (this._climbDustList.Contains(cd))
			{
				return;
			}
			this._climbDustList.Add(cd);
			cd.SetActive(false);
			if (this.OnDustStore != null)
			{
				this.OnDustStore();
			}
		}

		public Animator[] WallClimbDustAnimators;

		public SpriteRenderer[] SpriteRenderers;

		public GameObject WallClimbDustPrefab;

		private readonly int _wallClimbDustAnim = Animator.StringToHash("WallClimbEffect");

		private bool _enableAnimators;

		public Core.SimpleEvent OnDustStore;

		private List<GameObject> _climbDustList = new List<GameObject>();
	}
}
