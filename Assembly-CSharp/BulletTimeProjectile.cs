using System;
using System.Collections;
using Gameplay.GameControllers.Enemies.BellGhost;
using Gameplay.GameControllers.Enemies.Projectiles;
using UnityEngine;

public class BulletTimeProjectile : StraightProjectile
{
	protected override void OnAwake()
	{
		base.OnAwake();
		this.projectileWeapon = base.GetComponent<ProjectileWeapon>();
		if (this.projectileWeapon != null)
		{
			this.projectileWeapon.OnProjectileHitsSomething += this.OnProjectileHitsSomething;
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		this.projectileWeapon.OnProjectileHitsSomething -= this.OnProjectileHitsSomething;
	}

	private void OnProjectileHitsSomething(ProjectileWeapon obj)
	{
		if (this.slowOnHit)
		{
			this.counter = this.secondsToAccelerate * this.reductionOnHit;
		}
	}

	public void Accelerate(float maxVelocityModifier = 1f)
	{
		this.maxVelocityModifier = maxVelocityModifier;
		base.StopAllCoroutines();
		if (base.gameObject.activeInHierarchy)
		{
			base.StartCoroutine(this.AccelerateInSeconds(this.secondsToAccelerate));
		}
	}

	private IEnumerator AccelerateInSeconds(float seconds)
	{
		this.counter = 0f;
		while (this.counter < seconds)
		{
			this.counter += Time.deltaTime;
			float lerpValue = this.accelerationCurve.Evaluate(this.counter / seconds);
			this.velocity = this.velocity.normalized * Mathf.Lerp(this.minVelocity, this.maxVelocity * this.maxVelocityModifier, lerpValue);
			yield return null;
		}
		this.velocity = this.velocity.normalized * this.maxVelocity * this.maxVelocityModifier;
		yield break;
	}

	public AnimationCurve accelerationCurve;

	public float secondsToAccelerate = 1f;

	public float minVelocity = 1f;

	public float maxVelocity = 10f;

	private float maxVelocityModifier = 1f;

	public bool slowOnHit;

	public float reductionOnHit = 0.5f;

	private ProjectileWeapon projectileWeapon;

	private float counter;
}
