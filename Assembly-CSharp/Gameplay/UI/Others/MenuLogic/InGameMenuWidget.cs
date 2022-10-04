using System;
using System.Collections.Generic;
using Framework.Managers;
using Gameplay.UI.Widgets;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Gameplay.UI.Others.MenuLogic
{
	public class InGameMenuWidget : MonoBehaviour
	{
		public bool currentlyActive { get; private set; }

		private void Awake()
		{
			this.animator = base.GetComponent<Animator>();
			this.currentlyActive = false;
		}

		public void Show(bool b)
		{
			if (SceneManager.GetActiveScene().name == "MainMenu")
			{
				return;
			}
			if (FadeWidget.instance.Fading)
			{
				return;
			}
			if (UIController.instance.Paused)
			{
				return;
			}
			if (b && Core.Input.InputBlocked && !Core.Input.HasBlocker("PLAYER_LOGIC"))
			{
				return;
			}
			Array.ForEach<Button>(base.GetComponentsInChildren<Button>(), delegate(Button bto)
			{
				bto.OnDeselect(null);
			});
			Core.Input.SetBlocker("INGAME_MENU", b);
			this.currentlyActive = b;
			if (b)
			{
				Core.Logic.SetState(LogicStates.Unresponsive);
				EventSystem.current.SetSelectedGameObject(this.firstSelected);
			}
			else
			{
				Core.Logic.SetState(LogicStates.Playing);
			}
			this.animator.SetBool("ACTIVE", b);
		}

		public void MainMenu()
		{
			if (this.currentlyActive)
			{
				this.Show(false);
				Core.Logic.LoadMenuScene(true);
				Analytics.CustomEvent("QUIT_GAME", new Dictionary<string, object>
				{
					{
						"Scene",
						SceneManager.GetActiveScene().name
					}
				});
			}
		}

		public void Reset()
		{
			if (this.currentlyActive)
			{
				this.Show(false);
				Core.Logic.Penitent.KillInstanteneously();
			}
		}

		public void Configuration()
		{
			if (this.currentlyActive)
			{
				this.Show(false);
			}
		}

		public void Exit()
		{
			if (this.currentlyActive)
			{
			}
		}

		public void ChangeLanguage()
		{
			Core.Localization.SetNextLanguage();
		}

		private Animator animator;

		public GameObject firstSelected;
	}
}
