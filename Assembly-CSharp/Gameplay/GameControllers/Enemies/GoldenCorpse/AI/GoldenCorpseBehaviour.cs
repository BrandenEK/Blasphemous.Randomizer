using System;
using Framework.FrameworkCore;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.GoldenCorpse.AI
{
	public class GoldenCorpseBehaviour : EnemyBehaviour
	{
		public GoldenCorpse GoldenCorpse { get; set; }

		public override void OnStart()
		{
			base.OnStart();
			this.GoldenCorpse = (GoldenCorpse)this.Entity;
			this._bottomHits = new RaycastHit2D[2];
			int num = Random.Range(0, this.totalAnimationVariants);
			this.GoldenCorpse.Animator.SetInteger("ID", num);
			this.GoldenCorpse.Animator.Play("sleep" + num.ToString());
			if (Random.Range(0f, 1f) < 0.5f)
			{
				this.LookAtTarget(base.transform.position - Vector3.right);
			}
			if (this.startAwaken)
			{
				base.Invoke("Awaken", 1f);
			}
		}

		public bool CanWalk()
		{
			return !this._isSpawning;
		}

		public bool IsAwaken()
		{
			return this.isAwake;
		}

		public void Awaken()
		{
			this.UnFreezeAnimation();
			this.isAwake = true;
			this.GoldenCorpse.AnimatorInyector.PlayAwaken();
		}

		public void SleepForever()
		{
			this.StopMovement();
			this.UnFreezeAnimation();
			this.isAwake = false;
			this.GoldenCorpse.DamageArea.DamageAreaCollider.enabled = false;
			this.GoldenCorpse.DamageByContact = false;
			this.GoldenCorpse.AnimatorInyector.PlaySleep();
			base.BehaviourTree.StopBehaviour();
		}

		public void ReAwaken()
		{
			this.UnFreezeAnimation();
			this.GoldenCorpse.AnimatorInyector.PlayAwaken();
		}

		public void OnAwakeAnimationFinished()
		{
			if (!this.isAwake)
			{
				return;
			}
			this.GoldenCorpse.DamageArea.DamageAreaCollider.enabled = true;
			this.GoldenCorpse.DamageByContact = true;
			base.BehaviourTree.StartBehaviour();
		}

		private void Sleep()
		{
			this.StopMovement();
			this.GoldenCorpse.DamageArea.DamageAreaCollider.enabled = false;
			this.GoldenCorpse.DamageByContact = false;
			this.UnFreezeAnimation();
			this.isNapping = true;
			this.GoldenCorpse.AnimatorInyector.PlaySleep();
			this.sleepTime = Random.Range(this.minSleepTime, this.maxSleepTime);
			base.BehaviourTree.StopBehaviour();
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			this.GoldenCorpse.Status.CastShadow = (!this.isNapping && this.isAwake);
			if (this.isAwake && this.isNapping)
			{
				this.sleepTime -= Time.deltaTime;
				if (this.sleepTime < 0f)
				{
					this.isNapping = false;
					this.ReAwaken();
				}
			}
		}

		public override void Idle()
		{
			this.FreezeAnimation();
			this.StopMovement();
		}

		public override void Wander()
		{
		}

		private void FreezeAnimation()
		{
			if (this.GoldenCorpse.Animator.speed > 0.1f)
			{
				this.origAnimationSpeed = this.GoldenCorpse.Animator.speed;
				this.GoldenCorpse.Animator.speed = 0.01f;
			}
		}

		private void UnFreezeAnimation()
		{
			if (this.GoldenCorpse.Animator.speed < 0.1f)
			{
				this.GoldenCorpse.Animator.speed = this.origAnimationSpeed;
			}
		}

		public void Chase(Vector3 position)
		{
			this.UnFreezeAnimation();
			this.LookAtTarget(position);
			if (!this.GoldenCorpse.MotionChecker.HitsFloor || this.GoldenCorpse.MotionChecker.HitsBlock || this.GoldenCorpse.Status.Dead)
			{
				this.StopMovement();
				return;
			}
			float horizontalInput = (this.Entity.Status.Orientation != EntityOrientation.Right) ? -1f : 1f;
			this.GoldenCorpse.Input.HorizontalInput = horizontalInput;
			this.GoldenCorpse.AnimatorInyector.Walk();
		}

		public override void Chase(Transform targetPosition)
		{
		}

		public override void Attack()
		{
		}

		public override void Damage()
		{
			this.GoldenCorpse.Audio.PlayHit();
			this.Sleep();
		}

		public void Death()
		{
			this.StopMovement();
			this.GoldenCorpse.AnimatorInyector.Death();
		}

		public bool TargetCanBeVisible()
		{
			base.GetTarget();
			if (this.GoldenCorpse.Target == null)
			{
				return false;
			}
			float num = this.GoldenCorpse.Target.transform.position.y - this.GoldenCorpse.transform.position.y;
			float num2 = Mathf.Abs(this.GoldenCorpse.Target.transform.position.x - this.GoldenCorpse.transform.position.x);
			num = ((num <= 0f) ? (-num) : num);
			return num <= this.MaxVisibleHeight && num2 < this.ActivationDistance;
		}

		public override void StopMovement()
		{
			this.GoldenCorpse.Input.HorizontalInput = 0f;
			this.GoldenCorpse.Controller.PlatformCharacterPhysics.HSpeed = 0f;
			this.GoldenCorpse.AnimatorInyector.StopWalk();
		}

		public override void LookAtTarget(Vector3 targetPos)
		{
			if (targetPos.x > this.GoldenCorpse.transform.position.x)
			{
				if (this.GoldenCorpse.Status.Orientation == EntityOrientation.Right)
				{
					return;
				}
				this.GoldenCorpse.SetOrientation(EntityOrientation.Right, true, false);
			}
			else
			{
				if (this.GoldenCorpse.Status.Orientation == EntityOrientation.Left)
				{
					return;
				}
				this.GoldenCorpse.SetOrientation(EntityOrientation.Left, true, false);
			}
		}

		public bool isAwake;

		public bool isNapping;

		[FoldoutGroup("Activation Settings", true, 0)]
		public float ActivationDistance;

		public float DistanceToTarget;

		public float MaxVisibleHeight = 2f;

		[FoldoutGroup("Activation Settings", true, 0)]
		public bool startAwaken = true;

		[FoldoutGroup("Activation Settings", true, 0)]
		public float MaxTimeAwaitingBeforeGoBack;

		[FoldoutGroup("Motion Settings", true, 0)]
		public LayerMask GroundLayerMask;

		[SerializeField]
		[FoldoutGroup("Motion Settings", true, 0)]
		private RaycastHit2D[] _bottomHits;

		public float _myWidth;

		public float _myHeight;

		[SerializeField]
		[FoldoutGroup("Sleep Settings", true, 0)]
		private float minSleepTime = 8f;

		[SerializeField]
		[FoldoutGroup("Sleep Settings", true, 0)]
		private float maxSleepTime = 16f;

		public float sleepTime;

		public float origAnimationSpeed = 1f;

		private int totalAnimationVariants = 2;

		private bool _isSpawning;
	}
}
