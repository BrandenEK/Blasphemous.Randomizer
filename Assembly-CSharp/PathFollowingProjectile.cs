using System;
using FMODUnity;
using Framework.Util;
using Gameplay.GameControllers.Enemies.Projectiles;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.Weapon;
using Sirenix.OdinInspector;
using UnityEngine;

public class PathFollowingProjectile : Weapon
{
	public AttackArea AttackArea { get; private set; }

	public Vector3 GetPosition()
	{
		return base.transform.position;
	}

	protected override void OnAwake()
	{
		base.OnAwake();
		this._animator = base.GetComponentInChildren<Animator>();
		this._animator.SetBool("SPIN", true);
		this.AttackArea = base.GetComponentInChildren<AttackArea>();
		this._projectile = base.GetComponent<SplineFollowingProjectile>();
		this._projectile.OnSplineCompletedEvent += this._projectile_OnSplineCompletedEvent;
		this.CreateHit();
	}

	private void CreateHit()
	{
		this._hit = new Hit
		{
			DamageAmount = this.damage,
			DamageType = this.damageType,
			DamageElement = this.damageElement,
			Force = this.force,
			HitSoundId = this.hitSound,
			Unnavoidable = this.unavoidable,
			forceGuardslide = this.forceGuardslide
		};
	}

	private void _projectile_OnSplineCompletedEvent(SplineFollowingProjectile obj)
	{
		if (this.destroyOnSplineFinish)
		{
			base.Destroy();
		}
	}

	protected override void OnStart()
	{
		base.OnStart();
		this.results = new RaycastHit2D[4];
		this.AttackArea.OnEnter += this.AttackAreaOnEnter;
	}

	public override void OnObjectReuse()
	{
		base.OnObjectReuse();
		this._animator.SetBool("SPIN", true);
	}

	public new void SetHit(Hit hit)
	{
		this._hit.AttackingEntity = hit.AttackingEntity;
	}

	public void SetDamage(float damage)
	{
		this._hit.DamageAmount = damage;
	}

	protected override void OnUpdate()
	{
		base.OnUpdate();
		if (this.leaveSparks)
		{
			this.CheckCollision();
		}
	}

	private void CheckCollision()
	{
		if (this.instantiationTimer > 0f)
		{
			this.instantiationTimer -= Time.deltaTime;
		}
		else
		{
			Vector2 pos = base.transform.position + this.collisionPoint;
			this.CheckCollisionInDirection(pos, Vector2.down);
			this.CheckCollisionInDirection(pos, Vector2.left);
			this.CheckCollisionInDirection(pos, Vector2.up);
			this.CheckCollisionInDirection(pos, Vector2.right);
		}
	}

	private void CheckCollisionInDirection(Vector2 pos, Vector2 dir)
	{
		if (Physics2D.Raycast(pos, dir, this.filter, this.results, this.collisionRadius) > 0)
		{
			Vector2 point = this.results[0].point;
			this.instantiationTimer = this.secondsBetweenInstances;
			GameObject gameObject = Object.Instantiate<GameObject>(this.sparksPrefab, point, Quaternion.identity);
			gameObject.transform.up = -dir;
		}
	}

	private void AttackAreaOnEnter(object sender, Collider2DParam collider2DParam)
	{
		GameObject gameObject = collider2DParam.Collider2DArg.gameObject;
		this.Attack(this._hit);
	}

	public override void Attack(Hit weapondHit)
	{
		base.GetDamageableEntities();
		base.AttackDamageableEntities(weapondHit);
	}

	public override void OnHit(Hit weaponHit)
	{
	}

	public void SetOwner(Enemy enemy)
	{
		this.AttackingEntity = enemy;
		this.WeaponOwner = enemy;
	}

	private void OnDestroy()
	{
		this.AttackArea.OnEnter -= this.AttackAreaOnEnter;
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(base.transform.position + this.collisionPoint, base.transform.position + this.collisionPoint + Vector3.down * this.collisionRadius);
		Gizmos.DrawLine(base.transform.position + this.collisionPoint, base.transform.position + this.collisionPoint + Vector3.left * this.collisionRadius);
		Gizmos.DrawLine(base.transform.position + this.collisionPoint, base.transform.position + this.collisionPoint + Vector3.up * this.collisionRadius);
		Gizmos.DrawLine(base.transform.position + this.collisionPoint, base.transform.position + this.collisionPoint + Vector3.right * this.collisionRadius);
	}

	private Animator _animator;

	public Enemy AttackingEntity;

	[SerializeField]
	[BoxGroup("Audio", true, false, 0)]
	[EventRef]
	public string hitSound;

	public DamageArea.DamageType damageType;

	public DamageArea.DamageElement damageElement;

	public float force;

	public float damage;

	public bool unavoidable;

	public bool forceGuardslide;

	public LayerMask BlockLayerMask;

	private Hit _hit;

	public bool leaveSparks = true;

	public GameObject sparksPrefab;

	public Vector2 collisionPoint;

	public float collisionRadius;

	public ContactFilter2D filter;

	private RaycastHit2D[] results;

	private float instantiationTimer;

	public float secondsBetweenInstances = 0.5f;

	public bool destroyOnSplineFinish = true;

	private SplineFollowingProjectile _projectile;
}
