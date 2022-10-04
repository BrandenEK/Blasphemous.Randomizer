using System;
using System.Collections;
using Framework.FrameworkCore;
using UnityEngine;

namespace Gameplay.GameControllers.Entities.Traits
{
	public class VulnerablePeriodTrait : Trait
	{
		private IEnumerator VulnerablePeriod(Enemy e, float seconds)
		{
			e.IsVulnerable = true;
			float counter = 0f;
			while (counter < seconds)
			{
				counter += Time.deltaTime;
				yield return null;
			}
			e.IsVulnerable = false;
			yield break;
		}

		internal void StartVulnerablePeriod(Enemy e)
		{
			base.StartCoroutine(this.VulnerablePeriod(e, this.vulnerablePeriodDuration));
		}

		public float vulnerablePeriodDuration;
	}
}
