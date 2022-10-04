using System;
using FMODUnity;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Tools.Level.Actionables
{
	[SelectionBase]
	public class ImpacteableObject : MonoBehaviour, IDamageable
	{
		public void Damage(Hit hit = default(Hit))
		{
			Core.Audio.PlaySfx(this.onImpactSound, 0f);
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

		[SerializeField]
		[BoxGroup("Audio Settings", true, false, 0)]
		[EventRef]
		private string onImpactSound;

		[FoldoutGroup("Damage Settings", 0)]
		public bool bleedOnImpact;

		[FoldoutGroup("Damage Settings", 0)]
		public bool sparksOnImpact = true;

		[SerializeField]
		[BoxGroup("Attached References", true, false, 0)]
		private DamageArea damageArea;
	}
}
