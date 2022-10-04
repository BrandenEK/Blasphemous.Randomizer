using System;
using System.Collections;
using BezierSplines;
using DG.Tweening;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Enemies.Projectiles;
using Gameplay.GameControllers.Entities;
using Maikel.StatelessFSM;
using Maikel.SteeringBehaviors;
using Plugins.Maikel.StateMachine;
using UnityEngine;

public class AmanecidaAxeBehaviour : EnemyBehaviour
{
	public override void OnStart()
	{
		base.OnStart();
		this.agent = base.GetComponent<AutonomousAgent>();
		this.pathFollowingProjectile = base.GetComponent<PathFollowingProjectile>();
		this.seek = this.agent.GetComponent<Seek>();
		this.stControlled = new AmanecidaAxeSt_Controlled();
		this.stSeekTarget = new AmanecidaAxeSt_SeekPlayer();
		this.fsm = new StateMachine<AmanecidaAxeBehaviour>(this, this.stControlled, null, null);
	}

	public void InitDamageData(Hit h)
	{
		base.GetComponent<PathFollowingProjectile>().SetHit(h);
	}

	public void SetSeek(bool free)
	{
		if (free)
		{
			this.fsm.ChangeState(this.stSeekTarget);
		}
		else
		{
			this.fsm.ChangeState(this.stControlled);
		}
	}

	public override void OnUpdate()
	{
		if (this.active)
		{
			base.OnUpdate();
			this.fsm.DoUpdate();
		}
	}

	public void SeekTarget()
	{
		this.seek.target = this.target.position;
	}

	public void SeekTarget(Vector2 targetPosition)
	{
		this.seek.target = targetPosition;
	}

	public void ActivateAgent(bool active)
	{
		this.agent.enabled = active;
	}

	public void Show()
	{
		if (this.trailCleaner == null)
		{
			this.trailCleaner = base.GetComponentInChildren<ResetTrailRendererOnEnable>();
		}
		if (this.trailCleaner != null)
		{
			this.trailCleaner.Clean();
		}
		if (this.trailRenderer == null)
		{
			this.trailRenderer = base.GetComponentInChildren<TrailRenderer>();
		}
		if (this.trailRenderer != null)
		{
			this.trailRenderer.enabled = true;
		}
		if (this.particleSystem == null)
		{
			this.particleSystem = base.GetComponentInChildren<ParticleSystem>();
		}
		if (this.particleSystem != null)
		{
			this.particleSystem.Play();
		}
		this.animator.SetBool("HIDE", false);
	}

	public void Hide()
	{
		if (this.trailRenderer == null)
		{
			this.trailRenderer = base.GetComponentInChildren<TrailRenderer>();
		}
		if (this.trailRenderer != null)
		{
			this.trailRenderer.enabled = false;
		}
		if (this.particleSystem == null)
		{
			this.particleSystem = base.GetComponentInChildren<ParticleSystem>();
		}
		if (this.particleSystem != null)
		{
			this.particleSystem.Stop();
		}
		this.animator.SetBool("HIDE", true);
	}

	public void SetRepositionMode(bool isInReposition)
	{
		if (this.boxCollider == null)
		{
			this.boxCollider = base.GetComponentInChildren<BoxCollider2D>();
		}
		if (this.spriteRenderer == null)
		{
			this.spriteRenderer = base.GetComponentInChildren<SpriteRenderer>();
		}
		if (this.boxCollider != null)
		{
			this.boxCollider.enabled = !isInReposition;
		}
		this.pathFollowingProjectile.leaveSparks = !isInReposition;
		if (isInReposition)
		{
			this.Hide();
		}
		else
		{
			this.Show();
		}
	}

	public override void Attack()
	{
		throw new NotImplementedException();
	}

	public override void Chase(Transform targetPosition)
	{
		throw new NotImplementedException();
	}

	public override void Damage()
	{
		throw new NotImplementedException();
	}

