using System;
using System.Collections;
using System.Collections.Generic;
using Com.LuisPedroFonseca.ProCamera2D;
using DG.Tweening;
using Gameplay.GameControllers.Bosses.Amanecidas;
using Gameplay.GameControllers.Camera;
using Sirenix.OdinInspector;
using UnityEngine;

public class AmanecidaArena : MonoBehaviour
{
	[Button(0)]
	public void TestInfluence()
	{
		if (this.currentPhaseConfig.useCameraInfluence)
		{
			ProCamera2D.Instance.ApplyInfluence(this.currentPhaseConfig.cameraInfluence);
		}
	}

	private void Update()
	{
		this.ApplyPhaseInfluence();
	}

	private void ApplyPhaseInfluence()
	{
		if (this.currentPhaseConfig.useCameraInfluence)
		{
			ProCamera2D.Instance.ApplyInfluence(this.currentPhaseConfig.cameraInfluence);
		}
	}

	public void ActivateArena(Amanecidas amanecida, Vector2 origin, bool onlySetBattleBounds = false, AmanecidaArena.WEAPON_FIGHT_PHASE fightPhase = AmanecidaArena.WEAPON_FIGHT_PHASE.FIRST)
	{
		if (fightPhase == AmanecidaArena.WEAPON_FIGHT_PHASE.FIRST)
		{
			amanecida.Behaviour.battleBounds = this.battleBounds;
			amanecida.Behaviour.battleBounds.center = origin + this.boundsCenterOffset;
		}
		if (onlySetBattleBounds)
		{
			return;
		}
		AmanecidaArena.GameObjectsToActivateByPhase gameObjectsToActivateByPhase = this.gameobjectsToActivateEachPhase.Find((AmanecidaArena.GameObjectsToActivateByPhase x) => x.phase == fightPhase);
		this.currentPhaseConfig = gameObjectsToActivateByPhase;
		List<GameObject> gameobjects = gameObjectsToActivateByPhase.gameobjects;
		if (gameobjects != null)
		{
			foreach (GameObject gameObject in gameobjects)
			{
				gameObject.SetActive(true);
			}
		}
		if (amanecida.IsLaudes)
		{
			this.ActivateLaudesDeco(fightPhase);
		}
		else
		{
			CameraNumericBoundaries component = this.camBoundariesGo.GetComponent<CameraNumericBoundaries>();
			component.CenterKeepSize();
			component.SetBoundaries();
		}
	}

	public void StartIntro()
	{
		if (this.tweenParent != null)
		{
			this.tweenParent.gameObject.SetActive(true);
			float num = 2f;
			float num2 = 2f;
			this.tweenParent.transform.localPosition += Vector3.down * num;
			ShortcutExtensions.DOLocalMoveY(this.tweenParent.transform, this.tweenParent.transform.localPosition.y + num, num2, false);
		}
	}

	public void StartDeactivateArena()
	{
		base.StartCoroutine(this.DeactivateArena());
	}

	private IEnumerator DeactivateArena()
	{
		foreach (AmanecidaArena.GameObjectsToActivateByPhase gameObjectsToActivateByPhase in this.gameobjectsToActivateEachPhase)
		{
			AmanecidaArena.WEAPON_FIGHT_PHASE phase = gameObjectsToActivateByPhase.phase;
			this.DectivateLaudesDeco(phase);
		}
		yield return new WaitForSeconds(this.deactivateSeconds * 0.5f);
		foreach (AmanecidaArena.GameObjectsToActivateByPhase gameObjectsToActivateByPhase2 in this.gameobjectsToActivateEachPhase)
		{
			List<GameObject> gameobjects = gameObjectsToActivateByPhase2.gameobjects;
			if (gameobjects != null)
			{
				foreach (GameObject gameObject in gameobjects)
				{
					BoxCollider2D componentInChildren = gameObject.GetComponentInChildren<BoxCollider2D>();
					if (componentInChildren != null)
					{
						componentInChildren.enabled = false;
					}
				}
			}
		}
		yield return new WaitForSeconds(this.deactivateSeconds * 0.5f);
		foreach (AmanecidaArena.GameObjectsToActivateByPhase gameObjectsToActivateByPhase3 in this.gameobjectsToActivateEachPhase)
		{
			List<GameObject> gameobjects2 = gameObjectsToActivateByPhase3.gameobjects;
			if (gameobjects2 != null)
			{
				foreach (GameObject gameObject2 in gameobjects2)
				{
					gameObject2.SetActive(false);
				}
			}
		}
		this.currentPhaseConfig = default(AmanecidaArena.GameObjectsToActivateByPhase);
		yield break;
	}

