using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using I2.Loc;
using Rewired;
using Rewired.Data;
using Rewired.Data.Mapping;
using Tools;
using UnityEngine;

namespace Framework.Managers
{
	public class ControlRemapManager : GameSystem
	{
		public bool listeningForInput { get; private set; }

		public bool ListeningForInputDone
		{
			get
			{
				return !this.listeningForInput && this.timeSinceStoppingListening == 0;
			}
		}

		private Player player
		{
			get
			{
				return ReInput.players.GetPlayer(0);
			}
		}

		private ControllerMap controllerMap
		{
			get
			{
				if (this.controller == null)
				{
					return null;
				}
				return this.player.controllers.maps.GetMap(this.controller.type, this.controller.id, "Default", "Default");
			}
		}

		private ControllerMap mouseControllerMap
		{
			get
			{
				if (this.controller == null)
				{
					return null;
				}
				return this.player.controllers.maps.GetMap(1, this.player.controllers.Mouse.id, "Default", "Default");
			}
		}

		private Controller controller
		{
			get
			{
				return Core.Input.ActiveController;
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event ControlRemapManager.InputMappedDelegate InputMappedEvent;

		public override void Initialize()
		{
			this.keyboardAndJoystickInputMapper.options.timeout = 0f;
			this.mouseInputMapper.options.timeout = 0f;
			this.keyboardAndJoystickInputMapper.options.ignoreMouseXAxis = true;
			this.keyboardAndJoystickInputMapper.options.ignoreMouseYAxis = true;
			this.mouseInputMapper.options.ignoreMouseXAxis = false;
			this.mouseInputMapper.options.ignoreMouseYAxis = false;
			this.keyboardAndJoystickInputMapper.options.allowAxes = true;
			this.mouseInputMapper.options.allowAxes = true;
			this.keyboardAndJoystickInputMapper.options.allowKeyboardModifierKeyAsPrimary = true;
			this.keyboardAndJoystickInputMapper.options.allowKeyboardKeysWithModifiers = false;
			this.keyboardAndJoystickInputMapper.options.isElementAllowedCallback = new Predicate<ControllerPollingInfo>(this.OnIsElementAllowed);
			this.mouseInputMapper.options.isElementAllowedCallback = new Predicate<ControllerPollingInfo>(this.OnIsElementAllowed);
			this.keyboardAndJoystickInputMapper.InputMappedEvent += this.OnInputMapped;
			this.keyboardAndJoystickInputMapper.StoppedEvent += this.OnStopped;
			this.keyboardAndJoystickInputMapper.ConflictFoundEvent += this.OnConflictFound;
			this.mouseInputMapper.InputMappedEvent += this.OnInputMapped;
			this.mouseInputMapper.StoppedEvent += this.OnStopped;
			this.mouseInputMapper.ConflictFoundEvent += this.OnConflictFound;
			Core.Input.JoystickPressed += this.ActiveInputChanged;
			Core.Input.KeyboardPressed += this.ActiveInputChanged;
			ReInput.ControllerConnectedEvent += this.OnControllerConnected;
		}

		public override void Update()
		{
			if (this.controllerMap == null || this.controllerMap.AllMaps.Count == 0)
			{
				return;
			}
			if (this.prevControllerMap == null)
			{
				this.prevControllerMap = this.controllerMap;
			}
			if (!this.listeningForInput && this.timeSinceStoppingListening > 0)
			{
				this.timeSinceStoppingListening--;
			}
			if (!this.definedConflictingActions)
			{
				this.DefineConflictingActions();
				this.definedConflictingActions = true;
			}
			if (!this.readControlsSetting)
			{
				this.DeleteDeprecatedControlsSettingsFiles();
				this.LoadKeyboardAndMouseControlsSettingsFromFile();
				this.InitialLoadJoystickControlsSettingsFromFile();
				this.readControlsSetting = true;
			}
			this.player.controllers.hasMouse = true;
		}

		private void InitialLoadJoystickControlsSettingsFromFile()
		{
			if (ReInput.players == null || ReInput.players.playerCount == 0)
			{
				return;
			}
			Player player = ReInput.players.GetPlayer(0);
			if (player.controllers == null || player.controllers.joystickCount <= 0)
			{
				return;
			}
			foreach (Joystick joystick in player.controllers.Joysticks)
			{
				this.LoadJoystickControlsSettingsFromFile(joystick);
			}
		}

		private void DefineConflictingActions()
		{
			this.InitializeUserUnassignableActions();
			this.InitializeExceptionsWithoutConflict();
			this.InitializeConflictingActions();
			foreach (string text in this.conflictingActions.Keys)
			{
				foreach (string text2 in this.conflictingActions.Keys)
				{
					if (!text2.Equals(text))
					{
						if (!this.exceptionsWithoutConflictActions.ContainsKey(text2) || !this.exceptionsWithoutConflictActions[text2].Contains(text))
						{
							this.conflictingActions[text].Add(text2);
						}
					}
				}
			}
		}

		private void InitializeUserUnassignableActions()
		{
			this.userUnassignableActionIds.Add(39);
			this.userUnassignableActionIds.Add(28);
			this.userUnassignableActionIds.Add(64);
			this.userUnassignableActionIds.Add(29);
			this.userUnassignableActionIds.Add(45);
			this.userUnassignableActionIds.Add(43);
			this.userUnassignableActionIds.Add(51);
			this.userUnassignableActionIds.Add(52);
			this.userUnassignableActionIds.Add(48);
			this.userUnassignableActionIds.Add(50);
			this.userUnassignableActionIds.Add(49);
		}

		private void InitializeExceptionsWithoutConflict()
		{
			string name = ReInput.mapping.GetAction(65).name;
			string name2 = ReInput.mapping.GetAction(57).name;
			string name3 = ReInput.mapping.GetAction(7).name;
			this.exceptionsWithoutConflictActions[name] = new List<string>();
			this.exceptionsWithoutConflictActions[name].Add(name2);
			this.exceptionsWithoutConflictActions[name].Add(name3);
			this.exceptionsWithoutConflictActions[name2] = new List<string>();
			this.exceptionsWithoutConflictActions[name2].Add(name);
			this.exceptionsWithoutConflictActions[name3] = new List<string>();
			this.exceptionsWithoutConflictActions[name3].Add(name);
		}

		private void InitializeConflictingActions()
		{
			List<ActionElementMap> list = new List<ActionElementMap>(this.mouseControllerMap.AllMaps);
			list.AddRange(this.controllerMap.AllMaps);
			foreach (ActionElementMap actionElementMap in list)
			{
				if (!this.userUnassignableActionIds.Contains(actionElementMap.actionId))
				{
					string actionNameWithPolarity = this.GetActionNameWithPolarity(actionElementMap);
					if (!this.conflictingActions.ContainsKey(actionNameWithPolarity))
					{
						this.conflictingActions[actionNameWithPolarity] = new List<string>();
					}
				}
			}
		}

		public List<string> GetAllActionNamesInOrder()
		{
			List<string> list = new List<string>();
			foreach (int num in new List<int>
			{
				4,
				0,
				5,
				6,
				8,
				7,
				38,
				57,
				65,
				23,
				25,
				22,
				10,
				20,
				21
			})
			{
				InputAction action = ReInput.mapping.GetAction(num);
				string text = this.GetActionNameWithPolarity(action);
				list.Add(text);
				if (text.Contains("Pos"))
				{
					text = text.Replace("Pos", "Neg");
					list.Add(text);
				}
			}
			return list;
		}

		public Dictionary<string, ActionElementMap> GetRebindeableAemsByActionName(List<string> actionNames)
		{
			Dictionary<string, ActionElementMap> dictionary = new Dictionary<string, ActionElementMap>();
			foreach (string key in actionNames)
			{
				dictionary[key] = null;
			}
			List<ActionElementMap> list = new List<ActionElementMap>();
			list.AddRange(this.controllerMap.AllMaps);
			if (this.controllerMap.controllerType == null)
			{
				IList<ActionElementMap> allMaps = this.mouseControllerMap.AllMaps;
				foreach (ActionElementMap actionElementMap in allMaps)
				{
					if (actionElementMap.actionId != 51 && actionElementMap.actionId != 52 && actionElementMap.actionId != 48 && actionElementMap.actionId != 50 && actionElementMap.actionId != 49)
					{
						list.Add(actionElementMap);
					}
				}
			}
			list.Sort((ActionElementMap x, ActionElementMap y) => x.elementIndex.CompareTo(y.elementIndex));
			foreach (ActionElementMap actionElementMap2 in list)
			{
				if (!this.userUnassignableActionIds.Contains(actionElementMap2.actionId))
				{
					string actionNameWithPolarity = this.GetActionNameWithPolarity(actionElementMap2);
					dictionary[actionNameWithPolarity] = actionElementMap2;
				}
			}
			return dictionary;
		}

		public void StartListeningInput(int actionElementMapId)
		{
			ActionElementMap firstElementMapMatch = this.controllerMap.GetFirstElementMapMatch((ActionElementMap x) => x.id == actionElementMapId);
			if (firstElementMapMatch == null && this.controllerMap.controllerType == null && this.player.controllers.hasMouse)
			{
				firstElementMapMatch = this.mouseControllerMap.GetFirstElementMapMatch((ActionElementMap x) => x.id == actionElementMapId);
			}
			if (firstElementMapMatch != null)
			{
				if (!this.listeningForInput)
				{
					this.listeningForInput = true;
					this.currentActionElementMapId = firstElementMapMatch.id;
					AxisRange actionRange = (firstElementMapMatch.axisContribution != null) ? 2 : 1;
					this.keyboardAndJoystickInputMapper.Start(new InputMapper.Context
					{
						actionId = firstElementMapMatch.actionId,
						controllerMap = this.controllerMap,
						actionRange = actionRange,
						actionElementMapToReplace = this.controllerMap.GetElementMap(firstElementMapMatch.id)
					});
					if (this.controllerMap.controllerType == null && this.player.controllers.hasMouse)
					{
						ControllerMap map = this.player.controllers.maps.GetMap(1, this.player.controllers.Mouse.id, "Default", "Default");
						this.mouseInputMapper.Start(new InputMapper.Context
						{
							actionId = firstElementMapMatch.actionId,
							controllerMap = map,
							actionRange = actionRange,
							actionElementMapToReplace = map.GetElementMap(firstElementMapMatch.id)
						});
					}
				}
			}
			else
			{
				Debug.Log("Found no action element map assigned to action element map id: " + actionElementMapId);
			}
		}

		public void StopListeningInput()
		{
			this.keyboardAndJoystickInputMapper.Stop();
			this.mouseInputMapper.Stop();
		}

		public void RestoreDefaultMaps()
		{
			ControllerType type = this.controller.type;
			if (type != 2)
			{
				if (type == null)
				{
					this.player.controllers.maps.LoadDefaultMaps(0);
					this.player.controllers.maps.LoadDefaultMaps(1);
				}
			}
			else
			{
				this.player.controllers.maps.LoadDefaultMaps(2);
			}
		}

		public ActionElementMap FindLastElementMapByInputAction(InputAction inputAction, AxisRange axisRange, Controller controller)
		{
			string actionNameWithPolarity = this.GetActionNameWithPolarity(inputAction, axisRange);
			ControllerMap map = this.player.controllers.maps.GetMap(controller.type, controller.id, "Default", "Default");
			return this.FindLastElementMapByActionName(actionNameWithPolarity, map, this.mouseControllerMap);
		}

		private ActionElementMap FindLastElementMapByActionName(string actionName, ControllerMap controllerMap, ControllerMap mouseControllerMap)
		{
			ActionElementMap actionElementMap = null;
			Predicate<ActionElementMap> predicate;
			if (actionName.Contains("Pos"))
			{
				predicate = ((ActionElementMap aem) => this.GetActionNameWithPolarity(aem).Equals(actionName) && aem.axisContribution == 0);
			}
			else if (actionName.Contains("Neg"))
			{
				predicate = ((ActionElementMap aem) => this.GetActionNameWithPolarity(aem).Equals(actionName) && aem.axisContribution == 1);
			}
			else
			{
				predicate = ((ActionElementMap aem) => this.GetActionNameWithPolarity(aem).Equals(actionName));
			}
			ControllerMap map = this.player.controllers.maps.GetMap(this.controller.type, this.controller.id, "Menu", "Default");
			if (controllerMap.controllerType == null && this.player.controllers.hasMouse)
			{
				actionElementMap = this.FindLastElementMapMatch(mouseControllerMap, predicate);
			}
			if (actionElementMap == null)
			{
				actionElementMap = ((!actionName.StartsWith("UI")) ? this.FindLastElementMapMatch(controllerMap, predicate) : this.FindLastElementMapMatch(map, predicate));
			}
			if (actionElementMap == null && this.player.controllers.hasMouse)
			{
				Debug.Log("FindFirstElementMapByActionName: actionElementMap not found! actionName: " + actionName);
			}
			return actionElementMap;
		}

		private List<ActionElementMap> FindAllElementMapsByActionName(string actionName, ControllerMap controllerMap, ControllerMap mouseControllerMap)
		{
			List<ActionElementMap> list = new List<ActionElementMap>();
			Predicate<ActionElementMap> predicate;
			if (actionName.Contains("Pos"))
			{
				predicate = ((ActionElementMap aem) => this.GetActionNameWithPolarity(aem).Equals(actionName) && aem.axisContribution == 0);
			}
			else if (actionName.Contains("Neg"))
			{
				predicate = ((ActionElementMap aem) => this.GetActionNameWithPolarity(aem).Equals(actionName) && aem.axisContribution == 1);
			}
			else
			{
				predicate = ((ActionElementMap aem) => this.GetActionNameWithPolarity(aem).Equals(actionName));
			}
			ControllerMap map = this.player.controllers.maps.GetMap(this.controller.type, this.controller.id, "Menu", "Default");
			if (controllerMap.controllerType == null && this.player.controllers.hasMouse)
			{
				list = this.FindAllElementMapMatch(mouseControllerMap, predicate);
			}
			if (list.Count == 0)
			{
				list = ((!actionName.StartsWith("UI")) ? this.FindAllElementMapMatch(controllerMap, predicate) : this.FindAllElementMapMatch(map, predicate));
			}
			if (list.Count == 0 && this.player.controllers.hasMouse)
			{
				Debug.Log("actionElementMap not found! actionName: " + actionName);
			}
			return list;
		}

		public int CountConflictingActions()
		{
			return this.CountConflictingActions(this.controllerMap, this.mouseControllerMap);
		}

		private int CountConflictingActions(ControllerMap controllerMap)
		{
			return this.CountConflictingActions(controllerMap, this.mouseControllerMap);
		}

		private int CountConflictingActions(ControllerMap controllerMap, ControllerMap mouseControllerMap)
		{
			if (controllerMap == null)
			{
				Debug.Log("CountConflictingActions: controllerMap is null!");
			}
			else if (controllerMap.controllerType == null && this.player.controllers.hasMouse && mouseControllerMap == null)
			{
				Debug.Log("CountConflictingActions: mouseControllerMap is null!");
			}
			int num = 0;
			using (Dictionary<string, List<string>>.KeyCollection.Enumerator enumerator = this.conflictingActions.Keys.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ControlRemapManager.<CountConflictingActions>c__AnonStorey4 <CountConflictingActions>c__AnonStorey = new ControlRemapManager.<CountConflictingActions>c__AnonStorey4();
					<CountConflictingActions>c__AnonStorey.actionName = enumerator.Current;
					<CountConflictingActions>c__AnonStorey.$this = this;
					List<ActionElementMap> list = this.FindAllElementMapsByActionName(<CountConflictingActions>c__AnonStorey.actionName, controllerMap, mouseControllerMap);
					List<string> currentElementMapsIdentifierNames = new List<string>();
					foreach (ActionElementMap actionElementMap in list)
					{
						currentElementMapsIdentifierNames.Add(actionElementMap.elementIdentifierName);
					}
					Predicate<ActionElementMap> predicate = (ActionElementMap aem) => currentElementMapsIdentifierNames.Contains(aem.elementIdentifierName) && <CountConflictingActions>c__AnonStorey.$this.conflictingActions[<CountConflictingActions>c__AnonStorey.actionName].Contains(<CountConflictingActions>c__AnonStorey.$this.GetActionNameWithPolarity(aem));
					List<ActionElementMap> list2 = new List<ActionElementMap>();
					controllerMap.GetElementMapMatches(predicate, list2);
					if (list2.Count > 0)
					{
						Debug.Log("CountConflictingActions: Conflict! actionName: " + <CountConflictingActions>c__AnonStorey.actionName);
						num++;
					}
					else if (controllerMap.controllerType == null && this.player.controllers.hasMouse)
					{
						mouseControllerMap.GetElementMapMatches(predicate, list2);
						if (list2.Count > 0)
						{
							Debug.Log("CountConflictingActions: Conflict! actionName: " + <CountConflictingActions>c__AnonStorey.actionName);
							num++;
						}
					}
				}
			}
			return num;
		}

		public Dictionary<int, string> GetAllCurrentConflictingButtonsByAemId()
		{
			Dictionary<int, string> dictionary = new Dictionary<int, string>();
			using (Dictionary<string, List<string>>.KeyCollection.Enumerator enumerator = this.conflictingActions.Keys.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ControlRemapManager.<GetAllCurrentConflictingButtonsByAemId>c__AnonStorey6 <GetAllCurrentConflictingButtonsByAemId>c__AnonStorey = new ControlRemapManager.<GetAllCurrentConflictingButtonsByAemId>c__AnonStorey6();
					<GetAllCurrentConflictingButtonsByAemId>c__AnonStorey.actionName = enumerator.Current;
					<GetAllCurrentConflictingButtonsByAemId>c__AnonStorey.$this = this;
					List<ActionElementMap> list = this.FindAllElementMapsByActionName(<GetAllCurrentConflictingButtonsByAemId>c__AnonStorey.actionName, this.controllerMap, this.mouseControllerMap);
					List<string> currentElementMapsIdentifierNames = new List<string>();
					foreach (ActionElementMap actionElementMap in list)
					{
						currentElementMapsIdentifierNames.Add(actionElementMap.elementIdentifierName);
					}
					Predicate<ActionElementMap> predicate = delegate(ActionElementMap aem)
					{
						if (currentElementMapsIdentifierNames.Contains(aem.elementIdentifierName))
						{
						}
						return currentElementMapsIdentifierNames.Contains(aem.elementIdentifierName) && <GetAllCurrentConflictingButtonsByAemId>c__AnonStorey.$this.conflictingActions[<GetAllCurrentConflictingButtonsByAemId>c__AnonStorey.actionName].Contains(<GetAllCurrentConflictingButtonsByAemId>c__AnonStorey.$this.GetActionNameWithPolarity(aem));
					};
					List<ActionElementMap> list2 = new List<ActionElementMap>();
					this.controllerMap.GetElementMapMatches(predicate, list2);
					if (list2.Count == 0 && this.controllerMap.controllerType == null && this.player.controllers.hasMouse)
					{
						this.mouseControllerMap.GetElementMapMatches(predicate, list2);
					}
					if (list2.Count > 0)
					{
						Debug.Log(string.Concat(new object[]
						{
							"Conflict! actionName: ",
							<GetAllCurrentConflictingButtonsByAemId>c__AnonStorey.actionName,
							", searchResults.Count: ",
							list2.Count
						}));
						foreach (ActionElementMap actionElementMap2 in list)
						{
							dictionary[actionElementMap2.id] = actionElementMap2.elementIdentifierName;
						}
					}
				}
			}
			return dictionary;
		}

