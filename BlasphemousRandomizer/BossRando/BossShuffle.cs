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
    public class BossShuffle : IShuffle
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

        private readonly Dictionary<string, GameObject> loadedObjects = new();

        private IEnumerator LoadSceneForObject(string objectName, string sceneName, string objectPath)
        {
            //InLoadProcess = true;

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            // Load the object from this scene
            Scene tempScene = SceneManager.GetSceneByName(sceneName);
            GameObject sceneObject = FindObjectInScene(tempScene, objectPath, true);
            if (sceneObject == null)
            {
                Main.Randomizer.LogError($"Failed to load {objectPath} from {sceneName}");
            }
            else
            {
                GameObject obj = Object.Instantiate(sceneObject, Main.Instance.transform);
                obj.name = objectName;
                obj.transform.position = Vector3.zero;
                obj.SetActive(false);
                loadedObjects.Add(objectName, obj);
                Main.Randomizer.Log($"Loaded {objectPath} from {sceneName}");
            }

            yield return null;

            asyncLoad = SceneManager.UnloadSceneAsync(tempScene);
            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            //InLoadProcess = false;
        }

        private GameObject FindObjectInScene(Scene scene, string objectPath, bool disableRoots)
        {
            return FindObjectInScene(FindRootObjectsInScene(scene, disableRoots), objectPath);
        }

        private GameObject FindObjectInScene(Dictionary<string, Transform> rootObjects, string objectPath)
        {
            string[] transformPath = objectPath.Split('/');

            Transform currTransform = null;
            for (int i = 0; i < transformPath.Length; i++)
            {
                string finder = transformPath[i];
                if (i == 0)
                {
                    currTransform = rootObjects.ContainsKey(finder) ? rootObjects[finder] : null;
                }
                else if (finder.Length >= 3 && finder[0] == '{' && finder[finder.Length - 1] == '}')
                {
                    int childIdx = int.Parse(finder.Substring(1, finder.Length - 2));
                    currTransform = currTransform.childCount > childIdx ? currTransform.GetChild(childIdx) : null;
                }
                else
                {
                    currTransform = currTransform.Find(finder);
                }

                if (currTransform == null) break;
            }

            return currTransform?.gameObject;
        }

        private Dictionary<string, Transform> FindRootObjectsInScene(Scene scene, bool disableRoots)
        {
            Dictionary<string, Transform> rootObjects = new();
            foreach (GameObject obj in scene.GetRootGameObjects())
            {
                if (obj.name[0] != '=' && !rootObjects.ContainsKey(obj.name))
                {
                    rootObjects.Add(obj.name, obj.transform);
                    if (disableRoots)
                        obj.SetActive(false);
                }
            }
            return rootObjects;
        }

        private IEnumerator LoadObjects()
        {
            yield return Main.Instance.StartCoroutine(LoadSceneForObject("piedad", "D01Z04S18_LOGIC", "CHARACTERS/ENEMIES"));
            yield return Main.Instance.StartCoroutine(LoadSceneForObject("charred", "D02Z03S20_LOGIC", "CHARACTERS/BOSS"));
            yield return Main.Instance.StartCoroutine(LoadSceneForObject("isidora", "D01BZ08S01_LOGIC", "CHARACTERS/Isidora"));
            yield return Main.Instance.StartCoroutine(LoadSceneForObject("isidora-trigger", "D01BZ08S01_LOGIC", "LOGIC/SCRIPTS/BossTrigger"));
            yield return Main.Instance.StartCoroutine(LoadSceneForObject("isidora-song-trigger", "D01BZ08S01_LOGIC", "LOGIC/SCRIPTS/Isidora Song Trigger"));
            yield return Main.Instance.StartCoroutine(LoadSceneForObject("isidora-boss-manager", "D01BZ08S01_LOGIC", "CHARACTERS/BossManager"));
            yield return Main.Instance.StartCoroutine(LoadSceneForObject("isidora-bonfire", "D01BZ08S01_LOGIC", "TRAPS/Bonfire"));
            yield return Main.Instance.StartCoroutine(LoadSceneForObject("isidora-bounds", "D01BZ08S01_LOGIC", "LOGIC/SCRIPTS/BossBattleBounds"));
        }

        private IEnumerator TransferIsidoraObjects()
        {
            InLoadProcess = true;

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("D01BZ08S01_LOGIC", LoadSceneMode.Additive);
            //asyncLoad.allowSceneActivation = false;
            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            string[] objectsToMove = new string[]
            {
                "TRAPS/Bonfire",
                "LOGIC/SCRIPTS",
                "CHARACTERS/BossManager",
                "CHARACTERS/Isidora",
            };
            Vector3 positionDifference = new Vector3(316, -98);

            // Move the objects from this scene
            Scene realScene = SceneManager.GetActiveScene();
            Scene tempScene = SceneManager.GetSceneByName("D01BZ08S01_LOGIC");
            foreach (string objectPath in objectsToMove)
            {
                GameObject sceneObject = FindObjectInScene(tempScene, objectPath, false);
                if (sceneObject == null)
                {
                    Main.Randomizer.LogError($"Failed to load {objectPath} from {"D01BZ08S01_LOGIC"}");
                }
                else
                {
                    //GameObject obj = Object.Instantiate(sceneObject, Main.Instance.transform);
                    //obj.name = objectName;
                    sceneObject.transform.parent = null;
                    //Main.Randomizer.LogWarning(sceneObject.transform.DisplayHierarchy(3, true));
                    SceneManager.MoveGameObjectToScene(sceneObject, realScene);
                    sceneObject.transform.position += positionDifference;
                    //sceneObject.SetActive(true);
                    Main.Randomizer.Log($"Loaded {objectPath} from {"D01BZ08S01_LOGIC"}");
                    //if (sceneObject.GetComponentInChildren<IsidoraBehaviour>() != null)
                    //    sceneObject.GetComponentInChildren<IsidoraBehaviour>().battleBounds = new Rect(-100, -100, 15, 10);
                }
            }

            yield return null;

            asyncLoad = SceneManager.UnloadSceneAsync(tempScene);
            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            // Extra changes
            FindObjectInScene(realScene, "Isidora", false).GetComponentInChildren<IsidoraBehaviour>().battleBounds = new Rect(-100, -100, 15, 10);
            FindObjectInScene(realScene, "SCRIPTS/BossTrigger", false).GetComponent<PlayMakerFSM>().SetState("Wait for trigger enter");
            //foreach (var state in fsm.FsmStates)
            //{
            //    Main.Randomizer.LogWarning(state.Name + " " + state.Description + ": " + state.Active);
            //}

            InLoadProcess = false;
        }

        public static bool InLoadProcess { get; private set; }

        private void HidePietyObjects()
        {
            string[] objectsToHide = new string[]
            {
                "LOGIC/SCRIPTS",
                "LOGIC/BossFight",
                "CHARACTERS/ENEMIES",
            };

            Scene realScene = SceneManager.GetActiveScene();
            foreach (string objectPath in objectsToHide)
            {
                FindObjectInScene(realScene, objectPath, false).SetActive(false);
            }
        }

        public void levelLoaded(string scene)
        {
            if (scene == "MainMenu")
            {
                //Main.Instance.StartCoroutine(LoadObjects());
            }
            else if (scene == "D02Z03S20")
            {
                FindObjectInScene(SceneManager.GetActiveScene(), "CHARACTERS/BOSS", false).SetActive(false);
                GameObject obj = Object.Instantiate(loadedObjects["piedad"]);
                obj.transform.position = new Vector3(-456, 372);
                obj.SetActive(true);
            }
            else if (scene == "D01Z04S18")
            {
                HidePietyObjects();
                Main.Instance.StartCoroutine(TransferIsidoraObjects());
            }


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
