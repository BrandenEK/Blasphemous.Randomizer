using System;
using Gameplay.GameControllers.Environment;
using UnityEngine;

namespace Gameplay.UI.Others.UIGameEditor
{
	[ExecuteInEditMode]
	public class ClimbPreview : MonoBehaviour
	{
		private void Start()
		{
			if (Application.isPlaying)
			{
				this.preview.enabled = false;
			}
			else if (!Application.isPlaying)
			{
				this.preview.flipX = (this.cliffLede.cliffLedeSide == CliffLede.CliffLedeSide.Left);
			}
		}

		private void Update()
		{
			if (!Application.isPlaying && this.preview != null)
			{
				this.preview.flipX = (this.cliffLede.cliffLedeSide == CliffLede.CliffLedeSide.Left);
			}
		}

		public SpriteRenderer preview;

		public CliffLede cliffLede;
	}
}
