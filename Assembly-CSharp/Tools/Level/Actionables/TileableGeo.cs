using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using Framework.Managers;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Tools.Level.Actionables
{
	public class TileableGeo : MonoBehaviour, IActionable
	{
		private void Awake()
		{
			this.SetSize(0f);
			this.collider.enabled = false;
		}

		private void OnCollisionEnter2D(Collision2D other)
		{
			Debug.Log(other.collider.tag);
		}

		private void Update()
		{
			if (this.affectedByRelic)
			{
				this.CheckDistanceToPenitent();
			}
		}

		private void CheckDistanceToPenitent()
		{
			if (Core.Logic.Penitent != null)
			{
				string idRelic = "RE10";
				Vector2 a = Core.Logic.Penitent.transform.position;
				float num = Vector2.Distance(a, base.transform.position);
				if (num < this.relicEffectiveRadius && this.currentState == TileableGeo.TILEABLE_GEO_STATES.HIDDEN && Core.InventoryManager.IsRelicEquipped(idRelic))
				{
					this.Show();
				}
				else if (this.currentState == TileableGeo.TILEABLE_GEO_STATES.SHOWN && (!Core.InventoryManager.IsRelicEquipped(idRelic) || num > this.relicEffectiveRadius))
				{
					this.Hide();
				}
			}
		}

		private void SetSize(float newSize)
		{
			this.currentSize = newSize;
			this.spriteRenderer.size = new Vector2(this.spriteRenderer.size.x, newSize);
		}

		private void SetLayer(TileableGeo.TILEABLE_GEO_DIRECTIONS dir)
		{
			TileableGeo.LayerPerGeoDirection layerPerGeoDirection = this.layersPerGeoDirection.Find((TileableGeo.LayerPerGeoDirection x) => x.direction == dir);
			base.GetComponentInChildren<Collider2D>().gameObject.layer = LayerMask.NameToLayer(layerPerGeoDirection.layer);
		}

		private void SetDirection(TileableGeo.TILEABLE_GEO_DIRECTIONS dir)
		{
			float z = 0f;
			switch (dir)
			{
			case TileableGeo.TILEABLE_GEO_DIRECTIONS.UP:
				z = 180f;
				break;
			case TileableGeo.TILEABLE_GEO_DIRECTIONS.RIGHT:
				z = 90f;
				break;
			case TileableGeo.TILEABLE_GEO_DIRECTIONS.DOWN:
				z = 0f;
				break;
			case TileableGeo.TILEABLE_GEO_DIRECTIONS.LEFT:
				z = -90f;
				break;
			}
			this.rotationParent.transform.rotation = Quaternion.Euler(0f, 0f, z);
		}

		[BoxGroup("Design Settings", true, false, 0)]
		[Button("Apply direction and size", ButtonSizes.Large)]
		public void SetGrownState()
		{
			this.SetDirection(this.geoDirection);
			this.SetSize((float)this.maxSize);
			this.SetLayer(this.geoDirection);
		}

		private IEnumerator ChangeSizeCoroutine(float duration, float targetSize, AnimationCurve curve, Action callback)
		{
			float counter = 0f;
			float originSize = this.currentSize;
			while (counter < duration)
			{
				float normalizedValue = counter / duration;
				float curveValue = curve.Evaluate(normalizedValue);
				float newSize = Mathf.Lerp(originSize, targetSize, curveValue);
				this.SetSize(newSize);
				counter += Time.deltaTime;
				yield return null;
			}
			this.SetSize(targetSize);
			callback();
			yield break;
		}

		private void Show()
		{
			this.rootCoreAnimator.SetBool("ACTIVE", true);
			base.StartCoroutine(this.ChangeSizeCoroutine(this.secondsToGrow, (float)this.maxSize, this.growCurve, new Action(this.OnGrowFinished)));
			this.collider.enabled = true;
			this.currentState = TileableGeo.TILEABLE_GEO_STATES.SHOWING;
			this.TriggerGrow();
			if (this.rootCoreAnimator.GetComponent<SpriteRenderer>().isVisible)
			{
				this.PlayGrowRootsFx();
			}
		}

		private void OnGrowFinished()
		{
			this.rootCoreAnimator.SetBool("ACTIVE", false);
			this.currentState = TileableGeo.TILEABLE_GEO_STATES.SHOWN;
			this.StopGrowRootsFx();
		}

		public void Hide()
		{
			this.rootCoreAnimator.SetBool("ACTIVE", true);
			base.StartCoroutine(this.ChangeSizeCoroutine(this.secondsToGrow, 0f, this.growCurve, new Action(this.OnShortenFinished)));
			this.currentState = TileableGeo.TILEABLE_GEO_STATES.HIDING;
			this.TriggerShrink();
			if (this.rootCoreAnimator.GetComponent<SpriteRenderer>().isVisible)
			{
				this.PlayGrowRootsFx();
			}
		}

		private void OnShortenFinished()
		{
			this.collider.enabled = false;
			this.rootCoreAnimator.SetBool("ACTIVE", false);
			this.currentState = TileableGeo.TILEABLE_GEO_STATES.HIDDEN;
			this.StopGrowRootsFx();
		}

		public void Use()
		{
			if (this.currentState == TileableGeo.TILEABLE_GEO_STATES.HIDDEN)
			{
				this.Show();
			}
			else if (this.currentState == TileableGeo.TILEABLE_GEO_STATES.SHOWN)
			{
				this.Hide();
			}
		}

		public bool Locked { get; set; }

		private void Start()
		{
			this.CreateBeamBodies();
			this.ChangeAnimationSpeed();
			this.SetLayer(this.geoDirection);
		}

		private void ChangeAnimationSpeed()
		{
			float num = 0.5f;
			this.timePerTile = this.secondsToGrow / (float)this.bodyParts.Count;
			this.animatorSpeedPerTile = num / this.timePerTile;
			this.animationTimePerTile = num / this.animatorSpeedPerTile;
			foreach (GameObject gameObject in this.bodyParts)
			{
				gameObject.GetComponent<Animator>().speed = this.animatorSpeedPerTile;
			}
			this.secondsBetweenTileAnim = this.animationTimePerTile;
		}

		private void CreateBeamBodies()
		{
			this.bodyParts = new List<GameObject>();
			int num = this.maxSize / this.distanceBetweenBodySprites;
			for (int i = 0; i < num; i++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.bodyPartPrefab, this.bodyPartsParent);
				this.bodyParts.Add(gameObject);
				gameObject.transform.localPosition = new Vector3((float)(i * this.distanceBetweenBodySprites), 0f);
				gameObject.GetComponentInChildren<SpriteRenderer>().flipY = this.flipGraphic;
			}
			this.bodyParts[this.bodyParts.Count - 1].GetComponentInChildren<Animator>().SetBool("LAST_TILE", true);
		}

		private IEnumerator DelayedShrinkCoroutine(float delayBetweenTiles)
		{
			for (int i = this.bodyParts.Count - 1; i >= 0; i--)
			{
				this.bodyParts[i].GetComponentInChildren<Animator>().SetBool(this.growAnimatorBool, false);
				yield return new WaitForSeconds(delayBetweenTiles);
			}
			yield break;
		}

		private IEnumerator DelayedGrowCoroutine(float delayBetweenTiles)
		{
			for (int i = 0; i < this.bodyParts.Count; i++)
			{
				this.bodyParts[i].GetComponentInChildren<Animator>().SetBool(this.growAnimatorBool, true);
				yield return new WaitForSeconds(delayBetweenTiles);
			}
			this.rootCoreAnimator.SetBool("ACTIVE", false);
			yield break;
		}

		public void TriggerGrow()
		{
			base.StartCoroutine(this.DelayedGrowCoroutine(this.secondsBetweenTileAnim));
		}

		public void TriggerShrink()
		{
			base.StartCoroutine(this.DelayedShrinkCoroutine(this.secondsBetweenTileAnim));
		}

		public void PlayGrowRootsFx()
		{
			this.StopGrowRootsFx();
			if (this.grownAnimationAudioInstance.isValid() || string.IsNullOrEmpty(this.growAnimationFx))
			{
				return;
			}
			this.grownAnimationAudioInstance = Core.Audio.CreateEvent(this.growAnimationFx, default(Vector3));
			this.grownAnimationAudioInstance.start();
		}

		public void StopGrowRootsFx()
		{
			if (!this.grownAnimationAudioInstance.isValid())
			{
				return;
			}
			this.grownAnimationAudioInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
			this.grownAnimationAudioInstance.release();
			this.grownAnimationAudioInstance = default(EventInstance);
		}

		private void OnDestroy()
		{
			this.StopGrowRootsFx();
		}

		[SerializeField]
		[FoldoutGroup("Attached References", 0)]
		private SpriteRenderer spriteRenderer;

		[SerializeField]
		[FoldoutGroup("Attached References", 0)]
		private Collider2D collider;

		[SerializeField]
		[FoldoutGroup("Attached References", 0)]
		private Animator animator;

		[SerializeField]
		[FoldoutGroup("Attached References", 0)]
		private Transform rotationParent;

		[SerializeField]
		[FoldoutGroup("Attached References", 0)]
		private Animator rootCoreAnimator;

		[SerializeField]
		[BoxGroup("Relic Settings", true, false, 0)]
		private bool affectedByRelic = true;

		[ShowIf("affectedByRelic", true)]
		[SerializeField]
		[BoxGroup("Relic Settings", true, false, 0)]
		private float relicEffectiveRadius = 9f;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		private TileableGeo.TILEABLE_GEO_DIRECTIONS geoDirection;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		private int maxSize;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		private float secondsToGrow;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		private AnimationCurve growCurve;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		public List<TileableGeo.LayerPerGeoDirection> layersPerGeoDirection;

		[FoldoutGroup("Graphic Settings", false, 0)]
		public bool flipGraphic;

		[FoldoutGroup("Graphic Settings", false, 0)]
		public int distanceBetweenBodySprites = 2;

		[FoldoutGroup("Graphic Settings", false, 0)]
		public List<GameObject> bodyParts;

		[FoldoutGroup("Graphic Settings", false, 0)]
		public GameObject bodyPartPrefab;

		[FoldoutGroup("Graphic Settings", false, 0)]
		public Transform bodyPartsParent;

		[FoldoutGroup("Graphic Settings", false, 0)]
		public string growAnimatorBool = "GROW";

		[FoldoutGroup("Audio", false, 0)]
		[EventRef]
		public string growAnimationFx;

		[SerializeField]
		[FoldoutGroup("Debug", 0)]
		public TileableGeo.TILEABLE_GEO_STATES currentState;

		[SerializeField]
		[FoldoutGroup("Debug", 0)]
		public float currentSize = 5f;

		private float secondsBetweenTileAnim = 0.5f;

		private float timePerTile;

		private float animationTimePerTile;

		private float animatorSpeedPerTile;

		private Coroutine currentCoroutine;

		private EventInstance grownAnimationAudioInstance;

		[Serializable]
		public struct LayerPerGeoDirection
		{
			public TileableGeo.TILEABLE_GEO_DIRECTIONS direction;

			public string layer;
		}

		public enum TILEABLE_GEO_STATES
		{
			HIDDEN,
			SHOWING,
			SHOWN,
			HIDING
		}

		public enum TILEABLE_GEO_DIRECTIONS
		{
			UP,
			RIGHT,
			DOWN,
			LEFT
		}
	}
}
