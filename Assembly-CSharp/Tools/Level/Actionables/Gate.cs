using System;
using FMODUnity;
using Framework.FrameworkCore;
using Framework.Managers;
using Sirenix.OdinInspector;
using Tools.Gameplay;
using UnityEngine;

namespace Tools.Level.Actionables
{
	public class Gate : PersistentObject, IActionable
	{
		private SpriteRenderer SpriteRenderer { get; set; }

		private void Awake()
		{
			this.open = false;
			this.SpriteRenderer = base.GetComponentInChildren<SpriteRenderer>();
			if (this.startOpen)
			{
				this.Open(true, false, true);
			}
		}

		private void Open(bool doOpen, bool playAudio = true, bool instaAction = false)
		{
			if (!this.animator)
			{
				return;
			}
			this.animator.SetBool(Gate.InstaAction, instaAction);
			this.animator.SetBool(Gate.OpenParam, doOpen);
			this.collision.enabled = !doOpen;
			this.open = doOpen;
			if (playAudio && !instaAction)
			{
				this.PlayAudio(this.open);
			}
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

		private void PlayAudio(bool isOpen)
		{
			if (!this.SpriteRenderer)
			{
				return;
			}
			if (this.SpriteRenderer.isVisible)
			{
				Core.Audio.PlaySfx((!isOpen) ? this.closeSound : this.openSound, 0f);
			}
		}

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		private Animator animator;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		private Collider2D collision;

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

		private bool open;

		private static readonly int InstaAction = Animator.StringToHash("INSTA_ACTION");

		private static readonly int OpenParam = Animator.StringToHash("OPEN");
	}
}
