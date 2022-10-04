using System;
using System.Collections;
using System.Linq;
using FMODUnity;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using Sirenix.OdinInspector;
using Tools.Level.Interactables;
using UnityEngine;

namespace Gameplay.GameControllers.Combat
{
	public class EntityExecution : Trait
	{
		public Vector2 ExecutionPosition { get; private set; }

		protected override void OnStart()
		{
			base.OnStart();
			if (this.ExecutionPrefab == null)
			{
				Debug.LogError("An Execution Prefab Is Needed!");
			}
		}

		public void InstantiateExecution()
		{
			Core.Audio.PlaySfx(this.ExecutionAwareness, 0f);
			this.execution = UnityEngine.Object.Instantiate<GameObject>(this.ExecutionPrefab, base.EntityOwner.transform.position, Quaternion.identity);
			this.ExecutionPosition = new Vector3(this.execution.transform.position.x, this.execution.transform.position.y);
			this.execution.GetComponentInChildren<Execution>().ExecutedEntity = (base.EntityOwner as Enemy);
			SpriteRenderer spriteRenderer = this.execution.GetComponentsInChildren<SpriteRenderer>().First((SpriteRenderer x) => x.gameObject.CompareTag("Interactable"));
			spriteRenderer.flipX = (Core.Logic.Penitent.transform.position.x < this.execution.transform.position.x);
		}

		[Button(ButtonSizes.Small)]
		public void DestroyExecution()
		{
			if (this.execution == null)
			{
				return;
			}
			base.StartCoroutine(this.DestroyExecutionCoroutine());
		}

		private IEnumerator DestroyExecutionCoroutine()
		{
			Execution e = this.execution.GetComponentInChildren<Execution>();
			yield return new WaitForSeconds(e.SafeTimeThreshold + 0.1f);
			Hit hit = new Hit
			{
				DamageAmount = 100f
			};
			if (e != null)
			{
				e.Damage(hit);
			}
			yield break;
		}

		public GameObject ExecutionPrefab;

		[SerializeField]
		[BoxGroup("Audio Settings", true, false, 0)]
		[EventRef]
		protected string ExecutionAwareness;

		private GameObject execution;
	}
}
