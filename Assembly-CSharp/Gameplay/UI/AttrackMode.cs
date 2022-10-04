using System;
using Framework.Managers;
using Rewired;
using UnityEngine;

namespace Gameplay.UI
{
	public class AttrackMode : MonoBehaviour
	{
		private void Update()
		{
			if (Core.Logic.IsAttrackScene())
			{
				return;
			}
			bool flag = Input.anyKey;
			if (!flag)
			{
				Player player = ReInput.players.GetPlayer(0);
				if (player != null)
				{
					flag = (flag || Mathf.Abs(player.GetAxisRaw("Move Horizontal")) > 0.2f || Mathf.Abs(player.GetAxisRaw("Move Vertical")) > 0.2f || Mathf.Abs(player.GetAxisRaw("Move RVertical")) > 0.2f || Mathf.Abs(player.GetAxisRaw("Move RHorizontal")) > 0.2f);
				}
			}
			if (flag)
			{
				AttrackMode.TimeWithOutInput = 0f;
			}
			else
			{
				AttrackMode.TimeWithOutInput += Time.deltaTime;
			}
			this.currentTimeWithOutInput = AttrackMode.TimeWithOutInput;
			if (AttrackMode.TimeWithOutInput >= this.maxTimeWithoutInput)
			{
				AttrackMode.TimeWithOutInput = 0f;
				Core.Logic.LoadAttrackScene();
			}
		}

		private static float TimeWithOutInput;

		public float currentTimeWithOutInput;

		public float maxTimeWithoutInput = 60f;

		private const float THRESHOLD = 0.2f;
	}
}
