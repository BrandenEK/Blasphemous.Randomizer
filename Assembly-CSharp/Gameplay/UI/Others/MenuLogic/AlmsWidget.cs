using System;
using System.Collections;
using Framework.Managers;
using Rewired;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.Others.MenuLogic
{
	public class AlmsWidget : BasicUIBlockingWidget
	{
		protected override void OnWidgetInitialize()
		{
			this.Rewired = ReInput.players.GetPlayer(0);
		}

		protected override void OnWidgetShow()
		{
			this.CurrentNumber = 0f;
			this.NumberText.text = "0";
			this.TimePressing = 0f;
			this.TimesLastSound = -1f;
			this.WaitToSkinUI = false;
			this.IsPressing = false;
		}

		public void SubmitPressed()
		{
			if (this.WaitToSkinUI)
			{
				return;
			}
			bool flag = false;
			int tears = (int)this.CurrentNumber;
			if (this.CurrentNumber > 0f && Core.Alms.CanConsumeTears(tears))
			{
				string id = Core.Alms.Config.SoundAddedOk;
				if (Core.Alms.ConsumeTears(tears))
				{
					id = Core.Alms.Config.SoundNewTier;
				}
				this.WaitToSkinUI = UIController.instance.IsUnlockActive();
				flag = true;
				Core.Audio.PlaySfx(id, 0f);
			}
			if (flag)
			{
				if (this.WaitToSkinUI)
				{
					base.StartCoroutine(this.WaitSkinAndClose());
				}
				else
				{
					base.FadeHide();
				}
			}
		}

		private IEnumerator WaitSkinAndClose()
		{
			while (this.WaitToSkinUI)
			{
				this.WaitToSkinUI = UIController.instance.IsUnlockActive();
				yield return 0;
			}
			base.FadeHide();
			yield break;
		}

		private void Update()
		{
			if (this.Rewired == null || this.WaitToSkinUI)
			{
				return;
			}
			float currentNumber = this.CurrentNumber;
			float axisRaw = this.Rewired.GetAxisRaw(49);
			if (Mathf.Abs(axisRaw) >= 0.1f)
			{
				if (!this.IsPressing)
				{
					this.NumberBeforePress = (int)this.CurrentNumber;
					this.FactorBeforePress = ((axisRaw >= 0f) ? 1 : -1);
					this.IsPressing = true;
				}
				this.TimePressing += Time.unscaledDeltaTime;
				float num = axisRaw * Core.Alms.Config.NumberSpeed * Core.Alms.Config.NumberFactorByTime.Evaluate(this.TimePressing) * Time.unscaledDeltaTime;
				this.CurrentNumber += num;
				this.CheckCurrent();
				float num2 = Core.Alms.Config.SoundChangeUpdate.Evaluate(this.TimePressing);
				if (currentNumber != this.CurrentNumber && (this.TimesLastSound < 0f || this.TimesLastSound >= num2))
				{
					Core.Audio.PlayOneShot(Core.Alms.Config.SoundChange, default(Vector3));
					this.TimesLastSound = 0f;
				}
				else
				{
					this.TimesLastSound += Time.unscaledDeltaTime;
				}
			}
			else
			{
				if (this.IsPressing && this.NumberBeforePress == (int)this.CurrentNumber)
				{
					this.CurrentNumber += (float)this.FactorBeforePress;
					this.CheckCurrent();
				}
				this.IsPressing = false;
				this.TimePressing = 0f;
				this.TimesLastSound = -1f;
			}
			if (currentNumber != this.CurrentNumber)
			{
				this.NumberText.text = ((int)this.CurrentNumber).ToString();
			}
		}

		private void CheckCurrent()
		{
			if (this.CurrentNumber < 0f)
			{
				this.CurrentNumber = 0f;
			}
			else if (this.CurrentNumber > (float)Core.Alms.Config.MaxNumber)
			{
				this.CurrentNumber = (float)Core.Alms.Config.MaxNumber;
			}
			if (!Core.Alms.CanConsumeTears((int)this.CurrentNumber))
			{
				this.CurrentNumber = Core.Logic.Penitent.Stats.Purge.Current;
			}
		}

		[BoxGroup("Controls", true, false, 0)]
		public Text NumberText;

		private Player Rewired;

		private float CurrentNumber;

		private float TimePressing;

		private float TimesLastSound = -1f;

		private const float MovementEpsilon = 0.1f;

		private bool WaitToSkinUI;

		private int NumberBeforePress;

		private int FactorBeforePress;

		private bool IsPressing;
	}
}
