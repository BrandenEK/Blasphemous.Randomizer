using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.GoldenCorpse.AI
{
	public class GoldenCorpseAwakener : MonoBehaviour
	{
		[Button("GetAllCorpses", 0)]
		public void GetCorpsesInScene()
		{
			this.allCorpses = new List<GoldenCorpseBehaviour>(Object.FindObjectsOfType<GoldenCorpseBehaviour>());
		}

		private void Start()
		{
			this.awakenedOnes = new List<GoldenCorpseBehaviour>();
			this.asleepOnes = new List<GoldenCorpseBehaviour>();
			this.asleepOnes.AddRange(this.allCorpses);
		}

		private void AwakenCorpse(GoldenCorpseBehaviour c)
		{
			c.Awaken();
			this.awakenedOnes.Add(c);
			this.asleepOnes.Remove(c);
		}

		private void BackToSleep(GoldenCorpseBehaviour c)
		{
			c.SleepForever();
			this.awakenedOnes.Remove(c);
			this.asleepOnes.Add(c);
		}

		public void AllBackToSleep()
		{
			foreach (GoldenCorpseBehaviour goldenCorpseBehaviour in this.awakenedOnes)
			{
				goldenCorpseBehaviour.SleepForever();
			}
		}

		[Button("Awaken Random Corpse", 0)]
		public void AwakenRandomCorpse()
		{
			GoldenCorpseBehaviour randomAsleepCorpse = this.GetRandomAsleepCorpse();
			if (randomAsleepCorpse != null)
			{
				this.AwakenCorpse(randomAsleepCorpse);
			}
		}

		private GoldenCorpseBehaviour GetRandomAsleepCorpse()
		{
			if (this.asleepOnes.Count > 0)
			{
				return this.asleepOnes[Random.Range(0, this.asleepOnes.Count)];
			}
			Debug.Log("NOT ASLEEP ONES REMAIN");
			return null;
		}

		public List<GoldenCorpseBehaviour> allCorpses;

		public List<GoldenCorpseBehaviour> awakenedOnes;

		public List<GoldenCorpseBehaviour> asleepOnes;
	}
}
