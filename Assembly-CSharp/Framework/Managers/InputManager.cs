using System;
using System.Collections.Generic;
using System.Diagnostics;
using Framework.FrameworkCore;
using Framework.Util;
using Rewired;
using Tools.DataContainer;
using UnityEngine;

namespace Framework.Managers
{
	public class InputManager : GameSystem
	{
		public bool InputBlocked { get; private set; }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Core.SimpleEvent OnInputLocked;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Core.SimpleEvent OnInputUnlocked;

		private Controller Keyboard { get; set; }

		private Controller PrevKeyboard { get; set; }

		private Controller Joystick { get; set; }

		private Controller PrevJoystick { get; set; }

		public ControllerType ActiveControllerType
		{
			get
			{
				return (!this.UseDebugControllers) ? this._activeControllerType : this.debugControllerType;
			}
			set
			{
				this._activeControllerType = value;
			}
		}

		private bool JoystickConnected
		{
			get
			{
				return ReInput.controllers.Joysticks.Count > 0;
			}
		}

		public JoystickType ActiveJoystickModel
		{
			get
			{
				if (this.UseDebugControllers)
				{
					return this.debugJoystickType;
				}
				IList<Joystick> joysticks = ReInput.controllers.Joysticks;
				Joystick device = null;
				if (this.JoystickConnected)
				{
					Joystick joystick = (Joystick)ReInput.players.GetPlayer(0).controllers.GetLastActiveController(2);
					Joystick joystick2 = (ReInput.players.GetPlayer(0).controllers.joystickCount <= 0) ? null : ReInput.players.GetPlayer(0).controllers.Joysticks[0];
					device = (joystick ?? joystick2);
				}
				return this.GetJoystickType(device);
			}
		}

		public JoystickType GetJoystickType(Joystick device)
		{
			if (device == null)
			{
				return JoystickType.None;
			}
			if (device.name.ToLower().Contains("xinput") || device.hardwareTypeGuid == new Guid("19002688-7406-4f4a-8340-8d25335406c8") || device.hardwareTypeGuid == new Guid("d74a350e-fe8b-4e9e-bbcd-efff16d34115"))
			{
				return JoystickType.XBOX;
			}
			if (device.name.ToLower().Contains("dualshock") || device.hardwareTypeGuid == new Guid("71dfe6c8-9e81-428f-a58e-c7e664b7fbed") || device.hardwareTypeGuid == new Guid("cd9718bf-a87a-44bc-8716-60a0def28a9f") || device.hardwareTypeGuid == new Guid("c3ad3cad-c7cf-4ca8-8c2e-e3df8d9960bb"))
			{
				return JoystickType.PlayStation;
			}
			if (device.name.ToLower().Contains("switch") || device.hardwareTypeGuid == new Guid("7bf3154b-9db8-4d52-950f-cd0eed8a5819") || device.hardwareTypeGuid == new Guid("521b808c-0248-4526-bc10-f1d16ee76bf1") || device.hardwareTypeGuid == new Guid("1fbdd13b-0795-4173-8a95-a2a75de9d204"))
			{
				return JoystickType.Switch;
			}
			return JoystickType.Generic;
		}

		public Controller ActiveController
		{
			get
			{
				ControllerType activeControllerType = this.ActiveControllerType;
				if (activeControllerType == null)
				{
					return this.Keyboard;
				}
				if (activeControllerType != 2)
				{
					return this.Keyboard;
				}
				Joystick joystick = (Joystick)ReInput.players.GetPlayer(0).controllers.GetLastActiveController(2);
				Joystick joystick2 = (ReInput.players.GetPlayer(0).controllers.joystickCount <= 0) ? null : ReInput.players.GetPlayer(0).controllers.Joysticks[0];
				Joystick joystick3;
				if ((joystick3 = joystick) == null)
				{
					joystick3 = (joystick2 ?? this.Keyboard);
				}
				this.Joystick = joystick3;
				return this.Joystick;
			}
		}

