using System;
using System.Collections.Generic;
using Gameplay.GameControllers.Effects.Entity;
using UnityEngine;

public class AmanecidasAnimatorInyector : MonoBehaviour
{
	public AmanecidasAnimatorInyector.AMANECIDA_WEAPON GetWeapon()
	{
		return this.currentWeapon;
	}

	private void Awake()
	{
		this.bodySpr = this.bodyAnimator.GetComponent<SpriteRenderer>();
		this.wpnSpr = this.weaponAnimator.GetComponent<SpriteRenderer>();
	}

	public void Flip(bool flip)
	{
		this.bodySpr.flipX = flip;
		this.wpnSpr.flipX = flip;
	}

	public void SetAmanecidaColor(AmanecidasAnimatorInyector.AMANECIDA_COLOR color)
	{
		this.currentColor = color;
		AmanecidasAnimatorInyector.AmanecidaBodyAnimators amanecidaBodyAnimators = this.bodyAnimatorsByType.Find((AmanecidasAnimatorInyector.AmanecidaBodyAnimators x) => x.color == color);
		this.bodyAnimator.runtimeAnimatorController = amanecidaBodyAnimators.animatorController;
		this.amanecidaColor = amanecidaBodyAnimators.tintColor;
	}

	public Material GetCurrentBeamMaterial()
	{
		return this.bodyAnimatorsByType.Find((AmanecidasAnimatorInyector.AmanecidaBodyAnimators x) => x.color == this.currentColor).beamMaterial;
	}

	public void SetAmanecidaWeapon(AmanecidasAnimatorInyector.AMANECIDA_WEAPON weapon)
	{
		this.currentWeapon = weapon;
		AmanecidasAnimatorInyector.AmanecidaWeaponAnimators amanecidaWeaponAnimators = this.weaponAnimatorsByType.Find((AmanecidasAnimatorInyector.AmanecidaWeaponAnimators x) => x.weapon == weapon);
		this.weaponAnimator.runtimeAnimatorController = amanecidaWeaponAnimators.animatorController;
		this.energyChargeAnimator.GetComponentInChildren<SpriteRenderer>().material = amanecidaWeaponAnimators.recolorMaterial;
		this.SetParticlesColor(amanecidaWeaponAnimators.baseColor);
	}

	private void SetParticlesColor(Color c)
	{
	}

	public void SetVelocity(Vector2 v)
	{
		this.bodyAnimator.SetFloat("xVelocity", v.x);
		this.bodyAnimator.SetFloat("yVelocity", v.y);
		this.weaponAnimator.SetFloat("xVelocity", v.x);
		this.weaponAnimator.SetFloat("yVelocity", v.y);
		float num = Mathf.Abs(v.x) / 5f;
	}

	public bool IsTurning()
	{
		return this.bodyAnimator.GetCurrentAnimatorStateInfo(0).IsName("TURN AROUND");
	}

	public bool IsOut()
	{
		return this.bodyAnimator.GetCurrentAnimatorStateInfo(0).IsName("OUT");
	}

	public void PlayTurnAround(bool instant = false)
	{
		float normalizedTime = 0f;
		if (instant)
		{
			normalizedTime = 0.96f;
		}
		this.bodyAnimator.Play("TURN AROUND", 0, normalizedTime);
		this.weaponAnimator.Play("TURN AROUND", 0, normalizedTime);
	}

	public void PlayHurt()
	{
		this.SetDualTrigger("HURT");
	}

	public void PlayBlinkshot()
	{
		this.SetDualTrigger("BLINKSHOT");
	}

	public void PlaySummonWeapon()
	{
		this.SetDualTrigger("SUMMON_WEAPON");
	}

	public void PlayMeleeAttack()
	{
		this.SetDualTrigger("MELEE_ATTACK");
	}

	public void PlayStompAttack(bool doStompDamage)
	{
		this.SetDualTrigger("STOMP_ATTACK");
		this.SetDualBool("DO_STOMP_DAMAGE", doStompDamage);
	}

