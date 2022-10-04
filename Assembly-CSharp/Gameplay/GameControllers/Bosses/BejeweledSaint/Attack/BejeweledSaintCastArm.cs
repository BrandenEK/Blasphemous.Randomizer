using System;
using System.Collections;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.BejeweledSaint.Attack
{
	public class BejeweledSaintCastArm : EnemyAttack
	{
		public BejeweledSaintBoss Boss { get; private set; }

		public Animator Animator { get; private set; }

		private void Awake()
		{
			base.EntityOwner = this.Owner;
			this.Boss = this.Owner.GetComponentInParent<BejeweledSaintBoss>();
			this.Animator = base.GetComponent<Animator>();
		}

		public void CastBeams()
		{
			if (this.Boss == null)
			{
				return;
			}
			this.Boss.BeamManager.InstantiateDivineBeams();
		}

		public void CastSingleBeam(Vector2 pos)
		{
			if (this.Boss == null)
			{
				return;
			}
			this.Boss.BeamManager.InstantiateSingleBeam(pos);
		}

		public void CastSingleBeamDelayed(Vector2 pos, float delay)
		{
			if (this.Boss == null)
			{
				return;
			}
			base.StartCoroutine(this.DelayedSingleBeam(pos, delay));
		}

		private IEnumerator DelayedSingleBeam(Vector2 pos, float delay)
		{
			yield return new WaitForSeconds(delay);
			this.Boss.BeamManager.InstantiateSingleBeam(pos);
			yield break;
		}

		public void DoCastSign()
		{
			if (this.Animator == null)
			{
				return;
			}
			this.Animator.SetTrigger("CAST");
		}

		public Enemy Owner;
	}
}
