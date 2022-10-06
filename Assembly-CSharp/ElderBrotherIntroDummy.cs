using System;
using System.Collections;
using DG.Tweening;
using Framework.Managers;
using Gameplay.GameControllers.Bosses.ElderBrother;
using Gameplay.GameControllers.Enemies.MasterAnguish.Audio;
using Sirenix.OdinInspector;
using UnityEngine;

public class ElderBrotherIntroDummy : MonoBehaviour
{
	public void AnimEvent_ActivateBarrier()
	{
	}

	public void AnimEvent_DeactivateBarrier()
	{
	}

	[Button("TEST ANIMATION", 0)]
	public void TriggerIntro()
	{
		base.StartCoroutine(this.IntroDummyJump());
	}

	private void ShakeWave()
	{
		Core.Logic.CameraManager.ShockwaveManager.Shockwave(base.transform.position, 0.7f, 0.3f, 2f);
		Core.Logic.CameraManager.ProCamera2DShake.Shake(0.5f, Vector3.down * 1f, 12, 0.2f, 0f, default(Vector3), 0f, false);
	}

	private IEnumerator IntroDummyJump()
	{
		this.BigSmashPreparation();
		yield return new WaitForSeconds(1f);
		this.Smash();
		yield return new WaitForSeconds(0.3f);
		this.ShakeWave();
		yield return new WaitForSeconds(1f);
		this.BigSmashPreparation();
		yield return new WaitForSeconds(1f);
		this.Smash();
		yield return new WaitForSeconds(0.3f);
		this.ShakeWave();
		yield return new WaitForSeconds(1.6f);
		this.SetMidAir(true);
		yield return new WaitForSeconds(0.7f);
		TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMoveY(base.transform, base.transform.position.y + 12f, 0.8f, false), 9);
		yield return new WaitForSeconds(0.1f);
		this.elderBro.IntroJump();
		yield break;
	}

	public void BigSmashPreparation()
	{
		this.animator.SetTrigger("PREPARATION");
	}

	public void Smash()
	{
		this.animator.SetTrigger("SMASH");
	}

	public void SetMidAir(bool midAir)
	{
		this.animator.SetBool("MID-AIR", midAir);
	}

	public void PlayJump()
	{
		this.elderBroAudio.PlayDummyJump();
	}

	public void PlayAttack()
	{
		this.elderBroAudio.PlayAttack();
	}

	public void PlayAttackMove2()
	{
		this.elderBroAudio.PlayAttackMove2();
	}

	public void PlayAttackMove3()
	{
		this.elderBroAudio.PlayAttackMove3();
	}

	public Animator animator;

	public ElderBrotherBehaviour elderBro;

	public ElderBrotherAudio elderBroAudio;
}
