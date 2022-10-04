using System;
using System.Collections;
using FMODUnity;
using Framework.FrameworkCore;
using Framework.Managers;
using Sirenix.OdinInspector;
using Tools.Gameplay;
using UnityEngine;

namespace Tools.Level.Actionables
{
	public class ActionableLadder : PersistentObject, IActionable
	{
		private void Awake()
		{
			this.open = false;
			this.Open(this.startOpen, false, true);
		}

		private void Open(bool b, bool playAudio = true, bool automatic = false)
		{
			this.open = b;
			if (this.open)
			{
				this.StartGrow((!automatic) ? this.maxSeconds : 0f);
			}
			else
			{
				this.tileableLadder.maxRange = 0f;
			}
			if (playAudio)
			{
				Core.Audio.PlaySfx((!this.open) ? this.closeSound : this.openSound, 0f);
			}
		}

		private IEnumerator GrowCoroutine(float maxLength, float seconds)
		{
			float counter = 0f;
			while (counter < seconds)
			{
				float i = Mathf.Lerp(0f, maxLength, counter / seconds);
				this.tileableLadder.maxRange = i;
				counter += Time.deltaTime;
				yield return null;
			}
			this.tileableLadder.maxRange = maxLength;
			this.collision.enabled = true;
			this.collision.size += new Vector2(0f, maxLength);
			this.collision.offset -= new Vector2(0f, maxLength / 2f);
			yield break;
		}

		private void StartGrow(float seconds)
		{
			base.StartCoroutine(this.GrowCoroutine(this.maxRange, seconds));
		}

		public void Use()
		{
			this.Open(!this.open, true, false);
		}

		public bool Locked { get; set; }

		public override bool IsOpenOrActivated()
		{
			return this.open;
		}

		public override PersistentManager.PersistentData GetCurrentPersistentState(string dataPath, bool fullSave)
		{
			if (!this.persistState)
			{
				return null;
			}
			BasicPersistence basicPersistence = base.CreatePersistentData<BasicPersistence>();
			basicPersistence.triggered = this.open;
			return basicPersistence;
		}

		public override void SetCurrentPersistentState(PersistentManager.PersistentData data, bool isloading, string dataPath)
		{
			if (!this.persistState)
			{
				return;
			}
			BasicPersistence basicPersistence = (BasicPersistence)data;
			this.open = basicPersistence.triggered;
			this.Open(this.open, false, true);
		}

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		private Animator animator;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		private BoxCollider2D collision;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		private bool startOpen;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		private bool persistState;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		[EventRef]
		private string openSound;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		[EventRef]
		private string closeSound;

		public TileableBeamLauncher tileableLadder;

		public float maxSeconds = 1f;

		private bool open;

		public float maxRange = 15f;
	}
}
