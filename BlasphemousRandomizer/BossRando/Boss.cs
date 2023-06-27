
namespace BlasphemousRandomizer.BossRando
{
    public class Boss
    {
        public readonly string id;

        public readonly string defeatFlag;

        public readonly float bossStrength;

        public string BossRoomSceneId => "D22Z01S" + id.Substring(2);

        public string BossRoomTeleportId => "BOSSRUSH" + id.Substring(2);
    }
}
