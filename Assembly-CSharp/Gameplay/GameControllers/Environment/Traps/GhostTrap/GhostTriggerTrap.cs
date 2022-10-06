using System;
using FMODUnity;
using Framework.Managers;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Environment.Traps.GhostTrap
{
	public class GhostTriggerTrap : MonoBehaviour
	{
		private BoxCollider2D TrapCollider { get; set; }

		private GameObject Target { get; set; }

		private bool TargetIsInsideTrap { get; set; }

		private Vector3 OldSpawnPosition { get; set; }

		private void Awake()
		{
			this.TrapCollider = base.GetComponent<BoxCollider2D>();
		}

		private void Start()
		{
			if (this.GhostTriggerPrefab != null)
			{
				PoolManager.Instance.CreatePool(this.GhostTriggerPrefab, this.NumPages * 5);
			}
		}

		private void Update()
		{
			if (this.TargetIsInsideTrap)
			{
				this._currentTriggerLapse -= Time.deltaTime;
				float num = Vector2.Distance(this.Target.transform.position, this.OldSpawnPosition);
				if (this._currentTriggerLapse <= 0f && num > this._distanceOffset)
				{
					this.SpawnFlyingPages(this.Target.transform.position);
				}
			}
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (this.GhostTriggerPrefab == null)
			{
				return;
			}
			if ((this.TargetMask.value & 1 << other.gameObject.layer) <= 0)
			{
				return;
			}
			this.Target = other.gameObject;
			this.TargetIsInsideTrap = true;
			Vector3 position = this.Target.transform.position;
			this.SpawnFlyingPages(position);
		}

		private void OnTriggerExit2D(Collider2D other)
		{
			if ((this.TargetMask.value & 1 << other.gameObject.layer) <= 0)
			{
				return;
			}
			this.TargetIsInsideTrap = false;
		}

		public void SpawnFlyingPages(Vector3 position)
		{
			Vector3 position2 = this.GetSpawnPagesPosition(position) + this.Offset;
			this.OldSpawnPosition = new Vector2(position2.x, position2.y);
			this._currentTriggerLapse = this.TriggerTimeOffset;
			int num = Random.Range(1, this.NumPages + 1);
			this.PlayBlowPaperSheets();
			byte b = 0;
			while ((int)b < num)
			{
				PoolManager.Instance.ReuseObject(this.GhostTriggerPrefab, position2, Quaternion.identity, false, 1);
				b += 1;
			}
		}

		private Vector3 GetSpawnPagesPosition(Vector3 targetPosition)
		{
			float num = this.TrapCollider.bounds.min.x + 0.1f;
			float num2 = this.TrapCollider.bounds.max.x - 0.1f;
			targetPosition.x = Mathf.Clamp(targetPosition.x, num, num2);
			return targetPosition;
		}

		private void PlayBlowPaperSheets()
		{
			if (string.IsNullOrEmpty(this.BlowPaperSheetFx))
			{
				return;
			}
			Core.Audio.PlaySfx(this.BlowPaperSheetFx, 0f);
		}

		public LayerMask TargetMask;

		public GameObject GhostTriggerPrefab;

		public int NumPages = 3;

		public Vector2 Offset;

		public float TriggerTimeOffset = 0.1f;

		private float _currentTriggerLapse;

		private float _distanceOffset = 0.3f;

		[FoldoutGroup("Audio", 0)]
		[EventRef]
		public string BlowPaperSheetFx;
	}
}
