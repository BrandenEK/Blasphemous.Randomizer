using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Audio
{
	public class AudioCatalog : MonoBehaviour
	{
		public void Initialize()
		{
			this.indexPlaylists();
			this.indexSfx();
			this.indexSongs();
		}

		private void indexSfx()
		{
			this.sfxDictionary = new Dictionary<string, AudioCatalog.IndexedClipList>();
			for (int i = 0; i < this.SoundEffects.Length; i++)
			{
				this.sfxDictionary.Add(this.SoundEffects[i].Id, this.SoundEffects[i]);
			}
		}

		public AudioCatalog.IndexedClipList GetSfx(string Id)
		{
			AudioCatalog.IndexedClipList result = null;
			this.sfxDictionary.TryGetValue(Id, out result);
			return result;
		}

		private void indexSongs()
		{
			this.songsDictionary = new Dictionary<string, AudioCatalog.IndexedClip>();
			for (int i = 0; i < this.Songs.Length; i++)
			{
				this.songsDictionary.Add(this.Songs[i].Id, this.Songs[i]);
			}
		}

		public AudioCatalog.IndexedClip GetSong(string Id)
		{
			AudioCatalog.IndexedClip result = null;
			this.songsDictionary.TryGetValue(Id, out result);
			return result;
		}

		private void indexPlaylists()
		{
			this.playlistDictionary = new Dictionary<string, AudioCatalog.PlaylistDef>();
			for (int i = 0; i < this.Playlists.Length; i++)
			{
				this.playlistDictionary.Add(this.Playlists[i].Id, this.Playlists[i]);
			}
		}

		public AudioCatalog.PlaylistDef GetPlaylist(string Id)
		{
			AudioCatalog.PlaylistDef result = null;
			this.playlistDictionary.TryGetValue(Id, out result);
			return result;
		}

		public AudioCatalog.IndexedClipList[] SoundEffects;

		public AudioCatalog.IndexedClip[] Songs;

		public AudioCatalog.PlaylistDef[] Playlists;

		private Dictionary<string, AudioCatalog.IndexedClipList> sfxDictionary;

		private Dictionary<string, AudioCatalog.IndexedClip> songsDictionary;

		private Dictionary<string, AudioCatalog.PlaylistDef> playlistDictionary;

		[Serializable]
		public class IndexedClip
		{
			public override string ToString()
			{
				return this.Id;
			}

			public AudioClip Clip;

			public string Id;
		}

		[Serializable]
		public class IndexedClipList
		{
			public override string ToString()
			{
				return this.Id;
			}

			public AudioClip[] Clips;

			public string Id;

			public bool InmediateCC;
		}

		[Serializable]
		public class PlaylistDef
		{
			public override string ToString()
			{
				return this.Id;
			}

			public string Id;

			[NonSerialized]
			public int Index;

			public string[] Songs;
		}
	}
}
