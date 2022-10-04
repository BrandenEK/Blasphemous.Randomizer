using System;
using System.Collections.Generic;
using FMODUnity;
using Gameplay.GameControllers.Entities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Framework.Audio
{
	public class FMODAudioCatalog : MonoBehaviour
	{
		public void Initialize()
		{
			this.IndexSfx();
		}

		private void IndexSfx()
		{
			this._sfxDictionary = new Dictionary<string, FMODAudioCatalog.FMODIndexedClip>();
			for (int i = 0; i < this.SoundEffects.Length; i++)
			{
				this._sfxDictionary.Add(this.SoundEffects[i].Id, this.SoundEffects[i]);
			}
		}

		public FMODAudioCatalog.FMODIndexedClip GetSfx(string id)
		{
			FMODAudioCatalog.FMODIndexedClip result;
			this._sfxDictionary.TryGetValue(id, out result);
			return result;
		}

		[ShowIf("HasAssociatedCatalog", true)]
		public FMODAudioCatalog AssociatedCatalog;

		public bool HasAssociatedCatalog;

		public Entity Owner;

		public FMODAudioCatalog.FMODIndexedClip[] SoundEffects;

		private Dictionary<string, FMODAudioCatalog.FMODIndexedClip> _sfxDictionary;

		[Serializable]
		public class FMODIndexedClip
		{
			public override string ToString()
			{
				return this.Id;
			}

			[EventRef]
			public string FMODKey;

			public string Id;
		}

		[Serializable]
		public class FMODIndexedClipList
		{
			public override string ToString()
			{
				return this.Id;
			}

			public FMODAudioCatalog.FMODIndexedClip[] Clips;

			public string Id;

			public bool InmediateCC;
		}
	}
}
