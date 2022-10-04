using System;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Gameplay.GameControllers.Penitent.Attack
{
	[RequireComponent(typeof(Animator))]
	[RequireComponent(typeof(SpriteRenderer))]
	public class SwordAnimatorInyector : MonoBehaviour
	{
		private bool IsXFlip { get; set; }

		private bool IsDemakeMode
		{
			get
			{
				return Core.GameModeManager.IsCurrentMode(GameModeManager.GAME_MODES.DEMAKE);
			}
		}

		private void Start()
		{
			if (this.SwordAnimator == null)
			{
				Debug.LogError("A Sprite Animator is needed");
			}
			if (this.OwnerEntity == null)
			{
				Debug.LogError("A Owner is needed");
			}
			this.SpriteRenderer = base.GetComponent<SpriteRenderer>();
			this._lastEntityAnimatorState = this.OwnerEntity.Animator.GetCurrentAnimatorStateInfo(0).shortNameHash;
			this._defaultStateHashName = Animator.StringToHash("Default");
			this._attackStateHashName = Animator.StringToHash("Attack");
		}

		private void Update()
		{
			this.SpriteRendererFlip();
			this._currentEntityAnimatorState = this.OwnerEntity.Animator.GetCurrentAnimatorStateInfo(0).shortNameHash;
			if (this._lastEntityAnimatorState != this._currentEntityAnimatorState)
			{
				this._lastEntityAnimatorState = this._currentEntityAnimatorState;
				if (this.OwnerEntity.Animator.GetCurrentAnimatorStateInfo(0).shortNameHash == this._attackStateHashName || this.IsDemakeMode)
				{
					return;
				}
				if (this.SwordAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash != this._defaultStateHashName)
				{
					this.SwordAnimator.Play(this._defaultStateHashName);
				}
			}
		}

		public void SetSlashAnimation(PenitentSword.SwordSlash slashType)
		{
			switch (slashType.Type)
			{
			case PenitentSword.AttackType.Crouch:
				this.SetAnimatorParameters(1, slashType.Level, slashType.Color);
				break;
			case PenitentSword.AttackType.Basic1:
				this.SetAnimatorParameters(2, slashType.Level, slashType.Color);
				break;
			case PenitentSword.AttackType.Basic2:
				this.SetAnimatorParameters(3, slashType.Level, slashType.Color);
				break;
			case PenitentSword.AttackType.Combo:
				this.SetAnimatorParameters(4, slashType.Level, slashType.Color);
				break;
			case PenitentSword.AttackType.Air1:
				this.SetAnimatorParameters(5, slashType.Level, slashType.Color);
				break;
			case PenitentSword.AttackType.Air2:
				this.SetAnimatorParameters(6, slashType.Level, slashType.Color);
				break;
			case PenitentSword.AttackType.GroundUpward:
				this.SetAnimatorParameters(7, slashType.Level, slashType.Color);
				break;
			case PenitentSword.AttackType.AirUpward:
				this.SetAnimatorParameters(8, slashType.Level, slashType.Color);
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		private void SetAnimatorParameters(int type, int level, int color)
		{
			if (this.SwordAnimator == null)
			{
				return;
			}
			this.SwordAnimator.SetInteger("TYPE", type);
			this.SwordAnimator.SetInteger("LEVEL", level);
			this.SwordAnimator.SetInteger("COLOR", color);
		}

		public void ResetParameters()
		{
			if (this.SwordAnimator == null)
			{
				return;
			}
			this.SwordAnimator.SetInteger("LEVEL", 0);
			this.SwordAnimator.SetInteger("TYPE", 0);
			this.SwordAnimator.SetInteger("COLOR", 0);
		}

		public void SetAnimatorSpeed(float speed)
		{
			float speed2 = Mathf.Clamp(speed, 1f, 2f);
			this.SwordAnimator.speed = speed2;
		}

		public void PlayAttackDesiredTime(int attackLevel, float desiredTime, PenitentSword.AttackColor color, string attackLabel = "Basic1_Lv")
		{
			string text = string.Empty;
			if (attackLevel > 1)
			{
				if (color != PenitentSword.AttackColor.Default)
				{
					if (color != PenitentSword.AttackColor.Red)
					{
						text += string.Empty;
					}
					else
					{
						text += "_Red";
					}
				}
				else
				{
					text += string.Empty;
				}
			}
			string stateName = attackLabel + attackLevel + text;
			this.SwordAnimator.Play(stateName, 0, desiredTime);
			this.ResetParameters();
		}

		private void SpriteRendererFlip()
		{
			if (this.OwnerEntity.Status.Orientation == EntityOrientation.Left && !this.IsXFlip)
			{
				this.IsXFlip = true;
				this.SpriteRenderer.flipX = true;
			}
			else if (this.OwnerEntity.Status.Orientation == EntityOrientation.Right && this.IsXFlip)
			{
				this.IsXFlip = false;
				this.SpriteRenderer.flipX = false;
			}
		}

		public int GetColorValue(PenitentSword.AttackColor attackColor)
		{
			int result;
			if (attackColor != PenitentSword.AttackColor.Default)
			{
				if (attackColor != PenitentSword.AttackColor.Red)
				{
					result = 0;
				}
				else
				{
					result = 1;
				}
			}
			else
			{
				result = 0;
			}
			return result;
		}

		public Entity OwnerEntity;

		public Animator SwordAnimator;

		public SpriteRenderer SpriteRenderer;

		public const int CrouchAttack = 1;

		public const int BasicAttackType1 = 2;

		public const int BasicAttackType2 = 3;

		public const int ComboAttack = 4;

		public const int AirAttack1 = 5;

		public const int AirAttack2 = 6;

		public const int GroundUpward = 7;

		public const int AirUpward = 8;

		private int _lastEntityAnimatorState;

		private int _currentEntityAnimatorState;

		private int _defaultStateHashName;

		private int _attackStateHashName;
	}
}
