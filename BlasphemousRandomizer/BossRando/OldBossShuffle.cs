using System.Collections;
using System.Collections.Generic;
using Framework.Managers;
using Framework.FrameworkCore;
using Gameplay.UI;
using Gameplay.UI.Widgets;
using Tools.Level.Interactables;
using UnityEngine;
using UnityEngine.SceneManagement;
using Gameplay.GameControllers.Bosses.BossFight;
using Gameplay.GameControllers.Entities;
using Tools.Playmaker2;
using Gameplay.GameControllers.Bosses.Isidora;

namespace BlasphemousRandomizer.BossRando
{
    public class OldBossShuffle : IShuffle
    {
        // Gameplay status
        public enum BossFightStatus { Nothing, Entering, Returning };
        public BossFightStatus bossStatus;
        public bool usedStart, usedEnd;

        // Objects
        private GameObject bossActivator;
        private RuntimeAnimatorController kneelingAnimator;
        private Material kneelingMaterial;
        private GameObject currentActivator;

        // Reload spawn data
        public string spawnScene;
        public Vector3 spawnPosition;
        public EntityOrientation spawnDirection;

        public void setBossActivator(GameObject obj) { if (bossActivator == null) bossActivator = obj; }
        public void setKneelingAnimator(RuntimeAnimatorController anim, Material mat) { if (kneelingAnimator == null) { kneelingAnimator = anim; kneelingMaterial = mat; } }

        public void loadBossRoom()
        {
            UIController.instance.StartCoroutine(loadBossRoomCorroutine());

            IEnumerator loadBossRoomCorroutine()
            {
                yield return fadeToWhite();
                bossStatus = BossFightStatus.Entering;
                Core.SpawnManager.Teleport("TELEPORT_D22Z01S02");
            }
        }

        public void loadPreviousRoom()
        {
            UIController.instance.StartCoroutine(loadPreviousRoomCorroutine());

            IEnumerator loadPreviousRoomCorroutine()
            {
                yield return fadeToWhite();
                bossStatus = BossFightStatus.Returning;

                // Load based on room
                spawnScene = "D17Z01S11";
                spawnPosition = new Vector3(-810, 10, 0);
                spawnDirection = EntityOrientation.Right;

                Core.SpawnManager.SpawnFromCustom(true, Color.white);
            }
        }

        private IEnumerator fadeToWhite()
        {
            Main.Randomizer.playSoundEffect(3);
            yield return new WaitForSecondsRealtime(0.2f);
            yield return FadeWidget.instance.FadeCoroutine(new Color(0, 0, 0, 0), Color.white, 2, true, null);
        }

        public void levelLoaded(string scene)
        {
            // Check if boss shuffle is enabled
            return;

            usedStart = false;
            usedEnd = false;

            if (scene == "D17Z01S11")// && !Core.Events.GetFlag("D17Z01_BOSSDEAD"))
            {
                currentActivator = spawnBossActivator(new Vector3(-810, 10, 0));
                Main.Randomizer.Log("load: " + bossStatus.ToString());
                // When returning, make activator disappear & player stand up
                if (bossStatus == BossFightStatus.Returning && currentActivator != null)
                {
                    currentActivator.GetComponent<GuiltDropCollectibleItem>().Use();
                    stopPlayerKneeling();
                }
            }
        }

        private GameObject spawnBossActivator(Vector3 position)
        {
            if (bossActivator == null) return null;

            // Create object
            GameObject activator = Object.Instantiate(bossActivator, position, Quaternion.identity);
            activator.transform.GetChild(2).GetComponent<SpriteRenderer>().color = new Color(1, 0.21f, 0.17f);
            //ColorPaletteSwapper color = activator.transform.GetChild(1).gameObject.AddComponent<ColorPaletteSwapper>();
            //color.extraMaterial = kneelingMaterial;
            SpriteRenderer render = activator.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>();
            Sprite palette = Core.ColorPaletteManager.GetCurrentColorPaletteSprite();
            render.material.SetTexture("_PaletteTex", palette.texture);

            activator.GetComponent<InteractableGuiltDrop>().timeToWait = 99f;
            return activator;
        }

        public void startPlayerKneeling()
        {
            Core.Input.SetBlocker("rando_boss", true);
            if (currentActivator == null) return;

            Core.Logic.Penitent.SpriteRenderer.enabled = false;
            Animator playerAnim = currentActivator.transform.GetChild(1).GetComponent<Animator>();
            playerAnim.gameObject.GetComponent<SpriteRenderer>().flipX = Core.Logic.Penitent.Status.Orientation == EntityOrientation.Left;
            playerAnim.transform.position = Core.Logic.Penitent.transform.position;
            playerAnim.runtimeAnimatorController = kneelingAnimator;
            playerAnim.SetTrigger("KNEE_START");
        }

        private void stopPlayerKneeling()
        {
            if (currentActivator == null) return;
            UIController.instance.StartCoroutine(StopKneelCorroutine());

            IEnumerator StopKneelCorroutine()
            {
                startPlayerKneeling();
                Animator playerAnim = currentActivator.transform.GetChild(1).GetComponent<Animator>();
                playerAnim.ResetTrigger("KNEE_START");
                playerAnim.Play("Knee (Aura OFF)");
                yield return new WaitForSecondsRealtime(1);
                playerAnim.SetTrigger("KNEE_END");
                yield return new WaitForSecondsRealtime(0.5f);
                playerAnim.gameObject.SetActive(false);
                Core.Logic.Penitent.SpriteRenderer.enabled = true;
                Core.Input.SetBlocker("rando_boss", false);
            }
        }

        public void Init()
        {
            // Initialize filler
        }

        public void Reset()
        {
            // Set dictionary of bosses to null
        }

        public void Shuffle(int seed)
        {
            // Shuffle bosses
        }
    }
}
