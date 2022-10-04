using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.Snake
{
	public class SnakeBeamExplosionsCoordinator : MonoBehaviour
	{
		public void PlayBeams()
		{
			this.Animators.ForEach(delegate(Animator x)
			{
				x.SetBool("BEAM", true);
			});
		}

		public void StopBeams()
		{
			this.Animators.ForEach(delegate(Animator x)
			{
				x.SetBool("BEAM", false);
			});
		}

		public void PlayWarning()
		{
			this.Animators.ForEach(delegate(Animator x)
			{
				x.SetTrigger("WARNING");
			});
		}

		public List<Animator> Animators;
	}
}
