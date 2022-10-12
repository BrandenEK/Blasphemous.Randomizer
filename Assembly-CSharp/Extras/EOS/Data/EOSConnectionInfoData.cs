using System;
using Epic.OnlineServices.Auth;
using UnityEngine;

namespace Extras.EOS.Data
{
	[CreateAssetMenu(fileName = "EOSConnectionInfo", menuName = "TGK/Epic/Connection Information", order = 0)]
	public class EOSConnectionInfoData : ScriptableObject
	{
		public string m_ProductName = "Minotaur";

		public string m_ProductVersion = "3.0.32";

		public string m_ProductId = "6b0291b5b26b4015a4cbb47a40ebb809";

		public string m_SandboxId = "eddb735dde6b47cda8193f2643cff886";

		public string m_DeploymentId = "431c31e5db794dee92ed16227d6623f7";

		public string m_ClientId = "xyza7891eIjsX1QvGA50lwMwOLZdlMjI";

		public string m_ClientSecret = "bwhmUkbD0sgyiRXqN2m8mImttkjSOqODSlj5BosHL+k";

		public LoginCredentialType m_LoginCredentialType = LoginCredentialType.Developer;

		public string m_LoginCredentialId = "localhost:12345";

		public string m_LoginCredentialToken = "tgk_sgarcia";
	}
}
