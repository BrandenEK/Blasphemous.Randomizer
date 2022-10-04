using System;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Penitent;
using Tools.Level;

namespace Tools.Playmaker2
{
	public class PlaymakerCache
	{
		public static Penitent selectedPlayer;

		public static Entity lastCreated;

		public static Entity lastDamaged;

		public static Interactable lastActivated;

		public static Interactable lastInteracted;
	}
}
