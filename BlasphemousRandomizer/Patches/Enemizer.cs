using HarmonyLib;
using Tools.Level.Layout;
using Tools.Level.Utils;
using Tools.Level.Actionables;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Framework.EditorScripts.EnemiesBalance;
using Framework.FrameworkCore.Attributes;
using Gameplay.GameControllers.Penitent;
using Gameplay.GameControllers.Enemies.WallEnemy;
using Gameplay.GameControllers.Enemies.WallEnemy.AI;
using Gameplay.GameControllers.Enemies.PatrollingFlyingEnemy;
using Gameplay.GameControllers.Enemies.JarThrower;
using Gameplay.GameControllers.Enemies.MeltedLady;
using Gameplay.GameControllers.Enemies.MeltedLady.Attack;
using Gameplay.GameControllers.Enemies.MeltedLady.IA;
using Gameplay.GameControllers.Enemies.RangedBoomerang.IA;
using Gameplay.GameControllers.Enemies.GhostKnight;
using Gameplay.GameControllers.Enemies.GhostKnight.AI;
using Gameplay.GameControllers.Effects.Entity;
using UnityEngine;
using System.Collections.Generic;
using Framework.Audio;

namespace BlasphemousRandomizer.Patches
{
    [HarmonyPatch(typeof(AudioLoader), "Awake")]
    public class AudioLoader_Patch
    {
        public static void Prefix(AudioLoader __instance)
        {
            if (Main.Randomizer.enemyShuffler.audioCatalogs != null)
            {
                __instance.AudioCatalogs = Main.Randomizer.enemyShuffler.audioCatalogs;
                Main.Randomizer.enemyShuffler.audioCatalogs = null;
            }
        }
    }

    // Replace enemy with random one
    [HarmonyPatch(typeof(EnemySpawnPoint), "Awake")]
    public class EnemySpawnPoint_Patch
    {
        public static void Postfix(EnemySpawnPoint __instance, ref GameObject ___selectedEnemy, ref Transform ___spawnPoint)
        {
            //if (Main.Randomizer.gameConfig.enemies.type < 1)
            //    return;

            // Calculate location id
            string scene = Core.LevelManager.currentLevel.LevelName;
            string locationId = $"{scene}[{Mathf.RoundToInt(___spawnPoint.position.x)},{Mathf.RoundToInt(___spawnPoint.position.y)}]";

            // If this is a special arena, add to locationId to prevent duplicates
            if (__instance.SpawnOnArena && (scene.Contains("D19") || scene == "D03Z03S03"))
                locationId += $"({__instance.name})";

            // Get facing direction of this spawn point
            EnemySpawnConfigurator config = __instance.GetComponent<EnemySpawnConfigurator>();
            bool facingLeft = config == null ? true : config.facingLeft;

            // Retrieve new enemy
            GameObject newEnemy = Main.Randomizer.enemyShuffler.getEnemy(locationId, facingLeft);
            if (newEnemy != null)
                ___selectedEnemy = newEnemy;

            // Modify spawnpoint position
            RaycastHit2D hit = Physics2D.Raycast(___spawnPoint.position, Vector2.down, 30, (1 << 19 | 1 << 13));
            float testDiff = ___spawnPoint.position.y - hit.point.y;
            float locationOffset = Main.Randomizer.enemyShuffler.getLocationOffset(locationId);
            float enemyOffset = Main.Randomizer.enemyShuffler.getEnemyOffset(___selectedEnemy.GetComponent<Enemy>().Id);
            ___spawnPoint.position = new Vector3(___spawnPoint.position.x, ___spawnPoint.position.y - locationOffset, ___spawnPoint.position.z);

            // Extra data collection stuff
            //string output = "{\r\n\t\"locationId\": \"";
            //output += locationId;
            //output += "\",\r\n\t\"originalEnemy\": \"";
            //output += ___selectedEnemy.GetComponentInChildren<Enemy>().Id;
            //output += "\"\r\n},\r\n";
            string output = locationId + ": " + testDiff + "\n";
            Shufflers.EnemyShuffle.enemyData += output;

            // Can modify spawn position here too
        }
    }

    // Scale enemy stats based on area
    [HarmonyPatch(typeof(EnemyStatsImporter), "SetEnemyStats")]
    public class EnemyStatsImporter_Patch
    {
        public static void Postfix(Enemy enemy)
        {
            if (enemy == null || enemy.Id == "" || enemy.Id == "EV09")
                return;

            // Get ratings of enemy & area
            string scene = Core.LevelManager.currentLevel.LevelName;
            if (scene.Substring(0, 6) != "D19Z01")
            {
                scene = scene.Substring(0, 6);
            }
            int areaRating = Main.Randomizer.enemyShuffler.getRating(scene);
            int enemyRating = Main.Randomizer.enemyShuffler.getRating(enemy.Id);
            float percent = 0.07f;

            // If areaScaling is enabled, calculate percent scaling
            if (areaRating != 0 && enemyRating != 0 && Main.Randomizer.gameConfig.enemies.type > 0 && Main.Randomizer.gameConfig.enemies.areaScaling)
            {
                percent *= areaRating - enemyRating;
            }

            // Change stats
            enemy.Stats.Strength = new Strength(enemy.Stats.Strength.Base + enemy.Stats.Strength.Base * percent, enemy.Stats.StrengthUpgrade, 1f);
            enemy.Stats.Life = new Life(enemy.Stats.Life.Base + enemy.Stats.Life.Base * percent, enemy.Stats.LifeUpgrade, enemy.Stats.LifeMaximun, 1f);
            enemy.purgePointsWhenDead += enemy.purgePointsWhenDead * percent;
            EnemyAttack attack = enemy.GetComponentInChildren<EnemyAttack>();
            if (attack != null)
                attack.ContactDamageAmount += attack.ContactDamageAmount * percent;
        }

