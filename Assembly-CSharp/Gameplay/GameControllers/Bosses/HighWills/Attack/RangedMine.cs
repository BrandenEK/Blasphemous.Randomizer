using System;
using System.Collections;
using DG.Tweening;
using FMODUnity;
using Framework.Managers;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.Weapon;
using Gameplay.GameControllers.Penitent;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.HighWills.Attack
{
	public class RangedMine : Weapon, IDamageable, IDirectAttack
	{
		public BoxCollider2D Collider { get; set; }

		public Animator Animator { get; set; }

		public AttackArea AttackArea { get; set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			this.AttackArea = base.GetComponentInChildren<AttackArea>();
			this.Collider = base.GetComponent<BoxCollider2D>();
			this.Animator = base.GetComponentInChildren<Animator>();
		}

		protected override void OnStart()
		{
			base.OnStart();
			this.mineHit = this.GetHit();
			this.initialHeight = base.transform.position.y;
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			if (!this.startTweenDone)
			{
				return;
			}
			this.currentLifeTime += Time.deltaTime;
			if (this.currentLifeTime > this.MaxLifeTime)
			{
				this.ExplodeMine();
				return;
			}
			if (this.CreatesLinkEffect)
			{
				bool flag = this.PriorMine != null && this.PriorMine.gameObject.activeInHierarchy;
				this.EffectRenderer.enabled = flag;
				if (flag)
				{
					this.EffectRenderer.transform.position = Vector3.Lerp(this.PriorMine.transform.position, base.transform.position, 0.5f);
					float x = Vector3.Distance(this.PriorMine.transform.position, base.transform.position);
					this.EffectRenderer.size = new Vector2(x, this.EffectRenderer.size.y);
					Vector2 vector = (this.EffectRenderer.transform.position - base.transform.position).normalized;
					float z = 180f + 57.29578f * Mathf.Atan2(vector.y, vector.x);
					this.EffectRenderer.transform.rotation = Quaternion.Euler(0f, 0f, z);
				}
			}
			if (this.horTween == null)
			{
				float num = Mathf.Lerp(this.MinMaxHorSpeed.x, this.MinMaxHorSpeed.y, UnityEngine.Random.Range(0f, 1f));
				this.horTween = base.transform.DOMoveX(base.transform.position.x + num, 1f, false).OnComplete(delegate
				{
					this.horTween = null;
				}).SetEase(Ease.Linear);
			}
			if (this.verTween == null)
			{
				float endValue = this.initialHeight + this.MaxRelativeHeight;
				float duration = this.VerMovementTime * 0.5f;
				if (base.transform.position.y < this.initialHeight)
				{
					endValue = this.initialHeight + this.MaxRelativeHeight;
					duration = this.VerMovementTime;
				}
				else if (base.transform.position.y > this.initialHeight)
				{
					endValue = this.initialHeight + this.MinRelativeHeight;
					duration = this.VerMovementTime;
				}
				this.verTween = base.transform.DOMoveY(endValue, duration, false).OnComplete(delegate
				{
					this.verTween = null;
				}).SetEase(Ease.InOutQuad);
			}
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.CompareTag("Penitent"))
			{
				Penitent componentInParent = other.GetComponentInParent<Penitent>();
				if (componentInParent.Status.Unattacable)
				{
					this.ForcedAttackToTarget(componentInParent, this.mineHit);
					this.ExplodeMine();
				}
				else
				{
					this.Attack(this.mineHit);
					this.ExplodeMine();
				}
			}
		}

		private void OnEnable()
		{
			this.Collider.enabled = true;
			this.GotDestroyed = false;
			this.startTweenDone = false;
			base.StartCoroutine(this.WaitAndDoStartMove());
		}

		private IEnumerator WaitAndDoStartMove()
		{
			yield return new WaitForSeconds(0.2f);
			this.currentLifeTime = 0f;
			base.transform.DOMove(base.transform.position + this.TargetPositionAfterSpawn.localPosition, 0.4f, false).OnComplete(delegate
			{
				this.startTweenDone = true;
			}).SetEase(Ease.OutBack);
			yield break;
		}

		public void SetPriorMine(RangedMine mine)
		{
			if (mine == null)
			{
				return;
			}
			this.PriorMine = mine;
			mine.NextMine = this;
		}

		public override void Attack(Hit weapondHit)
		{
			base.GetDamageableEntities();
			base.AttackDamageableEntities(weapondHit);
		}

		public override void OnHit(Hit weaponHit)
		{
		}

		private void ForcedAttackToTarget(Penitent penitent, Hit rootAttack)
		{
			penitent.DamageArea.TakeDamage(rootAttack, true);
		}

		private Hit GetHit()
		{
			return new Hit
			{
				AttackingEntity = base.gameObject,
				DamageAmount = this.DamageAmount,
				DamageType = DamageArea.DamageType.Normal,
				HitSoundId = this.HitSound
			};
		}

		public void Damage(Hit hit)
		{
			this.Life -= hit.DamageAmount;
			if (this.Life > 0f)
			{
				return;
			}
			this.DestroyMine();
		}

		public Vector3 GetPosition()
		{
			return base.transform.position;
		}

		public void DestroyMine()
		{
			Core.Audio.EventOneShotPanned(this.DestroyedSound, base.transform.position);
			this.GotDestroyed = true;
			if (this.Collider.enabled)
			{
				this.Collider.enabled = false;
			}
			this.Animator.SetTrigger("DESTROY");
		}

		public void ExplodeMine()
		{
			if (this.Collider.enabled)
			{
				this.Collider.enabled = false;
			}
			this.Animator.SetTrigger("EXPLODE");
		}

		public bool BleedOnImpact()
		{
			return false;
		}

		public bool SparkOnImpact()
		{
			return true;
		}

		public void CreateHit()
		{
		}

		public void SetDamage(int damage)
		{
			this.mineHit.DamageAmount = (float)damage;
			this.DamageAmount = (float)damage;
		}

		[SerializeField]
		[BoxGroup("Audio", true, false, 0)]
		[EventRef]
		public string HitSound;

		[SerializeField]
		[BoxGroup("Audio", true, false, 0)]
		[EventRef]
		public string DestroyedSound;

		public Transform TargetPositionAfterSpawn;

		[HideInInspector]
		public RangedMine PriorMine;

		[HideInInspector]
		public RangedMine NextMine;

		public bool CreatesLinkEffect;

		[ShowIf("CreatesLinkEffect", true)]
		public SpriteRenderer EffectRenderer;

		[MinMaxSlider(0f, 10f, true)]
		public Vector2 MinMaxHorSpeed;

		public float VerMovementTime = 1f;

		public float MaxRelativeHeight = 2f;

		public float MinRelativeHeight = -2f;

		private Hit mineHit;

		public float Life = 1f;

		public float DamageAmount;

		public bool GotDestroyed;

		public float MaxLifeTime = 20f;

		private bool startTweenDone;

		private Tween horTween;

		private Tween verTween;

		private float initialHeight;

		private float currentLifeTime;
	}
}
