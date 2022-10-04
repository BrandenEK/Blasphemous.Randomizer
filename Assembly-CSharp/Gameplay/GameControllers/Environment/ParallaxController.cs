using System;
using UnityEngine;

namespace Gameplay.GameControllers.Environment
{
	[ExecuteInEditMode]
	public class ParallaxController : MonoBehaviour
	{
		public void Start()
		{
			if (this.target == null && Application.isPlaying)
			{
				this.target = Camera.main.transform;
			}
			this.oldTargetPosition = base.transform.position;
			this.prePixelPerfect = new Vector3[this.layers.Length];
			for (int i = 0; i < this.prePixelPerfect.Length; i++)
			{
				if (this.layers[i].layer != null)
				{
					this.prePixelPerfect[i] = this.layers[i].layer.transform.position;
				}
			}
		}

		public void LateUpdate()
		{
			if (this.target != null)
			{
				this.MoveLayers();
			}
		}

		private void MoveLayers()
		{
			Vector3 vector = this.target.position - this.oldTargetPosition;
			this.oldTargetPosition = this.target.position;
			for (int i = 0; i < this.layers.Length; i++)
			{
				float num = Mathf.Floor(this.layers[i].speed * (float)this.gridSize) / (float)this.gridSize;
				GameObject layer = this.layers[i].layer;
				float x = vector.x * num * this.influenceX;
				float y = vector.y * num * this.influenceY;
				if (layer != null)
				{
					layer.transform.position = this.prePixelPerfect[i];
					layer.transform.Translate(x, y, 0f);
					this.prePixelPerfect[i] = layer.transform.position;
					this.PixelPerfectPosition(layer.transform);
				}
			}
		}

		private void PixelPerfectPosition(Transform transform)
		{
			float x = Mathf.Floor(transform.position.x * (float)this.gridSize) / (float)this.gridSize;
			float y = Mathf.Floor(transform.position.y * (float)this.gridSize) / (float)this.gridSize;
			transform.position = new Vector3(x, y, transform.position.z);
		}

		public ParallaxData[] Layers
		{
			get
			{
				return this.layers;
			}
		}

		public Transform target;

		public int gridSize = 32;

		[SerializeField]
		private ParallaxData[] layers;

		private Vector3 oldTargetPosition;

		[SerializeField]
		[Range(0f, 1f)]
		private float influenceX;

		[SerializeField]
		[Range(0f, 1f)]
		private float influenceY;

		private Vector3[] prePixelPerfect;
	}
}
