using Newtonsoft.Json;
using System;

namespace BlasphemousRandomizer.BossRando
{
    [Serializable]
    public class Boss
    {
        [JsonProperty] public readonly string id;

        [JsonProperty] public readonly string defeatFlag;

        [JsonProperty] public readonly float bossStrength;

        public string BossRoomSceneId => "D22Z01S" + id.Substring(2);

        public string BossRoomTeleportId => "BOSSRUSH" + id.Substring(2);
    }
}
