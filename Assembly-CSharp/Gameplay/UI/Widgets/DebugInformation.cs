using System;
using Framework.FrameworkCore;
using Framework.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Gameplay.UI.Widgets
{
	public class DebugInformation : MonoBehaviour
	{
		private void Awake()
		{
			LevelManager.OnLevelLoaded += this.OnLevelLoaded;
			LevelManager.OnMenuLoaded += this.OnMenuLoaded;
			base.gameObject.SetActive(false);
			this.isDebugBuild = Debug.isDebugBuild;
			this.DisableWhenRetailMode();
		}

		private void OnDestroy()
		{
			LevelManager.OnLevelLoaded -= this.OnLevelLoaded;
			LevelManager.OnMenuLoaded -= this.OnMenuLoaded;
		}

		private void OnMenuLoaded()
		{
			base.gameObject.SetActive(false);
		}

		private void OnLevelLoaded(Level oldLevel, Level newLevel)
		{
			base.gameObject.SetActive(true);
			string text = SceneManager.GetActiveScene().name.ToUpper();
			if (this.currentSceneLabel != null)
			{
				this.currentSceneLabel.text = text;
			}
		}

		private void Update()
		{
			if (!Core.ready)
			{
				return;
			}
			Cursor.visible = false;
			if (this.currentCoordinatesLabel != null && Core.Logic.Penitent != null)
			{
				this.currentCoordinatesLabel.text = Core.Logic.Penitent.transform.position.ToString();
			}
			Cursor.visible = (this.isDebugBuild && this.showCursor);
		}

		private void DisableWhenRetailMode()
		{
			if (this.currentCoordinatesLabel)
			{
				this.currentCoordinatesLabel.gameObject.SetActive(this.isDebugBuild);
			}
			if (this.currentSceneLabel)
			{
				this.currentSceneLabel.gameObject.SetActive(this.isDebugBuild);
			}
		}

		public Text currentSceneLabel;

		public Text currentCoordinatesLabel;

		public bool showCursor;

		private bool isDebugBuild;
	}
}
