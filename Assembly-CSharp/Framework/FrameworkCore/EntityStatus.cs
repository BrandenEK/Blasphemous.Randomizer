using System;

namespace Framework.FrameworkCore
{
	public class EntityStatus
	{
		public bool Unattacable { get; set; }

		public bool Invulnerable { get; set; }

		public bool Paralyzed { get; set; }

		public bool Dead { get; set; }

		public bool IsGrounded { get; set; }

		public bool AbilitiesDisabled { get; set; }

		public EntityOrientation Orientation { get; set; }

		public bool IsOnCliffLede { get; set; }

		public bool IsVisibleOnCamera { get; set; }

		public bool IsHurt { get; set; }

		public bool IsIdle { get; set; }

		public bool CastShadow { get; set; }
	}
}
