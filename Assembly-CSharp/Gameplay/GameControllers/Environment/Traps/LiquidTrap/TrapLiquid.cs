using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Environment.Traps.LiquidTrap
{
	public class TrapLiquid : MonoBehaviour
	{
		private void ClearAllOriginChildren()
		{
			while (this.origin.childCount > 0)
			{
				Object.DestroyImmediate(this.origin.GetChild(0).gameObject);
			}
			this.bodyAnimators.Clear();
		}

		[FoldoutGroup("Design", 0)]
		[Button("AUTO CONFIGURE", 22)]
		public void AutoConfigureLength()
		{
			this.ClearAllOriginChildren();
			int num = Physics2D.RaycastNonAlloc(this.origin.position, Vector2.down, this.results, 100f, this.groundLayerMask);
			if (num > 0)
			{
				Vector2 point = this.results[0].point;
				float num2 = Vector2.Distance(this.origin.position, point);
				int num3 = Mathf.RoundToInt(num2 / this.liquidBodyHeight);
				for (int i = 0; i < num3; i++)
				{
					GameObject gameObject = Object.Instantiate<GameObject>(this.liquidBodyPrefab, this.origin);
					gameObject.transform.position = this.origin.position + Vector3.down * ((float)i * this.liquidBodyHeight);
					this.bodyAnimators.Add(gameObject.GetComponent<Animator>());
				}
			}
		}

		private void CreateBodyParts()
		{
		}

		[Button("ACTIVATE", 0)]
		public void ActivateTrap()
		{
			this.trapanimator.SetBool("ATTACK", true);
			this.TestStartToFall();
			this.TestToWiden();
		}

		[Button("DEACTIVATE", 0)]
		public void DeactivateTrap()
		{
			this.trapanimator.SetBool("ATTACK", false);
			this.TestStartToFall();
			this.TestToWiden();
		}

		[FoldoutGroup("Test animation", 0)]
		[Button(0)]
		public void TestStartToFall()
		{
			this.TriggerAllDelayed("FALL", this.fallDelay);
		}

		[FoldoutGroup("Test animation", 0)]
		[Button(0)]
		public void TestToWiden()
		{
			this.TriggerAllDelayed("WIDEN", this.widenDelay);
		}

		[FoldoutGroup("Test animation", 0)]
		[Button(0)]
		public void TestToStretch()
		{
			this.TriggerAllDelayed("STRETCH", this.stretchDelay);
		}

		public void TriggerAllDelayed(string trigger, float delay)
		{
			for (int i = 0; i < this.bodyAnimators.Count; i++)
			{
				this.SetTriggerDelayed(delay * (float)i, this.bodyAnimators[i], trigger);
			}
		}

		public void SetTriggerDelayed(float seconds, Animator a, string trigger)
		{
			base.StartCoroutine(this.SetTriggerDelayedCoroutine(seconds, a, trigger));
		}

		private IEnumerator SetTriggerDelayedCoroutine(float seconds, Animator a, string trigger)
		{
			yield return new WaitForSeconds(seconds);
			a.SetTrigger(trigger);
			yield break;
		}

		[FoldoutGroup("References", 0)]
		public List<Animator> bodyAnimators;

		[FoldoutGroup("References", 0)]
		public GameObject liquidBodyPrefab;

		[FoldoutGroup("References", 0)]
		public Transform origin;

		[FoldoutGroup("Animation settings", 0)]
		public float fallDelay = 0.3f;

		[FoldoutGroup("Animation settings", 0)]
		public float widenDelay = 0.3f;

		[FoldoutGroup("Animation settings", 0)]
		public float stretchDelay = 0.3f;

		[FoldoutGroup("Animation settings", 0)]
		public float liquidBodyHeight = 1f;

		[FoldoutGroup("Design", 0)]
		public LayerMask groundLayerMask;

		public Animator trapanimator;

		private RaycastHit2D[] results = new RaycastHit2D[1];
	}
}
