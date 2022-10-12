using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FMODUnity;
using Framework.Managers;
using Gameplay.GameControllers.Effects.Entity;
using Gameplay.GameControllers.Entities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Tools.Level.Actionables
{
	public class AshPlatform : MonoBehaviour, IActionable
	{
		private void Awake()
		{
			if (this.collision != null)
			{
				this.collision.enabled = false;
			}
			this.spriteEffects = base.GetComponentInChildren<MasterShaderEffects>();
		}

		private void Start()
		{
			PoolManager.Instance.CreatePool(this.nextPlatformParticles, this.target.Length);
			this.animator.SetBool("ENABLED", false);
			this.showing = false;
			this.collision.enabled = false;
		}

		private void OnDestroy()
		{
		}

		private IEnumerator DeactivateAfterSeconds(float seconds, float warningPercentage = 0.8f, Action callbackOnWarning = null)
		{
			float counter = 0f;
			bool warningActivated = false;
			while (counter < seconds)
			{
				counter += Time.deltaTime;
				yield return null;
				if (warningPercentage < counter / seconds && !warningActivated)
				{
					callbackOnWarning();
					warningActivated = true;
				}
			}
			this.HideAction();
			yield break;
		}

		private void DeactivationWarning()
		{
			this.spriteEffects.TriggerColorizeLerp(0f, 1f, 1f, null);
		}

		public void Show()
		{
			this.spriteEffects.DeactivateColorize();
			this.ShowAction();
		}

		public void Hide(float delay, float warningPercentage = 0.8f)
		{
			base.StopAllCoroutines();
			this.spriteEffects.DeactivateColorize();
			base.StartCoroutine(this.DeactivateAfterSeconds(delay, warningPercentage, new Action(this.DeactivationWarning)));
		}

		private void ShowAction()
		{
			if (this.spriteRenderer.isVisible && !this.showing)
			{
				Core.Audio.PlaySfx(this.appearSound, 0f);
			}
			this.animator.SetBool("ENABLED", true);
			this.showing = true;
			this.collision.enabled = true;
		}

		private void HideAction()
		{
			if (this.spriteRenderer.isVisible && this.showing)
			{
				Core.Audio.PlaySfx(this.disappearSound, 0f);
			}
			this.animator.SetBool("ENABLED", false);
			this.showing = false;
			this.collision.enabled = false;
		}

		private void EffectToNextPlatform(AshPlatform p)
		{
			Vector3 position = base.transform.position;
			Vector3 position2 = p.transform.position;
			GameObject gameObject = PoolManager.Instance.ReuseObject(this.nextPlatformParticles, position, Quaternion.identity, false, 1).GameObject;
			gameObject.transform.DOMove(position2, 0.2f, false).SetEase(Ease.OutCubic);
		}

		public List<GameObject> GetTargets()
		{
			return new List<GameObject>(this.target);
		}

		public void Use()
		{
			this.Show();
		}

		public bool Locked { get; set; }

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		private bool firstPlatform;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		private GameObject[] target;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		[EventRef]
		private string appearSound;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		[EventRef]
		private string disappearSound;

		public bool showing;

		private Tween currentTween;

		[SerializeField]
		[BoxGroup("Attached References", true, false, 0)]
		private SpriteRenderer spriteRenderer;

		[SerializeField]
		[BoxGroup("Attached References", true, false, 0)]
		private CollisionSensor collisionSensor;

		[SerializeField]
		[BoxGroup("Attached References", true, false, 0)]
		private Collider2D collision;

		[SerializeField]
		[BoxGroup("Attached References", true, false, 0)]
		private Animator animator;

		[SerializeField]
		[BoxGroup("Attached References", true, false, 0)]
		private GameObject nextPlatformParticles;

		private MasterShaderEffects spriteEffects;
	}
}
