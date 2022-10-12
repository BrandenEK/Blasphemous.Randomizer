using System;
using System.Collections;
using BezierSplines;
using Sirenix.OdinInspector;
using UnityEngine;

public class PathGrowerAttack : MonoBehaviour
{
	[Button(ButtonSizes.Small)]
	private void GrowBody()
	{
		base.StartCoroutine(this.GrowCoroutine(this.maxSeconds));
	}

	private void CreatePart(Vector2 pos, Vector2 dir)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.growerPrefab, pos, Quaternion.identity);
		gameObject.transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(dir.y, dir.x) * 57.29578f);
	}

	private IEnumerator GrowCoroutine(float seconds)
	{
		float counter = 0f;
		Vector3 lastPos = Vector3.zero;
		while (counter < seconds)
		{
			float v = counter / seconds;
			Vector3 p = this.spline.GetPoint(v);
			float d = Vector3.Distance(lastPos, p);
			if (d > this.maxDistanceBetweenInstances)
			{
				this.CreatePart(p, this.spline.GetDirection(v));
				lastPos = p;
			}
			yield return null;
			counter += Time.deltaTime;
		}
		yield break;
	}

	public BezierSpline spline;

	public float maxSeconds;

	public float maxDistanceBetweenInstances = 0.3f;

	public GameObject growerPrefab;
}
