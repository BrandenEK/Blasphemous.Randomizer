using System;
using Sirenix.Serialization;
using UnityEngine;

namespace Framework.Map
{
	[Serializable]
	public class CellKey
	{
		public CellKey(CellKey other)
		{
			this.X = other.X;
			this.Y = other.Y;
		}

		public CellKey(int x, int y)
		{
			this.X = x;
			this.Y = y;
		}

		[OdinSerialize]
		public int X { get; private set; }

		[OdinSerialize]
		public int Y { get; private set; }

		public override int GetHashCode()
		{
			string text = this.X.ToString() + "|" + this.Y.ToString();
			return text.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as CellKey);
		}

		public bool Equals(CellKey obj)
		{
			return obj != null && obj.X == this.X && obj.Y == this.Y;
		}

		public Vector2Int GetVector2()
		{
			return new Vector2Int(this.X, this.Y);
		}

		public override string ToString()
		{
			return string.Format("({0}, {1})", this.X, this.Y);
		}
	}
}
