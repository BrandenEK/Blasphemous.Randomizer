using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Bosses.HighWills.Attack;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Enemies.PontiffHusk.Animator;
using Gameplay.GameControllers.Enemies.PontiffHusk.Attack;
using Gameplay.GameControllers.Entities;
using NodeCanvas.BehaviourTrees;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.PontiffHusk.AI
{
	public class PontiffHuskRangedBehaviour : EnemyBehaviour
	{
		public bool IsAwake { get; set; }

		public bool IsAppear { get; set; }

		public bool IsDisappearing { get; set; }

		public bool Asleep { get; private set; }

		public bool IsRamming
		{
			get
			{
				return false;
			}
		}

		public PontiffHuskAnimatorInyector AnimatorInyector { get; private set; }

		[Button(0)]
		public void AddAnotherSegmentToShootingSequence()
		{
			PontiffHuskRangedBehaviour.ShootAndMove item = new PontiffHuskRangedBehaviour.ShootAndMove
			{
				Movement = Vector2.right,
				HorMovementTime = 1f,
				VerMovementTime = 1f,
				HorMovementEase = 27,
				VerMovementEase = 7,
				WaitTimeAfterMovement = 1f
			};
			this.ShootingSequence.Add(item);
		}

		public override void OnAwake()
		{
			base.OnAwake();
			this._PontiffHuskRanged = (PontiffHuskRanged)this.Entity;
			this.AnimatorInyector = this._PontiffHuskRanged.GetComponentInChildren<PontiffHuskAnimatorInyector>();
		}

		private void OnLerpStop()
		{
			if (this._PontiffHuskRanged.IsAttacking)
			{
				this._PontiffHuskRanged.IsAttacking = false;
			}
			this._PontiffHuskRanged.GhostTrail.EnableGhostTrail = false;
			this._PontiffHuskRanged.FloatingMotion.IsFloating = true;
		}

		public override void OnStart()
		{
			base.OnStart();
			base.BehaviourTree = this._PontiffHuskRanged.GetComponent<BehaviourTreeOwner>();
			this._PontiffHuskRanged.OnDeath += this.PontiffHuskRangedOnEntityDie;
			MotionLerper motionLerper = this._PontiffHuskRanged.MotionLerper;
			motionLerper.OnLerpStop = (Core.SimpleEvent)Delegate.Combine(motionLerper.OnLerpStop, new Core.SimpleEvent(this.OnLerpStop));
			if (this._PontiffHuskRanged.TargetDistance > this._PontiffHuskRanged.ActivationRange)
			{
				this.Disappear(0f);
			}
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			if (this._PontiffHuskRanged.Target == null || this.IsDisappearing || base.IsDead())
			{
				return;
			}
			if (this._time < this._attackTime && this.IsAppear)
			{
				this._time += Time.deltaTime;
			}
			if (base.IsAttacking)
			{
				if (this.numMinesShooted > 0)
				{
					if (!this.AttackOnCooldown())
					{
						this.Shoot();
					}
					if (this.numMinesShooted == this.ShootingSequence.Count)
					{
						this.AnimatorInyector.StopShootingMines();
						this._PontiffHuskRanged.IsAttacking = false;
						this.numMinesShooted = 0;
					}
				}
			}
			else if (this.ShouldReturnToOrigin())
			{
				this.ResetState();
			}
		}

		private bool ShouldReturnToOrigin()
		{
			return Vector2.Distance(this.origin, base.transform.position) > this.MaxDistanceFromOrigin;
		}

		public override void Idle()
		{
			throw new NotImplementedException();
		}

		public override void Wander()
		{
			throw new NotImplementedException();
		}

		public override void Chase(Transform targetPosition)
		{
			if (this.AnimatorInyector.IsFading || !this._PontiffHuskRanged.gameObject.activeSelf || !this._PontiffHuskRanged.Target || this.IsDisappearing)
			{
				return;
			}
			if (this.attackingHorMove != null || this.attackingVerMove != null)
			{
				return;
			}
			if (this._PontiffHuskRanged.SpriteRenderer.color.a < 1f && !base.IsChasing)
			{
				this.AnimatorInyector.Fade(1f, 0.3f);
				this._PontiffHuskRanged.Audio.Appear();
				this.AnimatorInyector.PlayAppear();
			}
			if (this._target == null)
			{
				this._target = this._PontiffHuskRanged.Target.transform;
			}
			this.LookAtTarget(this._target.position);
			base.IsChasing = true;
			if (this.normalHorMove == null)
			{
				int num = (this.Entity.Status.Orientation != EntityOrientation.Left) ? -1 : 1;
				float num2 = base.transform.position.x + (float)num * this.Speed;
				this.normalHorMove = TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMoveX(base.transform, num2, 1f, false), 1), delegate()
				{
					this.normalHorMove = null;
				});
			}
			this._PontiffHuskRanged.Audio.UpdateFloatingPanning();
		}

		public void MoveAfterShooting()
		{
			if (this.attackingHorMove != null || this.attackingVerMove != null)
			{
				return;
			}
			if (this.normalHorMove != null)
			{
				TweenExtensions.Kill(this.normalHorMove, true);
			}
			int index = this.numMinesShooted - 1;
			PontiffHuskRangedBehaviour.ShootAndMove shootAndMove = this.ShootingSequence[index];
			float num = base.transform.position.x + shootAndMove.Movement.x;
			float horMovementTime = shootAndMove.HorMovementTime;
			Ease horMovementEase = shootAndMove.HorMovementEase;
			this.attackingHorMove = TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMoveX(base.transform, num, horMovementTime, false), horMovementEase), delegate()
			{
				this.attackingHorMove = null;
			});
			float num2 = base.transform.position.y + shootAndMove.Movement.y;
			float verMovementTime = shootAndMove.VerMovementTime;
			Ease verMovementEase = shootAndMove.VerMovementEase;
			this.attackingVerMove = TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMoveY(base.transform, num2, verMovementTime, false), verMovementEase), delegate()
			{
				this.attackingVerMove = null;
			});
			this._attackTime = shootAndMove.WaitTimeAfterMovement + Mathf.Max(horMovementTime, verMovementTime);
		}

		public float GetDistanceToTarget()
		{
			this._target = base.GetTarget();
			if (this._target == null)
			{
				return 1000f;
			}
			return Vector3.Distance(this._target.position, base.transform.position);
		}

		public bool CanSeePenitent()
		{
			return this._PontiffHuskRanged.VisionCone.CanSeeTarget(Core.Logic.Penitent.transform, "Penitent", false);
		}

		public bool IsTargetInsideShootingRange()
		{
			return this.GetDistanceToTarget() < this.MinTargetDistance && this.CanSeePenitent();
		}

		public bool CanShoot()
		{
			return this.IsTargetInsideShootingRange() && !this.AttackOnCooldown();
		}

		public override void Attack()
		{
			if (this._target == null)
			{
				return;
			}
			this.BulletVariantAttack();
		}

		private void BulletVariantAttack()
		{
			if (base.IsAttacking)
			{
				return;
			}
			this._PontiffHuskRanged.Audio.StopFloating();
			this._PontiffHuskRanged.FloatingMotion.IsFloating = true;
			this.LookAtTarget(this._target.position);
			this.AnimatorInyector.StartShootingMines();
		}

		public bool AttackOnCooldown()
		{
			return this._time < this._attackTime;
		}

		public override void Damage()
		{
			throw new NotImplementedException();
		}

		public override void StopMovement()
		{
			throw new NotImplementedException();
		}

		public void Appear(float time)
		{
			if (this.AnimatorInyector.IsFading)
			{
				return;
			}
			this.SetColliderScale();
			if (this.IsAppear)
			{
				return;
			}
			this.IsAwake = true;
			this.AnimatorInyector.Appear(time);
			this._PontiffHuskRanged.Audio.Appear();
			this.origin = base.transform.position;
			this.originSetted = true;
			this.numMinesShooted = 0;
			this.rangedMineInstances.Clear();
			this.currentMinePrefab = this.ShootingSequence[0].MinePrefabToShoot;
		}

		public void Disappear(float time)
		{
			if (this.AnimatorInyector.IsFading)
			{
				return;
			}
			this.IsDisappearing = true;
			this.IsAwake = false;
			this.AnimatorInyector.Disappear(time);
			this._PontiffHuskRanged.Audio.Dissapear();
			base.StartCoroutine(this.AfterDisappear(time));
			this._PontiffHuskRanged.BehaviourTree.enabled = false;
		}

		private IEnumerator AfterDisappear(float disappearTime)
		{
			yield return new WaitForSeconds(disappearTime);
			this.IsDisappearing = false;
			yield return null;
			this._PontiffHuskRanged.BehaviourTree.enabled = true;
			yield break;
		}

		public void HurtDisplacement(GameObject attackingEntity)
		{
			if (this._PontiffHuskRanged.MotionLerper.IsLerping)
			{
				this._PontiffHuskRanged.MotionLerper.StopLerping();
			}
			Vector3 position = attackingEntity.transform.position;
			if (attackingEntity.name.Equals(this.Entity.name))
			{
				position = Core.Logic.Penitent.GetPosition();
			}
			Vector2 vector = (position.x < base.transform.position.x) ? Vector2.right : Vector2.left;
			this._PontiffHuskRanged.GhostTrail.EnableGhostTrail = true;
			this._PontiffHuskRanged.MotionLerper.distanceToMove = 3f;
			this._PontiffHuskRanged.MotionLerper.TimeTakenDuringLerp = 0.5f;
			this._PontiffHuskRanged.MotionLerper.StartLerping(vector);
		}

		public void Shoot()
		{
			this._time = 0f;
			this.BulletAttack.MinePrefab = this.currentMinePrefab;
			this.rangedMineInstances.Add(this.BulletAttack.Shoot());
			RangedMine priorMine = null;
			foreach (RangedMine rangedMine in this.rangedMineInstances)
			{
				rangedMine.SetPriorMine(priorMine);
				priorMine = rangedMine;
			}
			this.numMinesShooted++;
			if (this.numMinesShooted < this.ShootingSequence.Count)
			{
				this.currentMinePrefab = this.ShootingSequence[this.numMinesShooted].MinePrefabToShoot;
			}
			this.MoveAfterShooting();
		}

		private void PontiffHuskRangedOnEntityDie()
		{
			this._PontiffHuskRanged.OnDeath -= this.PontiffHuskRangedOnEntityDie;
			if (this._PontiffHuskRanged.AttackArea != null)
			{
				this._PontiffHuskRanged.AttackArea.WeaponCollider.enabled = false;
			}
			this._PontiffHuskRanged.EntityDamageArea.DamageAreaCollider.enabled = false;
			if (base.BehaviourTree.isRunning)
			{
				base.BehaviourTree.StopBehaviour();
			}
		}

		public override void LookAtTarget(Vector3 targetPos)
		{
		}

		private void SetColliderScale()
		{
			this._PontiffHuskRanged.EntityDamageArea.DamageAreaCollider.transform.localScale = new Vector3((float)((this._PontiffHuskRanged.Status.Orientation != EntityOrientation.Right) ? -1 : 1), 1f, 1f);
		}

		public void SetAsleep()
		{
			if (!this.Asleep)
			{
				this.Asleep = true;
			}
			if (base.BehaviourTree.isRunning)
			{
				base.BehaviourTree.StopBehaviour();
			}
		}

		private void OnDestroy()
		{
			if (this._PontiffHuskRanged)
			{
				MotionLerper motionLerper = this._PontiffHuskRanged.MotionLerper;
				motionLerper.OnLerpStop = (Core.SimpleEvent)Delegate.Remove(motionLerper.OnLerpStop, new Core.SimpleEvent(this.OnLerpStop));
			}
		}

		public void ResetState()
		{
			if (!this.originSetted)
			{
				return;
			}
			base.gameObject.SetActive(true);
			this._PontiffHuskRanged.Stats.Life.SetToCurrentMax();
			this._PontiffHuskRanged.Status.Dead = false;
			this._PontiffHuskRanged.IsAttacking = false;
			this.AnimatorInyector.StopShootingMines();
			float num = 0.5f;
			this.Disappear(num);
			base.StartCoroutine(this.AfterDisappearResetState(num));
		}

		private IEnumerator AfterDisappearResetState(float disappearTime)
		{
			yield return new WaitForSeconds(disappearTime);
			ShortcutExtensions.DOKill(base.transform, true);
			base.transform.position = this.origin;
			this.AnimatorInyector.EntityAnimator.Play("IDLE");
			yield break;
		}

		public PontiffHuskRangedVariantAttack BulletAttack;

		public float MaxRndTimeAttack = 6f;

		public float MinRndTimeAttack = 3f;

		public float MinTargetDistance = 1f;

		public float Speed = 5f;

		public float AttackSpeed = 10f;

		public float MaxDistanceFromOrigin = 40f;

		public float BackDashDistance = 0.3f;

		public float BackDashTime = 0.2f;

		public List<PontiffHuskRangedBehaviour.ShootAndMove> ShootingSequence = new List<PontiffHuskRangedBehaviour.ShootAndMove>();

		private PontiffHuskRanged _PontiffHuskRanged;

		private Transform _target;

		private float _attackTime;

		private float _time;

		private Tween normalHorMove;

		private int numMinesShooted;

		private List<RangedMine> rangedMineInstances = new List<RangedMine>();

		private Tween attackingHorMove;

		private Tween attackingVerMove;

		private Vector3 origin;

		private bool originSetted;

		private GameObject currentMinePrefab;

		[Serializable]
		public struct ShootAndMove
		{
			[Title("~ Segment ~", null, 0, true, true)]
			public GameObject MinePrefabToShoot;

			public Vector2 Movement;

			public float HorMovementTime;

			public float VerMovementTime;

			public Ease HorMovementEase;

			public Ease VerMovementEase;

			public float WaitTimeAfterMovement;
		}
	}
}
