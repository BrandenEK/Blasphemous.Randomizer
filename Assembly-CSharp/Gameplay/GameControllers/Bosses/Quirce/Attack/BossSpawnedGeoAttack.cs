using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Managers;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.Weapon;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.Quirce.Attack
{
	public class BossSpawnedGeoAttack : Weapon, IDirectAttack
	{
		protected override void OnAwake()
		{
			base.OnAwake();
			this._bottomHits = new RaycastHit2D[1];
			this.SetupSprite(AmanecidasFightSpawner.Instance.amanecidaFight);
		}

		protected override void OnStart()
		{
			base.OnStart();
		}

		private List<BossSpawnedGeoAttack.AmanecidaRockSprites> GetListFromFightNumber(int fn)
		{
			if (fn == 0)
			{
				return this.nightSprites;
			}
			if (fn < 2)
			{
				return this.dawnSprites;
			}
			return this.daySprites;
		}

		public void SetupSprite(int fightNumber)
		{
			List<BossSpawnedGeoAttack.AmanecidaRockSprites> listFromFightNumber = this.GetListFromFightNumber(fightNumber);
			if (listFromFightNumber.Count <= 0)
			{
				return;
			}
			int index = Random.Range(0, listFromFightNumber.Count);
			BossSpawnedGeoAttack.AmanecidaRockSprites amanecidaRockSprites = listFromFightNumber[index];
			GameObject dustVFX = amanecidaRockSprites.dustVFX;
			PoolManager.Instance.CreatePool(dustVFX, 1);
			foreach (AmanecidaSpike amanecidaSpike in this.spikes)
			{
				amanecidaSpike.dustPrefab = dustVFX;
			}
			this.topSpriteRenderer.sprite = amanecidaRockSprites.topSprite;
			this.bodySpriteRenderer.sprite = amanecidaRockSprites.bodySprite;
		}

		public override void Attack(Hit weapondHit)
		{
		}

		public void SpawnGeo(float delay = 0f, float heightPercentage = 1f)
		{
			foreach (AmanecidaSpike amanecidaSpike in this.spikes)
			{
				amanecidaSpike.Show(0.5f, delay, heightPercentage);
			}
			this.DelayedHide(delay + this.duration);
		}

		private void DelayedHide(float delay)
		{
			base.StartCoroutine(this.HideAll(delay));
		}

		private IEnumerator HideAll(float delay)
		{
			yield return new WaitForSeconds(delay);
			foreach (AmanecidaSpike amanecidaSpike in this.spikes)
			{
				if (amanecidaSpike != null)
				{
					amanecidaSpike.Hide();
				}
			}
			yield return new WaitForSeconds(delay);
			this.Recycle();
			yield break;
		}

		public override void OnHit(Hit weaponHit)
		{
		}

		private void SnapToGround()
		{
			Collider2D[] componentsInChildren = base.GetComponentsInChildren<Collider2D>();
			foreach (Collider2D collider2D in componentsInChildren)
			{
				collider2D.enabled = false;
			}
			Vector2 vector = base.transform.position;
			bool flag = Physics2D.LinecastNonAlloc(vector, vector + Vector2.down * this.RangeGroundDetection, this._bottomHits, this.groundMask) > 0;
			if (flag)
			{
				base.transform.position += Vector3.down * this._bottomHits[0].distance;
			}
			foreach (Collider2D collider2D2 in componentsInChildren)
			{
				collider2D2.enabled = true;
			}
		}

		public override void OnObjectReuse()
		{
			base.OnObjectReuse();
			if (this.snapToGround)
			{
				this.SnapToGround();
			}
		}

		public void Recycle()
		{
			base.Destroy();
		}

		private void OnDestroy()
		{
			base.StopAllCoroutines();
		}

		public void CreateHit()
		{
		}

		public void SetDamage(int damage)
		{
		}

		public List<BossSpawnedGeoAttack.AmanecidaRockSprites> nightSprites;

		public List<BossSpawnedGeoAttack.AmanecidaRockSprites> dawnSprites;

		public List<BossSpawnedGeoAttack.AmanecidaRockSprites> daySprites;

		public SpriteRenderer topSpriteRenderer;

		public SpriteRenderer bodySpriteRenderer;

		[FoldoutGroup("Collision settings", 0)]
		public bool snapToGround;

		[FoldoutGroup("Collision settings", 0)]
		public LayerMask groundMask;

		[FoldoutGroup("Collision settings", 0)]
		public float RangeGroundDetection = 2f;

		public List<AmanecidaSpike> spikes;

		public float duration = 2f;

		private RaycastHit2D[] _bottomHits;

		[Serializable]
		public struct AmanecidaRockSprites
		{
			public Sprite topSprite;

			public Sprite bodySprite;

			public GameObject dustVFX;
		}
	}
}
