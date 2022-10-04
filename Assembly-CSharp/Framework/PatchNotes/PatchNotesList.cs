using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace Framework.PatchNotes
{
	[Serializable]
	public class PatchNotesList : SerializedScriptableObject
	{
		public List<PatchNotes> patchNotesList = new List<PatchNotes>();
	}
}
