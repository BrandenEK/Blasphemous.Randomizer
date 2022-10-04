using System;
using System.Collections.Generic;
using I2.Loc;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Tools.DataContainer
{
	[CreateAssetMenu(fileName = "Localization Spacing Data", menuName = "Blasphemous/Localization Spacing Data", order = 0)]
	public class LocalizationSpacingData : ScriptableObject
	{
		private IList<ValueDropdownItem<string>> MyLanguages()
		{
			ValueDropdownList<string> valueDropdownList = new ValueDropdownList<string>();
			string[] array = LocalizationManager.GetAllLanguages(true).ToArray();
			Array.Sort<string>(array);
			foreach (string text in array)
			{
				valueDropdownList.Add(text, text);
			}
			return valueDropdownList;
		}

		[ValueDropdown("MyLanguages")]
		public string Language;

		public int extraSpacing;

		public int extraAfterSpacing;

		public float verticalSpacing;

		public bool addCharacterWidth;
	}
}
