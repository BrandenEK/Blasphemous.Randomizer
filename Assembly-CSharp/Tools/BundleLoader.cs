using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tools
{
	[ExecuteInEditMode]
	[DefaultExecutionOrder(2)]
	public class BundleLoader : MonoBehaviour
	{
		private bool IsMainScene
		{
			get
			{
				return SceneManager.GetActiveScene().name.EndsWith("MAIN");
			}
		}

		private void Start()
		{
		}

		private void LoadEditorBundle()
		{
		}
	}
}
