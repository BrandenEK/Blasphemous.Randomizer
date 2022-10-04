using System;
using System.Collections.Generic;

namespace Framework.Dialog
{
	public class MsgObject : BaseObject
	{
		public override string GetPrefix()
		{
			return "MSG_";
		}

		public List<string> msgLines = new List<string>();
	}
}