	public override void Idle()
	{
		throw new NotImplementedException();
	}

	public override void StopMovement()
	{
		throw new NotImplementedException();
	}

	public override void Wander()
	{
		throw new NotImplementedException();
	}

	public void SetVisible(bool v)
	{
		if (this.animator == null)
		{
			this.animator = base.GetComponentInChildren<Animator>();
		}
		this.animator.gameObject.SetActive(v);
		this.active = v;
		base.GetComponent<PathFollowingProjectile>().leaveSparks = v;
	}

	public SplineFollowingProjectile splineFollower;

	public AmanecidaAxeBehaviour.SplineFollowingProjectile_FollowSpline_EnemyAction axeSplineFollowAction = new AmanecidaAxeBehaviour.SplineFollowingProjectile_FollowSpline_EnemyAction();

	public StateMachine<AmanecidaAxeBehaviour> fsm;

	public State<AmanecidaAxeBehaviour> stControlled;

	public State<AmanecidaAxeBehaviour> stSeekTarget;

	public Transform target;

	public bool active = true;

	public AutonomousAgent agent;

	private Seek seek;

	private Animator animator;

	private BoxCollider2D boxCollider;

	private SpriteRenderer spriteRenderer;

	private TrailRenderer trailRenderer;

	private ParticleSystem particleSystem;

	private ResetTrailRendererOnEnable trailCleaner;

	private PathFollowingProjectile pathFollowingProjectile;

	public class SplineFollowingProjectile_FollowSpline_EnemyAction : EnemyAction
	{
		public EnemyAction StartAction(EnemyBehaviour e, SplineFollowingProjectile follower, SplineThrowData throwData, BezierSpline spline)
		{
			this.follower = follower;
			this.spline = spline;
			this.duration = throwData.duration;
			this.curve = throwData.curve;
			this.origin = spline.GetPoint(0f);
			return base.StartAction(e);
		}

		public EnemyAction StartAction(EnemyBehaviour e, SplineFollowingProjectile follower, Vector2 origin, SplineThrowData throwData, BezierSpline spline)
		{
			return this.StartAction(e, follower, throwData, spline);
		}

		public EnemyAction StartAction(EnemyBehaviour e, SplineFollowingProjectile follower, Vector2 origin, Vector2 end, int endPointIndex, SplineThrowData throwData, BezierSpline spline)
		{
			spline.transform.position = origin;
			Vector2 b = spline.GetControlPoint(endPointIndex) - spline.GetControlPoint(endPointIndex - 1);
			Vector2 b2 = spline.GetControlPoint(endPointIndex) - spline.GetControlPoint(endPointIndex + 1);
			Vector2 vector = spline.transform.InverseTransformPoint(end);
			spline.SetControlPoint(endPointIndex, vector);
			spline.SetControlPoint(endPointIndex - 1, vector - b);
			spline.SetControlPoint(endPointIndex + 1, vector - b2);
			return this.StartAction(e, follower, origin, throwData, spline);
		}

		protected override void DoOnStart()
		{
			base.DoOnStart();
		}

		protected override void DoOnStop()
		{
			base.DoOnStop();
			this.follower.Stop();
		}

		protected override IEnumerator BaseCoroutine()
		{
			this.ACT_MOVE.StartAction(this.owner, this.origin, 0.1f, Ease.InOutCubic, null, true, null, true, true, 1.7f);
			yield return this.ACT_MOVE.waitForCompletion;
			this.follower.Init(this.origin, this.spline, this.duration, this.curve);
			yield return new WaitUntil(() => !this.follower.IsFollowing());
			base.FinishAction();
			yield break;
		}

		private MoveEasing_EnemyAction ACT_MOVE = new MoveEasing_EnemyAction();

		private SplineFollowingProjectile follower;

		private Vector2 origin;

		private BezierSpline spline;

		private float duration;

		private AnimationCurve curve;
	}
}