		private void ActiveInputChanged()
		{
			if (this.controllerMap == null || this.prevControllerMap == null)
			{
				return;
			}
			int num = this.CountConflictingActions(this.prevControllerMap);
			if (this.prevControllerMap.controllerType == null)
			{
				if (num > 0)
				{
					this.player.controllers.maps.LoadDefaultMaps(0);
					this.player.controllers.maps.LoadDefaultMaps(1);
				}
				Controller mouse = this.player.controllers.Mouse;
				this.player.controllers.RemoveController(mouse);
			}
			else if (this.prevControllerMap.controllerType != null)
			{
				if (num > 0)
				{
					this.player.controllers.maps.LoadDefaultMaps(2);
				}
				if (!this.player.controllers.hasMouse)
				{
					Controller mouse2 = this.player.controllers.Mouse;
					this.player.controllers.AddController(mouse2, false);
				}
			}
			this.prevControllerMap = this.controllerMap;
		}

		private string GetActionNameWithPolarity(InputAction inputAction)
		{
			string result = string.Empty;
			if (inputAction == null)
			{
				Debug.LogError("GetActionNameWithPolarity: inputAction is null!");
			}
			else if (inputAction.type == 1)
			{
				result = inputAction.name;
			}
			else
			{
				result = inputAction.name + " Pos";
			}
			return result;
		}

