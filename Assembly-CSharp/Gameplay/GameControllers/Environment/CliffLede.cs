using System;
using Framework.FrameworkCore;
using UnityEngine;

namespace Gameplay.GameControllers.Environment
{
	[RequireComponent(typeof(BoxCollider2D))]
	public class CliffLede : MonoBehaviour
	{
		private void Awake()
		{
			this.cliffLedeGrabSideAllowed = ((this.cliffLedeSide != CliffLede.CliffLedeSide.Left) ? EntityOrientation.Right : EntityOrientation.Left);
		}

		private void Start()
		{
			this.rootTarget = base.GetComponentInChildren<CliffLedeRootTarget>();
		}

		public EntityOrientation CliffLedeGrabSideAllowed
		{
			get
			{
				return this.cliffLedeGrabSideAllowed;
			}
		}

		public CliffLedeRootTarget RootTarget
		{
			get
			{
				return this.rootTarget;
			}
		}

		private void OnValidate()
		{
			if (this.gizmo != null)
			{
				this.gizmo.flipX = (this.cliffLedeSide == CliffLede.CliffLedeSide.Left);
			}
		}

		[Tooltip("The entity orientation allowed to grab the cliff lede hook.")]
		public CliffLede.CliffLedeSide cliffLedeSide;

		[Tooltip("Determines whether the cliff lede is climbable. True by default.")]
		public bool isClimbable = true;

		[Tooltip("Sprite representation of the cliff")]
		public SpriteRenderer gizmo;

		private EntityOrientation cliffLedeGrabSideAllowed;

		private CliffLedeRootTarget rootTarget;

		public enum CliffLedeSide
		{
			Left,
			Right
		}
	}
}
