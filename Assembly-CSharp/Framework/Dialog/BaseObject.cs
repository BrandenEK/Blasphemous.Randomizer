using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Framework.Dialog
{
	public class BaseObject : ScriptableObject
	{
		public void OnIdChanged(string value)
		{
			this.id = value.Replace(' ', '_').ToUpper();
			string prefix = this.GetPrefix();
			if (prefix.Length > 0 && !this.id.StartsWith(prefix))
			{
				this.id = prefix + this.id;
			}
		}

		public virtual string GetPrefix()
		{
			return string.Empty;
		}

		public string GetBaseTranslationID()
		{
			if (this.translationCategory.Length > 0)
			{
				return this.translationCategory + "/" + this.id;
			}
			return this.id;
		}

		[OnValueChanged("OnIdChanged", false)]
		public string id = string.Empty;

		public string sortDescription = string.Empty;

		[TextArea(3, 10)]
		public string description = string.Empty;

		[HideInInspector]
		public string translationCategory = string.Empty;
	}
}
