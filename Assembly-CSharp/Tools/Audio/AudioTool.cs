using System;
using FMODUnity;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Tools.Audio
{
	[ExecuteInEditMode]
	[SelectionBase]
	public class AudioTool : MonoBehaviour
	{
		protected bool IsEmitter { get; set; }

		public bool PlayerInsideTrigger { get; private set; }

		protected virtual void BaseAwake()
		{
		}

		protected virtual void BaseStart()
		{
		}

		protected virtual void BaseDestroy()
		{
		}

		protected virtual void BaseUpdate()
		{
		}

		protected virtual void BaseTriggerEnter2D(Collider2D col)
		{
		}

		protected virtual void BaseTriggerExit2D(Collider2D col)
		{
		}

		protected virtual void BaseDrawGizmos()
		{
		}

		private void Awake()
		{
			this.BaseAwake();
		}

		private void Start()
		{
			this.BaseStart();
		}

		private void OnDestroy()
		{
			this.BaseDestroy();
		}

		private void OnDrawGizmos()
		{
			this.BaseDrawGizmos();
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.CompareTag("Penitent"))
			{
				this.PlayerInsideTrigger = true;
				this.BaseTriggerEnter2D(other);
			}
		}

		private void OnTriggerExit2D(Collider2D other)
		{
			if (other.CompareTag("Penitent"))
			{
				this.PlayerInsideTrigger = false;
				this.BaseTriggerExit2D(other);
			}
		}

		private void Update()
		{
			if (Application.isPlaying)
			{
				this.BaseUpdate();
			}
		}

		[SerializeField]
		[BoxGroup("Audio Settings", true, false, 0)]
		[ShowIf("IsEmitter", true)]
		[EventRef]
		protected string trackIdentifier;
	}
}
