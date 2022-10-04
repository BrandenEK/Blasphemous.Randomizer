using System;

namespace Framework.PatchNotes
{
	[Serializable]
	public class PatchNotes
	{
		public PatchNotes(string version)
		{
			this.version = version;
		}

		public string version;
	}
}
