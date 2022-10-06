using System;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using Tools.Items;
using Tools.Level.Actionables;
using UnityEngine;

namespace Framework.Inventory
{
	public abstract class BaseInventoryObject : MonoBehaviour, ILocalizable
	{
		public bool IsOwned { get; private set; }

		public virtual bool IsEquipable()
		{
			return false;
		}

		public virtual bool AskForPercentageCompletition()
		{
			return false;
		}

		public virtual bool AddPercentageCompletition()
		{
			return false;
		}

		public virtual bool IsDecipherable()
		{
			return false;
		}

		public virtual bool IsDeciphered()
		{
			return true;
		}

		public virtual bool HasLore()
		{
			return true;
		}

		public abstract InventoryManager.ItemType GetItemType();

		public string GetInternalId()
		{
			return this.GetItemType().ToString() + "__" + this.id;
		}

		public void Add()
		{
			this.IsOwned = true;
			base.SendMessage("OnAddInventoryObject", 1);
			string text = this.id + "_OWNED";
			if (this.AddPercentageCompletition() && !Core.Events.GetFlag(text))
			{
				Core.Events.SetFlag(text, true, this.preserveInNewGamePlus);
				switch (this.GetItemType())
				{
				case InventoryManager.ItemType.Relic:
					Core.AchievementsManager.Achievements["AC17"].AddProgressSafeTo99(14.285714f);
					break;
				case InventoryManager.ItemType.Prayer:
					Core.AchievementsManager.Achievements["AC16"].AddProgressSafeTo99(7.6923075f);
					break;
				case InventoryManager.ItemType.Bead:
					Core.AchievementsManager.Achievements["AC18"].AddProgressSafeTo99(3.3333333f);
					break;
				case InventoryManager.ItemType.Collectible:
					Core.AchievementsManager.Achievements["AC19"].AddProgressSafeTo99(2.2727273f);
					Core.AchievementsManager.CheckFlagsToGrantAC19();
					break;
				case InventoryManager.ItemType.Sword:
					Core.AchievementsManager.Achievements["AC20"].AddProgressSafeTo99(11.111111f);
					break;
				}
			}
		}

		public void Remove()
		{
			this.IsOwned = false;
			base.SendMessage("OnRemoveInventoryObject", 1);
		}

		public void Reset()
		{
			this.IsOwned = false;
			base.SendMessage("OnResetInventoryObject", 1);
		}

		public void HitEnemy(Hit hit)
		{
			base.SendMessage("OnHitEnemy", hit, 1);
		}

		public void KillEnemy(Enemy e)
		{
			base.SendMessage("OnKillEnemy", e, 1);
		}

		public void HitReceived(Hit hit)
		{
			base.SendMessage("OnHitReceived", hit, 1);
		}

		public void PenitentHealthChanged(float life)
		{
			base.SendMessage("OnPenitentHealthChanged", life, 1);
		}

		public void BreakableBreak(BreakableObject breakable)
		{
			base.SendMessage("OnBreakBreakable", breakable, 1);
		}

		public void PenitentDead()
		{
			base.SendMessage("OnPenitentDead", 1);
		}

		public void NumberOfCurrentFlasksChanged(float newNumberOfFlasks)
		{
			base.SendMessage("OnNumberOfCurrentFlasksChanged", newNumberOfFlasks, 1);
		}

		public string GetBaseTranslationID()
		{
			return base.GetType().Name + "/" + this.id;
		}

		public bool WillBlockSwords()
		{
			bool result = false;
			foreach (ItemTemporalEffect itemTemporalEffect in base.GetComponents<ItemTemporalEffect>())
			{
				if (itemTemporalEffect.enabled && itemTemporalEffect.ContainsEffect(ItemTemporalEffect.PenitentEffects.DisableUnEquipSword))
				{
					result = true;
					break;
				}
			}
			return result;
		}

		public void RemoveAllObjectEffets()
		{
			foreach (ObjectEffect objectEffect in base.GetComponents<ObjectEffect>())
			{
				if (objectEffect.IsApplied)
				{
					objectEffect.RemoveEffect(true);
				}
			}
		}

		public string id = string.Empty;

		public string caption = string.Empty;

		[TextArea(3, 10)]
		public string description = string.Empty;

		[TextArea(6, 10)]
		public string lore = string.Empty;

		public Sprite picture;

		public bool carryonstart;

		public bool preserveInNewGamePlus;

		private const int TOTAL_NUMBER_OF_PRAYERS_FOR_AC16 = 13;

		private const int TOTAL_NUMBER_OF_RELIC_FOR_AC17 = 7;

		private const int TOTAL_NUMBER_OF_BEADS_FOR_AC18 = 30;

		public const int TOTAL_NUMBER_OF_COLLECTIBLE_FOR_AC19 = 44;

		private const int TOTAL_NUMBER_OF_SWORD_FOR_AC20 = 9;
	}
}
