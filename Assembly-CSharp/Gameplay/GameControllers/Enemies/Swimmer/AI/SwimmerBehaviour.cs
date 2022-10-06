using System;
using System.Collections;
using Framework.Managers;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Enemies.Swimmer.Animator;
using Gameplay.GameControllers.Enemies.Swimmer.Attack;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Swimmer.AI
{
	public class SwimmerBehaviour : EnemyBehaviour
	{
		[FoldoutGroup("Activation Settings", true, 0)]
		public float DistanceToTarget { get; private set; }

		public Swimmer Swimmer { get; private set; }

		public bool IsTriggerAttack { get; set; }

		public bool IsSwimming { get; set; }

		public bool IsJumping { get; set; }

		public bool IsVisible { get; private set; }

		public override void OnStart()
		{
			base.OnStart();
			this.Swimmer = (Swimmer)this.Entity;
			if (this.SwimmerTerrainEffects)
			{
				PoolManager.Instance.CreatePool(this.SwimmerTerrainEffects, 1);
			}
			this.Swimmer.Controller.MaxWalkingSpeed = Random.Range(this.ChasingSpeed.x, this.ChasingSpeed.y);
			this.SetVisible(true);
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			float vspeed = this.Swimmer.Controller.PlatformCharacterPhysics.VSpeed;
			if (vspeed > 1f)
			{
				this.EnableCollision = false;
			}
			if (this.DistanceToJumpPosition < 1f && vspeed < -0.1f)
			{
				this.EnableCollision = true;
			}
			if (this.IsTriggerAttack || this.Swimmer.Status.Dead)
			{
				this.StopMovement();
				return;
			}
			this.DistanceToTarget = Vector2.Distance(this.Swimmer.transform.position, base.GetTarget().position);
			base.IsChasing = (this.DistanceToTarget <= this.ActivationDistance && this.IsTargetAbove);
			if (base.IsChasing)
			{
				this.SetVisible(true);
				this.Chase(base.GetTarget());
			}
			else
			{
				this.SetVisible(false);
				this.StopMovement();
			}
			if (this.CanJump && this.IsTargetAbove)
			{
				this.Jump();
				this.IsTriggerAttack = true;
			}
		}

		public override void Chase(Transform targetPosition)
		{
			float horizontalInput = (base.GetTarget().position.x <= base.transform.position.x) ? -1f : 1f;
			if (!this.CanChase)
			{
				horizontalInput = 0f;
				this.StopMovement();
			}
			this.Swimmer.Input.HorizontalInput = horizontalInput;
			this.IsSwimming = true;
			this.IsJumping = false;
			this.LookAtTarget(targetPosition.position);
		}

		public bool CanChase
		{
			get
			{
				return this.Swimmer.MotionChecker.HitsFloor && !this.Swimmer.MotionChecker.HitsBlock;
			}
		}

		private void SetVisible(bool visible = true)
		{
			if (this.IsVisible && !visible)
			{
				this.Swimmer.AnimatorInjector.SpriteVisible(false, 0.5f, new Action(this.OnBecomeInvisible));
				this.IsVisible = false;
			}
			else if (!this.IsVisible && visible)
			{
				this.Swimmer.AnimatorInjector.SpriteVisible(true, 0.5f, null);
				this.IsVisible = true;
			}
		}

		private void OnBecomeInvisible()
		{
			if (!base.IsChasing)
			{
				this.Swimmer.transform.position = new Vector2(this.Swimmer.StartPoint.x, this.Swimmer.transform.position.y);
			}
		}

		private bool IsTargetAbove
		{
			get
			{
				return this.Swimmer.Target && this.Swimmer.transform.position.y <= this.Swimmer.Target.transform.position.y + 0.1f;
			}
		}

		public override void StopMovement()
		{
			this.Swimmer.Controller.PlatformCharacterPhysics.HSpeed = 0f;
			this.Swimmer.Input.HorizontalInput = 0f;
			this.IsSwimming = false;
		}

		public bool CanJump
		{
			get
			{
				return Mathf.Abs(base.GetTarget().position.x - base.transform.position.x) < this.AttackDistance;
			}
		}

		public bool EnableCollision
		{
			get
			{
				return this.Swimmer.Collider.EnableCollision2D;
			}
			set
			{
				this.Swimmer.Collider.EnableCollision2D = value;
			}
		}

		public void Jump()
		{
			SwimmerAttack swimmerAttack = (SwimmerAttack)this.Swimmer.EnemyAttack();
			swimmerAttack.JumpPosition = this.Swimmer.transform.position;
			this.IsJumping = true;
			this.JumpPosition = new Vector2(base.transform.position.x, base.transform.position.y);
			base.StartCoroutine(this.JumpPress());
		}

		private float DistanceToJumpPosition
		{
			get
			{
				return Vector2.Distance(base.transform.position, this.JumpPosition);
			}
		}

		private IEnumerator JumpPress()
		{
			yield return new WaitForSeconds(Random.Range(this.LapseBeforeJump.x, this.LapseBeforeJump.y));
			this.Swimmer.Input.Jump = true;
			yield return new WaitForSeconds(1f);
			this.Swimmer.Input.Jump = false;
			yield break;
		}

		public void RisingTerrainEffect(bool isRising, Vector2 position)
		{
			PoolManager.ObjectInstance objectInstance = PoolManager.Instance.ReuseObject(this.SwimmerTerrainEffects, position, Quaternion.identity, false, 1);
			if (objectInstance == null)
			{
				return;
			}
			SwimmerTerrainEffect component = objectInstance.GameObject.GetComponent<SwimmerTerrainEffect>();
			component.RisingEffect(isRising);
		}

		public override void Idle()
		{
			throw new NotImplementedException();
		}

		public override void Wander()
		{
			throw new NotImplementedException();
		}

		public override void Attack()
		{
			if (!this.Swimmer)
			{
				return;
			}
			SwimmerAttack swimmerAttack = (SwimmerAttack)this.Swimmer.EntityAttack;
			if (swimmerAttack.IsTargetTouched)
			{
				swimmerAttack.ContactAttack(Core.Logic.Penitent);
			}
		}

		public override void Damage()
		{
			throw new NotImplementedException();
		}

		[FoldoutGroup("Activation Settings", true, 0)]
		public float ActivationDistance;

		[FoldoutGroup("Activation Settings", true, 0)]
		public float AttackDistance = 0.5f;

		[MinMaxSlider(5f, 10f, false)]
		[FoldoutGroup("Attack Settings", true, 0)]
		public Vector2 ChasingSpeed;

		public GameObject SwimmerTerrainEffects;

		protected Vector2 JumpPosition;

		private const float TargetHeightOffset = 0.1f;

		[FoldoutGroup("Attack Settings", 0)]
		[MinMaxSlider(0f, 1f, false)]
		public Vector2 LapseBeforeJump;
	}
}