		private string GetActionNameWithPolarity(InputAction inputAction, AxisRange axisRange)
		{
			string result = string.Empty;
			if (inputAction == null)
			{
				Debug.LogError("GetActionNameWithPolarity: inputAction is null!");
			}
			else if (inputAction.type == 1)
			{
				result = inputAction.name;
			}
			else if (axisRange == 2)
			{
				result = inputAction.name + " Neg";
			}
			else
			{
				result = inputAction.name + " Pos";
			}
			return result;
		}

		private string GetActionNameWithPolarity(ActionElementMap actionElementMap)
		{
			string result = string.Empty;
			if (actionElementMap == null)
			{
				Debug.LogError("GetActionNameWithPolarity: actionElementMap is null!");
			}
			else
			{
				InputAction action = ReInput.mapping.GetAction(actionElementMap.actionId);
				if (action.type == 1)
				{
					result = action.name;
				}
				else if (actionElementMap.axisContribution == null)
				{
					result = action.name + " Pos";
				}
				else
				{
					result = action.name + " Neg";
				}
			}
			return result;
		}

		public string LocalizeActionName(string actionName)
		{
			string result = string.Empty;
			string text = "UI_Controls/" + actionName.Replace(" ", "_").ToUpperInvariant();
			string text2 = ScriptLocalization.Get(text, true, 0, true, false, null, null);
			if (string.IsNullOrEmpty(text2))
			{
				result = text;
				Debug.LogError("Action Name: '" + text + "' has no localization term!");
			}
			else
			{
				result = text2;
			}
			return result;
		}

