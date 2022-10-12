using System;
using System.Collections.Generic;
using DG.Tweening;
using Framework.Managers;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Environment.Traps.FireTrap
{
	public class ElmFireTrap : MonoBehaviour
	{
		[HideInInspector]
		public Collider2D Collider { get; set; }

		public Animator Animator { get; set; }

		public ElmFireTrapAttack Attack { get; set; }

		private void Awake()
		{
			if (this.linkType == ElmFireTrap.LinkType.Core)
			{
				this._trapCore = new ElmFireTrapCore(this);
			}
			this.Animator = base.GetComponentInChildren<Animator>();
			this.Collider = base.GetComponentInChildren<CircleCollider2D>();
			this.Attack = base.GetComponent<ElmFireTrapAttack>();
		}

		private void Start()
		{
			this.Animator.Play("Burning", 0, UnityEngine.Random.value);
			if (this.LightningPrefab)
			{
				PoolManager.Instance.CreatePool(this.LightningPrefab.gameObject, 1);
			}
		}

		private void Update()
		{
			if (this._trapCore != null)
			{
				this._trapCore.Update();
			}
			if (this.linkType != ElmFireTrap.LinkType.Static)
			{
				return;
			}
			if (this.Collider.enabled)
			{
				this.currentTimeActive += Time.deltaTime;
				if (this.currentTimeActive > this.maxTimeActive)
				{
					this.currentTimeActive = 0f;
					this.currentTimeInactive = 0f;
					this.Animator.SetTrigger("HIDE");
				}
			}
			else
			{
				this.currentTimeInactive += Time.deltaTime;
				if (this.currentTimeInactive > this.maxTimeInactive)
				{
					this.currentTimeActive = 0f;
					this.currentTimeInactive = 0f;
					this.Animator.SetTrigger("SHOW");
				}
			}
		}

		public void SetCurrentCycleCooldownToMax()
		{
			this._trapCore.SetCurrentCycleCooldownToMax();
		}

		public void AnimEvent_ActivateCollider()
		{
			this.Collider.enabled = true;
		}

		public void AnimEvent_DeactivateCollider()
		{
			this.Collider.enabled = false;
		}

		public void ChargeLightnings()
		{
			if (!this.target)
			{
				if (this.OnCycleFinished != null)
				{
					this.OnCycleFinished();
				}
			}
			else
			{
				TileableBeamLauncher tileableBeam = this.GetTileableBeam(this.target);
				if (!tileableBeam)
				{
					return;
				}
				this.ChargedLightnings.Add(tileableBeam);
				float maxRange = Vector2.Distance(this.target.transform.position, base.transform.position);
				tileableBeam.maxRange = maxRange;
				tileableBeam.ActivateDelayedBeam(0f, true);
				if (this.hasMoreTargets)
				{
					foreach (ElmFireTrap elmFireTrap in this.additionalTargets)
					{
						tileableBeam = this.GetTileableBeam(elmFireTrap);
						if (!tileableBeam)
						{
							return;
						}
						this.ChargedLightnings.Add(tileableBeam);
						maxRange = Vector2.Distance(elmFireTrap.transform.position, base.transform.position);
						tileableBeam.maxRange = maxRange;
						tileableBeam.ActivateDelayedBeam(0f, true);
					}
				}
				this.StartCastLightningsSequence();
			}
		}

		private TileableBeamLauncher GetTileableBeam(ElmFireTrap fireTrap)
		{
			Vector3 position = fireTrap.transform.position;
			Vector3 position2 = (position + base.transform.position) / 2f;
			PoolManager.ObjectInstance objectInstance = PoolManager.Instance.ReuseObject(this.LightningPrefab.gameObject, position2, this.GetLightningRotation(position), false, 1);
			this.lightningBeamAttack = objectInstance.GameObject.GetComponentInChildren<BeamAttack>();
			return objectInstance.GameObject.GetComponent<TileableBeamLauncher>();
		}

		public void SetDamage(float damage)
		{
			if (this.lightningBeamAttack)
			{
				this.lightningBeamAttack.lightningHit.DamageAmount = damage;
			}
		}

		private Quaternion GetLightningRotation(Vector3 targetPos)
		{
			Vector3 vector = targetPos - base.transform.position;
			float z = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
			return Quaternion.Euler(0f, 0f, z);
		}

		private void StartCastLightningsSequence()
		{
			if (!this.target)
			{
				return;
			}
			if (this.OnChargeStart != null)
			{
				this.OnChargeStart(this.chargingTime);
			}
			DOTween.Sequence().SetDelay(this.chargingTime).OnComplete(delegate
			{
				foreach (TileableBeamLauncher tileableBeamLauncher in this.ChargedLightnings)
				{
					tileableBeamLauncher.TriggerBeamBodyAnim();
				}
				this.ChargeTargetLightningsSequence();
				this.ChargedLightnings.Clear();
				if (this.OnLightningCast != null)
				{
					this.OnLightningCast(this.target.gameObject.transform.position);
				}
			});
		}

		private void ChargeTargetLightningsSequence()
		{
			DOTween.Sequence().SetDelay(this.targetLightningChargeTimeout).OnComplete(new TweenCallback(this.ChargeTargetsLightnings));
		}

		private void ChargeTargetsLightnings()
		{
			if (!this.target)
			{
				return;
			}
			this.target.ChargeLightnings();
			if (this.hasMoreTargets)
			{
				foreach (ElmFireTrap elmFireTrap in this.additionalTargets)
				{
					elmFireTrap.ChargeLightnings();
				}
			}
		}

		public void ResetTrapCycle()
		{
			this._trapCore.ResetCycle();
		}

		public Core.SimpleEvent OnCycleFinished;

		public Core.SimpleEventParam OnChargeStart;

		public Core.SimpleEventParam OnLightningCast;

		[FoldoutGroup("Target Settings", 0)]
		public ElmFireTrap.LinkType linkType;

		[FoldoutGroup("Target Settings", 0)]
		[ShowIf("linkType", ElmFireTrap.LinkType.Static, true)]
		public float maxTimeActive;

		[FoldoutGroup("Target Settings", 0)]
		[ShowIf("linkType", ElmFireTrap.LinkType.Static, true)]
		public float maxTimeInactive;

		[FoldoutGroup("Target Settings", 0)]
		[HideIf("linkType", ElmFireTrap.LinkType.Static, true)]
		public ElmFireTrap target;

		[FoldoutGroup("Target Settings", 0)]
		[HideIf("linkType", ElmFireTrap.LinkType.Static, true)]
		public bool hasMoreTargets;

		[FoldoutGroup("Target Settings", 0)]
		[ShowIf("hasMoreTargets", true)]
		public List<ElmFireTrap> additionalTargets;

		[FoldoutGroup("Lightning Settings", 0)]
		[ShowIf("linkType", ElmFireTrap.LinkType.Core, true)]
		public float lightningCycleCooldown;

		[FoldoutGroup("Lightning Settings", 0)]
		[HideIf("linkType", ElmFireTrap.LinkType.Static, true)]
		public float chargingTime;

		[FoldoutGroup("Lightning Settings", 0)]
		[HideIf("linkType", ElmFireTrap.LinkType.Static, true)]
		public float targetLightningChargeTimeout = 1.85f;

		[BoxGroup("Debug", true, false, 0)]
		public bool drawGizmos;

		public TileableBeamLauncher LightningPrefab;

		private ElmFireTrapCore _trapCore;

		private float currentTimeActive;

		private float currentTimeInactive;

		private BeamAttack lightningBeamAttack;

		private List<TileableBeamLauncher> ChargedLightnings = new List<TileableBeamLauncher>();

		public enum LinkType
		{
			Core,
			Joint,
			Static
		}
	}
}
