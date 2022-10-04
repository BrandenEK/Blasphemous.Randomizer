using System;
using Com.LuisPedroFonseca.ProCamera2D;
using Sirenix.OdinInspector;
using UnityEngine;

public class HighWillsCamInfluenceController : MonoBehaviour
{
	private void Update()
	{
		ProCamera2D.Instance.ApplyInfluence(this.NormalInfluence);
	}

	[BoxGroup("Cam Settings", true, false, 0)]
	public Vector2 NormalInfluence = new Vector2(5f, 0f);
}
