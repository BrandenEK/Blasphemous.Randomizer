using System;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Penitent.Gizmos;
using Sirenix.OdinInspector;
using UnityEngine;

public class FloatingWeapon : Entity
{
	private void SetTargetPosition()
	{
		if (this.rootMotion == null)
		{
			return;
		}
		if (this.ownerEntity.Status.Orientation == EntityOrientation.Right)
		{
			this._targetPosition = this.rootMotion.transform.position;
			this.SetOrientation(EntityOrientation.Right, true, false);
			if (this.currentState == FloatingWeapon.FLOATING_WEAPON_STATES.FLOATING)
			{
				this.targetAngle = this.rootMotion.transform.localEulerAngles.z;
			}
		}
		else
		{
			this._targetPosition = this.rootMotion.ReversePosition;
			this.SetOrientation(EntityOrientation.Left, true, false);
			if (this.currentState == FloatingWeapon.FLOATING_WEAPON_STATES.FLOATING)
			{
				this.targetAngle = -this.rootMotion.transform.localEulerAngles.z;
			}
		}
	}

	public void ChangeState(FloatingWeapon.FLOATING_WEAPON_STATES st)
	{
		this.currentState = st;
	}

	private void SetAimingTarget(Transform aimTarget)
	{
		this._aimTarget = aimTarget;
	}

	protected override void OnStart()
	{
		base.OnStart();
		this.animator = base.GetComponentInChildren<Animator>();
		this.results = new RaycastHit2D[5];
	}

	public void SetAutoFollow(bool follow)
	{
		this.doFollow = follow;
	}

	protected override void OnUpdate()
	{
		base.OnUpdate();
		if (this.doFollow)
		{
			this.SetTargetPosition();
		}
		FloatingWeapon.FLOATING_WEAPON_STATES floating_WEAPON_STATES = this.currentState;
		if (floating_WEAPON_STATES != FloatingWeapon.FLOATING_WEAPON_STATES.FLOATING)
		{
			if (floating_WEAPON_STATES == FloatingWeapon.FLOATING_WEAPON_STATES.AIMING)
			{
				this.UpdateAimingAngle();
			}
		}
		else
		{
			this.UpdateFloatingOffset();
		}
		if (this.doFollow)
		{
			this.UpdatePositionAndRotation();
		}
		if (this.isSpinning)
		{
			this.CheckCollision();
		}
	}

	private void UpdatePositionAndRotation()
	{
		this._followPosition = this._targetPosition + this._floatingOffset;
		base.transform.position = Vector3.Lerp(base.transform.position, this._followPosition, this.smoothFactor);
		Quaternion quaternion = Quaternion.Euler(0f, 0f, this.targetAngle);
		base.transform.rotation = Quaternion.Slerp(base.transform.rotation, quaternion, this.smoothFactor);
	}

	private void UpdateAimingAngle()
	{
		Vector2 vector = this._aimTarget.transform.position + this.aimOffset - base.transform.position;
		this.targetAngle = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
	}

	private void CheckCollision()
	{
		if (this.instantiationTimer > 0f)
		{
			this.instantiationTimer -= Time.deltaTime;
		}
		else
		{
			this.instantiationTimer = this.secondsBetweenInstances;
			Vector2 vector = base.transform.position + this.collisionPoint;
			if (Physics2D.Raycast(vector, Vector2.down, this.filter, this.results, this.collisionRadius) > 0)
			{
				Vector2 point = this.results[0].point;
				Object.Instantiate<GameObject>(this.sparksPrefab, point, Quaternion.identity);
			}
		}
	}

	public void AimToPlayer()
	{
		if (Core.Logic.Penitent != null)
		{
			this.SetAimingTarget(Core.Logic.Penitent.transform);
		}
	}

	private void UpdateFloatingOffset()
	{
		float num = Mathf.Sin(this.floatingFrequency * Time.time) * this.floatingAmplitude;
		this._floatingOffset = new Vector2(0f, num);
	}

	public void SetSpinning(bool spin)
	{
		this.isSpinning = spin;
		this.instantiationTimer = this.secondsBetweenInstances;
		this.animator.SetBool("SPIN", spin);
	}

	public void Hide(bool vanishAnimation = false)
	{
		this.hidden = true;
		if (vanishAnimation)
		{
			this.animator.SetBool("SHOW", false);
		}
		else
		{
			base.gameObject.SetActive(false);
		}
	}

	public void Show(bool vanishAnimation = false)
	{
		this.hidden = false;
		base.gameObject.SetActive(true);
		if (vanishAnimation)
		{
			this.animator.SetBool("SHOW", true);
		}
	}

	public void Activate(bool act, bool vanishAnimation = false)
	{
		if (act)
		{
			this.Show(vanishAnimation);
		}
		else
		{
			this.Hide(vanishAnimation);
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(base.transform.position + this.collisionPoint, base.transform.position + this.collisionPoint + Vector3.down * this.collisionRadius);
	}

	[FoldoutGroup("Follow settings", 0)]
	public RootMotionDriver rootMotion;

	[FoldoutGroup("Follow settings", 0)]
	public Entity ownerEntity;

	[FoldoutGroup("Follow settings", 0)]
	public bool doFollow = true;

	[FoldoutGroup("Follow settings", 0)]
	public float targetAngle;

	[FoldoutGroup("Floating settings", 0)]
	public float floatingFrequency;

	[FoldoutGroup("Floating settings", 0)]
	public float floatingAmplitude;

	[FoldoutGroup("Floating settings", 0)]
	public float smoothFactor;

	[FoldoutGroup("Floating settings", 0)]
	public float maxAngle = 10f;

	private Vector2 _targetPosition;

	private Vector2 _followPosition;

	private Vector2 _floatingOffset;

	[FoldoutGroup("Sparks settings", 0)]
	public GameObject sparksPrefab;

	[FoldoutGroup("Sparks settings", 0)]
	public Vector2 collisionPoint;

	[FoldoutGroup("Sparks settings", 0)]
	public float collisionRadius;

	[FoldoutGroup("Sparks settings", 0)]
	public ContactFilter2D filter;

	[FoldoutGroup("Sparks settings", 0)]
	public float secondsBetweenInstances = 0.5f;

	private RaycastHit2D[] results;

	private float instantiationTimer;

	public FloatingWeapon.FLOATING_WEAPON_STATES currentState;

	public bool isSpinning;

	public bool hidden;

	private Transform _aimTarget;

	public Vector2 aimOffset;

	private Animator animator;

	public enum FLOATING_WEAPON_STATES
	{
		FLOATING,
		AIMING,
		STOP
	}
}
