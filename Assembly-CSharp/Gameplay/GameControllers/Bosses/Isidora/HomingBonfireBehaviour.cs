using System;
using DG.Tweening;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Entities.StateMachine;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.Isidora
{
	public class HomingBonfireBehaviour : EnemyBehaviour
	{
		private HomingBonfire HomingBonfire { get; set; }

		private StateMachine StateMachine { get; set; }

		public override void OnAwake()
		{
			base.OnAwake();
			this.StateMachine = base.GetComponent<StateMachine>();
			this.IsSpawningIsidora = true;
		}

		public override void OnStart()
		{
			base.OnStart();
			this.HomingBonfire = (HomingBonfire)this.Entity;
			this.SwitchToState(HomingBonfireBehaviour.HomingBonfireStates.Idle);
			this.HaloMaskGameObject.SetActive(false);
			this.RampLight.SetActive(false);
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			switch (this.CurrentState)
			{
			case HomingBonfireBehaviour.HomingBonfireStates.Idle:
				this.Idle();
				break;
			case HomingBonfireBehaviour.HomingBonfireStates.Attack:
				this.Attack();
				break;
			case HomingBonfireBehaviour.HomingBonfireStates.ChargeIsidora:
				this.ChargeIsidora();
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		public void ActivateBonfire(bool changeMask, float animDelay = 1f, float enlargeDelay = 0f)
		{
			Sequence sequence = DOTween.Sequence();
			TweenSettingsExtensions.AppendInterval(sequence, animDelay);
			TweenSettingsExtensions.OnComplete<Sequence>(sequence, delegate()
			{
				this.SetActive(true, false);
			});
			TweenExtensions.Play<Sequence>(sequence);
			if (changeMask)
			{
				this.HaloMaskGameObject.transform.localScale = Vector3.one * 0.01f;
				this.RampLight.transform.localScale = Vector3.one * 0.01f;
				this.HaloMaskGameObject.SetActive(true);
				this.RampLight.SetActive(true);
				Ease ease = 9;
				Ease ease2 = 9;
				float num = 2f;
				float num2 = 1f;
				Sequence sequence2 = DOTween.Sequence();
				TweenSettingsExtensions.SetDelay<Sequence>(sequence2, enlargeDelay);
				TweenSettingsExtensions.Append(sequence2, TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOScale(this.HaloMaskGameObject.transform, new Vector3(2.4f, 2.5f, 1f), num), ease));
				TweenSettingsExtensions.AppendInterval(sequence2, 2f);
				TweenSettingsExtensions.Append(sequence2, TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOScale(this.HaloMaskGameObject.transform, Vector3.zero, num2), ease2));
				TweenExtensions.Play<Sequence>(sequence2);
				Sequence sequence3 = DOTween.Sequence();
				TweenSettingsExtensions.SetDelay<Sequence>(sequence3, enlargeDelay);
				TweenSettingsExtensions.Append(sequence3, TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOScale(this.RampLight.transform, new Vector3(2.5f, 2.5f, 1f), num), ease));
				TweenSettingsExtensions.AppendInterval(sequence3, 2.5f);
				TweenSettingsExtensions.Append(sequence3, TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOScale(this.RampLight.transform, Vector3.zero, num2), ease2));
				TweenExtensions.Play<Sequence>(sequence3);
			}
		}

		public void StartChargingIsidora(float timeToMaxRate)
		{
			if (!this.IsActive)
			{
				this.ActivateBonfire(true, 1f, 0f);
			}
			this.IsChargingIsidora = true;
			this.BonfireAttack.ChargesIsidora = true;
			this.TimeToMaxRate = timeToMaxRate;
		}

		public void EnlargeMask()
		{
			this.IsFullyCharged = true;
			this.SetActive(true, false);
			ShortcutExtensions.DOKill(this.HaloMaskGameObject.transform, false);
			ShortcutExtensions.DOKill(this.RampLight.transform, false);
			float num = 10f;
			TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOScale(this.HaloMaskGameObject.transform, Vector3.one * 100f, num), 5), delegate()
			{
				this.HaloMaskGameObject.transform.localScale = Vector3.one * 100f;
			});
			TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOScale(this.RampLight.transform, Vector3.one * 100f, num), 5), delegate()
			{
				this.RampLight.transform.localScale = Vector3.one * 100f;
			});
		}

		public void DeactivateBonfire(bool changeMask, bool explode = false)
		{
			this.IsFullyCharged = (this.IsFullyCharged && !changeMask);
			this.SetActive(false, explode);
			if (changeMask)
			{
				ShortcutExtensions.DOKill(this.HaloMaskGameObject.transform, false);
				ShortcutExtensions.DOKill(this.RampLight.transform, false);
				float num = (!explode) ? 0f : 2f;
				float num2 = 1f;
				TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetDelay<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOScale(this.HaloMaskGameObject.transform, Vector3.one * 0.01f, num2), 5), num), delegate()
				{
					this.HaloMaskGameObject.SetActive(false);
				});
				TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetDelay<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOScale(this.RampLight.transform, Vector3.one * 0.01f, num2), 5), num), delegate()
				{
					this.RampLight.SetActive(false);
				});
			}
		}

		public void SetupAttack(int numProjectiles, Vector2 castingPosition, float horizontalSpacingFactor, float verticalSpacingFactor)
		{
			this.SetupAttack(this.AttackCooldown, numProjectiles, true, castingPosition, horizontalSpacingFactor, verticalSpacingFactor);
		}

		public void SetupAttack(float attackCooldown, int numProjectiles, bool useCastingPosition, Vector2 castingPosition, float horizontalSpacingFactor, float verticalSpacingFactor)
		{
			this.AttackCooldown = attackCooldown;
			this.BonfireAttack.NumProjectiles = numProjectiles;
			this.BonfireAttack.UseCastingPosition = useCastingPosition;
			this.BonfireAttack.CastingPosition = castingPosition;
			this.BonfireAttack.HorizontalSpacingFactor = horizontalSpacingFactor;
			this.BonfireAttack.VerticalSpacingFactor = verticalSpacingFactor;
		}

		public void SetActive(bool active, bool explode = false)
		{
			if (active)
			{
				this.ActivateHalfCharged();
				if (this.IsFullyCharged)
				{
					this.ActivateFullCharged();
				}
			}
			else
			{
				this.DeactivateHalfCharged();
				this.DeactivateFullCharged();
			}
			if (explode)
			{
				this.ActivateExplode();
			}
			this.IsActive = active;
		}

		public void FireProjectile()
		{
			this.BonfireAttack.FireProjectile();
		}

		public void ActivateHalfCharged()
		{
			this.HomingBonfire.AnimationInyector.SetParamHalf(true);
		}

		public void ActivateFullCharged()
		{
			this.HomingBonfire.AnimationInyector.SetParamFull(true);
		}

		public void DeactivateHalfCharged()
		{
			this.HomingBonfire.AnimationInyector.SetParamHalf(false);
		}

		public void DeactivateFullCharged()
		{
			this.HomingBonfire.AnimationInyector.SetParamFull(false);
		}

		public void ActivateExplode()
		{
			this.HomingBonfire.AnimationInyector.SetParamExplode();
		}

		public override void Idle()
		{
			if (!this.IsActive || this.IsSpawningIsidora)
			{
				return;
			}
			if (this.IsChargingIsidora)
			{
				this.SwitchToState(HomingBonfireBehaviour.HomingBonfireStates.ChargeIsidora);
			}
			else
			{
				this.SwitchToState(HomingBonfireBehaviour.HomingBonfireStates.Attack);
			}
		}

		public override void Attack()
		{
			if (this.IsActive && !this.IsChargingIsidora)
			{
				return;
			}
			if (this.IsChargingIsidora)
			{
				this.SwitchToState(HomingBonfireBehaviour.HomingBonfireStates.ChargeIsidora);
			}
			else
			{
				this.SwitchToState(HomingBonfireBehaviour.HomingBonfireStates.Idle);
			}
		}

		public void ChargeIsidora()
		{
			if (this.IsChargingIsidora && this.IsActive)
			{
				return;
			}
			if (this.IsActive)
			{
				this.SwitchToState(HomingBonfireBehaviour.HomingBonfireStates.Attack);
			}
			else
			{
				this.SwitchToState(HomingBonfireBehaviour.HomingBonfireStates.Idle);
			}
		}

		private void SwitchToState(HomingBonfireBehaviour.HomingBonfireStates targetState)
		{
			this.CurrentState = targetState;
			if (targetState != HomingBonfireBehaviour.HomingBonfireStates.Idle)
			{
				if (targetState != HomingBonfireBehaviour.HomingBonfireStates.Attack)
				{
					if (targetState == HomingBonfireBehaviour.HomingBonfireStates.ChargeIsidora)
					{
						this.StateMachine.SwitchState<HomingBonfireChargeIsidoraState>();
					}
				}
				else
				{
					this.StateMachine.SwitchState<HomingBonfireAttackState>();
				}
			}
			else
			{
				this.StateMachine.SwitchState<HomingBonfireIdleState>();
			}
		}

		public bool IsAnyProjectileVisible()
		{
			return this.BonfireAttack.IsAnyProjectileActive();
		}

		public override void Damage()
		{
		}

		public override void Wander()
		{
		}

		public override void Chase(Transform targetPosition)
		{
		}

		public override void StopMovement()
		{
		}

		[FoldoutGroup("Attack Settings", 0)]
		public float AttackCooldown;

		[FoldoutGroup("Attack Settings", 0)]
		public HomingBonfireAttack BonfireAttack;

		[FoldoutGroup("Halo GameObjects", 0)]
		public GameObject HaloMaskGameObject;

		[FoldoutGroup("Halo GameObjects", 0)]
		public GameObject RampLight;

		[FoldoutGroup("Charge Settings", 0)]
		public AnimationCurve ChargeRate;

		[HideInInspector]
		public float TimeToMaxRate;

		[FoldoutGroup("Debug", 0)]
		public bool IsSpawningIsidora;

		[FoldoutGroup("Debug", 0)]
		public bool IsActive;

		[FoldoutGroup("Debug", 0)]
		public bool IsChargingIsidora;

		[FoldoutGroup("Debug", 0)]
		public bool IsFullyCharged;

		private HomingBonfireBehaviour.HomingBonfireStates CurrentState;

		private enum HomingBonfireStates
		{
			Idle,
			Attack,
			ChargeIsidora
		}
	}
}
