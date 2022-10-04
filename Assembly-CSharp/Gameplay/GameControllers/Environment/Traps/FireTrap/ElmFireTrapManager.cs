using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Managers;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Environment.Traps.FireTrap
{
	[RequireComponent(typeof(ElmFireTrap))]
	public class ElmFireTrapManager : MonoBehaviour
	{
		private void Awake()
		{
			this._elmFireTrap = base.GetComponent<ElmFireTrap>();
			if (this._elmFireTrap.linkType != ElmFireTrap.LinkType.Core)
			{
				base.enabled = false;
			}
		}

		private void Start()
		{
			this.CheckForUniqueChainCore(new Action(this.SubscribeCycleFinishEvent));
			if (this.AutoLoops)
			{
				this.InstantHideElmFireTraps();
				this.StartLoopCoroutine();
			}
			float damage = (!Core.GameModeManager.IsCurrentMode(GameModeManager.GAME_MODES.NEW_GAME_PLUS)) ? this.DamageForNG : this.DamageForNGPlus;
			this.RecursiveSetUpTrapDamage(damage);
		}

		public void RecursiveSetUpTrapDamage(float damage)
		{
			this._elmFireTrap.LightningPrefab.GetComponentInChildren<BeamAttack>().lightningHit.DamageAmount = damage;
			this.RecursiveSetUpTrapDamage(this._elmFireTrap, damage);
		}

		private void RecursiveSetUpTrapDamage(ElmFireTrap elmFireTrap, float damage)
		{
			elmFireTrap.Attack.ProximityHitAttack.DamageAmount = damage;
			elmFireTrap.SetDamage(damage);
			if (elmFireTrap.target != null)
			{
				this.RecursiveSetUpTrapDamage(elmFireTrap.target, damage);
				if (elmFireTrap.hasMoreTargets)
				{
					foreach (ElmFireTrap elmFireTrap2 in elmFireTrap.additionalTargets)
					{
						this.RecursiveSetUpTrapDamage(elmFireTrap2, damage);
					}
				}
			}
		}

		private void StartLoopCoroutine()
		{
			base.StartCoroutine(this.LoopCoroutine(this.WaitTimeToStart, this.WaitTimeToShowEachTrap, this.WaitTimeBeforeStartCharging, this.ChargingTime, this.WaitTimeBeforeStartHiding, this.WaitTimeToHideEachTrap));
			this.OnCycleFinish = (Action)Delegate.Remove(this.OnCycleFinish, new Action(this.StartLoopCoroutine));
			this.OnCycleFinish = (Action)Delegate.Combine(this.OnCycleFinish, new Action(this.StartLoopCoroutine));
		}

		private void SubscribeCycleFinishEvent()
		{
			foreach (ElmFireTrap elmFireTrap in this.elmFireTrapNodes)
			{
				if (!elmFireTrap.target)
				{
					this._cycleFinisherTrap = elmFireTrap;
					ElmFireTrap cycleFinisherTrap = this._cycleFinisherTrap;
					cycleFinisherTrap.OnCycleFinished = (Core.SimpleEvent)Delegate.Combine(cycleFinisherTrap.OnCycleFinished, new Core.SimpleEvent(this.OnCycleFinished));
					break;
				}
			}
		}

		private void CheckForUniqueChainCore(Action callbackAction)
		{
			int num = 0;
			foreach (ElmFireTrap elmFireTrap in this.elmFireTrapNodes)
			{
				if (elmFireTrap.linkType == ElmFireTrap.LinkType.Core)
				{
					num++;
				}
			}
			if (num == 1 && callbackAction != null)
			{
				callbackAction();
			}
			else
			{
				Debug.LogError("There isn't ONE core trap in the chain!");
			}
		}

		private void OnEnableTrapPropertyChange()
		{
			if (this.enableTraps)
			{
				this.EnableTraps();
			}
			else
			{
				this.DisableTraps();
			}
		}

		public void EnableTraps()
		{
			foreach (ElmFireTrap elmFireTrap in this.elmFireTrapNodes)
			{
				elmFireTrap.enabled = true;
			}
		}

		public void DisableTraps()
		{
			foreach (ElmFireTrap elmFireTrap in this.elmFireTrapNodes)
			{
				elmFireTrap.enabled = false;
			}
		}

		private void OnCycleFinished()
		{
			this._elmFireTrap.ResetTrapCycle();
		}

		private void OnDestroy()
		{
			if (this._cycleFinisherTrap)
			{
				ElmFireTrap cycleFinisherTrap = this._cycleFinisherTrap;
				cycleFinisherTrap.OnCycleFinished = (Core.SimpleEvent)Delegate.Remove(cycleFinisherTrap.OnCycleFinished, new Core.SimpleEvent(this.OnCycleFinished));
			}
			this.OnCycleFinish = (Action)Delegate.Remove(this.OnCycleFinish, new Action(this.StartLoopCoroutine));
		}

		public IEnumerator LoopCoroutine(float waitTimeToStart, float waitTimeToShowEachTrap, float waitTimeBeforeStartCharging, float chargingTime, float waitTimeBeforeStartHiding, float waitTimeToHideEachTrap)
		{
			yield return new WaitForSeconds(waitTimeToStart);
			this.ShowElmFireTrapRecursively(this.elmFireTrapNodes[0], waitTimeToShowEachTrap, chargingTime, true);
			yield return new WaitForSeconds(waitTimeBeforeStartCharging);
			this.elmFireTrapNodes[0].SetCurrentCycleCooldownToMax();
			this.EnableTraps();
			yield return new WaitForSeconds(waitTimeBeforeStartHiding);
			this.DisableTraps();
			this.HideElmFireTrapRecursively(this.elmFireTrapNodes[0], waitTimeToHideEachTrap);
			yield return new WaitUntil(() => this.ElmFireLoopEndReached);
			if (this.OnCycleFinish != null)
			{
				this.OnCycleFinish();
			}
			yield break;
		}

		public void InstantHideElmFireTraps()
		{
			this.DisableTraps();
			this.InstantHideElmFireTrapRecursively(this._elmFireTrap);
		}

		private void InstantHideElmFireTrapRecursively(ElmFireTrap elmFireTrap)
		{
			if (elmFireTrap.Collider)
			{
				elmFireTrap.Collider.enabled = false;
			}
			elmFireTrap.Animator.Play("Hidden");
			if (elmFireTrap.target != null)
			{
				this.InstantHideElmFireTrapRecursively(elmFireTrap.target);
				if (elmFireTrap.hasMoreTargets)
				{
					foreach (ElmFireTrap elmFireTrap2 in elmFireTrap.additionalTargets)
					{
						this.InstantHideElmFireTrapRecursively(elmFireTrap2);
					}
				}
			}
		}

		public void HideElmFireTrapRecursively(ElmFireTrap elmFireTrap, float waitTime)
		{
			this.ElmFireLoopEndReached = false;
			base.StartCoroutine(this.ShowOrHideTrapRecursiveCoroutine(elmFireTrap, waitTime, "HIDE", "SHOW", -1f, false));
		}

		public void ShowElmFireTrapRecursively(ElmFireTrap elmFireTrap, float waitTime, float lightningChargeLapse, bool applyChargingTimeToAll)
		{
			this.ElmFireLoopEndReached = false;
			base.StartCoroutine(this.ShowOrHideTrapRecursiveCoroutine(elmFireTrap, waitTime, "SHOW", "HIDE", lightningChargeLapse, applyChargingTimeToAll));
		}

		private IEnumerator ShowOrHideTrapRecursiveCoroutine(ElmFireTrap elmFireTrap, float waitTime, string triggerNameToSet, string triggerNameToReset, float chargingTime = -1f, bool applyChargingTimeToAll = false)
		{
			if (chargingTime >= 0f)
			{
				elmFireTrap.chargingTime = chargingTime;
			}
			if (!applyChargingTimeToAll)
			{
				chargingTime = -1f;
			}
			elmFireTrap.Animator.SetTrigger(triggerNameToSet);
			elmFireTrap.Animator.ResetTrigger(triggerNameToReset);
			if (elmFireTrap.target != null)
			{
				yield return new WaitForSeconds(waitTime);
				base.StartCoroutine(this.ShowOrHideTrapRecursiveCoroutine(elmFireTrap.target, waitTime, triggerNameToSet, triggerNameToReset, chargingTime, applyChargingTimeToAll));
				if (elmFireTrap.hasMoreTargets)
				{
					foreach (ElmFireTrap elmFireTrap2 in elmFireTrap.additionalTargets)
					{
						base.StartCoroutine(this.ShowOrHideTrapRecursiveCoroutine(elmFireTrap2, waitTime, triggerNameToSet, triggerNameToReset, chargingTime, applyChargingTimeToAll));
					}
				}
			}
			else
			{
				this.ElmFireLoopEndReached = true;
			}
			yield break;
		}

		public List<ElmFireTrap> elmFireTrapNodes;

		public bool AutoLoops;

		[ShowIf("AutoLoops", true)]
		public float WaitTimeToStart;

		[ShowIf("AutoLoops", true)]
		public float WaitTimeToShowEachTrap;

		[ShowIf("AutoLoops", true)]
		public float WaitTimeBeforeStartCharging;

		[ShowIf("AutoLoops", true)]
		public float ChargingTime;

		[ShowIf("AutoLoops", true)]
		public float WaitTimeBeforeStartHiding;

		[ShowIf("AutoLoops", true)]
		public float WaitTimeToHideEachTrap;

		public float DamageForNG;

		public float DamageForNGPlus;

		[HideInInspector]
		public bool ElmFireLoopEndReached;

		[SerializeField]
		[OnValueChanged("OnEnableTrapPropertyChange", false)]
		private bool enableTraps = true;

		private ElmFireTrap _elmFireTrap;

		private ElmFireTrap _cycleFinisherTrap;

		private Action OnCycleFinish;
	}
}
