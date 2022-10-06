using System;
using System.Collections;
using System.Collections.Generic;
using BezierSplines;
using Framework.Managers;
using Gameplay.GameControllers.Bosses.CommonAttacks;
using Gameplay.GameControllers.Bosses.Quirce.Attack;
using Gameplay.GameControllers.Enemies.Projectiles;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Framework.IA
{
	public class EnemyActionTesterBehaviour : EnemyBehaviour
	{
		private void Start()
		{
			this.countDown_EA = new CountdownFromTen_EnemyAction();
			this.randomWait_EA = new WaitSeconds_EnemyAction();
			this.waitBetweenActions_EA = new WaitSeconds_EnemyAction();
			this.launchMethod_EA = new LaunchMethod_EnemyAction();
			this.moveEasing_EA = new MoveEasing_EnemyAction();
			this.moveAttacking_EA = new EnemyActionTesterBehaviour.MoveAroundAndAttack_EnemyAction();
			this.jumpBackNShoot_EA = new EnemyActionTesterBehaviour.JumpBackAndShoot_EnemyAction();
			this.setBulletNActivate_EA = new EnemyActionTesterBehaviour.SetBulletsAndActivate_EnemyAction();
			this.damageParameters = new Dictionary<string, float>();
			this.instantProjectileAttack = base.GetComponentInChildren<BossInstantProjectileAttack>();
			this.jumpAttack = base.GetComponentInChildren<BossJumpAttack>();
			this.bulletTimeProjectileAttack = base.GetComponentInChildren<BossStraightProjectileAttack>();
			this.bullets = new List<BulletTimeProjectile>();
			PoolManager.Instance.CreatePool(this.amanecidaAxePrefab, 2);
			this.availableAttacks = new List<EnemyAction>
			{
				this.jumpBackNShoot_EA,
				this.moveAttacking_EA
			};
			this.SpawnAxe(Vector2.right * 2f);
			this.SpawnAxe(Vector2.left * 2f);
		}

		private void SpawnAxe(Vector2 dir)
		{
			if (this.axes == null)
			{
				this.axes = new List<AmanecidaAxeBehaviour>();
			}
			AmanecidaAxeBehaviour component = PoolManager.Instance.ReuseObject(this.amanecidaAxePrefab, base.transform.position + Vector2.up * 1f + dir, Quaternion.identity, false, 1).GameObject.GetComponent<AmanecidaAxeBehaviour>();
			PathFollowingProjectile component2 = component.GetComponent<PathFollowingProjectile>();
			Hit h = new Hit
			{
				AttackingEntity = base.gameObject,
				DamageAmount = this.GetDamageParameter("AXE_DMG"),
				DamageType = component2.damageType,
				DamageElement = component2.damageElement,
				Force = component2.force,
				HitSoundId = component2.hitSound,
				Unnavoidable = component2.unavoidable
			};
			component.InitDamageData(h);
			this.axes.Add(component);
		}

		public void SetDamageParameter(string key, float value)
		{
			this.damageParameters[key] = value;
		}

		private float GetDamageParameter(string key)
		{
			return this.damageParameters[key];
		}

		private void Update()
		{
			if (Input.GetKeyDown(49))
			{
				this.LaunchNewAction();
			}
			if (Input.GetKeyDown(50))
			{
				this.currentAction.StopAction();
			}
			if (Input.GetKeyDown(51))
			{
				this.setBulletNActivate_EA.StartAction(this, 8, new Action(this.DoProjectileAttack), new Action(this.ActivateBullets), this.timeSlowCurve);
			}
			if (Input.GetKeyDown(53))
			{
				this.moveEasing_EA.StopAction();
				Debug.Log("TESTING RANDOM MOVE");
				this.moveEasing_EA.StartAction(this, 5f, 1f, 10, 1.7f);
			}
			if (Input.GetKeyDown(54))
			{
				this.moveEasing_EA.StopAction();
				this.moveAttacking_EA.StopAction();
				Debug.Log("TESTING ATTACK ROUTINE MOVE");
				this.moveAttacking_EA.StartAction(this, 5, 3f, 1f, new Action(this.DoDummyAttack));
			}
			if (Input.GetKeyDown(55))
			{
				this.jumpBackNShoot_EA.StopAction();
				Debug.Log("TESTING JUMPBACK AND SHOOT");
				this.jumpBackNShoot_EA.StartAction(this, 3, new Action(this.DoDummyBackJump), new Action(this.DoDummyAttack));
			}
		}

		private void LaunchActionWithParameters(EnemyAction act)
		{
			if (act == this.jumpBackNShoot_EA)
			{
				this.jumpBackNShoot_EA.StartAction(this, 3, new Action(this.DoDummyBackJump), new Action(this.DoDummyAttack));
			}
			else if (act == this.moveAttacking_EA)
			{
				this.moveAttacking_EA.StartAction(this, 3, 3f, 1f, new Action(this.DoDummyAttack));
			}
		}

		private void LaunchNewAction()
		{
			if (this.currentAction != null)
			{
				this.currentAction.StopAction();
			}
			this.currentAction = this.availableAttacks[Random.Range(0, this.availableAttacks.Count)];
			this.currentAction.OnActionEnds += this.CurrentAction_OnActionEnds;
			this.currentAction.OnActionIsStopped += this.CurrentAction_OnActionStops;
			this.LaunchActionWithParameters(this.currentAction);
		}

		private void WaitBetweenActions()
		{
			this.waitBetweenActions_EA.StartAction(this, 1f, 3f);
			this.waitBetweenActions_EA.OnActionEnds += this.CurrentAction_OnActionEnds;
		}

		private void CurrentAction_OnActionEnds(EnemyAction e)
		{
			e.OnActionEnds -= this.CurrentAction_OnActionEnds;
			if (e == this.waitBetweenActions_EA)
			{
				this.WaitBetweenActions();
			}
			else
			{
				this.LaunchNewAction();
			}
		}

		private void CurrentAction_OnActionStops(EnemyAction e)
		{
			e.OnActionIsStopped -= this.CurrentAction_OnActionStops;
		}

		private void Foo_PlayAnimation()
		{
			Debug.Log("PLAYING ANIMATION THROUGH THE ANIMATOR INYECTOR");
		}

		public void DoDummyAttack()
		{
			Debug.Log("<DUMMY ATTACK>");
			this.instantProjectileAttack.Shoot(base.transform.position, Core.Logic.Penitent.transform.position - base.transform.position);
		}

		public void DoDummyBackJump()
		{
			Debug.Log("<DUMMY ATTACK>");
			Vector2 vector = -base.transform.right * 4f;
			Vector2 vector2 = base.transform.position + vector;
			this.jumpAttack.Use(base.transform, vector2);
		}

		public void DoProjectileAttack()
		{
			Debug.Log("<PROJECTILE ATTACK>");
			Vector2 dirToPenitent = this.GetDirToPenitent(base.transform.position);
			StraightProjectile straightProjectile = this.bulletTimeProjectileAttack.Shoot(dirToPenitent);
			this.bullets.Add(straightProjectile as BulletTimeProjectile);
		}

		public void ActivateBullets()
		{
			Debug.Log("<ACTIVATE BULLETS>");
			if (this.bullets == null || this.bullets.Count == 0)
			{
				return;
			}
			foreach (BulletTimeProjectile bulletTimeProjectile in this.bullets)
			{
				bulletTimeProjectile.Accelerate(1f);
			}
			this.bullets.Clear();
		}

		private Vector2 GetDirToPenitent(Vector2 point)
		{
			return Core.Logic.Penitent.transform.position - point;
		}

		public override void Attack()
		{
		}

		public override void Chase(Transform targetPosition)
		{
		}

		public override void Damage()
		{
		}

		public override void Idle()
		{
		}

		public override void StopMovement()
		{
		}

		public override void Wander()
		{
		}

		private CountdownFromTen_EnemyAction countDown_EA;

		private WaitSeconds_EnemyAction randomWait_EA;

		private WaitSeconds_EnemyAction waitBetweenActions_EA;

		private LaunchMethod_EnemyAction launchMethod_EA;

		private MoveEasing_EnemyAction moveEasing_EA;

		private EnemyActionTesterBehaviour.MoveAroundAndAttack_EnemyAction moveAttacking_EA;

		private EnemyActionTesterBehaviour.JumpBackAndShoot_EnemyAction jumpBackNShoot_EA;

		private EnemyActionTesterBehaviour.SetBulletsAndActivate_EnemyAction setBulletNActivate_EA;

		private BossInstantProjectileAttack instantProjectileAttack;

		private BossJumpAttack jumpAttack;

		private List<EnemyAction> availableAttacks;

		private EnemyAction currentAction;

		private BossStraightProjectileAttack bulletTimeProjectileAttack;

		private List<BulletTimeProjectile> bullets;

		public AnimationCurve timeSlowCurve;

		public BezierSpline horizontalSpline;

		public GameObject amanecidaAxePrefab;

		public List<AmanecidaAxeBehaviour> axes;

		private Dictionary<string, float> damageParameters;

		private const string AXE_DAMAGE = "AXE_DMG";

		public class JumpBackAndShoot_EnemyAction : EnemyAction
		{
			protected override void DoOnStop()
			{
				base.DoOnStop();
				this.ACT_WAIT = new WaitSeconds_EnemyAction();
				this.ACT_JUMP = new LaunchMethod_EnemyAction();
				this.ACT_SHOOT = new LaunchMethod_EnemyAction();
			}

			public EnemyAction StartAction(EnemyBehaviour e, int _n, Action _jumpMethod, Action _shootMethod)
			{
				this.n = _n;
				this.shootMethod = _shootMethod;
				this.jumpMethod = _jumpMethod;
				return base.StartAction(e);
			}

			protected override IEnumerator BaseCoroutine()
			{
				WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();
				LaunchMethod_EnemyAction ACT_JUMP = new LaunchMethod_EnemyAction();
				LaunchMethod_EnemyAction ACT_SHOOT = new LaunchMethod_EnemyAction();
				ACT_JUMP.StartAction(this.owner, this.jumpMethod);
				for (int i = 0; i < this.n; i++)
				{
					ACT_WAIT.StartAction(this.owner, 0.2f, 0.2f);
					yield return ACT_WAIT.waitForCompletion;
					ACT_SHOOT.StartAction(this.owner, this.shootMethod);
				}
				ACT_WAIT.StartAction(this.owner, 0.6f, 0.6f);
				yield return ACT_WAIT.waitForCompletion;
				base.FinishAction();
				yield break;
			}

			private int n;

			private float distance;

			private float seconds;

			private Action jumpMethod;

			private Action shootMethod;

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private LaunchMethod_EnemyAction ACT_JUMP = new LaunchMethod_EnemyAction();

			private LaunchMethod_EnemyAction ACT_SHOOT = new LaunchMethod_EnemyAction();
		}

		public class MoveAroundAndAttack_EnemyAction : EnemyAction
		{
			public EnemyAction StartAction(EnemyBehaviour e, int _n, float _distance, float _seconds, Action _method)
			{
				this.n = _n;
				this.distance = _distance;
				this.seconds = _seconds;
				this.method = _method;
				return base.StartAction(e);
			}

			protected override void DoOnStop()
			{
				this.ACT_WAIT.StopAction();
				this.ACT_MOVE.StopAction();
				this.ACT_METHOD.StopAction();
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				this.ACT_MOVE.StartAction(this.owner, this.owner.transform.position + Vector2.up * 6f, 2f, 21, null, true, null, true, true, 1.7f);
				yield return this.ACT_MOVE.waitForCompletion;
				for (int i = 0; i < this.n; i++)
				{
					this.ACT_MOVE.StartAction(this.owner, this.distance, this.seconds, 21, 1.7f);
					yield return this.ACT_MOVE.waitForCompletion;
					this.ACT_WAIT.StartAction(this.owner, 0.5f, 0.5f);
					yield return this.ACT_WAIT.waitForCompletion;
					this.ACT_METHOD.StartAction(this.owner, this.method);
					yield return this.ACT_METHOD.waitForCompletion;
					this.ACT_WAIT.StartAction(this.owner, 0.2f, 0.2f);
					yield return this.ACT_WAIT.waitForCompletion;
					this.ACT_METHOD.StartAction(this.owner, this.method);
					yield return this.ACT_METHOD.waitForCompletion;
					this.ACT_WAIT.StartAction(this.owner, 0.2f, 0.2f);
					yield return this.ACT_WAIT.waitForCompletion;
					this.ACT_METHOD.StartAction(this.owner, this.method);
					yield return this.ACT_METHOD.waitForCompletion;
					this.ACT_WAIT.StartAction(this.owner, 0.5f, 1.5f);
					yield return this.ACT_WAIT.waitForCompletion;
				}
				base.FinishAction();
				yield break;
			}

			private int n;

			private float distance;

			private float seconds;

			private Action method;

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private MoveEasing_EnemyAction ACT_MOVE = new MoveEasing_EnemyAction();

			private LaunchMethod_EnemyAction ACT_METHOD = new LaunchMethod_EnemyAction();
		}

		public class SetBulletsAndActivate_EnemyAction : EnemyAction
		{
			public EnemyAction StartAction(EnemyBehaviour e, int _n, Action _setBulletMethod, Action _activateMethod, AnimationCurve _timeSlowCurve)
			{
				this.n = _n;
				this.setBulletMethod = _setBulletMethod;
				this.activateMethod = _activateMethod;
				this.timeSlowCurve = _timeSlowCurve;
				return base.StartAction(e);
			}

			protected override void DoOnStop()
			{
				this.ACT_WAIT.StopAction();
				this.ACT_MOVE.StopAction();
				this.ACT_ACTIVATE.StopAction();
				this.ACT_SETBULLET.StopAction();
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				Vector2 pointA = Core.Logic.Penitent.transform.position + Vector2.up * 5f + Vector2.left * 5f;
				Vector2 pointB = pointA + Vector2.right * 10f;
				Core.Logic.ScreenFreeze.Freeze(0.15f, 3f, 0f, this.timeSlowCurve);
				Vector2 target;
				for (int i = 0; i < this.n; i++)
				{
					target = Vector2.Lerp(pointA, pointB, (float)i / (float)this.n);
					target += Vector2.up * (float)Random.Range(-1, 1) * 1f;
					this.ACT_MOVE.StartAction(this.owner, target, 0.15f, 7, null, false, null, true, true, 1.7f);
					yield return this.ACT_MOVE.waitForCompletion;
					this.ACT_SETBULLET.StartAction(this.owner, this.setBulletMethod);
					yield return this.ACT_SETBULLET.waitForCompletion;
				}
				target = Vector2.Lerp(pointA, pointB, 0.5f) + Vector2.up * 2f;
				this.ACT_MOVE.StartAction(this.owner, target, 0.5f, 7, null, true, null, true, true, 1.7f);
				yield return this.ACT_MOVE.waitForCompletion;
				this.ACT_ACTIVATE.StartAction(this.owner, this.activateMethod);
				yield return this.ACT_ACTIVATE.waitForCompletion;
				base.FinishAction();
				yield break;
			}

			private int n;

			private Action setBulletMethod;

			private Action activateMethod;

			private AnimationCurve timeSlowCurve;

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private MoveEasing_EnemyAction ACT_MOVE = new MoveEasing_EnemyAction();

			private LaunchMethod_EnemyAction ACT_SETBULLET = new LaunchMethod_EnemyAction();

			private LaunchMethod_EnemyAction ACT_ACTIVATE = new LaunchMethod_EnemyAction();
		}
	}
}