		private ActionElementMap FindLastElementMapMatch(ControllerMap controllerMap, Predicate<ActionElementMap> predicate)
		{
			ActionElementMap result = null;
			List<ActionElementMap> list = new List<ActionElementMap>();
			controllerMap.GetElementMapMatches(predicate, list);
			if (list.Count > 0)
			{
				result = list[list.Count - 1];
			}
			return result;
		}

		private List<ActionElementMap> FindAllElementMapMatch(ControllerMap controllerMap, Predicate<ActionElementMap> predicate)
		{
			List<ActionElementMap> list = new List<ActionElementMap>();
			controllerMap.GetElementMapMatches(predicate, list);
			return list;
		}

		private void OnInputMapped(InputMapper.InputMappedEventData data)
		{
			ActionElementMap actionElementMap = data.actionElementMap;
			Debug.Log("Button " + actionElementMap.elementIdentifierName + " is now assigned to the Action " + ReInput.mapping.GetAction(actionElementMap.actionId).name);
			Debug.Log("It has been assigned with pole: " + actionElementMap.axisContribution);
			if (ControlRemapManager.InputMappedEvent != null)
			{
				ControlRemapManager.InputMappedEvent(actionElementMap.elementIdentifierName, actionElementMap.id);
				ControlRemapManager.InputMappedEvent = null;
			}
		}