	public void ActivateDeco(int index)
	{
		for (int i = 0; i < this.decoParentsInOrder.Count; i++)
		{
			this.decoParentsInOrder[i].SetActive(index == i);
		}
	}

	public void ActivateLaudesDeco(AmanecidaArena.WEAPON_FIGHT_PHASE fightPhase)
	{
		for (int i = 0; i < this.decoParentsInOrder[(int)fightPhase].transform.childCount; i++)
		{
			LaudesPlatformController[] componentsInChildren = this.decoParentsInOrder[(int)fightPhase].GetComponentsInChildren<LaudesPlatformController>();
			foreach (LaudesPlatformController laudesPlatformController in componentsInChildren)
			{
				laudesPlatformController.ShowAllPlatforms();
			}
		}
	}

	public void PlayParticles(AmanecidaArena.WEAPON_FIGHT_PHASE fightPhase)
	{
		for (int i = 0; i < this.decoParentsInOrder[(int)fightPhase].transform.childCount; i++)
		{
			ParticleSystem[] componentsInChildren = this.decoParentsInOrder[(int)fightPhase].transform.GetChild(i).GetComponentsInChildren<ParticleSystem>();
			foreach (ParticleSystem particleSystem in componentsInChildren)
			{
				particleSystem.Play();
			}
		}
	}

	public void DectivateLaudesDeco(AmanecidaArena.WEAPON_FIGHT_PHASE fightPhase)
	{
		for (int i = 0; i < this.decoParentsInOrder[(int)fightPhase].transform.childCount; i++)
		{
			LaudesPlatformController[] componentsInChildren = this.decoParentsInOrder[(int)fightPhase].GetComponentsInChildren<LaudesPlatformController>();
			foreach (LaudesPlatformController laudesPlatformController in componentsInChildren)
			{
				laudesPlatformController.HideAllPlatforms();
			}
		}
	}

	[Button("Editor Tool: Fill deco parents", 0)]
	public void FillDecoParents()
	{
		this.decoParentsInOrder.Clear();
		Transform transform = base.transform.Find("ARENA_DECO");
		if (transform != null)
		{
			IEnumerator enumerator = transform.transform.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					Transform transform2 = (Transform)obj;
					this.decoParentsInOrder.Add(transform2.gameObject);
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			Debug.Log("Deco parents added");
		}
		else
		{
			Debug.LogError("Couldnt find child object << ARENA_DECO >>");
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(base.transform.position + this.boundsCenterOffset, 0.1f);
		Gizmos.DrawWireCube(this.battleBounds.center, this.battleBounds.size);
	}

	public List<AmanecidaArena.GameObjectsToActivateByPhase> gameobjectsToActivateEachPhase;

	public GameObject camBoundariesGo;

	public List<GameObject> decoParentsInOrder;

	public Rect battleBounds;

	public Vector2 boundsCenterOffset;

	private float deactivateSeconds = 0.8f;

	private float activateSeconds = 0.8f;

	public GameObject tweenParent;

	private AmanecidaArena.GameObjectsToActivateByPhase currentPhaseConfig;

	public enum WEAPON_FIGHT_PHASE
	{
		FIRST,
		SECOND,
		THIRD
	}

	[Serializable]
	public struct GameObjectsToActivateByPhase
	{
		public AmanecidaArena.WEAPON_FIGHT_PHASE phase;

		public List<GameObject> gameobjects;

		public bool useCameraInfluence;

		public Vector2 cameraInfluence;
	}
}
