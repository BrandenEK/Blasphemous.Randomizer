using System;
using System.Linq;
using Framework.Managers;
using Tools.DataContainer;
using UnityEngine;

namespace Gameplay.GameControllers.Penitent.InputSystem
{
	public class EntityRumble : MonoBehaviour
	{
		public void UsePreset(string presetId)
		{
			try
			{
				EntityRumble.RumblePreset rumblePreset = this.RumblePresets.Single((EntityRumble.RumblePreset x) => x.Id.Equals(presetId));
				Core.Input.ApplyRumble(rumblePreset.RumbleAsset);
			}
			catch (ArgumentNullException value)
			{
				Console.WriteLine(value);
			}
			catch (InvalidOperationException value2)
			{
				Console.WriteLine(value2);
			}
		}

		[SerializeField]
		public EntityRumble.RumblePreset[] RumblePresets;

		[Serializable]
		public struct RumblePreset
		{
			public RumblePreset(string id, RumbleData rumbleAsset)
			{
				this.Id = id;
				this.RumbleAsset = rumbleAsset;
			}

			public string Id;

			public RumbleData RumbleAsset;
		}
	}
}
