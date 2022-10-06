using System;
using System.Collections;
using FMODUnity;
using Framework.Managers;
using Framework.Pooling;
using Sirenix.OdinInspector;
using UnityEngine;

public class TileableBeamLauncher : PoolObject
{
	private void Start()
	{
		this.results = new RaycastHit2D[1];
	}

	public float GetDistance()
	{
		if (Physics2D.RaycastNonAlloc(base.transform.position, base.transform.right, this.results, this.maxRange, this.beamCollisionMask) > 0)
		{
			Vector2 point = this.results[0].point;
			Debug.DrawLine(base.transform.position, point, Color.magenta, 6f);
			return Vector2.Distance(point, base.transform.position);
		}
		return this.maxRange;
	}

	public override void OnObjectReuse()
	{
		base.OnObjectReuse();
		this.results = new RaycastHit2D[1];
	}

	private void Update()
	{
		this.LaunchBeam();
	}

	public void ActivateEndAnimation(bool active, bool applyChanges = false)
	{
		this.displayEndAnimation = active;
		if (applyChanges && this.endAnimator)
		{
			this.endAnimator.SetBool(TileableBeamLauncher.Active, this.displayEndAnimation);
		}
	}

	[Button(0)]
	public void TriggerBeamBodyAnim()
	{
		Animator componentInChildren = this.bodySprite.GetComponentInChildren<Animator>();
		if (componentInChildren)
		{
			componentInChildren.SetTrigger(TileableBeamLauncher.Beam);
		}
	}

	public void ActivateDelayedBeam(float delay, bool warningAnimation)
	{
		if (warningAnimation)
		{
			this.bodyAnimator.SetTrigger(TileableBeamLauncher.Warning);
		}
		if (this.hasSfxOnActivation)
		{
			Core.Audio.PlayOneShot(this.sfxOnActivation, default(Vector3));
		}
		this.displayEndAnimation = false;
		base.StartCoroutine(this.ActivateBeamCoroutine(delay));
	}

	private IEnumerator ActivateBeamCoroutine(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		this.ActivateBeamAnimation(true);
		yield break;
	}

	public void ClearAll()
	{
		base.StopAllCoroutines();
	}

	public void ActivateBeamAnimation(bool active)
	{
		this.displayEndAnimation = active;
		this.bodyAnimator.SetBool(TileableBeamLauncher.Active, active);
	}

	private void LaunchBeam()
	{
		Vector2 vector;
		if (Physics2D.RaycastNonAlloc(base.transform.position, base.transform.right, this.results, this.maxRange, this.beamCollisionMask) > 0)
		{
			vector = this.results[0].point;
			if (this.endSprite)
			{
				this.endSprite.position = vector;
				this.endSprite.up = this.results[0].normal;
			}
			if (this.endSprite2)
			{
				this.endSprite2.position = vector;
				this.endSprite2.up = this.results[0].normal;
			}
			if (this.displayEndAnimation)
			{
				if (this.endAnimator)
				{
					this.endAnimator.SetBool(TileableBeamLauncher.Active, true);
				}
				this.endSprite.gameObject.SetActive(true);
				if (this.endSprite2)
				{
					this.endSprite2.gameObject.SetActive(true);
				}
			}
			else if (this.endSprite)
			{
				this.endSprite.gameObject.SetActive(false);
				if (this.endSprite2)
				{
					this.endSprite2.gameObject.SetActive(false);
				}
			}
			if (this.growSprite)
			{
				this.growSprite.gameObject.SetActive(false);
			}
			GizmoExtensions.DrawDebugCross(base.transform.position, Color.green, 0.1f);
			GizmoExtensions.DrawDebugCross(vector, Color.green, 0.1f);
		}
		else
		{
			if (this.displayEndAnimation)
			{
				if (this.endAnimator)
				{
					this.endAnimator.SetBool(TileableBeamLauncher.Active, false);
				}
			}
			else if (this.endSprite)
			{
				this.endSprite.gameObject.SetActive(false);
				if (this.endSprite2)
				{
					this.endSprite2.gameObject.SetActive(false);
				}
			}
			vector = base.transform.position + base.transform.right * this.maxRange;
			if (this.growSprite)
			{
				this.growSprite.gameObject.SetActive(true);
				this.growSprite.position = vector;
			}
			if (this.permanentEndSprite)
			{
				this.permanentEndSprite.transform.position = vector;
			}
			Color c = Color.HSVToRGB(Random.Range(0f, 1f), 1f, 1f);
			GizmoExtensions.DrawDebugCross(base.transform.position, c, 0.1f);
			GizmoExtensions.DrawDebugCross(vector, c, 0.1f);
		}
		this.DrawBeam(base.transform.position, vector);
	}

	private void DrawBeam(Vector2 origin, Vector2 end)
	{
		float magnitude = (end - origin).magnitude;
		if (this.stretchWidth)
		{
			this.bodySprite.size = new Vector2(magnitude, this.bodySprite.size.y);
			if (this.stretchCollider)
			{
				this.collider.size = new Vector2(magnitude, this.collider.size.y);
			}
		}
		else
		{
			this.bodySprite.size = new Vector2(this.bodySprite.size.x, magnitude);
			if (this.stretchCollider)
			{
				this.collider.size = new Vector2(this.collider.size.x, magnitude);
			}
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawLine(base.transform.position, base.transform.position + base.transform.right * this.maxRange);
	}

	public LayerMask beamCollisionMask;

	public SpriteRenderer bodySprite;

	public Transform endSprite;

	public Transform endSprite2;

	public Transform permanentEndSprite;

	public Transform growSprite;

	public Animator endAnimator;

	public Animator bodyAnimator;

	public float maxRange;

	public bool stretchWidth = true;

	public bool stretchCollider;

	[ShowIf("stretchCollider", true)]
	public BoxCollider2D collider;

	public bool displayEndAnimation = true;

	public bool hasSfxOnActivation;

	[ShowIf("hasSfxOnActivation", true)]
	[EventRef]
	public string sfxOnActivation;

	private static readonly int Active = Animator.StringToHash("ACTIVE");

	private static readonly int Warning = Animator.StringToHash("WARNING");

	private static readonly int Beam = Animator.StringToHash("BEAM");

	private RaycastHit2D[] results;
}
