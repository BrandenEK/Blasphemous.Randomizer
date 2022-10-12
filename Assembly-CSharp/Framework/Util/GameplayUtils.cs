using System;
using System.Collections;
using System.Collections.Generic;
using Gameplay.GameControllers.Enemies.BellGhost;
using UnityEngine;

namespace Framework.Util
{
	public static class GameplayUtils
	{
		public static Vector2 GetGroundPosition(Vector2 pos, LayerMask layerMask, out bool groundExists)
		{
			RaycastHit2D[] array = new RaycastHit2D[1];
			float d = 100f;
			Vector2 vector = pos;
			groundExists = (Physics2D.LinecastNonAlloc(vector, vector - Vector2.up * d, array, layerMask) > 0);
			if (groundExists)
			{
				pos = array[0].point;
			}
			return pos;
		}

		public static void DrawDebugCross(Vector2 point, Color c, float seconds)
		{
			float d = 0.6f;
			Debug.DrawLine(point - Vector2.up * d, point + Vector2.up * d, c, seconds);
			Debug.DrawLine(point - Vector2.right * d, point + Vector2.right * d, c, seconds);
		}

		public static IEnumerator LerpMoveWithCurveCoroutine(Transform t, Vector3 origin, Vector3 target, AnimationCurve animationCurve, float seconds, Action<Transform> endCallback = null, Action<float> loopCallback = null)
		{
			for (float counter = 0f; counter < seconds; counter += Time.deltaTime)
			{
				float normalizedValue = animationCurve.Evaluate(counter / seconds);
				Vector3 p = Vector3.Lerp(origin, target, normalizedValue);
				t.position = p;
				if (loopCallback != null)
				{
					loopCallback(normalizedValue);
				}
				yield return null;
			}
			if (loopCallback != null)
			{
				loopCallback(1f);
			}
			t.position = Vector3.Lerp(origin, target, 1f);
			if (endCallback != null)
			{
				endCallback(t);
			}
			yield break;
		}

		public static void DestroyAllProjectiles()
		{
			List<ProjectileWeapon> list = new List<ProjectileWeapon>(UnityEngine.Object.FindObjectsOfType<ProjectileWeapon>());
			foreach (ProjectileWeapon projectileWeapon in list)
			{
				projectileWeapon.ForceDestroy();
			}
		}
	}
}
