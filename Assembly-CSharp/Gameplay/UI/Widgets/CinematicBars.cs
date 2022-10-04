using System;
using Framework.FrameworkCore;
using Framework.Managers;
using UnityEngine;

namespace Gameplay.UI.Widgets
{
	[RequireComponent(typeof(Animator))]
	public class CinematicBars : UIWidget
	{
		private void Awake()
		{
			this.animator = base.GetComponent<Animator>();
		}

		public void CinematicMode(bool active)
		{
			Log.Trace("Cinematic", "Cinematic mode is " + active, null);
			this.InCinematicMode = active;
			this.animator.SetBool("SHOW", active);
			Core.UI.GameplayUI.gameObject.SetActive(!active);
			Core.Input.SetBlocker("CINEMATIC_MODE", active);
		}

		public bool InCinematicMode { get; private set; }

		private Animator animator;
	}
}
