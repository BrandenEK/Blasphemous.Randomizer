using System;
using System.Collections;
using DG.Tweening;
using Framework.Managers;
using Gameplay.GameControllers.Bosses.BurntFace;
using Gameplay.GameControllers.Effects.Entity;
using Gameplay.GameControllers.Effects.Player.GhostTrail;
using Gameplay.GameControllers.Penitent;
using Sirenix.OdinInspector;
using UnityEngine;

public class BlindBabyGrabManager : MonoBehaviour
{
	private void PlayWarning()
	{
		this.grabWarningAnimator.SetTrigger("ACTIVATE");
	}

	private void Awake()
	{
		this.results = new Collider2D[1];
		this.attackCollider = base.GetComponentInChildren<Collider2D>();
		this.babyAnimator = base.GetComponentInChildren<Animator>();
		this.hasGrabbedPenitent = false;
	}

	public void PlayDeath()
	{
		this.StopSway();
		this.grabWarningAnimator.gameObject.SetActive(false);
		ShortcutExtensions.DOKill(base.transform, false);
		base.StopAllCoroutines();
		this.babyAnimator.SetTrigger("DEATH");
		ShortcutExtensions.DOMoveY(base.transform, base.transform.position.y - 10f, 10f, false);
	}

	public void StartSway()
	{
		this.swayTween = TweenSettingsExtensions.SetLoops<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMoveY(base.transform, base.transform.position.y - 1.5f, 5f, false), 7), -1, 1);
		TweenSettingsExtensions.OnComplete<Tween>(this.swayTween, new TweenCallback(this.OnSwayComplete));
	}

	private void OnSwayComplete()
	{
		if (this.moveTowardsPenitent)
		{
			TweenExtensions.Pause<Tween>(this.swayTween);
			Vector2 vector = Core.Logic.Penitent.transform.position - base.transform.position;
			float num = 3f;
			float num2 = num * Mathf.Sign(vector.x);
			TweenSettingsExtensions.SetEase<Tweener>(TweenSettingsExtensions.OnComplete<Tweener>(ShortcutExtensions.DOMoveX(base.transform, base.transform.position.x + num2, 0.5f, false), new TweenCallback(this.OnStepCompleted)), 7);
		}
	}

	private void OnStepCompleted()
	{
		TweenExtensions.Play<Tween>(this.swayTween);
	}

	public void StopSway()
	{
		ShortcutExtensions.DOKill(base.transform, false);
	}

	public void BabyIntro()
	{
		this.StopSway();
		this.moveTowardsPenitent = true;
		TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMoveY(base.transform, base.transform.position.y + 11f, 3f, false), 7), new TweenCallback(this.StartSway));
		this.babyEffects.TriggerColorizeLerp(1f, this.hiddenLerpValue, 3f, null);
		this.motherEffects.TriggerColorizeLerp(1f, this.hiddenLerpValue, 3f, null);
	}

	public void StartGrabAttack()
	{
		this.StopSway();
		this.SetCry(true);
		this.originPoint = base.transform.position;
		base.StartCoroutine(this.GrabAttackCoroutine(this.grabPointTransform));
	}

	public bool HasGrabbedPenitent()
	{
		return this.hasGrabbedPenitent;
	}

	private void Start()
	{
		this.StartSway();
	}

	private IEnumerator GrabAttackCoroutine(Transform grabPoint)
	{
		this.GrabAnticipation(grabPoint);
		float ad = this.anticipationDuration * 0.75f;
		base.StartCoroutine(this.MakeGrabPointFollowPlayer(ad));
		yield return new WaitForSeconds(ad);
		this.BabyMovesIntoGrab(grabPoint);
		yield return new WaitForSeconds(this.movementDuration);
		this.TryGrabPlayer();
		yield break;
	}

	private IEnumerator MakeGrabPointFollowPlayer(float duration)
	{
		float c = 0f;
		while (c < duration)
		{
			c += Time.deltaTime;
			this.SetGrabPointToPlayerPosition();
			yield return null;
		}
		yield break;
	}

	private void SetGrabPointToPlayerPosition()
	{
		this.grabPointTransform.position = new Vector3(Core.Logic.Penitent.transform.position.x, this.grabPointTransform.position.y, this.grabPointTransform.position.z);
	}

	private IEnumerator GrabSuccessCoroutine()
	{
		this.hasGrabbedPenitent = true;
		GhostTrailGenerator.AreGhostTrailsAllowed = false;
		this.HidePlayer();
		this.PlayGrabAnimation();
		TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMoveY(base.transform, base.transform.position.y + 1.5f, 1.5f, false), 6);
		float grabAnimationDuration = 3f;
		yield return new WaitForSeconds(grabAnimationDuration);
		this.KillPlayer();
		yield break;
	}

	private void KillPlayer()
	{
		Debug.Log("KillPlayer()");
		Core.Input.SetBlocker("PLAYER_LOGIC", false);
		Core.Logic.Penitent.Kill();
	}

	private void PlayGrabAnimation()
	{
		Debug.Log("PlayGrabAnimation()");
		this.Audio.PlayBabyGrab_AUDIO();
		this.babyAnimator.SetTrigger("EXECUTION");
	}

	private void HidePlayer()
	{
		Debug.Log("HidePlayer()");
		Core.Input.SetBlocker("PLAYER_LOGIC", true);
		Penitent penitent = Core.Logic.Penitent;
		penitent.SpriteRenderer.enabled = false;
		penitent.Status.CastShadow = false;
	}

	private IEnumerator GrabFailCoroutine()
	{
		this.hasGrabbedPenitent = false;
		this.SetCry(true);
		this.ReturnBabyToIdle();
		yield return new WaitForSeconds(this.movementDuration * 2f);
		this.StartSway();
		this.moveTowardsPenitent = true;
		yield break;
	}

	public void SetCry(bool shake)
	{
		this.babyAnimator.SetTrigger("CRY");
		this.Audio.PlayCry_AUDIO();
		if (shake)
		{
			Core.Logic.CameraManager.ProCamera2DShake.Shake(2f, Vector3.down * 3f, 60, 0.2f, 0f, default(Vector3), 0.05f, false);
		}
	}

	public void ArmRipShake()
	{
		Core.Logic.CameraManager.ProCamera2DShake.Shake(0.5f, Vector3.right * 3f, 60, 0.2f, 0f, default(Vector3), 0.05f, false);
	}

	private void ReturnBabyToIdle()
	{
		TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(base.transform, this.originPoint, this.movementDuration * 2f, false), 10);
		this.babyEffects.TriggerColorizeLerp(0f, this.hiddenLerpValue, this.movementDuration, null);
		this.motherEffects.TriggerColorizeLerp(0f, this.hiddenLerpValue, this.movementDuration, null);
	}

	private void BabyMovesIntoGrab(Transform grabPoint)
	{
		TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(base.transform, grabPoint.position - this.attackCollider.offset, this.movementDuration, false), 10);
		this.babyEffects.TriggerColorizeLerp(this.hiddenLerpValue, 0f, this.movementDuration, null);
		this.motherEffects.TriggerColorizeLerp(this.hiddenLerpValue, 0f, this.movementDuration, null);
	}

	private void GrabAnticipation(Transform gpoint)
	{
		Vector2 vector = gpoint.position;
		this.PlayWarning();
		Vector2 vector2 = vector + Vector2.left * this.grabWidth / 2f;
		Vector2 vector3 = vector + Vector2.right * this.grabWidth / 2f;
		Debug.DrawLine(vector, vector2, Color.yellow, this.anticipationDuration);
		Debug.DrawLine(vector + Vector2.up * this.grabHeight, vector2 + Vector2.up * this.grabHeight, Color.yellow, this.anticipationDuration);
		Debug.DrawLine(vector, vector3, Color.yellow, this.anticipationDuration);
		Debug.DrawLine(vector + Vector2.up * this.grabHeight, vector3 + Vector2.up * this.grabHeight, Color.yellow, this.anticipationDuration);
	}

	private void TryGrabPlayer()
	{
		Vector2 vector = base.transform.position;
		Vector2 vector2 = vector + Vector2.left * this.grabWidth / 2f;
		Vector2 vector3 = vector + Vector2.right * this.grabWidth / 2f;
		Debug.DrawLine(vector, vector2, Color.yellow, this.anticipationDuration);
		Debug.DrawLine(vector + Vector2.up * this.grabHeight, vector2 + Vector2.up * this.grabHeight, Color.red, this.anticipationDuration);
		Debug.DrawLine(vector, vector3, Color.yellow, this.anticipationDuration);
		Debug.DrawLine(vector + Vector2.up * this.grabHeight, vector3 + Vector2.up * this.grabHeight, Color.red, this.anticipationDuration);
		if (this.attackCollider.OverlapCollider(this.contactFilter, this.results) > 0 && !Core.Logic.Penitent.Status.Invulnerable && (!Core.InventoryManager.IsPrayerEquipped("PR202") || !Core.Logic.Penitent.PrayerCast.IsUsingAbility))
		{
			base.StartCoroutine(this.GrabSuccessCoroutine());
		}
		else
		{
			base.StartCoroutine(this.GrabFailCoroutine());
		}
	}

	[FoldoutGroup("Grab attack settings", 0)]
	public float grabWidth;

	[FoldoutGroup("Grab attack settings", 0)]
	public float grabHeight;

	[FoldoutGroup("Grab attack settings", 0)]
	public float anticipationDuration = 3f;

	[FoldoutGroup("Grab attack settings", 0)]
	public float movementDuration = 2f;

	[FoldoutGroup("Grab attack settings", 0)]
	public ContactFilter2D contactFilter;

	public float followSpeed = 1f;

	public Transform grabPointTransform;

	public WickerWurmAudio Audio;

	public Animator grabWarningAnimator;

	private Animator babyAnimator;

	private Vector2 originPoint;

	private Collider2D attackCollider;

	private Collider2D[] results;

	public MasterShaderEffects babyEffects;

	public MasterShaderEffects motherEffects;

	public float hiddenLerpValue = 0.7f;

	private Tween swayTween;

	public bool moveTowardsPenitent;

	private bool hasGrabbedPenitent;
}
