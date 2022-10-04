using System;
using Gameplay.GameControllers.Entities.Animations;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.PontiffHusk
{
	public class PontiffHuskBossAnimatorInyector : EnemyAnimatorInyector
	{
		public void PlayDeath()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			this.ResetAll();
			this.StopHide();
			this.SetDualBool(PontiffHuskBossAnimatorInyector.B_Death, true);
		}

		public void StopDeath()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			this.SetDualBool(PontiffHuskBossAnimatorInyector.B_Death, false);
		}

		public void PlayCharge()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			this.SetDualBool(PontiffHuskBossAnimatorInyector.B_Charge, true);
		}

		public void StopCharge()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			this.SetDualBool(PontiffHuskBossAnimatorInyector.B_Charge, false);
		}

		public void PlayShoot()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			this.SetDualBool(PontiffHuskBossAnimatorInyector.B_Shoot, true);
		}

		public void StopShoot()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			this.SetDualBool(PontiffHuskBossAnimatorInyector.B_Shoot, false);
		}

		public void PlayAltShoot()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			this.SetDualBool(PontiffHuskBossAnimatorInyector.B_AltShoot, true);
		}

		public void StopAltShoot()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			this.SetDualBool(PontiffHuskBossAnimatorInyector.B_AltShoot, false);
		}

		public void PlayCast()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			this.SetDualBool(PontiffHuskBossAnimatorInyector.B_Cast, true);
		}

		public void StopCast()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			this.SetDualBool(PontiffHuskBossAnimatorInyector.B_Cast, false);
		}

		public void PlayBeam()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			this.SetDualBool(PontiffHuskBossAnimatorInyector.B_Beam, true);
		}

		public void StopBeam()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			this.SetDualBool(PontiffHuskBossAnimatorInyector.B_Beam, false);
		}

		public void PlayTurn()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			this.SetDualBool(PontiffHuskBossAnimatorInyector.B_Charge, false);
			this.SetDualBool(PontiffHuskBossAnimatorInyector.B_Shoot, false);
			this.SetDualBool(PontiffHuskBossAnimatorInyector.B_AltShoot, false);
			this.SetDualTrigger(PontiffHuskBossAnimatorInyector.T_Turn);
		}

		public bool IsTurnQeued()
		{
			return this.mainAnimator.GetBool(PontiffHuskBossAnimatorInyector.T_Turn);
		}

		public void PlayHide()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			this.SetDualBool(PontiffHuskBossAnimatorInyector.T_Turn, false);
			this.SetDualBool(PontiffHuskBossAnimatorInyector.B_Hide, true);
		}

		public void StopHide()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			this.SetDualBool(PontiffHuskBossAnimatorInyector.B_Hide, false);
		}

		public bool GetHide()
		{
			return base.EntityAnimator && this.GetDualBool(PontiffHuskBossAnimatorInyector.B_Hide);
		}

		public void ResetAll()
		{
			this.SetDualBool(PontiffHuskBossAnimatorInyector.B_Death, false);
			this.SetDualBool(PontiffHuskBossAnimatorInyector.B_Charge, false);
			this.SetDualBool(PontiffHuskBossAnimatorInyector.B_Shoot, false);
			this.SetDualBool(PontiffHuskBossAnimatorInyector.B_AltShoot, false);
			this.SetDualBool(PontiffHuskBossAnimatorInyector.B_Cast, false);
			this.SetDualBool(PontiffHuskBossAnimatorInyector.B_Beam, false);
		}

		private void ResetDualTrigger(int name)
		{
			this.mainAnimator.ResetTrigger(name);
		}

		private void SetDualTrigger(int name)
		{
			this.mainAnimator.SetTrigger(name);
		}

		private void SetDualBool(int name, bool value)
		{
			this.mainAnimator.SetBool(name, value);
		}

		private bool GetDualBool(int name)
		{
			return this.mainAnimator.GetBool(name);
		}

		private static readonly int B_Death = Animator.StringToHash("DEATH");

		private static readonly int B_Hide = Animator.StringToHash("HIDE");

		private static readonly int B_Charge = Animator.StringToHash("CHARGE");

		private static readonly int B_Shoot = Animator.StringToHash("SHOOT");

		private static readonly int B_AltShoot = Animator.StringToHash("ALT_SHOOT");

		private static readonly int B_Cast = Animator.StringToHash("CAST");

		private static readonly int B_Beam = Animator.StringToHash("BEAM");

		private static readonly int T_Turn = Animator.StringToHash("TURN");

		public Animator mainAnimator;
	}
}
