using System;
using Framework.FrameworkCore;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Tools.Level.Layout;
using UnityEngine;

namespace Framework.Editor
{
	public class BundleToolset : MonoBehaviour
	{
		[Button(ButtonSizes.Small)]
		[UsedImplicitly]
		public void ShowGreybox()
		{
			BundleToolset.ShowGraybox(true);
		}

		[Button(ButtonSizes.Small)]
		[UsedImplicitly]
		public void HideGreybox()
		{
			BundleToolset.ShowGraybox(false);
		}

		private static void ShowGraybox(bool b)
		{
			LayoutElement[] array = UnityEngine.Object.FindObjectsOfType<LayoutElement>();
			Log.Trace("Decoration", string.Concat(new object[]
			{
				"Modifiying graybox visibility. Visible: ",
				b,
				" Afecteds: ",
				array.Length
			}), null);
			for (int i = 0; i < array.Length; i++)
			{
				SpriteRenderer[] componentsInChildren = array[i].GetComponentsInChildren<SpriteRenderer>();
				if (componentsInChildren != null && array[i].showInGame)
				{
					componentsInChildren.ForEach(delegate(SpriteRenderer x)
					{
						x.enabled = true;
					});
				}
				else if (componentsInChildren != null)
				{
					componentsInChildren.ForEach(delegate(SpriteRenderer x)
					{
						x.enabled = b;
					});
				}
			}
		}
	}
}
