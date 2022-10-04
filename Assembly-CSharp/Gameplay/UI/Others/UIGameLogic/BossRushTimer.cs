using System;
using Framework.Managers;
using Framework.Util;
using Sirenix.OdinInspector;
using UnityEngine.UI;

namespace Gameplay.UI.Others.UIGameLogic
{
	public class BossRushTimer : SerializedMonoBehaviour
	{
		private void Update()
		{
			if (Singleton<Core>.Instance == null || !Core.ready)
			{
				return;
			}
			if (!Core.GameModeManager.IsCurrentMode(GameModeManager.GAME_MODES.BOSS_RUSH))
			{
				this.Hide();
			}
			this.text.text = Core.BossRushManager.GetCurrentRunDurationString();
		}

		public void Show()
		{
			base.gameObject.SetActive(true);
		}

		public void Hide()
		{
			base.gameObject.SetActive(false);
		}

		public Text text;
	}
}
