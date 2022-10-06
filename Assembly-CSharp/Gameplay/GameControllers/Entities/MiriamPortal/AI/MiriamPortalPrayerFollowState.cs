using System;
using Framework.Managers;
using Gameplay.GameControllers.Entities.StateMachine;
using UnityEngine;

namespace Gameplay.GameControllers.Entities.MiriamPortal.AI
{
	public class MiriamPortalPrayerFollowState : State
	{
		public override void OnStateInitialize(StateMachine machine)
		{
			base.OnStateInitialize(machine);
			this._behaviour = this.Machine.GetComponent<MiriamPortalPrayerBehaviour>();
		}

		public override void OnStateEnter()
		{
			base.OnStateEnter();
			this.originalY = this._behaviour.transform.position.y;
			this._behaviour.MiriamPortal.Audio.PlayFollow();
		}

		public override void OnStateExit()
		{
			base.OnStateExit();
			this._behaviour.MiriamPortal.Audio.StopFollow();
		}

		public override void Update()
		{
			base.Update();
			this.Float();
		}

		public override void LateUpdate()
		{
			base.LateUpdate();
			this.FollowMaster();
			this._behaviour.LookAtMaster();
		}

		private void FollowMaster()
		{
			if (this._behaviour.Master == null)
			{
				return;
			}
			float num = Mathf.Lerp(this._behaviour.FollowSpeed.x, this._behaviour.FollowSpeed.y, this.GetFollowSpeedFactor);
			float num2 = num * Time.deltaTime;
			this.Machine.transform.position = Vector3.SmoothDamp(this.Machine.transform.position, this._behaviour.GetMasterOffSetPosition, ref this._refVelocity, this._behaviour.SmoothDampElongation, num2);
		}

		private void Float()
		{
			if (Core.Logic.IsPaused)
			{
				return;
			}
			Vector3 position = this._behaviour.transform.position;
			float x = position.x;
			float num = position.y + (float)Math.Sin((double)(Time.time * this._behaviour.FloatingSpeed)) * this._behaviour.FloatingVerticalElongation;
			this._behaviour.transform.position = new Vector2(x, num);
		}

		private float GetFollowSpeedFactor
		{
			get
			{
				float num = (this._behaviour.GetMasterDistance - this._behaviour.FollowDistance.x) / (this._behaviour.FollowDistance.y - this._behaviour.FollowDistance.x);
				return Mathf.Clamp01(num);
			}
		}

		private MiriamPortalPrayerBehaviour _behaviour;

		private Vector3 _refVelocity;

		private float originalY;
	}
}