		private bool AxisTouched
		{
			get
			{
				Joystick joystick = (Joystick)ReInput.controllers.GetLastActiveController(2);
				if (joystick != null)
				{
					IList<Controller.Axis> axes = joystick.Axes;
					for (int i = 0; i < axes.Count; i++)
					{
						if (axes[i].value != 0f)
						{
							return true;
						}
					}
				}
				return false;
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Core.SimpleEvent KeyboardPressed;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Core.SimpleEvent JoystickPressed;

		public override void Start()
		{
			PlayMakerGUI.LockCursor = false;
			PlayMakerGUI.HideCursor = true;
			Debug.LogWarning("BLOCKING INPUT until main menu is fully shown");
			this.SetBlocker("InitialBlocker", true);
			this.ActiveControllerType = ((!this.JoystickConnected) ? 0 : 2);
			this.Keyboard = ReInput.players.GetPlayer(0).controllers.Keyboard;
			IList<Joystick> joysticks = ReInput.players.GetPlayer(0).controllers.Joysticks;
			this.Joystick = ((joysticks.Count <= 0) ? null : joysticks[0]);
			this.SendInputChangedEvent();
		}

		public override void Update()
		{
			if (!ReInput.isReady)
			{
				return;
			}
			if (!Core.ControlRemapManager.ListeningForInputDone)
			{
				return;
			}
			this.UpdateMainInput();
		}

		private void UpdateMainInput()
		{
			Joystick joystick = (Joystick)ReInput.players.GetPlayer(0).controllers.GetLastActiveController(2);
			if (this.PrevJoystick == null)
			{
				this.PrevJoystick = (this.Joystick ?? joystick);
			}
			if (this.PrevKeyboard == null)
			{
				this.PrevKeyboard = this.Keyboard;
			}
			if (ReInput.controllers.Keyboard.GetAnyButton() || ReInput.controllers.Mouse.GetAnyButton())
			{
				this.SetMainInput(0, this.Keyboard.id);
				this.PrevKeyboard = this.Keyboard;
			}
			else if (this.JoystickConnected && joystick != null && (joystick.GetAnyButton() || this.AxisTouched))
			{
				this.SetMainInput(2, this.PrevJoystick.id);
				this.PrevJoystick = this.Joystick;
			}
		}

		public void ResetManager()
		{
			this.SendInputChangedEvent();
			this.RemoveBlockers();
		}

		public void SetBlocker(string name, bool blocking)
		{
			if (!this.inputBlockers.Contains(name) && blocking)
			{
				this.inputBlockers.Add(name);
				if (this.OnInputLocked != null)
				{
					this.OnInputLocked();
				}
				Log.Trace("Input", "The input blocker <color=green>" + name + "</color> has been enabled.", null);
			}
			if (this.inputBlockers.Contains(name) && !blocking)
			{
				this.inputBlockers.Remove(name);
				if (this.OnInputUnlocked != null)
				{
					this.OnInputUnlocked();
				}
				Log.Trace("Input", "The input blocker <color=yellow>" + name + "</color> has been disabled.", null);
			}
			this.InputBlocked = (this.inputBlockers.Count > 0);
		}

		public bool HasBlocker(string name)
		{
			return this.inputBlockers.Contains(name);
		}

		private void RemoveBlockers()
		{
			this.inputBlockers.Clear();
			Log.Trace("Input", "All blockers have been removed.", null);
		}

		private void SetMainInput(ControllerType type, int controllerId)
		{
			if (this.ActiveControllerType == type && this.ActiveController.id == controllerId)
			{
				return;
			}
			this.ActiveControllerType = type;
			this.SendInputChangedEvent();
			Log.Trace("Input", "Main input is " + this.ActiveControllerType.ToString().ToUpper() + ".", null);
		}

		private void SendInputChangedEvent()
		{
			if (this.ActiveControllerType == 2 && this.JoystickPressed != null)
			{
				this.JoystickPressed();
			}
			if (this.ActiveControllerType == null && this.KeyboardPressed != null)
			{
				this.KeyboardPressed();
			}
		}

		public void ApplyRumble(RumbleData rumble)
		{
			SingletonSerialized<RumbleSystem>.Instance.ApplyRumble(rumble);
		}

		public void StopRumble(string id)
		{
			SingletonSerialized<RumbleSystem>.Instance.StopRumble(id);
		}

		public void StopAllRumbles()
		{
			SingletonSerialized<RumbleSystem>.Instance.StopAllRumbles();
		}

		public List<string> AppliedRumbles()
		{
			return SingletonSerialized<RumbleSystem>.Instance.AppliedRumbles();
		}

		public override void OnGUI()
		{
			base.DebugResetLine();
			base.DebugDrawTextLine("******    Input", 10, 1500);
			base.DebugDrawTextLine("Blockers", 10, 1500);
			base.DebugDrawTextLine("------------------------", 10, 1500);
			foreach (string text in this.inputBlockers)
			{
				base.DebugDrawTextLine(text, 10, 1500);
			}
		}

		private readonly List<string> inputBlockers = new List<string>();

		public readonly bool UseDebugControllers;

		public ControllerType debugControllerType;

		public JoystickType debugJoystickType;

		private ControllerType _activeControllerType;
	}
}
