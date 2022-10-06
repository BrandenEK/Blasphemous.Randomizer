using System;
using System.Collections;
using System.Diagnostics;
using DG.Tweening;
using FMODUnity;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Tools.Level.Actionables
{
	public class TriggerBasedTrap : MonoBehaviour, IActionable, IDamageable
	{
		public bool Locked { get; set; }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<TriggerBasedTrap> OnUsedEvent;

		private void Start()
		{
			PoolManager.Instance.CreatePool(this.toInstantiate, 1);
		}

		protected virtual void OnUsed()
		{
			if (this.maxDelay == 0f)
			{
				this.ActivateTrap();
			}
			else
			{
				this.lastDelay = Random.Range(this.minDelay, this.maxDelay);
				base.StartCoroutine(this.DelayedActivateTrap(this.lastDelay));
			}
			if (this.OnUsedEvent != null)
			{
				this.OnUsedEvent(this);
			}
		}

		private IEnumerator DelayedActivateTrap(float s)
		{
			yield return new WaitForSeconds(s);
			this.ActivateTrap();
			yield break;
		}

		private void ActivateTrap()
		{
			ShockwaveArea component = PoolManager.Instance.ReuseObject(this.toInstantiate, base.transform.position + this.offset, Quaternion.identity, false, 1).GameObject.GetComponent<ShockwaveArea>();
			if (this.animator != null)
			{
				this.animator.SetTrigger("ATTACK");
			}
			this.lastArea = component.GetComponentInChildren<Collider2D>();
			this._cdCounter = this.cooldown;
			this.currentState = TriggerBasedTrap.TRIGGER_TRAP_STATES.ACTIVE;
		}

		public void Use()
		{
			this.OnUsed();
		}

		private void Update()
		{
			if (this.currentState == TriggerBasedTrap.TRIGGER_TRAP_STATES.ACTIVE)
			{
				this.UpdateActive();
			}
			else if (this.currentState == TriggerBasedTrap.TRIGGER_TRAP_STATES.CHARGING)
			{
				this.UpdateCharging();
			}
		}

		private void UpdateActive()
		{
			if (this._cdCounter > 0f)
			{
				this._cdCounter -= Time.deltaTime;
			}
			else
			{
				this.currentState = TriggerBasedTrap.TRIGGER_TRAP_STATES.IDLE;
			}
		}

		private void UpdateCharging()
		{
			if (this._chargeCounter > 0f)
			{
				this._chargeCounter -= Time.deltaTime;
			}
			else
			{
				this.Use();
			}
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.red;
			Gizmos.DrawSphere(base.transform.position + this.offset, 0.1f);
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (this.currentState != TriggerBasedTrap.TRIGGER_TRAP_STATES.IDLE || collision == this.lastArea)
			{
				return;
			}
			TrapTriggererArea component = collision.gameObject.GetComponent<TrapTriggererArea>();
			if (component != null && component.triggerID == this.triggerID)
			{
				this.Use();
			}
		}

		public void Damage(Hit hit)
		{
			if (this.currentState == TriggerBasedTrap.TRIGGER_TRAP_STATES.IDLE && this.reactToPlayer)
			{
				this.currentState = TriggerBasedTrap.TRIGGER_TRAP_STATES.CHARGING;
				this._chargeCounter = this.chargeTime;
				ShortcutExtensions.DOPunchScale(base.transform, Vector3.one * 0.2f, this.chargeTime, 10, 1f);
			}
			Core.Audio.PlayOneShot(this.OnHitSound, default(Vector3));
		}

		public Vector3 GetPosition()
		{
			return base.transform.position;
		}

		private void OnDestroy()
		{
			base.StopAllCoroutines();
		}

		public bool BleedOnImpact()
		{
			return false;
		}

		public bool SparkOnImpact()
		{
			return true;
		}

		public GameObject toInstantiate;

		public Animator animator;

		public string triggerID;

		public Vector2 offset;

		public string playerLayer = "Water";

		public bool reactToPlayer = true;

		[SerializeField]
		[BoxGroup("Audio", true, false, 0)]
		[EventRef]
		protected string OnHitSound;

		private Collider2D lastArea;

		public TriggerBasedTrap.TRIGGER_TRAP_STATES currentState;

		public float cooldown = 1f;

		private float _cdCounter;

		public float chargeTime = 0.5f;

		private float _chargeCounter;

		public float minDelay;

		public float maxDelay;

		public float lastDelay;

		private const string ATTACK_TRIGGER = "ATTACK";

		public enum TRIGGER_TRAP_STATES
		{
			IDLE,
			CHARGING,
			ACTIVE
		}
	}
}
