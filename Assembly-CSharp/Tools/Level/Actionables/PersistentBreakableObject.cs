using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Effects.Entity;
using Gameplay.GameControllers.Entities;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Tools.Level.Layout;
using UnityEngine;

namespace Tools.Level.Actionables
{
	[RequireComponent(typeof(Collider2D))]
	public class PersistentBreakableObject : PersistentObject, IActionable, IDamageable
	{
		public void Use()
		{
			this.Damage(new Hit
			{
				DamageAmount = 100f
			});
		}

		public bool Locked { get; set; }

		public void Damage(Hit hit = default(Hit))
		{
			if (this.breakableFrom != PersistentBreakableObject.DAMAGEABLE_DIRECTION_LOCK.BOTH)
			{
				bool flag = Core.Logic.Penitent.transform.position.x > base.transform.position.x;
				if ((this.breakableFrom == PersistentBreakableObject.DAMAGEABLE_DIRECTION_LOCK.RIGHT && !flag) || (this.breakableFrom == PersistentBreakableObject.DAMAGEABLE_DIRECTION_LOCK.LEFT && flag))
				{
					return;
				}
			}
			if (this.destroyed)
			{
				return;
			}
			this.HitReaction();
			float num = (!this.DamageByHits) ? hit.DamageAmount : 1f;
			this.health -= num;
			this.SendHealthToAnimator();
			this.CheckDamageEvents((int)this.health);
			if (this.health <= 0f)
			{
				this.destroyed = true;
				if (this.changeLayerOnDestroy)
				{
					base.gameObject.layer = LayerMask.NameToLayer("Floor");
				}
				this.PlayFxSound(this.breakFxSound);
				base.StartCoroutine(this.DestroyWall());
			}
			else
			{
				this.PlayFxSound(this.hitFxSound);
			}
		}

		private void SendHealthToAnimator()
		{
			if (this.animator != null)
			{
				this.animator.SetFloat("HEALTH", this.health);
			}
		}

		private void HitReaction()
		{
			Core.Logic.ScreenFreeze.Freeze(0.01f, 0.05f, 0f, null);
			this.colorFlash.TriggerColorFlash();
		}

		public Vector3 GetPosition()
		{
			return base.transform.position;
		}

		private void Awake()
		{
			this.layoutElement = base.GetComponent<LayoutElement>();
			this.wallCollider = base.GetComponent<Collider2D>();
			this.colorFlash = base.GetComponent<ColorFlash>();
			this.layoutMode = (this.layoutElement.category == Category.Layout);
			this.SendHealthToAnimator();
		}

		private IEnumerator DestroyWall()
		{
			if (this.layoutMode)
			{
				this.SetColor(Color.green);
			}
			Core.Audio.PlaySfxOnCatalog("BREAK_WALL");
			for (int i = 0; i < this.OnDestroy.Length; i++)
			{
				if (!(this.OnDestroy[i] == null))
				{
					IActionable[] components = this.OnDestroy[i].GetComponents<IActionable>();
					LinqExtensions.ForEach<IActionable>(components, delegate(IActionable actionable)
					{
						actionable.Use();
					});
				}
			}
			yield return new WaitForSeconds(this.RemainTimeAfterHit);
			this.wallCollider.enabled = false;
			if (this.layoutMode)
			{
				Color color = this.layoutElement.SpriteRenderer.material.color;
				color.a = 0f;
				this.layoutElement.SpriteRenderer.material.color = color;
			}
			yield break;
		}

		private void SetColor(Color c)
		{
			Color color;
			color..ctor(c.r, c.g, c.b);
			this.layoutElement.SpriteRenderer.material.color = color;
		}

		private void PlayFxSound(string eventId)
		{
			if (string.IsNullOrEmpty(eventId))
			{
				return;
			}
			Core.Audio.PlaySfx(eventId, 0f);
		}

		public bool Destroyed
		{
			get
			{
				return this.destroyed;
			}
		}

		private void CheckDamageEvents(int currentLife)
		{
			if (!this.DamageByHits)
			{
				return;
			}
			foreach (PersistentBreakableObject.DamageEvent damageEvent2 in from damageEvent in this.DamageEvents
			where currentLife == damageEvent.TriggerValue
			select damageEvent)
			{
				if (!StringExtensions.IsNullOrWhitespace(damageEvent2.EventName))
				{
					string text = damageEvent2.EventName.Trim();
					PlayMakerFSM.BroadcastEvent(text);
				}
			}
		}

		public virtual void SetDestroyedState()
		{
			if (this.brokenStateName != string.Empty)
			{
				this.animator.Play(this.brokenStateName);
			}
			if (this.changeLayerOnDestroy)
			{
				base.gameObject.layer = LayerMask.NameToLayer("Floor");
			}
			if (this.layoutMode)
			{
				Color color = this.layoutElement.SpriteRenderer.material.color;
				color.a = 0f;
				this.layoutElement.SpriteRenderer.material.color = color;
			}
		}

		public override PersistentManager.PersistentData GetCurrentPersistentState(string dataPath, bool fullSave)
		{
			PersistentBreakableObject.BreakablePersistentData breakablePersistentData = base.CreatePersistentData<PersistentBreakableObject.BreakablePersistentData>();
			breakablePersistentData.Life = this.health;
			return breakablePersistentData;
		}

		public override void SetCurrentPersistentState(PersistentManager.PersistentData data, bool isloading, string dataPath)
		{
			PersistentBreakableObject.BreakablePersistentData breakablePersistentData = (PersistentBreakableObject.BreakablePersistentData)data;
			this.health = breakablePersistentData.Life;
			this.SendHealthToAnimator();
			this.destroyed = (this.health <= 0f);
			if (this.destroyed)
			{
				this.SetDestroyedState();
			}
			this.wallCollider.enabled = !this.destroyed;
		}

		public bool BleedOnImpact()
		{
			return false;
		}

		public bool SparkOnImpact()
		{
			return true;
		}

		[BoxGroup("Design Settings", true, false, 0)]
		public bool DamageByHits;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		[ShowIf("DamageByHits", true)]
		protected List<PersistentBreakableObject.DamageEvent> DamageEvents;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		public GameObject[] OnDestroy;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		private float health;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		private float RemainTimeAfterHit = 2f;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		private string brokenStateName = string.Empty;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		private PersistentBreakableObject.DAMAGEABLE_DIRECTION_LOCK breakableFrom;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		private bool changeLayerOnDestroy = true;

		[SerializeField]
		[FoldoutGroup("Audio", 0)]
		[EventRef]
		private string hitFxSound;

		[SerializeField]
		[FoldoutGroup("Audio", 0)]
		[EventRef]
		private string breakFxSound;

		private ColorFlash colorFlash;

		private bool destroyed;

		private LayoutElement layoutElement;

		private bool layoutMode;

		private Collider2D wallCollider;

		public Animator animator;

		public enum DAMAGEABLE_DIRECTION_LOCK
		{
			BOTH,
			RIGHT,
			LEFT
		}

		[Serializable]
		public struct DamageEvent
		{
			public string EventName;

			public int TriggerValue;
		}

		private class BreakablePersistentData : PersistentManager.PersistentData
		{
			public BreakablePersistentData(string id) : base(id)
			{
			}

			public float Life;
		}
	}
}
