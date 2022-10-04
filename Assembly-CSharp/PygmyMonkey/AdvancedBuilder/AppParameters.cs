using System;
using UnityEngine;

namespace PygmyMonkey.AdvancedBuilder
{
	[Serializable]
	public class AppParameters : ScriptableObject
	{
		public static AppParameters Get
		{
			get
			{
				if (AppParameters.mInstance == null)
				{
					AppParameters.mInstance = (AppParameters)Resources.Load("AppParameters", typeof(AppParameters));
					if (AppParameters.mInstance == null)
					{
						throw new Exception("We could not find the ScriptableObject AppParameters.asset inside the folder 'PygmyMonkey/AdvancedBuilder/Resources/'");
					}
				}
				return AppParameters.mInstance;
			}
		}

		public void updateParameters(string releaseType, string platformType, string distributionPlatform, string platformArchitecture, string textureCompression, string productName, string bundleIdentifier, string bundleVersion, int buildNumber)
		{
			this.m_releaseType = releaseType;
			this.m_platformType = platformType;
			this.m_distributionPlatform = distributionPlatform;
			this.m_platformArchitecture = platformArchitecture;
			this.m_textureCompression = textureCompression;
			this.m_productName = productName;
			this.m_bundleIdentifier = bundleIdentifier;
			this.m_bundleVersion = bundleVersion;
			this.m_buildNumber = buildNumber;
		}

		public string releaseType
		{
			get
			{
				return this.m_releaseType;
			}
		}

		public string platformType
		{
			get
			{
				return this.m_platformType;
			}
		}

		public string distributionPlatform
		{
			get
			{
				return this.m_distributionPlatform;
			}
		}

		public string platformArchitecture
		{
			get
			{
				return this.m_platformArchitecture;
			}
		}

		public string textureCompression
		{
			get
			{
				return this.m_textureCompression;
			}
		}

		public string productName
		{
			get
			{
				return this.m_productName;
			}
		}

		public string bundleIdentifier
		{
			get
			{
				return this.m_bundleIdentifier;
			}
		}

		public string bundleVersion
		{
			get
			{
				return this.m_bundleVersion;
			}
		}

		public int buildNumber
		{
			get
			{
				return this.m_buildNumber;
			}
		}

		private static AppParameters mInstance;

		[SerializeField]
		private string m_releaseType = string.Empty;

		[SerializeField]
		private string m_platformType = string.Empty;

		[SerializeField]
		private string m_distributionPlatform = string.Empty;

		[SerializeField]
		private string m_platformArchitecture = string.Empty;

		[SerializeField]
		private string m_textureCompression = string.Empty;

		[SerializeField]
		private string m_productName = string.Empty;

		[SerializeField]
		private string m_bundleIdentifier = string.Empty;

		[SerializeField]
		private string m_bundleVersion = string.Empty;

		[SerializeField]
		private int m_buildNumber;
	}
}
