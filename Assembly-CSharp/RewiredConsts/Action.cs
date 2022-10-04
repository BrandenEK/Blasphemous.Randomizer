using System;
using Rewired.Dev;

namespace RewiredConsts
{
	public static class Action
	{
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Move Horizontal")]
		public const int Move_Horizontal = 0;

		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Move Vertical")]
		public const int Move_Vertical = 4;

		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Attack")]
		public const int Attack = 5;

		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Jump")]
		public const int Jump = 6;

		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Dash")]
		public const int Dash = 7;

		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Interact")]
		public const int Interact = 8;

		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Pause")]
		public const int Pause = 10;

		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Move Vertical Right Stick")]
		public const int Move_RVertical = 20;

		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Move Horizontal Right Stick")]
		public const int Move_RHorizontal = 21;

		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Open the inventory")]
		public const int Inventory = 22;

		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Use a Flask")]
		public const int Flask = 23;

		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Use prayer")]
		public const int Prayer = 25;

		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Perform a Parry movement")]
		public const int Parry = 38;

		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Tab to left in inventory")]
		public const int Inventory_Left = 28;

		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Inventory Scroll Down")]
		public const int Inventory_Scroll_Down = 45;

		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Inventory Scroll Up")]
		public const int Inventory_Scroll_Up = 43;

		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Tab to right in inventory")]
		public const int Inventory_Right = 29;

		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Launch Range Attack")]
		public const int Range_Attack = 57;

		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Dialog Skip")]
		public const int Dialog_Skip = 39;

		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Show lore dialog")]
		public const int Inventory_Lore = 64;

		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Cancel a grab")]
		public const int Grab_Cancel = 65;

		[ActionIdFieldInfo(categoryName = "Menu", friendlyName = "UI Horizontal")]
		public const int UI_Horizontal = 48;

		[ActionIdFieldInfo(categoryName = "Menu", friendlyName = "UI Vertical")]
		public const int UI_Vertical = 49;

		[ActionIdFieldInfo(categoryName = "Menu", friendlyName = "UI Submit")]
		public const int UI_Submit = 50;

		[ActionIdFieldInfo(categoryName = "Menu", friendlyName = "UI Back")]
		public const int UI_Back = 51;

		[ActionIdFieldInfo(categoryName = "Menu", friendlyName = "UI Contextual")]
		public const int UI_Contextual = 52;

		[ActionIdFieldInfo(categoryName = "Menu", friendlyName = "UI Center")]
		public const int UI_Center = 60;

		[ActionIdFieldInfo(categoryName = "Menu", friendlyName = "UI Options")]
		public const int UI_Options = 61;
	}
}