		private void OnStopped(InputMapper.StoppedEventData data)
		{
			if (data.inputMapper.Equals(this.keyboardAndJoystickInputMapper))
			{
				if (this.mouseInputMapper.status == 1)
				{
					this.mouseInputMapper.Stop();
				}
			}
			else
			{
				this.keyboardAndJoystickInputMapper.Stop();
			}
			this.currentActionElementMapId = -1;
			ControlRemapManager.InputMappedEvent = null;
			this.listeningForInput = false;
			this.timeSinceStoppingListening = 30;
		}

		private void OnConflictFound(InputMapper.ConflictFoundEventData data)
		{
			Debug.Log("OnConflictFound: data.assignment.action.name: " + data.assignment.action.name);
			Debug.Log("OnConflictFound: data.conflicts[0].action.name: " + data.conflicts[0].action.name);
			if (data.isProtected)
			{
				data.responseCallback(0);
			}
			else
			{
				data.responseCallback(2);
			}
		}

		private bool OnIsElementAllowed(ControllerPollingInfo info)
		{
			bool flag = true;
			if (info.controllerType == 1)
			{
				string text = info.elementIdentifierName.ToUpper();
			}
			else if (info.controllerType == null)
			{
				if (info.keyboardKey == 27 || info.keyboardKey == 271 || info.keyboardKey == 13)
				{
					flag = false;
				}
			}
			else
			{
				string item = info.elementIdentifierName.ToUpper();
				if (this.consoleButtonsNotAllowed.Contains(item))
				{
					flag = false;
				}
			}
			if (flag)
			{
				if (this.controllerMap.GetFirstElementMapMatch((ActionElementMap aem) => aem.id == this.currentActionElementMapId) != null)
				{
					this.controllerMap.DeleteElementMap(this.currentActionElementMapId);
				}
				else
				{
					this.mouseControllerMap.DeleteElementMap(this.currentActionElementMapId);
				}
			}
			return flag;
		}

