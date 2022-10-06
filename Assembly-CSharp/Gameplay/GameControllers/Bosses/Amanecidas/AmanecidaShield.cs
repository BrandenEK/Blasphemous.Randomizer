using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Framework.Managers;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.Amanecidas
{
	public class AmanecidaShield : MonoBehaviour
	{
		private void Awake()
		{
			this.shieldFragments = new List<AmanecidasShieldFragment>();
			foreach (Transform transform in this.shieldFragmentTransforms)
			{
				this.shieldFragments.Add(transform.GetComponentInChildren<AmanecidasShieldFragment>());
			}
		}

		public void SetDamagePercentage(float percentage)
		{
			this.CheckFragmentsToBreak(percentage);
		}

		public void Death()
		{
			base.StopAllCoroutines();
			this.BreakShield();
		}

		public void FlashShieldFromPenitent()
		{
			Vector2 vector = Core.Logic.Penitent.transform.position;
			Vector2 vector2 = vector - base.transform.position + Vector2.up;
			Vector2 p = base.transform.position + vector2.normalized * this.radius;
			base.StopAllCoroutines();
			base.StartCoroutine(this.FlashAllFragmentsRoutine(p, 0.3f));
		}

		public void FlashShieldFromDown()
		{
			base.StopAllCoroutines();
			Vector2 p = base.transform.position + Vector2.down * 4f;
			base.StartCoroutine(this.FlashAllFragmentsRoutine(p, 0.2f));
		}

		public void SetAlpha(SpriteRenderer spr, float a)
		{
			spr.color = new Color(spr.color.r, spr.color.g, spr.color.b, a);
		}

		public void BreakShield()
		{
			ShortcutExtensions.DOKill(base.transform, false);
			this.brokenFragmentParticles.transform.position = base.transform.position;
			this.brokenFragmentParticles.Emit(50);
			foreach (AmanecidasShieldFragment amanecidasShieldFragment in this.shieldFragments)
			{
				AmanecidasShieldFragment.AMA_SHIELD_FRAGMENT_STATES currentState = amanecidasShieldFragment.currentState;
				if (currentState != AmanecidasShieldFragment.AMA_SHIELD_FRAGMENT_STATES.SHIELD)
				{
					if (currentState == AmanecidasShieldFragment.AMA_SHIELD_FRAGMENT_STATES.RISING)
					{
						amanecidasShieldFragment.FadeOut(0.2f);
					}
				}
				else
				{
					amanecidasShieldFragment.BreakFromShield((amanecidasShieldFragment.transform.position - base.transform.position).normalized * 8f);
				}
			}
		}

		private IEnumerator RecoverShieldCoroutine(float secondsRising = 1f, float secondsWait = 1f, float maxSecondsToTransform = 1f, float timeToPunchTransform = 1f, Action callback = null)
		{
			float rising = secondsRising / (float)this.shieldFragments.Count;
			foreach (AmanecidasShieldFragment item in this.shieldFragments)
			{
				Vector2 p = new Vector2(base.transform.position.x + (float)Random.Range(-6, 6), base.transform.position.y);
				item.RaiseFromGround(p, rising);
				yield return new WaitForSeconds(rising);
			}
			yield return new WaitForSeconds(secondsWait);
			foreach (AmanecidasShieldFragment amanecidasShieldFragment in this.shieldFragments)
			{
				amanecidasShieldFragment.GoToShieldTransform(Random.Range(0.1f, maxSecondsToTransform));
			}
			yield return new WaitForSeconds(maxSecondsToTransform);
			this.SlowPunchShieldTransforms(timeToPunchTransform);
			if (callback != null)
			{
				callback();
			}
			yield break;
		}

		public void SlowPunchShieldTransforms(float totalTime)
		{
			float num = 3f;
			foreach (AmanecidasShieldFragment amanecidasShieldFragment in this.shieldFragments)
			{
				Vector2 vector = amanecidasShieldFragment.shieldTransform.localPosition;
				Vector2 vector2 = vector + vector.normalized * num;
				Debug.DrawLine(amanecidasShieldFragment.shieldTransform.TransformPoint(vector), amanecidasShieldFragment.shieldTransform.TransformPoint(vector), Color.red, 10f);
				Sequence sequence = DOTween.Sequence();
				TweenSettingsExtensions.Append(sequence, TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMove(amanecidasShieldFragment.shieldTransform, vector2, totalTime * 0.8f, false), 10));
				TweenSettingsExtensions.Append(sequence, TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMove(amanecidasShieldFragment.shieldTransform, vector, totalTime * 0.2f, false), 8));
				TweenExtensions.Play<Sequence>(sequence);
			}
		}

		public void StartToRecoverShield(float secondsRising = 1f, float secondsWait = 1f, float maxSecondsToTransform = 1f, float timeToPunchTransform = 1f, Action callback = null)
		{
			this.currentCoroutine = base.StartCoroutine(this.RecoverShieldCoroutine(secondsRising, secondsWait, maxSecondsToTransform, timeToPunchTransform, callback));
		}

		public void CheckFragmentsToBreak(float currentPercentage)
		{
			List<AmanecidasShieldFragment> list = (from x in this.shieldFragments
			where x.currentState == AmanecidasShieldFragment.AMA_SHIELD_FRAGMENT_STATES.BROKEN
			select x).ToList<AmanecidasShieldFragment>();
			float num = (float)list.Count / (float)this.fragmentsToDestroy;
			if (1f - num > currentPercentage)
			{
				this.DestroyFragment();
			}
		}

		private void DestroyFragment()
		{
			List<AmanecidasShieldFragment> list = (from x in this.shieldFragments
			where x.currentState == AmanecidasShieldFragment.AMA_SHIELD_FRAGMENT_STATES.SHIELD
			select x).ToList<AmanecidasShieldFragment>();
			if (list.Count > 0)
			{
				AmanecidasShieldFragment amanecidasShieldFragment = list[Random.Range(0, list.Count)];
				amanecidasShieldFragment.BreakFromShield(amanecidasShieldFragment.transform.position - base.transform.position);
				this.brokenFragmentParticles.transform.position = amanecidasShieldFragment.transform.position;
				this.brokenFragmentParticles.Emit(30);
			}
		}

		public void ShakeWave(Vector2 pos)
		{
			Core.Logic.ScreenFreeze.Freeze(0.1f, 0.15f, 0f, null);
			Core.Logic.CameraManager.ShockwaveManager.Shockwave(base.transform.position, 0.5f, 0.3f, 2f);
			Core.Logic.CameraManager.ProCamera2DShake.Shake(0.5f, Vector3.down * 0.5f, 12, 0.2f, 0.01f, default(Vector3), 0.01f, true);
		}

		private IEnumerator FlashAllFragmentsRoutine(Vector2 p, float totalSeconds)
		{
			List<AmanecidasShieldFragment> fragmentsInShield = (from x in this.shieldFragments
			where x.currentState == AmanecidasShieldFragment.AMA_SHIELD_FRAGMENT_STATES.SHIELD
			select x).ToList<AmanecidasShieldFragment>();
			for (int j = 0; j < fragmentsInShield.Count; j++)
			{
				fragmentsInShield[j].FadeIn(0.1f);
			}
			if (fragmentsInShield.Count > 0)
			{
				List<AmanecidasShieldFragment> byDistance = this.SortFragmentsByDistance(p, fragmentsInShield);
				float secondsBetweenFlash = totalSeconds / (float)byDistance.Count;
				if (this.amanecidas.AnimatorInyector.IsOut())
				{
					secondsBetweenFlash = 0.01f;
				}
				for (int i = 0; i < byDistance.Count; i++)
				{
					byDistance[i].Flash();
					yield return new WaitForSeconds(secondsBetweenFlash);
				}
			}
			yield return new WaitForSeconds(0.15f);
			float fadeOutTime = 0.4f;
			if (this.amanecidas.AnimatorInyector.IsOut())
			{
				fadeOutTime = 0.05f;
			}
			for (int k = 0; k < fragmentsInShield.Count; k++)
			{
				fragmentsInShield[k].FadeOut(fadeOutTime);
			}
			yield break;
		}

		public List<AmanecidasShieldFragment> SortFragmentsByDistance(Vector2 point, List<AmanecidasShieldFragment> eligibleFragments)
		{
			List<AmanecidaShield.FragmentSortingData> list = new List<AmanecidaShield.FragmentSortingData>();
			for (int i = 0; i < eligibleFragments.Count; i++)
			{
				AmanecidaShield.FragmentSortingData item = new AmanecidaShield.FragmentSortingData
				{
					d = Vector2.Distance(point, eligibleFragments[i].transform.position),
					fr = eligibleFragments[i]
				};
				list.Add(item);
			}
			return (from x in list
			orderby x.d
			select x.fr).ToList<AmanecidasShieldFragment>();
		}

		public void InterruptShieldRecharge()
		{
			if (this.currentCoroutine != null)
			{
				base.StopCoroutine(this.currentCoroutine);
			}
			foreach (AmanecidasShieldFragment amanecidasShieldFragment in this.shieldFragments)
			{
				amanecidasShieldFragment.FadeOut(0.2f);
			}
		}

		public float timeToShow = 0.1f;

		public float timeToHide = 0.6f;

		public ParticleSystem brokenShieldParticles;

		public ParticleSystem brokenFragmentParticles;

		public ParticleSystem recoverShieldParticles;

		public float alphaHidden;

		public float alphaShowing = 0.4f;

		public Color originColor;

		public Color damagedColor;

		public float fadeDuration = 1f;

		public float secondaryDuration = 1f;

		public SpriteRenderer mainSprite;

		public SpriteRenderer secondarySprite;

		public Animator shieldAnimator;

		public ParticleSystem stencilFragmentsParticles;

		public float radius = 3f;

		public List<Transform> shieldFragmentTransforms;

		public Amanecidas amanecidas;

		private List<AmanecidasShieldFragment> shieldFragments;

		private Coroutine currentCoroutine;

		private int fragmentsToDestroy = 8;

		private struct FragmentSortingData
		{
			public AmanecidasShieldFragment fr;

			public float d;
		}
	}
}
