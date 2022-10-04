using System;
using FMODUnity;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tools.Level.Actionables
{
	[SelectionBase]
	public class BreakableObject : PersistentObject, IActionable, IDamageable
	{
		private void Start()
		{
			SceneManager.activeSceneChanged += this.OnActiveSceneChanged;
			LogicManager logic = Core.Logic;
			logic.OnUsePrieDieu = (Core.SimpleEvent)Delegate.Combine(logic.OnUsePrieDieu, new Core.SimpleEvent(this.OnUsePrieDieu));
		}

		private bool IsBroken
		{
			get
			{
				return Core.Logic.BreakableManager.ContainsBreakable(this.GetHashId);
			}
		}

		private void OnUsePrieDieu()
		{
			this.Restore();
		}

		private void OnDestroy()
		{
			SceneManager.activeSceneChanged -= this.OnActiveSceneChanged;
			LogicManager logic = Core.Logic;
			logic.OnUsePrieDieu = (Core.SimpleEvent)Delegate.Remove(logic.OnUsePrieDieu, new Core.SimpleEvent(this.OnUsePrieDieu));
		}

		private void OnActiveSceneChanged(Scene oldScene, Scene newScene)
		{
		}

		public void Damage(Hit hit = default(Hit))
		{
			if (this.animator == null || this.destroyed)
			{
				return;
			}
			if (hit.AttackingEntity != null)
			{
				Transform transform = hit.AttackingEntity.transform;
				this.spriteRenderer.flipX = (this.GetAttackDirection(transform) > 0f);
			}
			Core.Audio.PlaySfx(this.destroySound, 0f);
			this.animator.SetTrigger(this.breakTrigger);
			this.damageArea.DamageAreaCollider.enabled = false;
			this.destroyed = true;
			if (this.OnBreak != null)
			{
				this.OnBreak();
			}
			Core.InventoryManager.OnBreakBreakable(this);
			Core.Logic.BreakableManager.AddBreakable(this.GetHashId);
		}

		private int GetHashId
		{
			get
			{
				return Animator.StringToHash(base.gameObject.name + Core.LevelManager.currentLevel.LevelName);
			}
		}

		public void Restore()
		{
			if (!this.destroyed)
			{
				return;
			}
			this.destroyed = false;
			this.damageArea.DamageAreaCollider.enabled = true;
			this.animator.enabled = true;
			this.spriteRenderer.enabled = true;
			this.animator.Play("idle");
			Core.Logic.BreakableManager.RemoveBreakable(this.GetHashId);
		}

		private void SoftDisable()
		{
			this.damageArea.DamageAreaCollider.enabled = false;
			this.animator.Play("broken");
		}

		private void HardDisable()
		{
			this.damageArea.DamageAreaCollider.enabled = false;
			this.animator.enabled = false;
			this.spriteRenderer.enabled = false;
		}

		public void Use()
		{
			this.Damage(default(Hit));
			if (BreakableObject.OnDead != null)
			{
				BreakableObject.OnDead(base.gameObject);
			}
		}

		private float GetAttackDirection(Transform attacker)
		{
			return Mathf.Sign(attacker.position.x - this.damageArea.transform.position.x);
		}

		public Vector3 GetPosition()
		{
			return base.transform.position;
		}

		public bool BleedOnImpact()
		{
			return this.bleedOnImpact;
		}

		public bool SparkOnImpact()
		{
			return this.sparksOnImpact;
		}

		public bool Locked { get; set; }

		public override PersistentManager.PersistentData GetCurrentPersistentState(string dataPath, bool fullSave)
		{
			BreakableObject.BreakablePersistenceData breakablePersistenceData = base.CreatePersistentData<BreakableObject.BreakablePersistenceData>();
			breakablePersistenceData.broken = this.destroyed;
			return breakablePersistenceData;
		}

		public override void SetCurrentPersistentState(PersistentManager.PersistentData data, bool isloading, string dataPath)
		{
			BreakableObject.BreakablePersistenceData breakablePersistenceData = (BreakableObject.BreakablePersistenceData)data;
			this.destroyed = breakablePersistenceData.broken;
			if (this.IsBroken)
			{
				if (this.SoftDisableWhenBroken)
				{
					this.SoftDisable();
				}
				else
				{
					this.HardDisable();
				}
			}
			else
			{
				this.Restore();
			}
		}

		public static Core.ObjectEvent OnDead;

		public Core.SimpleEvent OnBreak;

		[SerializeField]
		[BoxGroup("Audio Settings", true, false, 0)]
		[EventRef]
		private string destroySound;

		[SerializeField]
		[BoxGroup("Animation", true, false, 0)]
		private string breakTrigger = "BROKEN";

		[FoldoutGroup("Damage Settings", 0)]
		public bool bleedOnImpact;

		[FoldoutGroup("Damage Settings", 0)]
		public bool sparksOnImpact = true;

		[SerializeField]
		[BoxGroup("Attached References", true, false, 0)]
		private SpriteRenderer spriteRenderer;

		[SerializeField]
		[BoxGroup("Attached References", true, false, 0)]
		private Animator animator;

		[SerializeField]
		[BoxGroup("Attached References", true, false, 0)]
		private DamageArea damageArea;

		private bool destroyed;

		public bool SoftDisableWhenBroken;

		private class BreakablePersistenceData : PersistentManager.PersistentData
		{
			public BreakablePersistenceData(string id) : base(id)
			{
			}

			public bool broken;
		}
	}
}
