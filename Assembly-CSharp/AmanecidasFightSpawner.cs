using System;
using System.Collections.Generic;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Bosses.Amanecidas;
using Gameplay.GameControllers.Bosses.BossFight;
using Gameplay.GameControllers.Camera;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Penitent;
using I2.Loc;
using Sirenix.OdinInspector;
using UnityEngine;

public class AmanecidasFightSpawner : MonoBehaviour
{
	private void Awake()
	{
		this.bossMgr = base.GetComponent<BossFightManager>();
		AmanecidasFightSpawner.Instance = this;
		LevelManager.OnLevelLoaded += this.OnLevelLoaded;
	}

	private void OnDestroy()
	{
		AmanecidasFightSpawner.Instance = null;
	}

	private void OnLevelLoaded(Level oldLevel, Level newLevel)
	{
		LevelManager.OnLevelLoaded -= this.OnLevelLoaded;
		this.SpawnBoss();
		this.bossMgr.Boss = this.currentBoss.GetComponent<Enemy>();
		this.bossMgr.Init();
	}

	private void Update()
	{
	}

	public Vector3 GetPenitentSpawnPoint()
	{
		AmanecidasFightSpawner.AMANECIDAS_FIGHTS fightType = this.GetCurrentFightFromFlags();
		return this.fightData.Find((AmanecidasFightSpawner.AmanecidasFightData x) => x.fightType == fightType).spawnPoint.position;
	}

	public void SetPenitentOnSpawnPoint()
	{
		Penitent penitent = Core.Logic.Penitent;
		if (penitent != null)
		{
			Vector2 v = this.GetPenitentSpawnPoint();
			penitent.transform.position = v;
		}
	}

	public void SpawnBoss()
	{
		this.currentBoss = UnityEngine.Object.Instantiate<GameObject>(this.bossPrefab);
		this.amanecida = this.currentBoss.GetComponent<Amanecidas>();
		this.amanecida.transform.position = base.transform.position + Vector3.up * 25f;
		AmanecidasFightSpawner.AMANECIDAS_FIGHTS fightType = this.GetCurrentFightFromFlags();
		Debug.Log("Current fight " + fightType);
		this.amanecida.SetupFight(fightType);
		GameObject gameObject = new GameObject("AMANECIDA_FIGHT_STUFF");
		gameObject.transform.SetParent(base.transform, false);
		AmanecidasFightSpawner.AmanecidasFightData amanecidasFightData = this.fightData.Find((AmanecidasFightSpawner.AmanecidasFightData x) => x.fightType == fightType);
		this.amanecida.displayName = amanecidasFightData.displayName;
		if (amanecidasFightData.prefabsToInstantiate != null)
		{
			foreach (GameObject gameObject2 in amanecidasFightData.prefabsToInstantiate)
			{
				if (gameObject2 != null)
				{
					GameObject gameObject3 = UnityEngine.Object.Instantiate<GameObject>(gameObject2);
					gameObject3.transform.SetParent(gameObject.transform, false);
					gameObject3.transform.localPosition = Vector3.zero;
					CameraNumericBoundaries componentInChildren = gameObject3.GetComponentInChildren<CameraNumericBoundaries>();
					if (componentInChildren != null)
					{
						componentInChildren.CenterKeepSize();
						componentInChildren.SetBoundaries();
					}
					this.arena = gameObject3.GetComponent<AmanecidaArena>();
					if (this.arena != null)
					{
						this.arena.ActivateArena(this.amanecida, base.transform.position, false, AmanecidaArena.WEAPON_FIGHT_PHASE.FIRST);
						this.arena.ActivateDeco(this.amanecidaFight);
					}
					else
					{
						this.laudesArena = gameObject3.GetComponent<LaudesArena>();
						if (this.laudesArena != null)
						{
							this.amanecida.SetLaudesArena(this.laudesArena, base.transform.position, true);
						}
						else
						{
							Debug.LogError("404: Arena not found!");
						}
					}
				}
			}
		}
	}

	[Button("TEST START FIGHT", ButtonSizes.Large)]
	public void StartAmanecidaFight()
	{
		this.bossMgr.StartBossFight();
		this.amanecida.Behaviour.StartCombat();
		if (this.laudesArena != null)
		{
			this.amanecida.SetLaudesArena(this.laudesArena, base.transform.position, false);
		}
	}

	[Button("TEST START INTRO", ButtonSizes.Large)]
	public void StartIntro()
	{
		this.amanecida.Behaviour.StartIntro();
		if (this.arena != null)
		{
			this.arena.StartIntro();
		}
		else if (this.laudesArena != null)
		{
			this.laudesArena.StartIntro(this.amanecida.Behaviour.currentWeapon);
		}
		else
		{
			Debug.LogError("Arena component not found!");
		}
	}

	private AmanecidasFightSpawner.AMANECIDAS_FIGHTS GetCurrentFightFromFlags()
	{
		if (!this.useFlagsForFightType)
		{
			return this.TEST_AmanecidaFightType;
		}
		if (Core.Events.GetFlag("SANTOS_LAUDES_ACTIVATED"))
		{
			return AmanecidasFightSpawner.AMANECIDAS_FIGHTS.LAUDES;
		}
		if (Core.Events.GetFlag("SANTOS_AMANECIDA_AXE_ACTIVATED"))
		{
			return AmanecidasFightSpawner.AMANECIDAS_FIGHTS.AXE;
		}
		if (Core.Events.GetFlag("SANTOS_AMANECIDA_FALCATA_ACTIVATED"))
		{
			return AmanecidasFightSpawner.AMANECIDAS_FIGHTS.FALCATA;
		}
		if (Core.Events.GetFlag("SANTOS_AMANECIDA_LANCE_ACTIVATED"))
		{
			return AmanecidasFightSpawner.AMANECIDAS_FIGHTS.LANCE;
		}
		if (Core.Events.GetFlag("SANTOS_AMANECIDA_BOW_ACTIVATED"))
		{
			return AmanecidasFightSpawner.AMANECIDAS_FIGHTS.BOW;
		}
		return this.TEST_AmanecidaFightType;
	}

	private const string AXE_FLAG = "SANTOS_AMANECIDA_AXE_ACTIVATED";

	private const string BOW_FLAG = "SANTOS_AMANECIDA_BOW_ACTIVATED";

	private const string FALCATA_FLAG = "SANTOS_AMANECIDA_FALCATA_ACTIVATED";

	private const string LANCE_FLAG = "SANTOS_AMANECIDA_LANCE_ACTIVATED";

	public GameObject bossPrefab;

	public GameObject currentBoss;

	public BossFightManager bossMgr;

	public AmanecidasFightSpawner.AMANECIDAS_FIGHTS TEST_AmanecidaFightType;

	public bool useFlagsForFightType = true;

	private Amanecidas amanecida;

	public int amanecidaFight;

	private AmanecidaArena arena;

	private LaudesArena laudesArena;

	public static AmanecidasFightSpawner Instance;

	public List<AmanecidasFightSpawner.AmanecidasFightData> fightData;

	public enum AMANECIDAS_FIGHTS
	{
		LANCE,
		AXE,
		FALCATA,
		BOW,
		LAUDES
	}

	[Serializable]
	public struct AmanecidasFightData
	{
		public AmanecidasFightSpawner.AMANECIDAS_FIGHTS fightType;

		public List<GameObject> prefabsToInstantiate;

		public Transform spawnPoint;

		[SerializeField]
		public LocalizedString displayName;
	}
}
