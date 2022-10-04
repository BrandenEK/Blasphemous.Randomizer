using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LevelEditor
{
	public class ChainHook : MonoBehaviour
	{
		[Button("Update Chain", ButtonSizes.Small)]
		private void UpdateChainLink()
		{
			this.CleanupChain();
			this.CreateChainLink();
			this.ApplyMotorToFirstLink();
		}

		private void CleanupChain()
		{
			int childCount = base.transform.childCount;
			for (int i = 0; i < childCount; i++)
			{
				UnityEngine.Object.DestroyImmediate(base.transform.GetChild(0).gameObject);
			}
		}

		private void CreateChainLink()
		{
			Vector2 vector = Vector2.zero;
			Rigidbody2D component = base.GetComponent<Rigidbody2D>();
			for (int i = 0; i < this.numLinks; i++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.chainLinkPrefab, base.transform);
				gameObject.transform.localPosition = vector;
				gameObject.name = string.Format("Chain Link #{0:00}", i);
				vector += Vector2.down * this.linkVerticalOffset;
				gameObject.GetComponent<HingeJoint2D>().connectedBody = component;
				component = gameObject.GetComponent<Rigidbody2D>();
			}
			if (this.lastLinkPrefab != null)
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(this.lastLinkPrefab, base.transform);
				gameObject2.transform.localPosition = vector + Vector2.down * this.lastLinkVerticalOffset;
				gameObject2.name = "Last Chain Link";
				gameObject2.GetComponent<HingeJoint2D>().connectedBody = component;
			}
		}

		private void ApplyMotorToFirstLink()
		{
			HingeJoint2D component = base.transform.GetChild(0).GetComponent<HingeJoint2D>();
			JointMotor2D motor = component.motor;
			motor.motorSpeed = this.motorSpeed;
			motor.maxMotorTorque = this.maxMotorForce;
			component.motor = motor;
			component.useMotor = true;
		}

		[BoxGroup("Prefabs", true, false, 0)]
		public GameObject chainLinkPrefab;

		[BoxGroup("Prefabs", true, false, 0)]
		public GameObject lastLinkPrefab;

		[BoxGroup("Links properties", true, false, 0)]
		public float linkVerticalOffset = 0.15f;

		[BoxGroup("Links properties", true, false, 0)]
		public float lastLinkVerticalOffset = -1.3f;

		[BoxGroup("Links properties", true, false, 0)]
		[Range(1f, 50f)]
		public int numLinks = 5;

		[BoxGroup("First Link Motor", true, false, 0)]
		public float motorSpeed = 10f;

		[BoxGroup("First Link Motor", true, false, 0)]
		public float maxMotorForce = 10f;
	}
}