	public void ClearStompAttackTrigger()
	{
		this.ResetDualTrigger("STOMP_ATTACK");
	}

	public void PlayChargeAnticipation(bool isHorizontalCharge)
	{
		this.SetDualTrigger("CHARGE_ANTICIPATION");
		this.SetHorizontalCharge(isHorizontalCharge);
	}

	public void ClearAllTriggers()
	{
		this.ResetDualTrigger("HURT");
		this.ResetDualTrigger("CHARGE_ANTICIPATION");
		this.ResetDualTrigger("BLINKSHOT");
		this.ResetDualTrigger("MELEE_ATTACK");
		this.ResetDualTrigger("STOMP_ATTACK");
		this.ResetDualTrigger("SUMMON_WEAPON");
	}

	public void ClearAll(bool includeTriggers)
	{
		if (includeTriggers)
		{
			this.ClearAllTriggers();
		}
		this.SetRecharging(false);
		this.SetMeleeAnticipation(false);
		this.SetLunge(false);
		this.SetBlink(false);
		this.SetCharge(false);
	}

	public void SetRecharging(bool active)
	{
		this.SetDualBool("RECHARGE", active);
	}

	public void SetEnergyCharge(bool active)
	{
		this.energyChargeAnimator.SetBool("ACTIVE", active);
	}

	public void SetStuck(bool active)
	{
		this.SetDualBool("STUCK_WEAPON", active);
	}

	public void SetTired(bool active)
	{
		this.SetDualBool("TIRED", active);
	}

	public void SetShockwaveAnticipation(bool v)
	{
		this.SetDualBool("SHOCKWAVE_ANTICIPATION", v);
	}

	public void SetBlink(bool value)
	{
		this.SetDualBool("BLINK", value);
	}

	public void SetBow(bool value)
	{
		this.SetDualBool("BOW_LOOP", value);
	}

	public void SetMeleeHold(bool value)
	{
		this.SetDualBool("MELEE_HOLD_LOOP", value);
	}

	public void SetCharge(bool v)
	{
		this.ResetDualTrigger("CHARGE_ANTICIPATION");
		this.SetDualBool("CHARGE_LOOP", v);
	}

	public void Parry()
	{
		this.SetDualTrigger("PARRY");
	}

	public void SetLunge(bool v)
	{
		this.SetDualBool("LUNGE_LOOP", v);
	}

	public void SetMeleeAnticipation(bool v)
	{
		this.SetDualBool("MELEE_ANTICIPATION", v);
	}

	public void SetHorizontalCharge(bool v)
	{
		this.SetDualBool("HORIZONTAL_CHARGE", v);
	}

	private void ResetDualTrigger(string name)
	{
		this.bodyAnimator.ResetTrigger(name);
		this.weaponAnimator.ResetTrigger(name);
	}

	private void SetDualTrigger(string name)
	{
		this.bodyAnimator.SetTrigger(name);
		this.weaponAnimator.SetTrigger(name);
	}

	private void SetDualBool(string name, bool value)
	{
		this.bodyAnimator.SetBool(name, value);
		this.weaponAnimator.SetBool(name, value);
	}

	public void SetWeaponVisible(bool visible)
	{
		this.wpnSpr.enabled = visible;
	}

	public void FlipSpriteWithAngle(float angle)
	{
		this.Flip(false);
		float num = 1.5f;
		Debug.Log("ANGLE: " + angle);
		if (angle > 90f && angle < 270f)
		{
			this.bodySpr.flipY = true;
			this.wpnSpr.flipY = true;
			Debug.Log("FLIP Y");
		}
		else
		{
			this.bodySpr.flipY = false;
			this.wpnSpr.flipY = false;
			Debug.Log("NO FLIP Y");
		}
		float num2 = (float)((!this.bodySpr.flipY) ? -1 : 1);
		this.bodySpr.transform.localPosition = new Vector2(0f, num * num2);
		this.wpnSpr.transform.localPosition = new Vector2(0f, num * num2);
	}

