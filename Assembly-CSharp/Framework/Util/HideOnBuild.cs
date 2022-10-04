using System;
using UnityEngine;

namespace Framework.Util
{
	public class HideOnBuild : MonoBehaviour
	{
		private void Start()
		{
			if (!Application.isEditor && Application.isPlaying)
			{
				this.HideSprites();
				this.HideTexts();
			}
		}

		private void HideSprites()
		{
			SpriteRenderer[] componentsInChildren = base.gameObject.GetComponentsInChildren<SpriteRenderer>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = false;
			}
		}

		private void HideTexts()
		{
			TextMesh[] componentsInChildren = base.gameObject.GetComponentsInChildren<TextMesh>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].gameObject.SetActive(false);
			}
		}
	}
}