        // Prevent Wall Enemy climbable & attack error
        [HarmonyPatch(typeof(WallEnemy), "OnTriggerEnter2D")]
        public class WallEnemyTrigger_Patch
        {
            public static bool Prefix()
            {
                return false;
            }
        }
        [HarmonyPatch(typeof(WallEnemyBehaviour), "OnDestroy")]
        public class WallEnemyDestroy_Patch
        {
            public static bool Prefix(WallEnemyBehaviour __instance)
            {
                return __instance.CollisionSensor != null;
            }
        }
        [HarmonyPatch(typeof(Entity), "SetOrientation")]
        public class Entity_Patch
        {
            public static bool Prefix(Entity __instance)
            {
                return __instance.Id != "EN11" && __instance.Id != "EV15";
            }
        }

        // Prevent Flying Patrolling Enemy casting error
        [HarmonyPatch(typeof(FlyingPatrollingEnemySpawnConfigurator), "OnSpawn")]
        public class FlyingPatrollingEnemySpawnConfigurator_Patch
        {
            public static bool Prefix(Enemy e)
            {
                return e.GetType() == typeof(PatrollingFlyingEnemy);
            }
        }

        // Prevent New Flagellant material error
        [HarmonyPatch(typeof(MasterShaderEffects), "DamageEffectBlink")]
        public class MasterShaderEffects_Patch
        {
            public static bool Prefix(MasterShaderEffects __instance, Material effectMaterial)
            {
                return effectMaterial != null || __instance.damageEffectTestMaterial != null;
            }
        }

        // Prevent Jar Thrower loading error
        [HarmonyPatch(typeof(JarThrower), "OnPlayerSpawn")]
        public class JarThrower_Patch
        {
            public static bool Prefix(JarThrower __instance)
            {
                if (__instance.Behaviour == null)
                {
                    Object.Destroy(__instance);
                    return false;
                }
                return true;
            }
        }

        // Prevent Bell / Chime Ringer trigger error
        [HarmonyPatch(typeof(GlobalTrapTriggerer), "Awake")]
        public class GlobalTrapTriggerer_Patch
        {
            public static void Postfix(GlobalTrapTriggerer __instance)
            {
                if (__instance.trapManager == null)
                {
                    __instance.trapManager = new TriggerTrapManager();
                    __instance.trapManager.traps = new List<TriggerBasedTrap>();
                }
            }
        }

        // Prevent Melted Lady teleporting error
        [HarmonyPatch(typeof(MeltedLadyBehaviour), "GetNearestTeleportPointToTarget")]
        public class MeltedLadyBehaviourTeleport_Patch
        {
            public static bool Prefix(MeltedLadyBehaviour __instance, ref MeltedLadyTeleportPoint __result)
            {
                foreach (MeltedLadyTeleportPoint point in Object.FindObjectsOfType<MeltedLadyTeleportPoint>())
                {
                    if (point.name == __instance.OriginPosition.ToString())
                    {
                        __result = point;
                        return false;
                    }
                }
                __result = null;
                return false;
            }
        }
        [HarmonyPatch(typeof(MeltedLadyBehaviour), "OnStart")]
        public class MeltedLadyBehaviourStart_Patch
        {
            public static void Postfix(MeltedLadyBehaviour __instance)
            {
                __instance.gameObject.SetActive(true);
                GameObject obj = new GameObject(__instance.OriginPosition.ToString());
                obj.transform.position = new Vector3(__instance.transform.position.x, __instance.transform.position.y, __instance.transform.position.z);
                obj.AddComponent<MeltedLadyTeleportPoint>();
            }
        }

        // Prevent Librarian walking error
        [HarmonyPatch(typeof(RangedBoomerangBehaviour), "ReadSpawnerConfig")]
        public class RangedBoomerangBehaviour_Patch
        {
            public static bool Prefix(RangedBoomerangBehaviour __instance)
            {
                if (__instance.RangedBoomerang.Id == "EV22")
                    __instance.doPatrol = false;
                return false;
            }
        }

        // Prevent Ghost Knight disappear error
        [HarmonyPatch(typeof(GhostKnightBehaviour), "Damage")]
        public class GhostKnightBehaviour_Patch
        {
            public static void Postfix(GhostKnightBehaviour __instance)
            {
                __instance.GetComponent<GhostKnight>().EntityDamageArea.DamageAreaCollider.enabled = true;
            }
        }

        // Testing
        [HarmonyPatch(typeof(EnemySpawnPoint), "CreateEnemy")]
        public class SpawnPointTest
        {
            public static void Postfix(EnemySpawnPoint __instance, Transform ___spawnPoint)
            {
                create(Main.Randomizer.getImage(1), __instance.transform, ___spawnPoint.position);
                if (__instance.SpawnedEnemy != null)
                    create(Main.Randomizer.getImage(0), __instance.SpawnedEnemy.transform, __instance.SpawnedEnemy.transform.position);
            }

            private static void create(Sprite sprite, Transform parent, Vector3 position)
            {
                GameObject obj = new GameObject();
                SpriteRenderer render = obj.AddComponent<SpriteRenderer>();
                render.sprite = sprite;
                render.sortingGroupOrder = -5;
                render.sortingOrder = -5;
                obj.transform.position = position;
                obj.transform.parent = parent;
            }
        }
    }
}
