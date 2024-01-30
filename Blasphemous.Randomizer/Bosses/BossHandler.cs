using Framework.Managers;
using UnityEngine;

namespace Blasphemous.Randomizer.Bosses;

public class BossHandler
{
    public bool BossBoundaryStatus
    {
        set
        {
            string currentScene = Core.LevelManager.currentLevel.LevelName;
            try
            {
                if (currentScene == "D17Z01S11")
                    GameObject.Find("CAMERAS").transform.Find("CombatBoundaries").gameObject.SetActive(value);
                else if (currentScene == "D01Z04S18")
                    GameObject.Find("CAMERAS").transform.Find("CombatBoundaries").gameObject.SetActive(value);
                else if (currentScene == "D02Z03S20")
                {
                    GameObject.Find("LOGIC").transform.Find("SCRIPTS/CombatElements").gameObject.SetActive(value);
                    GameObject.Find("LOGIC").transform.Find("SCRIPTS/CombatElementsAfterCombat").gameObject.SetActive(value);
                    GameObject.Find("LOGIC").transform.Find("SCRIPTS/VisualFakeWallLeft").gameObject.SetActive(value);
                    Transform leftWall = GameObject.Find("LOGIC").transform.Find("SCRIPTS/VisualLeftDoorFrame");
                    leftWall.GetChild(0).GetComponent<SpriteRenderer>().color = Color.white;
                    leftWall.GetChild(1).GetComponent<SpriteRenderer>().color = Color.white;
                }
                else if (currentScene == "D05Z02S14")
                    GameObject.Find("LOGIC").transform.Find("SCRIPTS/CombatBoundaries").gameObject.SetActive(value);

                Main.Randomizer.LogWarning($"Setting boss boundary status to {value}");
            }
            catch (System.NullReferenceException)
            {
                Main.Randomizer.LogError("Boss boundary path could not be found!");
            }
        }
    }
}
