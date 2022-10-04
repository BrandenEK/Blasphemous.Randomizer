using System;
using UnityEngine;

namespace Gameplay.GameControllers.Environment.MovingPlatforms
{
	public class MovingPlatformDestination : MonoBehaviour
	{
		private void Update()
		{
			base.gameObject.SetActive(false);
		}
	}
}
