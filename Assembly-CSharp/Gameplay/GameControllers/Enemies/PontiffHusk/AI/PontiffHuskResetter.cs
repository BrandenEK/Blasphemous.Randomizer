using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.PontiffHusk.AI
{
	public class PontiffHuskResetter : MonoBehaviour
	{
		public IEnumerator Start()
		{
			yield return new WaitForSeconds(1f);
			this.huskies = new List<PontiffHuskRangedBehaviour>(Object.FindObjectsOfType<PontiffHuskRangedBehaviour>());
			yield break;
		}

		public void ResetState()
		{
			this.huskies.ForEach(delegate(PontiffHuskRangedBehaviour x)
			{
				x.ResetState();
			});
		}

		private List<PontiffHuskRangedBehaviour> huskies = new List<PontiffHuskRangedBehaviour>();
	}
}
