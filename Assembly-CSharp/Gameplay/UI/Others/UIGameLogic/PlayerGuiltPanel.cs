using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.UI.Others.UIGameLogic
{
	public class PlayerGuiltPanel : SerializedMonoBehaviour
	{
		private void Awake()
		{
			this.animator = base.GetComponent<Animator>();
		}

		public void SetGuiltLevel(int value = 0, bool instantly = false)
		{
			if (value < 0 || value > 7)
			{
				Debug.LogError("Invalid guilt amount");
				return;
			}
			if (instantly)
			{
				this.animator.Play(string.Format("LEVEL{0}", value));
			}
			else
			{
				this.animator.SetInteger("GUILT_LEVEL", value);
			}
		}

		private Animator animator;

		private const int MAX_GUILT = 7;

		private const string GUILT_LEVEL_KEY = "GUILT_LEVEL";
	}
}
