using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.PontiffOldman
{
	public class PontiffOldmanBossfightPoints : MonoBehaviour
	{
		[Button("Load all lists", ButtonSizes.Large)]
		public void LoadListsFromParents()
		{
			this.repositionPoints.Clear();
			IEnumerator enumerator = this.repositionPointsParent.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					Transform item = (Transform)obj;
					this.repositionPoints.Add(item);
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
		}

		public Transform GetRandomToxicPoint()
		{
			int childCount = this.toxicPointsParent.childCount;
			int index = UnityEngine.Random.Range(0, childCount);
			return this.toxicPointsParent.GetChild(index);
		}

		public Transform GetPointInCenter()
		{
			return this.midPoints[UnityEngine.Random.Range(0, this.midPoints.Count)];
		}

		public Transform GetPointAwayOfPenitent(Vector2 p_position)
		{
			Transform transform = this.repositionPoints[0];
			float num = Vector2.Distance(transform.position, p_position);
			for (int i = 1; i < this.repositionPoints.Count; i++)
			{
				float num2 = Vector2.Distance(this.repositionPoints[i].position, p_position);
				if (num2 > num)
				{
					transform = this.repositionPoints[i];
					num = num2;
				}
			}
			return transform;
		}

		[SerializeField]
		[FoldoutGroup("References", 0)]
		public Transform toxicPointsParent;

		[SerializeField]
		[FoldoutGroup("References", 0)]
		public Transform leftLimitTransform;

		[SerializeField]
		[FoldoutGroup("References", 0)]
		public Transform rightLimitTransform;

		[SerializeField]
		[FoldoutGroup("References", 0)]
		public Transform fightCenterTransform;

		[SerializeField]
		[FoldoutGroup("References", 0)]
		private Transform repositionPointsParent;

		public List<Transform> repositionPoints;

		public List<Transform> midPoints;
	}
}
