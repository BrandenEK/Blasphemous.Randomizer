using System;
using Gameplay.GameControllers.Entities;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
	private void Start()
	{
		this.attachedEnemy = base.GetComponentInParent<Enemy>();
	}

	private void Update()
	{
		float x = Mathf.Lerp(this.healthBar.transform.localScale.x, this.attachedEnemy.Stats.Life.MissingRatio, 0.5f);
		float x2 = Mathf.Lerp(this.missingHealthBar.transform.localScale.x, this.attachedEnemy.Stats.Life.MissingRatio, 0.1f);
		this.healthBar.transform.localScale = new Vector2(x, this.healthBar.transform.localScale.y);
		this.missingHealthBar.transform.localScale = new Vector2(x2, this.missingHealthBar.transform.localScale.y);
	}

	public SpriteRenderer healthBar;

	public SpriteRenderer missingHealthBar;

	public Enemy attachedEnemy;
}
