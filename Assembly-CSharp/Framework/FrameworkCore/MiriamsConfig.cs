using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.FrameworkCore
{
	[CreateAssetMenu(fileName = "MiriamQuest", menuName = "Blasphemous/MiriamQuest Config")]
	public class MiriamsConfig : ScriptableObject
	{
		public string PortalSceneName = "D04Z04S01";

		public string PortalPlaymakerEventName = "SHARD CUTSCENE";

		public string CutsceneFlag = "MIRIAM_INGAME_CUTSCENE";

		public string PortalFlagPrefix = "MIRIAM_CHALLENGE_";

		public string PortalFlagSufix = "_RETURN";

		[SerializeField]
		public List<string> Scenes = new List<string>();
	}
}
