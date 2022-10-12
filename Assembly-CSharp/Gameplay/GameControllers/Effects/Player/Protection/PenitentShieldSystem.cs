using System;
using System.Collections.Generic;
using DG.Tweening;
using Framework.Managers;
using Framework.Pooling;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Penitent;
using UnityEngine;

namespace Gameplay.GameControllers.Effects.Player.Protection
{
	public class PenitentShieldSystem : PoolObject
	{
		public CircularMovingObjects CircularMovingObjects { get; private set; }

		private void Awake()
		{
			this.CircularMovingObjects = base.GetComponent<CircularMovingObjects>();
		}

		public override void OnObjectReuse()
		{
			base.OnObjectReuse();
			this.DeployShields(this.deployTime);
		}

		private void LateUpdate()
		{
			this.FollowPlayer();
		}

		private void DeployShields(float time)
		{
			this.CircularMovingObjects.Frequency = this.Frequency;
			this.CircularMovingObjects.Radious = 0f;
			DOTween.To(delegate(float x)
			{
				this.CircularMovingObjects.Radious = x;
			}, this.CircularMovingObjects.Radious, this.Radious, time).SetEase(Ease.Linear);
		}

		private void FollowPlayer()
		{
			Penitent penitent = Core.Logic.Penitent;
			if (!penitent)
			{
				return;
			}
			Vector3 b = penitent.GetPosition() + this.OffSetPosition;
			base.transform.position = Vector3.Slerp(base.transform.position, b, Time.deltaTime * this.FollowSpeed);
		}

		public void SetShieldsOwner(Entity owner)
		{
			List<Transform> objectList = this.CircularMovingObjects.ObjectList;
			for (int i = 0; i < objectList.Count; i++)
			{
				PenitentShield component = objectList[i].GetComponent<PenitentShield>();
				component.WeaponOwner = owner;
				component.AttackArea.Entity = owner;
			}
		}

		public void DisposeShield(float time)
		{
			if (!this.CircularMovingObjects)
			{
				this.CircularMovingObjects = base.GetComponent<CircularMovingObjects>();
			}
			DOTween.To(delegate(float x)
			{
				this.CircularMovingObjects.Radious = x;
			}, this.CircularMovingObjects.Radious, 0f, time).OnComplete(new TweenCallback(base.Destroy));
		}

		public Vector2 OffSetPosition;

		public float Radious = 2f;

		[Range(0.1f, 5f)]
		public float Frequency = 2f;

		public float FollowSpeed = 3f;

		public float deployTime = 5f;
	}
}
