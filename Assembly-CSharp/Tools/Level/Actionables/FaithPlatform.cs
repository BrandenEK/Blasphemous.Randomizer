using System;
using DG.Tweening;
using FMODUnity;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Tools.Level.Actionables
{
	public class FaithPlatform : MonoBehaviour, IActionable, INoSafePosition
	{
		private void Awake()
		{
			if (this.collision != null)
			{
				this.collision.enabled = false;
			}
			Core.Events.OnFlagChanged += this.OnFlagChanged;
			if (this.collisionSensor != null)
			{
				this.collisionSensor.SensorTriggerEnter += this.TriggerEnter2D;
				this.collisionSensor.SensorTriggerExit += this.TriggerExit2D;
			}
		}

		private void Start()
		{
			PoolManager.Instance.CreatePool(this.nextPlatformParticles, this.target.Length);
		}

		private void OnDestroy()
		{
			Core.Events.OnFlagChanged -= this.OnFlagChanged;
			if (this.collisionSensor != null)
			{
				this.collisionSensor.SensorTriggerEnter -= this.TriggerEnter2D;
				this.collisionSensor.SensorTriggerExit -= this.TriggerExit2D;
			}
		}

		private void OnCollisionEnter2D(Collision2D other)
		{
			Debug.Log(other.collider.tag);
		}

		private void TriggerEnter2D(Collider2D other)
		{
			if (other.CompareTag("PenitentFeet"))
			{
				this.Show(this.activationDelay);
				this.ShowNextPlatform(this.activationDelay);
			}
		}

		private void TriggerExit2D(Collider2D other)
		{
			if (other.CompareTag("PenitentFeet"))
			{
				this.Hide(this.deactivationDelay);
				this.HideNextPlatform(this.deactivationDelay);
			}
		}

		private void OnFlagChanged(string flag, bool flagActive)
		{
			if (flag == "REVEAL_FAITH_PLATFORMS" && flagActive && this.firstPlatform)
			{
				this.ShowAction();
			}
			if (flag == "REVEAL_FAITH_PLATFORMS" && !flagActive && this.showing)
			{
				this.HideAction();
			}
		}

		private void Show(float delay)
		{
			base.CancelInvoke("HideAction");
			base.Invoke("ShowAction", delay);
		}

		public void Hide(float delay)
		{
			if (this.firstPlatform)
			{
				return;
			}
			base.CancelInvoke("ShowAction");
			base.Invoke("HideAction", delay);
		}

		private void ShowAction()
		{
			if (this.spriteRenderer.isVisible && !this.showing)
			{
				Core.Audio.PlaySfx(this.appearSound, 0f);
			}
			this.animator.SetBool("ENABLED", true);
			this.showing = true;
			ShortcutExtensions43.DOColor(this.spriteRenderer, this.activeColor, this.swichTime);
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
			ShortcutExtensions43.DOColor(this.spriteRenderer, this.disabledColor, this.swichTime);
			this.collision.enabled = false;
		}

		private void ShowNextPlatform(float delay)
		{
			for (int i = 0; i < this.target.Length; i++)
			{
				if (!(this.target[i] == null))
				{
					FaithPlatform component = this.target[i].GetComponent<FaithPlatform>();
					if (component != null)
					{
						this.EffectToNextPlatform(component);
						component.Show(delay);
					}
				}
			}
		}

		private void EffectToNextPlatform(FaithPlatform p)
		{
			Vector3 position = base.transform.position;
			Vector3 position2 = p.transform.position;
			GameObject gameObject = PoolManager.Instance.ReuseObject(this.nextPlatformParticles, position, Quaternion.identity, false, 1).GameObject;
			TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(gameObject.transform, position2, 0.2f, false), 9);
		}

		private void HideNextPlatform(float delay)
		{
			for (int i = 0; i < this.target.Length; i++)
			{
				if (!(this.target[i] == null))
				{
					FaithPlatform component = this.target[i].GetComponent<FaithPlatform>();
					if (component != null)
					{
						component.Hide(delay);
					}
				}
			}
		}

		public void Use()
		{
			base.Invoke("Show", this.activationDelay);
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
		[Range(0f, 2f)]
		private float swichTime = 1f;

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

		private float activationDelay;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		[Range(0f, 10f)]
		private float deactivationDelay;

		private bool penitentTouching;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		private Color activeColor;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		private Color disabledColor;

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
	}
}
