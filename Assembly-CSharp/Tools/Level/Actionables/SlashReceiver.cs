using System;
using FMODUnity;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace Tools.Level.Actionables
{
	[RequireComponent(typeof(Collider2D))]
	public class SlashReceiver : PersistentObject, IActionable, IDamageable
	{
		public void Use()
		{
			Hit hit = new Hit
			{
				DamageAmount = 100f
			};
			this.Damage(hit);
		}

		public bool Locked { get; set; }

		public void Damage(Hit hit = default(Hit))
		{
			if (this.receiveHitsFrom != SlashReceiver.DAMAGEABLE_DIRECTION_LOCK.BOTH)
			{
				bool flag = Core.Logic.Penitent.transform.position.x > base.transform.position.x;
				if ((this.receiveHitsFrom == SlashReceiver.DAMAGEABLE_DIRECTION_LOCK.RIGHT && !flag) || (this.receiveHitsFrom == SlashReceiver.DAMAGEABLE_DIRECTION_LOCK.LEFT && flag))
				{
					return;
				}
			}
			this.HitReaction(hit);
			this.PlayHitSoundFx();
		}

		private void HitReaction(Hit hit)
		{
			for (int i = 0; i < this.OnHitUse.Length; i++)
			{
				IActionable[] components = this.OnHitUse[i].GetComponents<IActionable>();
				components.ForEach(delegate(IActionable actionable)
				{
					if (!(actionable is SlashReceiver))
					{
						if (hit.DamageType == DamageArea.DamageType.Heavy && actionable is ActionableForce)
						{
							(actionable as ActionableForce).HeavyUse();
						}
						else
						{
							actionable.Use();
						}
					}
				});
			}
		}

		public Vector3 GetPosition()
		{
			return base.transform.position;
		}

		public bool BleedOnImpact()
		{
			return false;
		}

		public bool SparkOnImpact()
		{
			return true;
		}

		private void PlayHitSoundFx()
		{
			if (string.IsNullOrEmpty(this.HitSoundFx))
			{
				return;
			}
			Core.Audio.PlaySfx(this.HitSoundFx, 0f);
		}

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		public GameObject[] OnHitUse;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		private SlashReceiver.DAMAGEABLE_DIRECTION_LOCK receiveHitsFrom;

		[SerializeField]
		[EventRef]
		[BoxGroup("Audio", true, false, 0)]
		private string HitSoundFx;

		public enum DAMAGEABLE_DIRECTION_LOCK
		{
			BOTH,
			RIGHT,
			LEFT
		}
	}
}
