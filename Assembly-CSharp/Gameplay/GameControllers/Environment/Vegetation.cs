using System;
using UnityEngine;

namespace Gameplay.GameControllers.Environment
{
	[RequireComponent(typeof(Animator))]
	[RequireComponent(typeof(BoxCollider2D))]
	public abstract class Vegetation : MonoBehaviour
	{
		public abstract void Shaking(float playTimeAnimation);

		protected Animator plantAnimator;

		protected BoxCollider2D plantCollider;

		protected bool isShaking;
	}
}
