using System;
using System.Collections;
using UnityEngine;

namespace Gameplay.UI.Others.MenuLogic
{
	public class GameMenu : MonoBehaviour
	{
		private void Start()
		{
			this.group = base.GetComponent<CanvasGroup>();
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Escape) && this.group.alpha == 0f)
			{
				base.StartCoroutine(this.Show());
			}
			else if (Input.GetKeyDown(KeyCode.Escape) && this.group.alpha == 1f)
			{
				base.StartCoroutine(this.Hide());
			}
		}

		private IEnumerator Show()
		{
			while (this.group.alpha < 1f)
			{
				yield return new WaitForEndOfFrame();
				this.group.alpha += Time.deltaTime * this.speed;
			}
			this.group.interactable = true;
			yield break;
		}

		private IEnumerator Hide()
		{
			while (this.group.alpha > 0f)
			{
				yield return new WaitForEndOfFrame();
				this.group.alpha -= Time.deltaTime * this.speed;
			}
			this.group.interactable = false;
			yield break;
		}

		public void ImportSouls()
		{
		}

		private CanvasGroup group;

		private float speed = 2f;
	}
}