		private string GetPathControlsSettings()
		{
			return PersistentManager.GetPathAppSettings("/keybinding_settings");
		}

		private string GetPathOldControlsSettings()
		{
			return PersistentManager.GetPathAppSettings("/controls_settings");
		}

		private void DeleteDeprecatedControlsSettingsFiles()
		{
			InputManager inputManager = Object.FindObjectOfType<InputManager>();
			ControllerDataFiles controllerDataFiles = null;
			if (inputManager != null)
			{
				controllerDataFiles = inputManager.dataFiles;
			}
			List<ControllerMap> list = new List<ControllerMap>(this.player.controllers.maps.GetAllMapsInCategory(0));
			foreach (ControllerMap controllerMap in list)
			{
				string path = string.Empty;
				if (controllerMap.controllerType == 2)
				{
					if (controllerDataFiles == null)
					{
						continue;
					}
					HardwareJoystickMap hardwareJoystickMap = controllerDataFiles.GetHardwareJoystickMap(controllerMap.hardwareGuid);
					if (hardwareJoystickMap == null)
					{
						continue;
					}
					path = this.GetPathOldControlsSettings() + "_" + hardwareJoystickMap.ControllerName.ToLowerInvariant().Replace(' ', '_') + ".xml";
				}
				else
				{
					path = this.GetPathOldControlsSettings() + "_" + controllerMap.controllerType.ToString().ToLowerInvariant() + ".xml";
				}
				if (File.Exists(path))
				{
					File.Delete(path);
				}
			}
		}

