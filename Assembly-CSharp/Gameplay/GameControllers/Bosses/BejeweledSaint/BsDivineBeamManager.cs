using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Managers;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.BejeweledSaint
{
	public class BsDivineBeamManager : SerializedMonoBehaviour
	{
		public DivineBeamOrigin DivineBeamOrigin { get; private set; }

		private void Start()
		{
			this.DivineBeamOrigin = base.GetComponentInChildren<DivineBeamOrigin>();
			PoolManager.Instance.CreatePool(this.DivineBeam, 10);
		}

		private void Shuffle<T>(IList<T> list)
		{
			int i = list.Count;
			while (i > 1)
			{
				i--;
				int index = this.rnd.Next(i + 1);
				T value = list[index];
				list[index] = list[i];
				list[i] = value;
			}
		}

		public void InstantiateDivineBeams()
		{
			this.GenerateDivineBeamPositions();
			this.Shuffle<Vector2>(this._divineBeamPositions);
			base.StartCoroutine(this.InstantiateBeamsCoroutine());
		}

		private IEnumerator InstantiateBeamsCoroutine()
		{
			foreach (Vector2 position in this._divineBeamPositions)
			{
				PoolManager.Instance.ReuseObject(this.DivineBeam, position, Quaternion.identity, false, 1);
				yield return this._beamDelay;
			}
			yield break;
		}

		public void InstantiateSingleBeam(Vector2 pos)
		{
			pos.y = this.DivineBeamOrigin.OriginPosition.y;
			PoolManager.Instance.ReuseObject(this.DivineBeam, pos, Quaternion.identity, false, 1);
		}

		private void GenerateDivineBeamPositions()
		{
			this._originPosition = this.DivineBeamOrigin.OriginPosition;
			this._divineBeamPositions.Clear();
			int num = Random.Range(0, 4);
			float num2 = this._originPosition.x;
			for (int i = 0; i < 19; i++)
			{
				if (this.CustomCellDrawing[i, num])
				{
					Vector2 item;
					item..ctor(num2, this._originPosition.y);
					this._divineBeamPositions.Add(item);
					num2 += 2f;
				}
				else
				{
					num2 += 1f;
				}
			}
		}

		private const int BeamsPatternRows = 4;

		private const int BeamsPatternColumns = 19;

		private readonly WaitForSeconds _beamDelay = new WaitForSeconds(0.2f);

		private readonly List<Vector2> _divineBeamPositions = new List<Vector2>();

		private Vector2 _originPosition;

		[BoxGroup("Divine Beam Patterns", true, false, 0)]
		[TableMatrix(DrawElementMethod = "DrawColoredEnumElement", ResizableColumns = false)]
		public bool[,] CustomCellDrawing = new bool[19, 4];

		public GameObject DivineBeam;

		private readonly Random rnd = new Random();
	}
}
