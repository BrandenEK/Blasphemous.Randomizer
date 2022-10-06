using System;
using UnityEngine;

namespace Framework.Util
{
	public class FPSDisplay : MonoBehaviour
	{
		private void Awake()
		{
			base.enabled = Debug.isDebugBuild;
		}

		private void Start()
		{
			this._style = new GUIStyle();
			this.SetFontSize(this._style);
			this._label = "{0:0.0} ms ({1:0.} fps)";
			this._screenRect = this.GetScreenRect();
		}

		private void Update()
		{
			this._deltaTime += (Time.unscaledDeltaTime - this._deltaTime) * 0.1f;
		}

		private void OnGUI()
		{
			if (!Debug.isDebugBuild)
			{
				return;
			}
			float num = this._deltaTime * 1000f;
			float num2 = 1f / this._deltaTime;
			this._style.normal.textColor = ((num2 < 30f) ? Color.red : Color.cyan);
			string text = string.Format(this._label, num, num2);
			GUI.Label(this._screenRect, text, this._style);
		}

		private Rect GetScreenRect()
		{
			int width = Screen.width;
			int height = Screen.height;
			float num = (float)(height * 2) / 100f;
			return new Rect(0f, 0f, (float)width, num);
		}

		private void SetFontSize(GUIStyle style)
		{
			if (style == null)
			{
				return;
			}
			style.alignment = 2;
			style.fontSize = Screen.height * 2 / 100;
		}

		private float _deltaTime;

		private string _label;

		private GUIStyle _style;

		private Rect _screenRect;
	}
}
