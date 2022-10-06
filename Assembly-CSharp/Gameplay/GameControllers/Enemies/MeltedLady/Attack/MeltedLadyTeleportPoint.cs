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
				float num = position.x + Random.Range(-this.RndHorizontalTeleportPosition, this.RndHorizontalTeleportPosition);
				float num2 = position.y + Random.Range(-this.RndVerticalTeleportPosition, this.RndVerticalTeleportPosition);
				return new Vector3(num, num2, position.z);
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
			float num2 = radius * Mathf.Cos(num);
			float num3 = radius * Mathf.Sin(num);
			Vector3 vector = base.transform.position + new Vector3(num2, num3);
			Vector3 vector2 = vector;
			for (num = 0.1f; num < 6.2831855f; num += 0.1f)
			{
				num2 = radius * Mathf.Cos(num);
				num3 = radius * Mathf.Sin(num);
				Vector3 vector3 = base.transform.position + new Vector3(num2, num3);
				Gizmos.DrawLine(vector, vector3);
				vector = vector3;
			}
			Gizmos.DrawLine(vector, vector2);
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
