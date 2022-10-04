using System;
using Framework.Util;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.Weapon;
using Sirenix.OdinInspector;

public class ShockwaveAreaAttack : EnemyAttack
{
	public AttackArea circleArea { get; set; }

	public bool TargetInAttackArea { get; private set; }

	protected override void OnStart()
	{
		base.OnStart();
		base.CurrentEnemyWeapon = base.GetComponentInChildren<Weapon>();
		this.circleArea = base.GetComponentInChildren<AttackArea>();
		this.circleArea.OnEnter += this.OnEnterAttackArea;
		this.circleArea.OnExit += this.OnExitAttackArea;
		Entity entityOwner = base.EntityOwner;
		this._areaAttackHit = new Hit
		{
			AttackingEntity = entityOwner.gameObject,
			DamageAmount = (float)this.damage,
			DamageType = this.DamageType,
			DamageElement = this.DamageElement,
			HitSoundId = this.HitSound,
			Unnavoidable = this.unavoidable,
			forceGuardslide = this.forceGuardslide,
			ThrowbackDirByOwnerPosition = true
		};
	}

	public override void CurrentWeaponAttack()
	{
		base.CurrentWeaponAttack();
	}

	private void OnEnterAttackArea(object sender, Collider2DParam e)
	{
		base.CurrentEnemyWeapon.Attack(this._areaAttackHit);
	}

	private void OnExitAttackArea(object sender, Collider2DParam e)
	{
	}

	private void OnDestroy()
	{
		if (this.circleArea)
		{
			this.circleArea.OnEnter -= this.OnEnterAttackArea;
		}
		if (this.circleArea)
		{
			this.circleArea.OnExit -= this.OnExitAttackArea;
		}
	}

	private Hit _areaAttackHit;

	private const string PENITENT_TAG = "Penitent";

	[BoxGroup("Shockwave damage settings", true, false, 0)]
	public int damage = 25;

	[BoxGroup("Shockwave damage settings", true, false, 0)]
	public bool unavoidable = true;

	[BoxGroup("Shockwave damage settings", true, false, 0)]
	public bool forceGuardslide = true;
}
