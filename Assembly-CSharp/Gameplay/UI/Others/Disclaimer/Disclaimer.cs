using System;
using System.Collections;
using Framework.Managers;
using UnityEngine;

namespace Gameplay.UI.Others.Disclaimer
{
	public class Disclaimer : MonoBehaviour
	{
		private void Awake()
		{
			base.StartCoroutine(this.GoToMainMenu());
		}

		public IEnumerator GoToMainMenu()
		{
			yield return new WaitForSeconds(this.waitTime);
			Core.Logic.LoadMenuScene(true);
			yield break;
		}

		public float waitTime;
	}
}