		public void LoadKeyboardAndMouseControlsSettingsFromFile()
		{
			List<ControllerMap> list = new List<ControllerMap>(this.player.controllers.maps.GetAllMapsInCategory(0));
			foreach (ControllerMap controllerMap in list)
			{
				if (controllerMap.controllerType != 2)
				{
					string text = this.GetPathControlsSettings() + "_" + controllerMap.controllerType.ToString().ToLowerInvariant() + ".xml";
					if (File.Exists(text))
					{
						string text2 = File.ReadAllText(text);
						if (string.IsNullOrEmpty(text2))
						{
							Debug.LogError("LoadKeyboardAndMouseControlsSettingsFromFile: there is no data inside the controls settings file with path: " + text);
						}
						else
						{
							ControllerMap controllerMap2 = ControllerMap.CreateFromXml(controllerMap.controllerType, text2);
							ControllerType controllerType = controllerMap.controllerType;
							if (controllerType != null)
							{
								if (controllerType == 1)
								{
									this.player.controllers.maps.AddMap<MouseMap>(controllerMap.id, controllerMap2);
								}
							}
							else
							{
								this.player.controllers.maps.AddMap<KeyboardMap>(controllerMap.controllerId, controllerMap2);
							}
						}
					}
					else
					{
						Debug.Log("LoadKeyboardAndMouseControlsSettingsFromFile: file not found, loading deafult maps");
						this.player.controllers.maps.LoadDefaultMaps(controllerMap.controllerType);
					}
				}
			}
			bool flag = this.FixOldActions(0, 64);
			flag |= this.FixOldActions(0, 65);
			if (flag)
			{
				Core.ControlRemapManager.WriteControlsSettingsToFile();
			}
		}

		public void OnControllerConnected(ControllerStatusChangedEventArgs args)
		{
			if (args.controllerType == 2)
			{
				Joystick joystick = (Joystick)this.player.controllers.GetController(2, args.controllerId);
				this.LoadJoystickControlsSettingsFromFile(joystick);
			}
		}

		public void LoadJoystickControlsSettingsFromFile(Joystick joystick)
		{
			string text = joystick.name.ToLowerInvariant().Replace(' ', '_');
			if (this.joystickSettingsCache.ContainsKey(text))
			{
				ControllerMap controllerMap = ControllerMap.CreateFromXml(2, this.joystickSettingsCache[text]);
				this.player.controllers.maps.AddMap<JoystickMap>(joystick.id, controllerMap);
			}
			else
			{
				string text2 = this.GetPathControlsSettings() + "_" + text + ".xml";
				if (File.Exists(text2))
				{
					Debug.Log("Loading mapping definitions for :" + text);
					string text3 = File.ReadAllText(text2);
					if (string.IsNullOrEmpty(text3))
					{
						Debug.LogError("LoadJoystickControlsSettingsFromFile: there is no data inside the controls settings file with path: " + text2);
					}
					else
					{
						ControllerMap controllerMap2 = ControllerMap.CreateFromXml(2, text3);
						this.player.controllers.maps.AddMap<JoystickMap>(joystick.id, controllerMap2);
						this.joystickSettingsCache.Add(text, text3);
					}
				}
				else
				{
					Debug.Log("LoadJoystickControlsSettingsFromFile: file not found, loading deafult maps. path: " + text2);
					this.player.controllers.maps.LoadDefaultMaps(2);
				}
			}
			bool flag = this.FixOldActions(2, 64);
			flag |= this.FixOldActions(2, 65);
			if (flag)
			{
				Core.ControlRemapManager.WriteControlsSettingsToFile();
			}
		}

