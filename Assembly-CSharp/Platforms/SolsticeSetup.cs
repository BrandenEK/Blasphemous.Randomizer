using System;
using UnityEngine;

namespace Platforms
{
	public static class SolsticeSetup
	{
		public static bool IsSolsticeRelease()
		{
			string environmentVariable = Environment.GetEnvironmentVariable(SolsticeSetup.SOLSTICE_MODE_ENVVAR);
			Debug.Log("Solstice Mode: " + environmentVariable);
			return environmentVariable == "RELEASE";
		}

		private static readonly string SOLSTICE_MODE_ENVVAR = "SOLSTICE_LAUNCH_MODE";
	}
}
