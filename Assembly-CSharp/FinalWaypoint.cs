using System;
using Framework.Managers;
using Gameplay.UI;
using Tools.Level.Interactables;
using UnityEngine;

public class FinalWaypoint : MonoBehaviour
{
	private void Start()
	{
		base.GetComponent<PrieDieu>().OnStartUsing += this.OnUseStart;
	}

	private void OnUseStart()
	{
		Core.Metrics.CustomEvent("GAME_COMPLETED", string.Empty, Time.time);
		Core.Audio.PlaySfxOnCatalog(this.impactAudio, this.audioTiming);
		base.Invoke("Fade", this.impactTiming);
		base.Invoke("FinishPrototype", this.sceneChangeTiming);
	}

	private void Fade()
	{
		UIController.instance.fade.CrossFadeAlpha(1f, 0f, false);
	}

	private void FinishPrototype()
	{
		Core.Logic.LoadMenuScene(true);
	}

	public string impactAudio;

	public float impactTiming = 0.5f;

	public float sceneChangeTiming = 1f;

	public float audioTiming = 0.35f;
}
