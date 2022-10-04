using System;
using FMODUnity;
using Framework.Inventory;
using Framework.Managers;
using UnityEngine;

namespace Tools.Items
{
	public class ItemAudio : ObjectEffect
	{
		protected override bool OnApplyEffect()
		{
			Core.Audio.PlaySfx(this.audioId, this.delay);
			return true;
		}

		[SerializeField]
		[EventRef]
		private string audioId;

		[SerializeField]
		private float delay;
	}
}
