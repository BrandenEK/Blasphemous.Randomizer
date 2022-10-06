using System;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Bosses.PietyMonster.Attack;
using Gameplay.GameControllers.Camera;
using Gameplay.GameControllers.Effects.Player.Dust;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.PietyMonster.Animation
{
	public class PietyAnimatorBridge : MonoBehaviour
	{
		public bool AllowWalkCameraShake { get; set; }

		private void Start()
		{
			this._cameraManager = Core.Logic.CameraManager;
		}

		public void StompCameraShake()
		{
			if (this._cameraManager == null)
			{
				return;
			}
			this._cameraManager.ProCamera2DShake.ShakeUsingPreset("PietyStomp");
			this.SmashRumble();
		}

		public void WalkCameraShake()
		{
			if (this._cameraManager == null)
			{
				return;
			}
			if (this.AllowWalkCameraShake)
			{
				this._cameraManager.ProCamera2DShake.ShakeUsingPreset("PietyStep");
			}
			this.StepRumble();
		}

		public void StepRumble()
		{
			if (this.PietyMonster == null)
			{
				return;
			}
			this.PietyMonster.Rumble.UsePreset("Step");
		}

		public void SmashRumble()
		{
			if (this.PietyMonster == null)
			{
				return;
			}
			this.PietyMonster.Rumble.UsePreset("Smash");
		}

		public void StompAttack()
		{
			if (this.PietyMonster == null)
			{
				return;
			}
			this.PietyMonster.PietyBehaviour.StompAttack.CurrentWeaponAttack();
		}

		public void ClawAttack()
		{
			if (this.PietyMonster == null)
			{
				return;
			}
			this.PietyMonster.PietyBehaviour.ClawAttack.CurrentWeaponAttack();
		}

		public void SmashAttack()
		{
			if (this.PietyMonster == null)
			{
				return;
			}
			this.PietyMonster.PietyBehaviour.SmashAttack.CurrentWeaponAttack();
		}

		public void GetStepDust()
		{
			if (!this.AllowWalkCameraShake)
			{
				return;
			}
			if (this.PietyMonster.DustSpawner != null)
			{
				StepDustRoot stepDustRoot = this.PietyMonster.DustSpawner.StepDustRoot;
				Vector2 stepDustPosition = stepDustRoot.transform.position;
				Vector2 vector = stepDustRoot.transform.localPosition;
				stepDustPosition.x = ((this.PietyMonster.Status.Orientation != EntityOrientation.Left) ? stepDustPosition.x : (stepDustPosition.x - vector.x * 2f));
				this.PietyMonster.DustSpawner.CurrentStepDustSpawn = StepDust.StepDustType.Running;
				this.PietyMonster.DustSpawner.GetStepDust(stepDustPosition);
				this.PlayStepStomp();
			}
		}

		public void ReadyToAttack()
		{
		}

		public void StompAttackResizeDamageArea()
		{
			Vector2 attackDamageAreaOffset = this.PietyMonster.PietyBehaviour.StompAttack.AttackDamageAreaOffset;
			Vector2 attackDamageAreaSize = this.PietyMonster.PietyBehaviour.StompAttack.AttackDamageAreaSize;
			BoxCollider2D boxCollider2D = (BoxCollider2D)this.PietyMonster.DamageArea.DamageAreaCollider;
			boxCollider2D.offset = attackDamageAreaOffset;
			boxCollider2D.size = attackDamageAreaSize;
		}

		public void SetDefaultDamageArea()
		{
			BoxCollider2D boxCollider2D = (BoxCollider2D)this.PietyMonster.DamageArea.DamageAreaCollider;
			boxCollider2D.size = this.PietyMonster.PietyBehaviour.StompAttack.DefaultDamageAreaSize;
			boxCollider2D.offset = this.PietyMonster.PietyBehaviour.StompAttack.DefaultDamageAreaOffset;
		}

		public void LaunchRoots()
		{
			if (this.PietyMonster == null)
			{
				return;
			}
			this.PietyMonster.PietyRootsManager.EnableNearestRoots();
		}

		public void LaunchDominoRoots()
		{
			if (this.PietyMonster == null)
			{
				return;
			}
			this.PietyMonster.PietyRootsManager.EnableDominoRoots();
		}

		public void Spit()
		{
			if (this.PietyMonster == null)
			{
				return;
			}
			this.PietyMonster.PietyBehaviour.SpitAttack.Spit();
			this.PietyMonster.PietyBehaviour.SpitAttack.Spitsthrown++;
			int currentSpitAmount = this.PietyMonster.PietyBehaviour.SpitAttack.CurrentSpitAmount;
			if (this.PietyMonster.PietyBehaviour.SpitAttack.Spitsthrown >= currentSpitAmount)
			{
				this.PietyMonster.PietyBehaviour.SpitAttack.Spitsthrown = 0;
				this.PietyMonster.PietyBehaviour.StompAttackCounter = 0;
				this.PietyMonster.AnimatorInyector.StopSpiting();
				this.PietyMonster.PietyBehaviour.Spiting = false;
			}
		}

		public void DestroyBushes()
		{
			PietyBushManager pietyBushManager = Object.FindObjectOfType<PietyBushManager>();
			if (pietyBushManager != null)
			{
				pietyBushManager.DestroyBushes();
			}
		}

		public void PlayStep()
		{
			if (this.PietyMonster == null)
			{
				return;
			}
			this.PietyMonster.Audio.PlayWalk();
		}

		public void PlayStepStomp()
		{
			if (this.PietyMonster == null)
			{
				return;
			}
			this.PietyMonster.Audio.PlayStepStomp();
		}

		public void Turn()
		{
			if (this.PietyMonster == null)
			{
				return;
			}
			this.PietyMonster.Audio.PlayTurn();
		}

		public void PlayStop()
		{
			if (this.PietyMonster == null)
			{
				return;
			}
			this.PietyMonster.Audio.PlayStop();
		}

		public void PlaySlash()
		{
			if (this.PietyMonster == null)
			{
				return;
			}
			this.PietyMonster.Audio.PlaySlash();
		}

		public void PlayStomp()
		{
			if (this.PietyMonster == null)
			{
				return;
			}
			this.PietyMonster.Audio.PlayStomp();
		}

		public void PlayDeath()
		{
			if (this.PietyMonster == null)
			{
				return;
			}
			this.PietyMonster.Audio.PlayDead();
		}

		public void PlayScream()
		{
			if (this.PietyMonster == null)
			{
				return;
			}
			this.PietyMonster.Audio.PlayScream();
		}

		public void PlaySmash()
		{
			if (this.PietyMonster == null)
			{
				return;
			}
			this.PietyMonster.Audio.PlaySmash();
		}

		public void PlayGetUp()
		{
			if (this.PietyMonster == null)
			{
				return;
			}
			this.PietyMonster.Audio.PlayGetUp();
		}

		public void ReadyToSpit()
		{
			if (this.PietyMonster == null)
			{
				return;
			}
			this.PietyMonster.Audio.ReadyToSpit();
		}

		public void PlaySpit()
		{
			if (this.PietyMonster == null)
			{
				return;
			}
			this.PietyMonster.Audio.Spit();
		}

		public void PlayStompHit()
		{
			if (this.PietyMonster == null)
			{
				return;
			}
			this.PietyMonster.Audio.PlayStompHit();
		}

		public PietyMonster PietyMonster;

		private CameraManager _cameraManager;
	}
}
