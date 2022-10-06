using System;
using System.Collections;
using FMODUnity;
using Framework.Inventory;
using Framework.Managers;
using Gameplay.GameControllers.Enemies.Framework.Damage;
using Gameplay.GameControllers.Enemies.Projectiles;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Penitent;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Tools.Items
{
	public class PenitentAreaAttack : ObjectEffect
	{
		protected override void OnStart()
		{
			SpawnManager.OnPlayerSpawn += this.OnPlayerSpawn;
		}

		protected void OnDisable()
		{
			base.StopAllCoroutines();
		}

		private void OnPlayerSpawn(Penitent penitent)
		{
			if (this.prefabIntoPlayer != null)
			{
				PoolManager.Instance.CreatePool(this.prefabIntoPlayer, 1);
			}
			if (this.prefabIntoEnemies != null)
			{
				PoolManager.Instance.CreatePool(this.prefabIntoEnemies, 10);
			}
		}

		protected override bool OnApplyEffect()
		{
			this.CreateHit();
			if (this.prefabIntoPlayer != null)
			{
				PoolManager.Instance.ReuseObject(this.prefabIntoPlayer, Core.Logic.Penitent.GetPosition() + this.intoPlayerOffset, Quaternion.identity, false, 1);
			}
			base.StartCoroutine(this.DamageCoroutine(this.attackHit));
			return base.OnApplyEffect();
		}

		private float CalculateDamage()
		{
			return this.Amount * (1f + (Core.Logic.Penitent.Stats.PrayerStrengthMultiplier.Final - 1f) * 0.5f);
		}

		private void CreateHit()
		{
			this.attackHit = new Hit
			{
				AttackingEntity = Core.Logic.Penitent.gameObject,
				DamageType = this.DamageType,
				Force = this.Force,
				DamageAmount = this.CalculateDamage(),
				HitSoundId = this.HitSound,
				Unnavoidable = this.Unnavoidable
			};
		}

		public void ShakeWave()
		{
			Core.Logic.ScreenFreeze.Freeze(0.05f, this.slowTimeDuration, 0f, this.slowTimeCurve);
			Core.Logic.CameraManager.ShockwaveManager.Shockwave(Core.Logic.Penitent.transform.position + this.intoPlayerOffset, 1.2f, 0.2f, 1.8f);
			Core.Logic.CameraManager.ProCamera2DShake.Shake(0.35f, Vector3.down * 0.5f, 12, 0.2f, 0.01f, default(Vector3), 0.01f, true);
		}

		private IEnumerator DamageCoroutine(Hit hit)
		{
			Penitent penitent = Core.Logic.Penitent;
			int max = 16;
			Quaternion rot = Quaternion.Euler(0f, 0f, 360f / (float)max);
			Vector2 v = Vector2.right * this.Radius;
			Vector3 playerPos = penitent.GetPosition();
			for (int j = 0; j < max; j++)
			{
				Debug.DrawLine(playerPos, playerPos + v, Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f, 1f, 1f), 5f);
				v = rot * v;
			}
			this.ShakeWave();
			Collider2D[] hits = new Collider2D[20];
			int overlappedEntities = Physics2D.OverlapCircleNonAlloc(penitent.GetPosition() + this.intoPlayerOffset, this.Radius, hits, this.damageMask);
			float currentDamageDelay = this.damageDelay;
			for (int i = 0; i < overlappedEntities; i++)
			{
				if (hits[i])
				{
					if (!hits[i].CompareTag("Penitent"))
					{
						EnemyDamageArea dmgArea = null;
						Enemy enemy = hits[i].GetComponentInParent<Enemy>();
						if (enemy)
						{
							dmgArea = enemy.GetComponentInChildren<EnemyDamageArea>();
							if (enemy.SpriteRenderer != null && !enemy.SpriteRenderer.IsVisibleFrom(Camera.main))
							{
								goto IL_42A;
							}
						}
						if (dmgArea != null)
						{
							if (this.prefabIntoEnemies)
							{
								PoolManager.Instance.ReuseObject(this.prefabIntoEnemies, hits[i].bounds.center + this.intoEnemiesOffset, Quaternion.identity, false, 1);
							}
							yield return new WaitForSeconds(currentDamageDelay);
							currentDamageDelay *= 0.8f;
							if (dmgArea && dmgArea.OwnerEntity)
							{
								((IDamageable)dmgArea.OwnerEntity).Damage(hit);
							}
						}
						else
						{
							IDamageable damageable = hits[i].GetComponentInChildren<IDamageable>();
							if (damageable != null && !(damageable is ProjectileReaction))
							{
								if (this.prefabIntoEnemies)
								{
									PoolManager.Instance.ReuseObject(this.prefabIntoEnemies, hits[i].gameObject.transform.position + this.intoEnemiesOffset, Quaternion.identity, false, 1);
								}
								yield return new WaitForSeconds(currentDamageDelay);
								currentDamageDelay *= 0.8f;
								if (damageable != null)
								{
									damageable.Damage(hit);
								}
							}
						}
					}
				}
				IL_42A:;
			}
			yield break;
		}

		[SerializeField]
		[BoxGroup("Attack settings", true, false, 0)]
		private float Radius;

		[SerializeField]
		[BoxGroup("Attack settings", true, false, 0)]
		private LayerMask damageMask;

		[SerializeField]
		[BoxGroup("Effects", true, false, 0)]
		private GameObject prefabIntoPlayer;

		[SerializeField]
		[BoxGroup("Effects", true, false, 0)]
		private GameObject prefabIntoEnemies;

		[SerializeField]
		[BoxGroup("Effects", true, false, 0)]
		private Vector2 intoPlayerOffset;

		[SerializeField]
		[BoxGroup("Effects", true, false, 0)]
		private Vector2 intoEnemiesOffset;

		[SerializeField]
		[BoxGroup("Effects", true, false, 0)]
		private float damageDelay;

		[SerializeField]
		[BoxGroup("Damage", true, false, 0)]
		private DamageArea.DamageType DamageType;

		[SerializeField]
		[BoxGroup("Damage", true, false, 0)]
		private DamageArea.DamageElement DamageElement;

		[SerializeField]
		[BoxGroup("Damage", true, false, 0)]
		private float Amount;

		[SerializeField]
		[BoxGroup("Damage", true, false, 0)]
		private float Force;

		[SerializeField]
		[BoxGroup("Damage", true, false, 0)]
		private bool Unnavoidable;

		[BoxGroup("Damage", true, false, 0)]
		public AnimationCurve slowTimeCurve;

		[BoxGroup("Damage", true, false, 0)]
		public float slowTimeDuration;

		[SerializeField]
		[BoxGroup("Audio", true, false, 0)]
		[EventRef]
		private string HitSound;

		private Hit attackHit;
	}
}
