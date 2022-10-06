using System;
using Framework.Managers;
using Gameplay.GameControllers.Penitent;
using Gameplay.GameControllers.Penitent.Attack;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Player.Attack
{
	public class FinishingComboStarterBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this._penitent == null)
			{
				this._penitent = Core.Logic.Penitent;
			}
			this._attackBuffer = 0f;
			this._joyStickBuffer = 0f;
			this._allowFinisherCombo = false;
			this._comboDirection = FinishingComboStarterBehaviour.JoyStickDirection.Center;
			Vector2 size;
			size..ctor(3.57f, 2.25f);
			Vector2 offset;
			offset..ctor(1.69f, 1.75f);
			this._penitent.AttackArea.SetSize(size);
			this._penitent.AttackArea.SetOffset(offset);
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateUpdate(animator, stateInfo, layerIndex);
			if (stateInfo.normalizedTime > 0.25f)
			{
				this._allowFinisherCombo = true;
			}
			this._attackBuffer += Time.deltaTime;
			if (this._penitent.PlatformCharacterInput.Attack)
			{
				this._attackBuffer = 0f;
			}
			this._verticalAxis = this._penitent.PlatformCharacterInput.FVerAxis;
			if (this._verticalAxis > 0f)
			{
				this._comboDirection = FinishingComboStarterBehaviour.JoyStickDirection.Up;
			}
			else if (this._verticalAxis < 0f)
			{
				this._comboDirection = FinishingComboStarterBehaviour.JoyStickDirection.Down;
			}
			if (stateInfo.normalizedTime > 0.65f && this._attackBuffer <= 0.25f)
			{
				PenitentAttack penitentAttack = (PenitentAttack)this._penitent.EntityAttack;
				if (penitentAttack.Combo.IsAvailable)
				{
					this.SetFinisherByAbilityTier(penitentAttack.Combo.GetMaxSkill.id, animator);
				}
			}
			if (this._comboDirection == FinishingComboStarterBehaviour.JoyStickDirection.Center)
			{
				return;
			}
			this._joyStickBuffer += Time.deltaTime;
			if (this._joyStickBuffer >= 0.5f)
			{
				this._joyStickBuffer = 0f;
				this._comboDirection = FinishingComboStarterBehaviour.JoyStickDirection.Center;
			}
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			if (!this._allowFinisherCombo)
			{
				return;
			}
			PenitentAttack penitentAttack = (PenitentAttack)this._penitent.EntityAttack;
			if (!penitentAttack.Combo.IsAvailable)
			{
				return;
			}
		}

		private void SetFinisherByAbilityTier(string skillId, Animator animator)
		{
			this._attackBuffer = 0f;
			if (skillId != null)
			{
				if (!(skillId == "COMBO_1"))
				{
					if (!(skillId == "COMBO_2"))
					{
						if (skillId == "COMBO_3")
						{
							if (this._comboDirection == FinishingComboStarterBehaviour.JoyStickDirection.Down)
							{
								animator.Play("ComboFinisherDown");
							}
							else if (this._comboDirection == FinishingComboStarterBehaviour.JoyStickDirection.Up)
							{
								animator.Play("ComboFinisherUp");
							}
							else
							{
								animator.Play("Combo_4");
							}
						}
					}
					else
					{
						animator.Play((this._comboDirection != FinishingComboStarterBehaviour.JoyStickDirection.Up) ? "Combo_4" : "ComboFinisherUp");
					}
				}
				else
				{
					animator.Play("Combo_4");
				}
			}
		}

		private Penitent _penitent;

		private float _attackBuffer;

		private float _joyStickBuffer;

		private float _verticalAxis;

		private const float COMBO_BUFFER_TIME = 0.25f;

		private bool _allowFinisherCombo;

		private FinishingComboStarterBehaviour.JoyStickDirection _comboDirection;

		private enum JoyStickDirection
		{
			Up,
			Down,
			Center
		}
	}
}
