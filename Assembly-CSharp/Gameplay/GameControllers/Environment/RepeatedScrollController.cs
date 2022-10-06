using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.GameControllers.Environment
{
	public class RepeatedScrollController : MonoBehaviour
	{
		private void Awake()
		{
		}

		public void Start()
		{
			if (this.targetReference == null && Application.isPlaying)
			{
				this.targetReference = Camera.main.transform;
			}
			this.oldTargetPosition = this.targetReference.position;
			int num = Mathf.CeilToInt(this.scrollArea.x / this.elementSize.x) + 1;
			this.elements = new Transform[num];
			float num2 = this.targetReference.position.x - this.scrollArea.x * 0.5f + this.elementSize.x * 0.5f;
			for (int i = 0; i < num; i++)
			{
				this.elements[i] = Object.Instantiate<Transform>(this.elementToRepeat, base.transform);
				Vector3 position = this.elements[i].position;
				position.x = num2 + (float)i * this.elementSize.x;
				this.elements[i].position = position;
			}
			this.elementToRepeat.gameObject.SetActive(false);
		}

		public void LateUpdate()
		{
			if (this.targetReference != null)
			{
				this.Move();
			}
		}

		private void Move()
		{
			Vector3 vector = this.targetReference.position - this.oldTargetPosition;
			this.oldTargetPosition = this.targetReference.position;
			float num = vector.x * (float)this.speed * this.influenceX;
			for (int i = 0; i < this.elements.Length; i++)
			{
				this.elements[i].Translate(num, 0f, 0f);
			}
			this.CheckBounds();
		}

		private void CheckBounds()
		{
			Vector2 vector = this.targetReference.position;
			Rect rect;
			rect..ctor(vector - this.scrollArea * 0.5f, this.scrollArea);
			Vector3 vector2 = this.targetReference.position;
			Vector3 vector3 = this.targetReference.position;
			int num = -1;
			for (int i = 0; i < this.elements.Length; i++)
			{
				vector = this.elements[i].position;
				Rect rect2;
				rect2..ctor(vector - this.elementSize * 0.5f, this.elementSize);
				if (!rect.Overlaps(rect2))
				{
					num = i;
				}
				else
				{
					vector2 = Vector3.Min(vector2, rect2.min);
					vector3 = Vector3.Max(vector3, rect2.max);
				}
			}
			if (num == -1)
			{
				return;
			}
			if (vector2.x > rect.min.x)
			{
				float x = vector2.x - this.elementSize.x * 0.5f;
				vector = this.elements[num].position;
				vector.x = x;
				this.elements[num].position = vector;
			}
			else if (vector3.x < rect.max.x)
			{
				float x2 = vector3.x + this.elementSize.x * 0.5f;
				vector = this.elements[num].position;
				vector.x = x2;
				this.elements[num].position = vector;
			}
			Array.Sort<Transform>(this.elements, this.sort);
		}

		private void PixelPerfectPosition(Transform transform, int ppu)
		{
			float num = Mathf.Floor(transform.position.x * (float)ppu) / (float)ppu;
			float num2 = Mathf.Floor(transform.position.y * (float)ppu) / (float)ppu;
			transform.position = new Vector3(num, num2, transform.position.z);
		}

		private void OnDrawGizmosSelected()
		{
			if (this.targetReference)
			{
				Gizmos.DrawWireCube(this.targetReference.position, this.scrollArea);
			}
			else
			{
				Gizmos.DrawWireCube(base.transform.position, this.scrollArea);
			}
			Gizmos.color = Color.blue;
			if (Application.isPlaying)
			{
				for (int i = 0; i < this.elements.Length; i++)
				{
					Gizmos.DrawWireCube(this.elements[i].position, this.elementSize);
				}
			}
			else if (this.elementToRepeat)
			{
				Gizmos.DrawWireCube(this.elementToRepeat.position, this.elementSize);
			}
		}

		public Transform targetReference;

		public Vector2 scrollArea;

		public int PPU = 32;

		public Transform elementToRepeat;

		public Vector2 elementSize;

		private Vector3 oldTargetPosition;

		[Range(-1f, 1f)]
		public int speed;

		[SerializeField]
		[Range(0f, 1f)]
		private float influenceX;

		private Transform[] elements = new Transform[0];

		private RepeatedScrollController.SortByXCoord sort = new RepeatedScrollController.SortByXCoord();

		public class SortByXCoord : IComparer<Transform>
		{
			public int Compare(Transform t1, Transform t2)
			{
				if (t1.position.x < t2.position.x)
				{
					return -1;
				}
				if (t1.position.x > t2.position.x)
				{
					return 1;
				}
				return 0;
			}
		}
	}
}
