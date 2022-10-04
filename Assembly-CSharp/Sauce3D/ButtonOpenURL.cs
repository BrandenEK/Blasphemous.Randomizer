using System;
using UnityEngine;

namespace Sauce3D
{
	public class ButtonOpenURL : MonoBehaviour
	{
		private void OpenUrl()
		{
			Application.OpenURL(this.url);
		}

	}
}
