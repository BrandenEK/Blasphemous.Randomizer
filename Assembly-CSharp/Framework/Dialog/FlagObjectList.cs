using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace Framework.Dialog
{
	[Serializable]
	public class FlagObjectList : SerializedScriptableObject
	{
		public List<FlagObject> flagList = new List<FlagObject>();
	}
}
