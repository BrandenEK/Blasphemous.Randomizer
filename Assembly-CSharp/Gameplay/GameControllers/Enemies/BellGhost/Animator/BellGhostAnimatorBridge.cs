using System;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.BellGhost.Animator
{
	public class BellGhostAnimatorBridge : MonoBehaviour
	{
		private void Start()
		{
			this._bellGhost = base.GetComponentInParent<BellGhost>();
		}

		public void PlayBrokenBell()
		{
			this._bellGhost.Audio.BrokenBell();
		}

		public void Ramming()
		{
			this._bellGhost.Behaviour.Ramming();
		}

		public void PlayChargeAttack()
		{
			this._bellGhost.Audio.ChargeAttack();
		}

		private BellGhost _bellGhost;
	}
}
