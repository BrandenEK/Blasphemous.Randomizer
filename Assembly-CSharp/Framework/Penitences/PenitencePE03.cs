using System;
using Framework.Managers;

namespace Framework.Penitences
{
	public class PenitencePE03 : IPenitence
	{
		public string Id
		{
			get
			{
				return "PE03";
			}
		}

		public bool Completed { get; set; }

		public bool Abandoned { get; set; }

		public void Activate()
		{
			Core.GuiltManager.DropSingleGuilt = true;
			Core.GuiltManager.DropTearsAlongWithGuilt = true;
			Core.PenitenceManager.UseFervourFlasks = true;
		}

		public void Deactivate()
		{
			Core.GuiltManager.DropSingleGuilt = false;
			Core.GuiltManager.DropTearsAlongWithGuilt = false;
			Core.PenitenceManager.UseFervourFlasks = false;
		}

		private const string id = "PE03";
	}
}
