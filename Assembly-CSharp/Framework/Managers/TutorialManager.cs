using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Framework.FrameworkCore;
using Gameplay.UI;
using UnityEngine;

namespace Framework.Managers
{
	public class TutorialManager : GameSystem, PersistentInterface
	{
		public override void Start()
		{
			this.LoadAllTutorials();
			this.IsShowwingTutorial = false;
		}

		private void LoadAllTutorials()
		{
			Tutorial[] array = Resources.LoadAll<Tutorial>("Tutorial/");
			this.tutorials.Clear();
			foreach (Tutorial tutorial in array)
			{
				tutorial.unlocked = tutorial.startUnlocked;
				this.tutorials[tutorial.id] = tutorial;
			}
			Log.Debug("Tutorial", this.tutorials.Count.ToString() + " tutorials loaded succesfully.", null);
		}

		public override void AllInitialized()
		{
			Core.Persistence.AddPersistentManager(this);
		}

		public bool AnyTutorialIsUnlocked()
		{
			return this.GetUnlockedTutorials().Count > 0;
		}

		public List<Tutorial> GetUnlockedTutorials()
		{
			return (from x in this.tutorials.Values
			where x.unlocked
			orderby x.order
			select x).ToList<Tutorial>();
		}

		public void UnlockTutorial(string id)
		{
			if (this.tutorials.ContainsKey(id))
			{
				this.tutorials[id].unlocked = true;
			}
		}

		public bool IsTutorialUnlocked(string id)
		{
			bool result = false;
			if (this.tutorials.ContainsKey(id))
			{
				result = this.tutorials[id].unlocked;
			}
			return result;
		}

		public IEnumerator ShowTutorial(string id, bool blockPlayer = true)
		{
			if (!this.TutorialsEnabled)
			{
				yield return null;
			}
			else if (this.tutorials.ContainsKey(id))
			{
				while (this.IsShowwingTutorial)
				{
					yield return null;
				}
				this.IsShowwingTutorial = true;
				if (blockPlayer)
				{
					Core.Input.SetBlocker("TUTORIAL", true);
					Core.Logic.PauseGame();
				}
				Tutorial tut = this.tutorials[id];
				tut.unlocked = true;
				GameObject uiroot = UIController.instance.GetTutorialRoot();
				IEnumerator enumerator = uiroot.transform.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						object obj = enumerator.Current;
						Transform transform = (Transform)obj;
						UnityEngine.Object.Destroy(transform.gameObject);
					}
				}
				finally
				{
					IDisposable disposable;
					if ((disposable = (enumerator as IDisposable)) != null)
					{
						disposable.Dispose();
					}
				}
				GameObject tutObj = UnityEngine.Object.Instantiate<GameObject>(tut.prefab, Vector3.zero, Quaternion.identity, uiroot.transform);
				tutObj.transform.localPosition = Vector3.zero;
				TutorialWidget widget = tutObj.GetComponent<TutorialWidget>();
				widget.ShowInGame();
				CanvasGroup gr = uiroot.GetComponent<CanvasGroup>();
				gr.alpha = 0f;
				uiroot.SetActive(true);
				DOTween.defaultTimeScaleIndependent = true;
				DOTween.To(() => gr.alpha, delegate(float x)
				{
					gr.alpha = x;
				}, 1f, 1f);
				while (!widget.WantToExit)
				{
					yield return null;
				}
				TweenerCore<float, float, FloatOptions> teen = DOTween.To(() => gr.alpha, delegate(float x)
				{
					gr.alpha = x;
				}, 0f, 1f);
				yield return new WaitForSecondsRealtime(0.5f);
				if (blockPlayer)
				{
					Core.Input.SetBlocker("TUTORIAL", false);
					Core.Logic.ResumeGame();
				}
				uiroot.SetActive(false);
				DOTween.defaultTimeScaleIndependent = false;
			}
			this.IsShowwingTutorial = false;
			yield return null;
			yield break;
		}

		public int GetOrder()
		{
			return 0;
		}

		public string GetPersistenID()
		{
			return "ID_TUTORIAL";
		}

		public void ResetPersistence()
		{
			this.LoadAllTutorials();
		}

		public PersistentManager.PersistentData GetCurrentPersistentState(string dataPath, bool fullSave)
		{
			TutorialManager.TutorialPersistenceData tutorialPersistenceData = new TutorialManager.TutorialPersistenceData();
			foreach (Tutorial tutorial in this.tutorials.Values)
			{
				tutorialPersistenceData.Tutorials[tutorial.id] = tutorial.unlocked;
			}
			tutorialPersistenceData.TutorialsEnabled = this.TutorialsEnabled;
			return tutorialPersistenceData;
		}

		public void SetCurrentPersistentState(PersistentManager.PersistentData data, bool isloading, string dataPath)
		{
			TutorialManager.TutorialPersistenceData tutorialPersistenceData = (TutorialManager.TutorialPersistenceData)data;
			foreach (KeyValuePair<string, bool> keyValuePair in tutorialPersistenceData.Tutorials)
			{
				if (this.tutorials.ContainsKey(keyValuePair.Key))
				{
					this.tutorials[keyValuePair.Key].unlocked = keyValuePair.Value;
				}
			}
		}

		private const string TUTORIAL_RESOUCE_DIR = "Tutorial/";

		public bool TutorialsEnabled = true;

		private Dictionary<string, Tutorial> tutorials = new Dictionary<string, Tutorial>();

		private bool IsShowwingTutorial;

		private const string PERSITENT_ID = "ID_TUTORIAL";

		[Serializable]
		public class TutorialPersistenceData : PersistentManager.PersistentData
		{
			public TutorialPersistenceData() : base("ID_TUTORIAL")
			{
			}

			public Dictionary<string, bool> Tutorials = new Dictionary<string, bool>();

			public bool TutorialsEnabled = true;
		}
	}
}