		private bool FixOldActions(ControllerType mapType, int requiredAction)
		{
			bool flag = false;
			foreach (ControllerMap controllerMap in this.player.controllers.maps.GetAllMaps(mapType))
			{
				if (controllerMap.ContainsAction(requiredAction))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				Debug.LogWarningFormat("Missing action: {0}, restoring default controls for {1}", new object[]
				{
					requiredAction,
					mapType
				});
				this.player.controllers.maps.LoadDefaultMaps(mapType);
			}
			return !flag;
		}

		public void WriteControlsSettingsToFile()
		{
			InputManager inputManager = Object.FindObjectOfType<InputManager>();
			ControllerDataFiles controllerDataFiles = null;
			if (inputManager != null)
			{
				controllerDataFiles = inputManager.dataFiles;
			}
			foreach (ControllerMap controllerMap in this.player.controllers.maps.GetAllMaps())
			{
				if (controllerMap.categoryId != 4)
				{
					ControllerType controllerType = controllerMap.controllerType;
					switch (controllerType)
					{
					case 0:
					case 1:
						break;
					case 2:
					{
						if (controllerDataFiles == null)
						{
							continue;
						}
						HardwareJoystickMap hardwareJoystickMap = controllerDataFiles.GetHardwareJoystickMap(controllerMap.hardwareGuid);
						if (hardwareJoystickMap == null)
						{
							continue;
						}
						string text = this.player.controllers.GetController<Joystick>(controllerMap.controllerId).name.ToLowerInvariant().Replace(' ', '_');
						string path = this.GetPathControlsSettings() + "_" + text + ".xml";
						if (!File.Exists(path))
						{
							File.CreateText(path).Close();
						}
						string text2 = controllerMap.ToXmlString();
						FileTools.SaveSecure(path, text2);
						this.joystickSettingsCache[text] = text2;
						continue;
					}
					default:
						if (controllerType != 20)
						{
							continue;
						}
						break;
					}
					string path2 = this.GetPathControlsSettings() + "_" + controllerMap.controllerType.ToString().ToLowerInvariant() + ".xml";
					if (!File.Exists(path2))
					{
						File.CreateText(path2).Close();
					}
					string encryptedData = controllerMap.ToXmlString();
					FileTools.SaveSecure(path2, encryptedData);
				}
			}
		}

		private const string DEFAULT_CATEGORY = "Default";

		private const string MENU_CATEGORY = "Menu";

		private const string DEFAULT_LAYOUT = "Default";

		private const string OLD_CONTROLS_SETTINGS_PATH = "/controls_settings";

		private const string CONTROLS_SETTINGS_PATH = "/keybinding_settings";

		private readonly Dictionary<string, List<string>> conflictingActions = new Dictionary<string, List<string>>();

		private readonly Dictionary<string, List<string>> exceptionsWithoutConflictActions = new Dictionary<string, List<string>>();

		private readonly InputMapper keyboardAndJoystickInputMapper = new InputMapper();

		private readonly InputMapper mouseInputMapper = new InputMapper();

		private readonly List<int> userUnassignableActionIds = new List<int>();

		private readonly List<string> consoleButtonsNotAllowed = new List<string>
		{
			"TOUCH PAD",
			"LEFT STICK UP",
			"LEFT STICK DOWN",
			"LEFT STICK LEFT",
			"LEFT STICK RIGHT",
			"PS BUTTON"
		};

		private int currentActionElementMapId;

		private bool definedConflictingActions;

		private ControllerMap prevControllerMap;

		private bool readControlsSetting;

		private Dictionary<string, string> joystickSettingsCache = new Dictionary<string, string>();

		private const int ListeningCooldown = 30;

		private int timeSinceStoppingListening;

		public delegate void InputMappedDelegate(string newButtonName = "", int newActionElementMapId = -1);
	}
}
