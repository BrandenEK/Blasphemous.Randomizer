using System;
using UnityEngine;

namespace Gameplay.GameControllers.Environment
{
	public class PlatformTest : MonoBehaviour
	{
		private void FixedUpdate()
		{
			base.transform.position = new Vector3(base.transform.position.x, Mathf.PingPong(Time.time, 4f), base.transform.position.z);
		}
	}
}
