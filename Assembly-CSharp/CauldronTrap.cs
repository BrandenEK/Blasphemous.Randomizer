using System;
using System.Collections;
using FMOD.Studio;
using FMODUnity;
using Framework.Managers;
using Sirenix.OdinInspector;
using Tools.Level;
using UnityEngine;

public class CauldronTrap : MonoBehaviour, IActionable
{
	public bool TrapActivated { get; set; }

	public bool Locked { get; set; }

	private void Awake()
	{
		this.TrapActivated = false;
	}

	private IEnumerator GrowCoroutine(float maxLength, float seconds)
	{
		float counter = 0f;
		while (counter < seconds)
		{
			float i = Mathf.Lerp(0f, maxLength, counter / seconds);
			this.beam.maxRange = i;
			counter += Time.deltaTime;
			yield return null;
			this.beamAnimator.speed = 0.01f;
		}
		this.beam.maxRange = maxLength;
		this.beamAnimator.speed = 1f;
		yield break;
	}

	private void StartGrow()
	{
		base.StartCoroutine(this.GrowCoroutine(this.beam.GetDistance() + 0.2f, this.maxSeconds));
	}

	[Button("Test Activate", ButtonSizes.Small)]
	private void Activate()
	{
		this.TrapActivated = true;
		this.trapAnimator.SetBool("ATTACK", true);
		this.PlayFallAudio();
		base.StartCoroutine(this.DeactivateAfterSeconds());
	}

	[Button("Test End", ButtonSizes.Small)]
	public void Deactivate()
	{
		this.StopFallAudio();
		this.TrapActivated = false;
		this.trapAnimator.SetBool("ATTACK", false);
	}

	private IEnumerator DeactivateAfterSeconds()
	{
		yield return new WaitForSeconds(this.timeActive);
		if (this.TrapActivated)
		{
			this.Use();
		}
		yield break;
	}

	public void StartFall()
	{
		this.beamAnimator.SetTrigger("FALL");
		this.beam.ActivateEndAnimation(true, false);
		this.StartGrow();
	}

	public void StopFall()
	{
		this.beamAnimator.SetTrigger("VANISH");
		this.beam.growSprite.gameObject.SetActive(false);
		this.beam.ActivateEndAnimation(false, true);
	}

	public void Use()
	{
		Debug.Log("USING TRAP");
		if (this.fallDelay > 0f)
		{
			base.Invoke("Toggle", this.fallDelay);
		}
		else
		{
			this.Toggle();
		}
	}

	private void Toggle()
	{
		if (this.TrapActivated)
		{
			this.Deactivate();
		}
		else
		{
			this.Activate();
		}
	}

	private void PlayFallAudio()
	{
		if (string.IsNullOrEmpty(this.FallAudio))
		{
			return;
		}
		this.DisposeFallAudioInstance();
		Core.Audio.PlayEventNoCatalog(ref this._fallAudioEvent, this.FallAudio, default(Vector3));
	}

	private void StopFallAudio()
	{
		ParameterInstance parameterInstance;
		this._fallAudioEvent.getParameter("End", out parameterInstance);
		parameterInstance.setValue(1f);
	}

	private void DisposeFallAudioInstance()
	{
		if (!this._fallAudioEvent.isValid())
		{
			return;
		}
		this._fallAudioEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		this._fallAudioEvent.release();
		this._fallAudioEvent = default(EventInstance);
	}

	private void OnDestroy()
	{
		this.DisposeFallAudioInstance();
	}

	[FoldoutGroup("References", 0)]
	public Animator trapAnimator;

	[FoldoutGroup("References", 0)]
	public Animator beamAnimator;

	[FoldoutGroup("References", 0)]
	public TileableBeamLauncher beam;

	[FoldoutGroup("References", 0)]
	public Transform origin;

	[FoldoutGroup("Design settings", 0)]
	public float fallDelay = 0.3f;

	[FoldoutGroup("Audio", 0)]
	[EventRef]
	public string FallAudio;

	private EventInstance _fallAudioEvent;

	public float timeActive = 5f;

	public float maxSeconds = 1f;
}
