using System;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Enemies.Stoners.Rock;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Stoners.Attack
{
	public class StonersAttack : EnemyAttack
	{
		public Vector3 CurrentTargetPos { get; set; }

		public Vector2 BullsEyePos { get; set; }

		protected override void OnStart()
		{
			base.OnStart();
			this._stoners = base.GetComponentInParent<Stoners>();
			this.VisualSensor.OnEntityEnter += this.VisualSensorOnEntityEnter;
		}

		private void Update()
		{
			if (this.EntityTarget == null || this._stoners == null)
			{
				return;
			}
			if (this._stoners.Status.IsVisibleOnCamera)
			{
				this.CurrentTargetPos = new Vector2(this.EntityTarget.transform.position.x, this.EntityTarget.transform.position.y);
				if (this.GetDistanceToTarget(this.CurrentTargetPos) < this.CriticalDistance)
				{
					this._stoners.StonerBehaviour.CurrentAtackWaitTime = 0f;
				}
			}
		}

		public void SetBullsEyeWhenThrow()
		{
			if (this.EntityTarget == null)
			{
				return;
			}
			this.BullsEyePos = new Vector2(this.CurrentTargetPos.x, this.CurrentTargetPos.y);
		}

		public void ThrowRock()
		{
			try
			{
				Vector3 position = this._stoners.RockSpawnPoint.transform.position;
				GameObject rock = this._stoners.RockPool.GetRock(position);
				StonersRock componentInChildren = rock.GetComponentInChildren<StonersRock>();
				componentInChildren.SetOwner(this._stoners);
				float distanceToTarget = this.GetDistanceToTarget(this.BullsEyePos);
				if (distanceToTarget > this.NearDistanceRange)
				{
					this.SetProjectileMotion(rock, this.BullsEyePos);
				}
				else
				{
					this.SetStraightMotion(rock, this.BullsEyePos);
				}
			}
			catch (Exception ex)
			{
				Debug.LogError(ex.Message + ex.StackTrace);
			}
		}

		private void SetProjectileMotion(GameObject rock, Vector3 target)
		{
			Vector2 vector;
			vector..ctor(Random.Range(target.x - this.RndThrowHorOffset, target.x + this.RndThrowHorOffset), target.y);
			float num = vector.x - rock.transform.position.x;
			float num2 = vector.y - rock.transform.position.y;
			float num3 = Mathf.Atan((num2 + 7f) / num);
			float num4 = num / Mathf.Cos(num3);
			float num5 = num4 * Mathf.Cos(num3);
			float num6 = num4 * Mathf.Sin(num3);
			Rigidbody2D component = rock.GetComponent<Rigidbody2D>();
			component.velocity = new Vector2(num5, num6);
		}

		private void SetStraightMotion(GameObject rock, Vector3 target)
		{
			Vector3 vector;
			vector..ctor(Random.Range(target.x - this.RndThrowHorOffset, target.x + this.RndThrowHorOffset), target.y + this.TargetHeightOffset);
			Vector3 normalized = (vector - rock.transform.position).normalized;
			Rigidbody2D component = rock.GetComponent<Rigidbody2D>();
			component.AddForce(normalized * this.StraightImpulseForce, 1);
		}

		private void VisualSensorOnEntityEnter(Entity entity)
		{
			this.EntityTarget = entity;
		}

		private void OnDestroy()
		{
			this.VisualSensor.OnEntityEnter -= this.VisualSensorOnEntityEnter;
		}

		private float GetDistanceToTarget(Vector3 target)
		{
			float result = 0f;
			if (this.EntityTarget != null)
			{
				result = Vector3.Distance(this._stoners.transform.position, target);
			}
			return result;
		}

		private Stoners _stoners;

		public float CriticalDistance = 2f;

		public Entity EntityTarget;

		public float NearDistanceRange = 4f;

		public float RndThrowHorOffset = 1f;

		public float StraightImpulseForce = 30f;

		public float TargetHeightOffset = 0.5f;

		public CollisionSensor VisualSensor;
	}
}
