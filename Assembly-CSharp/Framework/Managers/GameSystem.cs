using System;
using UnityEngine;

namespace Framework.Managers
{
	public class GameSystem
	{
		public virtual void Initialize()
		{
		}

		public virtual void AllPreInitialized()
		{
		}

		public virtual void AllInitialized()
		{
		}

		public virtual void Awake()
		{
		}

		public virtual void Start()
		{
		}

		public virtual void Update()
		{
		}

		public virtual void Dispose()
		{
		}

		public virtual void OnGUI()
		{
		}

		protected void DebugResetLine()
		{
			this.posYGUI = 10;
		}

		protected void DebugDrawTextLine(string text, int posx = 10, int sizex = 1500)
		{
			if (GameSystem.logStyle == null)
			{
				int num = 1;
				Color[] array = new Color[num * num];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = new Color(0f, 0f, 0f, 0.2f);
				}
				GameSystem.background = new Texture2D(num, num, 5, false);
				GameSystem.background.SetPixels(array);
				GameSystem.background.Apply();
				GameSystem.logStyle = new GUIStyle();
				GameSystem.logStyle.font = Resources.Load<Font>("consolefont");
				GameSystem.logStyle.normal.textColor = new Color(255f, 255f, 255f);
				GameSystem.logStyle.normal.background = GameSystem.background;
				GameSystem.logStyle.fontSize = 10;
			}
			Rect rect;
			rect..ctor((float)posx, (float)this.posYGUI, (float)sizex, 13f);
			GUI.Label(rect, text, GameSystem.logStyle);
			this.posYGUI += 13;
		}

		private static GUIStyle logStyle;

		private static Texture2D background;

		private const string FONT_NAME = "consolefont";

		public bool ShowDebug;

		private int posYGUI;

		private const int FontSize = 10;

		private const int SeparationY = 3;
	}
}
