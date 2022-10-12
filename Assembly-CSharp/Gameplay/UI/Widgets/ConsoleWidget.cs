using System;
using System.Collections.Generic;
using System.Linq;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Penitent;
using Gameplay.UI.Console;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Gameplay.UI.Widgets
{
	public class ConsoleWidget : UIWidget
	{
		private void Awake()
		{
			SpawnManager.OnPlayerSpawn += this.OnPlayerSpawn;
		}

		private void OnDestroy()
		{
			SpawnManager.OnPlayerSpawn -= this.OnPlayerSpawn;
		}

		private void OnPlayerSpawn(Penitent penitent)
		{
			this.InitializeCommands();
			for (int i = 0; i < this.commands.Count; i++)
			{
				this.commands[i].Initialize(this);
				this.commands[i].Start();
			}
			this.isEnabled = false;
			this.elements.SetActive(this.isEnabled);
			ConsoleWidget.Instance = this;
		}

		public bool IsEnabled()
		{
			return this.isEnabled;
		}

		private void OnEnable()
		{
			EventSystem.current.SetSelectedGameObject(this.input.gameObject, null);
			this.input.OnPointerClick(new PointerEventData(EventSystem.current));
			this.input.ActivateInputField();
		}

		private void OnDisable()
		{
			if (EventSystem.current != null)
			{
				EventSystem.current.SetSelectedGameObject(null);
			}
			this.EraseInput();
			this.ConsoleIndexToStart();
		}

		private void Update()
		{
			for (int i = 0; i < this.commands.Count; i++)
			{
				this.commands[i].Update();
			}
			if (Input.GetKeyDown(KeyCode.Backslash))
			{
				this.SetEnabled(!this.isEnabled);
			}
			if (this.isEnabled)
			{
				if (this.scrollToBottom)
				{
					this.EnsureVisible();
				}
				this.CheckConsoleKeys();
			}
			if (base.enabled && this.forceScrollPosition)
			{
				this.scrollRect.verticalNormalizedPosition = this.forceScrollPositionValue;
			}
		}

		public List<string> GetAllNames()
		{
			List<string> list = new List<string>();
			foreach (ConsoleCommand consoleCommand in this.commands)
			{
				foreach (string item in consoleCommand.GetNames())
				{
					list.Add(item);
				}
			}
			list.Sort();
			return list;
		}

		public void SetEnabled(bool enabled)
		{
			this.isEnabled = enabled;
			this.elements.SetActive(this.isEnabled);
			Core.Input.SetBlocker("CONSOLE", this.isEnabled);
			if (enabled)
			{
				this.OnEnable();
			}
			else
			{
				this.OnDisable();
			}
		}

		public void ProcessCommand(string rawText)
		{
			string text = rawText.Replace("\r", string.Empty);
			if (text.Trim().Length == 0)
			{
				return;
			}
			string[] array = text.ToLower().Split(new char[]
			{
				' '
			});
			string[] source = text.Split(new char[]
			{
				' '
			});
			string text2 = array[0];
			string[] array2 = array.Skip(1).ToArray<string>();
			string[] parameters = source.Skip(1).ToArray<string>();
			ConsoleCommand consoleCommand = null;
			string commandFromName = this.GetCommandFromName(text2, out consoleCommand);
			if (consoleCommand != null)
			{
				if (consoleCommand.HasLowerParameters())
				{
					string[] parameters2 = (from s in array2
					select s.ToLowerInvariant()).ToArray<string>();
					consoleCommand.Execute(commandFromName, parameters2);
				}
				else if (consoleCommand.ToLowerAll())
				{
					consoleCommand.Execute(commandFromName, array2);
				}
				else
				{
					consoleCommand.Execute(commandFromName, parameters);
				}
			}
			else if (!this.SharedCommand.ExecuteIfIsCommand(text2))
			{
				this.Write("Command not found. Use Help for more information.");
			}
		}

		public void Submit()
		{
			if (this.input.text.IsNullOrWhitespace())
			{
				return;
			}
			ConsoleWidget.previousCommands.Add(this.input.text);
			this.ConsoleIndexToStart();
			this.Write("> " + this.input.text);
			string text = this.input.text;
			if (!this.ProcessInternalCommand(text))
			{
				this.ProcessCommand(text);
			}
			this.EraseInput();
			this.input.ActivateInputField();
			this.scrollToBottom = true;
		}

		public void Write(string message)
		{
			if (this.LineAmount > 200)
			{
				this.ClearLines(1);
			}
			GameObject gameObject = new GameObject();
			Text text = gameObject.AddComponent<Text>();
			text.fontSize = 22;
			text.font = (Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font);
			text.text = message;
			gameObject.transform.SetParent(this.content);
			gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			if (this.mirrorToDebugLog)
			{
				Debug.Log(string.Format("[CONSOLE] - {0}", message));
			}
		}

		public void WriteFormat(string str, params object[] format)
		{
			this.Write(string.Format(str, format));
		}

		public void EraseInput()
		{
			this.input.text = string.Empty;
		}

		public void ClearLines(int amount = -1)
		{
			float num = (float)((amount <= 0 || amount >= this.content.childCount) ? this.content.childCount : amount);
			int num2 = 0;
			while ((float)num2 < num)
			{
				UnityEngine.Object.Destroy(this.content.GetChild(num2).gameObject);
				num2++;
			}
		}

		public int LineAmount
		{
			get
			{
				return this.content.childCount;
			}
		}

		private void InitializeCommands()
		{
			this.commands.Clear();
			this.commands.Add(new Invincible());
			this.commands.Add(new FervourRefill());
			this.commands.Add(new Kill());
			this.commands.Add(new Help());
			this.commands.Add(new Restart());
			this.commands.Add(new LoadLevel());
			this.commands.Add(new LanguageCommand());
			this.commands.Add(new InventoryCommand());
			this.commands.Add(new StatsCommand());
			this.commands.Add(new BonusCommand());
			this.commands.Add(new SendEvent());
			this.commands.Add(new MaxFervour());
			this.commands.Add(new DialogCommand());
			this.commands.Add(new ExitCommand());
			this.commands.Add(new Graybox());
			this.commands.Add(new SaveGameCommand());
			this.commands.Add(new SkillCommand());
			this.commands.Add(new TimescaleCommand());
			this.commands.Add(new TeleportCommand());
			this.commands.Add(new ShowUICommand());
			this.commands.Add(new ExecutionCommand());
			this.commands.Add(new DebugCommand());
			this.commands.Add(new AudioCommand());
			this.commands.Add(new FlagCommand());
			this.commands.Add(new GuiltCommand());
			this.commands.Add(new TestPlanCommand());
			this.commands.Add(new AchievementCommand());
			this.commands.Add(new SkinCommand());
			this.commands.Add(new DebugUICommand());
			this.commands.Add(new MapCommand());
			this.commands.Add(new ShowCursor());
			this.commands.Add(new GameModeCommand());
			this.commands.Add(new PenitenceCommand());
			this.SharedCommand = new SharedCommandsCommand();
			this.commands.Add(this.SharedCommand);
			this.commands.Add(new AlmsCommand());
			this.commands.Add(new TutorialsCommand());
			this.commands.Add(new BossRushCommand());
			this.commands.Add(new MiriamCommand());
			this.commands.Add(new DemakeCommand());
			this.commands.Add(new CameraCommand());
			this.commands.Add(new CompletionCommand());
		}

		private void CheckConsoleKeys()
		{
			string text = string.Empty;
			if (Input.GetKeyDown(KeyCode.Return))
			{
				this.Submit();
			}
			if (Input.GetKeyUp(KeyCode.DownArrow))
			{
				text = this.NextCommand();
			}
			if (Input.GetKeyUp(KeyCode.UpArrow))
			{
				text = this.PreviousCommand();
			}
			if (!text.IsNullOrWhitespace())
			{
				this.input.text = text;
			}
			if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.DownArrow))
			{
				this.CursorToEnd();
			}
			if (Input.GetKeyUp(KeyCode.PageDown))
			{
				this.Scroll(-0.5f);
			}
			if (Input.GetKeyUp(KeyCode.PageUp))
			{
				this.Scroll(0.5f);
			}
		}

		private string PreviousCommand()
		{
			if (ConsoleWidget.previousCommands.Count == 0 || ConsoleWidget.currentInputCommand == 0)
			{
				return string.Empty;
			}
			ConsoleWidget.currentInputCommand--;
			string storedCommand = this.GetStoredCommand();
			Log.Trace("Console", string.Concat(new object[]
			{
				"Current: ",
				ConsoleWidget.currentInputCommand,
				" Count: ",
				ConsoleWidget.previousCommands.Count
			}), null);
			return storedCommand;
		}

		private string NextCommand()
		{
			if (ConsoleWidget.currentInputCommand < ConsoleWidget.previousCommands.Count - 1)
			{
				ConsoleWidget.currentInputCommand++;
			}
			if (ConsoleWidget.previousCommands.Count == 0 || ConsoleWidget.currentInputCommand >= ConsoleWidget.previousCommands.Count)
			{
				return string.Empty;
			}
			Log.Trace("Console", string.Concat(new object[]
			{
				"Current: ",
				ConsoleWidget.currentInputCommand,
				" Count: ",
				ConsoleWidget.previousCommands.Count
			}), null);
			return this.GetStoredCommand();
		}

		private void ConsoleIndexToStart()
		{
			ConsoleWidget.currentInputCommand = ConsoleWidget.previousCommands.Count;
		}

		private void CursorToEnd()
		{
			this.input.caretPosition = this.input.text.Length;
		}

		private string GetStoredCommand()
		{
			return ConsoleWidget.previousCommands[ConsoleWidget.currentInputCommand];
		}

		private void ResetPreviousCommands()
		{
			ConsoleWidget.previousCommands.Clear();
			ConsoleWidget.currentInputCommand = -1;
		}

		private void KeepFocus()
		{
			if (this.isEnabled && !this.input.isFocused)
			{
				EventSystem.current.SetSelectedGameObject(this.input.gameObject, null);
				this.input.OnPointerClick(new PointerEventData(EventSystem.current));
			}
		}

		private void Scroll(float amount)
		{
			float height = this.scrollRect.viewport.rect.height;
			float height2 = this.scrollRect.content.rect.height;
			float num = height / height2;
			this.scrollRect.verticalNormalizedPosition += amount * num;
		}

		private bool ProcessInternalCommand(string cmd)
		{
			bool result = true;
			string text = cmd.ToUpper();
			if (text != null)
			{
				if (text == "CLEAR" || text == "CLS")
				{
					this.ClearLines(-1);
					this.scrollRect.verticalNormalizedPosition = 1f;
					return result;
				}
				if (text == "MIRRORLOG")
				{
					this.mirrorToDebugLog = !this.mirrorToDebugLog;
					this.WriteFormat("Mirror to Unity log: {0}", new object[]
					{
						this.mirrorToDebugLog
					});
					return result;
				}
			}
			result = false;
			return result;
		}

		private void EnsureVisible()
		{
			this.scrollRect.verticalNormalizedPosition = 0f;
			this.scrollToBottom = false;
		}

		private string GetCommandFromName(string id, out ConsoleCommand result)
		{
			string idUpper = id.ToUpper();
			result = null;
			string text = string.Empty;
			foreach (ConsoleCommand consoleCommand in this.commands)
			{
				text = consoleCommand.GetNames().Find((string name) => name.ToUpper() == idUpper);
				if (text != null && text != string.Empty)
				{
					result = consoleCommand;
					break;
				}
			}
			if (result == null)
			{
				foreach (ConsoleCommand consoleCommand2 in this.commands)
				{
					text = consoleCommand2.GetNames().Find((string name) => name.ToUpper().StartsWith(idUpper));
					if (text != null && text != string.Empty)
					{
						result = consoleCommand2;
						break;
					}
				}
			}
			return text;
		}

		public const int MAX_LINES = 200;

		public const int LINES_SHOW = 17;

		public InputField input;

		public RectTransform content;

		public ScrollRect scrollRect;

		public GameObject elements;

		public int scrollSize = 4;

		public bool forceScrollPosition;

		[Range(0f, 1f)]
		public float forceScrollPositionValue;

		private static List<string> previousCommands = new List<string>();

		private static int currentInputCommand = -1;

		public static ConsoleWidget Instance;

		private List<ConsoleCommand> commands = new List<ConsoleCommand>();

		private bool isEnabled;

		private float scrollZero = 189f;

		private float elementSize = 22.115f;

		private bool mirrorToDebugLog;

		private SharedCommandsCommand SharedCommand;

		private bool scrollToBottom;
	}
}
