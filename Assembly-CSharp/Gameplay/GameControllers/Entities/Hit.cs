using System;
using FMODUnity;
using UnityEngine;

namespace Gameplay.GameControllers.Entities
{
	[Serializable]
	public struct Hit
	{
		[EventRef]
		public string HitSoundId;

		public GameObject AttackingEntity;

		public DamageArea.DamageType DamageType;

		public DamageArea.DamageElement DamageElement;

		public bool Unnavoidable;

		public bool ForceGuardSlideDirection;

		public bool Unparriable;

		public bool Unblockable;

		public bool DestroysProjectiles;

		public bool DontSpawnBlood;

		public bool forceGuardslide;

		public bool CheckOrientationsForGuardslide;

		public bool ThrowbackDirByOwnerPosition;

		public float DamageAmount;

		public float Force;

		public Action<Hit> OnGuardCallback;
	}
}
