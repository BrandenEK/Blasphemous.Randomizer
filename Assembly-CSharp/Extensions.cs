using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class Extensions
{
	public static Selectable FindSelectableFromList(this Selectable s, Vector3 dir, List<Selectable> candidates)
	{
		dir = dir.normalized;
		Vector3 b = s.transform.TransformPoint(Extensions.MyGetPointOnRectEdge(s.transform as RectTransform, Quaternion.Inverse(s.transform.rotation) * dir));
		float num = float.NegativeInfinity;
		Selectable result = null;
		for (int i = 0; i < candidates.Count; i++)
		{
			Selectable selectable = candidates[i];
			if (!(selectable == s) && !(selectable == null) && selectable.IsInteractable() && selectable.navigation.mode != Navigation.Mode.None)
			{
				RectTransform rectTransform = selectable.transform as RectTransform;
				Vector3 position = (rectTransform != null) ? rectTransform.rect.center : Vector3.zero;
				Vector3 rhs = selectable.transform.TransformPoint(position) - b;
				float num2 = Vector3.Dot(dir, rhs);
				if ((double)num2 > 0.0)
				{
					float num3 = num2 / rhs.sqrMagnitude;
					if ((double)num3 > (double)num)
					{
						num = num3;
						result = selectable;
					}
				}
			}
		}
		return result;
	}

	private static Vector3 MyGetPointOnRectEdge(RectTransform rect, Vector2 dir)
	{
		if (rect == null)
		{
			return Vector3.zero;
		}
		if (dir != Vector2.zero)
		{
			dir /= Mathf.Max(Mathf.Abs(dir.x), Mathf.Abs(dir.y));
		}
		dir = rect.rect.center + Vector2.Scale(rect.rect.size, dir * 0.5f);
		return dir;
	}

	public static void SafeInvoke(this Action action)
	{
		if (action != null)
		{
			action();
		}
	}

	public static void SafeInvoke<T>(this Action<T> action, T arg)
	{
		if (action != null)
		{
			action(arg);
		}
	}
}
