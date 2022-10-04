using System;
using Gameplay.GameControllers.Entities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.MeltedLady.Attack
{
	public class MeltedLadyTeleportPoint : SpawnPoint
	{
		public Vector3 TeleportPosition
		{
			get
			{
				Vector3 position = base.transform.position;
				float x = position.x + UnityEngine.Random.Range(-this.RndHorizontalTeleportPosition, this.RndHorizontalTeleportPosition);
				float y = position.y + UnityEngine.Random.Range(-this.RndVerticalTeleportPosition, this.RndVerticalTeleportPosition);
				return new Vector3(x, y, position.z);
			}
		}

		private void OnDrawGizmos()
		{
			Gizmos.DrawIcon(base.transform.position, "Blasphemous/MeltedLadySpawnReference.png", true);
			if (this.EnableGizmoReference)
			{
				this.DrawCircle(this.RndHorizontalTeleportPosition, Color.magenta);
			}
		}

		private void DrawCircle(float radius, Color color)
		{
			Gizmos.color = color;
			float num = 0f;
			float x = radius * Mathf.Cos(num);
			float y = radius * Mathf.Sin(num);
			Vector3 vector = base.transform.position + new Vector3(x, y);
			Vector3 to = vector;
			for (num = 0.1f; num < 6.2831855f; num += 0.1f)
			{
				x = radius * Mathf.Cos(num);
				y = radius * Mathf.Sin(num);
				Vector3 vector2 = base.transform.position + new Vector3(x, y);
				Gizmos.DrawLine(vector, vector2);
				vector = vector2;
			}
			Gizmos.DrawLine(vector, to);
		}

		[FoldoutGroup("Attack Settings", true, 0)]
		[Range(0f, 2f)]
		public float RndHorizontalTeleportPosition;

		[FoldoutGroup("Attack Settings", true, 0)]
		[Range(0f, 2f)]
		public float RndVerticalTeleportPosition;

		public bool EnableGizmoReference;
	}
}
