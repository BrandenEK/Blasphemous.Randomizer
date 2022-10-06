using System;
using System.Collections;
using DG.Tweening;
using FMOD.Studio;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.Audio;
using Tools.Level.Layout;
using UnityEngine;

namespace Tools.Level.Actionables
{
	[RequireComponent(typeof(Collider2D))]
	[RequireComponent(typeof(LayoutElement))]
	public class ElusivePlatform : MonoBehaviour, IActionable, INoSafePosition
	{
		private void Awake()
		{
			this._collider = base.GetComponent<Collider2D>();
			this._layoutElement = base.GetComponent<LayoutElement>();
			this._animator = base.GetComponentInChildren<Animator>();
			this._spriteRenderer = base.GetComponentInChildren<SpriteRenderer>();
		}

		private void Start()
		{
			this._grabColliders = this.GetGlimbLedes();
			this.SetFxAudio();
		}

		private void Update()
		{
			if (this._targetIsOnPlatform)
			{
				if (!this._isTransitioning)
				{
					this._isTransitioning = true;
					base.StartCoroutine((!(this._animator != null)) ? this.DisappearLayout() : this.DisappearDeco());
				}
			}
			else if (this._disappear)
			{
				this._disappear = !this._disappear;
				this._isTransitioning = true;
				base.StartCoroutine((!(this._animator != null)) ? this.AppearLayout() : this.AppearDeco());
			}
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			if ((this.TargetLayer.value & 1 << other.gameObject.layer) <= 0)
			{
				return;
			}
			this._entityOnTop = other.GetComponentInParent<Entity>();
		}

		private void OnTriggerStay2D(Collider2D other)
		{
			if ((this.TargetLayer.value & 1 << other.gameObject.layer) <= 0)
			{
				return;
			}
			if (other.bounds.min.y + 0.1f >= this._collider.bounds.max.y && this._entityOnTop.Status.IsGrounded && !this._entityOnTop.Status.Dead && !this._targetIsOnPlatform)
			{
				this._targetIsOnPlatform = true;
			}
		}

		private void OnTriggerExit2D(Collider2D other)
		{
			if ((this.TargetLayer.value & 1 << other.gameObject.layer) <= 0)
			{
				return;
			}
			if (this._targetIsOnPlatform)
			{
				this._targetIsOnPlatform = !this._targetIsOnPlatform;
			}
		}

		private IEnumerator DisappearLayout()
		{
			float remainTime = this.RemainTime;
			Color c = this._layoutElement.SpriteRenderer.material.color;
			Color newColor = new Color(c.r, c.g, c.b);
			while (remainTime >= 0f)
			{
				remainTime -= Time.deltaTime;
				float percentageRemain = Mathf.Clamp01(remainTime / this.RemainTime);
				newColor.a = percentageRemain;
				this._layoutElement.SpriteRenderer.material.color = newColor;
				yield return new WaitForEndOfFrame();
			}
			this._collider.enabled = false;
			this._targetIsOnPlatform = false;
			this.EnableGrabColliders(false);
			this._isTransitioning = false;
			this._disappear = true;
			yield break;
		}

		private IEnumerator AppearLayout()
		{
			yield return new WaitForSeconds(this.RecoverTime);
			Color c = this._layoutElement.SpriteRenderer.material.color;
			Color newColor = new Color(c.r, c.g, c.b);
			newColor.a = 1f;
			this._layoutElement.SpriteRenderer.material.color = newColor;
			this._collider.enabled = true;
			this.EnableGrabColliders(true);
			this._isTransitioning = false;
			yield break;
		}

		private IEnumerator DisappearDeco()
		{
			float remainTime = this.RemainTime;
			this._animator.Play(this._stepOverAnim);
			this.PlayFxAudio(this._materialFxPrefix + "STEP");
			while (remainTime >= 0f)
			{
				remainTime -= Time.deltaTime;
				yield return new WaitForEndOfFrame();
			}
			this._animator.Play(this._collapseAnim);
			this._collider.enabled = false;
			this._targetIsOnPlatform = false;
			this.EnableGrabColliders(false);
			this._isTransitioning = false;
			this._disappear = true;
			if (this._spriteRenderer.isVisible)
			{
				this.PlayFxAudio(this._materialFxPrefix + "COLLAPSE");
			}
			yield break;
		}

		private IEnumerator AppearDeco()
		{
			yield return new WaitForSeconds(this.RecoverTime);
			this.SpriteFadeIn();
			this._animator.Play(this._appearAnim);
			this._collider.enabled = true;
			this.EnableGrabColliders(true);
			this._isTransitioning = false;
			if (this._spriteRenderer.isVisible)
			{
				this.PlayFxAudio(this._materialFxPrefix + "RESPAWN");
			}
			yield break;
		}

		public void SpriteFadeOut()
		{
			if (this._spriteRenderer == null)
			{
				return;
			}
			ShortcutExtensions43.DOFade(this._spriteRenderer, 0f, 0.1f);
		}

		public void SpriteFadeIn()
		{
			if (this._spriteRenderer == null)
			{
				return;
			}
			ShortcutExtensions43.DOFade(this._spriteRenderer, 1f, 0.1f);
		}

		public void Use()
		{
			base.StartCoroutine(this.DisappearLayout());
		}

		private Collider2D[] GetGlimbLedes()
		{
			return Physics2D.OverlapAreaAll(this._collider.bounds.min, this._collider.bounds.max, this.GrabTriggerLayer);
		}

		private void EnableGrabColliders(bool enable = true)
		{
			foreach (Collider2D collider2D in this._grabColliders)
			{
				collider2D.enabled = enable;
			}
		}

		public bool Locked { get; set; }

		private void SetFxAudio()
		{
			if (base.tag.Equals("Material:Stone"))
			{
				this._materialFxPrefix = "BP_STONE_";
			}
			else if (base.tag.Equals("Material:Wood"))
			{
				this._materialFxPrefix = "BP_WOOD_";
			}
			else if (base.tag.Equals("Material:Glass"))
			{
				this._materialFxPrefix = "BP_GLASS_";
			}
			else if (base.tag.Equals("Material:Demake"))
			{
				this._materialFxPrefix = "BP_DEMAKE_";
			}
		}

		private void PlayFxAudio(string idEvent)
		{
			this._soundEventInstance = Core.Audio.CreateCatalogEvent(idEvent, default(Vector3));
			this._soundEventInstance.setCallback(EntityAudio.SetPanning(this._soundEventInstance, base.transform.position), 1);
			this._soundEventInstance.start();
			this._soundEventInstance.release();
		}

		private readonly int _stepOverAnim = Animator.StringToHash("StepOver");

		private readonly int _collapseAnim = Animator.StringToHash("Collapse");

		private readonly int _appearAnim = Animator.StringToHash("Appear");

		private string _materialFxPrefix;

		public LayerMask TargetLayer;

		public LayerMask GrabTriggerLayer;

		public float RemainTime = 2f;

		public float RecoverTime = 2f;

		private bool _targetIsOnPlatform;

		private bool _platformIsActive;

		private bool _isTransitioning;

		private bool _disappear;

		private Collider2D _collider;

		private Collider2D[] _grabColliders;

		private Animator _animator;

		private SpriteRenderer _spriteRenderer;

		private LayoutElement _layoutElement;

		private Entity _entityOnTop;

		private EventInstance _soundEventInstance;
	}
}
