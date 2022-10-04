using System;

namespace Framework.Penitences
{
	public interface IPenitence
	{
		string Id { get; }

		bool Completed { get; set; }

		bool Abandoned { get; set; }

		void Activate();

		void Deactivate();
	}
}
