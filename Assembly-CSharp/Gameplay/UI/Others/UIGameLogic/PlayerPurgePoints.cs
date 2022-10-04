using System;
using System.Collections;
using System.Collections.Generic;
using Framework.FrameworkCore;
using Framework.FrameworkCore.Attributes;
using Framework.Managers;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.Others.UIGameLogic
{
	public class PlayerPurgePoints : SerializedMonoBehaviour
	{
		private void Awake()
		{
			this.currentPoints = 0f;
			this.targetPoints = 0f;
			this.text.text = "0";
			this.RefreshPoints(true);
			LevelManager.OnLevelLoaded += this.OnLevelLoaded;
		}

		private void OnLevelLoaded(Level oldLevel, Level newLevel)
		{
			if (this.IsAffectedByGameMode && ((this.IsDemakeVersion && !Core.GameModeManager.IsCurrentMode(GameModeManager.GAME_MODES.DEMAKE)) || (!this.IsDemakeVersion && Core.GameModeManager.IsCurrentMode(GameModeManager.GAME_MODES.DEMAKE))))
			{
				base.gameObject.SetActive(false);
			}
			if (newLevel.LevelName.StartsWith("Main"))
			{
				this.currentPoints = 0f;
				this.targetPoints = 0f;
				this.text.text = "0";
			}
			if (Core.Logic != null && Core.Logic.Penitent != null)
			{
				Core.Logic.Penitent.Stats.Purge.OnChanged += this.RunRefreshPointsCoroutine;
			}
			this.RefreshPoints(true);
		}

		private void Start()
		{
			this.RefreshPoints(true);
		}

		private void RunRefreshPointsCoroutine()
		{
			if (!base.gameObject.activeInHierarchy)
			{
				return;
			}
			if (this.refreshCoroutine != null)
			{
				base.StopCoroutine(this.refreshCoroutine);
			}
			this.refreshCoroutine = base.StartCoroutine(this.RefreshPointsCoroutine());
		}

		private IEnumerator RefreshPointsCoroutine()
		{
			if (Core.Logic == null || Core.Logic.Penitent == null || Core.Logic.Penitent.Stats == null)
			{
				yield break;
			}
			Purge points = Core.Logic.Penitent.Stats.Purge;
			if (points == null)
			{
				yield break;
			}
			if (Math.Abs(this.targetPoints - points.Current) > Mathf.Epsilon)
			{
				this.targetPoints = points.Current;
				this.numbersPerSeconds = (this.targetPoints - this.currentPoints) / this.animationDuration;
				this.PlayPurgePointsFx();
			}
			float timeLeft = this.animationDuration;
			float inc = this.numbersPerSeconds * Time.unscaledDeltaTime;
			while (timeLeft > 0f)
			{
				this.currentPoints += inc;
				timeLeft -= Time.unscaledDeltaTime;
				this.text.text = ((int)this.currentPoints).ToString();
				yield return null;
			}
			this.currentPoints = this.targetPoints;
			this.text.text = ((int)this.currentPoints).ToString();
			yield break;
		}

		public void RefreshGuilt(bool whenDead)
		{
			if (whenDead)
			{
				base.StartCoroutine(this.RefreshGuiltWhenDead());
			}
			else
			{
				this.guiltPanel.SetGuiltLevel(Core.GuiltManager.GetDropsCount(), false);
			}
		}

		private IEnumerator RefreshGuiltWhenDead()
		{
			this.guiltPanel.SetGuiltLevel(0, true);
			yield return new WaitForSeconds(2f);
			this.guiltPanel.SetGuiltLevel(Core.GuiltManager.GetDropsCount(), false);
			yield break;
		}

		public void RefreshPoints(bool inmediate)
		{
			if (Core.Logic == null || Core.Logic.Penitent == null || Core.Logic.Penitent.Stats == null)
			{
				return;
			}
			Purge purge = Core.Logic.Penitent.Stats.Purge;
			if (purge == null)
			{
				return;
			}
			if (Math.Abs(this.targetPoints - purge.Current) > Mathf.Epsilon)
			{
				this.targetPoints = purge.Current;
				this.numbersPerSeconds = (this.targetPoints - this.currentPoints) / this.animationDuration;
				this.PlayPurgePointsFx();
			}
			if (inmediate)
			{
				this.currentPoints = this.targetPoints;
				this.numbersPerSeconds = 0f;
				this.text.text = ((int)this.currentPoints).ToString();
				return;
			}
			if (Math.Abs(this.targetPoints - this.currentPoints) < Mathf.Epsilon)
			{
				return;
			}
			float f = this.targetPoints - this.currentPoints;
			float num = this.numbersPerSeconds * Time.unscaledDeltaTime;
			if (Mathf.Abs(num) < Mathf.Abs(f))
			{
				this.currentPoints += num;
			}
			else
			{
				this.currentPoints = this.targetPoints;
				this.numbersPerSeconds = 0f;
			}
			this.text.text = ((int)this.currentPoints).ToString();
		}

		private void PlayPurgePointsFx()
		{
			if (string.IsNullOrEmpty("event:/SFX/UI/PurgeCounter"))
			{
				return;
			}
			Core.Audio.PlaySfx("event:/SFX/UI/PurgeCounter", 0f);
		}

		private void OnDestroy()
		{
			LevelManager.OnLevelLoaded -= this.OnLevelLoaded;
		}

		public const float EXECUTION_PURGE_BONUS = 0.5f;

		public const string PurgePointUpdateFx = "event:/SFX/UI/PurgeCounter";

		public Text text;

		public float animationDuration = 1f;

		public PlayerGuiltPanel guiltPanel;

		public bool IsDemakeVersion;

		public bool IsAffectedByGameMode = true;

		[SerializeField]
		private Image background;

		[OdinSerialize]
		private List<Sprite> levelList = new List<Sprite>();

		private float currentPoints;

		private float targetPoints;

		private float numbersPerSeconds;

		private Coroutine refreshCoroutine;
	}
}
