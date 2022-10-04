using System;
using Framework.Managers;
using Gameplay.GameControllers.Environment.AreaEffects;
using UnityEngine;

public class VenomFog : MonoBehaviour
{
	private void Awake()
	{
		this.spr = base.GetComponent<SpriteRenderer>();
		this.poisonArea = base.GetComponentInParent<PoisonAreaEffect>();
	}

	private void Update()
	{
		if (Core.Logic.Penitent == null)
		{
			return;
		}
		if (this.poisonArea != null && this.poisonArea.IsDisabled)
		{
			float num = Vector2.Distance(Core.Logic.Penitent.transform.position, base.transform.position);
			if (num < this.maxReactDistance)
			{
				Color color = Color.Lerp(this.venomColor, this.normalColor, 1f - num / this.maxReactDistance);
				this.spr.color = color;
			}
		}
	}

	public Color normalColor;

	public Color venomColor;

	public SpriteRenderer spr;

	public float maxReactDistance;

	public PoisonAreaEffect poisonArea;
}