	public void ActivateIntroColor()
	{
		this.shaderEffects.SetColorTint(this.amanecidaColor, 1f, true);
		this.shaderEffectsWeapon.SetColorTint(this.amanecidaColor, 1f, true);
	}

	public void DeactivateIntroColor()
	{
		this.shaderEffects.SetColorTint(this.amanecidaColor, 0f, false);
		this.shaderEffectsWeapon.SetColorTint(this.amanecidaColor, 0f, false);
	}

	public void SetSpriteRotation(float angle, float bowAngleDifference)
	{
		float z = (angle <= 90f || angle >= 270f) ? (angle + bowAngleDifference) : (angle - bowAngleDifference);
		Quaternion localRotation = Quaternion.Euler(0f, 0f, z);
		this.rotationParent.transform.localRotation = localRotation;
	}

	public Vector2 GetCurrentUp()
	{
		return this.bodySpr.transform.up * (float)((!this.bodySpr.flipY) ? 1 : -1);
	}

	public void ClearRotationAndFlip()
	{
		this.bodySpr.flipY = false;
		this.wpnSpr.flipY = false;
		float y = -1.5f;
		this.bodySpr.transform.localPosition = new Vector2(0f, y);
		this.wpnSpr.transform.localPosition = new Vector2(0f, y);
		this.SetSpriteRotation(0f, 0f);
	}

	public void PlayDeath()
	{
		this.SetDualTrigger("DEATH");
	}

	public void ShowSprites(bool show)
	{
		this.wpnSpr.enabled = show;
		this.bodySpr.enabled = show;
	}

	private const string TRIGGER_CHARGE_ANTICIPATION = "CHARGE_ANTICIPATION";

	private const string TRIGGER_HURT = "HURT";

	private const string TRIGGER_BLINKSHOT = "BLINKSHOT";

	private const string TRIGGER_SUMMON_WEAPON = "SUMMON_WEAPON";

	private const string TRIGGER_MELEE_ATTACK = "MELEE_ATTACK";

	private const string TRIGGER_STOMP_ATTACK = "STOMP_ATTACK";

	private const string TRIGGER_DO_STOMP_DAMAGE = "DO_STOMP_DAMAGE";

	public Animator bodyAnimator;

	public Animator weaponAnimator;

	public List<AmanecidasAnimatorInyector.AmanecidaBodyAnimators> bodyAnimatorsByType;

	public List<AmanecidasAnimatorInyector.AmanecidaWeaponAnimators> weaponAnimatorsByType;

	public Animator energyChargeAnimator;

	private AmanecidasAnimatorInyector.AMANECIDA_COLOR currentColor;

	private AmanecidasAnimatorInyector.AMANECIDA_WEAPON currentWeapon;

	public Transform rotationParent;

	private SpriteRenderer bodySpr;

	private SpriteRenderer wpnSpr;

	public MasterShaderEffects shaderEffects;

	public MasterShaderEffects shaderEffectsWeapon;

	private Color amanecidaColor;

	[Serializable]
	public struct AmanecidaBodyAnimators
	{
		public AmanecidasAnimatorInyector.AMANECIDA_COLOR color;

		public RuntimeAnimatorController animatorController;

		public Color tintColor;

		public Material beamMaterial;
	}

	[Serializable]
	public struct AmanecidaWeaponAnimators
	{
		public AmanecidasAnimatorInyector.AMANECIDA_WEAPON weapon;

		public RuntimeAnimatorController animatorController;

		public Material recolorMaterial;

		public Color baseColor;
	}

	public enum AMANECIDA_COLOR
	{
		BLUE,
		RED,
		SKYBLUE,
		WHITE,
		LAUDES
	}

	public enum AMANECIDA_WEAPON
	{
		HAND,
		AXE,
		LANCE,
		BOW,
		SWORD
	}
}
